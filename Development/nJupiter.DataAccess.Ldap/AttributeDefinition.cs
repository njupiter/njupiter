namespace nJupiter.DataAccess.Ldap {
	internal class AttributeDefinition : IAttributeDefinition {
		readonly string name;
		readonly bool excludeFromNameSearch;

		public string Name { get { return name; } }
		public bool ExcludeFromNameSearch { get { return excludeFromNameSearch; } }

		public AttributeDefinition(string name) : this(name, false) { }
		public AttributeDefinition(string name, bool excludeFromNameSearch) {
			this.name = name;
			this.excludeFromNameSearch = excludeFromNameSearch;
		}
	}
}
