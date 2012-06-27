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
using nJupiter.DataAccess.Ldap.DistinguishedNames;

namespace nJupiter.DataAccess.Ldap.Tests.Unit.Configuration {

	[TestFixture]
	public class UsersConfigFactoryTests {
		[Test]
		public void Create_FilterIsDefiendedInConfig_SetToConfigValue() {
			var config = CreateUserConfigWithServerConfig("<users><filter value='userfilter'/></users>");
			Assert.AreEqual("userfilter", config.Filter);
		}

		[Test]
		public void Create_FilterIsNotDefiendedInConfig_SetToDefaultValue() {
			var config = CreateUserConfigWithServerConfig();
			Assert.AreEqual(null, config.Filter);
		}

		[Test]
		public void Create_BaseIsDefiendedInConfig_SetToConfigValue() {
			var config = CreateUserConfigWithServerConfig("<users><base value='basepath'/></users>");
			Assert.AreEqual("basepath", config.Base);
		}

		[Test]
		public void Create_BaseIsNotDefiendedInConfig_SetToDefaultValue() {
			var config = CreateUserConfigWithServerConfig();
			Assert.AreEqual(null, config.Base);
		}

		[Test]
		public void Create_RdnAttributeIsDefiendedInConfig_SetToConfigValue() {
			var config = CreateUserConfigWithServerConfig("<users><rdnAttribute value='attributename'/></users>");
			Assert.AreEqual("attributename", config.RdnAttribute);
		}

		[Test]
		public void Create_RdnAttributeIsNotDefiendedInConfig_SetToDefaultValue() {
			var config = CreateUserConfigWithServerConfig();
			Assert.AreEqual("cn", config.RdnAttribute);
		}

		[Test]
		public void Create_SetRdnInPathIsDefiendedInConfig_SetToConfigValue() {
			var config = CreateUserConfigWithServerConfig("<users><rdnInPath value='false'/></users>");
			Assert.IsFalse(config.RdnInPath);
		}

		[Test]
		public void Create_SetRdnInPathIsNotDefiendedInConfig_SetToDefaultValue() {
			var config = CreateUserConfigWithServerConfig();
			Assert.IsTrue(config.RdnInPath);
		}

		[Test]
		public void Create_MembershipAttributeIsDefiendedInConfig_SetToConfigValue() {
			var config = CreateUserConfigWithServerConfig("<users><membershipAttribute value='attributename'/></users>");
			Assert.AreEqual("attributename", config.MembershipAttribute);
		}

		[Test]
		public void Create_LastLoginDateAttributeIsDefiendedInConfig_SetToConfigValue() {
			var config = CreateUserConfigWithServerConfig("<users><lastLoginDateAttribute value='attributename'/></users>");
			Assert.AreEqual("attributename", config.LastLoginDateAttribute);
		}

		[Test]
		public void Create_LastLoginDateAttributeIsNotDefiendedInConfig_SetToDefaultValue() {
			var config = CreateUserConfigWithServerConfig();
			Assert.AreEqual(null, config.LastLoginDateAttribute);
		}

		[Test]
		public void Create_EmailAttributeIsDefiendedInConfig_SetToConfigValue() {
			var config = CreateUserConfigWithServerConfig("<users><emailAttribute value='attributename'/></users>");
			Assert.AreEqual("attributename", config.EmailAttribute);
		}

		[Test]
		public void Create_EmailAttributeIsNotDefiendedInConfig_SetToDefaultValue() {
			var config = CreateUserConfigWithServerConfig();
			Assert.AreEqual("mail", config.EmailAttribute);
		}

		[Test]
		public void Create_LastPasswordChangedDateAttributeIsDefiendedInConfig_SetToConfigValue() {
			var config = CreateUserConfigWithServerConfig("<users><lastPasswordChangedDateAttribute value='attributename'/></users>");
			Assert.AreEqual("attributename", config.LastPasswordChangedDateAttribute);
		}

		[Test]
		public void Create_LastPasswordChangedDateAttributeIsNotDefiendedInConfig_SetToDefaultValue() {
			var config = CreateUserConfigWithServerConfig();
			Assert.AreEqual(null, config.LastPasswordChangedDateAttribute);
		}

		[Test]
		public void Create_CreationDateAttributeIsDefiendedInConfig_SetToConfigValue() {
			var config = CreateUserConfigWithServerConfig("<users><creationDateAttribute value='attributename'/></users>");
			Assert.AreEqual("attributename", config.CreationDateAttribute);
		}

		[Test]
		public void Create_CreationDateAttributeIsNotDefiendedInConfig_SetToDefaultValue() {
			var config = CreateUserConfigWithServerConfig();
			Assert.AreEqual(null, config.CreationDateAttribute);
		}

		[Test]
		public void Create_DescriptionAttributeIsDefiendedInConfig_SetToConfigValue() {
			var config = CreateUserConfigWithServerConfig("<users><descriptionAttribute value='attributename'/></users>");
			Assert.AreEqual("attributename", config.DescriptionAttribute);
		}

		[Test]
		public void Create_DescriptionAttributeIsNotDefiendedInConfig_SetToDefaultValue() {
			var config = CreateUserConfigWithServerConfig();
			Assert.AreEqual(null, config.DescriptionAttribute);
		}

		[Test]
		public void Create_NameTypeIsDefiendedInConfig_SetToConfigValue() {
			var config = CreateUserConfigWithServerConfig("<users><nameType value='rdn'/></users>");
			Assert.AreEqual(NameType.Rdn, config.NameType);
		}

		[Test]
		public void CreateNameTypeIsNotDefiendedInConfig_SetToDefaultValue() {
			var config = CreateUserConfigWithServerConfig();
			Assert.AreEqual(NameType.Cn, config.NameType);
		}

		[Test]
		public void Create_PathIsDefiendedInConfig_SetToConfigValue() {
			var config = CreateUserConfigWithServerConfig();
			Assert.AreEqual("LDAP://server/", config.Path);
		}

		[Test]
		public void Create_PathIsNotDefiendedInConfig_ThrowsConfigValueNotFoundException() {
			Assert.Throws<ConfigValueNotFoundException>(() => CreateUserConfig("<ldapServer />"));
		}

		[Test]
		public void
			Create_AttributeDefinitionListWithOneAttributeDefiendedInConfig_AttributeContainsOneAttributeWithCorrectName() {
			var config =
				CreateUserConfigWithServerConfig("<users><attributes><attribute value='attributeName'/></attributes></users>");
			Assert.IsTrue(config.Attributes.Any(a => a.Name == "attributeName"));
		}

		[Test]
		public void
			Create_AttributeDefinitionListWithOneAttributeThatShallBeExcludedFromSearchDefiendedInConfig_AttributeContainsOneAttributeThatIsExcludedFromSearch
			() {
			var config =
				CreateUserConfigWithServerConfig(
				                                 "<users><attributes><attribute value='attributeName' excludeFromNameSearch='true' /></attributes></users>");
			Assert.IsTrue(config.Attributes.First().ExcludeFromNameSearch);
		}

		[Test]
		public void
			Create_AttributeDefinitionListWithOneAttributeDefiendedInConfig_AttributeContainsOneAttributeThatIsNotExcludedFromSearch
			() {
			var config =
				CreateUserConfigWithServerConfig("<users><attributes><attribute value='attributeName' /></attributes></users>");
			Assert.IsFalse(config.Attributes.First(a => a.Name == "attributeName").ExcludeFromNameSearch);
		}

		[Test]
		public void Create_AttributeDefinitionListWithTwoAttributesDefiendedInConfig_AttributeContainsTwoAttributes() {
			const int defaultNumberOfAttributes = 1;
			var config =
				CreateUserConfigWithServerConfig(
				                                 "<users><attributes><attribute value='attribute1'/><attribute value='attribute2'/></attributes></users>");
			Assert.AreEqual(2 + defaultNumberOfAttributes, config.Attributes.Count());
		}

		[Test]
		public void Create_NoAttributeDefinitionListDefiendedInConfig_AttributeContainsOneDefaultAttribute() {
			const int defaultNumberOfAttributes = 1;
			var config = CreateUserConfigWithServerConfig();
			Assert.AreEqual(1 + defaultNumberOfAttributes, config.Attributes.Count());
		}

		[Test]
		public void Create_NoAttributeDefinitionListDefiendedInConfig_AttributeContainsCnAttributeByDefault() {
			var config = CreateUserConfigWithServerConfig();
			Assert.IsTrue(config.Attributes.Any(c => c.Name == "cn"));
		}

		[Test]
		public void Create_NoAttributeDefinitionListDefiendedInConfig_AttributeContainsEmailButItIsExcludedFromSearch() {
			var config = CreateUserConfigWithServerConfig();
			Assert.IsTrue(config.Attributes.Any(c => c.Name == "mail" && c.ExcludeFromNameSearch));
		}

		[Test]
		public void Create_AttributeDefinitionListWithTwoAttributesDefiendedInConfig_AttributeDoesNotContainsCnWhenCustomAttributesAreDefined() {
			var config = CreateUserConfigWithServerConfig("<users><attributes><attribute value='attribute1'/><attribute value='attribute2'/></attributes></users>");
			Assert.IsFalse(config.Attributes.Any(c => c.Name == "cn"));
		}

		[Test]
		public void Create_AttributeDefinitionListWithMembershipAttribute_OnlyOneAttributeDefinitionInList() {
			var config = CreateUserConfigWithServerConfig("<users><attributes><attribute value='memberOf'/></attributes></users>");
			Assert.AreEqual(1, config.Attributes.Count(c => c.Name == "memberOf"));
		}

		[Test]
		public void Create_AttributeDefinitionListWitEmailAttribute_OnlyOneAttributeDefinitionInList() {
			var config = CreateUserConfigWithServerConfig("<users><attributes><attribute value='mail'/></attributes></users>");
			Assert.AreEqual(1, config.Attributes.Count(c => c.Name == "mail"));
		}


		[Test]
		public void Create_AttributeDefinitionListWithMembershipAttributeNotExcludeFromNameSearch_OneAttributeDefinitionInListThatIsNotExcludedFromSearch() {
			var config = CreateUserConfigWithServerConfig("<users><attributes><attribute value='memberOf'/></attributes></users>");
			Assert.AreEqual(1, config.Attributes.Count(c => c.Name == "memberOf" && !c.ExcludeFromNameSearch));
		}

		[Test]
		public void Create_AttributeDefinitionListWithEmailAttributeNotExcludeFromNameSearch_OneAttributeDefinitionInListThatIsNotExcludedFromSearch() {
			var config = CreateUserConfigWithServerConfig("<users><attributes><attribute value='mail'/></attributes></users>");
			Assert.AreEqual(1, config.Attributes.Count(c => c.Name == "mail" && !c.ExcludeFromNameSearch));
		}


		[Test]
		public void Create_CreationDateAttributeDefiend_AddedToAttributesCollection() {
			var config = CreateUserConfigWithServerConfig("<users><creationDateAttribute value='creationDate' /></users>");
			Assert.AreEqual(1, config.Attributes.Count(c => c.Name == "creationDate"));
		}

		[Test]
		public void Create_LastLoginDateAttributeDefiend_AddedToAttributesCollection() {
			var config = CreateUserConfigWithServerConfig("<users><lastLoginDateAttribute value='lastLoginDate' /></users>");
			Assert.AreEqual(1, config.Attributes.Count(c => c.Name == "lastLoginDate"));
		}

		[Test]
		public void Create_LastPasswordChangedDateAttributeDefiend_AddedToAttributesCollection() {
			var config = CreateUserConfigWithServerConfig("<users><lastPasswordChangedDateAttribute value='lastPasswordChangedDate' /></users>");
			Assert.AreEqual(1, config.Attributes.Count(c => c.Name == "lastPasswordChangedDate"));
		}

		[Test]
		public void Create_DescriptionAttributeAttributeDefiend_AddedToAttributesCollection() {
			var config = CreateUserConfigWithServerConfig("<users><descriptionAttribute value='description' /></users>");
			Assert.AreEqual(1, config.Attributes.Count(c => c.Name == "description"));
		}

		[Test]
		public void Create_PassNullConfig_ThrowsArgumentNullException() {
			var factory = new UsersConfigFactory();
			Assert.Throws<ArgumentNullException>(() => factory.Create(null));
		}

		private IUsersConfig CreateUserConfigWithServerConfig() {
			return CreateUserConfigWithServerConfig(string.Empty);
		}

		private IUsersConfig CreateUserConfigWithServerConfig(string xml) {
			return CreateUserConfig(string.Format("<ldapServer><url value='ldap://server/'/>{0}</ldapServer>", xml));
		}

		private IUsersConfig CreateUserConfig(string xml) {
			var config = CreateConfig(xml);
			var factory = new UsersConfigFactory();
			return factory.Create(config);
		}

		private IConfig CreateConfig(string xml) {
			var document = new XmlDocument();
			document.LoadXml(xml);
			return ConfigFactory.Create("config", document.DocumentElement);
		}
	}
}