using System;
using System.DirectoryServices;
using System.Web.Security;

using nJupiter.DataAccess.Ldap.Abstractions;
using nJupiter.DataAccess.Ldap.Configuration;
using nJupiter.DataAccess.Ldap.NameParser;

namespace nJupiter.DataAccess.Ldap {
	internal class MembershipAdapter {

		private readonly ILdapConfig configuration;
		private readonly ISearcher userSearcher;
		private readonly IFilterBuilder filterBuilder;
		private readonly IDnParser dnParser;
		private readonly IDirectoryEntryAdapter directoryEntryAdapter;
		private readonly IMembershipUserFactory membershipUserFactory;

		public MembershipAdapter(ILdapConfig configuration,
		                         IMembershipUserFactory membershipUserFactory) {

			this.configuration = configuration;
			this.membershipUserFactory = membershipUserFactory;
			userSearcher = configuration.Container.UserSearcher;
			filterBuilder = configuration.Container.FilterBuilder;
			dnParser = configuration.Container.DnParser;
			directoryEntryAdapter = configuration.Container.DirectoryEntryAdapter;
		}

		public bool ValidateUser(string username, string password) {
			using(var entry = directoryEntryAdapter.GetUserEntry(username)) {
				if(!entry.IsBound()) {
					return false;
				}
				var dn = dnParser.GetDn(entry.Path);
				var uri = new Uri(configuration.Server.Url, dn);

				try {
					using(var authenticatedUser = directoryEntryAdapter.GetEntry(uri, dn, password)) {
						if(!authenticatedUser.IsBound()) {
							return false;
						}
						var searcher = userSearcher.Create(authenticatedUser, SearchScope.Base);
						searcher.Filter = filterBuilder.CreateUserFilter();
						var result = searcher.FindOne();
						if(result != null && result.Properties.Contains(configuration.Users.RdnAttribute)) {
							return result.Properties[configuration.Users.RdnAttribute].Count > 0;
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
			using(var entry = directoryEntryAdapter.GetUserEntry(username)) {
				if(!entry.IsBound()) {
					return null;
				}
				var searcher = userSearcher.Create(entry, SearchScope.Base);
				searcher.Filter = filterBuilder.CreateUserFilter();
				return membershipUserFactory.CreateUserFromSearcher(searcher);
			}
		}

		public string GetUserNameByEmail(string email) {
			using(var entry = directoryEntryAdapter.GetUsersEntry()) {
				if(!entry.IsBound()) {
					return null;
				}
				var searcher = userSearcher.Create(entry);
				searcher.Filter = filterBuilder.CreateUserEmailFilter(email);
				var user = membershipUserFactory.CreateUserFromSearcher(searcher);
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
			using(var entry = directoryEntryAdapter.GetUsersEntry()) {
				if(!entry.IsBound()) {
					totalRecords = users.Count;
					return users;
				}
				var searcher = userSearcher.Create(entry);
				searcher.Filter = filterBuilder.CreateUserFilter();
				if(configuration.Server.PageSize > 0) {
					searcher.PageSize = pageSize;
				}
				users = membershipUserFactory.CreateUsersFromSearcher(searcher);
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
			using(var entry = directoryEntryAdapter.GetUsersEntry()) {
				if(!entry.IsBound()) {
					totalRecords = users.Count;
					return users;
				}
				var searcher = userSearcher.Create(entry);
				searcher.Filter = filterBuilder.CreateUserNameFilter(usernameToMatch);
				if(configuration.Server.PageSize > 0) {
					searcher.PageSize = pageSize;
				}
				users = membershipUserFactory.CreateUsersFromSearcher(searcher);
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
			using(var entry = directoryEntryAdapter.GetUsersEntry()) {
				if(!entry.IsBound()) {
					totalRecords = users.Count;
					return users;
				}
				var searcher = userSearcher.Create(entry);
				searcher.Filter = filterBuilder.CreateUserEmailFilter(emailToMatch);
				if(configuration.Server.PageSize > 0) {
					searcher.PageSize = pageSize;
				}
				users = membershipUserFactory.CreateUsersFromSearcher(searcher);
				users = PageUserCollection(users, pageIndex, pageSize, out totalRecords);
			}
			return users;
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