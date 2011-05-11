#region Copyright & License
/*
	Copyright (c) 2005-2011 nJupiter

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
using System.Globalization;

using nJupiter.Configuration;

namespace nJupiter.Messaging {

	public abstract class MessageService : MarshalByRefObject {

		#region Constants
		private const string MessageServicesSection = "messageServices";
		private const string MessageServiceSection = MessageServicesSection + "/messageService";
		private const string MessageServiceSectionFormat = MessageServiceSection + "[@value='{0}']";
		private const string SettingsSectionFormat = MessageServiceSectionFormat + "/settings";
		#endregion

		#region Static Members
		private static readonly Hashtable Services = Hashtable.Synchronized(new Hashtable());
		#endregion

		#region Members
		private IConfig settings;
		#endregion

		#region Protected Properties
		protected IConfig Settings { get { return this.settings; } }
		#endregion

		public abstract void Register(MessageConsumer messageConsumer);
		public abstract void Publish(Message message);
		public abstract void GetMessageConsumers();
		public abstract void GetMessageConsumers(MessageDestination messageDestination);
		public abstract void GetMessageDestinations();
		public abstract void RemoveMessageDestination(MessageDestination messageDestination);
		public abstract void RemoveMessageConsumer(MessageConsumer messageConsumer);

		public MessageConsumer CreateMessageConsumer(string messageDestination, Uri notificationUrl) {
			return CreateMessageConsumer(CreateMessageDestination(messageDestination), notificationUrl);
		}

		public MessageConsumer CreateMessageConsumer(MessageDestination messageDestination, Uri notificationUri) {
			if(messageDestination == null)
				throw new ArgumentNullException("messageDestination");
			if(notificationUri == null)
				throw new ArgumentNullException("notificationUri");

			MessageConsumer messageConsumer = new MessageConsumer();
			messageConsumer.NotificationUrl = notificationUri.AbsoluteUri;
			messageConsumer.Created = DateTime.Now;
			messageConsumer.Destination = messageDestination;
			return messageConsumer;
		}
		public MessageDestination CreateMessageDestination(string messageDestination) {
			if(messageDestination == null)
				throw new ArgumentNullException("messageDestination");

			return new MessageDestination(messageDestination);
		}

		public Message CreateMessage(DateTime startPublish, DateTime stopPublish, string message, MessageDestination messageDestination) {
			if(message == null)
				throw new ArgumentNullException("message");
			if(messageDestination == null)
				throw new ArgumentNullException("messageDestination");

			return new Message(startPublish, stopPublish, message, messageDestination);
		}

		public static MessageService GetInstance() {
			const string section = MessageServiceSection + "[@default='true']";
			return GetServiceFromSection(section);
		}

		public static MessageService GetInstance(string name) {
			const string sectionFormat = MessageServiceSection + "[@value='{0}']";
			return GetServiceFromSection(string.Format(CultureInfo.InvariantCulture, sectionFormat, name));
		}

		private static MessageService GetServiceFromSection(string section) {
			const string assemblyPathKey = "assemblyPath";
			const string assemblyKey = "assembly";
			const string typeKey = "type";

			IConfig config = ConfigHandlerOld.GetConfig();
			string name = config.GetValue(section);

			if(Services.ContainsKey(name))
				return (MessageService)Services[name];

			lock(Services.SyncRoot) {
				if(!Services.ContainsKey(name)) {

					string assemblyPath = config.GetValue(section, assemblyPathKey);
					string assemblyName = config.GetValue(section, assemblyKey);
					string assemblyType = config.GetValue(section, typeKey);

					object instance = CreateInstance(assemblyPath, assemblyName, assemblyType);
					MessageService messageService = (MessageService)instance;
					if(messageService == null)
						throw new ConfigurationException(string.Format("Could not load MessageService from {0} {1} {2}.", assemblyName, assemblyType, assemblyPath));

					messageService.settings = config.GetConfigSection(string.Format(CultureInfo.InvariantCulture, SettingsSectionFormat, name));

					Services.Add(name, messageService);
					return messageService;
				}
				return (MessageService)Services[name];
			}
		}

		private static object CreateInstance(string assemblyPath, string assemblyName, string typeName) {
			Assembly assembly;
			if(!string.IsNullOrEmpty(assemblyPath)) {
				assembly = Assembly.LoadFrom(assemblyPath);
			} else if(assemblyName == null || assemblyName.Length.Equals(0) ||
				Assembly.GetExecutingAssembly().GetName().Name.Equals(assemblyName)) {
				assembly = Assembly.GetExecutingAssembly();	//Load current assembly
			} else {
				assembly = Assembly.Load(assemblyName); // Late binding to an assembly on disk (current directory)
			}
			return assembly.CreateInstance(
				typeName, false,
				BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly |
				BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.ExactBinding,
				null, null, null, null);
		}
	}
}
