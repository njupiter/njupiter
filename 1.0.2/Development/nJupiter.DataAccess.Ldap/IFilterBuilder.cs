namespace nJupiter.DataAccess.Ldap {
	internal interface IFilterBuilder {
		string CreateUserNameFilter(string usernameToMatch);
		string CreateUserEmailFilter(string emailToMatch);
		string CreateGroupMembershipRangeFilter(uint startIndex, uint endIndex);
		string CreateUserFilter();
		string CreateGroupFilter();
		string AttachFilter(string attributeToMatch, string valueToMatch, string defaultFilter);
		string AttachRdnFilter(string valueToMatch, string defaultFilter);
	}
}