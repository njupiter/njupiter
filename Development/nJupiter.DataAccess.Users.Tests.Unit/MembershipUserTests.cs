using System;

using FakeItEasy;

using nJupiter.DataAccess.Users.Web;

using NUnit.Framework;

namespace nJupiter.DataAccess.Users.Tests.Unit {
	
	[TestFixture]
	public class MembershipUserTests {

		[Test]
		public void UserName_CreateUserWithDomain_CreateUsernameWithDomain() {
			var user = A.Fake<IUser>();
			A.CallTo(() => user.UserName).Returns("username");
			A.CallTo(() => user.Domain).Returns("njupiter");
			var memebershipUser = new MembershipUser(user, "provider");
			Assert.AreEqual("njupiter\\username", memebershipUser.UserName);
		}

		[Test]
		public void UserName_CreateUserWithDomain_CreateUsernameWithoutDomain() {
			var user = A.Fake<IUser>();
			A.CallTo(() => user.UserName).Returns("username");
			var memebershipUser = new MembershipUser(user, "provider");
			Assert.AreEqual("username", memebershipUser.UserName);
		}

		[Test]
		public void UserName_CreateUserWithId_ProviderUserKeySameAsUserId() {
			var user = A.Fake<IUser>();
			A.CallTo(() => user.Id).Returns("userid");
			var memebershipUser = new MembershipUser(user, "provider");
			Assert.AreEqual("userid", memebershipUser.ProviderUserKey);
		}


		[Test]
		public void Properties_CreateUser_CheckAllProperties() {
			var user = A.Fake<IUser>();
			A.CallTo(() => user.Id).Returns("userid");

			A.CallTo(() => user.Properties.CreationDate).Returns(DateTime.Now);
			A.CallTo(() => user.Properties.Locked).Returns(true);
			A.CallTo(() => user.Properties.LastLockoutDate).Returns(DateTime.Now);
			A.CallTo(() => user.Properties.LastPasswordChangedDate).Returns(DateTime.Now);
			A.CallTo(() => user.Properties.PasswordQuestion).Returns("What is the meaning of life?");
			A.CallTo(() => user.Properties.LastLoginDate).Returns(DateTime.Now);
			A.CallTo(() => user.Properties.Description).Returns("Dummy user");
			A.CallTo(() => user.Properties.Email).Returns("email@test.org");
			A.CallTo(() => user.Properties.Active).Returns(true);
			A.CallTo(() => user.Properties.LastActivityDate).Returns(DateTime.Now);


			var memebershipUser = new MembershipUser(user, "provider");
			Assert.AreEqual("provider", memebershipUser.ProviderName);
			Assert.AreEqual(user.Properties.CreationDate, memebershipUser.CreationDate);
			Assert.AreEqual(user.Properties.Locked, memebershipUser.IsLockedOut);
			Assert.AreEqual(user.Properties.LastLockoutDate, memebershipUser.LastLockoutDate);
			Assert.AreEqual(user.Properties.LastPasswordChangedDate, memebershipUser.LastPasswordChangedDate);
			Assert.AreEqual(user.Properties.PasswordQuestion, memebershipUser.PasswordQuestion);
			Assert.AreEqual(user.Properties.LastLoginDate, memebershipUser.LastLoginDate);
			Assert.AreEqual(user.Properties.Description, memebershipUser.Comment);
			Assert.AreEqual(user.Properties.Email, memebershipUser.Email);
			Assert.AreEqual(user.Properties.Active, memebershipUser.IsApproved);
			Assert.AreEqual(user.Properties.LastActivityDate, memebershipUser.LastActivityDate);


		}


	}
}
