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

	public class Config : IConfig, IDisposable {

		private readonly ConfigCollection innerConfigurations = new ConfigCollection();
		private readonly Dictionary<string, object> configRepositories = new Dictionary<string, object>();
		private readonly object padlock = new object();
		private readonly IConfigSource source;
		private readonly string configKey;
		private bool isDiscarded;
		private readonly XmlElement configXml;

		private const string DefaultAttribute = "value";

		public string ConfigKey { get { return configKey; } }
		public XmlElement ConfigXml { get { return configXml; } }
		public IConfigSource ConfigSource { get{ return source; } }

		public event EventHandler Discarded;
		public bool IsDiscarded { get{ return isDiscarded; } }

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
			configXml = element;
			this.source = source ?? new ConfigSource(element);
			if(this.source.Watcher != null){
				this.source.Watcher.ConfigSourceUpdated += Discard;
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
			return GetAttribute<T>(null, key, attribute);
		}

		public T GetAttribute<T>(string section, string key, string attribute) {
			var node = GetKey(section, key);
			if(node != null && (attribute == null || AttributeExistsInNode(attribute, node))) {
				return GetValueFromXmlNode<T>(section, key, attribute, node);
			}
			throw new ConfigValueNotFoundException(string.Format("Value '{0}' was not found in the config with key '{1}'", GetXPath(section, key, attribute), ConfigKey));
		}

		public T[] GetValueArray<T>(string section, string key) {
			return GetAttributeArray<T>(section, key, null);
		}

		public T[] GetAttributeArray<T>(string section, string key, string attribute) {
			var xpath = GetXPath(section, key, attribute);
			var nodeList = ConfigXml.SelectNodes(xpath);
			var result = new T[nodeList.Count];
			for(var i = 0; i < nodeList.Count; i++) {
				result[i] = GetValueFromXmlNode<T>(section, key, attribute, nodeList[i]);
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
			return GetAttributeArray<string>(section, key, attribute);
		}

		public XmlNode GetKey(string key) {
			return GetKey(null, key);
		}

		public XmlNode GetKey(string section, string key) {
			var xpath = GetXPath(section, key);
			return ConfigXml.SelectSingleNode(xpath);
		}

		public IConfig GetConfigSection(string section) {
			var key = string.Format("{0}:{1}", ConfigKey, section);
			if(innerConfigurations.Contains(key))
				return innerConfigurations[key];
			lock(padlock) {
				if(!innerConfigurations.Contains(key)) {
					var node = ConfigXml.SelectSingleNode(section);
					var configElement = node as XmlElement;

					if(configElement == null)
						return null;

					innerConfigurations.Add(new Config(key, configElement));
				}
			}
			return innerConfigurations[key];
		}

		public bool ContainsKey(string section, string key) {
			return GetKey(section, key) != null;
		}

		public bool ContainsKey(string key) {
			return ContainsKey(null, key);
		}

		public bool ContainsAttribute(string section, string key, string attribute) {
			var node = GetKey(section, key);
			if(node == null) {
				return false;
			}
			return AttributeExistsInNode(attribute, node);
		}

		public bool ContainsAttribute(string key, string attribute) {
			return ContainsAttribute(null, key, attribute);
		}

		public object GetConfigurationSectionHandler(string section, Type configurationSectionHandlerType) {
			if(!configRepositories.ContainsKey(section)) {
				lock(padlock) {
					if(!configRepositories.ContainsKey(section)) {
						var result = GetConfigurationSectionHandlerInternal(section, configurationSectionHandlerType);
						configRepositories.Add(section, result);
					}
				}
			}
			return configRepositories[section];
		}

		private object GetConfigurationSectionHandlerInternal(string section, Type configurationSectionHandlerType) {
			var result = System.Configuration.ConfigurationManager.GetSection(section);
			if(result == null) {
				var node = configXml.SelectSingleNode(section);
				if(node != null) {
					result = CreateConfigurationSectionHandler(node, configurationSectionHandlerType);
				}
			}
			return result;
		}

		private static object CreateConfigurationSectionHandler(XmlNode node, Type configurationSectionHandlerType) {
			var configurationSectionHandler = Activator.CreateInstance(configurationSectionHandlerType) as System.Configuration.IConfigurationSectionHandler;
			if(configurationSectionHandler != null) {
				return configurationSectionHandler.Create(null, null, node);
			}
			return null;
		}

		private T ParseValue<T>(string section, string key, string attribute, string value, CultureInfo culture) {
			try {
				return StringParser.Instance.Parse<T>(value, culture);
			}catch(Exception ex) {
				throw new InvalidConfigValueException(string.Format("Error wile parsing value '{0}' with key '{1}' in config with key '{2}' of expected type '{3}' with culture '{4}'.", value, GetXPath(section, key, attribute), ConfigKey, typeof(T).Name, culture.Name), ex);
			}
		}

		private static bool AttributeExistsInNode(string attribute, XmlNode node) {
			return GetAttributeValueFromXmlNode(attribute, node) != null;
		}

		private T GetValueFromXmlNode<T>(string section, string key, string attribute, XmlNode node) {
			attribute = GetAttributeName(attribute);
			var value = GetAttributeValueFromXmlNode(attribute, node);
			if(value == null && DefaultAttribute.Equals(attribute)){
				value = node.InnerText;
			}
			var nodeCulture = GetCultureFromNode(node);

			return ParseValue<T>(section, key, attribute, value, nodeCulture);
		}

		private static CultureInfo GetCultureFromNode(XmlNode node) {
			var nodeReader = new XmlNodeReader(node);
			nodeReader.Read();
			var lang = nodeReader.XmlLang;
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
			Dispose();
		}

		public void Discard() {
			Dispose();
		}

		private void Dispose(bool disposing) {
			if(!isDiscarded) {
				lock(padlock){
					if(!isDiscarded) {
						isDiscarded = true;
						if(source != null && source.Watcher != null) {
							source.Watcher.ConfigSourceUpdated -= Discard;
						}
						if(disposing && Discarded != null) {
							Discarded(this, EventArgs.Empty);
						}
					}
				}
			}
			if(disposing){
				GC.SuppressFinalize(this);
			}
		}

		public void Dispose() {
			Dispose(true);
		}

		~Config() {
			Dispose(false);
		}
	}
}
