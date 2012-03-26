#region Copyright & License
/*
	Copyright (c) 2005-2010 nJupiter

	Permission is hereby granted, free of charge, to any person obtaining a copy
	of this software and associated documentation files (the "Software"), to deal
	in the Software without restriction, including without limitation the rights
	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	copies of the Software, and to permit persons to whom the Software is
	furnished to do so, subject to the following conditions:

	The above copyright notice and this permission notice shall be included in
	all copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
	THE SOFTWARE.
*/
#endregion

using System;
using System.Collections;
using System.Reflection;
using System.Timers;
using System.Xml;
using System.IO;
using System.Globalization;

// log4net
using log4net;

// njupiter
using nJupiter.Configuration;
using nJupiter.Messaging.Adapters.WebService;

namespace nJupiter.Messaging.Server {

	/// <summary>
	/// The message queue used by the service holding message objects
	/// </summary>
	internal sealed class MessageQueue {
		
		#region Constants
		private	const string MessageFileExt					= ".xml";
		private const string MessageServicesSection			= "messageServices";
		private const string MessageServiceSection			= MessageServicesSection + "/messageService";
		private const string MessageServiceSectionFormat	= MessageServiceSection + "[@value='{0}']";	
		private const string SettingsSectionFormat			= MessageServiceSectionFormat + "/settings";
		#endregion

		#region Static instance members
		private		static			RemoteMessageService	messageService;
		private		static readonly ILog					logger			= LogManager.GetLogger( Assembly.GetExecutingAssembly().GetType() );
        private     static readonly string					messageDir		= ConfigHandler.GetConfig(Assembly.GetAssembly(typeof(Message))).GetConfigSection(string.Format(CultureInfo.InvariantCulture, SettingsSectionFormat, "server")).GetValue("messageDirectory");
		private		static readonly object					padlock			= new object();
		#endregion
	
		#region Private instance members
		private readonly ArrayList	messageQueue;
		private readonly Timer		timer;						
		#endregion
		
		#region Properties
		internal	ArrayList	Items	{ get { return new ArrayList(this.messageQueue);}}
		#endregion

		#region Constructors
		internal MessageQueue(RemoteMessageService messageService) {
			MessageQueue.messageService = messageService;
			this.messageQueue = new ArrayList();
			this.timer = new Timer();
			this.timer.Elapsed += this.PopMessageFromQueue;
		}
		#endregion
		
		#region Internal methods
		internal void AddMessage(Message messageItem) {
			SaveMessage(messageItem);
			int i;
			bool inserted = false;
			lock(this.messageQueue.SyncRoot){
				for(i = 0; i < this.messageQueue.Count; i++){
					Message queuedmessage = (Message) this.messageQueue[i];
					if(messageItem.StartPublish < queuedmessage.StartPublish){
						this.messageQueue.Insert(i, messageItem);
						inserted = true;
						break;
					}
				}
				if(!inserted)
					this.messageQueue.Add(messageItem);
			}
			if(i == 0 || !this.timer.Enabled)
				this.SetTimer((Message) this.messageQueue[0]);
		}

		internal void ClearQueue(){
			this.timer.Stop();
			this.messageQueue.Clear();
		}
		
		internal void LoadQueue()
		{
			string dirName = messageDir;
			if(!Directory.Exists(dirName))	{
				Directory.CreateDirectory(dirName);
			}
			else {
				foreach(string fileName in Directory.GetFiles(dirName))	{
					if(fileName.EndsWith(MessageFileExt))	{
						Message messageItem = LoadMessage(fileName);
						lock( padlock ) {
							this.AddMessage( messageItem );
						}
					}
				}
			}
		}
		
		internal static Message LoadMessage(string fileName) {
			Message messageItem;
			using (Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read)) {
				XmlDocument xml = new XmlDocument();
				xml.Load(stream);
				messageItem = Message.Deserialize(xml);																
			}
			return messageItem;
		}
		#endregion

		#region Private methods
		private void SetTimer(Message messageItem)	{
			lock(this.timer) {
				this.timer.Stop();
				const int hourInMilliseconds = 60 * 60 * 1000;
				long interval = (messageItem.StartPublish.Ticks - DateTime.Now.Ticks) / 10000; // To miliseconds
				interval = (interval > hourInMilliseconds ? hourInMilliseconds : interval); // Set interval to be max 1 hour, basicly because of inaccuracy in the timer function and because an interval can not be longer than approximately 50 days
				this.timer.Interval = (interval > 0 ? interval : 1);
				this.timer.Start();
			}
		}
		
		private void PopMessageFromQueue(object sender, ElapsedEventArgs eventArgs){
			lock(this.messageQueue.SyncRoot){
				this.timer.Stop();
				Message messageItem = (Message) this.messageQueue[0];
				if(messageItem.StartPublish > DateTime.Now){
					this.SetTimer(messageItem); // Reset timer if nearest event hasn't occured yet.
				}else{
					SendMessage(messageItem);
					this.messageQueue.Remove(messageItem);
					if(messageItem.StartPublish > DateTime.Now)
						this.AddMessage(messageItem);
					else if(this.messageQueue.Count > 0)
						this.SetTimer((Message) this.messageQueue[0]);
				}
			}
		}
		
		private static void SaveMessage(Message message){
			XmlDocument xml = message.Serialize();
			using (Stream stream = new FileStream(MessageFileName(message) , FileMode.Create, FileAccess.Write, FileShare.None)){			
				logger.Info(string.Format("Stream: {0}", MessageFileName(message)) );
				xml.Save(stream);
			}
		}
		
		private static void DeleteMessage(Message messageItem){
			logger.Info("Delete");
			if(File.Exists(MessageFileName(messageItem)))
				File.Delete(MessageFileName(messageItem));
		}
		
		private static string MessageFileName(Message messageItem){
			return messageDir + messageItem.Id +  MessageFileExt;
		}
		
		private static void SendMessage(Message messageItem) {		
			logger.Info(string.Format("SendMessage: {0}", messageItem.Data));
			ArrayList destinations = messageService.consumerMap.GetMessageConsumers( messageItem.Destination );
			if(destinations != null){
				foreach(MessageConsumer msgConsumer in destinations){
					logger.Info(string.Format("SendMessage-Notify MessageConsumer: {0} send to {1}", msgConsumer, msgConsumer.NotificationUrl));
					try {
						WebServiceProxy eventReceiver = new WebServiceProxy(new Uri(msgConsumer.NotificationUrl));
						logger.Info(string.Format("WebServiceProxy.NotificationUrl: {0}", eventReceiver.Url));
						eventReceiver.BeginNotify(messageItem , NotifyCompleted, eventReceiver);
						logger.Info(string.Format("SendMessage-Notify message has been sent: to subs of: {0}", msgConsumer.Destination));
					}catch (Exception e) {
						logger.Error(string.Format("Send message error: {0}", e));
						throw; //TODO: Do not catch general exceptions
					}
				}
				if(messageItem.Interval > 0 && messageItem.StopPublish >= DateTime.Now){
					long i		= 1000 * 10 * 1000 * 60 * messageItem.Interval;	// mInterval in ticks
					long now	= DateTime.Now.Ticks;							// Has to be set to get the same value every time we use it
					long pt		= messageItem.StartPublish.Ticks;						// Pushtime in ticks

					// Set pushtime to the next approaching interval date
					messageItem.StartPublish = new DateTime(now + pt%i - now%i + (now >= now + pt%i - now%i ? i : 0));
					SaveMessage(messageItem); // Save the changes in pushtime to disk
				}else {
					DeleteMessage(messageItem);
				}
			}else {
				DeleteMessage(messageItem);
			}
		}
		private static void NotifyCompleted(IAsyncResult ar) {
			logger.Debug("NotifyCompleted");
			WebServiceProxy eventReceiver = (WebServiceProxy) ar.AsyncState;
			Message message = null;
			try {
				message = eventReceiver.EndNotify(ar);
				if(message == null)
					logger.Debug(string.Format("Message successfully sent to {0}.", eventReceiver.Url));
				else
					logger.Debug(string.Format("Message {0} successfully sent to {1}.", MessageFileName(message), eventReceiver.Url));
			}catch(Exception e){
				if(message != null)
					logger.Warn(string.Format("Error while trying to send event to {0}.\nEvent XML:\n{1}", eventReceiver.Url, message.Serialize().OuterXml), e);
				else
					logger.Warn(string.Format("Error while trying to send event to {0}.", eventReceiver.Url), e);
				throw; //TODO: Do not catch general exceptions
			}finally{
				eventReceiver.Dispose();
			}
		}
		#endregion
	}
}
