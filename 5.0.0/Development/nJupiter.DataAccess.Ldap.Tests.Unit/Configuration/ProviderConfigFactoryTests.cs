#region Copyright & License
// 
// 	Copyright (c) 2005-2012 nJupiter
// 
// 	Permission is hereby granted, free of charge, to any person obtaining a copy
// 	of this software and associated documentation files (the "Software"), to deal
// 	in the Software without restriction, including without limitation the rights
// 	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// 	copies of the Software, and to permit persons to whom the Software is
// 	furnished to do so, subject to the following conditions:
// 
// 	The above copyright notice and this permission notice shall be included in
// 	all copies or substantial portions of the Software.
// 
// 	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// 	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// 	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// 	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// 	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// 	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// 	THE SOFTWARE.
// 
#endregion

using System.Collections.Specialized;

using FakeItEasy;

using NUnit.Framework;

using nJupiter.DataAccess.Ldap.Configuration;

namespace nJupiter.DataAccess.Ldap.Tests.Unit.Configuration {

	[TestFixture]
	public class ProviderConfigFactoryTests {

		[Test]
		public void Create_SendInName_ReturnsConfigWithName() {
			var ldapConfigFactory = A.Fake<ILdapConfigFactory>();
			var factory = new ProviderConfigFactory(ldapConfigFactory);

			var providerConfig = factory.Create<ProviderConfigFactoryTests>("providername", null);

			Assert.AreEqual("providername", providerConfig.Name);
		}

		[Test]
		public void Create_DoNotSendInApplicationNameInConfig_ReturnsConfigWithApplicationNameSetToTypeName() {
			var ldapConfigFactory = A.Fake<ILdapConfigFactory>();
			var factory = new ProviderConfigFactory(ldapConfigFactory);

			var providerConfig = factory.Create<ProviderConfigFactoryTests>(null, null);

			Assert.AreEqual(typeof(ProviderConfigFactoryTests).Name, providerConfig.ApplicationName);
		}

		[Test]
		public void Create_SendInApplicationNameInConfig_ReturnsConfigWithApplicationNameFromConfig() {
			var ldapConfigFactory = A.Fake<ILdapConfigFactory>();
			var factory = new ProviderConfigFactory(ldapConfigFactory);

			var config = new NameValueCollection();
			config.Add("applicationName", "anyname");

			var providerConfig = factory.Create<ProviderConfigFactoryTests>(null, config);

			Assert.AreEqual("anyname", providerConfig.ApplicationName);
		}

		[Test]
		public void Create_DoNotSendInName_ReturnsConfigWithNameSetToApplicationName() {
			var ldapConfigFactory = A.Fake<ILdapConfigFactory>();
			var factory = new ProviderConfigFactory(ldapConfigFactory);

			var config = new NameValueCollection();
			config.Add("applicationName", "applicationName");

			var providerConfig = factory.Create<ProviderConfigFactoryTests>(null, config);

			Assert.AreEqual("applicationName", providerConfig.Name);
		}

		[Test]
		public void Create_DoNotSendInAnyLdapServerInConfig_ReturnsDefaultLdapServer() {
			var ldapConfigFactory = A.Fake<ILdapConfigFactory>();
			var ldapConfig = A.Fake<ILdapConfig>();
			
			A.CallTo(() => ldapConfigFactory.GetConfig(A<string>.Ignored)).ReturnsLazily(c => {
				A.CallTo(() => ldapConfig.Name).ReturnsLazily(() => c.GetArgument<string>(0));
				return ldapConfig;
			});

			var factory = new ProviderConfigFactory(ldapConfigFactory);
			var providerConfig = factory.Create<ProviderConfigFactoryTests>(null, null);

			Assert.AreEqual(string.Empty, providerConfig.LdapConfig.Name);
		}

		[Test]
		public void Create_SendInLdapServerInConfig_ReturnsConfigForLdapServer() {
			var ldapConfigFactory = A.Fake<ILdapConfigFactory>();
			var ldapConfig = A.Fake<ILdapConfig>();
			
			A.CallTo(() => ldapConfigFactory.GetConfig(A<string>.Ignored)).ReturnsLazily(c => {
				A.CallTo(() => ldapConfig.Name).ReturnsLazily(() => c.GetArgument<string>(0));
				return ldapConfig;
			});
			
			var factory = new ProviderConfigFactory(ldapConfigFactory);

			var config = new NameValueCollection();
			config.Add("ldapServer", "serverName");

			var providerConfig = factory.Create<ProviderConfigFactoryTests>(null, config);

			Assert.AreEqual("serverName", providerConfig.LdapConfig.Name);
		}

		[Test]
		public void Create_SendInName_ReturnsMembershipUserFactoryWithName() {
			var ldapConfigFactory = A.Fake<ILdapConfigFactory>();
			var factory = new ProviderConfigFactory(ldapConfigFactory);

			var providerConfig = factory.Create<ProviderConfigFactoryTests>("providername", null);

			Assert.AreEqual("providername", providerConfig.MembershipUserFactory.Name);
		}
	}
}