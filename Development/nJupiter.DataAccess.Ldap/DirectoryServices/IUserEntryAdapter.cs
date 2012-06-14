using System;
using System.Collections.Generic;
using System.DirectoryServices;

using nJupiter.DataAccess.Ldap.DirectoryServices.Abstractions;

namespace nJupiter.DataAccess.Ldap.DirectoryServices {
	internal interface IUserEntryAdapter {
		IEntry GetUserEntry(string username);
		IEntry GetUserEntry(string username, bool loadProperties);
		string GetUserName(IEntry entry);
		string GetUserName(string entryName);
		IEntry GetUserEntry(string username, string password);
		IEnumerable<string> GetUsersFromEntry(IEntry entry, string propertyName);

		[Obsolete("Make this private later")]
		IDirectoryEntry GetUsersEntry();

		[Obsolete("Make this private later")]
		IDirectorySearcher CreateSearcher(IEntry entry);

		[Obsolete("Make this private later")]
		IEntry GetSearchedUserEntry(IEntry entry);

		[Obsolete("Make this private later")]
		IDirectorySearcher CreateSearcher(IEntry entry, SearchScope searchScope);
	}
}