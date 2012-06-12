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
using System.Web.Security;

using nJupiter.DataAccess.Ldap.Configuration;

namespace nJupiter.DataAccess.Ldap {

	public class LdapMembershipProvider : MembershipProvider {

		private string providerName;
		private string appName;
		private string ldapServer;

		private MembershipAdapter membershipAdapter;

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

			var configuration = LdapConfigFactory.Instance.GetConfig(ldapServer);
			var ldapMembershipUserFactory = new LdapMembershipUserFactory(providerName, configuration);
			membershipAdapter = new MembershipAdapter(configuration, ldapMembershipUserFactory);

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
			return membershipAdapter.ValidateUser(username, password);
		}

		public override bool UnlockUser(string userName) {
			throw new NotSupportedException();
		}

		public override MembershipUser GetUser(object providerUserKey, bool userIsOnline) {
			return membershipAdapter.GetUser(providerUserKey, userIsOnline);
		}

		public override MembershipUser GetUser(string username, bool userIsOnline) {
			return membershipAdapter.GetUser(username, userIsOnline);
		}

		public override string GetUserNameByEmail(string email) {
			return membershipAdapter.GetUserNameByEmail(email);
		}

		public override bool DeleteUser(string username, bool deleteAllRelatedData) {
			throw new NotSupportedException();
		}

		public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords) {
			return membershipAdapter.GetAllUsers(pageIndex, pageSize, out totalRecords);
		}

		public override int GetNumberOfUsersOnline() {
			throw new NotSupportedException();
		}

		public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords) {
			return membershipAdapter.FindUsersByName(usernameToMatch, pageIndex, pageSize, out totalRecords);
		}

		public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords) {
			return membershipAdapter.FindUsersByEmail(emailToMatch, pageIndex, pageSize, out totalRecords);
		}
	}
}
