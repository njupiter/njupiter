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
using System.Collections.Specialized;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Web.Security;

namespace nJupiter.DataAccess.Users.Web {
	public class HybridMembershipProvider : MembershipProvider {
		private System.Web.Security.MembershipProvider primaryMembershipProvider;
		private string primaryMembershipProviderName;
		private readonly object padLock = new object();

		private HybridMembershipUser GetHybridMembershipUser(System.Web.Security.MembershipUser membershipUser) {
			if(membershipUser == null) {
				return null;
			}
			var name = GetUserNameFromMembershipUserName(membershipUser.UserName);
			var domain = GetDomainFromMembershipUserName(membershipUser.UserName);
			var user = UserRepository.GetUserByUserName(name, domain);
			if(user == null) {
				lock(padLock) {
					user = UserRepository.GetUserByUserName(name, domain);
					if(user == null) {
						user = UserRepository.CreateUserInstance(name, domain);
						user.Properties.Email = membershipUser.Email;
						UserRepository.SetPassword(user, Guid.NewGuid().ToString("N"));
						try {
							UserRepository.SaveUser(user);
						} catch(UserNameAlreadyExistsException) {
							user = UserRepository.GetUserByUserName(name, domain);
						}
					}
				}
			}
			return new HybridMembershipUser(membershipUser, user, Name);
		}

		private HybridMembershipUser GetHybridMembershipUser(string username) {
			var membershipUser = PrimaryMembershipProvider.GetUser(username, false);
			return GetHybridMembershipUser(membershipUser);
		}

		private MembershipUserCollection GetHybridMembershipUserCollection(MembershipUserCollection membershipUserCollection) {
			if(membershipUserCollection == null) {
				return null;
			}
			var hybridMembershipUserCollection = new MembershipUserCollection();
			foreach(System.Web.Security.MembershipUser membershipUser in membershipUserCollection) {
				hybridMembershipUserCollection.Add(GetHybridMembershipUser(membershipUser));
			}
			return hybridMembershipUserCollection;
		}

		private static string GetStringConfigValue(NameValueCollection config, string configKey, string defaultValue) {
			if((config != null) && (config[configKey] != null)) {
				return config[configKey];
			}
			return defaultValue;
		}

		public System.Web.Security.MembershipProvider PrimaryMembershipProvider {
			get {
				if(primaryMembershipProvider == null) {
					primaryMembershipProvider = Membership.Providers[primaryMembershipProviderName];
					if(primaryMembershipProvider == null) {
						throw new ConfigurationErrorsException(
							string.Format("The MembershipProvider {0} configured as the primary repository does not exist",
							              primaryMembershipProviderName));
					}
				}
				return primaryMembershipProvider;
			}
		}

		public override void Initialize(string name, NameValueCollection config) {
			primaryMembershipProviderName = GetStringConfigValue(config, "primaryMembershipProvider", string.Empty);
			if(string.IsNullOrEmpty(primaryMembershipProviderName)) {
				throw new ConfigurationErrorsException("No primaryMembershipProvider configured");
			}
			base.Initialize(name, config);
		}

		public override bool ChangePassword(string username, string oldPassword, string newPassword) {
			var result = PrimaryMembershipProvider.ChangePassword(username, oldPassword, newPassword);
			if(result) {
				var user = GetHybridMembershipUser(username);
				if(user != null) {
					base.ChangePassword(username, oldPassword, newPassword, false);
				}
			}
			return result;
		}

		public override bool ChangePasswordQuestionAndAnswer(string username,
		                                                     string password,
		                                                     string newPasswordQuestion,
		                                                     string newPasswordAnswer) {
			var result = PrimaryMembershipProvider.ChangePasswordQuestionAndAnswer(username,
			                                                                        password,
			                                                                        newPasswordQuestion,
			                                                                        newPasswordAnswer);
			if(result) {
				var user = GetHybridMembershipUser(username);
				if(user != null) {
					base.ChangePasswordQuestionAndAnswer(username, password, newPasswordQuestion, newPasswordAnswer, false);
				}
			}
			return result;
		}

		private static string HashToString(string text) {
			var md5 = new MD5CryptoServiceProvider();
			var hashArray = md5.ComputeHash(Encoding.Unicode.GetBytes(text));
			return BitConverter.ToString(hashArray).Replace("-", string.Empty);
		}

		public override System.Web.Security.MembershipUser CreateUser(string username,
		                                                              string password,
		                                                              string email,
		                                                              string passwordQuestion,
		                                                              string passwordAnswer,
		                                                              bool isApproved,
		                                                              object providerUserKey,
		                                                              out MembershipCreateStatus status) {
			var membershipUser = PrimaryMembershipProvider.CreateUser(username,
			                                                          password,
			                                                          email,
			                                                          passwordQuestion,
			                                                          passwordAnswer,
			                                                          isApproved,
			                                                          providerUserKey,
			                                                          out status);
			if(membershipUser == null) {
				return null;
			}
			if(status.Equals(MembershipCreateStatus.Success)) {
				string userId;
				var providerId = membershipUser.ProviderUserKey ?? membershipUser.UserName;
				try {
					var guid = new Guid(providerId.ToString());
					userId = guid.ToString("N");
				} catch(FormatException) {
					userId = HashToString(providerId.ToString());
				}
				base.CreateUser(username, password, email, passwordQuestion, passwordAnswer, isApproved, userId, out status);
				if(!status.Equals(MembershipCreateStatus.Success)) {
					PrimaryMembershipProvider.DeleteUser(username, true);
				}
			}
			return GetHybridMembershipUser(membershipUser);
		}

		public override bool EnablePasswordReset { get { return PrimaryMembershipProvider.EnablePasswordReset; } }

		public override bool EnablePasswordRetrieval { get { return PrimaryMembershipProvider.EnablePasswordRetrieval; } }

		public override bool DeleteUser(string username, bool deleteAllRelatedData) {
			var result = PrimaryMembershipProvider.DeleteUser(username, deleteAllRelatedData);
			if(result) {
				base.DeleteUser(username, deleteAllRelatedData);
			}
			return result;
		}

		public override MembershipUserCollection FindUsersByEmail(string emailToMatch,
		                                                          int pageIndex,
		                                                          int pageSize,
		                                                          out int totalRecords) {
			return
				GetHybridMembershipUserCollection(PrimaryMembershipProvider.FindUsersByEmail(emailToMatch,
				                                                                             pageIndex,
				                                                                             pageSize,
				                                                                             out totalRecords));
		}

		public override MembershipUserCollection FindUsersByName(string usernameToMatch,
		                                                         int pageIndex,
		                                                         int pageSize,
		                                                         out int totalRecords) {
			return
				GetHybridMembershipUserCollection(PrimaryMembershipProvider.FindUsersByName(usernameToMatch,
				                                                                            pageIndex,
				                                                                            pageSize,
				                                                                            out totalRecords));
		}

		public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords) {
			return GetHybridMembershipUserCollection(PrimaryMembershipProvider.GetAllUsers(pageIndex, pageSize, out totalRecords));
		}

		public override int GetNumberOfUsersOnline() {
			return PrimaryMembershipProvider.GetNumberOfUsersOnline();
		}

		public override string GetPassword(string username, string passwordAnswer) {
			return PrimaryMembershipProvider.GetPassword(username, passwordAnswer);
		}

		public override System.Web.Security.MembershipUser GetUser(object providerUserKey, bool userIsOnline) {
			return GetHybridMembershipUser(PrimaryMembershipProvider.GetUser(providerUserKey, userIsOnline));
		}

		public override int MaxInvalidPasswordAttempts { get { return PrimaryMembershipProvider.MaxInvalidPasswordAttempts; } }

		public override int MinRequiredNonAlphanumericCharacters { get { return PrimaryMembershipProvider.MinRequiredNonAlphanumericCharacters; } }

		public override int MinRequiredPasswordLength { get { return PrimaryMembershipProvider.MinRequiredPasswordLength; } }

		public override int PasswordAttemptWindow { get { return PrimaryMembershipProvider.PasswordAttemptWindow; } }

		public override System.Web.Security.MembershipUser GetUser(string username, bool userIsOnline) {
			return GetHybridMembershipUser(PrimaryMembershipProvider.GetUser(username, userIsOnline));
		}

		public override string GetUserNameByEmail(string email) {
			return PrimaryMembershipProvider.GetUserNameByEmail(email);
		}

		public override MembershipPasswordFormat PasswordFormat { get { return PrimaryMembershipProvider.PasswordFormat; } }

		public override string PasswordStrengthRegularExpression { get { return PrimaryMembershipProvider.PasswordStrengthRegularExpression; } }

		public override bool RequiresQuestionAndAnswer { get { return PrimaryMembershipProvider.RequiresQuestionAndAnswer; } }

		public override bool RequiresUniqueEmail { get { return PrimaryMembershipProvider.RequiresUniqueEmail; } }

		public override string ResetPassword(string username, string passwordAnswer) {
			return PrimaryMembershipProvider.ResetPassword(username, passwordAnswer);
		}

		public override bool ValidateUser(string username, string password) {
			var result = PrimaryMembershipProvider.ValidateUser(username, password);
			if(result) {
				if(!base.ValidateUser(username, password, false)) {
					GetHybridMembershipUser(username);
				}
			}
			return result;
		}

		public override bool UnlockUser(string username) {
			var result = PrimaryMembershipProvider.UnlockUser(username);
			if(result) {
				var user = GetHybridMembershipUser(username);
				if(user != null) {
					base.UnlockUser(username);
				}
			}
			return result;
		}

		public override void UpdateUser(System.Web.Security.MembershipUser user) {
			var hybridMembershipUser = user as HybridMembershipUser;
			if(hybridMembershipUser == null) {
				throw new ArgumentException(string.Format("User is not of type {0}", typeof(HybridMembershipUser).Name), "user");
			}
			PrimaryMembershipProvider.UpdateUser(hybridMembershipUser.PrimaryMembershipUser);
			base.UpdateUser(hybridMembershipUser);
		}
	}
}