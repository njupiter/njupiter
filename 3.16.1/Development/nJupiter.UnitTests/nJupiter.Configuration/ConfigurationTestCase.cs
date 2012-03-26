// NUnit
using NUnit.Framework;

namespace nJupiter.Configuration {

	[TestFixture]
	public class ConfigurationTestCase {
		[TestFixtureSetUp]
		public void Init() {			
			//
		}

		[TestFixtureTearDown]
		public void Dispose() {
			//
		}

		[Test]
		public void ConfigurationTest() {
			ConfigHandler.GetConfig();
		}
	}

}
