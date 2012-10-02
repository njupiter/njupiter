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
using System.Configuration.Provider;
using System.Linq;
using System.Web.Profile;

namespace nJupiter.DataAccess.Users.Web {
	public class ProfileProvider : System.Web.Profile.ProfileProvider {
		private string appName;
		private string providerName;
		private bool automaticallyCreateNonExistingUsers;
		private bool deleteUsersOnProfileDeleting;
		private IUserRepository userRepository;
		private readonly IUserRepositoryManager userRepositoryManager;

		public ProfileProvider() {
			userRepositoryManager = UserRepositoryManager.Instance;
		}

		public ProfileProvider(IUserRepositoryManager userRepositoryManager) {
			this.userRepositoryManager = userRepositoryManager;
		}

		/// <summary>
		/// Gets the userRepository instance associated with this repository.
		/// </summary>
		/// <value>The userRepository instance associated with this repository.</value>
		public IUserRepository UserRepository { get { return userRepository; } }

		public bool AutomaticallyCreateNonExistingUsers { get { return automaticallyCreateNonExistingUsers; } }
		public bool DeleteUsersOnProfileDeleting { get { return deleteUsersOnProfileDeleting; } }

		public override string ApplicationName { get { return appName; } set { appName = value; } }

		public override void Initialize(string name, NameValueCollection config) {
			if(config == null) {
				throw new ArgumentNullException("config");
			}
			var provider = GetStringConfigValue(config, "userRepository", string.Empty);
			userRepository = userRepositoryManager.GetRepository(provider);
			providerName = !string.IsNullOrEmpty(name) ? name : userRepository.Name;
			automaticallyCreateNonExistingUsers =
				bool.Parse(GetStringConfigValue(config, "automaticallyCreateNonExistingUsers", "true"));
			deleteUsersOnProfileDeleting = bool.Parse(GetStringConfigValue(config, "deleteUsersOnProfileDeleting", "true"));
			base.Initialize(providerName, config);
			appName = GetStringConfigValue(config, "applicationName", userRepository.Name);
		}

		protected static string GetUserNameFromMembershipUserName(string membershipUserName) {
			if(membershipUserName.Contains("\\")) {
				return membershipUserName.Substring(membershipUserName.IndexOf("\\") + 1);
			}
			return membershipUserName;
		}

		protected static string GetDomainFromMembershipUserName(string membershipUserName) {
			if(membershipUserName.Contains("\\")) {
				return membershipUserName.Substring(0, membershipUserName.IndexOf("\\"));
			}
			return null;
		}

		public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext sc,
		                                                                  SettingsPropertyCollection properties) {
			var propertySettings = CreateSettingsCollectionFromPropertyCollection(properties);
			var username = (string)sc["UserName"];
			if(!string.IsNullOrEmpty(username) && propertySettings.Count > 0) {
				var user = GetExistingOrCreateNewUser(username);
				PopulatePropertyCollectionFromUser(user, propertySettings);
			}
			return propertySettings;
		}

		public override void SetPropertyValues(SettingsContext sc, SettingsPropertyValueCollection properties) {
			var username = (string)sc["UserName"];
			var isIsAuthenticated = (bool)sc["IsAuthenticated"];
			if(isIsAuthenticated && !string.IsNullOrEmpty(username) && properties.Count > 0) {
				var user = GetUserFromUserNameWritable(username);
				if(user != null) {
					PopulateUserPropertiesFromPropertyCollction(user, properties);
				}
			}
		}

		public override int DeleteProfiles(ProfileInfoCollection profiles) {
			var usernames = (from ProfileInfo info in profiles
			                 select info.UserName).ToArray();
			return DeleteProfiles(usernames);
		}

		public override int DeleteProfiles(string[] usernames) {
			var count = 0;
			if(DeleteUsersOnProfileDeleting) {
				foreach(var username in usernames) {
					var user = GetUserFromUserName(username);
					if(user != null) {
						UserRepository.DeleteUser(user);
						count++;
					}
				}
			}
			return count;
		}

		public override int DeleteInactiveProfiles(ProfileAuthenticationOption authenticationOption,
		                                           DateTime userInactiveSinceDate) {
			return 0;
		}

		public override int GetNumberOfInactiveProfiles(ProfileAuthenticationOption authenticationOption,
		                                                DateTime userInactiveSinceDate) {
			return 0;
		}

		public override ProfileInfoCollection GetAllProfiles(ProfileAuthenticationOption authenticationOption,
		                                                     int pageIndex,
		                                                     int pageSize,
		                                                     out int totalRecords) {
			var profiles = new ProfileInfoCollection();
			totalRecords = 0;
			if(!authenticationOption.Equals(ProfileAuthenticationOption.Anonymous)) {
				var uc = UserRepository.GetAllUsers(pageIndex, pageSize, out totalRecords);
				foreach(var user in uc) {
					var username = GetUsername(user);
					profiles.Add(new ProfileInfo(username,
					                             user.Properties.IsAnonymous,
					                             user.Properties.LastActivityDate,
					                             user.Properties.LastUpdatedDate,
					                             0));
				}
				totalRecords = profiles.Count;
			}
			return profiles;
		}

		public override ProfileInfoCollection GetAllInactiveProfiles(ProfileAuthenticationOption authenticationOption,
		                                                             DateTime userInactiveSinceDate,
		                                                             int pageIndex,
		                                                             int pageSize,
		                                                             out int totalRecords) {
			totalRecords = 0;
			return new ProfileInfoCollection();
		}

		public override ProfileInfoCollection FindProfilesByUserName(ProfileAuthenticationOption authenticationOption,
		                                                             string usernameToMatch,
		                                                             int pageIndex,
		                                                             int pageSize,
		                                                             out int totalRecords) {
			if(usernameToMatch == null) {
				throw new ArgumentNullException("usernameToMatch");
			}
			var pic = new ProfileInfoCollection();
			var user = GetUserFromUserName(usernameToMatch);
			totalRecords = 0;
			if(user != null) {
				var username = GetUsername(user);
				pic.Add(new ProfileInfo(username,
				                        user.Properties.IsAnonymous,
				                        user.Properties.LastActivityDate,
				                        user.Properties.LastUpdatedDate,
				                        0));
				totalRecords = 1;
			}
			return pic;
		}

		private static string GetUsername(IUser user) {
			return string.IsNullOrEmpty(user.Domain) ? user.UserName : string.Format("{0}\\{1}", user.Domain, user.UserName);
		}

		public override ProfileInfoCollection FindInactiveProfilesByUserName(ProfileAuthenticationOption authenticationOption,
		                                                                     string usernameToMatch,
		                                                                     DateTime userInactiveSinceDate,
		                                                                     int pageIndex,
		                                                                     int pageSize,
		                                                                     out int totalRecords) {
			totalRecords = 0;
			return new ProfileInfoCollection();
		}

		private bool SetUserPropertyFromPropertyValue(IUser user, SettingsPropertyValue propertyValue) {
			var propertyChanged = false;
			var userProperty = GetProperty(user, propertyValue.Name);
			if(userProperty != null) {
				if(propertyValue.IsDirty) {
					userProperty.Value = propertyValue.PropertyValue;
					propertyChanged |= userProperty.IsDirty;
				}
			} else {
				throw new ProviderException(
					string.Format("userRepository {0} is not configured to handle a property with the name {1}",
					              UserRepository.Name,
					              propertyValue.Name));
			}
			return propertyChanged;
		}

		private IProperty GetProperty(IUser user, string propertyName) {
			var contextName = UserRepository.PropertyNames.GetContextName(propertyName);
			var context = Context.DefaultContext;
			if(!string.IsNullOrEmpty(contextName)) {
				context = UserRepository.GetContext(contextName);
			}
			return user.Properties.GetProperty(propertyName, context);
		}

		private void PopulateUserPropertiesFromPropertyCollction(IUser user, SettingsPropertyValueCollection properties) {
			var userIsDirty = false;
			foreach(SettingsPropertyValue propertyValue in properties) {
				userIsDirty |= SetUserPropertyFromPropertyValue(user, propertyValue);
			}
			if(userIsDirty) {
				UserRepository.SaveUser(user);
			}
		}

		private void PopulatePropertyCollectionFromUser(IUser user, SettingsPropertyValueCollection propertySettings) {
			foreach(SettingsPropertyValue propertyValue in propertySettings) {
				var userProperty = GetProperty(user, propertyValue.Name);
				if(userProperty != null) {
					propertyValue.PropertyValue = userProperty.Value;
					propertyValue.IsDirty = false;
					propertyValue.Deserialized = true;
				}
			}
		}

		private IUser GetExistingOrCreateNewUser(string username) {
			var user = GetUserFromUserName(username);
			if(user == null && AutomaticallyCreateNonExistingUsers) {
				user = CreateUserFromUserName(username);
			}
			return user;
		}

		private static SettingsPropertyValueCollection CreateSettingsCollectionFromPropertyCollection(
			SettingsPropertyCollection properties) {
			var propertyValues = new SettingsPropertyValueCollection();
			if(properties.Count > 0) {
				foreach(SettingsProperty property in properties) {
					if(property.SerializeAs == SettingsSerializeAs.ProviderSpecific) {
						if(property.PropertyType.IsPrimitive || (property.PropertyType == typeof(string))) {
							property.SerializeAs = SettingsSerializeAs.String;
						} else {
							property.SerializeAs = SettingsSerializeAs.Xml;
						}
					}
					propertyValues.Add(new SettingsPropertyValue(property));
				}
			}
			return propertyValues;
		}

		private IUser CreateUserFromUserName(string username) {
			var name = GetUserNameFromMembershipUserName(username);
			var domain = GetDomainFromMembershipUserName(username);
			var user = UserRepository.CreateUserInstance(name, domain);
			UserRepository.SetPassword(user, Guid.NewGuid().ToString("N"));
			UserRepository.SaveUser(user);
			return user;
		}

		private IUser GetUserFromUserNameWritable(string username) {
			var user = GetUserFromUserName(username);
			if(user != null && user.IsReadOnly) {
				user = user.CreateWritable();
			}
			return user;
		}

		private IUser GetUserFromUserName(string username) {
			var name = GetUserNameFromMembershipUserName(username);
			var domain = GetDomainFromMembershipUserName(username);
			return UserRepository.GetUserByUserName(name, domain);
		}

		private static string GetStringConfigValue(NameValueCollection config, string configKey, string defaultValue) {
			if((config != null) && (config[configKey] != null)) {
				return config[configKey];
			}
			return defaultValue;
		}
	}
}