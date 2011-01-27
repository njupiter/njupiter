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
using System.Xml.Serialization;

namespace nJupiter.Messaging {

	[Serializable]
	public class MessageConsumer {

		#region Private instance Members
		private MessageDestination messageDestination;
		private DateTime dateCreated;
		private string notificationUrl;
		#endregion

		#region Properties - Xml serializable
		[XmlAttribute(DataType = "dateTime", AttributeName = "created")]
		public DateTime Created { get { return this.dateCreated; } set { this.dateCreated = value; } }

		[XmlAttribute(DataType = "string", AttributeName = "notificationurl")]
		public string NotificationUrl { get { return this.notificationUrl; } set { this.notificationUrl = value; } }

		[XmlElement("messagedestination")]
		public MessageDestination Destination { get { return this.messageDestination; } set { this.messageDestination = value; } }
		#endregion

		#region MessageConsumer Members
		/// <summary>
		/// Implemented by the client
		/// </summary>
		/// <param name="message">The message that the client will recive</param>
		public virtual void Notify(Message message) {
			throw new NotImplementedException();
		}
		#endregion

		public override bool Equals(object obj) {
			MessageConsumer msgConsumer = obj as MessageConsumer;

			if(msgConsumer == null)
				return false;

			return this.Destination.Equals(msgConsumer.Destination) && this.NotificationUrl.Equals(msgConsumer.NotificationUrl);
		}

		public override int GetHashCode() {
			return this.Destination.GetHashCode() + this.NotificationUrl.GetHashCode() * 17;
		}
	}
}
