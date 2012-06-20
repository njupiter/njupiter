using System.Collections.Generic;

namespace nJupiter.DataAccess.Ldap.DirectoryServices {
	internal interface IUserEntryAdapter {
		IEntry GetUserEntry(string username);
		IEntry GetUserEntryAndLoadProperties(string username);
		string GetUserName(string entryName);
		IEntry GetUserEntry(string username, string password);
		IEntry GetUserEntryByEmail(string email);
		IEnumerable<string> GetUsersFromEntry(IEntry entry, string propertyName);
		IEntryCollection GetAllUserEntries(int pageIndex, int pageSize, out int totalRecords);
		IEntryCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords);
		IEntryCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords);
	}


	
}