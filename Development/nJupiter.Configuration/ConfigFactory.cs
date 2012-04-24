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
using System.Xml;

namespace nJupiter.Configuration {
	public static class ConfigFactory {
		
		public static IConfig Create(string configKey, Stream stream, IConfigSource source) {
			var xmlElement = GetConfigXmlElement(stream);
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
			var xmlElement = GetDocumentElementFromStream(configStream);
			return GetXmlElementFromXmlNode(xmlElement);
		}

		private static XmlElement GetDocumentElementFromStream(Stream configStream) {
			var doc = new XmlDocument();
			var reader = CreateXmlReaderFromStream(configStream);
			doc.Load(reader);
			return doc.DocumentElement;
		}

		private static XmlReader CreateXmlReaderFromStream(Stream configStream) {
			var readerSettings = new XmlReaderSettings();
			var xtr = new XmlTextReader(configStream);
			readerSettings.ValidationType = ValidationType.None;
			xtr.EntityHandling = EntityHandling.ExpandEntities;
			return XmlReader.Create(xtr, readerSettings);
		}

		private static XmlElement GetXmlElementFromXmlNode(XmlNode element) {
			if(element == null) {
				throw new ArgumentNullException("element");
			}
			// Copy the xml data into the root of a new document
			// this isolates the xml config data from the rest of  the document
			var newDoc = new XmlDocument();
			var newElement = (XmlElement)newDoc.AppendChild(newDoc.ImportNode(element, true));
			return newElement;
		}
	}
}
