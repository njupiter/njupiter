using System;
using System.IO;

using nJupiter.Configuration;

using NUnit.Framework;

namespace nJupiter.Tests.Configuration {

	[TestFixture]
	public class ConfigSourceFactoryTests {

		[Test]
		public void CreateConfigSource_CreateInstanceFromFileInfo_ReturnsFileConfigSource() {
			var file = new FileInfo("c:\\dummyfile.txt");
			var configSource = ConfigSourceFactory.CreateConfigSource(file);
			Assert.IsTrue(configSource.Source is FileInfo);
		}

		[Test]
		public void CreateConfigSource_CreateInstanceFromFileInfoAndAddWatcher_ReturnsFileConfigSourceWithWatcher() {
			var file = new FileInfo("c:\\dummyfile.txt");
			var configSource = ConfigSourceFactory.CreateConfigSource(file, true);
			Assert.IsTrue(configSource.Watcher is FileConfigSourceWatcher);
		}

		[Test]
		public void CreateConfigSource_CreateInstanceFromCustomClass_ReturnsConfigSource() {
			var dummyClass = new MyDummyClass();
			var configSource = ConfigSourceFactory.CreateConfigSource(dummyClass);
			Assert.IsTrue(configSource is ConfigSource);
			Assert.AreEqual(dummyClass, configSource.Source);
		}

		public class MyDummyClass{}


	}
}
