using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.DirectoryServices;
using System.Linq;

using nJupiter.DataAccess.Ldap.Abstractions;
using nJupiter.DataAccess.Ldap.Configuration;

namespace nJupiter.DataAccess.Ldap {
	internal class RoleAdapter {

		private readonly IDirectoryEntryAdapter directoryEntryAdapter;
		private readonly ILdapConfig configuration;
		private readonly ILdapNameHandler ldapNameHandler;
		private readonly ISearcher userSearcher;
		private readonly ISearcher groupSearcher;
		private readonly IFilterBuilder filterBuilder;

		public RoleAdapter(ILdapConfig configuration) {
			this.configuration = configuration;
			directoryEntryAdapter = configuration.Container.DirectoryEntryAdapter;
			ldapNameHandler = configuration.Container.LdapNameHandler;
			userSearcher = configuration.Container.UserSearcher;
			groupSearcher = configuration.Container.GroupSearcher;
			filterBuilder = configuration.Container.FilterBuilder;
		}

		public bool IsUserInRole(string username, string roleName) {
			if(username == null) {
				throw new ArgumentNullException("username");
			}
			if(roleName == null) {
				throw new ArgumentNullException("roleName");
			}
			using(var entry = directoryEntryAdapter.GetUserEntry(username)) {
				if(!entry.IsBound()) {
					return false;
				}
				if(entry.Properties.Contains(configuration.Users.MembershipAttribute)) {
					foreach(var groupObject in entry.Properties[configuration.Users.MembershipAttribute]) {
						var group = GetPropertyValue(groupObject);
						if(ldapNameHandler.GroupsEqual(@group, roleName)) {
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
							if(ldapNameHandler.GroupsEqual(@group, roleName)) {
								return true;
							}
						}
					}
				}
			}
			return false;
		}

		public string[] GetRolesForUser(string username) {
			if(username == null) {
				throw new ArgumentNullException("username");
			}
			using(var userEntry = directoryEntryAdapter.GetUserEntry(username)) {
				if(!userEntry.IsBound()) {
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
		
		public bool RoleExists(string roleName) {
			if(roleName == null) {
				throw new ArgumentNullException("roleName");
			}
			using(var entry = directoryEntryAdapter.GetGroupEntry(roleName)) {
				if(entry.IsBound() && entry.Properties.Contains(configuration.Groups.RdnAttribute)) {
					return true;
				}
			}
			return false;
		}

		public string[] GetUsersInRole(string roleName) {
			if(roleName == null) {
				throw new ArgumentNullException("roleName");
			}
			var builder = new List<string>();
			using(var entry = directoryEntryAdapter.GetGroupEntry(roleName)) {
				if(!entry.IsBound()) {
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

		public string[] GetAllRoles() {
			using(var entry = directoryEntryAdapter.GetGroupsEntry()) {
				if(!entry.IsBound()) {
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