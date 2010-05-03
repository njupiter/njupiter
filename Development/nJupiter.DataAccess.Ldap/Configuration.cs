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
using System.DirectoryServices;

using nJupiter.Configuration;

namespace nJupiter.DataAccess.Ldap {

	internal class Configuration {

		private readonly string ldapServer;
		private ServerConfig serverConfig;
		private UsersConfig usersConfig;
		private GroupsConfig groupsConfig;

		private static readonly Dictionary<string, Configuration> Configurations = new Dictionary<string,Configuration>();
		private static readonly object Padlock = new object();

		public ServerConfig Server {
			get {
				return serverConfig;
			}
		}

		public UsersConfig Users {
			get {
				return usersConfig;
			}
		}

		public GroupsConfig Groups {
			get {
				return groupsConfig;
			}
		}

		public static Configuration GetConfig() {
			return GetConfig(string.Empty);
		}

		public static Configuration GetConfig(string ldapServer) {
			if(!Configurations.ContainsKey(ldapServer)) {
				lock(Padlock) {
					if(!Configurations.ContainsKey(ldapServer)) {
						Configurations.Add(ldapServer, new Configuration(ldapServer));
					}
				}
			}
			return Configurations[ldapServer];
		}

		private Configuration(string ldapServer) {
			this.ldapServer = ldapServer;
			this.Configure(null, EventArgs.Empty);
		}

		private void Configure(object sender, EventArgs e) {
			lock(Padlock) {
				ServerConfig server = new ServerConfig();
				UsersConfig users = new UsersConfig();
				GroupsConfig groups = new GroupsConfig();

				Config config = ConfigHandler.GetConfig();
				Config configSection = this.GetConfigSection(config);
				if(configSection != null) {

					server.Url = new Uri(configSection.GetValue("url"));

					if(configSection.ContainsKey("username")) {
						server.Username = configSection.GetValue("username");
					}

					if(configSection.ContainsKey("password")) {
						server.Password = configSection.GetValue("password");
					}

					if(configSection.ContainsKey("allowWildcardSearch")) {
						server.AllowWildcardSearch = configSection.GetBoolValue("allowWildcardSearch");
					}

					if(configSection.ContainsKey("ldapType")) {
						string ldapType = configSection.GetValue("ldapType");
						server.Type = (LdapType)Enum.Parse(typeof(LdapType), ldapType, true);
					} else {
						server.Type = LdapType.Generic;
					}

					if(configSection.ContainsKey("timeLimit")) {
						int timeLimit = configSection.GetIntValue("timeLimit");
						server.TimeLimit = TimeSpan.FromSeconds(timeLimit);
					} else {
						server.TimeLimit = TimeSpan.FromSeconds(30);
					}

					if(configSection.ContainsKey("pageSize")) {
						int pageSize = configSection.GetIntValue("pageSize");
						server.PageSize = pageSize;
					} else {
						server.PageSize = 1000;
					}

					if(configSection.ContainsKey("authenticationTypes")) {
						string[] authenticationTypes = configSection.GetValueArray("authenticationTypes", "authenticationType");
						foreach(string authenticationType in authenticationTypes) {
							server.AuthenticationTypes |=
								(AuthenticationTypes)Enum.Parse(typeof(AuthenticationTypes), authenticationType, true);
						}
					} else {
						server.AuthenticationTypes = AuthenticationTypes.None;
					}

					if(configSection.ContainsKey("users", "objectClass")) {
						users.ObjectClass = configSection.GetValue("users", "objectClass");
					}

					if(configSection.ContainsKey("users", "base")) {
						users.Base = configSection.GetValue("users", "base");
					}

					if(configSection.ContainsKey("users", "rdnAttribute")) {
						users.RdnAttribute = configSection.GetValue("users", "rdnAttribute");
					} else {
						users.RdnAttribute = "cn";
					}

					if(configSection.ContainsKey("users", "nameAttributes")) {
						users.NameAttributes = configSection.GetValueArray("users/nameAttributes", "nameAttribute");
					} else {
						users.NameAttributes = new string[0];
					}

					if(configSection.ContainsKey("users", "membershipAttribute")) {
						users.MembershipAttribute = configSection.GetValue("users", "membershipAttribute");
					}

					if(configSection.ContainsKey("users", "emailAttribute")) {
						users.EmailAttribute = configSection.GetValue("users", "emailAttribute");
					} else {
						users.EmailAttribute = "mail";
					}

					if(configSection.ContainsKey("users", "creationDateAttribute")) {
						users.CreationDateAttribute = configSection.GetValue("users", "creationDateAttribute");
					}

					if(configSection.ContainsKey("users", "lastLoginDateAttribute")) {
						users.LastLoginDateAttribute = configSection.GetValue("users", "lastLoginDateAttribute");
					}


					if(configSection.ContainsKey("users", "lastPasswordChangedDateAttribute")) {
						users.LastPasswordChangedDateAttribute = configSection.GetValue("users", "lastPasswordChangedDateAttribute");
					}

					if(configSection.ContainsKey("users", "descriptionAttribute")) {
						users.DescriptionAttribute = configSection.GetValue("users", "descriptionAttribute");
					}

					if(configSection.ContainsKey("users", "nameType")) {
						string nameType = configSection.GetValue("users", "nameType");
						users.NameType = (NameType)Enum.Parse(typeof(NameType), nameType, true);
					} else {
						users.NameType = NameType.Dn;
					}

					if(configSection.ContainsKey("users", "authenticationHeader")) {
						users.AuthenticationHeader = configSection.GetValue("users", "authenticationHeader");
					} else {
						users.AuthenticationHeader = "sm_userdn";
					}

					if(configSection.ContainsKey("groups", "objectClass")) {
						groups.ObjectClass = configSection.GetValue("groups", "objectClass");
					}

					if(configSection.ContainsKey("groups", "base")) {
						groups.Base = configSection.GetValue("groups", "base");
					}

					if(configSection.ContainsKey("groups", "rdnAttribute")) {
						groups.RdnAttribute = configSection.GetValue("groups", "rdnAttribute");
					} else {
						groups.RdnAttribute = "cn";
					}

					if(configSection.ContainsKey("groups", "nameAttributes")) {
						groups.NameAttributes = configSection.GetValueArray("groups/nameAttributes", "nameAttribute");
					} else {
						groups.NameAttributes = new[] { "cn" };
					}

					if(configSection.ContainsKey("groups", "membershipAttribute")) {
						groups.MembershipAttribute = configSection.GetValue("groups", "membershipAttribute");
					} else {
						groups.MembershipAttribute = "member";
					}

					if(configSection.ContainsKey("groups", "nameType")) {
						string nameType = configSection.GetValue("groups", "nameType");
						groups.NameType = (NameType)Enum.Parse(typeof(NameType), nameType, true);
					} else {
						groups.NameType = NameType.Dn;
					}

					if(configSection.ContainsKey("rangeRetrievalSupport")) {
						server.RangeRetrievalSupport = configSection.GetBoolValue("rangeRetrievalSupport");
					} else {
						server.RangeRetrievalSupport = true;
					}

					switch(server.Type) {
						case LdapType.Ad:
						case LdapType.Adam:
							if(string.IsNullOrEmpty(users.ObjectClass)) {
								users.ObjectClass = "user";
							}
							if(string.IsNullOrEmpty(users.MembershipAttribute)) {
								users.MembershipAttribute = "memberOf";
							}
							if(string.IsNullOrEmpty(groups.ObjectClass)) {
								groups.ObjectClass = "group";
							}
							if(string.IsNullOrEmpty(groups.MembershipAttribute)) {
								groups.MembershipAttribute = "member";
							}
							break;
					}

					if(users.NameAttributes == null || users.NameAttributes.Length == 0) {
						users.NameAttributes = new[] { "cn" };
					}

					if(string.IsNullOrEmpty(users.MembershipAttribute)) {
						users.MembershipAttribute = "groupMembership";
					}

					if(string.IsNullOrEmpty(groups.ObjectClass)) {
						groups.ObjectClass = "groupOfNames";
					}

					Uri userUri;
					if(string.IsNullOrEmpty(users.Base)) {
						userUri = server.Url;
					} else {
						userUri = new Uri(server.Url, users.Base);
					}
					users.Path = LdapPathHandler.UriToPath(userUri);

					Uri groupUri;
					if(string.IsNullOrEmpty(groups.Base)) {
						groupUri = server.Url;
					} else {
						groupUri = new Uri(server.Url, groups.Base);
					}
					groups.Path = LdapPathHandler.UriToPath(groupUri);

					this.serverConfig = server;
					this.usersConfig = users;
					this.groupsConfig = groups;
				}
				// Auto reconfigure all values when this config object is disposed (droped from the cache)
				config.Disposed += this.Configure;
			}

		}

		private Config GetConfigSection(Config config) {
			Config configSection;
			if(string.IsNullOrEmpty(this.ldapServer)) {
				configSection = config.GetConfigSection("ldapServers/ldapServer[@default='true']");
				if(configSection == null) {
					throw new ConfigurationException("No default LDAP server specified");
				}
			} else {
				configSection = config.GetConfigSection(string.Format("ldapServers/ldapServer[@value='{0}']", this.ldapServer));
				if(configSection == null) {
					throw new ConfigurationException(string.Format("No default LDAP server with name {0} specified", this.ldapServer));
				}
			}
			return configSection;
		}

		public class ServerConfig {
			public Uri Url { get; internal set; }
			public string Username { get; internal set; }
			public string Password { get; internal set; }
			public LdapType Type { get; internal set; }
			public AuthenticationTypes AuthenticationTypes { get; internal set; }
			public TimeSpan TimeLimit { get; internal set; }
			public int PageSize { get; internal set; }
			public bool AllowWildcardSearch;
			public bool RangeRetrievalSupport { get; internal set; }

			internal ServerConfig() {
			}

		}


		public class UsersConfig { 

			public string ObjectClass { get; internal set; }
			public string Base { get; internal set; }
			public string Path { get; internal set; }
			public string RdnAttribute { get; internal set; }
			public string[] NameAttributes  { get; internal set; }
			public string MembershipAttribute { get; internal set; }
			public string EmailAttribute { get; internal set; }
			public string CreationDateAttribute { get; internal set; }
			public string LastLoginDateAttribute { get; internal set; }
			public string LastPasswordChangedDateAttribute { get; internal set; }
			public string DescriptionAttribute { get; internal set; }
			public NameType NameType { get; internal set; }
			public string AuthenticationHeader { get; internal set; }

			internal UsersConfig() {
			}

		}
		public class GroupsConfig { 

			public string ObjectClass { get; internal set; }
			public string Base { get; internal set; }
			public string Path { get; internal set; }
			public string RdnAttribute { get; internal set; }
			public string[] NameAttributes  { get; internal set; }
			public string MembershipAttribute { get; internal set; }
			public NameType NameType { get; internal set; }

			internal GroupsConfig() {
			}

		}



	}
}
