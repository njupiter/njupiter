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

using System.Xml;

using FakeItEasy;

using NUnit.Framework;

using nJupiter.Configuration;
using nJupiter.DataAccess.Ldap.Configuration;

namespace nJupiter.DataAccess.Ldap.Tests.Unit.Configuration {
	[TestFixture]
	public class LdapConfigTests {
		private IConfigRepository configRepository;
		private IServerConfigFactory serverConfigFactory;
		private IUsersConfigFactory usersConfigFactory;
		private IGroupConfigFactory groupsConfigFactory;

		[SetUp]
		public void SetUp() {
			configRepository = A.Fake<IConfigRepository>();
			serverConfigFactory = A.Fake<IServerConfigFactory>();
			usersConfigFactory = A.Fake<IUsersConfigFactory>();
			groupsConfigFactory = A.Fake<IGroupConfigFactory>();
		}

		[Test]
		public void Constructor_LdapNameDefinedInConfig_ReturnsLdapConfigWithServerConfigSetFromFactory() {
			var config = CreateConfig("<ldapServer default='true' value='ldapServer' />");
			A.CallTo(() => configRepository.GetConfig()).Returns(config);

			var serverConfig = A.Fake<IServerConfig>();
			A.CallTo(() => serverConfigFactory.Create(A<IConfig>.Ignored)).Returns(serverConfig);

			var ldapConfig = new LdapConfig("ldapServer",
			                                configRepository,
			                                serverConfigFactory,
			                                usersConfigFactory,
			                                groupsConfigFactory);

			Assert.AreSame(serverConfig, ldapConfig.Server);
		}

		[Test]
		public void Constructor_LdapNameDefinedInConfig_ReturnsLdapConfigWithUsersConfigSetFromFactory() {
			var config = CreateConfig("<ldapServer default='true' value='ldapServer' />");
			A.CallTo(() => configRepository.GetConfig()).Returns(config);

			var usersConfig = A.Fake<IUsersConfig>();
			A.CallTo(() => usersConfigFactory.Create(A<IConfig>.Ignored)).Returns(usersConfig);

			var ldapConfig = new LdapConfig("ldapServer",
			                                configRepository,
			                                serverConfigFactory,
			                                usersConfigFactory,
			                                groupsConfigFactory);

			Assert.AreSame(usersConfig, ldapConfig.Users);
		}

		[Test]
		public void Constructor_LdapNameDefinedInConfig_ReturnsLdapConfigWithGroupsConfigSetFromFactory() {
			var config = CreateConfig("<ldapServer default='true' value='ldapServer' />");
			A.CallTo(() => configRepository.GetConfig()).Returns(config);

			var groupsConfig = A.Fake<IGroupsConfig>();
			A.CallTo(() => groupsConfigFactory.Create(A<IConfig>.Ignored)).Returns(groupsConfig);

			var ldapConfig = new LdapConfig("ldapServer",
			                                configRepository,
			                                serverConfigFactory,
			                                usersConfigFactory,
			                                groupsConfigFactory);

			Assert.AreSame(groupsConfig, ldapConfig.Groups);
		}

		[Test]
		public void Constructor_LdapNameDefinedButNoLdapConfigDefinedWithThatName_ThrowsConfigurationException() {
			var config = CreateConfig("<ldapServer default='true' value='ldapServer' />");
			A.CallTo(() => configRepository.GetConfig()).Returns(config);

			Assert.Throws<ConfigurationException>(() =>
			                                      new LdapConfig("otherLdapServer",
			                                                     configRepository,
			                                                     serverConfigFactory,
			                                                     usersConfigFactory,
			                                                     groupsConfigFactory)
				);
		}

		[Test]
		public void Constructor_NoLdapNameDefinedAndNoDefaultLdapConfigDefined_ThrowsConfigurationException() {
			var config = CreateConfig("<ldapServer value='ldapServer' />");
			A.CallTo(() => configRepository.GetConfig()).Returns(config);

			Assert.Throws<ConfigurationException>(() =>
			                                      new LdapConfig(string.Empty,
			                                                     configRepository,
			                                                     serverConfigFactory,
			                                                     usersConfigFactory,
			                                                     groupsConfigFactory)
				);
		}

		[Test]
		public void Configure_ConfigChanged_NewContainerCreated() {
			var originalConfig = CreateConfig("<ldapServer default='true' value='ldapServer' />");
			var newConfig = CreateConfig("<ldapServer default='true' value='ldapServer' />");
			A.CallTo(() => configRepository.GetConfig()).Returns(originalConfig);

			var serverConfig = A.Fake<IServerConfig>();
			A.CallTo(() => serverConfigFactory.Create(A<IConfig>.Ignored)).Returns(serverConfig);

			var ldapConfig = new LdapConfig("ldapServer",
			                                configRepository,
			                                serverConfigFactory,
			                                usersConfigFactory,
			                                groupsConfigFactory);

			var origianlContainer = ldapConfig.Container;
			originalConfig.Discard();

			Assert.AreNotSame(origianlContainer, ldapConfig.Container);
		}

		private IConfig CreateConfig(string xml) {
			var document = new XmlDocument();
			document.LoadXml(string.Format("<config><ldapServers>{0}</ldapServers></config>", xml));
			return ConfigFactory.Create("config", document.DocumentElement);
		}
	}
}