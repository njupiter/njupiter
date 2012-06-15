namespace nJupiter.DataAccess.Ldap {
	internal interface IAttributeDefinition {
		string Name { get; }
		bool ExcludeFromNameSearch { get; }
	}
}