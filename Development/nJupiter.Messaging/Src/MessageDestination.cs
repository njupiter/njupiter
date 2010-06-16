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
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Globalization;

namespace nJupiter.Messaging {

	[XmlTypeAttribute(Namespace = "urn:njupiter:messaging:messagedestination")]
	[Serializable]
	public class MessageDestination {

		#region Private instance members
		private string name;
		#endregion

		#region Properties - Xml serializable
		[XmlAttribute(DataType = "string", AttributeName = "name")]
		public string Name { get { return this.name; } set { this.name = value; } }
		#endregion

		#region Constructs
		/// <summary>
		/// Public construct, used for Xml serialization.
		/// </summary>
		public MessageDestination() { }

		internal MessageDestination(string name) {
			if(name == null)
				throw new ArgumentNullException("name");

			this.name = name;
		}
		#endregion

		#region Methods
		public override bool Equals(object obj) {
			MessageDestination msgDest = obj as MessageDestination;
			if(msgDest == null)
				return false;

			return this.Name.Equals(msgDest.Name);
		}

		public override int GetHashCode() {
			return this.Name.GetHashCode();
		}

		public override string ToString() {
			return this.Name;
		}

		public XmlDocument Serialize() {

			XmlSerializer serializer = new XmlSerializer(typeof(MessageDestination));
			StringWriter stringwriter = new StringWriter(CultureInfo.InvariantCulture);
			XmlTextWriter xmlwriter = new XmlTextWriter(stringwriter);
			serializer.Serialize(xmlwriter, this);
			XmlDocument xml = new XmlDocument();
			xml.LoadXml(stringwriter.ToString());
			return xml;
		}

		static public MessageDestination Deserialize(XmlDocument xml) {
			if(xml == null)
				throw new ArgumentNullException("xml");

			XmlSerializer serializer = new XmlSerializer(typeof(MessageDestination));
			StringReader stringReader = new StringReader(xml.OuterXml);
			XmlTextReader xmlReader = new XmlTextReader(stringReader);
			return (MessageDestination)serializer.Deserialize(xmlReader);
		}

		static public MessageDestination Deserialize(string xml) {
			if(xml == null)
				throw new ArgumentNullException("xml");

			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(xml);

			return Deserialize(xmlDoc);
		}
		#endregion

	}
}
