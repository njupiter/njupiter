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
using System.Xml;
using System.Globalization;
using System.Collections.Generic;

namespace nJupiter.Configuration {

	internal class Config : IConfig {

		#region Members
		private readonly ConfigCollection innerConfigurations = new ConfigCollection();
		private readonly Dictionary<string, object> configHandlers = new Dictionary<string, object>();
		private readonly object padlock = new object();
		private readonly IConfigSource source;
		private readonly string configKey;
		private bool disposed;
		private XmlElement configXml;
		#endregion

		#region Constants
		private const string DefaultAttribute = "value";
		#endregion

		#region Properties
		public string ConfigKey { get { return this.configKey; } }
		public XmlElement ConfigXml { get { return this.configXml; } internal set { configXml = value; } }
		public IConfigSource ConfigSource { get{ return source; } }
		#endregion

		#region Events
		public event EventHandler Disposed;
		#endregion

		#region Constructors
		internal Config(string configKey, XmlElement element)
			: this(configKey, element, null) {
		}

		internal Config(string configKey, XmlElement element, IConfigSource source) {
			this.configKey = configKey;
			this.configXml = element;
			this.source = source ?? new ConfigSource();
		}
		#endregion

		#region Methods
		public bool GetBoolValue(string key) {
			return GetBoolValue(".", key);
		}

		public bool GetBoolValue(string section, string key) {
			return string.Compare(GetValue(section, key), "true", true, CultureInfo.InvariantCulture) == 0;
		}

		public int GetIntValue(string key) {
			return GetIntValue(".", key);
		}

		public int GetIntValue(string section, string key) {
			int value;
			if(!int.TryParse(GetValue(section, key), NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out value)) {
				throw new InvalidConfigValueException(string.Format("Value [{0}/{1}] is not of type integer.", section, key));
			}
			return value;
		}

		public char GetCharValue(string key) {
			return GetCharValue(".", key);
		}

		public char GetCharValue(string section, string key) {
			char value;
			if(!char.TryParse(GetValue(section, key), out value)) {
				throw new InvalidConfigValueException(string.Format("Value [{0}/{1}] is not of type char.", section, key));
			}
			return value;
		}

		public string GetValue(string key) {
			return GetValue(".", key);
		}

		public string GetValue(string section, string key) {
			XmlNode node = this.GetKey(section, key);
			if(node != null) {
				return GetXmlNodeValue(node, null);
			}
			throw new ConfigValueNotFoundException(string.Format("Value [{0}/{1}] was not found in the config with key [{2}].", section, key, this.ConfigKey));
		}

		public bool GetBoolAttribute(string key, string attribute) {
			return GetBoolAttribute(".", key, attribute);
		}

		public bool GetBoolAttribute(string section, string key, string attribute) {
			return string.Compare(GetAttribute(section, key, attribute), "true", true, CultureInfo.InvariantCulture) == 0;
		}

		public int GetIntAttribute(string key, string attribute) {
			return GetIntAttribute(".", key, attribute);
		}

		public int GetIntAttribute(string section, string key, string attribute) {
			int value;
			if(!int.TryParse(GetAttribute(section, key, attribute), NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out value)) {
				throw new InvalidConfigValueException(string.Format("Attribute [{0}/{1}/@{2}] is not of type integer.", section, key, attribute));
			}
			return value;
		}

		public char GetCharAttribute(string key, string attribute) {
			return GetCharAttribute(".", key, attribute);
		}

		public char GetCharAttribute(string section, string key, string attribute) {
			char value;
			if(!char.TryParse(GetAttribute(section, key, attribute), out value)) {
				throw new InvalidConfigValueException(string.Format("Attribute [{0}/{1}/@{2}] is not of type char.", section, key, attribute));
			}
			return value;
		}

		public string GetAttribute(string key, string attribute) {
			return GetAttribute(".", key, attribute);
		}

		public string GetAttribute(string section, string key, string attribute) {
			XmlNode node = this.GetKey(section, key);
			if(node != null) {
				XmlAttribute xmlattr = node.Attributes[attribute];
				if(xmlattr != null) {
					return xmlattr.Value;
				}
				throw new ConfigValueNotFoundException(string.Format("Attribute [{0}/{1}/@{2}] was not found in the config with key [{3}].", section, key, attribute, this.ConfigKey));
			}
			throw new ConfigValueNotFoundException(string.Format("Value [{0}/{1}] was not found in the config with key [{2}].", section, key, this.ConfigKey));
		}


		public XmlNode GetKey(string key) {
			return GetKey(".", key);
		}

		public XmlNode GetKey(string section, string key) {
			return this.ConfigXml.SelectSingleNode(section + "/" + key);
		}

		public char[] GetCharValueArray(string section, string key) {
			return GetCharAttributeArray(section, key, null);
		}

		public char[] GetCharAttributeArray(string section, string key, string attribute) {
			XmlNodeList nodeList = this.ConfigXml.SelectNodes(section + "/" + key + (!string.IsNullOrEmpty(attribute) ? "[@" + attribute + "]" : string.Empty));
			if(nodeList != null) {
				char[] result = new char[nodeList.Count];
				for(int i = 0; i < nodeList.Count; i++) {
					char value;
					if(!char.TryParse(GetXmlNodeValue(nodeList[i], attribute), out value)) {
						throw new InvalidConfigValueException(string.Format("Value [{0}/{1}[{2}]] is not of type char.", section, key, i));
					}
					result[i] = value;
			}
				return result;
			}
			return new char[0];
		}

		public int[] GetIntValueArray(string section, string key) {
			return GetIntAttributeArray(section, key, null);
		}

		public int[] GetIntAttributeArray(string section, string key, string attribute) {
			XmlNodeList nodeList = this.ConfigXml.SelectNodes(section + "/" + key + (!string.IsNullOrEmpty(attribute) ? "[@" + attribute + "]" : string.Empty));
			if(nodeList != null) {
				int[] result = new int[nodeList.Count];
				for(int i = 0; i < nodeList.Count; i++) {
					int value;
					if(!int.TryParse(GetXmlNodeValue(nodeList[i], attribute), NumberStyles.Integer, CultureInfo.InvariantCulture, out value)) {
						throw new InvalidConfigValueException(string.Format("Value [{0}/{1}[{2}]] is not of type integer.", section, key, i));
					}
					result[i] = value;
				}
				return result;
			}
			return new int[0];
		}

		public string[] GetValueArray(string section, string key) {
			return GetAttributeArray(section, key, null);
		}

		public string[] GetAttributeArray(string section, string key, string attribute) {
			XmlNodeList nodeList = this.ConfigXml.SelectNodes(section + "/" + key + (!string.IsNullOrEmpty(attribute) ? "[@" + attribute + "]" : string.Empty));
			if(nodeList != null) {
				string[] result = new string[nodeList.Count];
				for(int i = 0; i < nodeList.Count; i++) {
					result[i] = GetXmlNodeValue(nodeList[i], attribute);
				}
				return result;
			}
			return new string[0];
		}

		public IConfig GetConfigSection(string section) {
			string key = this.ConfigKey + ":" + section;
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
			XmlNode node = this.GetKey(section, key);
			if(node != null) {
				return true;
			}
			return false;
		}

		public bool ContainsKey(string key) {
			return ContainsKey(".", key);
		}

		public bool ContainsAttribute(string section, string key, string attribute) {
			XmlNode node = this.GetKey(section, key);
			if(node != null) {
				XmlAttribute xmlattr = node.Attributes[attribute];
				if(xmlattr != null) {
					return true;
				}
			}
			return false;
		}

		public bool ContainsAttribute(string key, string attribute) {
			return ContainsAttribute(".", key, attribute);
		}

		public object GetConfigurationSectionHandler(string sectionName, Type configurationSectionHandlerType) {

			if(!configHandlers.ContainsKey(sectionName)) {
				lock(padlock) {
					if(!configHandlers.ContainsKey(sectionName)) {
						object result = System.Configuration.ConfigurationManager.GetSection(sectionName);
						if(result == null) {
							XmlNode xmlNode = this.configXml.SelectSingleNode(sectionName);
							if(xmlNode != null) {
								System.Configuration.IConfigurationSectionHandler configurationSectionHandler = Activator.CreateInstance(configurationSectionHandlerType) as System.Configuration.IConfigurationSectionHandler;
								if(configurationSectionHandler != null) {
									result = configurationSectionHandler.Create(null, null, xmlNode);
								}
							}
						}
						configHandlers.Add(sectionName, result);
					}
				}
			}
			return configHandlers[sectionName];
		}
		#endregion

		#region Private Methods
		private static string GetXmlNodeValue(XmlNode xmlNode, string attribute) {
			attribute = !string.IsNullOrEmpty(attribute) ? attribute : DefaultAttribute;
			if(xmlNode.Attributes != null && xmlNode.Attributes[attribute] != null)
				return xmlNode.Attributes[attribute].Value;
			return DefaultAttribute.Equals(attribute) ? xmlNode.InnerText : null;
		}
		#endregion

		#region IDisposable Members
		private void Dispose(bool disposing) {
			if(!this.disposed) {
				this.disposed = true;
				// Suppress finalization of this disposed instance.
				if(disposing) {
					GC.SuppressFinalize(this);
					if(this.Disposed != null) {
						this.Disposed(this, EventArgs.Empty);
					}
				}
			}
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose() {
			Dispose(true);
		}

		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="Config"/> is reclaimed by garbage collection.
		/// </summary>
		~Config() {
			Dispose(false);
		}
		#endregion
	}
}
