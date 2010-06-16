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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.DirectoryServices;
using System.Web.Security;

namespace nJupiter.DataAccess.Ldap {

	public class LdapRoleProvider : RoleProvider {

		private string providerName;
		private string appName;
		private string ldapServer;
		private Configuration configuration;
		private Searcher userSeracher;
		private Searcher groupSeracher;
		private FilterBuilder filterBuilder;
		private DirectoryEntryAdapter directoryEntryAdapter;
		private LdapNameHandler ldapNameHandler;

		public override string ApplicationName { get { return this.appName; } set { this.appName = value; } }
		public override string Name {
			get {
				return string.IsNullOrEmpty(this.providerName) ? this.GetType().Name : this.providerName;
			}
		}

		public override void Initialize(string name, NameValueCollection config) {
			if(config == null) {
				throw new ArgumentNullException("config");
			}
			this.appName = LdapRoleProvider.GetStringConfigValue(config, "applicationName", typeof(LdapRoleProvider).GetType().Name);
			this.providerName = !string.IsNullOrEmpty(name) ? name : this.appName;

			this.ldapServer = GetStringConfigValue(config, "ldapServer", string.Empty);

			this.configuration = Configuration.GetConfig(this.ldapServer);
			this.userSeracher = SearcherFactory.GetSearcher("user", configuration);
			this.groupSeracher = SearcherFactory.GetSearcher("group", configuration);
			this.filterBuilder = FilterBuilder.GetInstance(this.configuration);
			this.directoryEntryAdapter = DirectoryEntryAdapter.GetInstance(this.configuration, this.userSeracher, this.groupSeracher, this.filterBuilder);
			this.ldapNameHandler = LdapNameHandler.GetInstance(configuration);

			base.Initialize(this.providerName, config);
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
			using(DirectoryEntry entry = directoryEntryAdapter.GetUserEntry(username)) {
				if(!DirectoryEntryAdapter.IsBound(entry)) {
					throw new ProviderException(string.Format("Could not locate user {0}", username));
				}
				if(entry.Properties.Contains(configuration.Users.MembershipAttribute)) {
					foreach(object groupObject in entry.Properties[configuration.Users.MembershipAttribute]) {
						string group = GetPropertyValue(groupObject);
						if(ldapNameHandler.GroupsEqual(group, roleName)) {
							return true;
						}
					}
				} else {
					DirectorySearcher searcher = this.userSeracher.Create(entry, SearchScope.Base);
					searcher.Filter = filterBuilder.CreateUserFilter();
					SearchResult result = searcher.FindOne();
					if((result != null) && result.Properties.Contains(configuration.Users.MembershipAttribute)) {
						foreach(object groupObject in result.Properties[configuration.Users.MembershipAttribute]) {
							string group = GetPropertyValue(groupObject);
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
			using(DirectoryEntry userEntry = directoryEntryAdapter.GetUserEntry(username)) {
				if(!DirectoryEntryAdapter.IsBound(userEntry)) {
					throw new ProviderException(string.Format("Could not locate user {0}", username));
				}
				List<string> builder = new List<string>();
				if(userEntry.Properties.Contains(configuration.Users.MembershipAttribute)) {
					foreach(object groupObject in userEntry.Properties[configuration.Users.MembershipAttribute]) {
						string group = GetPropertyValue(groupObject);
						string name = ldapNameHandler.GetGroupName(group);
						builder.Add(name);
					}
				} else {
					DirectorySearcher searcher = this.userSeracher.Create(userEntry, SearchScope.Base);
					searcher.Filter = filterBuilder.CreateUserFilter();
					SearchResult result = searcher.FindOne();
					if(result.Properties.Contains(configuration.Users.MembershipAttribute)) {
						foreach(object groupObject in result.Properties[configuration.Users.MembershipAttribute]) {
							string group = GetPropertyValue(groupObject);
							string name = ldapNameHandler.GetGroupName(group);
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
			using(DirectoryEntry entry = directoryEntryAdapter.GetGroupEntry(roleName)) {
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
			List<string> builder = new List<string>();
			using(DirectoryEntry entry = directoryEntryAdapter.GetGroupEntry(roleName)) {
				if(!DirectoryEntryAdapter.IsBound(entry)) {
					throw new ProviderException(string.Format("Could not locate role {0}", roleName));
				}
				DirectorySearcher searcher = groupSeracher.CreateSearcher(entry, SearchScope.Base);
				searcher.Filter = filterBuilder.CreateGroupFilter();

				if(configuration.Server.RangeRetrievalSupport) {
					// Inspired by http://www.netid.washington.edu/documentation/enumeratingLargeGroups.aspx
					// Shall be cleaned up later
					uint rangeLow = 0;
					uint rangeHigh = rangeLow;
					bool isLastQuery = false;
					bool endOfRange = false;
					do {
						string userMembershipRangeFilter = filterBuilder.CreateGroupMembershipRangeFilter(rangeLow, isLastQuery ? uint.MaxValue : rangeHigh);
						searcher.PropertiesToLoad.Clear();
						searcher.PropertiesToLoad.Add(userMembershipRangeFilter);
						SearchResult result = searcher.FindOne();
						if(result != null && result.Properties.Contains(userMembershipRangeFilter)) {
							foreach(object userObject in result.Properties[userMembershipRangeFilter]) {
								string user = GetPropertyValue(userObject);
								string name = ldapNameHandler.GetUserName(user);
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
					SearchResult result = searcher.FindOne();
					if((result != null) && result.Properties.Contains(configuration.Groups.MembershipAttribute)) {
						foreach(string user in result.Properties[configuration.Groups.MembershipAttribute]) {
							string name = ldapNameHandler.GetUserName(user);
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
			using(DirectoryEntry entry = directoryEntryAdapter.GetGroupsEntry()) {
				if(!DirectoryEntryAdapter.IsBound(entry)) {
					throw new ProviderException("Could not load role list.");
				}
				DirectorySearcher searcher = groupSeracher.Create(entry);
				searcher.Filter = filterBuilder.CreateGroupFilter();
				SearchResultCollection results = searcher.FindAll();
				List<string> builder = new List<string>();
				if(results.Count > 0) {
					foreach(SearchResult result in results) {
						DirectoryEntry directoryEntry = result.GetDirectoryEntry();
						string name = ldapNameHandler.GetGroupNameFromEntry(directoryEntry);
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
			byte[] b = valueObject as byte[];
			if(b != null) {
				return System.Text.Encoding.UTF8.GetString(b);
			}
			return valueObject as string;
		}
	}
}
