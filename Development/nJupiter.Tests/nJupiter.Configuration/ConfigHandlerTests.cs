using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using nJupiter.Configuration;
using NUnit.Framework;

namespace nJupiter.Tests.Configuration {
	
	[TestFixture]
	public class ConfigHandlerTests {

		[Test]
		public void GetAppConfig_CreateConfigHandlerWithDefaultValuesAndLoadAppConfig_ReturnsConfigWithCorrectAppConfigKey() {
			var configLoader = new FakeLoader();
			var configHandler = new ConfigHandler(configLoader);
			IConfig config = configHandler.GetAppConfig();
			Assert.AreEqual("nJupiter.Tests.dll", config.ConfigKey);
		}

		[Test]
		public void GetSystemConfig_CreateConfigHandlerWithDefaultValuesAndLoadSystemConfig_ReturnsConfigWithCorrectSystemConfigKey() {
			var configLoader = new FakeLoader();
			var configHandler = new ConfigHandler(configLoader);
			IConfig config = configHandler.GetSystemConfig();
			Assert.AreEqual("System", config.ConfigKey);
		}

		[Test]
		public void GetAppConfig_CreateConfigHandlerWithCustomValuesAndLoadAppConfig_ReturnsConfigWithCorrectAppConfigKey() {
			var configLoader = new FakeLoader();
			var configHandler = new ConfigHandler(configLoader, "CustomSystemKey", "CustomAppKey");
			IConfig config = configHandler.GetAppConfig();
			Assert.AreEqual("CustomAppKey", config.ConfigKey);
		}

		[Test]
		public void GetSystemConfig_CreateConfigHandlerWithCustomValuesAndLoadSystemConfig_ReturnsConfigWithCorrectSystemConfigKey() {
			var configLoader = new FakeLoader();
			var configHandler = new ConfigHandler(configLoader, "CustomSystemKey", "CustomAppKey");
			IConfig config = configHandler.GetSystemConfig();
			Assert.AreEqual("CustomSystemKey", config.ConfigKey);
		}

		[Test]
		public void GetConfig_LoadCurrentConfig_ReturnsConfigWithCorrectSystemConfigKey() {
			var configLoader = new FakeLoader();
			var configHandler = new ConfigHandler(configLoader);
			IConfig config = configHandler.GetConfig();
			Assert.AreEqual("nJupiter.Tests", config.ConfigKey);
		}

		[Test]
		public void GetConfig_LoadCurrentConfigThatDoesNotExist_ThrowsConfigurationException() {
			var configLoader = new FakeLoader(true);
			var configHandler = new ConfigHandler(configLoader);
			Assert.Throws<ConfigurationException>(() => configHandler.GetConfig());
			Assert.AreEqual("nJupiter.Tests", configLoader.ConfigKeysLoaded[0]);
		}


		[Test]
		public void GetConfig_LoadCurrentConfigThatDoesNotExistAndSupressExceptions_ReturnsNull() {
			var configLoader = new FakeLoader(true);
			var configHandler = new ConfigHandler(configLoader);
			IConfig config = configHandler.GetConfig(true);
			Assert.IsNull(config);
			Assert.AreEqual("nJupiter.Tests", configLoader.ConfigKeysLoaded[0]);
		}

		[Test]
		public void GetConfig_LoadConfigForCustomAssembly_ReturnsConfigWithCorrectSystemConfigKey() {
			Assembly assembly = typeof(FakeItEasy.A).Assembly;
			var configLoader = new FakeLoader();
			var configHandler = new ConfigHandler(configLoader);
			IConfig config = configHandler.GetConfig(assembly);
			Assert.AreEqual(assembly.GetName().Name, config.ConfigKey);
		}

		[Test]
		public void GetConfig_LoadConfigForCustomAssemblyDoesNotExist_ThrowsConfigurationException() {
			Assembly assembly = typeof(FakeItEasy.A).Assembly;
			var configLoader = new FakeLoader(true);
			var configHandler = new ConfigHandler(configLoader);
			Assert.Throws<ConfigurationException>(() => configHandler.GetConfig(assembly));
			Assert.AreEqual(assembly.GetName().Name, configLoader.ConfigKeysLoaded[0]);
		}


		[Test]
		public void GetConfig_LoadConfigForCustomAssemblyDoesNotExistAndSupressExceptions_ReturnsNull() {
			Assembly assembly = typeof(FakeItEasy.A).Assembly;
			var configLoader = new FakeLoader(true);
			var configHandler = new ConfigHandler(configLoader);
			IConfig config = configHandler.GetConfig(assembly, true);
			Assert.IsNull(config);
			Assert.AreEqual(assembly.GetName().Name, configLoader.ConfigKeysLoaded[0]);
		}

//
		[Test]
		public void GetConfig_LoadConfigForCustomConfig_ReturnsConfigWithCorrectSystemConfigKey() {
			const string configKey = "MyCustomConfig";
			var configLoader = new FakeLoader();
			var configHandler = new ConfigHandler(configLoader);
			IConfig config = configHandler.GetConfig(configKey);
			Assert.AreEqual(configKey, config.ConfigKey);
		}

		[Test]
		public void GetConfig_LoadConfigForCustomConfigDoesNotExist_ThrowsConfigurationException() {
			const string configKey = "MyCustomConfig";
			var configLoader = new FakeLoader(true);
			var configHandler = new ConfigHandler(configLoader);
			Assert.Throws<ConfigurationException>(() => configHandler.GetConfig(configKey));
			Assert.AreEqual(configKey, configLoader.ConfigKeysLoaded[0]);
		}


		[Test]
		public void GetConfig_LoadConfigForCustomConfigDoesNotExistAndSupressExceptions_ReturnsNull() {
			const string configKey = "MyCustomConfig";
			var configLoader = new FakeLoader(true);
			var configHandler = new ConfigHandler(configLoader);
			IConfig config = configHandler.GetConfig(configKey, true);
			Assert.IsNull(config);
			Assert.AreEqual(configKey, configLoader.ConfigKeysLoaded[0]);
		}
		
		[Test]
		public void GetConfig_GetSameConfigTwice_ReturnsSameConfig() {
			var configLoader = new FakeLoader();
			var configHandler = new ConfigHandler(configLoader);
			IConfig config1 = configHandler.GetConfig("myConfig");
			Assert.IsTrue(configHandler.Configurations.Contains("myConfig"));
			IConfig config2 = configHandler.GetConfig("myConfig");
			Assert.AreEqual(config1, config2);
		}		
		
		
		[Test]
		public void GetConfig_PassingNullAssembly_ThrowsArgumentNullExeption() {
			Assert.Throws<ArgumentNullException>(() => ConfigHandler.Instance.GetConfig((Assembly)null));
		}		

		class FakeLoader : IConfigLoader {
			private readonly bool configDoesNotExist;

			public FakeLoader() {}

			public FakeLoader(bool configDoesNotExist) {
				this.configDoesNotExist = configDoesNotExist;
			}

			private readonly List<string> configKeys = new List<string>();

			public List<string> ConfigKeysLoaded {
				get { return configKeys; }
			}

			public ConfigCollection LoadOnInit() {
				return new ConfigCollection();
			}

			public void InitializeCollection(ConfigCollection configs) {
			}

			public IConfig Load(string configKey) {
				this.configKeys.Add(configKey);
				if(configDoesNotExist) {
					return null;
				}
				return new Config(configKey, GetConfigXmlDocument("<test />"));
			}
		}

		private static XmlElement GetConfigXmlDocument(string innerXml) {
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(string.Format(@"<configuration>{0}</configuration>", innerXml));
			return xmlDocument.DocumentElement;
		}
	}
}
