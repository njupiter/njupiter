using System.DirectoryServices;

using nJupiter.DataAccess.Ldap.Abstractions;

namespace nJupiter.DataAccess.Ldap {
	internal interface ISearcher {
		IDirectorySearcher Create(IDirectoryEntry entry);
		IDirectorySearcher Create(IDirectoryEntry entry, SearchScope searchScope);
		IDirectorySearcher CreateSearcher(IDirectoryEntry entry, SearchScope searchScope);
	}
}