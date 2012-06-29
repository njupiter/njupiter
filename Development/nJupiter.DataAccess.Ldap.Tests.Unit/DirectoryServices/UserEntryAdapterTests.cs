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
using System.DirectoryServices;
using System.Linq;

using FakeItEasy;

using NUnit.Framework;

using nJupiter.DataAccess.Ldap.Configuration;
using nJupiter.DataAccess.Ldap.DirectoryServices;
using nJupiter.DataAccess.Ldap.DirectoryServices.Abstraction;
using nJupiter.DataAccess.Ldap.DistinguishedNames;

namespace nJupiter.DataAccess.Ldap.Tests.Unit.DirectoryServices {
	
	[TestFixture]
	public class UserEntryAdapterTests {

		private INameParser nameParser;
		private ISearcherFactory searcherFactory;
		private IDirectoryEntryAdapter directoryEntryAdapter;
		private ILdapConfig ldapConfig;
		private IFilterBuilder filterBuilder;
		private TestableUserEntryAdapter adapter;

		[SetUp]
		public void SetUp() {
			nameParser = new NameParser();
			searcherFactory = A.Fake<ISearcherFactory>();
			directoryEntryAdapter = A.Fake<IDirectoryEntryAdapter>();
			ldapConfig = A.Fake<ILdapConfig>();
			filterBuilder = new FilterBuilder(ldapConfig.Server);
			adapter = new TestableUserEntryAdapter(ldapConfig, directoryEntryAdapter, searcherFactory, filterBuilder, nameParser);
		}

		[Test]
		public void GetUserEntry_UserEntryLoadedFromDirectoryEntryAdapter_ReturnsEntryFromAdapter() {
			var entry = A.Fake<IDirectoryEntry>();

			A.CallTo(() =>  directoryEntryAdapter.GetEntry("anyusername", ldapConfig.Users, A<Func<IEntry ,IDirectorySearcher>>.Ignored)).Returns(entry);

			var result = adapter.GetUserEntry("anyusername");

			Assert.AreSame(entry, result);
		}

		[Test]
		public void GetUserEntryAndLoadProperties_UserEntryLoadedFromDirectoryAndThenLoadedViaSearcher_ReturnsEntryFromSearcher() {
			var directoryEntry = A.Fake<IDirectoryEntry>();
			var searcher = A.Fake<IDirectorySearcher>();
			var entry = A.Fake<IDirectoryEntry>();

			A.CallTo(() => directoryEntryAdapter.GetEntry("anyusername", ldapConfig.Users, A<Func<IEntry ,IDirectorySearcher>>.Ignored)).Returns(directoryEntry);
			A.CallTo(() => searcherFactory.CreateSearcher(directoryEntry, SearchScope.Base, ldapConfig.Users)).Returns(searcher);
			A.CallTo(() => searcher.FindOne()).Returns(entry);

			var result = adapter.GetUserEntryAndLoadProperties("anyusername");

			Assert.AreSame(entry, result);
		}

		[Test]
		public void GetUserEntryAndLoadProperties_UserEntryLoadedFromDirectoryAndThenLoadedViaSearcher_FilterOnSearcherSetToUserFilter() {
			var searcher = A.Fake<IDirectorySearcher>();
			var entry = A.Fake<IDirectoryEntry>();

			A.CallTo(() => searcherFactory.CreateSearcher(A<IEntry>.Ignored, SearchScope.Base, ldapConfig.Users)).Returns(searcher);
			A.CallTo(() => searcher.FindOne()).Returns(entry);
			A.CallTo(() => ldapConfig.Users.Filter).Returns("userfilter");

			adapter.GetUserEntryAndLoadProperties("anyusername");

			Assert.AreEqual("userfilter", searcher.Filter);
		}

		[Test]
		public void GetUserEntryAndLoadProperties_DirectoryEntryAdaptersNull_ReturnsNull() {
			A.CallTo(() => directoryEntryAdapter.GetEntry("anyusername", ldapConfig.Users, A<Func<IEntry ,IDirectorySearcher>>.Ignored)).Returns(null);

			var result = adapter.GetUserEntryAndLoadProperties("anyusername");

			Assert.IsNull(result);
		}

		[Test]
		public void GetUserEntryByEmail_UsersEntryLoadedFromDirectoryAndThenUserLoadedViaSearcher_ReturnsEntryFromSearcher() {
			var directoryEntry = A.Fake<IDirectoryEntry>();
			var searcher = A.Fake<IDirectorySearcher>();
			var entry = A.Fake<IDirectoryEntry>();

			A.CallTo(() => directoryEntryAdapter.GetEntry(ldapConfig.Users.Path)).Returns(directoryEntry);
			A.CallTo(() => searcherFactory.CreateSearcher(directoryEntry, SearchScope.Subtree, ldapConfig.Users)).Returns(searcher);
			A.CallTo(() => searcher.FindOne()).Returns(entry);

			var result = adapter.GetUserEntryByEmail("anyemail");

			Assert.AreSame(entry, result);
		}

		[Test]
		public void GetUserEntryByEmail_UsersEntryLoadedFromDirectoryAndThenUserLoadedViaSearcher_FilterOnSearcherSetToUserFilterWithEmailAttribute() {
			var searcher = A.Fake<IDirectorySearcher>();
			var entry = A.Fake<IDirectoryEntry>();

			A.CallTo(() => searcherFactory.CreateSearcher(A<IEntry>.Ignored, SearchScope.Subtree, ldapConfig.Users)).Returns(searcher);
			A.CallTo(() => searcher.FindOne()).Returns(entry);
			A.CallTo(() => ldapConfig.Users.Filter).Returns("userfilter");
			A.CallTo(() => ldapConfig.Users.EmailAttribute).Returns("emailAttribute");

			adapter.GetUserEntryByEmail("anyemail");

			Assert.AreEqual("(&userfilter(emailAttribute=anyemail))", searcher.Filter);
		}

		[Test]
		public void GetUserEntryByEmail_DirectoryEntryAdaptersNull_ReturnsNull() {
			A.CallTo(() => directoryEntryAdapter.GetEntry(ldapConfig.Users.Path)).Returns(null);

			var result = adapter.GetUserEntryByEmail("anyemail");

			Assert.IsNull(result);
		}


		[Test]
		public void GetUserName_CnNameType_ReturnsPlayUserName() {
			A.CallTo(() => ldapConfig.Users.RdnAttribute).Returns("cn");
			A.CallTo(() => ldapConfig.Users.NameType).Returns(NameType.Cn);

			var userName = adapter.GetUserName("cn=username,dn=domain,o=organistaion");

			Assert.AreEqual("username", userName);
		}


		[Test]
		public void GetUserName_RdnNameType_ReturnsCnPartOfDn() {
			A.CallTo(() => ldapConfig.Users.RdnAttribute).Returns("cn");
			A.CallTo(() => ldapConfig.Users.NameType).Returns(NameType.Rdn);

			var userName = adapter.GetUserName("cn=username,dn=domain,o=organistaion");

			Assert.AreEqual("cn=username", userName);
		}

		[Test]
		public void GetUserName_RdnNameType_ReturnsFullUserName() {
			A.CallTo(() => ldapConfig.Users.RdnAttribute).Returns("cn");
			A.CallTo(() => ldapConfig.Users.NameType).Returns(NameType.Dn);

			var userName = adapter.GetUserName("cn=username,dn=domain,o=organistaion");

			Assert.AreEqual("cn=username,dn=domain,o=organistaion", userName);
		}

		[Test]
		public void GetUserName_CnNameTypeWhenRdnNotInPath_ReturnsPlayUserName() {
			A.CallTo(() => ldapConfig.Users.RdnAttribute).Returns("rdnattribute");
			A.CallTo(() => ldapConfig.Users.NameType).Returns(NameType.Cn);

			var entry = A.Fake<IDirectoryEntry>();
			var properties = new Dictionary<string, IEnumerable>();
			properties.Add("rdnattribute", new [] { "cn=username,dn=domain,o=organistaion" });
			A.CallTo(() => entry.Properties).Returns(properties);

			A.CallTo(() => directoryEntryAdapter.GetEntry("cn=username,dn=domain,o=organistaion", ldapConfig.Users, A<Func<IEntry ,IDirectorySearcher>>.Ignored)).Returns(entry);

	
			var userName = adapter.GetUserName("cn=username,dn=domain,o=organistaion");

			Assert.AreEqual("username", userName);
		}


		[Test]
		public void GetUserName_RdnNameTypeWhenRdnNotInPath_ReturnsCnPartOfDn() {
			A.CallTo(() => ldapConfig.Users.RdnAttribute).Returns("rdnattribute");
			A.CallTo(() => ldapConfig.Users.NameType).Returns(NameType.Rdn);

			var entry = A.Fake<IDirectoryEntry>();
			var properties = new Dictionary<string, IEnumerable>();
			properties.Add("rdnattribute", new [] { "cn=username, dn=domain, o=organistaion" });
			A.CallTo(() => entry.Properties).Returns(properties);

			A.CallTo(() => directoryEntryAdapter.GetEntry("cn=username,dn=domain,o=organistaion", ldapConfig.Users, A<Func<IEntry ,IDirectorySearcher>>.Ignored)).Returns(entry);

			var userName = adapter.GetUserName("cn=username,dn=domain,o=organistaion");

			Assert.AreEqual("cn=username", userName);
		}

		[Test]
		public void GetUserName_RdnNameTypeWhenRdnNotInPath_ReturnsFullUserName() {
			A.CallTo(() => ldapConfig.Users.RdnAttribute).Returns("rdnattribute");
			A.CallTo(() => ldapConfig.Users.NameType).Returns(NameType.Dn);

			var entry = A.Fake<IDirectoryEntry>();
			var properties = new Dictionary<string, IEnumerable>();
			properties.Add("rdnattribute", new [] { "cn=username, dn=domain, o=organistaion" });
			A.CallTo(() => entry.Properties).Returns(properties);

			A.CallTo(() => directoryEntryAdapter.GetEntry("cn=username,dn=domain,o=organistaion", ldapConfig.Users, A<Func<IEntry ,IDirectorySearcher>>.Ignored)).Returns(entry);

			var userName = adapter.GetUserName("cn=username,dn=domain,o=organistaion");

			Assert.AreEqual("cn=username,dn=domain,o=organistaion", userName);
		}

		[Test]
		public void GetUsersFromEntry_SendInAnEntryWithMembershipAttributeWhenRdnInPath_ExtractUserNameFromMembershipProperty() {
			A.CallTo(() => ldapConfig.Users.RdnAttribute).Returns("cn");
			A.CallTo(() => ldapConfig.Users.NameType).Returns(NameType.Cn);

			var entry = A.Fake<IEntry>();
			var properties = new Dictionary<string, IEnumerable>();
			properties.Add("membership", new [] { "cn=member,dn=domain,o=organistaion" });
			A.CallTo(() => entry.Properties).Returns(properties);

			var result = adapter.GetUsersFromEntry(entry, "membership");

			Assert.AreEqual("member", result.First());
		}

		[Test]
		public void GetUserEntry_UserDoesNotExistInTheUnderlyingAdaptor_ReturnsNulll() {
			A.CallTo(() =>  directoryEntryAdapter.GetEntry("username", ldapConfig.Users, A<Func<IEntry ,IDirectorySearcher>>.Ignored)).Returns(null);

			var result = adapter.GetUserEntry("username", "password");

			Assert.IsNull(result);
		}

		[Test]
		public void GetUserEntry_UserExistsInUnderlyingAdapterAndIsAuthenticatedAndFetchedViaSearcherToLoadProperties_ReturnsSearchedAuthenticatedUser() {
			var searcher = A.Fake<IDirectorySearcher>();
			var directoryEntry = A.Fake<IDirectoryEntry>();
			var authenticatedUser = A.Fake<IDirectoryEntry>();
			var searchedAuthenticatedUser = A.Fake<IEntry>();
			
			A.CallTo(() =>  directoryEntryAdapter.GetEntry("username", ldapConfig.Users, A<Func<IEntry ,IDirectorySearcher>>.Ignored)).Returns(directoryEntry);
			A.CallTo(() => directoryEntry.Path).Returns("ldap://server/cn=username, dn=domain, o=organistaion");
			A.CallTo(() => ldapConfig.Server.Url).Returns(new Uri("ldap://server/"));
			A.CallTo(() => directoryEntryAdapter.GetEntry(A<Uri>.Ignored, "cn=username,dn=domain,o=organistaion", "password")).Returns(authenticatedUser);
			A.CallTo(() => searcherFactory.CreateSearcher(authenticatedUser, SearchScope.Base, ldapConfig.Users)).Returns(searcher);
			A.CallTo(() => searcher.FindOne()).Returns(searchedAuthenticatedUser);

			var result = adapter.GetUserEntry("username", "password");

			Assert.AreSame(searchedAuthenticatedUser, result);
		}

		[Test]
		public void GetAllUserEntries_UsersEntryDoesNotExistInUnderlyingAdaptor_ReturnsEmptyArray() {
			A.CallTo(() => directoryEntryAdapter.GetEntry(ldapConfig.Users.Path)).Returns(null);

			int totalRecords;
			var result = adapter.GetAllUserEntries(0, 1, out totalRecords);

			Assert.AreEqual(0, totalRecords);
			CollectionAssert.IsEmpty(result);
		}

		[Test]
		public void GetAllUserEntries_GetTheSecondPageOfTenInACollectionOfHundred_ReturnsCorrectRange() {
			var searcher = A.Fake<IDirectorySearcher>();

			var entryCollection = new EntryCollection(A.CollectionOfFake<IEntry>(100));
			A.CallTo(() => searcherFactory.CreateSearcher(A<IEntry>.Ignored, SearchScope.Subtree, ldapConfig.Users)).Returns(searcher);
			A.CallTo(() => searcher.FindAll()).Returns(entryCollection);

			int totalRecords;
			var result = adapter.GetAllUserEntries(1, 10, out totalRecords);

			Assert.AreSame(entryCollection.ElementAt(10), result.First());
			Assert.AreSame(entryCollection.ElementAt(19), result.Last());
		}

		[Test]
		public void GetAllUserEntries_GetACollectionOfHundred_ReturnsHundredTotalRecors() {
			var searcher = A.Fake<IDirectorySearcher>();

			var entryCollection = new EntryCollection(A.CollectionOfFake<IEntry>(100));
			A.CallTo(() => searcherFactory.CreateSearcher(A<IEntry>.Ignored, SearchScope.Subtree, ldapConfig.Users)).Returns(searcher);
			A.CallTo(() => searcher.FindAll()).Returns(entryCollection);

			int totalRecords;
			adapter.GetAllUserEntries(0, 1, out totalRecords);

			Assert.AreEqual(100, totalRecords);
		}

		[Test]
		public void GetAllUserEntries_SearcherReturnsNoEntries_ReturnsEmptyArray() {
			var searcher = A.Fake<IDirectorySearcher>();

			var entryCollection = new EntryCollection();
			A.CallTo(() => searcherFactory.CreateSearcher(A<IEntry>.Ignored, SearchScope.Subtree, ldapConfig.Users)).Returns(searcher);
			A.CallTo(() => searcher.FindAll()).Returns(entryCollection);

			int totalRecords;
			var result = adapter.GetAllUserEntries(0, 1, out totalRecords);

			CollectionAssert.IsEmpty(result);
		}

		[Test]
		public void GetAllUserEntries_PageIndexLessThanZero_ArgumentOutOfRangeException() {
			var searcher = A.Fake<IDirectorySearcher>();

			var entryCollection = new EntryCollection(A.CollectionOfFake<IEntry>(1));
			A.CallTo(() => searcherFactory.CreateSearcher(A<IEntry>.Ignored, SearchScope.Subtree, ldapConfig.Users)).Returns(searcher);
			A.CallTo(() => searcher.FindAll()).Returns(entryCollection);

			int totalRecords;
			Assert.Throws(	Is.TypeOf<ArgumentOutOfRangeException>().And.Property("ParamName").EqualTo("pageIndex"),
							() => adapter.GetAllUserEntries(-1, 10, out totalRecords));

		}

		[Test]
		public void GetAllUserEntries_PageSizeLessThanOne_ArgumentOutOfRangeException() {
			var searcher = A.Fake<IDirectorySearcher>();

			var entryCollection = new EntryCollection(A.CollectionOfFake<IEntry>(1));
			A.CallTo(() => searcherFactory.CreateSearcher(A<IEntry>.Ignored, SearchScope.Subtree, ldapConfig.Users)).Returns(searcher);
			A.CallTo(() => searcher.FindAll()).Returns(entryCollection);

			int totalRecords;
			Assert.Throws(	Is.TypeOf<ArgumentOutOfRangeException>().And.Property("ParamName").EqualTo("pageSize"),
							() => adapter.GetAllUserEntries(0, 0, out totalRecords));

		}

		[Test]
		public void GetAllUserEntries_GetSecondPageWithTenUsersWithVirtualListViewSupportEnabled_ReturnsUnmodifedListFromSearcher() {
			var searcher = A.Fake<IDirectorySearcher>();

			var entryCollection = new EntryCollection(A.CollectionOfFake<IEntry>(100));
			A.CallTo(() => searcherFactory.CreateSearcher(A<IEntry>.Ignored, SearchScope.Subtree, ldapConfig.Users)).Returns(searcher);
			A.CallTo(() => searcher.FindAll()).Returns(entryCollection);
			A.CallTo(() => ldapConfig.Server.VirtualListViewSupport).Returns(true);

			int totalRecords;
			var result = adapter.GetAllUserEntries(1, 10, out totalRecords);

			CollectionAssert.AreEqual(entryCollection, result);
		}

		[Test]
		public void GetAllUserEntries_GetSecondPageWithTenUsersWithVirtualListViewSupportEnabled_ReturnsSeracherWithVirtualListViewRangeSetToLoadElventhEntyPlusNineAfterThat() {
			var searcher = A.Fake<IDirectorySearcher>();

			var entryCollection = new EntryCollection(A.CollectionOfFake<IEntry>(1));
			A.CallTo(() => searcherFactory.CreateSearcher(A<IEntry>.Ignored, SearchScope.Subtree, ldapConfig.Users)).Returns(searcher);
			A.CallTo(() => searcher.FindAll()).Returns(entryCollection);
			A.CallTo(() => ldapConfig.Server.VirtualListViewSupport).Returns(true);

			int totalRecords;
			adapter.GetAllUserEntries(1, 10, out totalRecords);

			Assert.AreEqual(0, searcher.VirtualListView.BeforeCount);
			Assert.AreEqual(9, searcher.VirtualListView.AfterCount);
			Assert.AreEqual(11, searcher.VirtualListView.Offset);

		}

		[Test]
		public void GetAllUserEntries_GetAllUsersWithVirtualListViewSupportEnabled_ReturnsTotalRecordsFromVirtualListView() {
			var searcher = A.Fake<IDirectorySearcher>();

			var entryCollection = new EntryCollection(A.CollectionOfFake<IEntry>(1));
			A.CallTo(() => searcherFactory.CreateSearcher(A<IEntry>.Ignored, SearchScope.Subtree, ldapConfig.Users)).Returns(searcher);
			A.CallTo(() => searcher.FindAll()).Returns(entryCollection);
			A.CallTo(() => ldapConfig.Server.VirtualListViewSupport).Returns(true);
			adapter.VirtualListViewTotalCount = 100;

			int totalRecords;
			adapter.GetAllUserEntries(1, 10, out totalRecords);

			Assert.AreEqual(100, totalRecords);
		}

		[Test]
		public void GetAllUserEntries_GetAllUsers_SearcherHasUsersFilterForSearch() {
			var searcher = A.Fake<IDirectorySearcher>();
			
			A.CallTo(() => ldapConfig.Users.Filter).Returns("usersfilter");
			A.CallTo(() => searcherFactory.CreateSearcher(A<IEntry>.Ignored, SearchScope.Subtree, ldapConfig.Users)).Returns(searcher);

			int totalRecords;
			adapter.GetAllUserEntries(0, 1, out totalRecords);

			Assert.AreEqual("usersfilter", searcher.Filter);
		}

		[Test]
		public void FindUsersByEmail_FindUsersByEmail_SearcherHasEmailFilterForSearch() {
			var searcher = A.Fake<IDirectorySearcher>();
			
			A.CallTo(() => ldapConfig.Users.Filter).Returns("usersfilter");
			A.CallTo(() => ldapConfig.Users.EmailAttribute).Returns("emailattribute");
			A.CallTo(() => searcherFactory.CreateSearcher(A<IEntry>.Ignored, SearchScope.Subtree, ldapConfig.Users)).Returns(searcher);

			int totalRecords;
			adapter.FindUsersByEmail("email", 0, 1, out totalRecords);

			Assert.AreEqual("(&usersfilter(emailattribute=email))", searcher.Filter);
		}

		[Test]
		public void FindUsersByName_FindUsersByName_SearcherHasUserFilterForSearch() {
			var searcher = A.Fake<IDirectorySearcher>();
			
			A.CallTo(() => ldapConfig.Users.Filter).Returns("usersfilter");
			A.CallTo(() => ldapConfig.Users.RdnAttribute).Returns("rdnattribute");
			A.CallTo(() => searcherFactory.CreateSearcher(A<IEntry>.Ignored, SearchScope.Subtree, ldapConfig.Users)).Returns(searcher);

			int totalRecords;
			adapter.FindUsersByName("username", 0, 1, out totalRecords);

			Assert.AreEqual("(&usersfilter(rdnattribute=username))", searcher.Filter);
		}

		[Test]
		public void FindUsersByName_OneUserAttributeThatShallBeIncludedInSearchConfigured_SearcherHasUserFilterWithAttributeForSearch() {
			var searcher = A.Fake<IDirectorySearcher>();
			
			A.CallTo(() => ldapConfig.Users.Filter).Returns("usersfilter");
			A.CallTo(() => ldapConfig.Users.RdnAttribute).Returns("rdnattribute");

			var attribute1 = new AttributeDefinition("includedInSearch", false);
			var attribute2 = new AttributeDefinition("excludedInSearch", true);
			var attributes = new List<IAttributeDefinition>();
			attributes.Add(attribute1);
			attributes.Add(attribute2);

			A.CallTo(() => ldapConfig.Users.Attributes).Returns(attributes);

			A.CallTo(() => searcherFactory.CreateSearcher(A<IEntry>.Ignored, SearchScope.Subtree, ldapConfig.Users)).Returns(searcher);

			int totalRecords;
			adapter.FindUsersByName("username", 0, 1, out totalRecords);

			Assert.AreEqual("(&usersfilter(|(rdnattribute=username)(includedInSearch=username)))", searcher.Filter);
		}

	}

	internal class TestableUserEntryAdapter : UserEntryAdapter {
		public TestableUserEntryAdapter(ILdapConfig configuration, IDirectoryEntryAdapter directoryEntryAdapter, ISearcherFactory searcherFactory, IFilterBuilder filterBuilder, INameParser nameParser) : base(configuration, directoryEntryAdapter, searcherFactory, filterBuilder, nameParser) {}

		public int VirtualListViewTotalCount { get; set; }

		protected override DirectoryVirtualListView CreateVirtualListView(int afterCount, int offset) {
			var baseView = base.CreateVirtualListView(afterCount, offset);
			baseView.ApproximateTotal = VirtualListViewTotalCount;
			return baseView;
		}
	}

}