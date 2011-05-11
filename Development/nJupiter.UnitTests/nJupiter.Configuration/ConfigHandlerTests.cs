using System;
using System.Collections.Generic;
using System.Xml;

using FakeItEasy;

using nJupiter.Configuration;

using NUnit.Framework;

namespace nJupiter.UnitTests.Configuration {
	
	[TestFixture]
	public class ConfigHandlerTests {

		[Test]
		public void Test() {
			var configLoader = new FakeLoader();
			var configHandler = new ConfigHandler(configLoader);
			configHandler.GetAppConfig();
			Assert.AreEqual("App", configLoader.ConfigKeysLoaded[0]);
		}

		class FakeLoader : IConfigLoader {
			private List<string> configKeys = new List<string>();

			public List<string> ConfigKeysLoaded {
				get { return configKeys; }
			}

			public ConfigCollection LoadOnInit() {
				return new ConfigCollection();
			}

			public IConfig Load(string configKey) {
				this.configKeys.Add(configKey);
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
