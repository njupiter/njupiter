using nJupiter.Configuration;

namespace nJupiter.DataAccess.Ldap.Configuration {
	internal interface IUsersConfigFactory {
		IUsersConfig Create(IConfig configSection);
	}
}