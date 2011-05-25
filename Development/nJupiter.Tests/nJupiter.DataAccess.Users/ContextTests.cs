using System;

using nJupiter.DataAccess.Users;

using NUnit.Framework;

namespace nJupiter.Tests.DataAccess.Users {

	[TestFixture]
	public class ContextTests {

		[Test]
		public void Constructor_PassingNull_ThrowsArgumentException() {
			Assert.Throws<ArgumentException>(() => new Context(null));
		}

		[Test]
		public void Constructor_PassingEmptyString_ThrowsArgumentException() {
			Assert.Throws<ArgumentException>(() => new Context(null));
		}

		[Test]
		public void Equals_TwoContextWithSameName_AreEqual() {
			var context1 = new Context("contextname");
			var context2 = new Context("contextname");
			Assert.True(context1.Equals(context2));
		}

		[Test]
		public void Equals_TwoContextWithDifferentName_AreNotEqual() {
			var context1 = new Context("contextname1");
			var context2 = new Context("contextname2");
			Assert.False(context1.Equals(context2));
		}

		[Test]
		public void Equals_ComparingContextWithNonContext_AreNotEqual() {
			var context = new Context("contextname");
			Assert.False(context.Equals(true));
		}



	}
}
