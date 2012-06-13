using System.DirectoryServices;

namespace nJupiter.DataAccess.Ldap.DirectoryServices.Abstractions {
	internal class DirectoryEntryFactory : IDirectoryEntryFactory {
		public IDirectoryEntry Create(string path, string username, string password, AuthenticationTypes authenticationTypes) {
			var entry = new DirectoryEntry(path, username, password, authenticationTypes);
			return new DirectoryEntryWrapper(entry);
		}
	}
}
