using System;

using nJupiter.DataAccess.Ldap.DirectoryServices.Abstraction;

namespace nJupiter.DataAccess.Ldap.DirectoryServices {
	internal interface IDirectoryEntryAdapter {
		IDirectoryEntry GetEntry(string path);
		IDirectoryEntry GetEntry(Uri uri, string username, string password);
		IDirectoryEntry GetEntry(string attribute, string attributeValue, string path, string defaultFilter, Func<IEntry, IDirectorySearcher> searcherFactoryMethod);
	}
}