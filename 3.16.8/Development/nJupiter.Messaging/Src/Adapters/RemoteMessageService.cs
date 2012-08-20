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
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace nJupiter.Messaging.Adapters {

	public sealed class RemoteMessageService : MessageService {
		public override void Publish(Message message) {
			if(message == null)
				throw new ArgumentNullException("message");
			MessageService messageService = GetRemoteMessageService();
			messageService.Publish(message);
		}

		public override void Register(MessageConsumer messageConsumer) {
			if(messageConsumer == null)
				throw new ArgumentNullException("messageConsumer");
			MessageService messageService = GetRemoteMessageService();
			messageService.Register(messageConsumer);
		}

		public override void GetMessageConsumers() { throw new NotImplementedException(); }
		public override void GetMessageConsumers(MessageDestination messageDestination) { throw new NotImplementedException(); }
		public override void GetMessageDestinations() { throw new NotImplementedException(); }
		public override void RemoveMessageDestination(MessageDestination messageDestination) { throw new NotImplementedException(); }
		public override void RemoveMessageConsumer(MessageConsumer messageConsumer) { throw new NotImplementedException(); }

		private MessageService GetRemoteMessageService() {
			//TODO rewrite...
			if(ChannelServices.RegisteredChannels.Length == 0) {
				IChannel chan = new TcpChannel();
				ChannelServices.RegisterChannel(chan, false);
			}
			MessageService remoteObj = (MessageService)Activator.GetObject(
				typeof(MessageService), this.Settings.GetValue("uri"));
			return remoteObj;
		}
	}
}
