using System.IO;
using System.Threading;

using nJupiter.Configuration;

using NUnit.Framework;

namespace nJupiter.Tests.UnitTests.Configuration {
	
	[TestFixture]
	public class FileConfigSourceWatcherTests {
		private string filePath = @"nJupiter.Configuration.FileConfigSourceWatcherTests.TestFile.config";

        [SetUp]
        public void SetUpFixture() {
            if (File.Exists(filePath)) {
                File.Delete(filePath);
            }
		}        
		
		[TearDown]
        public void TearDown() {
            if (File.Exists(filePath)) {
                File.Delete(filePath);
            }
			Assert.IsFalse(File.Exists(filePath));
        }


		[Test]
		public void CreateWatchedConfig_AddConfigToCollection_ConfigRemovedFromCollectionWhenFileChanged() {
			var configs = new ConfigCollection();
			File.WriteAllText(filePath, @"<configuration><test value=""Hello World"" /></configuration>");
			IConfig config1 = FileConfigFactory.CreateWithWatcher("myCustomKey", filePath);
			configs.Add(config1);
			Assert.AreEqual("Hello World", configs["myCustomKey"].GetValue("test"));
			File.WriteAllText(filePath, @"<configuration><test value=""Hello Universe"" /></configuration>");

			for(int retries = 100; retries >= 0; retries--) {
				if(config1.IsDiscarded) {
					break;
				}
				Thread.Sleep(10);
			}


			Assert.IsTrue(config1.IsDiscarded, "config not discarded after 1 second");

			Assert.IsFalse(configs.Contains("myCustomKey"));
			IConfig config2 = FileConfigFactory.Create("myCustomKey", filePath);
			Assert.AreEqual("Hello Universe", config2.GetValue("test"));
		}
	}
}
