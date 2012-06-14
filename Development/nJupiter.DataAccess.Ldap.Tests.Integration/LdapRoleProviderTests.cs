using System.Collections.Specialized;
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
