namespace nJupiter.DataAccess.Ldap.NameParser {
	internal interface INameParser {
		string GetCn(string name);
		string GetRdn(string name);
		string GetDn(string name);
		string GetDn(string name, string attribute, string basePath);
		Dn GetDnObject(string name);
		string GetName(NameType nameType, string entryName);
		bool NamesEqual(string name, string nameToMatch, string attribute, string basePath);
	}
}