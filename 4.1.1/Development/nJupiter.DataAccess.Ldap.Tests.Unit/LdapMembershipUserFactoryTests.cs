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
using System.Collections;
using System.Collections.Generic;
using System.Web.Security;

using FakeItEasy;

using NUnit.Framework;

using nJupiter.DataAccess.Ldap.Configuration;
using nJupiter.DataAccess.Ldap.DirectoryServices;

namespace nJupiter.DataAccess.Ldap.Tests.Unit {
	
	[TestFixture]
	public class LdapMembershipUserFactoryTests {

		[Test]
		public void Create_EntryObjectNotBound_ReturnsNull() {
			var config = A.Fake<ILdapConfig>();
			var factory = new LdapMembershipUserFactory("providername", config);

			var entry = A.Fake<IEntry>();
			A.CallTo(() => entry.NativeObject).Returns(null);
			var result = factory.Create(entry);

			Assert.IsNull(result);
		}

		[Test]
		public void Create_RdnAttributeSetOnEntry_ReturnsUserWithNameFromRdnAttribute() {
			var config = A.Fake<ILdapConfig>();
			A.CallTo(() => config.Users.RdnAttribute).Returns("rdnattribute");
			var factory = new TestableLdapMembershipUserFactory("providername", config);

			var entry = A.Fake<IEntry>();
			var properties = new Dictionary<string, IEnumerable>();
			properties.Add("rdnattribute", new[] { "username" });
			A.CallTo(() => entry.Properties).Returns(properties);

			var result = (TestableLdapMembershipUser)factory.Create(entry);

			Assert.AreEqual("username", result.UserName);
		}

		[Test]
		public void Create_RdnAttributeNotSetOnEntry_ReturnsUserWithNameSetToEntryName() {
			var config = A.Fake<ILdapConfig>();
			var factory = new TestableLdapMembershipUserFactory("providername", config);

			var entry = A.Fake<IEntry>();
			A.CallTo(() => entry.Name).Returns("username");

			var result = (TestableLdapMembershipUser)factory.Create(entry);

			Assert.AreEqual("username", result.UserName);
		}

		[Test]
		public void Create_RdnAttributeSetOnEntry_ReturnsUserWithIdFromRdnAttribute() {
			var config = A.Fake<ILdapConfig>();
			A.CallTo(() => config.Users.RdnAttribute).Returns("rdnattribute");
			var factory = new TestableLdapMembershipUserFactory("providername", config);

			var entry = A.Fake<IEntry>();
			var properties = new Dictionary<string, IEnumerable>();
			properties.Add("rdnattribute", new[] { "username" });
			A.CallTo(() => entry.Properties).Returns(properties);

			var result = (TestableLdapMembershipUser)factory.Create(entry);

			Assert.AreEqual("username", result.ProviderUserKey);
		}

		[Test]
		public void Create_RdnAttributeNotSetOnEntry_ReturnsUserWithProviderUserKeyFromEntryName() {
			var config = A.Fake<ILdapConfig>();
			var factory = new TestableLdapMembershipUserFactory("providername", config);

			var entry = A.Fake<IEntry>();
			A.CallTo(() => entry.Name).Returns("username");

			var result = (TestableLdapMembershipUser)factory.Create(entry);

			Assert.AreEqual("username", result.ProviderUserKey);
		}

		[Test]
		public void Create_EmailAttributeSetOnEntry_ReturnsUserWithEmailFromEmailAttribute() {
			var config = A.Fake<ILdapConfig>();
			A.CallTo(() => config.Users.EmailAttribute).Returns("emailattribute");
			var factory = new TestableLdapMembershipUserFactory("providername", config);

			var entry = A.Fake<IEntry>();
			var properties = new Dictionary<string, IEnumerable>();
			properties.Add("emailattribute", new[] { "email" });
			A.CallTo(() => entry.Properties).Returns(properties);

			var result = (TestableLdapMembershipUser)factory.Create(entry);

			Assert.AreEqual("email", result.Email);
		}

		[Test]
		public void Create_EmailAttributeNotSetOnEntry_ReturnsUserWithEmailSetToDefaultValue() {
			var config = A.Fake<ILdapConfig>();
			var factory = new TestableLdapMembershipUserFactory("providername", config);

			var entry = A.Fake<IEntry>();

			var result = (TestableLdapMembershipUser)factory.Create(entry);

			Assert.AreEqual(null, result.Email);
		}

		[Test]
		public void Create_DescriptionAttributeSetOnEntry_ReturnsUserWithDescriptionFromDescriptionAttribute() {
			var config = A.Fake<ILdapConfig>();
			A.CallTo(() => config.Users.DescriptionAttribute).Returns("descriptionattribute");
			var factory = new TestableLdapMembershipUserFactory("providername", config);

			var entry = A.Fake<IEntry>();
			var properties = new Dictionary<string, IEnumerable>();
			properties.Add("descriptionattribute", new[] { "description" });
			A.CallTo(() => entry.Properties).Returns(properties);

			var result = (TestableLdapMembershipUser)factory.Create(entry);

			Assert.AreEqual("description", result.Description);
		}

		[Test]
		public void Create_DescriptionAttributeNotSetOnEntry_ReturnsUserWithDescriptionSetToDefaultValue() {
			var config = A.Fake<ILdapConfig>();
			var factory = new TestableLdapMembershipUserFactory("providername", config);

			var entry = A.Fake<IEntry>();

			var result = (TestableLdapMembershipUser)factory.Create(entry);

			Assert.AreEqual(null, result.Description);
		}

		[Test]
		public void Create_CreationDateAttributeSetOnEntry_ReturnsUserWithCreationDateFromCreationDateAttribute() {
			var config = A.Fake<ILdapConfig>();
			A.CallTo(() => config.Users.CreationDateAttribute).Returns("creationdateattribute");
			var factory = new TestableLdapMembershipUserFactory("providername", config);

			var entry = A.Fake<IEntry>();
			var properties = new Dictionary<string, IEnumerable>();
			properties.Add("creationdateattribute", new[] { "2010-01-01" });
			A.CallTo(() => entry.Properties).Returns(properties);

			var result = (TestableLdapMembershipUser)factory.Create(entry);

			Assert.AreEqual(new DateTime(2010,01,01), result.CreationDate);
		}

		[Test]
		public void Create_CreationDateNotAttributeSetOnEntry_ReturnsUserWithCreationDateSetToDefaultValue() {
			var config = A.Fake<ILdapConfig>();
			var factory = new TestableLdapMembershipUserFactory("providername", config);

			var entry = A.Fake<IEntry>();

			var result = (TestableLdapMembershipUser)factory.Create(entry);

			Assert.AreEqual(DateTime.MinValue, result.CreationDate);
		}

		[Test]
		public void Create_LastLoginDateAttributeSetOnEntry_ReturnsUserWithLastLoginDateFromLastLoginDateAttribute() {
			var config = A.Fake<ILdapConfig>();
			A.CallTo(() => config.Users.LastLoginDateAttribute).Returns("lastlogindateattribute");
			var factory = new TestableLdapMembershipUserFactory("providername", config);

			var entry = A.Fake<IEntry>();
			var properties = new Dictionary<string, IEnumerable>();
			properties.Add("lastlogindateattribute", new[] { "2010-01-01" });
			A.CallTo(() => entry.Properties).Returns(properties);

			var result = (TestableLdapMembershipUser)factory.Create(entry);

			Assert.AreEqual(new DateTime(2010,01,01), result.LastLoginDate);
		}

		[Test]
		public void Create_LastLoginDateAttributeNotSetOnEntry_ReturnsUserWithLastLoginDateSetToDefaultValue() {
			var config = A.Fake<ILdapConfig>();
			var factory = new TestableLdapMembershipUserFactory("providername", config);

			var entry = A.Fake<IEntry>();

			var result = (TestableLdapMembershipUser)factory.Create(entry);

			Assert.AreEqual(DateTime.MinValue, result.LastLoginDate);
		}

		[Test]
		public void Create_LastPasswordChangedDateAttributeSetOnEntry_ReturnsUserWithLastPasswordChangedDateFromLastPasswordChangedDateAttribute() {
			var config = A.Fake<ILdapConfig>();
			A.CallTo(() => config.Users.LastPasswordChangedDateAttribute).Returns("lastpasswordchangeddateattribute");
			var factory = new TestableLdapMembershipUserFactory("providername", config);

			var entry = A.Fake<IEntry>();
			var properties = new Dictionary<string, IEnumerable>();
			properties.Add("lastpasswordchangeddateattribute", new[] { "2010-01-01" });
			A.CallTo(() => entry.Properties).Returns(properties);

			var result = (TestableLdapMembershipUser)factory.Create(entry);

			Assert.AreEqual(new DateTime(2010,01,01), result.LastPasswordChangedDate);
		}

		[Test]
		public void Create_LastPasswordChangedDateAttributeNotSetOnEntry_ReturnsUserWithLastPasswordChangedDateSetToDefaultValue() {
			var config = A.Fake<ILdapConfig>();
			var factory = new TestableLdapMembershipUserFactory("providername", config);

			var entry = A.Fake<IEntry>();

			var result = (TestableLdapMembershipUser)factory.Create(entry);

			Assert.AreEqual(DateTime.MinValue, result.LastPasswordChangedDate);
		}

		[Test]
		public void Create_CreationDateAttributeSetOnEntry_ReturnsUserWithLastLockoutDateFromCreationDateAttribute() {
			var config = A.Fake<ILdapConfig>();
			A.CallTo(() => config.Users.CreationDateAttribute).Returns("creationdateattribute");
			var factory = new TestableLdapMembershipUserFactory("providername", config);

			var entry = A.Fake<IEntry>();
			var properties = new Dictionary<string, IEnumerable>();
			properties.Add("creationdateattribute", new[] { "2010-01-01" });
			A.CallTo(() => entry.Properties).Returns(properties);

			var result = (TestableLdapMembershipUser)factory.Create(entry);

			Assert.AreEqual(new DateTime(2010,01,01), result.LastLockoutDate);
		}

		[Test]
		public void Create_CreationDateAttributeNotSetOnEntry_ReturnsUserWithLastLockoutDateSetToDefaultValue() {
			var config = A.Fake<ILdapConfig>();
			var factory = new TestableLdapMembershipUserFactory("providername", config);

			var entry = A.Fake<IEntry>();

			var result = (TestableLdapMembershipUser)factory.Create(entry);

			Assert.AreEqual(DateTime.MinValue, result.LastLockoutDate);
		}

		[Test]
		public void Create_CreateMembershipUser_LastActivitiyDateSetToNow() {
			var config = A.Fake<ILdapConfig>();
			var now = DateTime.Now;
			var factory = new TestableLdapMembershipUserFactory("providername", config, now);

			var entry = A.Fake<IEntry>();
			var result = (TestableLdapMembershipUser)factory.Create(entry);

			Assert.AreEqual(now, result.LastActivitiyDate);
		}

		[Test]
		public void Create_CreateMembershipUserFromEntry_UserGetsPathFromEntry() {
			var config = A.Fake<ILdapConfig>();
			var factory = new TestableLdapMembershipUserFactory("providername", config);

			var entry = A.Fake<IEntry>();
			A.CallTo(() => entry.Path).Returns("path");
			var result = (TestableLdapMembershipUser)factory.Create(entry);

			Assert.AreEqual("path", result.Path);
		}


		[Test]
		public void Create_CreateMembershipUserFromEntry_UserGetsPropertiesFromEntry() {
			var config = A.Fake<ILdapConfig>();
			var factory = new TestableLdapMembershipUserFactory("providername", config);

			var entry = A.Fake<IEntry>();
			A.CallTo(() => entry.Path).Returns("path");
			var result = (TestableLdapMembershipUser)factory.Create(entry);

			Assert.AreSame(entry.Properties, result.Properties);
		}

		[Test]
		public void CreateCollection_PassCollectionWithOneEntry_ReturnsMembershipUserCollectionWithOneUser() {
			var config = A.Fake<ILdapConfig>();
			var factory = new TestableLdapMembershipUserFactory("providername", config);

			var entries = new EntryCollection();
			var entry = A.Fake<IEntry>();
			A.CallTo(() => entry.Name).Returns("uniqueusername");
			entries.Add(entry);

			var result = factory.CreateCollection(entries);

			Assert.AreEqual(1, result.Count);
		}
	}

	internal class TestableLdapMembershipUserFactory : LdapMembershipUserFactory {
		private readonly DateTime now;

		public TestableLdapMembershipUserFactory(string providerName, ILdapConfig config, DateTime now) : this(providerName, config) {
			this.now = now;
		}

		public TestableLdapMembershipUserFactory(string providerName, ILdapConfig config) : base(providerName, config) {}

		protected override MembershipUser CreateMembershipUser(string name, string providerUserKey, string email, string description, DateTime lastActivitiyDate, DateTime lastLoginDate, DateTime lastLockoutDate, DateTime lastPasswordChangedDate, DateTime creationDate, IDictionary properties, string path) {
			return new TestableLdapMembershipUser(name, providerUserKey, email, description, lastActivitiyDate, lastLoginDate, lastLockoutDate, lastPasswordChangedDate, creationDate, properties, path);
		}

		protected override DateTime GetDateTimeNow() {
			return now;
		}
	}

	internal class TestableLdapMembershipUser : LdapMembershipUser {
		private readonly string providerUserKey;
		private readonly string userName;
		private readonly string email;
		private readonly string description;
		private readonly DateTime lastActivitiyDate;
		private readonly DateTime lastLoginDate;
		private readonly DateTime lastLockoutDate;
		private readonly DateTime lastPasswordChangedDate;
		private readonly DateTime creationDate;
		private readonly IDictionary properties;
		private readonly string path;

		internal TestableLdapMembershipUser(string userName, string providerUserKey, string email, string description, DateTime lastActivitiyDate, DateTime lastLoginDate, DateTime lastLockoutDate, DateTime lastPasswordChangedDate, DateTime creationDate, IDictionary properties, string path) {
			this.providerUserKey = providerUserKey;
			this.userName = userName;
			this.email = email;
			this.description = description;
			this.lastActivitiyDate = lastActivitiyDate;
			this.lastLoginDate = lastLoginDate;
			this.lastLockoutDate = lastLockoutDate;
			this.lastPasswordChangedDate = lastPasswordChangedDate;
			this.creationDate = creationDate;
			this.properties = properties;
			this.path = path;
		}

		public override object ProviderUserKey { get { return providerUserKey; } }
		public override string UserName { get { return userName; } }
		public override string Email { get { return email; } }
		public string Description { get { return description; } }
		public DateTime LastActivitiyDate { get { return lastActivitiyDate; } }
		public override DateTime LastLoginDate { get { return lastLoginDate; } }
		public override DateTime LastLockoutDate { get { return lastLockoutDate; } }
		public override DateTime LastPasswordChangedDate { get { return lastPasswordChangedDate; } }
		public override DateTime CreationDate { get { return creationDate; } }
		public IDictionary Properties { get { return properties; } }
		public override string Path { get { return path; } }
	}

}