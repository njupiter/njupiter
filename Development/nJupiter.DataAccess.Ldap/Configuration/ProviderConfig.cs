namespace  nJupiter.DataAccess.Ldap.Configuration {
	internal class ProviderConfig : IProviderConfig {
		private readonly ILdapConfig ldapConfig;
		private readonly string name;
		private readonly string applicationName;

		public ProviderConfig(ILdapConfig ldapConfig, string name, string applicationName) {
			this.ldapConfig = ldapConfig;
			this.name = name;
			this.applicationName = applicationName;
		}

		public string ApplicationName { get { return applicationName; } }
		public string Name { get { return name; } }
		public ILdapConfig LdapConfig { get { return ldapConfig; } }
	}
}