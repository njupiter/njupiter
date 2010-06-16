namespace nJupiter.DataAccess.Ldap {
	internal class AttributeDefinition {
		readonly string name;
		readonly bool excludeFromNameSearch;

		public string Name { get { return this.name; } }
		public bool ExcludeFromNameSearch { get { return this.excludeFromNameSearch; } }

		public AttributeDefinition(string name) : this(name, false) { }
		public AttributeDefinition(string name, bool excludeFromNameSearch) {
			this.name = name;
			this.excludeFromNameSearch = excludeFromNameSearch;
		}
	}
}
