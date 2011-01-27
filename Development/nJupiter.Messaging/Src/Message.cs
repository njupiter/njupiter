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
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Globalization;

namespace nJupiter.Messaging {

	[XmlTypeAttribute(Namespace = "urn:njupiter:messaging:message")]
	[Serializable]
	public class Message {

		#region Private instance members
		private long id;
		private DateTime created = DateTime.Now;
		private DateTime startPublish = DateTime.Now;
		private DateTime stopPublish = DateTime.MaxValue;
		private string data;
		private MessageDestination messageDestination;
		private int interval;
		#endregion

		#region  Properties - Xml serializable
		[XmlAttribute(DataType = "long", AttributeName = "id")]
		public long Id { get { return this.id; } set { this.id = value; } }

		[XmlAttribute(DataType = "dateTime", AttributeName = "created")]
		public DateTime Created { get { return this.created; } set { this.created = value; } }

		[XmlAttribute(DataType = "dateTime", AttributeName = "startpublish")]
		public DateTime StartPublish { get { return this.startPublish; } set { this.startPublish = value; } }

		[XmlAttribute(DataType = "dateTime", AttributeName = "stoppublish")]
		public DateTime StopPublish { get { return this.stopPublish; } set { this.stopPublish = value; } }

		[XmlAttribute(DataType = "string", AttributeName = "data")]
		public string Data { get { return this.data; } set { this.data = value; } }

		[XmlElement("messagedestination")]
		public MessageDestination Destination { get { return this.messageDestination; } set { this.messageDestination = value; } }

		[XmlAttribute(DataType = "int", AttributeName = "interval")]
		public int Interval { get { return this.interval; } set { this.interval = value; } }
		#endregion

		#region Constructor

		/// <summary>
		/// Public constructor, used for Xml deserialization
		/// </summary>
		public Message() { }

		/// <summary>
		/// Internal construct, used for fabrication
		/// </summary>
		/// <param name="startPublish"></param>
		/// <param name="stopPublish"></param>
		/// <param name="message"></param>
		/// <param name="messageDestination"></param>
		internal Message(DateTime startPublish, DateTime stopPublish, string message, MessageDestination messageDestination) {
			if(message == null)
				throw new ArgumentNullException("message");

			if(messageDestination == null)
				throw new ArgumentNullException("messageDestination");

			this.created = DateTime.Now;
			this.startPublish = startPublish;
			this.stopPublish = stopPublish;
			this.data = message;
			this.messageDestination = messageDestination;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Serialize a <code>Message</code> object to Xml. 
		/// </summary>
		/// <returns></returns>
		public XmlDocument Serialize() {

			XmlSerializer serializer = new XmlSerializer(typeof(Message));
			StringWriter stringwriter = new StringWriter(CultureInfo.InvariantCulture);
			XmlTextWriter xmlwriter = new XmlTextWriter(stringwriter);
			serializer.Serialize(xmlwriter, this);
			XmlDocument xml = new XmlDocument();
			xml.LoadXml(stringwriter.ToString());
			return xml;
		}

		/// <summary>
		/// Deserialize a <code>Message</code> object from Xml. 
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		static public Message Deserialize(XmlDocument xml) {
			if(xml == null)
				throw new ArgumentNullException("xml");
			XmlSerializer serializer = new XmlSerializer(typeof(Message));
			StringReader stringReader = new StringReader(xml.OuterXml);
			XmlTextReader xmlReader = new XmlTextReader(stringReader);
			return (Message)serializer.Deserialize(xmlReader);
		}

		static public Message Deserialize(string xml) {
			if(xml == null)
				throw new ArgumentNullException("xml");

			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(xml);

			return Deserialize(xmlDoc);
		}

		public override bool Equals(object obj) {
			Message msgObj = obj as Message;
			if(msgObj == null)
				return false;
			return this.Id == msgObj.Id;
		}

		public override int GetHashCode() {
			return this.Id.GetHashCode();
		}

		/// <summary>
		/// Returns a string representation of the <code>Message</code>object according to the following format
		/// [Id, Created, StartPublish, StopDate, Interval MessageDestination]
		/// </summary>
		/// <returns></returns>
		public override string ToString() {
			StringBuilder str = new StringBuilder();
			str.Append("[");
			str.Append(this.Id);
			str.Append(",");
			str.Append(Created);
			str.Append(",");
			str.Append(StartPublish);
			str.Append(",");
			str.Append(StopPublish);
			str.Append(",");
			str.Append(Interval);
			str.Append(",");
			str.Append(Destination);
			str.Append("]");
			return str.ToString();
		}

		#endregion
	}
}
