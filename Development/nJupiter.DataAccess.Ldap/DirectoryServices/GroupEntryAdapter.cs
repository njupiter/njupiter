using System.Collections.Generic;
using System.Configuration.Provider;
using System.DirectoryServices;
using System.Linq;

using nJupiter.DataAccess.Ldap.Configuration;
using nJupiter.DataAccess.Ldap.DirectoryServices.Abstractions;
using nJupiter.DataAccess.Ldap.NameParser;

namespace nJupiter.DataAccess.Ldap.DirectoryServices {
	internal class GroupEntryAdapter : IGroupEntryAdapter {

		private readonly IDirectoryEntryAdapter directoryEntryAdapter;
		private readonly IGroupsConfig serverConfig;
		private readonly INameParser nameHandler;
		private readonly ISearcherFactory searcherFactory;

		public GroupEntryAdapter(	IGroupsConfig serverConfig,
									IDirectoryEntryAdapter directoryEntryAdapter,
									ISearcherFactory searcherFactory,
									INameParser nameHandler) {
			this.serverConfig = serverConfig;
			this.directoryEntryAdapter = directoryEntryAdapter;
			this.nameHandler = nameHandler;
			this.searcherFactory = searcherFactory;
		}

		public IEntry GetGroupEntry(string groupname, bool loadProperties) {
			var entry = GetGroupEntry(groupname);
			return GetSearchedGroupEntry(entry);
		}

		public IEntry GetGroupEntry(string groupname) {
			var groupFilter = CreateGroupFilter();
			return directoryEntryAdapter.GetEntry(serverConfig.RdnAttribute, groupname, serverConfig.Path, groupFilter, CreateSearcher);
		}

		public IEnumerable<string> GetGroupMembersByRangedRetrival(string name) {
			using(var entry = GetGroupEntry(name)) {
				var searcher = GetGroupSearcher(entry, SearchScope.Base);
				return searcher.GetPropertiesByRangedFilter<string>(serverConfig.MembershipAttribute);
			}
		}

		public IEntityCollection GetAllRoleEntries() {
			using(var entry = directoryEntryAdapter.GetEntry(serverConfig.Path)) {
				if(!entry.IsBound()) {
					throw new ProviderException("Could not load role list.");
				}
				var searcher =  GetGroupSearcher(entry, SearchScope.Subtree);
				return searcher.FindAll();
			}
		}

		public string GetGroupName(IEntry entry) {
			var name = entry.GetProperties<string>(serverConfig.RdnAttribute).First();
			return GetGroupName(name);
		}

		public string GetGroupName(string entryName) {
			return nameHandler.GetName(serverConfig.NameType, entryName);
		}

		private IDirectorySearcher GetGroupSearcher(IEntry entry, SearchScope searchScope) {
			var searcher = CreateSearcher(entry, searchScope);
			searcher.Filter = CreateGroupFilter();
			return searcher;
		}

		private IEntry GetSearchedGroupEntry(IEntry entry) {
			if(!entry.IsBound()) {
				return null;
			}
			var searcher = GetGroupSearcher(entry, SearchScope.Base);
			return searcher.FindOne(serverConfig.MembershipAttribute);
		}

		private string CreateGroupFilter() {
			return serverConfig.Filter;
		}

		private IDirectorySearcher CreateSearcher(IEntry entry) {
			return CreateSearcher(entry, SearchScope.Subtree);
		}

		private IDirectorySearcher CreateSearcher(IEntry entry, SearchScope searchScope) {
			var searcher = searcherFactory.CreateSearcher(entry, searchScope, serverConfig.RdnAttribute, serverConfig.Attributes);
			searcher.PropertiesToLoad.Add(serverConfig.MembershipAttribute);
			return searcher;
		}
		
	}
}