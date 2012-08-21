using System.Collections.Generic;

using nJupiter.DataAccess.Ldap.DistinguishedNames;

namespace nJupiter.DataAccess.Ldap.Configuration {
	public interface IEntryConfig {
		string Filter { get; }
		string Base { get; }
		string Path { get; }
		string RdnAttribute { get; }
		IEnumerable<IAttributeDefinition> Attributes { get; }
		string MembershipAttribute { get; }
		NameType NameType { get; }
	}
}