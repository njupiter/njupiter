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

using System.Collections.Generic;
using System.Configuration.Provider;
using System.DirectoryServices;
using System.Linq;

using nJupiter.DataAccess.Ldap.Configuration;
using nJupiter.DataAccess.Ldap.DirectoryServices.Abstraction;
using nJupiter.DataAccess.Ldap.DistinguishedNames;

namespace nJupiter.DataAccess.Ldap.DirectoryServices {
	public class GroupEntryAdapter : EntryAdapterBase, IGroupEntryAdapter {
		private readonly IDirectoryEntryAdapter directoryEntryAdapter;
		private readonly IGroupsConfig groupConfig;
		private readonly INameParser nameParser;
		private readonly IFilterBuilder filterBuilder;

		public GroupEntryAdapter(IGroupsConfig groupConfig,
		                         IDirectoryEntryAdapter directoryEntryAdapter,
		                         ISearcherFactory searcherFactory,
								 IFilterBuilder filterBuilder,
		                         INameParser nameParser) : base(searcherFactory) {
			this.groupConfig = groupConfig;
			this.directoryEntryAdapter = directoryEntryAdapter;
			this.filterBuilder = filterBuilder;
			this.nameParser = nameParser;
		}

		protected override IEntryConfig Config { get { return groupConfig; } }

		public virtual IEntry GetGroupEntry(string groupname, bool loadProperties) {
			var entry = GetGroupEntry(groupname);
			return GetSearchedGroupEntry(entry);
		}

		public virtual IEntry GetGroupEntry(string groupname) {
			return directoryEntryAdapter.GetEntry(groupname, groupConfig, CreateSearcher);
		}

		public virtual IEnumerable<string> GetGroupMembersByRangedRetrival(string name) {
			using(var entry = GetGroupEntry(name)) {
				if(!entry.IsBound()) {
					return new string[0];
				}
				var searcher = GetGroupSearcher(entry, SearchScope.Base);
				return searcher.GetPropertiesByRangedFilter<string>(groupConfig.MembershipAttribute);
			}
		}

		public virtual IEntryCollection GetAllRoleEntries() {
			using(var entry = GetGroupEntry()) {
				var searcher = GetGroupSearcher(entry, SearchScope.Subtree);
				return searcher.FindAll();
			}
		}

		public virtual string GetGroupName(IEntry entry) {
			var name = entry.GetProperties<string>(groupConfig.RdnAttribute).First();
			return GetGroupName(name, true);
		}

		public virtual string GetGroupName(string entryName) {
			return GetGroupName(entryName, false);
		}

		private string GetGroupName(string entryName, bool nameFromEntry) {
			if(!groupConfig.RdnInPath && !nameFromEntry) {
				using(var entry = GetGroupEntry(entryName)) {
					entryName = entry.GetProperties<string>(groupConfig.RdnAttribute).First();
				}
			}
			return nameParser.GetName(groupConfig.NameType, entryName);
		}

		public virtual IEntryCollection GetGroupsWithEntryAsMemebership(IEntry membershipEntry) {
			using(var entry = GetGroupEntry()) {
				var searcher = GetGroupSearcher(entry, SearchScope.Subtree);
				var mebershipValue = nameParser.GetDn(membershipEntry.Path);
				searcher.Filter = filterBuilder.AttachFilter(groupConfig.MembershipAttribute, mebershipValue, groupConfig.Filter);
				return searcher.FindAll();
			}
		}

		private IEntry GetGroupEntry() {
			var entry = directoryEntryAdapter.GetEntry(groupConfig.Path);
			if(!entry.IsBound()) {
				throw new ProviderException("Could not load role list.");
			}
			return entry;
		}

		private IEntry GetSearchedGroupEntry(IEntry entry) {
			if(!entry.IsBound()) {
				return null;
			}
			var searcher = GetGroupSearcher(entry, SearchScope.Base);
			return searcher.FindOne(groupConfig.MembershipAttribute);
		}

		private IDirectorySearcher GetGroupSearcher(IEntry entry, SearchScope searchScope) {
			var searcher = CreateSearcher(entry, searchScope);
			searcher.Filter = groupConfig.Filter;
			return searcher;
		}

	}
}