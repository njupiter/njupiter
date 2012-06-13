using System.DirectoryServices;

namespace nJupiter.DataAccess.Ldap.DirectoryServices.Abstractions {
	internal interface IDirectoryEntryFactory {
		IDirectoryEntry Create(string path, string username, string password, AuthenticationTypes authenticationTypes);
	}
}