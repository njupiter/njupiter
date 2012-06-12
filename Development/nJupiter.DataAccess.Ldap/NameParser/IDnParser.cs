namespace nJupiter.DataAccess.Ldap.NameParser {
	internal interface IDnParser {
		string GetCn(string name);
		string GetRdn(string name);
		string GetDn(string name);
		string GetDn(string name, string attribute, string basePath);
		Dn GetDnObject(string name);
	}
}