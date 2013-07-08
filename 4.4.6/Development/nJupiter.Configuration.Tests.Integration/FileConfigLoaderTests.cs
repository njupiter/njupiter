using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

using NUnit.Framework;

namespace nJupiter.Configuration.Tests.Unit {
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
			var count = configs.Count();
			var acctualCount = Directory.GetFiles(nJupiterConfigPath, "*.config").Length;
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
			Assume.That(File.Exists(nJupiterDevPath + "nJupiter.CSharp.targets"));

			var c = new Config("testconfig", GetConfigXmlDocument(configForDevPath));
			var fileConfigLoader  = new FileConfigLoader(c);
			var config = fileConfigLoader.Load("nJupiter.CSharp");

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

		private static XmlElement GetConfigXmlDocument(string xml) {
			var xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(xml);
			return xmlDocument.DocumentElement;
		}

	}
}
