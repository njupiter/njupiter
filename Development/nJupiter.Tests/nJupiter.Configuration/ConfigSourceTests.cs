using System;
using System.Collections.Generic;
using nJupiter.Configuration;
using NUnit.Framework;

namespace nJupiter.Tests.Configuration {

	[TestFixture]
	public class ConfigSourceTests {
		
		[Test]
		public void Constructor_PassingNullList_ThrowsArgumentNullException() {
			Assert.Throws<ArgumentNullException>(() => new ConfigSource((List<object>)null));
		}		

		[Test]
		public void Constructor_PassingNullObject_ThrowsArgumentNullException() {
			Assert.Throws<ArgumentNullException>(() => new ConfigSource((object)null));
		}		
		
		[Test]
		public void ConfigSource_CreateWithOneFile_ReturnsCorrectFile() {
			var dummyClass = new MyDummyClass1();
			var source = new ConfigSource(dummyClass);
			Assert.AreEqual(dummyClass, source.GetConfigSource<MyDummyClass1>());
		}

		[Test]
		public void ConfigSource_CreateWithTwoFiles_ReturnsCorrectFiles() {
			var sources = new List<object>();
			var dummyClass1 = new MyDummyClass1();
			var dummyClass2 = new MyDummyClass2();
			sources.Add(dummyClass1);
			sources.Add(dummyClass2);

			var source = new ConfigSource(sources);
			Assert.AreEqual(dummyClass1, source.GetConfigSource<MyDummyClass1>());
			Assert.AreEqual(dummyClass2, source.GetConfigSource<MyDummyClass2>());
		}

		public class MyDummyClass1{}
		public class MyDummyClass2{}

	
	}
}
