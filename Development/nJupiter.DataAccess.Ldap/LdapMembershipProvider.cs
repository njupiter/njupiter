#region Copyright & License
/*
	Copyright (c) 2005-2011 nJupiter

	Permission is hereby granted, free of charge, to any person obtaining a copy
	of this software and associated documentation files (the "Software"), to deal
	in the Software without restriction, including without limitation the rights
	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	copies of the Software, and to permit persons to whom the Software is
	furnished to do so, subject to the following conditions:

	The above copyright notice and this permission notice shall be included in
	all copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
	THE SOFTWARE.
*/
#endregion

using System;
using System.Collections.Specialized;
using System.DirectoryServices;
using System.Web.Security;

using nJupiter.DataAccess.Ldap.Configuration;
using nJupiter.DataAccess.Ldap.NameParser;

namespace nJupiter.DataAccess.Ldap {

	public class LdapMembershipProvider : MembershipProvider {

		private string providerName;
		private string appName;
		private string ldapServer;
		private ILdapConfig configuration;
		private ISearcher userSearcher;
		private ISearcher groupSearcher;
		private IFilterBuilder filterBuilder;
		private IDirectoryEntryAdapter directoryEntryAdapter;
		private LdapMembershipUserFactory ldapMembershipUserFactory;

		public override string ApplicationName { get { return appName; } set { appName = value; } }
		public override string Name {
			get {
				return string.IsNullOrEmpty(providerName) ? GetType().Name : providerName;
			}
		}

		public override bool EnablePasswordRetrieval { get { return false; } }
		public override bool EnablePasswordReset { get { return false; } }
		public override bool RequiresQuestionAndAnswer { get { return false; } }
		public override int MaxInvalidPasswordAttempts { get { return 256; } }
		public override int PasswordAttemptWindow { get { return 1; } }
		public override bool RequiresUniqueEmail { get { return false; } }
		public override MembershipPasswordFormat PasswordFormat { get { return MembershipPasswordFormat.Clear; } }
		public override int MinRequiredPasswordLength { get { return 6; } }
		public override int MinRequiredNonAlphanumericCharacters { get { return 0; } }
		public override string PasswordStrengthRegularExpression { get { return string.Empty; } }

		public override void Initialize(string name, NameValueCollection config) {
			if(config == null) {
				throw new ArgumentNullException("config");
			}
			appName = GetStringConfigValue(config, "applicationName", typeof(LdapMembershipProvider).Name);
			providerName = !string.IsNullOrEmpty(name) ? name : appName;

			ldapServer = GetStringConfigValue(config, "ldapServer", string.Empty);

			configuration = LdapConfigFactory.Instance.GetConfig(ldapServer);
			userSearcher = SearcherFactory.GetSearcher("user", configuration);
			groupSearcher = SearcherFactory.GetSearcher("group", configuration);
			filterBuilder = FilterBuilderFactory.GetInstance(configuration);
			ldapMembershipUserFactory = LdapMembershipUserFactory.GetInstance(configuration);
			directoryEntryAdapter = DirectoryEntryAdapterFactory.GetInstance(configuration, userSearcher, groupSearcher, filterBuilder);

			base.Initialize(providerName, config);
		}

		private static string GetStringConfigValue(NameValueCollection config, string configKey, string defaultValue) {
			if((config != null) && (config[configKey] != null)) {
				return config[configKey];
			}
			return defaultValue;
		}

		public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status) {
			throw new NotSupportedException();
		}

		public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer) {
			throw new NotSupportedException();
		}

		public override string GetPassword(string username, string answer) {
			throw new NotSupportedException();
		}

		public override bool ChangePassword(string username, string oldPassword, string newPassword) {
			throw new NotSupportedException();
		}

		public override string ResetPassword(string username, string answer) {
			throw new NotSupportedException();
		}

		public override void UpdateUser(MembershipUser user) {
			throw new NotSupportedException();
		}

		public override bool ValidateUser(string username, string password) {
			using(var entry = directoryEntryAdapter.GetUserEntry(username)) {
				if(!DirectoryEntryAdapter.IsBound(entry)) {
					return false;
				}
				var dn = DnParser.GetDn(entry.Path);
				var uri = new Uri(configuration.Server.Url, dn);

				try {
					using(var authenticatedUser = directoryEntryAdapter.GetEntry(uri, dn, password)) {
						if(!DirectoryEntryAdapter.IsBound(authenticatedUser)) {
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

		public override bool UnlockUser(string userName) {
			throw new NotSupportedException();
		}

		public override MembershipUser GetUser(object providerUserKey, bool userIsOnline) {
			if(providerUserKey == null) {
				throw new ArgumentNullException("providerUserKey");
			}
			var username = providerUserKey.ToString();
			return GetUser(username, userIsOnline);
		}

		public override MembershipUser GetUser(string username, bool userIsOnline) {
			using(var entry = directoryEntryAdapter.GetUserEntry(username)) {
				if(!DirectoryEntryAdapter.IsBound(entry)) {
					return null;
				}
				var searcher = userSearcher.Create(entry, SearchScope.Base);
				searcher.Filter = filterBuilder.CreateUserFilter();
				return ldapMembershipUserFactory.CreateUserFromSearcher(Name, searcher);
			}
		}

		public override string GetUserNameByEmail(string email) {
			using(var entry = directoryEntryAdapter.GetUsersEntry()) {
				if(!DirectoryEntryAdapter.IsBound(entry)) {
					return null;
				}
				var searcher = userSearcher.Create(entry);
				searcher.Filter = filterBuilder.CreateUserEmailFilter(email);
				var user = ldapMembershipUserFactory.CreateUserFromSearcher(Name, searcher);
				return user != null ? user.UserName : null;
			}
		}

		public override bool DeleteUser(string username, bool deleteAllRelatedData) {
			throw new NotSupportedException();
		}

		public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords) {
			if(pageIndex < 0) {
				throw new ArgumentOutOfRangeException("pageIndex");
			}
			if(pageSize < 1) {
				throw new ArgumentOutOfRangeException("pageSize");
			}
			var users = new MembershipUserCollection();
			using(var entry = directoryEntryAdapter.GetUsersEntry()) {
				if(!DirectoryEntryAdapter.IsBound(entry)) {
					totalRecords = users.Count;
					return users;
				}
				var searcher = userSearcher.Create(entry);
				searcher.Filter = filterBuilder.CreateUserFilter();
				if(configuration.Server.PageSize > 0) {
					searcher.PageSize = pageSize;
				}
				users = ldapMembershipUserFactory.CreateUsersFromSearcher(Name, searcher);
				users = PageUserCollection(users, pageIndex, pageSize, out totalRecords);
			}
			return users;
		}

		public override int GetNumberOfUsersOnline() {
			throw new NotSupportedException();
		}

		public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords) {
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
				if(!DirectoryEntryAdapter.IsBound(entry)) {
					totalRecords = users.Count;
					return users;
				}
				var searcher = userSearcher.Create(entry);
				searcher.Filter = filterBuilder.CreateUserNameFilter(usernameToMatch);
				if(configuration.Server.PageSize > 0) {
					searcher.PageSize = pageSize;
				}
				users = ldapMembershipUserFactory.CreateUsersFromSearcher(Name, searcher);
				users = PageUserCollection(users, pageIndex, pageSize, out totalRecords);
			}
			return users;
		}

		public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords) {
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
				if(!DirectoryEntryAdapter.IsBound(entry)) {
					totalRecords = users.Count;
					return users;
				}
				var searcher = userSearcher.Create(entry);
				searcher.Filter = filterBuilder.CreateUserEmailFilter(emailToMatch);
				if(configuration.Server.PageSize > 0) {
					searcher.PageSize = pageSize;
				}
				users = ldapMembershipUserFactory.CreateUsersFromSearcher(Name, searcher);
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
