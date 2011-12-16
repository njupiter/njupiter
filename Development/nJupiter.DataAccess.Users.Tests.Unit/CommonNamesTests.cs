using System;

using NUnit.Framework;

namespace nJupiter.DataAccess.Users.Tests.Unit {

	[TestFixture]
	public class CommonNamesTests {

		[Test]
		public void GetName_NoSupportedName_TrhowsNotSupportedException() {
			var commonNames = DummyFactory.GetDummyCommonNames();
			Assert.Throws<NotSupportedException>(() => commonNames.GetName("NonExistantName"));
		}
	}
}
