using System.Collections.Generic;

using nJupiter.Configuration;

namespace nJupiter.DataAccess.Ldap.Configuration {
	internal class LdapConfigFactory {

		private readonly Dictionary<string, ILdapConfig> Configurations = new Dictionary<string, ILdapConfig>();
		private readonly object Padlock = new object();

		private readonly IConfigRepository configRepository;
		private readonly IServerConfigFactory serverConfigFactory;
		private readonly IUsersConfigFactory usersConfigFactory;
		private readonly IGroupConfigFactory groupsConfigFactory;

		public LdapConfigFactory(IConfigRepository configRepository, IServerConfigFactory serverConfigFactory, IUsersConfigFactory usersConfigFactory, IGroupConfigFactory groupsConfigFactory) {
			this.configRepository = configRepository;
			this.serverConfigFactory = serverConfigFactory;
			this.usersConfigFactory = usersConfigFactory;
			this.groupsConfigFactory = groupsConfigFactory;
		}

		public ILdapConfig GetConfig() {
			return GetConfig(string.Empty);
		}

		public ILdapConfig GetConfig(string ldapServer) {
			if(!Configurations.ContainsKey(ldapServer)) {
				lock(Padlock) {
					if(!Configurations.ContainsKey(ldapServer)) {
						Configurations.Add(ldapServer, new LdapConfig(ldapServer, configRepository, serverConfigFactory, usersConfigFactory, groupsConfigFactory));
					}
				}
			}
			return Configurations[ldapServer];
		}


		public static LdapConfigFactory Instance { get { return NestedSingleton.instance; } }
		// thread safe Singleton implementation with fully lazy instantiation and with full performance
		private sealed class NestedSingleton {
			// ReSharper disable EmptyConstructor
			static NestedSingleton() {} // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
			// ReSharper restore EmptyConstructor
			internal static readonly LdapConfigFactory instance = new LdapConfigFactory(
																							ConfigRepository.Instance,
																							new ServerConfigFactory(),
																							new UsersConfigFactory(),
																							new GroupsConfigFactory()
																						);
		}

	}
}