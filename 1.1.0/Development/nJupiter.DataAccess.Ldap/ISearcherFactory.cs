using System.Collections.Generic;
using System.DirectoryServices;

using nJupiter.DataAccess.Ldap.DirectoryServices.Abstractions;

namespace nJupiter.DataAccess.Ldap {
	internal interface ISearcherFactory {
		IDirectorySearcher CreateSearcher(IEntry entry, SearchScope searchScope);
		IDirectorySearcher CreateSearcher(IEntry entry, SearchScope searchScope, string rdnAttribute, IEnumerable<AttributeDefinition> otherAttributes);
	}
}