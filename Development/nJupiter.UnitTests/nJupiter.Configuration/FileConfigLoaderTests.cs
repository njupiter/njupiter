using System;
using System.IO;

using NUnit.Framework;

namespace nJupiter.UnitTests.nJupiter.Configuration {
	[TestFixture]
	public class FileConfigLoaderTests {
		[Test]
		public void Test() {
			Directory.GetDirectories(Environment.CurrentDirectory, "*.config");
		}

	}
}
