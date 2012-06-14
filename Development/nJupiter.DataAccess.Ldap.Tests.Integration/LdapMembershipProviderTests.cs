using System.Collections.Specialized;
using System.Reflection;
using System.Web.Security;

using NUnit.Framework;

namespace nJupiter.DataAccess.Ldap.Tests.Integration {
	
	[TestFixture, Explicit]
	public class LdapMembershipProviderTests{

		private MembershipProvider provider;

		private const string ExistingUserName = "existingUsername";
		private const string ExistingUsersCorrectPassword = "correctpassword";
		private const string ExistingUsersIncorrectPassword = "wrongpassword";
		private const string ExistingEmailAdress = "existing@email.org";

		[Test]
		public void GetUser_GetExistingUser_ReturnsMembershipUserWithSameUsername() {
			var user = provider.GetUser(ExistingUserName, false);

			Assert.AreEqual(ExistingUserName, user.UserName);
		}

		[Test]
		public void GetUserNameByEmail_GetExistingUser_ReturnsMembershipUserExistingUsername() {
			var userName = provider.GetUserNameByEmail(ExistingEmailAdress);

			Assert.AreEqual(ExistingUserName, userName);
		}

		[Test]
		public void ValidateUser_ValidateUserWithCorrectPassword_ReturnsTrue() {
			var result = provider.ValidateUser(ExistingUserName, ExistingUsersCorrectPassword);

			Assert.IsTrue(result);
		}

		[Test]
		public void ValidateUser_ValidateUserWithIncorrectPassword_ReturnsFalse() {
			var result = provider.ValidateUser(ExistingUserName, ExistingUsersIncorrectPassword);

			Assert.IsFalse(result);
		}

		[Test]
		public void GetAllUsers_GetAllUsers_ReturnsCollectionWithUsers() {
			int totalRecords;
			var result = provider.GetAllUsers(0, 1000, out totalRecords);

			Assert.IsTrue(result.Count > 0);
		}

		[Test]
		public void FindUsersByName_FindUsersByExistingUserName_ReturnsCollectionWithUser() {
			int totalRecords;
			var result = provider.FindUsersByName(ExistingUserName, 0, 1000, out totalRecords);

			Assert.IsNotNull(result[ExistingUserName]);
		}

		[Test]
		public void FindUsersByEmail_FindUsersByExistingEmail_ReturnsCollectionWithUser() {
			int totalRecords;
			var result = provider.FindUsersByEmail(ExistingEmailAdress, 0, 1000, out totalRecords);

			Assert.IsNotNull(result[ExistingUserName]);
		}

		[SetUp]
		public void SetUp() {
			var config = new NameValueCollection();
			config.Add("applicationName", "IntegrationTests");
			config.Add("ldapServer", "ad");
			var ldapProvider = new LdapMembershipProvider();

			ldapProvider.Initialize("IntegrationTests", config);

			var providers = Membership.Providers;
			var readOnlyField = typeof(System.Configuration.Provider.ProviderCollection).GetField("_ReadOnly",
			                                                                                      BindingFlags.NonPublic |
			                                                                                      BindingFlags.Instance);
			readOnlyField.SetValue(providers, false);
			providers.Add(ldapProvider);

			var registeredProvider = Membership.Providers["IntegrationTests"];

			Assert.IsNotNull(registeredProvider);
			
			provider = registeredProvider;
		}

		[TearDown]
		public void TearDown() {
			var providers = Membership.Providers;
			var readOnlyField = typeof(System.Configuration.Provider.ProviderCollection).GetField("_ReadOnly",
			                                                                                      BindingFlags.NonPublic |
			                                                                                      BindingFlags.Instance);
			readOnlyField.SetValue(providers, false);
			providers.Remove("IntegrationTests");
		}


	}

}
