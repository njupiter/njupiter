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
using System.IO;
using System.Globalization;
using System.Collections.Generic;

namespace nJupiter.Configuration {

	/// <summary>
	/// Represents a configuration Xml with shorcuts to access sections, elements and attributes
	/// </summary>
	public class Config : IDisposable {

		#region Members
		private readonly ConfigCollection innerConfigurations = new ConfigCollection();
		private readonly Dictionary<string, object> configHandlers = new Dictionary<string, object>();
		private readonly object padlock = new object();
		private readonly FileInfo configFile;
		private readonly Uri configUri;
		private readonly string configKey;
		private bool disposed;
		private XmlElement configXml;
		private WatchedConfigHandler watcher;
		#endregion

		#region Constants
		private const string DefaultAttribute = "value";
		#endregion

		#region Properties
		/// <summary>
		/// Gets the unique key for this configuration object. This is in most cases equal to the assembly name which the configuration object belongs to.
		/// </summary>
		/// <value>The config key.</value>
		public string ConfigKey { get { return this.configKey; } }
		/// <summary>
		/// Gets the Xml Element associated with the configuration object.
		/// </summary>
		/// <value>The config XML.</value>
		public XmlElement ConfigXML { get { return this.configXml; } internal set { configXml = value; } }
		/// <summary>
		/// If the Xml associated with the configuration object is stored localy on fire this property return the <see cref="FileInfo" /> for the Xml file.
		/// </summary>
		/// <value>The <see cref="FileInfo" /> object associated with the Xml if it is stored as a file on the local machine; otherwise, <c>null</c>.</value>
		public FileInfo ConfigFile { get { return this.configFile; } }
		/// <summary>
		/// Gets the <see cref="Uri" /> that contains the stream for the Xml.
		/// </summary>
		/// <value>If exists, the <see cref="Uri" /> associated with the Xml; otherwise, <c>null</c>.</value>
		public Uri ConfigUri { get { return this.configUri; } }
		internal WatchedConfigHandler Watcher { get { return this.watcher; } set { this.watcher = value; } }
		#endregion

		#region Events
		/// <summary>
		/// Occurs when the configuration object is disposed. This happens when it is droped from the cache.
		/// </summary>
		public event EventHandler Disposed;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="Config"/> class.
		/// </summary>
		/// <param name="element">The Xml element accociated with the config object.</param>
		public Config(XmlElement element)
			: this(Guid.NewGuid().ToString(), element, null, null) {
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Config"/> class.
		/// </summary>
		/// <param name="configKey">The config key for the config object.</param>
		public Config(string configKey)
			: this(configKey, null, null, null) {
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Config"/> class.
		/// </summary>
		/// <param name="configKey">The config key for the config object.</param>
		/// <param name="element">The Xml element accociated with the config object.</param>
		public Config(string configKey, XmlElement element)
			: this(configKey, element, null, null) {
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Config"/> class.
		/// </summary>
		/// <param name="configKey">The config key for the config object.</param>
		/// <param name="configFile">The config file accociated with the config object.</param>
		public Config(string configKey, FileInfo configFile)
			: this(configKey, null, configFile, null) {
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Config"/> class.
		/// </summary>
		/// <param name="configKey">The config key for the config object.</param>
		/// <param name="element">The Xml element accociated with the config object.</param>
		/// <param name="configFile">The config file accociated with the config object.</param>
		public Config(string configKey, XmlElement element, FileInfo configFile)
			: this(configKey, element, configFile, null) {
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Config"/> class.
		/// </summary>
		/// <param name="configKey">The config key for the config object.</param>
		/// <param name="configUri">The URI that contains a stream of the Xml associated with the config object.</param>
		public Config(string configKey, Uri configUri)
			: this(configKey, null, null, configUri) {
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Config"/> class.
		/// </summary>
		/// <param name="configKey">The config key for the config object.</param>
		/// <param name="element">The Xml element accociated with the config object.</param>
		/// <param name="configUri">The URI that contains a stream of the Xml associated with the config object.</param>
		public Config(string configKey, XmlElement element, Uri configUri)
			: this(configKey, element, null, configUri) {
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Config"/> class.
		/// </summary>
		/// <param name="configKey">The config key for the config object.</param>
		/// <param name="element">The Xml element accociated with the config object.</param>
		/// <param name="configFile">The config file accociated with the config object.</param>
		/// <param name="configUri">The URI that contains a stream of the Xml associated with the config object.</param>
		private Config(string configKey, XmlElement element, FileInfo configFile, Uri configUri) {
			this.configKey = configKey;
			this.configXml = element;
			this.configFile = configFile;

			this.configUri = configFile != null ? new Uri(configFile.FullName) : configUri;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Gets the value attribute for the element given in the key parameter as a bool. If no value attribute exists on the element the content of the element is returned.
		/// </summary>
		/// <param name="key">The name of the element. The parameter can contain XPath syntax.</param>
		/// <returns>The bool value for the key.</returns>
		public bool GetBoolValue(string key) {
			return GetBoolValue(".", key);
		}

		/// <summary>
		/// Gets the value attribute for the element given in the key parameter as a bool. If no value attribute exists on the element the content of the element is returned.
		/// </summary>
		/// <param name="section">The path to the element. The parameter can contain XPath syntax.</param>
		/// <param name="key">The name of the element. The parameter can contain XPath syntax.</param>
		/// <returns>The bool value for the key.</returns>
		public bool GetBoolValue(string section, string key) {
			return string.Compare(GetValue(section, key), "true", true, CultureInfo.InvariantCulture) == 0;
		}

		/// <summary>
		/// Gets the value attribute for the element given in the key parameter as an int. If no value attribute exists on the element the content of the element is returned.
		/// </summary>
		/// <param name="key">The name of the element. The parameter can contain XPath syntax.</param>
		/// <returns>The int value for the key.</returns>
		public int GetIntValue(string key) {
			return GetIntValue(".", key);
		}

		/// <summary>
		/// Gets the value attribute for the element given in the key parameter as an int. If no value attribute exists on the element the content of the element is returned.
		/// </summary>
		/// <param name="section">The path to the element. The parameter can contain XPath syntax.</param>
		/// <param name="key">The name of the element. The parameter can contain XPath syntax.</param>
		/// <returns>The int value for the key.</returns>
		public int GetIntValue(string section, string key) {
			try {
				return int.Parse(GetValue(section, key), NumberFormatInfo.InvariantInfo);
			} catch(FormatException) {
				throw new InvalidConfigValueException(string.Format("Value [{0}/{1}] is not of type integer.", section, key));
			}
		}

		/// <summary>
		/// Gets the value attribute for the element given in the key parameter. If no value attribute exists on the element the content of the element is returned.
		/// </summary>
		/// <param name="key">The name of the element. The parameter can contain XPath syntax.</param>
		/// <returns>The string value for the key.</returns>
		public string GetValue(string key) {
			return GetValue(".", key);
		}

		/// <summary>
		/// Gets the value attribute for the element given in the key parameter. If no value attribute exists on the element the content of the element is returned.
		/// </summary>
		/// <param name="section">The path to the element. The parameter can contain XPath syntax.</param>
		/// <param name="key">The name of the element. The parameter can contain XPath syntax.</param>
		/// <returns>The string value for the key.</returns>
		public string GetValue(string section, string key) {
			XmlNode node = this.GetKey(section, key);
			if(node != null) {
				return GetXmlNodeValue(node, null);
			}
			throw new ConfigValueNotFoundException(string.Format("Value [{0}/{1}] was not found in the config with key [{2}].", section, key, this.ConfigKey));
		}

		/// <summary>
		/// Gets a given attribute for the element given in the key parameter as an int.
		/// </summary>
		/// <param name="key">The name of the element. The parameter can contain XPath syntax.</param>
		/// <param name="attribute">The name of the attribute. The parameter can contain XPath syntax.</param>
		/// <returns>The int value of the attribute.</returns>
		public int GetIntAttribute(string key, string attribute) {
			return GetIntAttribute(".", key, attribute);
		}

		/// <summary>
		/// Gets a given attribute for the element given in the key parameter as an int.
		/// </summary>
		/// <param name="section">The path to the element. The parameter can contain XPath syntax.</param>
		/// <param name="key">The name of the element. The parameter can contain XPath syntax.</param>
		/// <param name="attribute">The name of the attribute. The parameter can contain XPath syntax.</param>
		/// <returns>The int value of the attribute.</returns>
		public int GetIntAttribute(string section, string key, string attribute) {
			try {
				return int.Parse(GetAttribute(section, key, attribute), NumberFormatInfo.InvariantInfo);
			} catch(FormatException) {
				throw new InvalidConfigValueException(string.Format("Attribute [{0}/{1}/@{2}] is not of type integer.", section, key, attribute));
			}
		}

		/// <summary>
		/// Gets a given attribute for the element given in the key parameter as a bool.
		/// </summary>
		/// <param name="key">The name of the element. The parameter can contain XPath syntax.</param>
		/// <param name="attribute">The name of the attribute. The parameter can contain XPath syntax.</param>
		/// <returns>The bool value of the attribute.</returns>
		public bool GetBoolAttribute(string key, string attribute) {
			return GetBoolAttribute(".", key, attribute);
		}

		/// <summary>
		/// Gets a given attribute for the element given in the key parameter as a bool.
		/// </summary>
		/// <param name="section">The path to the element. The parameter can contain XPath syntax.</param>
		/// <param name="key">The name of the element. The parameter can contain XPath syntax.</param>
		/// <param name="attribute">The name of the attribute. The parameter can contain XPath syntax.</param>
		/// <returns>The bool value of the attribute.</returns>
		public bool GetBoolAttribute(string section, string key, string attribute) {
			return string.Compare(GetAttribute(section, key, attribute), "true", true, CultureInfo.InvariantCulture) == 0;
		}

		/// <summary>
		/// Gets a given attribute for the element given in the key parameter as a string.
		/// </summary>
		/// <param name="key">The name of the element. The parameter can contain XPath syntax.</param>
		/// <param name="attribute">The name of the attribute. The parameter can contain XPath syntax.</param>
		/// <returns>The string value of the attribute.</returns>
		public string GetAttribute(string key, string attribute) {
			return GetAttribute(".", key, attribute);
		}

		/// <summary>
		/// Gets a given attribute for the element given in the key parameter as a string.
		/// </summary>
		/// <param name="section">The path to the element. The parameter can contain XPath syntax.</param>
		/// <param name="key">The name of the element. The parameter can contain XPath syntax.</param>
		/// <param name="attribute">The name of the attribute. The parameter can contain XPath syntax.</param>
		/// <returns>The string value of the attribute.</returns>
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

		/// <summary>
		/// Gets the <see cref="XmlNode"/> for the given key.
		/// </summary>
		/// <param name="key">The element name. The parameter can contain XPath syntax.</param>
		/// <returns>An <see cref="XmlNode"/> for the given key.</returns>
		public XmlNode GetKey(string key) {
			return GetKey(".", key);
		}

		/// <summary>
		/// Gets the <see cref="XmlNode"/> for the given key.
		/// </summary>
		/// <param name="section">The path to the element. The parameter can contain XPath syntax.</param>
		/// <param name="key">The element name. The parameter can contain XPath syntax.</param>
		/// <returns>An <see cref="XmlNode"/> for the given key.</returns>
		public XmlNode GetKey(string section, string key) {
			return this.ConfigXML.SelectSingleNode(section + "/" + key);
		}

		/// <summary>
		/// Gets an array of value attribues from elements with the same name. If no value attribute exists on the element the content of the element is returned.
		/// </summary>
		/// <param name="section">The path to the elements. The parameter can contain XPath syntax.</param>
		/// <param name="key">The name of the elements. The parameter can contain XPath syntax.</param>
		/// <returns>A string array.</returns>
		public string[] GetValueArray(string section, string key) {
			return GetAttributeArray(section, key, null);
		}

		/// <summary>
		/// Gets an array of attribues from elements with the same name.
		/// </summary>
		/// <param name="section">The path to the elements. The parameter can contain XPath syntax.</param>
		/// <param name="key">The name of the elements. The parameter can contain XPath syntax.</param>
		/// <param name="attribute">The name of the attributes. The parameter can contain XPath syntax.</param>
		/// <returns>A string array.</returns>
		public string[] GetAttributeArray(string section, string key, string attribute) {
			XmlNodeList nodeList = this.ConfigXML.SelectNodes(section + "/" + key + (!string.IsNullOrEmpty(attribute) ? "[@" + attribute + "]" : string.Empty));
			if(nodeList != null) {
				string[] result = new string[nodeList.Count];
				for(int i = 0; i < nodeList.Count; i++) {
					result[i] = GetXmlNodeValue(nodeList[i], attribute);
				}
				return result;
			}
			return new string[0];
		}

		/// <summary>
		/// Gets an int array of value attribues from elements with the same name. If no value attribute exists on the element the content of the element is returned.
		/// </summary>
		/// <param name="section">The path to the elements. The parameter can contain XPath syntax.</param>
		/// <param name="key">The name of the elements. The parameter can contain XPath syntax.</param>
		/// <returns>A int array.</returns>
		public int[] GetIntValueArray(string section, string key) {
			return GetIntAttributeArray(section, key, null);
		}

		/// <summary>
		/// Gets an int array of attribues from elements with the same name.
		/// </summary>
		/// <param name="section">The path to the elements. The parameter can contain XPath syntax.</param>
		/// <param name="key">The name of the elements. The parameter can contain XPath syntax.</param>
		/// <param name="attribute">The name of the attributes. The parameter can contain XPath syntax.</param>
		/// <returns>A int array.</returns>
		public int[] GetIntAttributeArray(string section, string key, string attribute) {
			XmlNodeList nodeList = this.ConfigXML.SelectNodes(section + "/" + key + (!string.IsNullOrEmpty(attribute) ? "[@" + attribute + "]" : string.Empty));
			if(nodeList != null) {
				int[] result = new int[nodeList.Count];
				for(int i = 0; i < nodeList.Count; i++) {
					try {
						result[i] = int.Parse(GetXmlNodeValue(nodeList[i], attribute), CultureInfo.InvariantCulture);
					} catch(FormatException) {
						throw new InvalidConfigValueException(string.Format("Value [{0}/{1}[{2}]] is not of type integer.", section, key, i));
					}
				}
				return result;
			}
			return new int[0];
		}

		/// <summary>
		/// Gets a new config object based on a subelement to the current config Xml.
		/// </summary>
		/// <param name="section">The XPath to the section.</param>
		/// <returns>A <see cref="Config"/> object.</returns>
		public Config GetConfigSection(string section) {
			string key = this.ConfigKey + ":" + section;
			if(this.innerConfigurations.Contains(key))
				return this.innerConfigurations[key];
			lock(padlock) {
				if(!this.innerConfigurations.Contains(key)) {
					XmlNode node = this.ConfigXML.SelectSingleNode(section);
					XmlElement configElement = node as XmlElement;

					if(configElement == null)
						return null;

					this.innerConfigurations.Add(new Config(key, configElement));
				}
			}
			return this.innerConfigurations[key];
		}

		/// <summary>
		/// Determines whether the current configuration contains a specified key.
		/// </summary>
		/// <param name="section">The path to the element. The parameter can contain XPath syntax.</param>
		/// <param name="key">The element name. The parameter can contain XPath syntax.</param>
		/// <returns>
		/// 	<c>true</c> if the current configuration contains the specified key; otherwise, <c>false</c>.
		/// </returns>
		public bool ContainsKey(string section, string key) {
			XmlNode node = this.GetKey(section, key);
			if(node != null) {
				return true;
			}
			return false;
		}

		/// <summary>
		/// Determines whether the current configuration contains a specified key.
		/// </summary>
		/// <param name="key">The element name. The parameter can contain XPath syntax.</param>
		/// <returns>
		/// 	<c>true</c> if the current configuration contains the specified key; otherwise, <c>false</c>.
		/// </returns>
		public bool ContainsKey(string key) {
			return ContainsKey(".", key);
		}

		/// <summary>
		/// Determines whether the current configuration contains a specified attribute.
		/// </summary>
		/// <param name="section">The path to the element. The parameter can contain XPath syntax.</param>
		/// <param name="key">The element name. The parameter can contain XPath syntax.</param>
		/// <param name="attribute">The attribute name. The parameter can contain XPath syntax.</param>
		/// <returns>
		/// 	<c>true</c> if the current configuration contains the specified attribute; otherwise, <c>false</c>.
		/// </returns>
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

		/// <summary>
		/// Determines whether the current configuration contains a specified attribute.
		/// </summary>
		/// <param name="key">The element name. The parameter can contain XPath syntax.</param>
		/// <param name="attribute">The attribute name. The parameter can contain XPath syntax.</param>
		/// <returns>
		/// 	<c>true</c> if the current configuration contains the specified attribute; otherwise, <c>false</c>.
		/// </returns>
		public bool ContainsAttribute(string key, string attribute) {
			return ContainsAttribute(".", key, attribute);
		}

		/// <summary>
		/// Returns the configuration section handler by using the specified section and location paths.
		/// </summary>
		/// <param name="sectionName">The path of the section to be returned.</param>
		/// <param name="configurationSectionHandlerType">The type of the <see cref="System.Configuration.IConfigurationSectionHandler" />.</param>
		/// <returns>The ConfigurationSection object.</returns>
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

		internal void OnRemovedFromCache(EventArgs e) {
			this.Dispose();
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
				if(this.watcher != null)
					this.watcher.Dispose();

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
