using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml;

using NUnit.Framework;

namespace nJupiter.Configuration.Tests.Integration {
	
	[TestFixture]
	public class ConfigRepositoryTests {

		[Test]
		public void GetAppConfig_CreateConfigRepositoryWithDefaultValuesAndLoadAppConfig_ReturnsConfigWithCorrectAppConfigKey() {
			var configLoader = new FakeLoader(false);
			var configRepository = new ConfigRepository(configLoader);
			IConfig config = configRepository.GetAppConfig();
			Assert.AreEqual("nJupiter.Configuration.Tests.Integration.dll", config.ConfigKey);
		}

		[Test]
		public void GetConfig_CanLoadConfigsFromMultipleThreads() {

			const string configKey = "MyCustomConfig";
			var configLoader = new FakeLoader(false, true);
			var configRepository = new ConfigRepository(configLoader);

			const int maxThreads = 10;

			Exception ex = null;
			IConfig config = null;

			var getConfigCompletedEvent = new ManualResetEvent(false);
			for(int i = 0; i < maxThreads; i++) {
				int remainingThreads = i;
				ThreadPool.QueueUserWorkItem(s => {
					try {
						config = configRepository.GetConfig(configKey, false);
						if(Interlocked.Decrement(ref remainingThreads) == 0) {
							getConfigCompletedEvent.Set();
						}
					} catch(Exception innerEx) {
						getConfigCompletedEvent.Set();
						ex = innerEx;
						throw;
					}
				});
			}
			getConfigCompletedEvent.WaitOne();
			getConfigCompletedEvent.Close();
			Assert.IsNotNull(config);
			Assert.IsNull(ex);
			
		}

		class FakeLoader : IConfigLoader {
			private readonly bool configDoesNotExist;
			private bool sleepOnLoad;

			public FakeLoader(bool configDoesNotExist) {
				this.configDoesNotExist = configDoesNotExist;
			}

			public FakeLoader(bool configDoesNotExist, bool sleepOnLoad) : this(configDoesNotExist) {
				this.sleepOnLoad = sleepOnLoad;
			}


			private readonly List<string> configKeys = new List<string>();

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
				if(sleepOnLoad) {
					Thread.Sleep(250);
					sleepOnLoad = false;
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
