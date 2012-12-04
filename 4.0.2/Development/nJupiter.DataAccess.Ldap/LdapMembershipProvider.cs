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
using System.Linq;
using System.Web.Security;

using nJupiter.Abstraction.Logging;
using nJupiter.DataAccess.Ldap.Configuration;
using nJupiter.DataAccess.Ldap.DirectoryServices;

namespace nJupiter.DataAccess.Ldap {

	public class LdapMembershipProvider : MembershipProvider {

		private readonly IProviderConfigFactory providerConfigFactory;
		private IProviderConfig providerConfig;
		private IMembershipUserFactory membershipUserFactory;
		private IUsersConfig usersConfig;
		private IUserEntryAdapter userEntryAdapter;
		private ILog<LdapMembershipProvider> log;

		public LdapMembershipProvider() {}

		internal LdapMembershipProvider(IProviderConfigFactory providerConfigFactory) {
			this.providerConfigFactory = providerConfigFactory;
		}

		public override string ApplicationName { get { return providerConfig.ApplicationName; } set { } }
		public override string Name { get { return providerConfig.Name; } }
		protected virtual IProviderConfigFactory ConfigFactory { get { return providerConfigFactory ?? ProviderConfigFactory.Instance; } }

		public override void Initialize(string name, NameValueCollection config) {
			providerConfig = ConfigFactory.Create<LdapMembershipProvider>(name, config);
			membershipUserFactory = providerConfig.MembershipUserFactory;
			usersConfig = providerConfig.LdapConfig.Users;
			userEntryAdapter = providerConfig.LdapConfig.Container.UserEntryAdapter;
			log = providerConfig.LdapConfig.Container.LogManager.GetLogger<LdapMembershipProvider>();
			base.Initialize(providerConfig.Name, config);
		}

		public override bool ValidateUser(string username, string password) {
			try {
				using(var user = userEntryAdapter.GetUserEntry(username, password)) {
					return user.GetProperties(usersConfig.RdnAttribute).Any();
				}
			} catch(Exception ex) {
				log.Debug(c => c(string.Format("Failed to validate user '{0}'.", username), ex));
			}
			return false;
		}

		public override MembershipUser GetUser(string username, bool userIsOnline) {
			using(var user = userEntryAdapter.GetUserEntryAndLoadProperties(username)) {
				return membershipUserFactory.Create(user);
			}
		}
	
		public override MembershipUser GetUser(object providerUserKey, bool userIsOnline) {
			return GetUser(providerUserKey as string, userIsOnline);
		}

		public override string GetUserNameByEmail(string email) {
			using(var user = userEntryAdapter.GetUserEntryByEmail(email)) {
				var membershipUser = membershipUserFactory.Create(user);
				return membershipUser != null ? membershipUser.UserName : null;
			}
		}

		public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords) {
			using(var results = userEntryAdapter.GetAllUserEntries(pageIndex, pageSize, out totalRecords)) {
				return membershipUserFactory.CreateCollection(results);
			}
		}


		public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords) {
			using(var results = userEntryAdapter.FindUsersByName(usernameToMatch, pageIndex, pageSize, out totalRecords)) {
				return membershipUserFactory.CreateCollection(results);
			}
		}

		public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords) {
			using(var results = userEntryAdapter.FindUsersByEmail(emailToMatch, pageIndex, pageSize, out totalRecords)) {
				return membershipUserFactory.CreateCollection(results);
			}
		}

		public override bool ChangePassword(string username, string oldPassword, string newPassword) {
			throw new NotSupportedException();
		}

		public override bool UnlockUser(string userName) {
			throw new NotSupportedException();
		}

		public override void UpdateUser(MembershipUser user) {
			throw new NotSupportedException();
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

		public override string ResetPassword(string username, string answer) {
			throw new NotSupportedException();
		}

		public override bool DeleteUser(string username, bool deleteAllRelatedData) {
			throw new NotSupportedException();
		}

		public override int GetNumberOfUsersOnline() {
			throw new NotSupportedException();
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
	}
}
