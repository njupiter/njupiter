using System.Collections.Generic;

using nJupiter.DataAccess.Ldap.DirectoryServices.Abstractions;

namespace nJupiter.DataAccess.Ldap.DirectoryServices {
	internal interface IGroupEntryAdapter {
		IEntry GetGroupEntry(string groupname, bool loadProperties);
		IEntry GetGroupEntry(string groupname);
		IDirectoryEntry GetGroupsEntry();
		IEnumerable<string> GetGroupMembersByRangedRetrival(string name);
		IEnumerable<IEntry> GetAllRoleEntries();
		string GetGroupName(IEntry entry);
		string GetGroupName(string entryName);
		bool GroupsEqual(string name, string nameToMatch);
	}
}