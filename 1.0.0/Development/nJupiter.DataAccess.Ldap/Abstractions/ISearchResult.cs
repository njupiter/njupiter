using System.DirectoryServices;

namespace nJupiter.DataAccess.Ldap.Abstractions {
	internal interface ISearchResult {
		IDirectoryEntry GetDirectoryEntry();
		string Path { get; }
		ResultPropertyCollection Properties { get; }
	}
}