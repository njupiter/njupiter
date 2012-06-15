using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Web.Security;

using NUnit.Framework;

namespace nJupiter.DataAccess.Ldap.Tests.Integration {

	[TestFixture, Explicit]
	public class LdapRoleProviderTests {

		private const string LdapServerInConfig = "ad";
		private const string ExistingUserName = "existingUsername";

		private RoleProvider provider;

		[Test]
		public void GetRolesForUser_GetRolesForExistingUser_ReturnsACollectionOfRoles() {
			 var roles = provider.GetRolesForUser(ExistingUserName);

			Assert.IsTrue(roles.Length > 0);
		}

		[Test]
		public void RoleExists_GetRolesForExistingUser_RetrunsTrueForAllRoles() {
			var roles = provider.GetRolesForUser(ExistingUserName);

			foreach(var role in roles) {
				Assert.IsTrue(provider.RoleExists(role), string.Format("Role with name '{0}' does not exist", role));
			}
		}

		[Test]
		public void IsUserInRole_GetRolesForUserAndCheckIfuserInAllRoles_RetrunsTrueForAllRoles() {
			var roles = provider.GetRolesForUser(ExistingUserName);

			foreach(var role in roles) {
				Assert.IsTrue(provider.IsUserInRole(ExistingUserName, role), string.Format("User '{0}' is not in role '{1}'", ExistingUserName, role));
			}
		}

		[Test]
		public void GetUsersInRole_GetRolesForExistingUserAndGetUsersInAllRoles_ExistingUserInRoles() {
			var roles = provider.GetRolesForUser(ExistingUserName);
			foreach(var role in roles) {
				IEnumerable<string> users = provider.GetUsersInRole(role);
				Assert.IsTrue(users.Contains(ExistingUserName));
			}
		}

		[Test]
		public void GetAllRoles_GetAllRolesForExistingUser_UsersRolesInAllRoles() {
			var roles = provider.GetAllRoles();
			var rolesForUsers = provider.GetRolesForUser(ExistingUserName);
			foreach(var userRole in rolesForUsers) {
				Assert.IsTrue(roles.Contains(userRole));
			}
		}

		[SetUp]
		public void SetUp() {
			var config = new NameValueCollection();
			config.Add("applicationName", "IntegrationTests");
			config.Add("ldapServer", LdapServerInConfig);
			var ldapProvider = new LdapRoleProvider();

			ldapProvider.Initialize("IntegrationTests", config);

			var enabledField = typeof(Roles).GetField("s_Enabled",
			                                    BindingFlags.NonPublic |
			                                    BindingFlags.Static);

			enabledField.SetValue(typeof(Roles), true);

			var initialized = typeof(Roles).GetField("s_Initialized",
			                                    BindingFlags.NonPublic |
			                                    BindingFlags.Static);

			initialized.SetValue(typeof(Roles), true);

			var providers = new RoleProviderCollection(); 

			var readOnlyField = typeof(Roles).GetField("s_Providers",
			                                                BindingFlags.NonPublic |
			                                                BindingFlags.Static);
			readOnlyField.SetValue(typeof(Roles), providers);

			providers.Add(ldapProvider);

			var registeredProvider = Roles.Providers["IntegrationTests"];

			Assert.IsNotNull(registeredProvider);
			
			provider = registeredProvider;
		}

		[TearDown]
		public void TearDown() {
			var providers = Roles.Providers;
			var readOnlyField = typeof(System.Configuration.Provider.ProviderCollection).GetField("_ReadOnly",
			                                                                                      BindingFlags.NonPublic |
			                                                                                      BindingFlags.Instance);
			readOnlyField.SetValue(providers, false);
			providers.Remove("IntegrationTests");
		}
	}
}
