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
			this.userRepositoryManager = UserRepositoryManager.Instance;
		}

		public ProfileProvider(IUserRepositoryManager userRepositoryManager) {
			this.userRepositoryManager = userRepositoryManager;
		}

		/// <summary>
		/// Gets the userRepository instance associated with this repository.
		/// </summary>
		/// <value>The userRepository instance associated with this repository.</value>
		public IUserRepository UserRepository { get { return this.userRepository; } }
		
		public bool AutomaticallyCreateNonExistingUsers { get { return this.automaticallyCreateNonExistingUsers; } }
		public bool DeleteUsersOnProfileDeleting { get { return this.deleteUsersOnProfileDeleting; } }

		public override string ApplicationName { get { return this.appName; } set { this.appName = value; } }

		public override void Initialize(string name, NameValueCollection config) {
			if(config == null) {
				throw new ArgumentNullException("config");
			}
			string provider = GetStringConfigValue(config, "userRepository", string.Empty);
			this.userRepository = this.userRepositoryManager.GetRepository(provider);
			this.providerName = !string.IsNullOrEmpty(name) ? name : this.userRepository.Name;
			this.automaticallyCreateNonExistingUsers = bool.Parse(GetStringConfigValue(config, "automaticallyCreateNonExistingUsers", "true"));
			this.deleteUsersOnProfileDeleting = bool.Parse(GetStringConfigValue(config, "deleteUsersOnProfileDeleting", "true"));
			base.Initialize(this.providerName, config);
			this.appName = GetStringConfigValue(config, "applicationName", this.userRepository.Name);
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

		public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext sc, SettingsPropertyCollection properties) {
			var propertySettings = CreateSettingsCollectionFromPropertyCollection(properties);
			string username = (string)sc["UserName"];
			if(!string.IsNullOrEmpty(username) && propertySettings.Count > 0) {
				IUser user = GetExistingOrCreateNewUser(username);
				PopulatePropertyCollectionFromUser(user, propertySettings);
			}
			return propertySettings;
		}

		public override void SetPropertyValues(SettingsContext sc, SettingsPropertyValueCollection properties) {
			string username = (string)sc["UserName"];
			bool isIsAuthenticated = (bool)sc["IsAuthenticated"];
			if(isIsAuthenticated && !string.IsNullOrEmpty(username) && properties.Count > 0) {
				IUser user = GetUserFromUserNameWritable(username);
				if(user != null) {
					PopulateUserPropertiesFromPropertyCollction(user, properties);
				}
			}
		}

		public override int DeleteProfiles(ProfileInfoCollection profiles) {
			var usernames = (from ProfileInfo info in profiles select info.UserName).ToArray();
			return this.DeleteProfiles(usernames);
		}

		public override int DeleteProfiles(string[] usernames) {
			int count = 0;
			if(this.DeleteUsersOnProfileDeleting){
				foreach(string username in usernames) {
					IUser user = GetUserFromUserName(username);
					if(user != null) {
						this.UserRepository.DeleteUser(user);
						count++;
					}
				}
			}
			return count;
		}

		public override int DeleteInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate) {
			return 0;
		}

		public override int GetNumberOfInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate) {
			return 0;
		}

		public override ProfileInfoCollection GetAllProfiles(ProfileAuthenticationOption authenticationOption, int pageIndex, int pageSize, out int totalRecords) {
			ProfileInfoCollection profiles = new ProfileInfoCollection();
			totalRecords = 0;
			if(!authenticationOption.Equals(ProfileAuthenticationOption.Anonymous)) {
				var uc = this.UserRepository.GetAllUsers(pageIndex, pageSize, out totalRecords);
				foreach(IUser user in uc) {
					string username = GetUsername(user);
					profiles.Add(new ProfileInfo(username, user.Properties.IsAnonymous, user.Properties.LastActivityDate, user.Properties.LastUpdatedDate, 0));
				}
				totalRecords = profiles.Count;
			}
			return profiles;
		}

		public override ProfileInfoCollection GetAllInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords) {
			totalRecords = 0;
			return new ProfileInfoCollection();
		}

		public override ProfileInfoCollection FindProfilesByUserName(ProfileAuthenticationOption authenticationOption, string usernameToMatch, int pageIndex, int pageSize, out int totalRecords) {
			if(usernameToMatch == null)
				throw new ArgumentNullException("usernameToMatch");
			ProfileInfoCollection pic = new ProfileInfoCollection();
			IUser user = GetUserFromUserName(usernameToMatch);
			totalRecords = 0;
			if(user != null) {
				string username = GetUsername(user);
				pic.Add(new ProfileInfo(username, user.Properties.IsAnonymous, user.Properties.LastActivityDate, user.Properties.LastUpdatedDate, 0));
				totalRecords = 1;
			}
			return pic;
		}

		private static string GetUsername(IUser user) {
			return string.IsNullOrEmpty(user.Domain) ? user.UserName : string.Format("{0}\\{1}", user.Domain, user.UserName);
		}

		public override ProfileInfoCollection FindInactiveProfilesByUserName(ProfileAuthenticationOption authenticationOption, string usernameToMatch, DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords) {
			totalRecords = 0;
			return new ProfileInfoCollection();
		}

		private bool SetUserPropertyFromPropertyValue(IUser user, SettingsPropertyValue propertyValue) {
			bool propertyChanged = false;
			var userProperty = this.GetProperty(user, propertyValue.Name);
			if(userProperty != null) {
				if(propertyValue.IsDirty) {
					userProperty.Value = propertyValue.PropertyValue;
					propertyChanged |= userProperty.IsDirty;
				}
			} else {
				throw new ProviderException(string.Format("userRepository {0} is not configured to handle a property with the name {1}", this.UserRepository.Name, propertyValue.Name));
			}
			return propertyChanged;
		}

		private IProperty GetProperty(IUser user, string propertyName) {
			string contextName = this.UserRepository.PropertyNames.GetContextName(propertyName);
			IContext context = Context.DefaultContext;
			if(!string.IsNullOrEmpty(contextName)) {
				context = this.UserRepository.GetContext(contextName);
			}
			return user.Properties.GetProperty(propertyName, context);
		}

		private void PopulateUserPropertiesFromPropertyCollction(IUser user, SettingsPropertyValueCollection properties) {
			bool userIsDirty = false;
			foreach(SettingsPropertyValue propertyValue in properties) {
				userIsDirty |= this.SetUserPropertyFromPropertyValue(user, propertyValue);
			}
			if(userIsDirty) {
				this.UserRepository.SaveUser(user);
			}
		}

		private void PopulatePropertyCollectionFromUser(IUser user, SettingsPropertyValueCollection propertySettings) {
			foreach(SettingsPropertyValue propertyValue in propertySettings) {
				IProperty userProperty = this.GetProperty(user, propertyValue.Name);
				if(userProperty != null) {
					propertyValue.PropertyValue = userProperty.Value;
					propertyValue.IsDirty = false;
					propertyValue.Deserialized = true;
				}
			}
		}

		private IUser GetExistingOrCreateNewUser(string username) {
			IUser user = this.GetUserFromUserName(username);
			if(user == null && this.AutomaticallyCreateNonExistingUsers) {
				user = this.CreateUserFromUserName(username);
			}
			return user;
		}

		private static SettingsPropertyValueCollection CreateSettingsCollectionFromPropertyCollection(SettingsPropertyCollection properties) {
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
			string name = GetUserNameFromMembershipUserName(username);
			string domain = GetDomainFromMembershipUserName(username);
			var user = this.UserRepository.CreateUserInstance(name, domain);
			this.UserRepository.SetPassword(user, Guid.NewGuid().ToString("N"));
			this.UserRepository.SaveUser(user);
			return user;
		}

		private IUser GetUserFromUserNameWritable(string username) {
			var user = GetUserFromUserName(username);
			if(user != null && user.IsReadOnly) {
				user = (IUser)user.Clone();
			}
			return user;
		}



		private IUser GetUserFromUserName(string username) {
			string name = GetUserNameFromMembershipUserName(username);
			string domain = GetDomainFromMembershipUserName(username);
			return this.UserRepository.GetUserByUserName(name, domain);
		}

		private static string GetStringConfigValue(NameValueCollection config, string configKey, string defaultValue) {
			if((config != null) && (config[configKey] != null)) {
				return config[configKey];
			}
			return defaultValue;
		}
	}
}

