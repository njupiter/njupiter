using nJupiter.DataAccess.Ldap.Abstractions;
using nJupiter.DataAccess.Ldap.NameParser;

namespace nJupiter.DataAccess.Ldap.Configuration {
	internal interface IContainer {
		IDnParser DnParser { get; }
		IDirectoryEntryFactory DirectoryEntryFactory { get; }
		ISearcher UserSearcher { get; }
		ISearcher GroupSearcher { get; }
		IFilterBuilder FilterBuilder { get; }
		ILdapNameHandler LdapNameHandler { get; }
		IDirectoryEntryAdapter DirectoryEntryAdapter { get; }
	}
}