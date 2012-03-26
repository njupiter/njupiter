using System;
using System.Xml;

using nJupiter.Configuration;

using NUnit.Framework;

namespace nJupiter.Tests.UnitTests.Configuration {
	
	[TestFixture]
	public class ConfigCollectionTests {

		[Test]
		public void Add_InsertConfig_CollectionContainsConfig() {
			var configs = new ConfigCollection();
			var config = new Config("testConfig", GetConfigXmlDocument(@"<test />"));
			configs.Add(config);
			Assert.IsTrue(configs.Contains(config));
		}

		[Test]
		public void Add_InsertConfigAndDiscardConfig_CollectionDoesNotContainConfig() {
			var configs = new ConfigCollection();
			var config = new Config("testConfig", GetConfigXmlDocument(@"<test />"));
			configs.Add(config);
			Assert.IsFalse(config.IsDiscarded);
			config.Discard();
			Assert.IsTrue(config.IsDiscarded);
			Assert.IsFalse(configs.Contains(config));
		}

		[Test]
		public void Add_InsertCofigWithSameKeyTwice_ThrowsArgumentException() {
			var configs = new ConfigCollection();
			var config = new Config("testConfig", GetConfigXmlDocument(@"<test />"));
			configs.Add(config);
			Assert.Throws<ArgumentException>(() => configs.Add(config));
		}

		[Test]
		public void Add_InsertConfigThatIsDiscarded_ThrowsConfigDiscardedException() {
			var configs = new ConfigCollection();
			var config = new Config("testConfig", GetConfigXmlDocument(@"<test />"));
			config.Discard();
			Assert.Throws<ConfigDiscardedException>(() => configs.Add(config));
		}

		private static XmlElement GetConfigXmlDocument(string innerXml) {
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(string.Format(@"<configuration>{0}</configuration>", innerXml));
			return xmlDocument.DocumentElement;
		}

	}
}
