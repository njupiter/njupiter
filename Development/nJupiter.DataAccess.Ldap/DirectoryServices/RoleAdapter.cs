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
using System.Collections.Generic;
using System.Linq;

using nJupiter.DataAccess.Ldap.Configuration;
using nJupiter.DataAccess.Ldap.DirectoryServices.Abstractions;

namespace nJupiter.DataAccess.Ldap.DirectoryServices {
	internal class RoleAdapter {
		private readonly ILdapConfig configuration;
		private readonly IGroupEntryAdapter groupEntryAdapter;
		private readonly IUserEntryAdapter userEntryAdapter;

		public RoleAdapter(	ILdapConfig configuration,
							IGroupEntryAdapter groupEntryAdapter,
							IUserEntryAdapter userEntryAdapter) {
			this.configuration = configuration;
			this.groupEntryAdapter = groupEntryAdapter;
			this.userEntryAdapter = userEntryAdapter;
		}

		public bool RoleExists(string roleName) {
			if(roleName == null) {
				throw new ArgumentNullException("roleName");
			}
			using(var role = groupEntryAdapter.GetGroupEntry(roleName)) {
				if(role.ContainsProperty(configuration.Groups.RdnAttribute)) {
					return true;
				}
			}
			return false;
		}

		public bool IsUserInRole(string username, string roleName) {
			if(roleName == null) {
				throw new ArgumentNullException("roleName");
			}
			var roles = GetRolesForUser(username);
			return roles.Contains(roleName, StringComparer.InvariantCultureIgnoreCase);
		}

		public string[] GetUsersInRole(string roleName) {
			if(roleName == null) {
				throw new ArgumentNullException("roleName");
			}
			IEnumerable<string> result;
			if(configuration.Server.RangeRetrievalSupport) {
				result = GetUsersInRoleEntityByRangedRetrival(roleName);
			} else {
				result = GetUsersInRoleEntity(roleName);
			}
			return ToOrderedArray(result);
		}

		public string[] GetAllRoles() {
			using(var roleEntries =  groupEntryAdapter.GetAllRoleEntries()) {
				var roles = roleEntries.Select(entry => groupEntryAdapter.GetGroupName(entry));
				return ToOrderedArray(roles);
			}
		}

		public string[] GetRolesForUser(string username) {
			if(username == null) {
				throw new ArgumentNullException("username");
			}
			IEnumerable<string> roles;
			if(string.IsNullOrEmpty(configuration.Users.MembershipAttribute)) {
				roles = GetRolesForUserWithoutMembershipAttribute(username);
			} else {
				roles = GetRolesForUserWithMembershipAttribute(username);
			}
			return ToOrderedArray(roles);
		}

		private IEnumerable<string> GetRolesForUserWithMembershipAttribute(string username) {
			using(var user = userEntryAdapter.GetUserEntry(username)) {
				return GetRoleNamesFromEntry(user);
			}			
		}

		private IEnumerable<string> GetRolesForUserWithoutMembershipAttribute(string username) {
			if(string.IsNullOrEmpty(configuration.Users.MembershipAttribute)) {
				var allRoles = GetAllRoles();
				foreach(var role in allRoles) {
					if(GetUsersInRole(role).Contains(username)) {
						yield return role;
					}
				}
			}		
		}

		private IEnumerable<string> GetRoleNamesFromEntry(IEntry entry) {
			var roles = entry.GetProperties<string>(configuration.Users.MembershipAttribute);
			return roles.Select(group => groupEntryAdapter.GetGroupName(group));
		}

		private IEnumerable<string> GetUsersInRoleEntity(string name) {
			using(var group = groupEntryAdapter.GetGroupEntry(name, true)) {
				return userEntryAdapter.GetUsersFromEntry(group, configuration.Groups.MembershipAttribute);
			}
		}

		private IEnumerable<string> GetUsersInRoleEntityByRangedRetrival(string name) {
			var users = groupEntryAdapter.GetGroupMembersByRangedRetrival(name);
			return users.Select(user => userEntryAdapter.GetUserName(user));
		}

		private static string[] ToOrderedArray(IEnumerable<string> strings) {
			strings = strings.OrderBy(s => s);
			return strings.ToArray();
		}
	}
}