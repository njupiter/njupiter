using System;
using System.Collections.Generic;
using nJupiter.Configuration;
using NUnit.Framework;

namespace nJupiter.Tests.Configuration {

	[TestFixture]
	public class ConfigSourceTests {
		
		[Test]
		public void Constructor_PassingNullObject_ThrowsArgumentNullException() {
			Assert.Throws<ArgumentNullException>(() => new ConfigSource(null));
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
