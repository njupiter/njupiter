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

using System;
using System.DirectoryServices;
using System.Xml;

using NUnit.Framework;

using nJupiter.Configuration;
using nJupiter.DataAccess.Ldap.Configuration;

namespace nJupiter.DataAccess.Ldap.Tests.Unit.Configuration {
	
	[TestFixture]
	public class ServerConfigFactoryTests {

		[Test]
		public void Create_UrlIsDefiendedInConfig_SetToConfigValue() {
			var config = CreateServerConfigWithServerConfig();
			Assert.AreEqual("ldap://server/", config.Url.OriginalString);
		}

		[Test]
		public void Create_UrlIsNotDefiendedInConfig_ThrowsConfigValueNotFoundException() {
			Assert.Throws<ConfigValueNotFoundException>(() => CreateGroupConfig("<ldapServer />"));
		}
		
		[Test]
		public void Create_UsernameIsDefiendedInConfig_SetToConfigValue() {
			var config = CreateServerConfigWithServerConfig("<username value='username'/>");
			Assert.AreEqual("username", config.Username);
		}

		[Test]
		public void Create_UsernameIsNotDefiendedInConfig_SetToDefaultValue() {
			var config = CreateServerConfigWithServerConfig();
			Assert.AreEqual(null, config.Username);
		}

		[Test]
		public void Create_PasswordIsDefiendedInConfig_SetToConfigValue() {
			var config = CreateServerConfigWithServerConfig("<password value='password'/>");
			Assert.AreEqual("password", config.Password);
		}

		[Test]
		public void Create_PasswordIsNotDefiendedInConfig_SetToDefaultValue() {
			var config = CreateServerConfigWithServerConfig();
			Assert.AreEqual(null, config.Password);
		}

		[Test]
		public void Create_AllowWildcardSearchDefiendedInConfig_SetToConfigValue() {
			var config = CreateServerConfigWithServerConfig("<allowWildcardSearch value='false'/>");
			Assert.IsFalse(config.AllowWildcardSearch);
		}

		[Test]
		public void Create_AllowWildcardSearchNotDefiendedInConfig_SetToDefaultValue() {
			var config = CreateServerConfigWithServerConfig();
			Assert.IsTrue(config.AllowWildcardSearch);
		}

		[Test]
		public void Create_TimeLimitDefiendedInConfig_SetToConfigValue() {
			const int oneSecondInTicks = 10000000;
			var config = CreateServerConfigWithServerConfig("<timeLimit value='1'/>");
			Assert.AreEqual(new TimeSpan(oneSecondInTicks), config.TimeLimit);
		}

		[Test]
		public void Create_TimeLimitNotDefiendedInConfig_SetToDefaultValue() {
			const int thirtySecondsInTicks = 300000000;
			var config = CreateServerConfigWithServerConfig();
			Assert.AreEqual(new TimeSpan(thirtySecondsInTicks), config.TimeLimit);
		}

		[Test]
		public void Create_PageSizeDefiendedInConfig_SetToConfigValue() {
			var config = CreateServerConfigWithServerConfig("<pageSize value='1'/>");
			Assert.AreEqual(1, config.PageSize);
		}

		[Test]
		public void Create_PageSizeNotDefiendedInConfig_SetToDefaultValue() {
			var config = CreateServerConfigWithServerConfig();
			Assert.AreEqual(0, config.PageSize);
		}

		[Test]
		public void Create_RangeRetrievalSupportDefiendedInConfig_SetToConfigValue() {
			var config = CreateServerConfigWithServerConfig("<rangeRetrievalSupport value='false'/>");
			Assert.IsFalse(config.RangeRetrievalSupport);
		}

		[Test]
		public void Create_RangeRetrievalSupportNotDefiendedInConfig_SetToDefaultValue() {
			var config = CreateServerConfigWithServerConfig();
			Assert.IsTrue(config.RangeRetrievalSupport);
		}

		[Test]
		public void Create_PropertySortingSupportDefiendedInConfig_SetToConfigValue() {
			var config = CreateServerConfigWithServerConfig("<propertySortingSupport value='false'/>");
			Assert.IsFalse(config.PropertySortingSupport);
		}

		[Test]
		public void Create_PropertySortingSupportNotDefiendedInConfig_SetToDefaultValue() {
			var config = CreateServerConfigWithServerConfig();
			Assert.IsTrue(config.PropertySortingSupport);
		}

		[Test]
		public void Create_AuthenticationTypesDefiendedInConfig_SetToConfigValue() {
			var config = CreateServerConfigWithServerConfig("<authenticationTypes><authenticationType value='ServerBind' /></authenticationTypes>");
			Assert.AreEqual(AuthenticationTypes.ServerBind, config.AuthenticationTypes);
		}

		[Test]
		public void Create_TwoAuthenticationTypesDefiendedInConfig_SetToConfigValue() {
			var config = CreateServerConfigWithServerConfig("<authenticationTypes><authenticationType value='ServerBind' /><authenticationType value='Encryption' /></authenticationTypes>");
			Assert.AreEqual(AuthenticationTypes.ServerBind | AuthenticationTypes.Encryption, config.AuthenticationTypes);
		}

		[Test]
		public void Create_AuthenticationTypesNotDefiendedInConfig_SetToDefaultValue() {
			var config = CreateServerConfigWithServerConfig();
			Assert.AreEqual(AuthenticationTypes.None, config.AuthenticationTypes);
		}

		[Test]
		public void Create_PassNullConfig_ThrowsArgumentNullException() {
			var factory = new ServerConfigFactory();
			Assert.Throws<ArgumentNullException>(() => factory.Create(null));
		}

		private IServerConfig CreateServerConfigWithServerConfig() {
			return CreateServerConfigWithServerConfig(string.Empty);
		}

		private IServerConfig CreateServerConfigWithServerConfig(string xml) {
			return CreateGroupConfig(string.Format("<ldapServer><url value='ldap://server/'/>{0}</ldapServer>", xml));
		}

		private IServerConfig CreateGroupConfig(string xml) {
			var config = CreateConfig(xml);
			var factory = new ServerConfigFactory();
			return factory.Create(config);
		}

		private IConfig CreateConfig(string xml) {
			var document = new XmlDocument();
			document.LoadXml(xml);
			return ConfigFactory.Create("config", document.DocumentElement);
		}
		 
	}
}