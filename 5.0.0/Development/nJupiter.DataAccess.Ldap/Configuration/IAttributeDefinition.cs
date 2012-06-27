namespace nJupiter.DataAccess.Ldap.Configuration {
	public interface IAttributeDefinition {
		string Name { get; }
		bool ExcludeFromNameSearch { get; }
	}
}