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
using System.Collections.Specialized;

using FakeItEasy;

using NUnit.Framework;

using nJupiter.DataAccess.Ldap.Configuration;
using nJupiter.DataAccess.Ldap.DirectoryServices;

namespace nJupiter.DataAccess.Ldap.Tests.Unit {
	
	[TestFixture]
	public class LdapRoleProviderTests {

		LdapRoleProvider provider;
		IProviderConfig providerConfig;
		IContainer container;

		[SetUp]
		public void SetUp() {
			providerConfig = A.Fake<IProviderConfig>();
			container = providerConfig.LdapConfig.Container;
			InitializeProvider();
		}

		[Test]
		public void RoleExists_GroupEntryAdapterReturnsEntryWithRdnAttributeForGroup_ReturnsTrue() {
			var entry = A.Fake<IEntry>();
			A.CallTo(() => providerConfig.LdapConfig.Groups.RdnAttribute).Returns("rdnattribute");
			A.CallTo(() => container.GroupEntryAdapter.GetGroupEntry("rolename")).Returns(entry);
			var properties = new Dictionary<string, IEnumerable>();
			properties.Add("rdnattribute", A.CollectionOfFake<object>(1));
			A.CallTo(() => entry.Properties).Returns(properties);

			var result = provider.RoleExists("rolename");

			Assert.IsTrue(result);
		}

		[Test]
		public void RoleExists_GroupEntryAdapterReturnsNull_ReturnsFalse() {
			A.CallTo(() => container.GroupEntryAdapter.GetGroupEntry("rolename")).Returns(null);

			var result = provider.RoleExists("rolename");

			Assert.IsFalse(result);
		}

		[Test]
		public void GetRolesForUser_UsersMembershipAttributeIsConfigured_RetrunsRolesFromUserEntry() {
			var entry = A.Fake<IEntry>();
			A.CallTo(() => providerConfig.LdapConfig.Users.MembershipAttribute).Returns("rdnattribute");
			A.CallTo(() => container.UserEntryAdapter.GetUserEntry("username")).Returns(entry);
			A.CallTo(() => container.GroupEntryAdapter.GetGroupName(A<string>.Ignored)).ReturnsLazily(c => c.GetArgument<string>(0));
			var properties = new Dictionary<string, IEnumerable>();
			var propertyValues = new [] { "rolename" };
			properties.Add("rdnattribute", propertyValues);
			A.CallTo(() => entry.Properties).Returns(properties);

			var result = provider.GetRolesForUser("username");

			CollectionAssert.AreEquivalent(propertyValues, result);
		}

		[Test]
		public void GetRolesForUser_UsersMembershipAttributeIsNotConfigured_RetrunsRolesFromAllRolesComparedToUsersRoles() {
			var roleEntries = new EntryCollection();
			var roleEntry = A.Fake<IEntry>();
			A.CallTo(() => roleEntry.Name).Returns("role");
			roleEntries.Add(roleEntry);
			A.CallTo(() => providerConfig.LdapConfig.Groups.MembershipAttribute).Returns("rdnattribute");
			A.CallTo(() => container.GroupEntryAdapter.GetAllRoleEntries()).Returns(roleEntries);
			A.CallTo(() => container.GroupEntryAdapter.GetGroupEntry("role", true)).Returns(roleEntry);
			A.CallTo(() => container.GroupEntryAdapter.GetGroupName(A<IEntry>.Ignored)).ReturnsLazily(c => c.GetArgument<IEntry>(0).Name);
			A.CallTo(() => container.UserEntryAdapter.GetUsersFromEntry(roleEntry, "rdnattribute")).Returns(new[]{ "username" });

			var result = provider.GetRolesForUser("username");

			Assert.AreEqual("role", result[0]);
		}

		[Test]
		public void GetRolesForUser_AddTwoRoles_ReturnRolesSortedAlphabetial() {
			var roleEntries = new EntryCollection();
			var roleEntry1 = A.Fake<IEntry>();
			var roleEntry2 = A.Fake<IEntry>();
			A.CallTo(() => roleEntry1.Name).Returns("role z");
			A.CallTo(() => roleEntry2.Name).Returns("role a");
			roleEntries.Add(roleEntry1);
			roleEntries.Add(roleEntry2);
			A.CallTo(() => providerConfig.LdapConfig.Groups.MembershipAttribute).Returns("rdnattribute");
			A.CallTo(() => container.GroupEntryAdapter.GetAllRoleEntries()).Returns(roleEntries);
			A.CallTo(() => container.GroupEntryAdapter.GetGroupEntry("role z", true)).Returns(roleEntry1);
			A.CallTo(() => container.GroupEntryAdapter.GetGroupEntry("role a", true)).Returns(roleEntry2);
			A.CallTo(() => container.GroupEntryAdapter.GetGroupName(A<IEntry>.Ignored)).ReturnsLazily(c => c.GetArgument<IEntry>(0).Name);
			A.CallTo(() => container.UserEntryAdapter.GetUsersFromEntry(A<IEntry>.Ignored, "rdnattribute")).Returns(new[]{ "username" });

			var result = provider.GetRolesForUser("username");

			Assert.AreEqual("role a", result[0]);
			Assert.AreEqual("role z", result[1]);
		}


		[Test]
		public void IsUserInRole_GetRolesForUserWhereRoleExistsAndCheckIfUserInRole_ReturnsTrue() {
			var entry = A.Fake<IEntry>();
			A.CallTo(() => providerConfig.LdapConfig.Users.MembershipAttribute).Returns("rdnattribute");
			A.CallTo(() => container.UserEntryAdapter.GetUserEntry("username")).Returns(entry);
			A.CallTo(() => container.GroupEntryAdapter.GetGroupName(A<string>.Ignored)).ReturnsLazily(c => c.GetArgument<string>(0));
			var properties = new Dictionary<string, IEnumerable>();
			var propertyValues = new [] { "rolename" };
			properties.Add("rdnattribute", propertyValues);
			A.CallTo(() => entry.Properties).Returns(properties);

			var result = provider.IsUserInRole("username", "rolename");

			Assert.IsTrue(result);
		}

		[Test]
		public void IsUserInRole_GetRolesForUserWhereRoleDoesNotExistAndCheckIfUserInRole_ReturnsFalse() {
			var entry = A.Fake<IEntry>();
			A.CallTo(() => providerConfig.LdapConfig.Users.MembershipAttribute).Returns("rdnattribute");
			A.CallTo(() => container.UserEntryAdapter.GetUserEntry("username")).Returns(entry);
			A.CallTo(() => container.GroupEntryAdapter.GetGroupName(A<string>.Ignored)).ReturnsLazily(c => c.GetArgument<string>(0));
			var properties = new Dictionary<string, IEnumerable>();
			var propertyValues = new [] { "anotherrolename" };
			properties.Add("rdnattribute", propertyValues);
			A.CallTo(() => entry.Properties).Returns(properties);

			var result = provider.IsUserInRole("username", "rolename");

			Assert.IsFalse(result);
		}

		[Test]
		public void GetUsersInRole_WithRangeRetrieval_ReturnUserInRole() {

			var members = new [] { "username" };
			
			A.CallTo(() => providerConfig.LdapConfig.Server.RangeRetrievalSupport).Returns(true);
			A.CallTo(() => container.GroupEntryAdapter.GetGroupMembersByRangedRetrival("role")).Returns(members);
			A.CallTo(() => container.UserEntryAdapter.GetUserName(A<string>.Ignored)).ReturnsLazily(c => c.GetArgument<string>(0));

			var result = provider.GetUsersInRole("role");

			CollectionAssert.AreEquivalent(result, members);					
		}

		[Test]
		public void GetUsersInRole_NoRangeRetrievalSupportSoGetUserEntriesFromGroupMembershipAttribute_ReturnUserInRole() {
			var roleEntries = new EntryCollection();
			var roleEntry = A.Fake<IEntry>();
			A.CallTo(() => roleEntry.Name).Returns("role");
			roleEntries.Add(roleEntry);

			A.CallTo(() => providerConfig.LdapConfig.Groups.MembershipAttribute).Returns("rdnattribute");
			A.CallTo(() => container.GroupEntryAdapter.GetGroupEntry("role", true)).Returns(roleEntry);
			A.CallTo(() => container.GroupEntryAdapter.GetGroupName(A<IEntry>.Ignored)).ReturnsLazily(c => c.GetArgument<IEntry>(0).Name);
			A.CallTo(() => container.UserEntryAdapter.GetUsersFromEntry(roleEntry, "rdnattribute")).Returns(new[]{ "username" });

			var result = provider.GetUsersInRole("role");

			Assert.AreEqual("username", result[0]);			
		}


		[Test]
		public void GetUsersInRole_GetTwoUsers_ReturnsUsersSortedAlphabetically() {
			var members = new [] { "username z", "username a" };
			
			A.CallTo(() => providerConfig.LdapConfig.Server.RangeRetrievalSupport).Returns(true);
			A.CallTo(() => container.GroupEntryAdapter.GetGroupMembersByRangedRetrival("role")).Returns(members);
			A.CallTo(() => container.UserEntryAdapter.GetUserName(A<string>.Ignored)).ReturnsLazily(c => c.GetArgument<string>(0));

			var result = provider.GetUsersInRole("role");

			Assert.AreEqual("username a", result[0]);
			Assert.AreEqual("username z", result[1]);
		}

		[Test]
		public void GetAllRoles_GetOneRoleEntriyFromAdapter_ReturnsRoleWithSameNameAsInAdapter() {
			var roleEntries = new EntryCollection();
			var roleEntry = A.Fake<IEntry>();
			A.CallTo(() => roleEntry.Name).Returns("role");
			roleEntries.Add(roleEntry);

			A.CallTo(() => container.GroupEntryAdapter.GetAllRoleEntries()).Returns(roleEntries);
			A.CallTo(() => container.GroupEntryAdapter.GetGroupName(A<IEntry>.Ignored)).ReturnsLazily(c => c.GetArgument<IEntry>(0).Name);

			var result = provider.GetAllRoles();

			Assert.AreEqual("role", result[0]);
		}

		[Test]
		public void GetAllRoles_AddTwoRolesThatWillBeReturnedFromAdapter_ReturnsRoleSortedAlphabetical() {
			var roleEntries = new EntryCollection();
			var roleEntry1 = A.Fake<IEntry>();
			var roleEntry2 = A.Fake<IEntry>();
			A.CallTo(() => roleEntry1.Name).Returns("role z");
			A.CallTo(() => roleEntry2.Name).Returns("role a");
			roleEntries.Add(roleEntry1);
			roleEntries.Add(roleEntry2);

			A.CallTo(() => container.GroupEntryAdapter.GetAllRoleEntries()).Returns(roleEntries);
			A.CallTo(() => container.GroupEntryAdapter.GetGroupName(A<IEntry>.Ignored)).ReturnsLazily(c => c.GetArgument<IEntry>(0).Name);

			var result = provider.GetAllRoles();

			Assert.AreEqual("role a", result[0]);
			Assert.AreEqual("role z", result[1]);
		}


		[Test]
		public void Name_ProviderConfigReturnsProviderName_ReturnsNameFromProvider() {
			A.CallTo(() => providerConfig.Name).Returns("ProviderName");

			Assert.AreEqual("ProviderName", provider.Name);
		}

		[Test]
		public void ApplicationName_ProviderConfigReturnsApplicationName_ReturnsNameFromProvider() {
			A.CallTo(() => providerConfig.ApplicationName).Returns("ApplicationName");

			Assert.AreEqual("ApplicationName", provider.ApplicationName);
		}

		[Test]
		public void GetPassword_NotImplemented_ThrowsNotSupportedException() {
			Assert.Throws<NotSupportedException>(() => provider.CreateRole("rolename"));
		}

		[Test]
		public void DeleteRole_NotImplemented_ThrowsNotSupportedException() {
			Assert.Throws<NotSupportedException>(() => provider.DeleteRole("rolename", true));
		}

		[Test]
		public void AddUsersToRoles_NotImplemented_ThrowsNotSupportedException() {
			Assert.Throws<NotSupportedException>(() => provider.AddUsersToRoles(new string[] {}, new string[] {}));
		}

		[Test]
		public void RemoveUsersFromRoles_NotImplemented_ThrowsNotSupportedException() {
			Assert.Throws<NotSupportedException>(() => provider.RemoveUsersFromRoles(new string[] {}, new string[] {}));
		}

		[Test]
		public void FindUsersInRole_NotImplemented_ThrowsNotSupportedException() {
			Assert.Throws<NotSupportedException>(() => provider.FindUsersInRole("rolename", "username"));
		}

		private void InitializeProvider() {
			const string name = "ProviderName";
			var factory = A.Fake<IProviderConfigFactory>();
			var config = new NameValueCollection();
		
			A.CallTo(() => providerConfig.Name).Returns(name);
			A.CallTo(() => factory.Create<LdapRoleProvider>(name, config)).Returns(providerConfig);

			provider = new LdapRoleProvider(factory);
			provider.Initialize(name, config);
		}
		 
	}
}