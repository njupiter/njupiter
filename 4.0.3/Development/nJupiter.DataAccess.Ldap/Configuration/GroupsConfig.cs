using System.Collections.Generic;

using nJupiter.DataAccess.Ldap.NameParser;

namespace nJupiter.DataAccess.Ldap.Configuration {
	internal class GroupsConfig : IGroupsConfig {
		public string Filter { get; internal set; }
		public string Base { get; internal set; }
		public string Path { get; internal set; }
		public string RdnAttribute { get; internal set; }
		public List<AttributeDefinition> Attributes { get; internal set; }
		public string MembershipAttribute { get; internal set; }
		public NameType NameType { get; internal set; }
	}
}