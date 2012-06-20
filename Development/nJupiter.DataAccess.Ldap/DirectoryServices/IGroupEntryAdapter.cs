using System.Collections.Generic;

using nJupiter.DataAccess.Ldap.DirectoryServices.Abstraction;

namespace nJupiter.DataAccess.Ldap.DirectoryServices {
	internal interface IGroupEntryAdapter {
		IEntry GetGroupEntry(string groupname, bool loadProperties);
		IEntry GetGroupEntry(string groupname);
		IEnumerable<string> GetGroupMembersByRangedRetrival(string name);
		IEntryCollection GetAllRoleEntries();
		string GetGroupName(IEntry entry);
		string GetGroupName(string entryName);
	}
}