using System.Collections.Generic;

namespace nJupiter.DataAccess.Ldap.Configuration {
	internal interface IUsersConfig {
		string Filter { get; }
		string Base { get; }
		string Path { get; }
		string RdnAttribute { get; }
		List<AttributeDefinition> Attributes { get; }
		string MembershipAttribute { get; }
		string EmailAttribute { get; }
		string CreationDateAttribute { get; }
		string LastLoginDateAttribute { get; }
		string LastPasswordChangedDateAttribute { get; }
		string DescriptionAttribute { get; }
		NameType NameType { get; }
	}
}