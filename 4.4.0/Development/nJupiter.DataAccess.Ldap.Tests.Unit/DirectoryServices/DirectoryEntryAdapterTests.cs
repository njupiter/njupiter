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

using FakeItEasy;

using NUnit.Framework;

using nJupiter.DataAccess.Ldap.Configuration;
using nJupiter.DataAccess.Ldap.DirectoryServices;
using nJupiter.DataAccess.Ldap.DirectoryServices.Abstraction;
using nJupiter.DataAccess.Ldap.DistinguishedNames;

namespace nJupiter.DataAccess.Ldap.Tests.Unit.DirectoryServices {
	
	[TestFixture]
	public class DirectoryEntryAdapterTests {
		private IServerConfig serverConfig;
		private IDirectoryEntryFactory directoryEntryFactory;
		private IFilterBuilder filterBuilde;
		private INameParser nameParser;
		private IDirectoryEntryAdapter adapter;

		[SetUp]
		public void SetUp() {
			serverConfig = A.Fake<IServerConfig>();
			directoryEntryFactory = A.Fake<IDirectoryEntryFactory>();
			filterBuilde = new FilterBuilder(serverConfig);
			nameParser = new NameParser();
			adapter = new DirectoryEntryAdapter(serverConfig, directoryEntryFactory, filterBuilde, nameParser);
		}

		[Test]
		public void GetEntry_ByPath_EntryCreatedByPath() {
			adapter.GetEntry("anypath");

			A.CallTo(()  => directoryEntryFactory.Create(	"anypath", 
															serverConfig.Username,
															serverConfig.Password,
															serverConfig.AuthenticationTypes)).MustHaveHappened(Repeated.Exactly.Once);
		}

		[Test]
		public void GetEntry_ByUriAndUsernameAndPassword_EntryCreatedByUriWithUsernameAndPassword() {
			var uri = new Uri("ldap://anyuri/");

			adapter.GetEntry(uri, "username", "password");

			A.CallTo(()  => directoryEntryFactory.Create(	"LDAP://anyuri/",
															"username",
															"password",
															serverConfig.AuthenticationTypes)).MustHaveHappened(Repeated.Exactly.Once);
		}

		[Test]
		public void GetEntry_SendInAttributeAsCn_EntryFetchedBySeracherWithCorrectFilter() {
			var searcher = A.Fake<IDirectorySearcher>();
			var config = A.Fake<IEntryConfig>();
			A.CallTo(() => config.Filter).Returns("(any=filter)");
			adapter.GetEntry("cn=attributeValue", config, e => searcher);

			Assert.AreEqual("(&(any=filter)(cn=attributeValue))", searcher.Filter);
		}

		[Test]
		public void GetEntry_SendInAttributeAsPlainValue_EntryFetchedBySeracherWithCorrectFilter() {
			var searcher = A.Fake<IDirectorySearcher>();
			var config = A.Fake<IEntryConfig>();
			A.CallTo(() => config.RdnAttribute).Returns("attributeName");
			A.CallTo(() => config.Filter).Returns("(any=filter)");

			adapter.GetEntry("attributeValue", config, e => searcher);

			Assert.AreEqual("(&(any=filter)(attributeName=attributeValue))", searcher.Filter);
		}

		[Test]
		public void GetEntry_SendInPath_DefaultEntryFetchedFromPath() {
			var searcher = A.Fake<IDirectorySearcher>();
			var config = A.Fake<IEntryConfig>();
			A.CallTo(() => config.Path).Returns("anypath");

			adapter.GetEntry("attributeValue", config, e => searcher);

			A.CallTo(()  => directoryEntryFactory.Create(	"anypath",
															serverConfig.Username,
															serverConfig.Password,
															serverConfig.AuthenticationTypes)).MustHaveHappened(Repeated.Exactly.Once);

		}

		[Test]
		public void GetEntry_SendInAttribute_DirectoryEntryReturnedFromSearcher() {
			var directoryEntry = A.Fake<IDirectoryEntry>();
			var searcher = A.Fake<IDirectorySearcher>();
			var entry = A.Fake<IEntry>();
			A.CallTo(() => entry.GetDirectoryEntry()).Returns(directoryEntry);
			A.CallTo(() => searcher.FindOne()).Returns(entry);

			var config = A.Fake<IEntryConfig>();

			var result = adapter.GetEntry("attributeValue", config, e => searcher);

			Assert.AreSame(directoryEntry, result);
		}

		[Test]
		public void GetEntry_SearcherReturnsNull_ReturnsNullEntry() {
			var searcher = A.Fake<IDirectorySearcher>();
			var config = A.Fake<IEntryConfig>();
			A.CallTo(() => searcher.FindOne()).Returns(null);
			var result = adapter.GetEntry("attributeValue", config, e => searcher);

			Assert.IsNull(result);
		}

		[Test]
		public void GetEntry_SendInNotNullAttributeValue_ReturnsEntry() {
			var searcher = A.Fake<IDirectorySearcher>();
			var config = A.Fake<IEntryConfig>();
			A.CallTo(() => searcher.FindOne()).Returns(A.Fake<IEntry>());
			var result = adapter.GetEntry("attributeValue", config, e => searcher);

			Assert.IsNotNull(result);
		}

		[Test]
		public void GetEntry_SendInNullAttributeValue_ReturnsNullEntry() {
			var searcher = A.Fake<IDirectorySearcher>();
			var config = A.Fake<IEntryConfig>();
			A.CallTo(() => searcher.FindOne()).Returns(A.Fake<IEntry>());
			var result = adapter.GetEntry(null, config, e => searcher);

			Assert.IsNull(result);
		}

		[Test]
		public void GetEntry_SendInAttributeAsDn_EntryCreatedWithCorrectPath() {
			var searcher = A.Fake<IDirectorySearcher>();

			var config = A.Fake<IEntryConfig>();
			A.CallTo(() => config.RdnAttribute).Returns("attributeName");
			A.CallTo(() => config.Path).Returns("ldap://anypath/cn=any,cn=subpath");

			adapter.GetEntry("cn=any,o=distinguished,dc=name", config, e => searcher);

			A.CallTo(()  => directoryEntryFactory.Create(	"LDAP://anypath/cn=any,o=distinguished,dc=name",
															serverConfig.Username,
															serverConfig.Password,
															serverConfig.AuthenticationTypes)).MustHaveHappened(Repeated.Exactly.Once);
		}

	}

}