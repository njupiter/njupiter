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
using System.Linq;
using System.Web.Security;

using nJupiter.DataAccess.Ldap.Configuration;
using nJupiter.DataAccess.Ldap.DirectoryServices;

namespace nJupiter.DataAccess.Ldap {

	public class LdapRoleProvider : RoleProvider {
		
		private readonly IProviderConfigFactory providerConfigFactory;
		private IProviderConfig providerConfig;
		private ILdapConfig ldapConfig;
		private IGroupEntryAdapter groupEntryAdapter;
		private IUserEntryAdapter userEntryAdapter;

		public override string ApplicationName { get { return providerConfig.ApplicationName; } set { } }
		public override string Name { get { return providerConfig.Name; } }
		private IProviderConfigFactory ConfigFactory { get { return providerConfigFactory ?? ProviderConfigFactory.Instance; } }

		public LdapRoleProvider() {}

		internal LdapRoleProvider(IProviderConfigFactory providerConfigFactory) {
			this.providerConfigFactory = providerConfigFactory;
		}

		public override void Initialize(string name, NameValueCollection config) {
			providerConfig = ConfigFactory.Create<LdapRoleProvider>(name, config);
			ldapConfig = providerConfig.LdapConfig;
			groupEntryAdapter = ldapConfig.Container.GroupEntryAdapter;
			userEntryAdapter = ldapConfig.Container.UserEntryAdapter;
			base.Initialize(providerConfig.Name, config);
		}

		public override bool RoleExists(string roleName) {
			using(var role = groupEntryAdapter.GetGroupEntry(roleName)) {
				if(role.ContainsProperty(ldapConfig.Groups.RdnAttribute)) {
					return true;
				}
			}
			return false;
		}

		public override bool IsUserInRole(string username, string roleName) {
			var roles = GetRolesForUser(username);
			return roles.Contains(roleName, StringComparer.InvariantCultureIgnoreCase);
		}
	
		public override string[] GetRolesForUser(string username) {
			IEnumerable<string> roles;
			if(string.IsNullOrEmpty(ldapConfig.Users.MembershipAttribute)) {
				roles = GetRolesForUserWithoutMembershipAttribute(username);
			} else {
				roles = GetRolesForUserWithMembershipAttribute(username);
			}
			return ToOrderedArray(roles);
		}

		public override string[] GetUsersInRole(string roleName) {
			IEnumerable<string> result;
			if(ldapConfig.Server.RangeRetrievalSupport) {
				result = GetUsersInRoleEntityByRangedRetrival(roleName);
			} else {
				result = GetUsersInRoleEntity(roleName);
			}
			return ToOrderedArray(result);
		}

		public override string[] GetAllRoles() {
			using(var roleEntries =  groupEntryAdapter.GetAllRoleEntries()) {
				var roles = roleEntries.Select(entry => groupEntryAdapter.GetGroupName(entry));
				return ToOrderedArray(roles);
			}
		}


		private IEnumerable<string> GetRolesForUserWithMembershipAttribute(string username) {
			using(var user = userEntryAdapter.GetUserEntry(username)) {
				return GetRoleNamesFromEntry(user);
			}			
		}

		private IEnumerable<string> GetRolesForUserWithoutMembershipAttribute(string username) {
			return GetAllRoles().Where(role => GetUsersInRole(role).Contains(username));
		}

		private IEnumerable<string> GetRoleNamesFromEntry(IEntry entry) {
			var roles = entry.GetProperties<string>(ldapConfig.Users.MembershipAttribute);
			return roles.Select(group => groupEntryAdapter.GetGroupName(group));
		}

		private IEnumerable<string> GetUsersInRoleEntity(string name) {
			using(var group = groupEntryAdapter.GetGroupEntry(name, true)) {
				return userEntryAdapter.GetUsersFromEntry(group, ldapConfig.Groups.MembershipAttribute);
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

		public override void CreateRole(string roleName) {
			throw new NotSupportedException();
		}

		public override bool DeleteRole(string roleName, bool throwOnPopulatedRole) {
			throw new NotSupportedException();
		}

		public override void AddUsersToRoles(string[] usernames, string[] roleNames) {
			throw new NotSupportedException();
		}

		public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames) {
			throw new NotSupportedException();
		}

		public override string[] FindUsersInRole(string roleName, string usernameToMatch) {
			throw new NotSupportedException();
		}
	}
}
