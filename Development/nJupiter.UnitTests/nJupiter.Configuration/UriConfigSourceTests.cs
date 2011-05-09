using System;

using nJupiter.Configuration;

using NUnit.Framework;

namespace nJupiter.UnitTests.Configuration {
	
	[TestFixture]
	public class UriConfigSourceTests {
		private const string uri = "http://www.test.org/";

		[Test]
		public void UriConfigSource_CreateSourceWithUri_ReturnsConfigConfigUri() {
			var configSource = new UriConfigSource(new Uri(uri));
			Assert.AreEqual(uri, configSource.ConfigUrl.OriginalString);
		}

	}
}
