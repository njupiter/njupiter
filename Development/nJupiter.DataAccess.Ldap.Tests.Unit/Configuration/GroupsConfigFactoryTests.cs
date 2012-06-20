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
using System.Linq;
using System.Xml;

using NUnit.Framework;

using nJupiter.Configuration;
using nJupiter.DataAccess.Ldap.Configuration;
using nJupiter.DataAccess.Ldap.NameParser;

namespace nJupiter.DataAccess.Ldap.Tests.Unit.Configuration {
	
	[TestFixture]
	public class GroupsConfigFactoryTests {

		[Test]
		public void Create_FilterIsDefiendedInConfig_SetToConfigValue() {
			var config = CreateGroupConfigWithServerConfig("<groups><filter value='groupfilter'/></groups>");
			Assert.AreEqual("groupfilter", config.Filter);
		}

		[Test]
		public void Create_FilterIsNotDefiendedInConfig_SetToDefaultValue() {
			var config = CreateGroupConfigWithServerConfig();
			Assert.AreEqual("(objectClass=groupOfNames)", config.Filter);
		}

		[Test]
		public void Create_BaseIsDefiendedInConfig_SetToConfigValue() {
			var config = CreateGroupConfigWithServerConfig("<groups><base value='basepath'/></groups>");
			Assert.AreEqual("basepath", config.Base);
		}

		[Test]
		public void Create_BaseIsNotDefiendedInConfig_SetToDefaultValue() {
			var config = CreateGroupConfigWithServerConfig();
			Assert.AreEqual(null, config.Base);
		}

		[Test]
		public void Create_RdnAttributeIsDefiendedInConfig_SetToConfigValue() {
			var config = CreateGroupConfigWithServerConfig("<groups><rdnAttribute value='attributename'/></groups>");
			Assert.AreEqual("attributename", config.RdnAttribute);
		}

		[Test]
		public void Create_RdnAttributeIsNotDefiendedInConfig_SetToDefaultValue() {
			var config = CreateGroupConfigWithServerConfig();
			Assert.AreEqual("cn", config.RdnAttribute);
		}

		[Test]
		public void Create_MembershipAttributeIsDefiendedInConfig_SetToConfigValue() {
			var config = CreateGroupConfigWithServerConfig("<groups><membershipAttribute value='attributename'/></groups>");
			Assert.AreEqual("attributename", config.MembershipAttribute);
		}

		[Test]
		public void Create_MembershipAttributeIsNotDefiendedInConfig_SetToDefaultValue() {
			var config = CreateGroupConfigWithServerConfig();
			Assert.AreEqual("member", config.MembershipAttribute);
		}

		[Test]
		public void Create_MembershipAttributeNameTypeIsDefiendedInConfig_SetToConfigValue() {
			var config = CreateGroupConfigWithServerConfig("<groups><membershipAttributeNameType value='rdn'/></groups>");
			Assert.AreEqual(NameType.Rdn, config.MembershipAttributeNameType);
		}

		[Test]
		public void Create_MembershipAttributeNameTypeIsNotDefiendedInConfig_SetToDefaultValue() {
			var config = CreateGroupConfigWithServerConfig();
			Assert.AreEqual(NameType.Cn, config.MembershipAttributeNameType);
		}

		[Test]
		public void Create_NameTypeIsDefiendedInConfig_SetToConfigValue() {
			var config = CreateGroupConfigWithServerConfig("<groups><nameType value='rdn'/></groups>");
			Assert.AreEqual(NameType.Rdn, config.NameType);
		}

		[Test]
		public void CreateNameTypeIsNotDefiendedInConfig_SetToDefaultValue() {
			var config = CreateGroupConfigWithServerConfig();
			Assert.AreEqual(NameType.Cn, config.NameType);
		}

		[Test]
		public void Create_PathIsDefiendedInConfig_SetToConfigValue() {
			var config = CreateGroupConfigWithServerConfig();
			Assert.AreEqual("LDAP://server/", config.Path);
		}

		[Test]
		public void Create_PathIsNotDefiendedInConfig_ThrowsConfigValueNotFoundException() {
			Assert.Throws<ConfigValueNotFoundException>(() => CreateGroupConfig("<ldapServer />"));
		}

		[Test]
		public void Create_AttributeDefinitionListWithOneAttributeDefiendedInConfig_AttributeContainsOneAttributeWithCorrectName() {
			var config = CreateGroupConfigWithServerConfig("<groups><attributes><attribute value='attributeName'/></attributes></groups>");
			Assert.AreEqual("attributeName", config.Attributes.First().Name);
		}

		[Test]
		public void Create_AttributeDefinitionListWithOneAttributeThatShallBeExcludedFromSearchDefiendedInConfig_AttributeContainsOneAttributeThatIsExcludedFromSearch() {
			var config = CreateGroupConfigWithServerConfig("<groups><attributes><attribute value='attributeName' excludeFromNameSearch='true' /></attributes></groups>");
			Assert.IsTrue(config.Attributes.First().ExcludeFromNameSearch);
		}

		[Test]
		public void Create_AttributeDefinitionListWithOneAttributeDefiendedInConfig_AttributeContainsOneAttributeThatIsNotExcludedFromSearch() {
			var config = CreateGroupConfigWithServerConfig("<groups><attributes><attribute value='attributeName' /></attributes></groups>");
			Assert.IsFalse(config.Attributes.First().ExcludeFromNameSearch);
		}

		[Test]
		public void Create_AttributeDefinitionListWithTwoAttributesDefiendedInConfig_AttributeContainsTwoAttributes() {
			var config = CreateGroupConfigWithServerConfig("<groups><attributes><attribute value='attribute1'/><attribute value='attribute2'/></attributes></groups>");
			Assert.AreEqual(2, config.Attributes.Count);
		}

		[Test]
		public void Create_NoAttributeDefinitionListDefiendedInConfig_AttributeContainsOneDefaultAttribute() {
			var config = CreateGroupConfigWithServerConfig();
			Assert.AreEqual(1, config.Attributes.Count);
		}

		[Test]
		public void Create_NoAttributeDefinitionListDefiendedInConfig_AttributeContainsCnAttributeByDefault() {
			var config = CreateGroupConfigWithServerConfig();
			Assert.IsTrue(config.Attributes.Any(c => c.Name == "cn"));
		}

		[Test]
		public void Create_PassNullConfig_ThrowsArgumentNullException() {
			var factory = new GroupsConfigFactory();
			Assert.Throws<ArgumentNullException>(() => factory.Create(null));
		}

		private IGroupsConfig CreateGroupConfigWithServerConfig() {
			return CreateGroupConfigWithServerConfig(string.Empty);
		}

		private IGroupsConfig CreateGroupConfigWithServerConfig(string xml) {
			return CreateGroupConfig(string.Format("<ldapServer><url value='ldap://server/'/>{0}</ldapServer>", xml));
		}

		private IGroupsConfig CreateGroupConfig(string xml) {
			var config = CreateConfig(xml);
			var factory = new GroupsConfigFactory();
			return factory.Create(config);
		}

		private IConfig CreateConfig(string xml) {
			var document = new XmlDocument();
			document.LoadXml(xml);
			return ConfigFactory.Create("config", document.DocumentElement);
		}
	}
}