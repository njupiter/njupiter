using System;

using nJupiter.DataAccess.Ldap.Abstractions;

namespace nJupiter.DataAccess.Ldap {
	internal interface IDirectoryEntryAdapter {
		IDirectoryEntry GetUserEntry(string username);
		IDirectoryEntry GetUsersEntry();
		IDirectoryEntry GetGroupEntry(string groupname);
		IDirectoryEntry GetGroupsEntry();
		IDirectoryEntry GetEntry(Uri uri, string username, string password);
	}
}