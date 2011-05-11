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
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Soap;
using System.Globalization;

using nJupiter.Configuration;

namespace nJupiter.Messaging.Server {

	/// <summary>
	/// The class that keep track of MessageProducers and its MessageConsumers. 
	/// </summary>
	[XmlTypeAttribute(Namespace = "urn:njupiter:messaging:server:messageconsumermap")]
	[Serializable]
	internal sealed class MessageConsumerMap {

		#region Constants
		private const string MessageServicesSection = "messageServices";
		private const string MessageServiceSection = MessageServicesSection + "/messageService";
		private const string MessageServiceSectionFormat = MessageServiceSection + "[@value='{0}']";
		private const string SettingsSectionFormat = MessageServiceSectionFormat + "/settings";
		#endregion

		#region Private static members
		private static readonly string ConsumersFilename = ConfigHandler.Instance.GetConfig(Assembly.GetAssembly(typeof(Message))).GetConfigSection(string.Format(CultureInfo.InvariantCulture, SettingsSectionFormat, "server")).GetValue("consumerFilename");
		#endregion

		#region Private instance members
		private Hashtable messageConsumers = new Hashtable();
		#endregion

		#region Properties
		[XmlElement("consumers")]
		public Hashtable ProducerConsumerMap { get { return this.messageConsumers; } set { this.messageConsumers = value; } }
		#endregion

		#region Methods
		internal void AddMessageConsumer(MessageConsumer consumer) {
			if(consumer == null)
				throw new ArgumentNullException("consumer");
			ArrayList consumers = this.messageConsumers[consumer.Destination] as ArrayList ?? new ArrayList();
			if(!consumers.Contains(consumer)) {
				consumers.Add(consumer);
			}
			this.messageConsumers[consumer.Destination] = consumers;
		}

		internal ArrayList GetMessageConsumers(MessageDestination messageDestination) {
			if(messageDestination == null)
				throw new ArgumentNullException("messageDestination");
			return (ArrayList)this.messageConsumers[messageDestination];
		}

		public void Serialize() {
			if(this.messageConsumers != null && this.messageConsumers.Count > 0) {
				using(Stream stream = new FileStream(ConsumersFilename, FileMode.Create, FileAccess.Write, FileShare.None)) {
					SoapFormatter formatter = new SoapFormatter();
					formatter.Serialize(stream, this);
				}
			}
		}

		static public MessageConsumerMap Deserialize() {
			if(File.Exists(ConsumersFilename)) {
				using(Stream stream = new FileStream(ConsumersFilename, FileMode.Open, FileAccess.Read, FileShare.Read)) {
					SoapFormatter formatter = new SoapFormatter();
					MessageConsumerMap messageConsMap = (MessageConsumerMap)formatter.Deserialize(stream);
					return messageConsMap;
				}
			}
			return new MessageConsumerMap();
		}
		#endregion
	}
}
