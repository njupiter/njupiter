using System;

using FakeItEasy;

using nJupiter.DataAccess.Users;

using NUnit.Framework;

namespace nJupiter.Tests.nJupiter.DataAccess.Users {
	
	[TestFixture]
	public class UserTests {

		[Test]
		public void Constructor_SetUsername_ReturnsUserWithCorrectUsername() {
			var properties = A.Fake<IPropertyCollection>();
			var user = new User("userid", "username", "userdomain", properties, null);
			Assert.AreEqual("username", user.UserName);
		}

		[Test]
		public void Constructor_SetUserId_ReturnsUserWithCorrectUserId() {
			var properties = A.Fake<IPropertyCollection>();
			var user = new User("userid", "username", "userdomain", properties, null);
			Assert.AreEqual("userid", user.Id);
		}

		[Test]
		public void Constructor_SetUserdomain_ReturnsUserWithCorrectUserdomain() {
			var properties = A.Fake<IPropertyCollection>();
			var user = new User("userid", "username", "userdomain", properties, null);
			Assert.AreEqual("userdomain", user.Domain);
		}

		[Test]
		public void Constructor_SetUserdomainToNull_ReturnsUserWithEmptyUserdomain() {
			var properties = A.Fake<IPropertyCollection>();
			var user = new User("userid", "username", null, properties, null);
			Assert.AreEqual(string.Empty, user.Domain);
		}

		[Test]
		public void Constructor_SetDefaultProperties_ReturnsUserWithCorrectProperties() {
			var properties = A.Fake<IPropertyCollection>();
			var user = new User("userid", "username", "userdomain", properties, null);
			Assert.AreEqual(properties, user.Properties.GetProperties());
		}

		[Test]
		public void Constructor_PassingEmptyUsername_ThrowsArgumentException() {
			var properties = A.Fake<IPropertyCollection>();
			Assert.Throws<ArgumentException>(() => new User("userid", string.Empty, "userdomain", properties, null));
			Assert.Throws<ArgumentException>(() => new User("userid", null, "userdomain", properties, null));
		}

		[Test]
		public void Constructor_PassingNullUserid_ThrowsArgumentNullException() {
			var properties = A.Fake<IPropertyCollection>();
			Assert.Throws<ArgumentNullException>(() => new User(null, "username", "userdomain", properties, null));
		}


		[Test]
		public void Equals_TwoUsersWithSameId_AreEqual() {
			var properties = A.Fake<IPropertyCollection>();
			var user1 = new User("userid", "username1", "userdomain1", properties, null);
			var user2 = new User("userid", "username2", "userdomain2", properties, null);
			Assert.IsTrue(user1.Equals(user2));
		}

		[Test]
		public void Equals_TwoUsersWithDifferentId_AreNotEqual() {
			var properties = A.Fake<IPropertyCollection>();
			var user1 = new User("userid1", "username", "userdomain", properties, null);
			var user2 = new User("userid2", "username", "userdomain", properties, null);
			Assert.IsFalse(user1.Equals(user2));
		}

		[Test]
		public void MakeReadOnly_MakeReadOnly_UserAndItsPropertyIsSetToReadOnly() {
			var properties = A.Fake<IPropertyCollection>();
			var user = new User("userid", "username", "userdomain", properties, null);
			user.MakeReadOnly();
			A.CallTo(() => properties.MakeReadOnly()).MustHaveHappened();
			Assert.IsTrue(user.IsReadOnly);
		}


		[Test]
		public void Clone_MakeReadOnly_ClonedObjectIsNotReadonly() {
			var properties = A.Fake<IPropertyCollection>();
			var newProperties = A.Fake<IPropertyCollection>();
			A.CallTo(() => properties.Clone()).Returns(newProperties);
			var user = new User("userid", "username", "userdomain", properties, null);
			user.MakeReadOnly();
			var newUser = (IUser)user.Clone();
			Assert.IsTrue(user.IsReadOnly);
			Assert.IsFalse(newUser.IsReadOnly);
		}


		[Test]
		public void Clone_CloneObject_ClonedObjectGetsClonedProperties() {
			var properties = A.Fake<IPropertyCollection>();
			var newProperties = A.Fake<IPropertyCollection>();
			A.CallTo(() => properties.Clone()).Returns(newProperties);
			var user = new User("userid", "username", "userdomain", properties, null);
			var newUser = (IUser)user.Clone();
			Assert.AreEqual(newProperties, newUser.Properties.GetProperties());
		}



	}
}
