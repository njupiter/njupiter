namespace nJupiter.DataAccess.Ldap.Configuration {
	internal interface ILdapConfig {
		IServerConfig Server { get; }
		IUsersConfig Users { get; }
		IGroupsConfig Groups { get; }
		IContainer Container { get; }
	}
}