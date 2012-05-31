using nJupiter.DataAccess.Ldap.Abstractions;

namespace nJupiter.DataAccess.Ldap {
	internal interface ILdapNameHandler {
		string GetUserNameFromEntry(IDirectoryEntry entry);
		string GetUserName(string entryName);
		string GetGroupNameFromEntry(IDirectoryEntry entry);
		string GetGroupName(string entryName);
		bool GroupsEqual(string name, string nameToMatch);
	}
}