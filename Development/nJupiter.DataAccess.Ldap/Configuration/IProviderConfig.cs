namespace  nJupiter.DataAccess.Ldap.Configuration {
	internal interface IProviderConfig {
		string Name { get; }
		string ApplicationName { get; }
		ILdapConfig LdapConfig { get; }
	}
}