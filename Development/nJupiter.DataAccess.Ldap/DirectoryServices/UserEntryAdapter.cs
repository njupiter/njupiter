using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;

using nJupiter.DataAccess.Ldap.Configuration;
using nJupiter.DataAccess.Ldap.DirectoryServices.Abstractions;
using nJupiter.DataAccess.Ldap.NameParser;

namespace nJupiter.DataAccess.Ldap.DirectoryServices {
	internal class UserEntryAdapter : IUserEntryAdapter {
		private readonly IDirectoryEntryAdapter directoryEntryAdapter;
		private readonly ILdapConfig configuration;
		private readonly INameParser nameHandler;
		private readonly ISearcherFactory searcherFactory;

		public UserEntryAdapter(ILdapConfig configuration) {
			this.configuration = configuration;
			directoryEntryAdapter = configuration.Container.DirectoryEntryAdapter;
			nameHandler = configuration.Container.NameParser;
			searcherFactory = configuration.Container.SearcherFactory;
		}

		public IEntry GetUserEntry(string username) {
			return GetUserEntry(username, false);
		}

		public IEntry GetUserEntry(string username, bool loadProperties) {
			var user = GetUserDirectoryEntry(username);
			if(!loadProperties && user.ContainsProperty(configuration.Users.MembershipAttribute)) {
				return user;
			}
			return GetSearchedUserEntry(user);
		}


		public string GetUserName(IEntry entry) {
			return GetUserName(entry.Name);
		}

		public string GetUserName(string entryName) {
			return nameHandler.GetName(configuration.Users.NameType, entryName);
		}

		public IEnumerable<string> GetUsersFromEntry(IEntry entry, string propertyName) {
			var properties = entry.GetProperties<string>(propertyName);
			return properties.Select(GetUserName);
		}

		private IDirectoryEntry GetUserDirectoryEntry(string username) {
			var userFilter = configuration.Users.Filter;
			return directoryEntryAdapter.GetEntry(configuration.Users.RdnAttribute, username, configuration.Users.Path, userFilter, CreateSearcher);
		}

		[Obsolete("Make this private later")]
		public IDirectoryEntry GetUsersEntry() {
			return directoryEntryAdapter.GetEntry(configuration.Users.Path);
		}

		private IEntry GetSearchedUserEntry(IDirectoryEntry entry) {
			if(!entry.IsBound()) {
				return null;
			}
			var searcher = CreateSearcher(entry, SearchScope.Base);
			searcher.Filter = configuration.Users.Filter;
			return searcher.FindOne();
		}

		[Obsolete("Make this private later")]
		public IDirectorySearcher CreateSearcher(IEntry entry) {
			return CreateSearcher(entry, SearchScope.Subtree);
		}

		[Obsolete("Make this private later")]
		public IDirectorySearcher CreateSearcher(IEntry entry, SearchScope searchScope) {

			var searcher = searcherFactory.CreateSearcher(entry, searchScope, configuration.Users.RdnAttribute, configuration.Users.Attributes);

			searcher.PropertiesToLoad.Add(configuration.Users.EmailAttribute);

			if(!string.IsNullOrEmpty(configuration.Users.CreationDateAttribute)) {
				searcher.PropertiesToLoad.Add(configuration.Users.CreationDateAttribute);
			}
			if(!string.IsNullOrEmpty(configuration.Users.LastLoginDateAttribute)) {
				searcher.PropertiesToLoad.Add(configuration.Users.LastLoginDateAttribute);
			}
			if(!string.IsNullOrEmpty(configuration.Users.LastPasswordChangedDateAttribute)) {
				searcher.PropertiesToLoad.Add(configuration.Users.LastPasswordChangedDateAttribute);
			}
			if(!string.IsNullOrEmpty(configuration.Users.DescriptionAttribute)) {
				searcher.PropertiesToLoad.Add(configuration.Users.DescriptionAttribute);
			}

			searcher.PropertiesToLoad.Add(configuration.Users.MembershipAttribute);
			return searcher;
		}

		
	}
}