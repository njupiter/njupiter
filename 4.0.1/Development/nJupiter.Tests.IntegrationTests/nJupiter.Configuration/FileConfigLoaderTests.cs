using System.Collections.Generic;
using System.IO;
using System.Xml;

using nJupiter.Configuration;

using NUnit.Framework;

namespace nJupiter.Tests.UnitTests.Configuration {
	[TestFixture]
	public class FileConfigLoaderTests {
		const string nJupiterDevPath = @"C:\Projects\nJupiter\Development\";
		static readonly string nJupiterConfigPath = string.Format(@"{0}Shared Resources\Config\", nJupiterDevPath);
		static readonly string configForDevPath  = string.Format(@"<nJupiterConfiguration><configDirectories enableFileWatching=""false"" configSuffix="".targets""><configDirectory value=""{0}""/></configDirectories></nJupiterConfiguration>", nJupiterDevPath);
		static readonly string configForConfigFolder  = string.Format(@"<nJupiterConfiguration><configDirectories loadAllConfigFilesOnInit=""true""><configDirectory value=""{0}""/></configDirectories></nJupiterConfiguration>", nJupiterConfigPath);

		[Test]
		public void LoadAll_LoadnjupiterConfigFolder_ReturnCorrectNumberOfConfigs() {
			Assume.That(Directory.Exists(nJupiterConfigPath));

			var c = new Config("testconfig", GetConfigXmlDocument(configForConfigFolder));
			var fileConfigLoader  = new FileConfigLoader(c);
			var configs = fileConfigLoader.LoadOnInit();
			int count = 0;
			foreach(IConfig config in configs) {
				count++;
			}
			int acctualCount = Directory.GetFiles(nJupiterConfigPath, "*.config").Length;
			Assert.AreEqual(acctualCount, count);
		}

		[Test]
		public void LoadAll_LoadAll_ThrowsKeyNotFoundExceptionOnNonExistingConfig() {
			Assume.That(Directory.Exists(nJupiterConfigPath));
			Assume.That(!File.Exists(nJupiterConfigPath + "NotExistingConfigFile.config"));

			var c = new Config("testconfig", GetConfigXmlDocument(configForConfigFolder));
			var fileConfigLoader  = new FileConfigLoader(c);
			var configs = fileConfigLoader.LoadOnInit();
			
			Assert.Throws<KeyNotFoundException>(() => configs["NotExistingConfigFile"].ToString());
		}

		[Test]
		public void Load_LoadSystemConfigFromnjupiterConfigFolder_ReturnSystemInfo() {
			Assume.That(File.Exists(nJupiterConfigPath + "System.config"));

			var c = new Config("testconfig", GetConfigXmlDocument(configForConfigFolder));
			var fileConfigLoader  = new FileConfigLoader(c);
			var config = fileConfigLoader.Load("System");

			Assert.NotNull(config);
		}

		[Test]
		public void Load_HasConfigToLoadConfigsWithSuffixTargets_LoadnJupiterDeploymentTargets() {
			Assume.That(File.Exists(nJupiterDevPath + "nJupiter.Deployment.targets"));

			var c = new Config("testconfig", GetConfigXmlDocument(configForDevPath));
			var fileConfigLoader  = new FileConfigLoader(c);
			var config = fileConfigLoader.Load("nJupiter.Deployment");

			Assert.NotNull(config);
		}

		[Test]
		public void Load_LoadConfigWithIlligalCharacters_ReturnsNull() {
			Assume.That(Directory.Exists(nJupiterConfigPath));

			var c = new Config("testconfig", GetConfigXmlDocument(configForConfigFolder));
			var fileConfigLoader  = new FileConfigLoader(c);
			var config = fileConfigLoader.Load("??");

			Assert.IsNull(config);
		}

		[Test]
		public void OpenFile_TryOpenNonExistingFile_ThrowsConfiguratorException() {
			Assume.That(!File.Exists("NoExistingFile.config"));
			Assert.Throws<ConfiguratorException>(() => FileConfigLoader.OpenFile(new FileInfo("NoExistingFile.config")));
		}

		private static XmlElement GetConfigXmlDocument(string xml) {
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(xml);
			return xmlDocument.DocumentElement;
		}

	}
}
