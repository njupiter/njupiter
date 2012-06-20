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

using System.Collections.Generic;

using nJupiter.Configuration;

namespace nJupiter.DataAccess.Ldap.Configuration {
	internal class LdapConfigFactory : ILdapConfigFactory {

		private readonly Dictionary<string, ILdapConfig> configurations = new Dictionary<string, ILdapConfig>();
		private readonly object padlock = new object();

		private readonly IConfigRepository configRepository;
		private readonly IServerConfigFactory serverConfigFactory;
		private readonly IUsersConfigFactory usersConfigFactory;
		private readonly IGroupConfigFactory groupsConfigFactory;

		public LdapConfigFactory(IConfigRepository configRepository,
		                         IServerConfigFactory serverConfigFactory,
		                         IUsersConfigFactory usersConfigFactory,
		                         IGroupConfigFactory groupsConfigFactory) {
			this.configRepository = configRepository;
			this.serverConfigFactory = serverConfigFactory;
			this.usersConfigFactory = usersConfigFactory;
			this.groupsConfigFactory = groupsConfigFactory;
		}

		public ILdapConfig GetConfig() {
			return GetConfig(string.Empty);
		}

		public ILdapConfig GetConfig(string ldapServer) {
			if(!configurations.ContainsKey(ldapServer)) {
				lock(padlock) {
					if(!configurations.ContainsKey(ldapServer)) {
						configurations.Add(ldapServer, CreateLdapConfig(ldapServer));
					}
				}
			}
			return configurations[ldapServer];
		}

		protected virtual ILdapConfig CreateLdapConfig(string ldapServer) {
			return new LdapConfig(ldapServer,
			                      configRepository,
			                      serverConfigFactory,
			                      usersConfigFactory,
			                      groupsConfigFactory);
		}

		public static ILdapConfigFactory Instance { get { return NestedSingleton.instance; } }

		private static class NestedSingleton {
			static NestedSingleton() {}

			internal static readonly ILdapConfigFactory instance = new LdapConfigFactory(
				ConfigRepository.Instance,
				new ServerConfigFactory(),
				new UsersConfigFactory(),
				new GroupsConfigFactory()
				);
		}
	}
}