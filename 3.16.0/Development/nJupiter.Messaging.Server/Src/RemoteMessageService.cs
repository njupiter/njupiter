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
using System.Runtime.CompilerServices;
using System.Reflection;

// log4net
using log4net;

namespace nJupiter.Messaging.Server {

	public class RemoteMessageService : MessageService, IDisposable {
		#region Instance Members
		internal MessageConsumerMap ConsumerMap;
		internal MessageQueue MessageQueue;
		private long ticks;
		private readonly ILog logger = LogManager.GetLogger(Assembly.GetExecutingAssembly().GetType());
		#endregion

		#region Constructor
		public RemoteMessageService() {
			this.MessageQueue = new MessageQueue(this);
			this.ticks = DateTime.Now.Ticks;
			this.ConsumerMap = MessageConsumerMap.Deserialize();
			this.MessageQueue.LoadQueue();
		}
		#endregion

		#region Methods
		[MethodImpl(MethodImplOptions.Synchronized)]
		public override void Publish(Message message) {
			if(message == null) {
				throw new ArgumentNullException("message");
			}
			message.Id = GetUniqueId();
			lock(this.MessageQueue) {
				this.MessageQueue.AddMessage(message);
			}
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		public override void Register(MessageConsumer messageConsumer) {
			if(messageConsumer == null) {
				throw new ArgumentNullException("messageConsumer");
			}
			lock(this.MessageQueue) {
				this.logger.Info("AddMessageConsumer");
				this.logger.Info("MessageConsumerDest: " + messageConsumer.Destination);
				this.ConsumerMap.AddMessageConsumer(messageConsumer);
			}
		}
		public override void GetMessageConsumers() {
			throw new NotImplementedException();
		}
		public override void GetMessageConsumers(MessageDestination messageDestination) {
			throw new NotImplementedException();
		}
		public override void GetMessageDestinations() {
			throw new NotImplementedException();
		}
		public override void RemoveMessageDestination(MessageDestination messageDestination) {
			throw new NotImplementedException();
		}
		public override void RemoveMessageConsumer(MessageConsumer messageConsumer) {
			throw new NotImplementedException();
		}
		public override object InitializeLifetimeService() {
			// Sets the time to live for this object. When this expires, the object will have to
			// be re-marshalled.
			// To make your object live for the life of the service process, return null here
			return null;
		}
		#endregion

		#region Util Methods
		/// <summary>
		/// Generate an unique is that is used for id and name on event-files
		/// </summary>
		/// <returns>Unique number</returns>
		[MethodImpl(MethodImplOptions.Synchronized)]
		private long GetUniqueId() {
			return this.ticks++;
		}
		#endregion

		#region IDisposable Members
		public void Dispose() {
			this.MessageQueue.ClearQueue();
			this.ConsumerMap.Serialize();
			this.ConsumerMap = null;
		}
		#endregion
	}
}
