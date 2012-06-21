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

using nJupiter.Configuration;

namespace nJupiter.DataAccess.Ldap.Configuration {
	internal class LdapConfig : ILdapConfig {
		private readonly string ldapServer;
		private IServerConfig serverConfig;
		private IUsersConfig usersConfig;
		private IGroupsConfig groupsConfig;
		private IContainer container;
		private readonly IConfigRepository configRepository;
		private readonly IServerConfigFactory serverConfigFactory;
		private readonly IUsersConfigFactory usersConfigFactory;
		private readonly IGroupConfigFactory groupsConfigFactory;

		internal LdapConfig(string ldapServer,
		                    IConfigRepository configRepository,
		                    IServerConfigFactory serverConfigFactory,
		                    IUsersConfigFactory usersConfigFactory,
		                    IGroupConfigFactory groupsConfigFactory) {
			this.configRepository = configRepository;
			this.ldapServer = ldapServer;
			this.serverConfigFactory = serverConfigFactory;
			this.usersConfigFactory = usersConfigFactory;
			this.groupsConfigFactory = groupsConfigFactory;
			Configure(null, EventArgs.Empty);
		}

		public string Name { get { return ldapServer; } }

		public IServerConfig Server { get { return serverConfig; } }

		public IUsersConfig Users { get { return usersConfig; } }

		public IGroupsConfig Groups { get { return groupsConfig; } }

		public IContainer Container { get { return container; } }

		private void Configure(object sender, EventArgs e) {
			var config = sender as IConfig;
			if(config != null) {
				config.Discarded -= Configure;
			}

			config = configRepository.GetConfig();
			var configSection = GetConfigSection(config);

			serverConfig = serverConfigFactory.Create(configSection);
			usersConfig = usersConfigFactory.Create(configSection);
			groupsConfig = groupsConfigFactory.Create(configSection);

			container = new Container(this);

			// Auto reconfigure all values when this config object is disposed (droped from the cache)
			config.Discarded += Configure;
		}

		private IConfig GetConfigSection(IConfig config) {
			IConfig configSection;
			if(string.IsNullOrEmpty(ldapServer)) {
				configSection = config.GetConfigSection("ldapServers/ldapServer[@default='true']");
				if(configSection == null) {
					throw new ConfigurationException("No default LDAP server specified");
				}
			} else {
				configSection = config.GetConfigSection(string.Format("ldapServers/ldapServer[@value='{0}']", ldapServer));
				if(configSection == null) {
					throw new ConfigurationException(string.Format("No default LDAP server with name {0} specified", ldapServer));
				}
			}
			return configSection;
		}
	}
}