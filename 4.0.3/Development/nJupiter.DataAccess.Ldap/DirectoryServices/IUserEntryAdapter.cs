using System.Collections.Generic;

using nJupiter.DataAccess.Ldap.DirectoryServices.Abstractions;

namespace nJupiter.DataAccess.Ldap.DirectoryServices {
	internal interface IUserEntryAdapter {
		IEntry GetUserEntry(string username);
		IEntry GetUserEntry(string username, bool loadProperties);
		string GetUserName(IEntry entry);
		string GetUserName(string entryName);
		IEntry GetUserEntry(string username, string password);
		IEntry GetUserEntryByEmail(string email);
		IEnumerable<string> GetUsersFromEntry(IEntry entry, string propertyName);
		IEnumerable<IEntry> GetAllUserEntries(int pageIndex, int pageSize, out int totalRecords);
		IEnumerable<IEntry> FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords);
		IEnumerable<IEntry> FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords);
	}
}