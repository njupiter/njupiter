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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.DirectoryServices;
using System.Linq;
using System.Web.Security;

using nJupiter.DataAccess.Ldap.Configuration;

namespace nJupiter.DataAccess.Ldap {

	public class LdapRoleProvider : RoleProvider {
		
		private string providerName;
		private string appName;
		private string ldapServer;
		private ILdapConfig configuration;
		private ISearcher userSearcher;
		private ISearcher groupSearcher;
		private IFilterBuilder filterBuilder;
		private IDirectoryEntryAdapter directoryEntryAdapter;
		private ILdapNameHandler ldapNameHandler;

		public override string ApplicationName { get { return appName; } set { appName = value; } }
		public override string Name {
			get {
				return string.IsNullOrEmpty(providerName) ? GetType().Name : providerName;
			}
		}

		public override void Initialize(string name, NameValueCollection config) {
			if(config == null) {
				throw new ArgumentNullException("config");
			}
			appName = GetStringConfigValue(config, "applicationName", typeof(LdapRoleProvider).Name);
			providerName = !string.IsNullOrEmpty(name) ? name : appName;

			ldapServer = GetStringConfigValue(config, "ldapServer", string.Empty);

			configuration = LdapConfigFactory.Instance.GetConfig(ldapServer);
			userSearcher = SearcherFactory.GetSearcher("user", configuration);
			groupSearcher = SearcherFactory.GetSearcher("group", configuration);
			filterBuilder = FilterBuilderFactory.GetInstance(configuration);
			ldapNameHandler = LdapNameHandlerFactory.GetInstance(configuration);
			directoryEntryAdapter = DirectoryEntryAdapterFactory.GetInstance(configuration, userSearcher, groupSearcher, filterBuilder);

			base.Initialize(providerName, config);
		}

		private static string GetStringConfigValue(NameValueCollection config, string configKey, string defaultValue) {
			if((config != null) && (config[configKey] != null)) {
				return config[configKey];
			}
			return defaultValue;
		}

		public override bool IsUserInRole(string username, string roleName) {
			if(username == null) {
				throw new ArgumentNullException("username");
			}
			if(roleName == null) {
				throw new ArgumentNullException("roleName");
			}
			using(var entry = directoryEntryAdapter.GetUserEntry(username)) {
				if(!DirectoryEntryAdapter.IsBound(entry)) {
					return false;
				}
				if(entry.Properties.Contains(configuration.Users.MembershipAttribute)) {
					foreach(var groupObject in entry.Properties[configuration.Users.MembershipAttribute]) {
						var group = GetPropertyValue(groupObject);
						if(ldapNameHandler.GroupsEqual(group, roleName)) {
							return true;
						}
					}
				} else {
					var searcher = userSearcher.Create(entry, SearchScope.Base);
					searcher.Filter = filterBuilder.CreateUserFilter();
					var result = searcher.FindOne();
					if((result != null) && result.Properties.Contains(configuration.Users.MembershipAttribute)) {
						foreach(var groupObject in result.Properties[configuration.Users.MembershipAttribute]) {
							var group = GetPropertyValue(groupObject);
							if(ldapNameHandler.GroupsEqual(group, roleName)) {
								return true;
							}
						}
					}
				}
			}
			return false;
		}

		public override string[] GetRolesForUser(string username) {
			if(username == null) {
				throw new ArgumentNullException("username");
			}
			using(var userEntry = directoryEntryAdapter.GetUserEntry(username)) {
				if(!DirectoryEntryAdapter.IsBound(userEntry)) {
					return new string[0];
				}
				var builder = new List<string>();
				if(userEntry.Properties.Contains(configuration.Users.MembershipAttribute)) {
					foreach(var groupObject in userEntry.Properties[configuration.Users.MembershipAttribute]) {
						var group = GetPropertyValue(groupObject);
						var name = ldapNameHandler.GetGroupName(group);
						builder.Add(name);
					}
				} else {
					var searcher = userSearcher.Create(userEntry, SearchScope.Base);
					searcher.Filter = filterBuilder.CreateUserFilter();
					var result = searcher.FindOne();
					if(result.Properties.Contains(configuration.Users.MembershipAttribute)) {
						foreach(var groupObject in result.Properties[configuration.Users.MembershipAttribute]) {
							var group = GetPropertyValue(groupObject);
							var name = ldapNameHandler.GetGroupName(group);
							builder.Add(name);
						}
					}
				}
				if(builder.Count > 0) {
					builder.Sort();
					return builder.ToArray();
				}
			}
			return new string[0];
		}

		public override void CreateRole(string roleName) {
			throw new NotSupportedException();
		}

		public override bool DeleteRole(string roleName, bool throwOnPopulatedRole) {
			throw new NotSupportedException();
		}

		public override bool RoleExists(string roleName) {
			if(roleName == null) {
				throw new ArgumentNullException("roleName");
			}
			using(var entry = directoryEntryAdapter.GetGroupEntry(roleName)) {
				if(DirectoryEntryAdapter.IsBound(entry) && entry.Properties.Contains(configuration.Groups.RdnAttribute)) {
					return true;
				}
			}
			return false;
		}

		public override void AddUsersToRoles(string[] usernames, string[] roleNames) {
			throw new NotSupportedException();
		}

		public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames) {
			throw new NotSupportedException();
		}

		public override string[] GetUsersInRole(string roleName) {
			if(roleName == null) {
				throw new ArgumentNullException("roleName");
			}
			var builder = new List<string>();
			using(var entry = directoryEntryAdapter.GetGroupEntry(roleName)) {
				if(!DirectoryEntryAdapter.IsBound(entry)) {
					return new string[0];
				}
				var searcher = groupSearcher.CreateSearcher(entry, SearchScope.Base);
				searcher.Filter = filterBuilder.CreateGroupFilter();

				if(configuration.Server.RangeRetrievalSupport) {
					// Inspired by http://www.netid.washington.edu/documentation/enumeratingLargeGroups.aspx
					// Shall be cleaned up later
					uint rangeLow = 0;
					var rangeHigh = rangeLow;
					var isLastQuery = false;
					var endOfRange = false;
					do {
						var userMembershipRangeFilter = filterBuilder.CreateGroupMembershipRangeFilter(rangeLow, isLastQuery ? uint.MaxValue : rangeHigh);
						searcher.PropertiesToLoad.Clear();
						searcher.PropertiesToLoad.Add(userMembershipRangeFilter);
						var result = searcher.FindOne();
						if(result != null && result.Properties.Contains(userMembershipRangeFilter)) {
							foreach(var userObject in result.Properties[userMembershipRangeFilter]) {
								var user = GetPropertyValue(userObject);
								var name = ldapNameHandler.GetUserName(user);
								builder.Add(name);
							}
							if(isLastQuery) {
								endOfRange = true;
							}
						} else {
							isLastQuery = true;
						}
						if(!isLastQuery) {
							rangeLow = rangeHigh + 1;
							rangeHigh = rangeLow;
						}
					}
					while(!endOfRange);
				} else {
					searcher.PropertiesToLoad.Clear();
					searcher.PropertiesToLoad.Add(configuration.Groups.MembershipAttribute);
					var result = searcher.FindOne();
					if((result != null) && result.Properties.Contains(configuration.Groups.MembershipAttribute)) {
						foreach(string user in result.Properties[configuration.Groups.MembershipAttribute]) {
							var name = ldapNameHandler.GetUserName(user);
							builder.Add(name);
						}
					}
				}
			}
			if(builder.Count > 0) {
				builder.Sort();
				return builder.ToArray();
			}
			return new string[0];
		}

		public override string[] GetAllRoles() {
			using(var entry = directoryEntryAdapter.GetGroupsEntry()) {
				if(!DirectoryEntryAdapter.IsBound(entry)) {
					throw new ProviderException("Could not load role list.");
				}
				var searcher = groupSearcher.Create(entry);
				searcher.Filter = filterBuilder.CreateGroupFilter();
				var results = searcher.FindAll();
				var builder = new List<string>();
				if(results.Any()) {
					foreach(var result in results) {
						var directoryEntry = result.GetDirectoryEntry();
						var name = ldapNameHandler.GetGroupNameFromEntry(directoryEntry);
						builder.Add(name);
					}
				}
				if(builder.Count > 0) {
					builder.Sort();
					return builder.ToArray();
				}
			}
			return new string[0];
		}

		public override string[] FindUsersInRole(string roleName, string usernameToMatch) {
			throw new NotSupportedException();
		}

		private static string GetPropertyValue(object valueObject) {
			// Properties are in some systems loaded as byte arrays instead of strings
			// In thouse cases we convert them to strings
			// TODO: I do not realy know the reason of this behaviour, has to be investigate why and if I do something wrong.
			var b = valueObject as byte[];
			if(b != null) {
				return System.Text.Encoding.UTF8.GetString(b);
			}
			return valueObject as string;
		}
	}
}
