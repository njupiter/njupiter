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
using System.Reflection;

namespace nJupiter.Configuration {
	public class ConfigHandler : IConfigHandler {

		private readonly ConfigCollection configurations;
		private readonly IConfigLoader configLoader;
		private readonly string systemConfigKey;
		private readonly string appConfigKey;

		public string SystemConfigKey { get { return systemConfigKey; } }
		public string AppConfigKey { get { return this.appConfigKey; } }

		/// <summary>
		/// Returns the default instance of IConfigHandler
		/// </summary>
		public static IConfigHandler Instance { get { return NestedSingleton.instance; } }

		public ConfigHandler(IConfigLoader configLoader) : this(configLoader, "System", null) {
			this.appConfigKey = GetAppConfigKey();
		}

		public ConfigHandler(IConfigLoader configLoader, string systemConfigKey, string appConfigKey) {
			this.configLoader = configLoader;
			this.configurations = configLoader.LoadOnInit();
			this.systemConfigKey = systemConfigKey;
			this.appConfigKey = appConfigKey;
		}

		public ConfigCollection Configurations {
			get {
				return this.configurations;
			}
		}

		public IConfig GetSystemConfig() {
			return GetConfig(SystemConfigKey);
		}

		public IConfig GetAppConfig() {
			return GetConfig(AppConfigKey);
		}

		public IConfig GetConfig() {
			return GetConfig(Assembly.GetCallingAssembly(), false);
		}

		public IConfig GetConfig(bool suppressMissingConfigException) {
			return GetConfig(Assembly.GetCallingAssembly(), suppressMissingConfigException);
		}

		public IConfig GetConfig(Assembly assembly) {
			return GetConfig(assembly, false);
		}
		public IConfig GetConfig(Assembly assembly, bool suppressMissingConfigException) {
			if(assembly == null) {
				throw new ArgumentNullException("assembly");
			}
			return GetConfig(assembly.GetName().Name, suppressMissingConfigException);
		}

		public IConfig GetConfig(string configKey) {
			return GetConfig(configKey, false);
		}

		public IConfig GetConfig(string configKey, bool suppressMissingConfigException) {
			if(this.configurations.Contains(configKey)){
				return this.configurations[configKey];
			}
			try {
				IConfig config = this.configLoader.Load(configKey);
				if(config == null) {
					throw new ConfigurationException(string.Format("The config with the config key [{0}] was not found.", configKey));
				}
				this.configurations.Add(config);
				return config;
			} catch(Exception ex) {
				if(suppressMissingConfigException)
					return null;
					throw new ConfigurationException(string.Format("Error loading config file with the config key [{0}].", configKey), ex);				
			}
		}

		private static string GetAppConfigKey() {
			var file = new FileInfo(AppDomain.CurrentDomain.GetData("APP_CONFIG_FILE").ToString());
			return file.Name.Substring(0, file.Name.Length - file.Extension.Length);
		}

		// thread safe Singleton implementation with fully lazy instantiation and with full performance
		private sealed class NestedSingleton {
			// ReSharper disable EmptyConstructor
			static NestedSingleton() {} // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
			// ReSharper restore EmptyConstructor
			internal static readonly IConfigHandler instance = new ConfigHandler(ConfigLoaderFactory.Create());
		}
	}
}

