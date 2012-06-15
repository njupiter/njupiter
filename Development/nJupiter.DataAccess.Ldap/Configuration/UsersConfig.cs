using System.Collections.Generic;

using nJupiter.DataAccess.Ldap.NameParser;

namespace nJupiter.DataAccess.Ldap.Configuration {
	internal class UsersConfig : IUsersConfig {
		public string Filter { get; internal set; }
		public string Base { get; internal set; }
		public string Path { get; internal set; }
		public string RdnAttribute { get; internal set; }
		public List<IAttributeDefinition> Attributes { get; internal set; }
		public string MembershipAttribute { get; internal set; }
		public string EmailAttribute { get; internal set; }
		public string CreationDateAttribute { get; internal set; }
		public string LastLoginDateAttribute { get; internal set; }
		public string LastPasswordChangedDateAttribute { get; internal set; }
		public string DescriptionAttribute { get; internal set; }
		public NameType NameType { get; internal set; }
	}
}