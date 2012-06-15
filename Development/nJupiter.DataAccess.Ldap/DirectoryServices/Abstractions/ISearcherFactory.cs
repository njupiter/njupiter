using System.Collections.Generic;
using System.DirectoryServices;

namespace nJupiter.DataAccess.Ldap.DirectoryServices.Abstractions {
	internal interface ISearcherFactory {
		IDirectorySearcher CreateSearcher(IEntry entry, SearchScope searchScope);
		IDirectorySearcher CreateSearcher(IEntry entry, SearchScope searchScope, string rdnAttribute, IEnumerable<IAttributeDefinition> otherAttributes);
	}
}