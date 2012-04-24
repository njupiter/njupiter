using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

using NUnit.Framework;

namespace nJupiter.Configuration.Tests.Unit {
	
	[TestFixture]
	public class ConfigRepositoryTests {

		[Test]
		public void GetSystemConfig_CreateConfigRepositoryWithDefaultValuesAndLoadSystemConfig_ReturnsConfigWithCorrectSystemConfigKey() {
			var configLoader = new FakeLoader();
			var configRepository = new ConfigRepository(configLoader);
			IConfig config = configRepository.GetSystemConfig();
			Assert.AreEqual("System", config.ConfigKey);
		}

		[Test]
		public void GetAppConfig_CreateConfigRepositoryWithCustomValuesAndLoadAppConfig_ReturnsConfigWithCorrectAppConfigKey() {
			var configLoader = new FakeLoader();
			var configRepository = new ConfigRepository(configLoader, "CustomSystemKey", "CustomAppKey");
			IConfig config = configRepository.GetAppConfig();
			Assert.AreEqual("CustomAppKey", config.ConfigKey);
		}

		[Test]
		public void GetSystemConfig_CreateConfigRepositoryWithCustomValuesAndLoadSystemConfig_ReturnsConfigWithCorrectSystemConfigKey() {
			var configLoader = new FakeLoader();
			var configRepository = new ConfigRepository(configLoader, "CustomSystemKey", "CustomAppKey");
			IConfig config = configRepository.GetSystemConfig();
			Assert.AreEqual("CustomSystemKey", config.ConfigKey);
		}

		[Test]
		public void GetConfig_LoadCurrentConfig_ReturnsConfigWithCorrectSystemConfigKey() {
			var configLoader = new FakeLoader();
			var configRepository = new ConfigRepository(configLoader);
			IConfig config = configRepository.GetConfig();
			Assert.AreEqual("nJupiter.Configuration.Tests.Unit", config.ConfigKey);
		}

		[Test]
		public void GetConfig_LoadCurrentConfigThatDoesNotExist_ThrowsConfigurationException() {
			var configLoader = new FakeLoader(true);
			var configRepository = new ConfigRepository(configLoader);
			Assert.Throws<ConfigurationException>(() => configRepository.GetConfig());
			Assert.AreEqual("nJupiter.Configuration.Tests.Unit", configLoader.ConfigKeysLoaded[0]);
		}


		[Test]
		public void GetConfig_LoadCurrentConfigThatDoesNotExistAndSupressExceptions_ReturnsNull() {
			var configLoader = new FakeLoader(true);
			var configRepository = new ConfigRepository(configLoader);
			IConfig config = configRepository.GetConfig(true);
			Assert.IsNull(config);
			Assert.AreEqual("nJupiter.Configuration.Tests.Unit", configLoader.ConfigKeysLoaded[0]);
		}

		[Test]
		public void GetConfig_LoadConfigForCustomAssembly_ReturnsConfigWithCorrectSystemConfigKey() {
			Assembly assembly = typeof(FakeItEasy.A).Assembly;
			var configLoader = new FakeLoader();
			var configRepository = new ConfigRepository(configLoader);
			IConfig config = configRepository.GetConfig(assembly);
			Assert.AreEqual(assembly.GetName().Name, config.ConfigKey);
		}

		[Test]
		public void GetConfig_LoadConfigForCustomAssemblyDoesNotExist_ThrowsConfigurationException() {
			Assembly assembly = typeof(FakeItEasy.A).Assembly;
			var configLoader = new FakeLoader(true);
			var configRepository = new ConfigRepository(configLoader);
			Assert.Throws<ConfigurationException>(() => configRepository.GetConfig(assembly));
			Assert.AreEqual(assembly.GetName().Name, configLoader.ConfigKeysLoaded[0]);
		}


		[Test]
		public void GetConfig_LoadConfigForCustomAssemblyDoesNotExistAndSupressExceptions_ReturnsNull() {
			Assembly assembly = typeof(FakeItEasy.A).Assembly;
			var configLoader = new FakeLoader(true);
			var configRepository = new ConfigRepository(configLoader);
			IConfig config = configRepository.GetConfig(assembly, true);
			Assert.IsNull(config);
			Assert.AreEqual(assembly.GetName().Name, configLoader.ConfigKeysLoaded[0]);
		}

//
		[Test]
		public void GetConfig_LoadConfigForCustomConfig_ReturnsConfigWithCorrectSystemConfigKey() {
			const string configKey = "MyCustomConfig";
			var configLoader = new FakeLoader();
			var configRepository = new ConfigRepository(configLoader);
			IConfig config = configRepository.GetConfig(configKey);
			Assert.AreEqual(configKey, config.ConfigKey);
		}

		[Test]
		public void GetConfig_LoadConfigForCustomConfigDoesNotExist_ThrowsConfigurationException() {
			const string configKey = "MyCustomConfig";
			var configLoader = new FakeLoader(true);
			var configRepository = new ConfigRepository(configLoader);
			Assert.Throws<ConfigurationException>(() => configRepository.GetConfig(configKey));
			Assert.AreEqual(configKey, configLoader.ConfigKeysLoaded[0]);
		}


		[Test]
		public void GetConfig_LoadConfigForCustomConfigDoesNotExistAndSupressExceptions_ReturnsNull() {
			const string configKey = "MyCustomConfig";
			var configLoader = new FakeLoader(true);
			var configRepository = new ConfigRepository(configLoader);
			IConfig config = configRepository.GetConfig(configKey, true);
			Assert.IsNull(config);
			Assert.AreEqual(configKey, configLoader.ConfigKeysLoaded[0]);
		}
		
		[Test]
		public void GetConfig_GetSameConfigTwice_ReturnsSameConfig() {
			var configLoader = new FakeLoader();
			var configRepository = new ConfigRepository(configLoader);
			IConfig config1 = configRepository.GetConfig("myConfig");
			Assert.IsTrue(configRepository.Configurations.Contains("myConfig"));
			IConfig config2 = configRepository.GetConfig("myConfig");
			Assert.AreEqual(config1, config2);
		}		
		
		
		[Test]
		public void GetConfig_PassingNullAssembly_ThrowsArgumentNullExeption() {
			Assert.Throws<ArgumentNullException>(() => ConfigRepository.Instance.GetConfig((Assembly)null));
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
