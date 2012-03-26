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
using System.Collections;
using System.Reflection;
using System.Xml;
using System.Web;
using System.IO;

using log4net;

namespace nJupiter.Configuration {

	/// <summary>
	/// The configuration handler class that server configurations
	/// </summary>
	public sealed class ConfigHandler {
		#region Constants
		private const string ConfigElement = "nJupiterConfiguration";
		private const string ConfigSystemKey = "System";
		private const string ConfigSuffix = ".config";
		#endregion

		#region Static Members
		private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		private static readonly object PadLock = new object();
		private static readonly char[] IllegalPathCharacters = new[] { '\\', '/', '"', '*', '?', '<', '>' };
		#endregion

		#region Members
		private bool allConfigLoaded;
		private readonly ConfigCollection configurations = new ConfigCollection();
		private readonly ArrayList configDirs = new ArrayList();
		#endregion

		#region Properties
		/// <summary>
		/// Gets the config key used for SystemConfig.
		/// </summary>
		/// <value>The system config key.</value>
		public static string SystemConfigKey { get { return ConfigSystemKey; } }
		/// <summary>
		/// Gets all configuration objects.
		/// </summary>
		/// <value>The configurations.</value>
		public static ConfigCollection Configurations {
			get {
				ConfigHandler.Instance.LoadAllConfigs();
				return ConfigHandler.Instance.configurations;
			}
		}
		#endregion

		#region Singleton Implementation
		private ConfigHandler() { }

		/// <summary>
		/// Returns a ConfigHandler instance
		/// </summary>
		internal static ConfigHandler Instance {
			get {
				return NestedSingleton.instance;
			}
		}

		// thread safe Singleton implementation with fully lazy instantiation and with full performance
		private sealed class NestedSingleton {
			// Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
			// ReSharper disable EmptyConstructor
			static NestedSingleton() {
				// ReSharper restore EmptyConstructor
			}
			internal static readonly ConfigHandler instance = new ConfigHandler();
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Gets the system config object.
		/// </summary>
		/// <returns>The system config object</returns>
		/// <exception cref="ConfigurationException">The config does not exist.</exception>
		public static Config GetSystemConfig() {
			return GetConfig(SystemConfigKey);
		}

		/// <summary>
		/// Gets the config object for the current assembly calling the config handler.
		/// </summary>
		/// <returns>The config object for the current assembly.</returns>
		/// <exception cref="ConfigurationException">The config does not exist.</exception>
		public static Config GetConfig() {
			return GetConfig(Assembly.GetCallingAssembly(), false);
		}

		/// <summary>
		/// Gets the config object for the current assembly calling the config handler.
		/// </summary>
		/// <param name="suppressMissingConfigException">if set to <c>true</c> suppress exception if config for the calling assembly is missing.</param>
		/// <returns>The config object for the current assembly.</returns>
		/// <exception cref="ConfigurationException">The config does not exist.</exception>
		public static Config GetConfig(bool suppressMissingConfigException) {
			return GetConfig(Assembly.GetCallingAssembly(), suppressMissingConfigException);
		}

		/// <summary>
		/// Gets the config object for a given assembly.
		/// </summary>
		/// <param name="assembly">The assembly.</param>
		/// <returns>A config object for the given assembly.</returns>
		/// <exception cref="ConfigurationException">The config does not exist.</exception>
		public static Config GetConfig(Assembly assembly) {
			return GetConfig(assembly, false);
		}

		/// <summary>
		/// Gets the config object for a given assembly.
		/// </summary>
		/// <param name="assembly">The assembly.</param>
		/// <param name="suppressMissingConfigException">if set to <c>true</c> suppress exception if config for the given assembly is missing.</param>
		/// <returns>A config object for the given assembly.</returns>
		/// <exception cref="ConfigurationException">The config does not exist.</exception>
		public static Config GetConfig(Assembly assembly, bool suppressMissingConfigException) {
			if(assembly == null)
				throw new ArgumentNullException("assembly");
			Config config = GetConfig(assembly.GetName().Name, suppressMissingConfigException);
			if(!suppressMissingConfigException && config == null)
				throw new ConfigurationException(string.Format("The config file for the assembly [{0}] was not found.", assembly.GetName().Name));
			return config;
		}

		/// <summary>
		///  Gets the config object with a given config key.
		/// </summary>
		/// <param name="configKey">The config key.</param>
		/// <returns>A config object with the given config key.</returns>
		/// <exception cref="ConfigurationException">The config does not exist.</exception>
		public static Config GetConfig(string configKey) {
			return GetConfig(configKey, false);
		}

		/// <summary>
		///  Gets the config object with a given config key.
		/// </summary>
		/// <param name="configKey">The config key.</param>
		/// <param name="suppressMissingConfigException">if set to <c>true</c> suppress exception if config for the given assembly is missing.</param>
		/// <returns>A config object with the given config key.</returns>
		/// <exception cref="ConfigurationException">The config does not exist.</exception>
		public static Config GetConfig(string configKey, bool suppressMissingConfigException) {
			if(ConfigHandler.Instance.configurations.Contains(configKey))
				return ConfigHandler.Instance.configurations[configKey];

			try {
				return ConfigHandler.Instance.LoadConfig(configKey);
			} catch(ConfigurationException) {
				if(suppressMissingConfigException)
					return null;
				throw;
			}
		}
		#endregion

		#region Private and Internal Methods
		private void LoadAllConfigs() {
			if(this.allConfigLoaded)
				return;
			lock(PadLock) {
				if(!this.allConfigLoaded) {
					foreach(DirectoryInfo dir in this.configDirs) {
						if(dir.Exists) {
							FileInfo[] files = dir.GetFiles("*" + ConfigSuffix);
							foreach(FileInfo file in files) {
								string configKey = file.Name.Substring(0, file.Name.Length - ConfigSuffix.Length);
								LoadConfig(configKey);
							}
						}
					}
					this.allConfigLoaded = true;
				}
			}
		}

		private Config LoadConfig(string configKey) {
			if(this.configurations.Contains(configKey))
				return this.configurations[configKey];

			lock(PadLock) {
				if(Log.IsDebugEnabled) { Log.Debug(string.Format("Loding config with key [{0}]", configKey)); }

				if(!this.configurations.Contains(this.GetType().Assembly.GetName().Name))
					this.Configure();

				if(configKey.IndexOfAny(IllegalPathCharacters) < 0) {
					foreach(DirectoryInfo dir in this.configDirs) {
						if(dir.Exists) {
							try {
								FileInfo[] files = dir.GetFiles(configKey + ConfigSuffix);
								if(files.Length > 0) {
									Configurator.Configure(configKey, files[0]);
									break;
								}
							} catch(IOException ex) {
								// Ignore IOException in case of incorrect syntax
								if(Log.IsDebugEnabled && !configKey.Contains(":")) { Log.Debug(string.Format("Incorrect syntax in config key [{0}]", configKey), ex); }
							}
						}
					}
				} else {
					if(Log.IsDebugEnabled) { Log.Debug(string.Format("Illegal path characters in config key [{0}]", configKey)); }
				}
				if(this.configurations.Contains(configKey)) {
					return this.configurations[configKey];
				}
				return null;
			}
		}

		internal static void SetConfig(Config config) {
			if(config.ConfigFile != null) {
				try {
					// Create a watch handler that will reload the configuration whenever the config file is modified.
					if(Log.IsDebugEnabled) { Log.Debug(string.Format("Start watching file {0} for config [{1}]", config.ConfigFile.FullName, config.ConfigKey)); }
					config.Watcher = WatchedConfigHandler.StartWatching(config.ConfigKey, config.ConfigFile);
				} catch(Exception ex) {
					throw new ConfiguratorException(string.Format("Failed to initialize configuration file watcher for file [{0}].", config.ConfigFile.FullName), ex);
				}
			}

			lock(ConfigHandler.Instance.configurations.SyncRoot) {
				if(ConfigHandler.Instance.configurations.Contains(config.ConfigKey)) {
					ConfigHandler.Instance.configurations[config.ConfigKey] = config;
				} else {
					ConfigHandler.Instance.configurations.Add(config);
				}
			}
		}

		#endregion

		#region Handler Configuration
		/// <summary>
		/// Configure the handler iteself
		/// </summary>
		private void Configure() {
			try {
				XmlElement configElement = System.Configuration.ConfigurationManager.GetSection(ConfigElement) as XmlElement;
				if(configElement == null) {
					// Failed to load the xml config using configuration settings handler
					throw new ConfiguratorException(string.Format("Failed to find configuration section '{0}' in the application's .config file. Check your .config file for the <{1}> and <configSections> elements. The configuration section should look like: <section name=\"{2}\" type=\"nJupiter.Configuration.nJupiterConfigurationSectionHandler,nJupiter.Configuration\" />", ConfigElement, ConfigElement, ConfigElement));
				}
				// Configure using the xml loaded from the config file
				Configurator.Configure(this.GetType().Assembly, configElement);
				// Configure using the 'elementName' element
				XmlNodeList configNodeList = ConfigHandler.GetConfig().ConfigXML.GetElementsByTagName(ConfigElement);
				if(configNodeList.Count > 1) {
					throw new ConfiguratorException(string.Format("XML configuration contains [{0}] <{1}> elements. Only one is allowed. Configuration Aborted.", configNodeList.Count, ConfigElement));
				}
				this.Init();
			} catch(System.Configuration.ConfigurationException confEx) {
				string configFile = AppDomain.CurrentDomain.GetData("APP_CONFIG_FILE").ToString();
				if(confEx.BareMessage.IndexOf("Unrecognized element") >= 0) {
					// Looks like the XML file is not valid
					throw new ConfiguratorException(string.Format("Failed to parse config file [{0}]. Check your .config file is well formed XML.", configFile), confEx);
				}
				// This exception is typically due to the assembly name not being correctly specified in the section type.
				string configSectionStr = string.Format("<section name=\"{0}\" type=\"nJupiter.Configuration.nJupiterConfigurationSectionHandler,{1}\" />", ConfigElement, Assembly.GetExecutingAssembly().FullName);
				throw new ConfiguratorException(string.Format("Failed to parse config file [{0}]. Is the <configSections> specified as: {1}", configFile, configSectionStr), confEx);
			}
		}

		private void Init() {
			// TODO: Do redirects, specifed config files etc.

			// Read config dirs
			string[] dirs = ConfigHandler.GetConfig().GetValueArray("configDirectories", "configDirectory");
			foreach(string dir in dirs) {
				if(!string.IsNullOrEmpty(dir)) {
					string directory = dir;
					if(HttpContext.Current != null && dir.StartsWith("~")) {
						directory = HttpContext.Current.Server.MapPath(dir);
					}
					DirectoryInfo dirInfo = new DirectoryInfo(directory);
					if(dirInfo.Exists) {
						this.configDirs.Add(dirInfo);
					}
				}
			}
		}
		#endregion
	}
}
