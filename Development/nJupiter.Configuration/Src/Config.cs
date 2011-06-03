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
using System.Globalization;
using System.Xml;
using System.Collections.Generic;

namespace nJupiter.Configuration {

	public class Config : IConfig {

		private readonly ConfigCollection innerConfigurations = new ConfigCollection();
		private readonly Dictionary<string, object> configHandlers = new Dictionary<string, object>();
		private readonly object padlock = new object();
		private readonly IConfigSource source;
		private readonly string configKey;
		private bool isDiscarded;
		private readonly XmlElement configXml;

		private const string DefaultAttribute = "value";

		public string ConfigKey { get { return this.configKey; } }
		public XmlElement ConfigXml { get { return this.configXml; } }
		public IConfigSource ConfigSource { get{ return source; } }

		public event EventHandler Discarded;
		public bool IsDiscarded { get{ return this.isDiscarded; } }

		internal Config(string configKey, XmlElement element)
			: this(configKey, element, null) {
		}

		internal Config(string configKey, XmlElement element, IConfigSource source) {
			if(configKey == null) {
				throw new ArgumentNullException("configKey");
			}
			if(element == null) {
				throw new ArgumentNullException("element");
			}
			this.configKey = configKey;
			this.configXml = element;
			this.source = source ?? new ConfigSource();
			if(this.source.Watcher != null){
				this.source.Watcher.ConfigSourceUpdated += this.Discard;
			}
		}

		public string GetValue(string key) {
			return GetValue<string>(key);
		}

		public T GetValue<T>(string key) {
			return GetValue<T>(null, key);
		}

		public T GetValue<T>(string section, string key) {
			return GetAttribute<T>(section, key, null);
		}

		public T GetAttribute<T>(string key, string attribute) {
			return this.GetAttribute<T>(null, key, attribute);
		}

		public T GetAttribute<T>(string section, string key, string attribute) {
			XmlNode node = this.GetKey(section, key);
			if(node != null && (attribute == null || AttributeExistsInNode(attribute, node))) {
				return this.GetValueFromXmlNode<T>(section, key, attribute, node);
			}
			throw new ConfigValueNotFoundException(string.Format("Value '{0}' was not found in the config with key '{1}'", GetXPath(section, key), this.ConfigKey));
		}

		public T[] GetValueArray<T>(string section, string key) {
			return this.GetAttributeArray<T>(section, key, null);
		}

		public T[] GetAttributeArray<T>(string section, string key, string attribute) {
			string xpath = GetXPath(section, key, attribute);
			XmlNodeList nodeList = this.ConfigXml.SelectNodes(xpath);
			var result = new T[nodeList.Count];
			for(int i = 0; i < nodeList.Count; i++) {
				result[i] = this.GetValueFromXmlNode<T>(section, key, attribute, nodeList[i]);
			}
			return result;
		}

		public string GetValue(string section, string key) {
			return GetValue<string>(section, key);
		}

		public string GetAttribute(string key, string attribute) {
			return GetAttribute<string>(key, attribute);
		}

		public string GetAttribute(string section, string key, string attribute) {
			return GetAttribute<string>(section, key, attribute);
		}

		public string[] GetValueArray(string section, string key) {
			return GetValueArray<string>(section, key);
		}

		public string[] GetAttributeArray(string section, string key, string attribute) {
			return this.GetAttributeArray<string>(section, key, attribute);
		}

		public XmlNode GetKey(string key) {
			return GetKey(null, key);
		}

		public XmlNode GetKey(string section, string key) {
			string xpath = GetXPath(section, key);
			return this.ConfigXml.SelectSingleNode(xpath);
		}

		public IConfig GetConfigSection(string section) {
			string key = string.Format("{0}:{1}", this.ConfigKey, section);
			if(this.innerConfigurations.Contains(key))
				return this.innerConfigurations[key];
			lock(padlock) {
				if(!this.innerConfigurations.Contains(key)) {
					XmlNode node = this.ConfigXml.SelectSingleNode(section);
					XmlElement configElement = node as XmlElement;

					if(configElement == null)
						return null;

					this.innerConfigurations.Add(new Config(key, configElement));
				}
			}
			return this.innerConfigurations[key];
		}

		public bool ContainsKey(string section, string key) {
			return this.GetKey(section, key) != null;
		}

		public bool ContainsKey(string key) {
			return ContainsKey(null, key);
		}

		public bool ContainsAttribute(string section, string key, string attribute) {
			XmlNode node = this.GetKey(section, key);
			if(node == null) {
				return false;
			}
			return AttributeExistsInNode(attribute, node);
		}

		public bool ContainsAttribute(string key, string attribute) {
			return ContainsAttribute(null, key, attribute);
		}

		public object GetConfigurationSectionHandler(string section, Type configurationSectionHandlerType) {
			if(!configHandlers.ContainsKey(section)) {
				lock(padlock) {
					if(!configHandlers.ContainsKey(section)) {
						object result = this.GetConfigurationSectionHandlerInternal(section, configurationSectionHandlerType);
						configHandlers.Add(section, result);
					}
				}
			}
			return configHandlers[section];
		}

		private object GetConfigurationSectionHandlerInternal(string section, Type configurationSectionHandlerType) {
			object result = System.Configuration.ConfigurationManager.GetSection(section);
			if(result == null) {
				XmlNode node = this.configXml.SelectSingleNode(section);
				if(node != null) {
					result = CreateConfigurationSectionHandler(node, configurationSectionHandlerType);
				}
			}
			return result;
		}

		private static object CreateConfigurationSectionHandler(XmlNode node, Type configurationSectionHandlerType) {
			System.Configuration.IConfigurationSectionHandler configurationSectionHandler = Activator.CreateInstance(configurationSectionHandlerType) as System.Configuration.IConfigurationSectionHandler;
			if(configurationSectionHandler != null) {
				return configurationSectionHandler.Create(null, null, node);
			}
			return null;
		}

		private T ParseValue<T>(string section, string key, string attribute, string value, CultureInfo culture) {
			try {
				return StringParser.Instance.Parse<T>(value, culture);
			}catch(Exception ex) {
				throw new InvalidConfigValueException(string.Format("Error wile parsing value '{0}' with key '{1}' in config with key '{2}' of expected type '{3}' with culture '{4}'.", value, GetXPath(section, key, attribute), this.ConfigKey, typeof(T).Name, culture.Name), ex);
			}
		}

		private static bool AttributeExistsInNode(string attribute, XmlNode node) {
			return GetAttributeValueFromXmlNode(attribute, node) != null;
		}

		private T GetValueFromXmlNode<T>(string section, string key, string attribute, XmlNode node) {
			attribute = GetAttributeName(attribute);
			string value = GetAttributeValueFromXmlNode(attribute, node);
			if(value == null && DefaultAttribute.Equals(attribute)){
				value = node.InnerText;
			}
			CultureInfo nodeCulture = GetCultureFromNode(node);

			return this.ParseValue<T>(section, key, attribute, value, nodeCulture);
		}

		private static CultureInfo GetCultureFromNode(XmlNode node) {
			XmlNodeReader nodeReader = new XmlNodeReader(node);
			nodeReader.Read();
			string lang = nodeReader.XmlLang;
			if(!string.IsNullOrEmpty(lang)) {
				return CultureInfo.CreateSpecificCulture(lang);
			}
			return CultureInfo.InvariantCulture;
		}

		private static string GetAttributeValueFromXmlNode(string attribute, XmlNode node) {
			if(node.Attributes != null && node.Attributes[attribute] != null) {
				return node.Attributes[attribute].Value;
			}
			return null;
		}

		private static string GetAttributeName(string attribute) {
			return !string.IsNullOrEmpty(attribute) ? attribute : DefaultAttribute;
		}

		private static string GetXPath(string section, string key) {
			return GetXPath(section, key, null);
		}

		private static string GetXPath(string section, string key, string attribute) {
			section = section ?? ".";
			attribute = !string.IsNullOrEmpty(attribute) ? string.Format("[@{0}]", attribute) : string.Empty;
			return string.Format("{0}/{1}{2}", section, key, attribute);
		}

		public void Discard(object source, EventArgs e) {
			this.Discard();
		}

		public void Discard() {
			if(!this.isDiscarded) {
				lock(padlock){
					if(!this.isDiscarded) {
						this.isDiscarded = true;
						if(this.source != null && this.source.Watcher != null) {
							this.source.Watcher.ConfigSourceUpdated -= this.Discard;
						}
						if(this.Discarded != null) {
							this.Discarded(this, EventArgs.Empty);
						}
					}
				}
			}
		}

		~Config() {
			this.Discard();
		}
	}
}
