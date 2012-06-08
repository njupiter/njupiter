using System.Collections.Generic;

namespace nJupiter.DataAccess.Ldap.Configuration {
	internal interface IGroupsConfig {
		string Filter { get; }
		string Base { get; }
		string Path { get; }
		string RdnAttribute { get; }
		List<AttributeDefinition> Attributes { get; }
		string MembershipAttribute { get; }
		NameType NameType { get; }
	}
}