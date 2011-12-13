using System;

using NUnit.Framework;

namespace nJupiter.Tests.UnitTests.DataAccess.Users {

	[TestFixture]
	public class CommonNamesTests {

		[Test]
		public void GetName_NoSupportedName_TrhowsNotSupportedException() {
			var commonNames = DummyFactory.GetDummyCommonNames();
			Assert.Throws<NotSupportedException>(() => commonNames.GetName("NonExistantName"));
		}
	}
}
