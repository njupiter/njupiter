using System;
using System.IO;

using nJupiter.Configuration;

using NUnit.Framework;

namespace nJupiter.UnitTests.Configuration {

	[TestFixture]
	public class ConfigSourceFactoryTests {

		[Test]
		public void CreateConfigSource_CreateInstanceFromFileInfo_ReturnsFileConfigSource() {
			var file = new FileInfo("c:\\dummyfile.txt");
			IConfigSource configSource = ConfigSourceFactory.GetInstance().CreateConfigSource(file);
			Assert.IsTrue(configSource is FileConfigSource);
		}

		[Test]
		public void CreateConfigSource_CreateInstanceFromUri_ReturnsUriConfigSource() {
			var uri = new Uri("http://www.test.org/");
			IConfigSource configSource = ConfigSourceFactory.GetInstance().CreateConfigSource(uri);
			Assert.IsTrue(configSource is UriConfigSource);
		}

		[Test]
		public void CreateConfigSource_CreateInstanceFromCustomClass_ReturnsConfigSource() {
			var dummyClass = new MyDummyClass();
			IConfigSource configSource = ConfigSourceFactory.GetInstance().CreateConfigSource(dummyClass);
			Assert.IsTrue(configSource is ConfigSource);
			Assert.AreEqual(dummyClass, configSource.GetConfigSource<MyDummyClass>());
		}


		public class MyDummyClass{}


	}
}
