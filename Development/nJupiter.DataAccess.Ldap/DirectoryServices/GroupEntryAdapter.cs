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
		private readonly ILdapConfig configuration;
		private readonly INameParser nameHandler;
		private readonly ISearcherFactory searcherFactory;

		public GroupEntryAdapter(ILdapConfig configuration) {
			this.configuration = configuration;
			directoryEntryAdapter = configuration.Container.DirectoryEntryAdapter;
			nameHandler = configuration.Container.NameParser;
			searcherFactory = configuration.Container.SearcherFactory;
		}

		public IEntry GetGroupEntry(string groupname, bool loadProperties) {
			var entry = GetGroupEntry(groupname);
			return GetSearchedGroupEntry(entry);
		}

		public IEntry GetGroupEntry(string groupname) {
			var groupFilter = CreateGroupFilter();
			return directoryEntryAdapter.GetEntry(configuration.Groups.RdnAttribute, groupname, configuration.Groups.Path, groupFilter, CreateSearcher);
		}

		public IEnumerable<string> GetGroupMembersByRangedRetrival(string name) {
			using(var entry = GetGroupEntry(name)) {
				var searcher = GetGroupSearcher(entry, SearchScope.Base);
				return searcher.GetPropertiesByRangedFilter<string>(configuration.Groups.MembershipAttribute);
			}
		}

		public IEntityCollection GetAllRoleEntries() {
			using(var entry = directoryEntryAdapter.GetEntry(configuration.Groups.Path)) {
				if(!entry.IsBound()) {
					throw new ProviderException("Could not load role list.");
				}
				var searcher =  GetGroupSearcher(entry, SearchScope.Subtree);
				return searcher.FindAll();
			}
		}

		public string GetGroupName(IEntry entry) {
			var name = entry.GetProperties<string>(configuration.Groups.RdnAttribute).First();
			return GetGroupName(name);
		}

		public string GetGroupName(string entryName) {
			return nameHandler.GetName(configuration.Groups.NameType, entryName);
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
			return searcher.FindOne(configuration.Groups.MembershipAttribute);
		}

		private string CreateGroupFilter() {
			return configuration.Groups.Filter;
		}

		private IDirectorySearcher CreateSearcher(IEntry entry) {
			return CreateSearcher(entry, SearchScope.Subtree);
		}

		private IDirectorySearcher CreateSearcher(IEntry entry, SearchScope searchScope) {
			var searcher = searcherFactory.CreateSearcher(entry, searchScope, configuration.Groups.RdnAttribute, configuration.Groups.Attributes);
			searcher.PropertiesToLoad.Add(configuration.Groups.MembershipAttribute);
			return searcher;
		}
		
	}
}