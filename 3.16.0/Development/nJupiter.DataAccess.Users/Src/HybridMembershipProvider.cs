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
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Web.Security;

namespace nJupiter.DataAccess.Users {

	public class HybridMembershipProvider : UsersDAOMembershipProvider {

		#region Fields
		private MembershipProvider primaryMembershipProvider;
		private string primaryMembershipProviderName;
		private readonly object padLock = new object();
		#endregion

		#region Private Methods
		private HybridMembershipUser GetHybridMembershipUser(MembershipUser membershipUser) {
			if(membershipUser == null)
				return null;
			string name = GetUserNameFromMembershipUserName(membershipUser.UserName);
			string domain = GetDomainFromMembershipUserName(membershipUser.UserName);
			User user = this.UsersDAO.GetUserByUserName(name, domain);
			if(user == null) {
				lock(padLock) {
					user = this.UsersDAO.GetUserByUserName(name, domain);
					if(user == null) {
						user = this.UsersDAO.CreateUserInstance(name, domain);
						user.Properties.Email = membershipUser.Email;
						this.UsersDAO.SetPassword(user, Guid.NewGuid().ToString("N"));
						try {
							this.UsersDAO.SaveUser(user);
						} catch(UserNameAlreadyExistsException) {
							user = this.UsersDAO.GetUserByUserName(name, domain);
						}
					}
				}
			}
			return new HybridMembershipUser(membershipUser, user, this.Name);
		}

		private HybridMembershipUser GetHybridMembershipUser(string username) {
			MembershipUser membershipUser = this.PrimaryMembershipProvider.GetUser(username, false);
			return GetHybridMembershipUser(membershipUser);
		}

		private MembershipUserCollection GetHybridMembershipUserCollection(MembershipUserCollection membershipUserCollection) {
			if(membershipUserCollection == null)
				return null;
			MembershipUserCollection hybridMembershipUserCollection = new MembershipUserCollection();
			foreach(MembershipUser membershipUser in membershipUserCollection) {
				hybridMembershipUserCollection.Add(this.GetHybridMembershipUser(membershipUser));
			}
			return hybridMembershipUserCollection;
		}

		private static string GetStringConfigValue(NameValueCollection config, string configKey, string defaultValue) {
			if((config != null) && (config[configKey] != null)) {
				return config[configKey];
			}
			return defaultValue;
		}
		#endregion

		public MembershipProvider PrimaryMembershipProvider {
			get {
				if(primaryMembershipProvider == null) {
					primaryMembershipProvider = Membership.Providers[primaryMembershipProviderName];
					if(primaryMembershipProvider == null) {
						throw new ConfigurationErrorsException(string.Format("The MembershipProvider {0} configured as the primary provider does not exist", primaryMembershipProviderName));
					}
				}
				return primaryMembershipProvider;
			}
		}

		#region Overridden Methods
		public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config) {
			primaryMembershipProviderName = HybridMembershipProvider.GetStringConfigValue(config, "primaryMembershipProvider", string.Empty);
			if(string.IsNullOrEmpty(primaryMembershipProviderName)) {
				throw new ConfigurationErrorsException("No primaryMembershipProvider configured");
			}
			base.Initialize(name, config);
		}

		public override bool ChangePassword(string username, string oldPassword, string newPassword) {
			bool result = this.PrimaryMembershipProvider.ChangePassword(username, oldPassword, newPassword);
			if(result) {
				HybridMembershipUser user = GetHybridMembershipUser(username);
				if(user != null) {
					base.ChangePassword(username, oldPassword, newPassword, false);
				}
			}
			return result;

		}

		public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer) {
			bool result = this.PrimaryMembershipProvider.ChangePasswordQuestionAndAnswer(username, password, newPasswordQuestion, newPasswordAnswer);
			if(result) {
				HybridMembershipUser user = GetHybridMembershipUser(username);
				if(user != null) {
					base.ChangePasswordQuestionAndAnswer(username, password, newPasswordQuestion, newPasswordAnswer, false);
				}
			}
			return result;
		}

		private static string HashToString(string text) {
			MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
			byte[] hashArray = md5.ComputeHash(Encoding.Unicode.GetBytes(text));
			return BitConverter.ToString(hashArray).Replace("-", string.Empty);
		}

		public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status) {
			MembershipUser membershipUser = this.PrimaryMembershipProvider.CreateUser(username, password, email, passwordQuestion, passwordAnswer, isApproved, providerUserKey, out status);
			if(membershipUser == null)
				return null;
			if(status.Equals(MembershipCreateStatus.Success)) {
				string userId;
				object providerId = membershipUser.ProviderUserKey ?? membershipUser.UserName;
				try {
					Guid guid = new Guid(providerId.ToString());
					userId = guid.ToString("N");
				} catch(FormatException) {
					userId = HashToString(providerId.ToString());
				}
				base.CreateUser(username, password, email, passwordQuestion, passwordAnswer, isApproved, userId, out status);
				if(!status.Equals(MembershipCreateStatus.Success)) {
					this.PrimaryMembershipProvider.DeleteUser(username, true);
				}
			}
			return this.GetHybridMembershipUser(membershipUser);
		}

		public override bool EnablePasswordReset {
			get {
				return this.PrimaryMembershipProvider.EnablePasswordReset;
			}
		}

		public override bool EnablePasswordRetrieval {
			get {
				return this.PrimaryMembershipProvider.EnablePasswordRetrieval;
			}
		}

		public override bool DeleteUser(string username, bool deleteAllRelatedData) {
			bool result = this.PrimaryMembershipProvider.DeleteUser(username, deleteAllRelatedData);
			if(result) {
				base.DeleteUser(username, deleteAllRelatedData);
			}
			return result;
		}

		public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords) {
			return GetHybridMembershipUserCollection(this.PrimaryMembershipProvider.FindUsersByEmail(emailToMatch, pageIndex, pageSize, out totalRecords));
		}

		public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords) {
			return GetHybridMembershipUserCollection(this.PrimaryMembershipProvider.FindUsersByName(usernameToMatch, pageIndex, pageSize, out totalRecords));
		}

		public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords) {
			return GetHybridMembershipUserCollection(this.PrimaryMembershipProvider.GetAllUsers(pageIndex, pageSize, out totalRecords));
		}

		public override int GetNumberOfUsersOnline() {
			return this.PrimaryMembershipProvider.GetNumberOfUsersOnline();
		}

		public override string GetPassword(string username, string passwordAnswer) {
			return this.PrimaryMembershipProvider.GetPassword(username, passwordAnswer);
		}

		public override MembershipUser GetUser(object providerUserKey, bool userIsOnline) {
			return this.GetHybridMembershipUser(this.PrimaryMembershipProvider.GetUser(providerUserKey, userIsOnline));
		}

		public override int MaxInvalidPasswordAttempts {
			get {
				return this.PrimaryMembershipProvider.MaxInvalidPasswordAttempts;
			}
		}

		public override int MinRequiredNonAlphanumericCharacters {
			get {
				return this.PrimaryMembershipProvider.MinRequiredNonAlphanumericCharacters;
			}
		}

		public override int MinRequiredPasswordLength {
			get {
				return this.PrimaryMembershipProvider.MinRequiredPasswordLength;
			}
		}

		public override int PasswordAttemptWindow {
			get {
				return this.PrimaryMembershipProvider.PasswordAttemptWindow;
			}
		}

		public override MembershipUser GetUser(string username, bool userIsOnline) {
			return this.GetHybridMembershipUser(this.PrimaryMembershipProvider.GetUser(username, userIsOnline));
		}

		public override string GetUserNameByEmail(string email) {
			return this.PrimaryMembershipProvider.GetUserNameByEmail(email);
		}

		public override MembershipPasswordFormat PasswordFormat {
			get {
				return this.PrimaryMembershipProvider.PasswordFormat;
			}
		}

		public override string PasswordStrengthRegularExpression {
			get {
				return this.PrimaryMembershipProvider.PasswordStrengthRegularExpression;
			}
		}

		public override bool RequiresQuestionAndAnswer {
			get {
				return this.PrimaryMembershipProvider.RequiresQuestionAndAnswer;
			}
		}

		public override bool RequiresUniqueEmail {
			get {
				return this.PrimaryMembershipProvider.RequiresUniqueEmail;
			}
		}

		public override string ResetPassword(string username, string passwordAnswer) {
			return this.PrimaryMembershipProvider.ResetPassword(username, passwordAnswer);
		}

		public override bool ValidateUser(string username, string password) {
			bool result = this.PrimaryMembershipProvider.ValidateUser(username, password);
			if(result) {
				if(!base.ValidateUser(username, password, false)) {
					this.GetHybridMembershipUser(username);
				}
			}
			return result;
		}

		public override bool UnlockUser(string username) {
			bool result = this.PrimaryMembershipProvider.UnlockUser(username);
			if(result) {
				HybridMembershipUser user = GetHybridMembershipUser(username);
				if(user != null) {
					base.UnlockUser(username);
				}
			}
			return result;
		}

		public override void UpdateUser(MembershipUser user) {
			HybridMembershipUser hybridMembershipUser = user as HybridMembershipUser;
			if(hybridMembershipUser == null) {
				throw new ArgumentException(string.Format("User is not of type {0}", typeof(HybridMembershipUser).Name), "user");
			}
			this.PrimaryMembershipProvider.UpdateUser(hybridMembershipUser.PrimaryMembershipUser);
			base.UpdateUser(hybridMembershipUser);
		}
		#endregion

	}

}
