using System.IO;

using nJupiter.Configuration;

using NUnit.Framework;

namespace nJupiter.UnitTests.Configuration {

	[TestFixture]
	public class FileConfigSourceTests {
		private const string filepath = "c:\\dummyfile.txt";

		[Test]
		public void FileConfigSource_CreateSourceWithFile_ReturnsConfigConfigFileAndUri() {
			var file = new FileInfo(filepath);
			var configSource = new FileConfigSource(file);
			
			Assert.AreEqual(file, configSource.ConfigFile);
			Assert.AreEqual(filepath, configSource.ConfigUrl.OriginalString);
		}

	}
}
