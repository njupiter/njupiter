using System;
using System.DirectoryServices;
using System.Linq;
using System.Web.Security;

using nJupiter.DataAccess.Ldap.Configuration;
using nJupiter.DataAccess.Ldap.DirectoryServices.Abstractions;
using nJupiter.DataAccess.Ldap.NameParser;

namespace nJupiter.DataAccess.Ldap.DirectoryServices {
	internal class MembershipAdapter {

		private readonly ILdapConfig configuration;
		private readonly IFilterBuilder filterBuilder;
		private readonly INameParser nameParser;
		private readonly IDirectoryEntryAdapter directoryEntryAdapter;
		private readonly IMembershipUserFactory membershipUserFactory;
		private readonly IUserEntryAdapter userEntryAdapter;

		public MembershipAdapter(ILdapConfig configuration,
		                         IMembershipUserFactory membershipUserFactory) {

			this.configuration = configuration;
			this.membershipUserFactory = membershipUserFactory;
			filterBuilder = configuration.Container.FilterBuilder;
			nameParser = configuration.Container.NameParser;
			directoryEntryAdapter = configuration.Container.DirectoryEntryAdapter;
			userEntryAdapter = configuration.Container.UserEntryAdapter;
		}

		public bool ValidateUser(string username, string password) {
			using(var entry = userEntryAdapter.GetUserEntry(username)) {
				if(!entry.IsBound()) {
					return false;
				}
				var dn = nameParser.GetDn(entry.Path);
				var uri = new Uri(configuration.Server.Url, dn);

				try {
					using(var authenticatedUser = directoryEntryAdapter.GetEntry(uri, dn, password)) {
						if(!authenticatedUser.IsBound()) {
							return false;
						}
						var searcher = userEntryAdapter.CreateSearcher(authenticatedUser, SearchScope.Base);
						searcher.Filter = CreateUserFilter();
						var result = searcher.FindOne();
						if(result != null && result.Properties.Contains(configuration.Users.RdnAttribute)) {
							return result.GetProperties(configuration.Users.RdnAttribute).Any();
						}
					}
				} catch(Exception) {
					// Failed to validate user
				}
			}
			return false;
		}

		public bool UnlockUser(string userName) {
			throw new NotSupportedException();
		}

		public MembershipUser GetUser(object providerUserKey, bool userIsOnline) {
			if(providerUserKey == null) {
				throw new ArgumentNullException("providerUserKey");
			}
			var username = providerUserKey.ToString();
			return GetUser(username, userIsOnline);
		}

		public MembershipUser GetUser(string username, bool userIsOnline) {
			using(var entry = userEntryAdapter.GetUserEntry(username, true)) {
				if(!entry.IsBound()) {
					return null;
				}
				return membershipUserFactory.Create(entry);
			}
		}

		public string GetUserNameByEmail(string email) {
			using(var entry = userEntryAdapter.GetUsersEntry()) {
				if(!entry.IsBound()) {
					return null;
				}
				var searcher = userEntryAdapter.CreateSearcher(entry);
				searcher.Filter = CreateUserEmailFilter(email);
				var result = searcher.FindOne();
				var user = membershipUserFactory.Create(result);
				return user != null ? user.UserName : null;
			}
		}

		public MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords) {
			if(pageIndex < 0) {
				throw new ArgumentOutOfRangeException("pageIndex");
			}
			if(pageSize < 1) {
				throw new ArgumentOutOfRangeException("pageSize");
			}
			var users = new MembershipUserCollection();
			using(var entry = userEntryAdapter.GetUsersEntry()) {
				if(!entry.IsBound()) {
					totalRecords = users.Count;
					return users;
				}
				var searcher = userEntryAdapter.CreateSearcher(entry);
				searcher.Filter = CreateUserFilter();
				if(configuration.Server.PageSize > 0) {
					searcher.PageSize = pageSize;
				}
				users = membershipUserFactory.CreateCollection(searcher.FindAll());
				users = PageUserCollection(users, pageIndex, pageSize, out totalRecords);
			}
			return users;
		}

		public MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords) {
			if(pageIndex < 0) {
				throw new ArgumentOutOfRangeException("pageIndex");
			}
			if(pageSize < 1) {
				throw new ArgumentOutOfRangeException("pageSize");
			}
			var users = new MembershipUserCollection();
			if(string.IsNullOrEmpty(usernameToMatch)) {
				totalRecords = 0;
				return users;
			}
			using(var entry = userEntryAdapter.GetUsersEntry()) {
				if(!entry.IsBound()) {
					totalRecords = users.Count;
					return users;
				}
				var searcher = userEntryAdapter.CreateSearcher(entry);
				searcher.Filter = CreateUserNameFilter(usernameToMatch);
				if(configuration.Server.PageSize > 0) {
					searcher.PageSize = pageSize;
				}
				users = membershipUserFactory.CreateCollection(searcher.FindAll());
				users = PageUserCollection(users, pageIndex, pageSize, out totalRecords);
			}
			return users;
		}

		public MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords) {
			if(pageIndex < 0) {
				throw new ArgumentOutOfRangeException("pageIndex");
			}
			if(pageSize < 1) {
				throw new ArgumentOutOfRangeException("pageSize");
			}

			var users = new MembershipUserCollection();
			if(string.IsNullOrEmpty(emailToMatch)) {
				totalRecords = users.Count;
				return users;
			}
			using(var entry = userEntryAdapter.GetUsersEntry()) {
				if(!entry.IsBound()) {
					totalRecords = users.Count;
					return users;
				}
				var searcher = userEntryAdapter.CreateSearcher(entry);
				searcher.Filter = CreateUserEmailFilter(emailToMatch);
				if(configuration.Server.PageSize > 0) {
					searcher.PageSize = pageSize;
				}
				users = membershipUserFactory.CreateCollection(searcher.FindAll());
				users = PageUserCollection(users, pageIndex, pageSize, out totalRecords);
			}
			return users;
		}

		public string CreateUserNameFilter(string usernameToMatch) {
			var defaultFilter = CreateUserFilter();
			if(configuration.Users.Attributes.Count > 0) {
				return filterBuilder.AttachAttributeFilters(usernameToMatch, defaultFilter, configuration.Users.RdnAttribute, configuration.Users.Attributes);
			}
			return filterBuilder.AttachFilter(configuration.Users.RdnAttribute, usernameToMatch, defaultFilter);
		}

		public string CreateUserEmailFilter(string emailToMatch) {
			var userFilter = CreateUserFilter();
			return filterBuilder.AttachFilter(configuration.Users.EmailAttribute, emailToMatch, userFilter);
		}

		public string CreateUserFilter() {
			return configuration.Users.Filter;
		}

		private static MembershipUserCollection PageUserCollection(MembershipUserCollection userCollection, int pageIndex, int pageSize, out int totalRecords) {
			// TODO: How do I page the users directly on the ldap server?
			var users = new MembershipUserCollection();
			totalRecords = userCollection.Count;
			var index = 0;
			var startIndex = pageIndex * pageSize;
			var endIndex = startIndex + pageSize;
			foreach(MembershipUser user in userCollection) {
				if(index >= startIndex) {
					users.Add(user);
				}
				index++;
				if(index >= endIndex) {
					break;
				}
			}
			return users;
		}
		
	}
}