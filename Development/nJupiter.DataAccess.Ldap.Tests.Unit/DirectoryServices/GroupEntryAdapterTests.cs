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
using System.Configuration.Provider;
using System.DirectoryServices;

using FakeItEasy;

using NUnit.Framework;

using nJupiter.DataAccess.Ldap.Configuration;
using nJupiter.DataAccess.Ldap.DirectoryServices;
using nJupiter.DataAccess.Ldap.DirectoryServices.Abstraction;
using nJupiter.DataAccess.Ldap.DistinguishedNames;

namespace nJupiter.DataAccess.Ldap.Tests.Unit.DirectoryServices {
	
	[TestFixture]
	public class GroupEntryAdapterTests {
		private INameParser nameParser;
		private ISearcherFactory searcherFactory;
		private IDirectoryEntryAdapter directoryEntryAdapter;
		private IGroupsConfig groupConfig;
		private IGroupEntryAdapter adapter;

		[SetUp]
		public void SetUp() {
			nameParser = new NameParser();
			searcherFactory = A.Fake<ISearcherFactory>();
			directoryEntryAdapter = A.Fake<IDirectoryEntryAdapter>();
			groupConfig = A.Fake<IGroupsConfig>();
			adapter = new GroupEntryAdapter(groupConfig, directoryEntryAdapter, searcherFactory, nameParser);
		}

		[Test]
		public void GetGroupEntry_GetEntryByName_EntryLoadedViaAdapter() {
			var directoryEntry = A.Fake<IDirectoryEntry>();
			A.CallTo(() => directoryEntryAdapter.GetEntry("groupname", groupConfig, A<Func<IEntry ,IDirectorySearcher>>.Ignored)).Returns(directoryEntry);
			var result = adapter.GetGroupEntry("groupname");
			Assert.AreSame(directoryEntry, result);
		}

		[Test]
		public void GetGroupEntry_AdapterReturnsNull_ReturnsNull() {
			A.CallTo(() => directoryEntryAdapter.GetEntry("groupname", groupConfig, A<Func<IEntry ,IDirectorySearcher>>.Ignored)).Returns(null);
			var result = adapter.GetGroupEntry("groupname", true);
			Assert.IsNull(result);
		}

		[Test]
		public void GetGroupEntry_GetEntryAndSearchForItViaSearcher_ReturnsSearchedEntry() {
			var directoryEntry = A.Fake<IDirectoryEntry>();
			A.CallTo(() => directoryEntryAdapter.GetEntry("groupname", groupConfig, A<Func<IEntry ,IDirectorySearcher>>.Ignored)).Returns(directoryEntry);
			var searcher = A.Fake<IDirectorySearcher>();
			var searchedEntry = A.Fake<IEntry>();
			A.CallTo(() => searcherFactory.CreateSearcher(directoryEntry, SearchScope.Base, groupConfig)).Returns(searcher);
			A.CallTo(() => searcher.FindOne(groupConfig.MembershipAttribute)).Returns(searchedEntry);
			 
			var result = adapter.GetGroupEntry("groupname", true);

			Assert.AreSame(searchedEntry, result);
		}

		[Test]
		public void GetGroupEntry_GetEntryAndSearchForItViaSearcher_EntrySearcheByGroupFilter() {
			var directoryEntry = A.Fake<IDirectoryEntry>();
			A.CallTo(() => directoryEntryAdapter.GetEntry("groupname", groupConfig, A<Func<IEntry ,IDirectorySearcher>>.Ignored)).Returns(directoryEntry);
			var searcher = A.Fake<IDirectorySearcher>();
			var searchedEntry = A.Fake<IEntry>();
			A.CallTo(() => searcherFactory.CreateSearcher(directoryEntry, SearchScope.Base, groupConfig)).Returns(searcher);
			A.CallTo(() => searcher.FindOne(groupConfig.MembershipAttribute)).Returns(searchedEntry);
			A.CallTo(() => groupConfig.Filter).Returns("groupfilter");
			 
			adapter.GetGroupEntry("groupname", true);

			Assert.AreSame("groupfilter", searcher.Filter);
		}

		[Test]
		public void GetGroupMembersByRangedRetrival_GetMembersFromGruop_MembersLoadedViaSearcherByRangeRetrival() {
			var members = new [] { "username" };
			var directoryEntry = A.Fake<IDirectoryEntry>();
			A.CallTo(() => directoryEntryAdapter.GetEntry("groupname", groupConfig, A<Func<IEntry ,IDirectorySearcher>>.Ignored)).Returns(directoryEntry);

			var searcher = A.Fake<IDirectorySearcher>();
			A.CallTo(() => searcher.GetPropertiesByRangedFilter<string>(groupConfig.MembershipAttribute)).Returns(members);
			A.CallTo(() => searcherFactory.CreateSearcher(directoryEntry, SearchScope.Base, groupConfig)).Returns(searcher);

			var result = adapter.GetGroupMembersByRangedRetrival("groupname");

			Assert.AreSame(members, result);
		}

		[Test]
		public void GetAllRoleEntries_ProviderReturnsNull_ThrowsProviderException() {
			A.CallTo(() => directoryEntryAdapter.GetEntry(groupConfig.Path)).Returns(null);

			Assert.Throws<ProviderException>(() => adapter.GetAllRoleEntries());
		}

		[Test]
		public void GetAllRoleEntries_AllEntriesLoadedViaSearcher_ReturnsEntryCollectionFromSearcher() {
			var entries = A.Fake<IEntryCollection>();
			var directoryEntry = A.Fake<IDirectoryEntry>();
			var searcher = A.Fake<IDirectorySearcher>();
			A.CallTo(() => directoryEntryAdapter.GetEntry(groupConfig.Path)).Returns(directoryEntry);
			A.CallTo(() => searcherFactory.CreateSearcher(directoryEntry, SearchScope.Subtree, groupConfig)).Returns(searcher);
			A.CallTo(() => searcher.FindAll()).Returns(entries);

			var result = adapter.GetAllRoleEntries();

			Assert.AreSame(entries, result);
		}

		[Test]
		public void GetGroupName_CnNameType_ReturnsPlayGroupName() {
			A.CallTo(() => groupConfig.NameType).Returns(NameType.Cn);

			var groupName = adapter.GetGroupName("cn=groupname,dn=domain,o=organistaion");

			Assert.AreEqual("groupname", groupName);
		}


		[Test]
		public void GetGroupName_RdnNameType_ReturnsCnPartOfDn() {
			A.CallTo(() => groupConfig.NameType).Returns(NameType.Rdn);

			var groupName = adapter.GetGroupName("cn=groupname,dn=domain,o=organistaion");

			Assert.AreEqual("cn=groupname", groupName);
		}

		[Test]
		public void GetGroupName_RdnNameType_ReturnsFullGroupName() {
			A.CallTo(() => groupConfig.NameType).Returns(NameType.Dn);

			var groupName = adapter.GetGroupName("cn=groupname,dn=domain,o=organistaion");

			Assert.AreEqual("cn=groupname,dn=domain,o=organistaion", groupName);
		}

		[Test]
		public void GetGroupName_EntryContainsPropertyWithRdn_ReturnGroupName() {
			var entry = A.Fake<IEntry>();
			
			A.CallTo(() => groupConfig.NameType).Returns(NameType.Cn);
			A.CallTo(() => groupConfig.RdnAttribute).Returns("rdnattribute");

			var properties = new Dictionary<string, IEnumerable>();
			properties.Add("rdnattribute", new [] { "cn=groupname, dn=domain, o=organistaion" });

			A.CallTo(() => entry.Properties).Returns(properties);

			var result = adapter.GetGroupName(entry);

			Assert.AreEqual("groupname", result);	
		}
	}
}