
using System;
using System.IO;
using System.Xml;

namespace nJupiter.Configuration {
	public class ConfigFactory {
		
		public static IConfig Create(string configKey, Stream stream, IConfigSource source) {
			XmlElement xmlElement = GetConfigXmlElement(stream);
			return new Config(configKey, xmlElement, source);
		}

		public static IConfig Create(string configKey, XmlElement element) {
			return new Config(configKey, element);
			
		}

		public static IConfig Create(string configKey, XmlElement element, IConfigSource source) {
			return new Config(configKey, element, source);
		}

		private static XmlElement GetConfigXmlElement(Stream configStream) {
			if(configStream == null) {
				throw new ArgumentNullException("configStream");
			}
			XmlElement xmlElement = GetDocumentElementFromStream(configStream);
			return GetXmlElementFromXmlNode(xmlElement);
		}

		private static XmlElement GetDocumentElementFromStream(Stream configStream) {
			XmlDocument doc = new XmlDocument();
			var reader = CreateXmlReaderFromStream(configStream);
			doc.Load(reader);
			return doc.DocumentElement;
		}

		private static XmlReader CreateXmlReaderFromStream(Stream configStream) {
			XmlReaderSettings readerSettings = new XmlReaderSettings();
			XmlTextReader xtr = new XmlTextReader(configStream);
			readerSettings.ValidationType = ValidationType.None;
			xtr.EntityHandling = EntityHandling.ExpandEntities;
			return XmlReader.Create(xtr, readerSettings);
		}

		private static XmlElement GetXmlElementFromXmlNode(XmlNode element) {
			if(element == null)
				throw new ArgumentNullException("element");
			// Copy the xml data into the root of a new document
			// this isolates the xml config data from the rest of  the document
			XmlDocument newDoc = new XmlDocument();
			XmlElement newElement = (XmlElement)newDoc.AppendChild(newDoc.ImportNode(element, true));
			return newElement;
		}
	}
}
