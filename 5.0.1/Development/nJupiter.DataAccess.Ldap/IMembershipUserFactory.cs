using System.Collections.Generic;
using System.Web.Security;

using nJupiter.DataAccess.Ldap.DirectoryServices;

namespace nJupiter.DataAccess.Ldap {
	public interface IMembershipUserFactory {
		string Name { get; }
		MembershipUser Create(IEntry entry);
		MembershipUserCollection CreateCollection(IEnumerable<IEntry> entries);
	}
}