using System;
using System.IO;
using FakeItEasy;

using NUnit.Framework;

namespace nJupiter.Configuration.Tests.Integration {

	[TestFixture]
	internal class FileConfigFactoryTest {
		const string systemConfigPath = @"C:\Projects\nJupiter\Development\Shared Resources\Config\System.config";
		const string nonExistingFile = @"C:\Projects\nJupiter\Development\Shared Resources\Config\NonExistingFile.config";
		
		[Test]
		public void CreateWithWatcher_CreateFromPath_ReturnConfigWithWatcher() {
			Assume.That(File.Exists(systemConfigPath));
			var config = FileConfigFactory.CreateWithWatcher(systemConfigPath);
			Assert.IsNotNull(config);
			Assert.IsNotNull(config.ConfigSource);
			Assert.IsNotNull(config.ConfigSource.Watcher);
			Assert.AreEqual("System", config.ConfigKey);
		}

		[Test]
		public void CreateWithWatcher_CreateFromPathWithCustomKey_ReturnConfigWithWatcher() {
			Assume.That(File.Exists(systemConfigPath));
			string key = "myKey";
			var config = FileConfigFactory.CreateWithWatcher(key, systemConfigPath);
			Assert.IsNotNull(config);
			Assert.IsNotNull(config.ConfigSource);
			Assert.IsNotNull(config.ConfigSource.Watcher);
			Assert.AreEqual(key, config.ConfigKey);
		}

		[Test]
		public void CreateWithWatcher_CreateFromFileInfo_ReturnConfigWithWatcher() {
			Assume.That(File.Exists(systemConfigPath));
			var config = FileConfigFactory.CreateWithWatcher(new FileInfo(systemConfigPath));
			Assert.IsNotNull(config);
			Assert.IsNotNull(config.ConfigSource);
			Assert.IsNotNull(config.ConfigSource.Watcher);
			Assert.AreEqual("System", config.ConfigKey);
		}

		[Test]
		public void CreateWithWatcher_CreateFromFileInfoWithCustomKey_ReturnConfigWithWatcher() {
			Assume.That(File.Exists(systemConfigPath));
			const string key = "myKey";
			var config = FileConfigFactory.CreateWithWatcher(key, new FileInfo(systemConfigPath));
			Assert.IsNotNull(config);
			Assert.IsNotNull(config.ConfigSource);
			Assert.IsNotNull(config.ConfigSource.Watcher);
			Assert.AreEqual(key, config.ConfigKey);
		}

		[Test]
		public void CreateWithWatcher_CreateFromPathNonExistingFile_ReturnConfigWithWatcher() {
			Assume.That(!File.Exists(nonExistingFile));
			Assert.Throws<FileNotFoundException>(() => FileConfigFactory.CreateWithWatcher(nonExistingFile));
		}

		[Test]
		public void Create_CreateFromPath_ReturnConfigWithoutWatcher() {
			Assume.That(File.Exists(systemConfigPath));
			var config = FileConfigFactory.Create(systemConfigPath);
			Assert.IsNotNull(config);
			Assert.IsNotNull(config.ConfigSource);
			Assert.IsNull(config.ConfigSource.Watcher);
			Assert.AreEqual("System", config.ConfigKey);
		}

		[Test]
		public void Create_CreateFromPathWithCustomKey_ReturnConfigWithoutWatcher() {
			Assume.That(File.Exists(systemConfigPath));
			string key = "myKey";
			var config = FileConfigFactory.Create(key, systemConfigPath);
			Assert.IsNotNull(config);
			Assert.IsNotNull(config.ConfigSource);
			Assert.IsNull(config.ConfigSource.Watcher);
			Assert.AreEqual(key, config.ConfigKey);
		}

		[Test]
		public void Create_CreateFromFileInfo_ReturnConfigWithoutWatcher() {
			Assume.That(File.Exists(systemConfigPath));
			var config = FileConfigFactory.Create(new FileInfo(systemConfigPath));
			Assert.IsNotNull(config);
			Assert.IsNotNull(config.ConfigSource);
			Assert.IsNull(config.ConfigSource.Watcher);
			Assert.AreEqual("System", config.ConfigKey);
		}

		[Test]
		public void Create_CreateFromFileInfoWithCustomKey_ReturnConfigWithoutWatcher() {
			Assume.That(File.Exists(systemConfigPath));
			const string key = "myKey";
			var config = FileConfigFactory.Create(key, new FileInfo(systemConfigPath));
			Assert.IsNotNull(config);
			Assert.IsNotNull(config.ConfigSource);
			Assert.IsNull(config.ConfigSource.Watcher);
			Assert.AreEqual(key, config.ConfigKey);
		}
		
		
		[Test]
		public void Create_CreateFromPathWithCustomSource_ReturnConfigWithCustomSource() {
			Assume.That(File.Exists(systemConfigPath));
			const string key = "myKey";
			var source = A.Fake<IConfigSource>();
			var config = FileConfigFactory.Create(key, systemConfigPath, source);
			Assert.IsNotNull(config);
			Assert.AreEqual(source, config.ConfigSource);
			Assert.AreEqual(key, config.ConfigKey);
		}		
		[Test]
		public void Create_PassingNullFileInfo_ThrowsArgumentNullException() {
			Assert.Throws<ArgumentNullException>(() => FileConfigFactory.Create((FileInfo)null, A.Fake<IConfigSource>()));
		}
		

	}
}
