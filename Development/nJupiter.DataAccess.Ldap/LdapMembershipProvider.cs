#region Copyright & License
/*
	Copyright (c) 2005-2010 nJupiter

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

using log4net;

using nJupiter.DataAccess.Ldap.NameParser;

namespace nJupiter.DataAccess.Ldap {

	public class LdapMembershipProvider : MembershipProvider {

		private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private string providerName;
		private string appName;
		private string ldapServer;
		private Configuration configuration;
		private Searcher userSeracher;
		private Searcher groupSeracher;
		private FilterBuilder filterBuilder;
		private DirectoryEntryAdapter directoryEntryAdapter;
		private LdapMembershipUserFactory ldapMembershipUserFactory;

		public override string ApplicationName { get { return this.appName; } set { this.appName = value; } }
		public override string Name {
			get {
				return string.IsNullOrEmpty(this.providerName) ? this.GetType().Name : this.providerName;
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
			this.appName = LdapMembershipProvider.GetStringConfigValue(config, "applicationName", typeof(LdapMembershipProvider).GetType().Name);
			this.providerName = !string.IsNullOrEmpty(name) ? name : this.appName;

			this.ldapServer = GetStringConfigValue(config, "ldapServer", string.Empty);

			this.configuration = Configuration.GetConfig(this.ldapServer);
			this.userSeracher = SearcherFactory.GetSearcher("user", configuration);
			this.groupSeracher = SearcherFactory.GetSearcher("group", configuration);
			this.filterBuilder = FilterBuilder.GetInstance(this.configuration);
			this.ldapMembershipUserFactory = LdapMembershipUserFactory.GetInstance(this.configuration);
			this.directoryEntryAdapter = DirectoryEntryAdapter.GetInstance(this.configuration, this.userSeracher, this.groupSeracher, this.filterBuilder);

			base.Initialize(this.providerName, config);
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
			using(DirectoryEntry entry = directoryEntryAdapter.GetUserEntry(username)) {
				if(!DirectoryEntryAdapter.IsBound(entry)) {
					return false;
				}
				string dn = DnParser.GetDn(entry.Path);
				Uri uri = new Uri(configuration.Server.Url, dn);

				try {
					using(DirectoryEntry authenticatedUser = directoryEntryAdapter.GetEntry(uri, dn, password)) {
						if(!DirectoryEntryAdapter.IsBound(authenticatedUser)) {
							return false;
						}
						DirectorySearcher searcher = userSeracher.Create(authenticatedUser, SearchScope.Base);
						searcher.Filter = filterBuilder.CreateUserFilter();
						SearchResult result = searcher.FindOne();
						if(result != null && result.Properties.Contains(configuration.Users.RdnAttribute)) {
							return result.Properties[configuration.Users.RdnAttribute].Count > 0;
						}
					}
				} catch(Exception exeption) {
					if(Log.IsDebugEnabled) { Log.Debug(string.Format("Failed to validate user {0}", username), exeption); }
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
			string username = providerUserKey.ToString();
			return GetUser(username, userIsOnline);
		}

		public override MembershipUser GetUser(string username, bool userIsOnline) {
			using(DirectoryEntry entry = directoryEntryAdapter.GetUserEntry(username)) {
				if(!DirectoryEntryAdapter.IsBound(entry)) {
					return null;
				}
				DirectorySearcher searcher = userSeracher.Create(entry, SearchScope.Base);
				searcher.Filter = filterBuilder.CreateUserFilter();
				return this.ldapMembershipUserFactory.CreateUserFromSearcher(this.Name, searcher);
			}
		}

		public override string GetUserNameByEmail(string email) {
			using(DirectoryEntry entry = directoryEntryAdapter.GetUsersEntry()) {
				if(!DirectoryEntryAdapter.IsBound(entry)) {
					return null;
				}
				DirectorySearcher searcher = this.userSeracher.Create(entry);
				searcher.Filter = filterBuilder.CreateUserEmailFilter(email);
				MembershipUser user = this.ldapMembershipUserFactory.CreateUserFromSearcher(this.Name, searcher);
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
			MembershipUserCollection users = new MembershipUserCollection();
			using(DirectoryEntry entry = directoryEntryAdapter.GetUsersEntry()) {
				if(!DirectoryEntryAdapter.IsBound(entry)) {
					totalRecords = users.Count;
					return users;
				}
				DirectorySearcher searcher = userSeracher.Create(entry);
				searcher.Filter = filterBuilder.CreateUserFilter();
				searcher.PageSize = pageSize;
				users = this.ldapMembershipUserFactory.CreateUsersFromSearcher(this.Name, searcher);
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
			MembershipUserCollection users = new MembershipUserCollection();
			if(string.IsNullOrEmpty(usernameToMatch)) {
				totalRecords = 0;
				return users;
			}
			using(DirectoryEntry entry = directoryEntryAdapter.GetUsersEntry()) {
				if(!DirectoryEntryAdapter.IsBound(entry)) {
					totalRecords = users.Count;
					return users;
				}
				DirectorySearcher searcher = this.userSeracher.Create(entry);
				searcher.Filter = filterBuilder.CreateUserNameFilter(usernameToMatch);
				searcher.PageSize = pageSize;
				users = this.ldapMembershipUserFactory.CreateUsersFromSearcher(this.Name, searcher);
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

			MembershipUserCollection users = new MembershipUserCollection();
			if(string.IsNullOrEmpty(emailToMatch)) {
				totalRecords = users.Count;
				return users;
			}
			using(DirectoryEntry entry = directoryEntryAdapter.GetUsersEntry()) {
				if(!DirectoryEntryAdapter.IsBound(entry)) {
					totalRecords = users.Count;
					return users;
				}
				DirectorySearcher searcher = this.userSeracher.Create(entry);
				searcher.Filter = filterBuilder.CreateUserEmailFilter(emailToMatch);
				searcher.PageSize = pageSize;
				users = this.ldapMembershipUserFactory.CreateUsersFromSearcher(this.Name, searcher);
				users = PageUserCollection(users, pageIndex, pageSize, out totalRecords);
			}
			return users;
		}

		private static MembershipUserCollection PageUserCollection(MembershipUserCollection userCollection, int pageIndex, int pageSize, out int totalRecords) {
			// TODO: How do I page the users directly on the ldap server?
			MembershipUserCollection users = new MembershipUserCollection();
			totalRecords = userCollection.Count;
			int index = 0;
			int startIndex = pageIndex * pageSize;
			int endIndex = startIndex + pageSize;
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
