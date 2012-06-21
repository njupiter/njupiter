using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;

using nJupiter.DataAccess.Ldap.Configuration;
using nJupiter.DataAccess.Ldap.DirectoryServices.Abstraction;
using nJupiter.DataAccess.Ldap.DistinguishedNames;

namespace nJupiter.DataAccess.Ldap.DirectoryServices {
	internal class UserEntryAdapter : IUserEntryAdapter {
		private readonly IDirectoryEntryAdapter directoryEntryAdapter;
		private readonly ILdapConfig configuration;
		private readonly INameParser nameHandler;
		private readonly ISearcherFactory searcherFactory;
		private readonly IFilterBuilder filterBuilder;

		public UserEntryAdapter(	ILdapConfig configuration,
									IDirectoryEntryAdapter directoryEntryAdapter,
									ISearcherFactory searcherFactory,
									IFilterBuilder filterBuilder,
									INameParser nameHandler) {
			this.configuration = configuration;
			this.directoryEntryAdapter = directoryEntryAdapter;
			this.nameHandler = nameHandler;
			this.searcherFactory = searcherFactory;
			this.filterBuilder = filterBuilder;
		}

		public IEntry GetUserEntry(string username) {
			return GetUserEntry(username, false);
		}

		public IEntry GetUserEntryAndLoadProperties(string username) {
			return GetUserEntry(username, true);
		}

		private IEntry GetUserEntry(string username, bool loadProperties) {
			var user = GetUserDirectoryEntry(username);
			if(!loadProperties) {
				return user;
			}
			return GetUserEntryFromSearcher(user);
		}

		public IEntry GetUserEntryByEmail(string email) {
			using(var entry = directoryEntryAdapter.GetEntry(configuration.Users.Path)) {
				return GetUserEntryFromSearcher(entry, CreateUserEmailFilter(email), SearchScope.Subtree);
			}
		}

		public string GetUserName(string entryName) {
			if(	configuration.Groups.MembershipAttributeNameType != configuration.Users.NameType) {
				using(var entry = GetUserDirectoryEntry(entryName)) {
					entryName = entry.GetProperties<string>(configuration.Users	.RdnAttribute).First();
				}
			}
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

		public IEntry GetUserEntry(string username, string password) {
			using(var user = GetUserEntry(username)) {
				if(!user.IsBound()) {
					return null;
				}
				var dn = nameHandler.GetDn(user.Path);
				var uri = new Uri(configuration.Server.Url, dn);
				var authenticatedUser = directoryEntryAdapter.GetEntry(uri, dn, password);
				return GetUserEntryFromSearcher(authenticatedUser);
			}
		}

		public IEntryCollection GetAllUserEntries(int pageIndex, int pageSize, out int totalRecords) {
			return GetUserEntries(configuration.Users.Filter, pageIndex, pageSize, out totalRecords);
		}

		public IEntryCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords) {
			return GetUserEntries(CreateUserNameFilter(usernameToMatch), pageIndex, pageSize, out totalRecords);
		}

		public IEntryCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords) {
			return GetUserEntries(CreateUserEmailFilter(emailToMatch), pageIndex, pageSize, out totalRecords);
		}

		private IEntryCollection GetUserEntries(string filter, int pageIndex, int pageSize, out int totalRecords) {
			using(var entry = directoryEntryAdapter.GetEntry(configuration.Users.Path)) {
				if(!entry.IsBound()) {
					totalRecords = 0;
					return new EntryCollection();
				}
				var searcher = CreateSearcher(entry);
				searcher.Filter = filter;
				if(configuration.Server.PageSize > 0) {
					searcher.PageSize = pageSize;
				}
				using(var users = searcher.FindAll()) {
					totalRecords = users.Count();
					return users.GetPaged(pageIndex, pageSize);
				}
			}
		}

		private IEntry GetUserEntryFromSearcher(IEntry entry) {
			return GetUserEntryFromSearcher(entry, configuration.Users.Filter, SearchScope.Base);
		}

		private IEntry GetUserEntryFromSearcher(IEntry entry, string filter, SearchScope searchScope) {
			if(!entry.IsBound()) {
				return null;
			}
			var searcher = CreateSearcher(entry, searchScope);
			searcher.Filter = filter;
			return searcher.FindOne();
		}

		private string CreateUserEmailFilter(string emailToMatch) {
			var userFilter = configuration.Users.Filter;
			return filterBuilder.AttachFilter(configuration.Users.EmailAttribute, emailToMatch, userFilter);
		}

		private string CreateUserNameFilter(string usernameToMatch) {
			var defaultFilter = configuration.Users.Filter;
			if(configuration.Users.Attributes.Count > 0) {
				return filterBuilder.AttachAttributeFilters(usernameToMatch, defaultFilter, configuration.Users.RdnAttribute, configuration.Users.Attributes);
			}
			return filterBuilder.AttachFilter(configuration.Users.RdnAttribute, usernameToMatch, defaultFilter);
		}

		private IDirectorySearcher CreateSearcher(IEntry entry) {
			return CreateSearcher(entry, SearchScope.Subtree);
		}

		private IDirectorySearcher CreateSearcher(IEntry entry, SearchScope searchScope) {

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

			if(!string.IsNullOrEmpty(configuration.Users.MembershipAttribute)) {
				searcher.PropertiesToLoad.Add(configuration.Users.MembershipAttribute);
			}
			return searcher;
		}

		
	}
}