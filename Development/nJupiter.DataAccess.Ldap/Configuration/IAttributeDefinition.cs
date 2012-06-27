namespace nJupiter.DataAccess.Ldap.Configuration {
	internal interface IAttributeDefinition {
		string Name { get; }
		bool ExcludeFromNameSearch { get; }
	}
}