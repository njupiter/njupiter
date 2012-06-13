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
using nJupiter.DataAccess.Ldap.DirectoryServices;

namespace nJupiter.DataAccess.Ldap {

	public class LdapRoleProvider : RoleProvider {
		
		private string providerName;
		private string appName;
		private RoleAdapter roleAdapter;

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
			
			var ldapServer = GetStringConfigValue(config, "ldapServer", string.Empty);

			var configuration = LdapConfigFactory.Instance.GetConfig(ldapServer);
			roleAdapter = new RoleAdapter(configuration);

			base.Initialize(providerName, config);
		}

		private static string GetStringConfigValue(NameValueCollection config, string configKey, string defaultValue) {
			if((config != null) && (config[configKey] != null)) {
				return config[configKey];
			}
			return defaultValue;
		}

		public override bool IsUserInRole(string username, string roleName) {
			return roleAdapter.IsUserInRole(username, roleName);
		}

		public override string[] GetRolesForUser(string username) {
			return roleAdapter.GetRolesForUser(username);
		}

		public override void CreateRole(string roleName) {
			throw new NotSupportedException();
		}

		public override bool DeleteRole(string roleName, bool throwOnPopulatedRole) {
			throw new NotSupportedException();
		}

		public override bool RoleExists(string roleName) {
			return roleAdapter.RoleExists(roleName);
		}

		public override void AddUsersToRoles(string[] usernames, string[] roleNames) {
			throw new NotSupportedException();
		}

		public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames) {
			throw new NotSupportedException();
		}

		public override string[] GetUsersInRole(string roleName) {
			return roleAdapter.GetUsersInRole(roleName);
		}

		public override string[] GetAllRoles() {
			return roleAdapter.GetAllRoles();
		}

		public override string[] FindUsersInRole(string roleName, string usernameToMatch) {
			throw new NotSupportedException();
		}
	}
}
