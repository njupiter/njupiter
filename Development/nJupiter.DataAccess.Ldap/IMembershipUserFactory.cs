using System.Web.Security;

using nJupiter.DataAccess.Ldap.Abstractions;

namespace nJupiter.DataAccess.Ldap {
	internal interface IMembershipUserFactory {
		MembershipUser CreateUserFromSearcher(IDirectorySearcher searcher);
		MembershipUserCollection CreateUsersFromSearcher(IDirectorySearcher searcher);
	}
}