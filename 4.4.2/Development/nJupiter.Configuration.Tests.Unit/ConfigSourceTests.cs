using System;

using FakeItEasy;

using NUnit.Framework;

namespace nJupiter.Configuration.Tests.Unit {

	[TestFixture]
	public class ConfigSourceTests {
		
		[Test]
		public void Constructor_PassingNullObject_ThrowsArgumentNullException() {
			Assert.Throws<ArgumentNullException>(() => new ConfigSource(null));
		}

		[Test]
		public void Constructor_PassingOnlyWatcher_ReturnConfigSourceWithBothSourceAndWatcherSetToInput() {
			var watcher = A.Fake<IConfigSourceWatcher>();
			var source = new ConfigSource(watcher);
			Assert.AreEqual(watcher, source.Source);
			Assert.AreEqual(watcher, source.Watcher);
		}
		
		[Test]
		public void ConfigSource_CreateWithOneFile_ReturnsCorrectFile() {
			var dummyClass = new MyDummyClass();
			var source = new ConfigSource(dummyClass);
			Assert.AreEqual(dummyClass, source.Source);
		}

		public class MyDummyClass{}
	
	}
}
