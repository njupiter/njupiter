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
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;

using nJupiter.DataAccess.Ldap.Configuration;
using nJupiter.DataAccess.Ldap.DirectoryServices.Abstraction;
using nJupiter.DataAccess.Ldap.DistinguishedNames;

namespace nJupiter.DataAccess.Ldap.DirectoryServices {
	internal class UserEntryAdapter : EntryAdapterBase, IUserEntryAdapter {
		private readonly IDirectoryEntryAdapter directoryEntryAdapter;
		private readonly ILdapConfig configuration;
		private readonly INameParser nameHandler;
		private readonly IFilterBuilder filterBuilder;

		public UserEntryAdapter(ILdapConfig configuration,
		                        IDirectoryEntryAdapter directoryEntryAdapter,
		                        ISearcherFactory searcherFactory,
		                        IFilterBuilder filterBuilder,
		                        INameParser nameHandler) : base(searcherFactory) {
			this.configuration = configuration;
			this.directoryEntryAdapter = directoryEntryAdapter;
			this.nameHandler = nameHandler;
			this.filterBuilder = filterBuilder;
		}

		protected override IEntryConfig Config { get { return configuration.Users; } }

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
			if(configuration.Groups.MembershipAttributeNameType != configuration.Users.NameType) {
				using(var entry = GetUserDirectoryEntry(entryName)) {
					entryName = entry.GetProperties<string>(configuration.Users.RdnAttribute).First();
				}
			}
			return nameHandler.GetName(configuration.Users.NameType, entryName);
		}

		public IEnumerable<string> GetUsersFromEntry(IEntry entry, string propertyName) {
			var properties = entry.GetProperties<string>(propertyName);
			return properties.Select(GetUserName);
		}

		private IDirectoryEntry GetUserDirectoryEntry(string username) {
			return directoryEntryAdapter.GetEntry(username, configuration.Users, CreateSearcher);
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
				return filterBuilder.AttachAttributeFilters(usernameToMatch,
				                                            defaultFilter,
				                                            configuration.Users.RdnAttribute,
				                                            configuration.Users.Attributes);
			}
			return filterBuilder.AttachFilter(configuration.Users.RdnAttribute, usernameToMatch, defaultFilter);
		}
	}
}