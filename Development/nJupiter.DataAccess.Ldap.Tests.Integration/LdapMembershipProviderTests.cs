using System.Collections.Specialized;
using System.Reflection;
using System.Web.Security;

using NUnit.Framework;

namespace nJupiter.DataAccess.Ldap.Tests.Integration {
	
	[TestFixture]
	public class LdapMembershipProviderTests{

		MembershipProvider provider;

		[Test]
		public void Method_Scenario_ExprectedResult() {
			var user = provider.GetUser("modhelius", false);

			Assert.AreEqual("modhelius", user.UserName);
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
