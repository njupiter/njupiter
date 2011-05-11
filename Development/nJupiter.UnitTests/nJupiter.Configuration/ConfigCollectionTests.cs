using System.Xml;

using nJupiter.Configuration;

using NUnit.Framework;

namespace nJupiter.UnitTests.Configuration {
	
	[TestFixture]
	public class ConfigCollectionTests {

		[Test]
		public void Insert_InsertConfig_CollectionContainsConfig() {
			var configs = new ConfigCollection();
			var config = new Config("testConfig", GetConfigXmlDocument(@"<test />"));
			configs.Insert(config);
			Assert.IsTrue(configs.Contains(config));
		}

		[Test]
		public void Insert_InsertConfigAndDiscardConfig_CollectionDoesNotContainConfig() {
			var configs = new ConfigCollection();
			var config = new Config("testConfig", GetConfigXmlDocument(@"<test />"));
			configs.Insert(config);
			config.Discard();
			Assert.IsFalse(configs.Contains(config));
		}

		[Test]
		public void Insert_InsertConfigThatIsDiscarded_ThrowsConfigDiscardedException() {
			var configs = new ConfigCollection();
			var config = new Config("testConfig", GetConfigXmlDocument(@"<test />"));
			config.Discard();
			Assert.Throws<ConfigDiscardedException>(() => configs.Insert(config));
		}

		private static XmlElement GetConfigXmlDocument(string innerXml) {
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(string.Format(@"<configuration>{0}</configuration>", innerXml));
			return xmlDocument.DocumentElement;
		}

	}
}
