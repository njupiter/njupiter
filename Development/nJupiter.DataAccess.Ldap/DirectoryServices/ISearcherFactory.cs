using System.Collections.Generic;
using System.DirectoryServices;

using nJupiter.DataAccess.Ldap.DirectoryServices.Abstraction;

namespace nJupiter.DataAccess.Ldap.DirectoryServices {
	internal interface ISearcherFactory {
		IDirectorySearcher CreateSearcher(IEntry entry, SearchScope searchScope);
		IDirectorySearcher CreateSearcher(IEntry entry, SearchScope searchScope, string rdnAttribute, IEnumerable<IAttributeDefinition> otherAttributes);
	}
}