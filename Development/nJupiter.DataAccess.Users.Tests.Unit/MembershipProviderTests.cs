using System;
using System.Collections.Specialized;
using System.Web.Security;

using FakeItEasy;

using nJupiter.DataAccess.Users;

using NUnit.Framework;

using MembershipProvider = nJupiter.DataAccess.Users.Web.MembershipProvider;

namespace nJupiter.DataAccess.Users.Tests.Unit {
	
	[TestFixture]
	public class MembershipProviderTests {

		[Test]
		public void ChangePassword_PasswordWithNonAlphaCharacters_ThrowsArgumentException() {
			var userRepository = A.Fake<IUserRepository>();
			var provider = GetProvider(userRepository);
			Assert.Throws<ArgumentException>(() => provider.ChangePassword("modhelius", "oldpassword", "newpassword"));
		}

		[Test]
		public void ChangePassword_PasswordLenghtToShort_ThrowsArgumentException() {
			var userRepository = A.Fake<IUserRepository>();
			var provider = GetProvider(userRepository);
			Assert.Throws<ArgumentException>(() => provider.ChangePassword("modhelius", "oldpassword", "a"));
		}

		[Test]
		public void ChangePassword_PasswordNonConfirmWithRegEx_ThrowsArgumentException() {
			var userRepository = A.Fake<IUserRepository>();
			var config = new NameValueCollection();
			config.Add("passwordStrengthRegularExpression", "password123\\.");
			var provider = GetProvider(userRepository, config);
			Assert.Throws<ArgumentException>(() => provider.ChangePassword("modhelius", "oldpassword", "password321;"));
		}

		[Test]
		public void ChangePassword_PasswordSet_SetPasswordAndSaveUserCalled() {
			var userRepository = A.Fake<IUserRepository>();
			var provider = GetProvider(userRepository);

			var originalUser = A.Fake<IUser>();
			var clonedUser = A.Fake<IUser>();

			A.CallTo(() => userRepository.GetUserByUserName("modhelius", null)).Returns(originalUser);
			A.CallTo(() => originalUser.IsReadOnly).Returns(true);
			A.CallTo(() => originalUser.Clone()).Returns(clonedUser);

			A.CallTo(() => userRepository.CheckPassword(A<IUser>.Ignored, "oldpassword")).Returns(true);
			Assert.IsTrue(provider.ChangePassword("modhelius", "oldpassword", "password321;"));

			A.CallTo(() => userRepository.SetPassword(A<IUser>.Ignored, "password321;")).MustHaveHappened(Repeated.Exactly.Once);
			A.CallTo(() => userRepository.SaveUser(A<IUser>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
		}

		[Test]
		public void ChangePassword_PassWrongOldPassowrd_DoNotSetPasswordAndSaveUserCalled() {
			var userRepository = A.Fake<IUserRepository>();
			var provider = GetProvider(userRepository);
			A.CallTo(() => userRepository.CheckPassword(A<IUser>.Ignored, "wrongoldpassword")).Returns(false);
			Assert.IsFalse(provider.ChangePassword("modhelius", "wrongoldpassword", "password321;"));

		}


		[Test]
		public void ChangePassword_UserDoesNotExist_ReturnsFalse() {
			var userRepository = A.Fake<IUserRepository>();
			var provider = GetProvider(userRepository);
			A.CallTo(() => userRepository.GetUserByUserName(A<string>.Ignored, A<string>.Ignored)).Returns(null);
			Assert.IsFalse(provider.ChangePassword("modhelius", "oldpassword", "password321;"));
		}

		[Test]
		public void ChangePassword_PassNullUsername_ThrowsArgumentNullException() {
			var userRepository = A.Fake<IUserRepository>();
			var provider = GetProvider(userRepository);
			Assert.Throws<ArgumentNullException>(() => provider.ChangePassword(null, "oldpassword", "password321;"));
		}

		[Test]
		public void ValidateUser_ValidatePasswordAndSetLastLoginDate_LoginDateSetAndSaved() {
			var userRepository = A.Fake<IUserRepository>();
			var provider = GetProvider(userRepository);
			var user = A.Fake<IUser>();
			var clonedUser = A.Fake<IUser>();

			user.Properties.LastLoginDate = DateTime.MinValue;
			A.CallTo(() => userRepository.GetUserByUserName("modhelius", "njupiter")).Returns(user);
			A.CallTo(() => user.IsReadOnly).Returns(true);
			A.CallTo(() => user.Clone()).Returns(clonedUser);

			A.CallTo(() => userRepository.CheckPassword(user, "password")).Returns(true);
			Assert.IsTrue(provider.ValidateUser("njupiter\\modhelius", "password"));
			Assert.AreEqual(DateTime.UtcNow.DayOfYear, clonedUser.Properties.LastLoginDate.DayOfYear);
			A.CallTo(() => userRepository.SaveUser(clonedUser)).MustHaveHappened(Repeated.Exactly.Once);
		}

		[Test]
		public void ValidateUser_PassNullUsername_ThrowsArgumentNullException() {
			var userRepository = A.Fake<IUserRepository>();
			var provider = GetProvider(userRepository);
			Assert.Throws<ArgumentNullException>(() => provider.ValidateUser(null, "password"));
		}

		[Test]
		public void ValidateUser_UserDoesNotExist_ReturnsFalse() {
			var userRepository = A.Fake<IUserRepository>();
			var provider = GetProvider(userRepository);
			A.CallTo(() => userRepository.GetUserByUserName(A<string>.Ignored, A<string>.Ignored)).Returns(null);
			Assert.IsFalse(provider.ValidateUser("modhelius", "password"));
		}


		private static MembershipProvider GetProvider(IUserRepository userRepository) {
			return GetProvider(userRepository, new NameValueCollection());
		}

		private static MembershipProvider GetProvider(IUserRepository userRepository, NameValueCollection config) {
			var userRepositoryManager = A.Fake<IUserRepositoryManager>();
			A.CallTo(() => userRepositoryManager.GetRepository(userRepository.Name)).Returns(userRepository);
			var provider = new MembershipProvider(userRepositoryManager);
			provider.Initialize("provider", config);
			return provider;
		}

		[Test]
		public void Initialize_SetAllValuesAndCheckThem_CorrectValuesSet() {
			var userRepository = A.Fake<IUserRepository>();
			A.CallTo(() => userRepository.Name).Returns("MyApplication");			
			
			var config = new NameValueCollection();
			
			config.Add("userRepository", "MyApplication");
			config.Add("requiresUniqueEmail", "true");
			config.Add("requiresQuestionAndAnswer", "true");
			config.Add("passwordAttemptWindow", "3");
			config.Add("minRequiredPasswordLength", "4");
			config.Add("minRequiredNonalphanumericCharacters", "2");
			config.Add("maxInvalidPasswordAttempts", "3");
			config.Add("enablePasswordReset", "true");
			config.Add("enablePasswordRetrieval", "true");
			config.Add("passwordStrengthRegularExpression", "*");
			config.Add("passwordFormat", "Clear");

			var provider = GetProvider(userRepository, config);

			Assert.AreEqual(userRepository, provider.UserRepository);
			Assert.AreEqual("MyApplication", provider.ApplicationName);
			Assert.AreEqual("provider", provider.Name);
			Assert.IsTrue(provider.RequiresUniqueEmail);
			Assert.IsTrue(provider.RequiresQuestionAndAnswer);
			Assert.AreEqual(3, provider.PasswordAttemptWindow);
			Assert.AreEqual(4, provider.MinRequiredPasswordLength);
			Assert.AreEqual(2, provider.MinRequiredNonAlphanumericCharacters);
			Assert.AreEqual(3, provider.MaxInvalidPasswordAttempts);
			Assert.IsTrue(provider.EnablePasswordReset);
			Assert.IsTrue(provider.EnablePasswordRetrieval);
			Assert.AreEqual("*", provider.PasswordStrengthRegularExpression);
			Assert.AreEqual(MembershipPasswordFormat.Clear, provider.PasswordFormat);






		}
	}
}
