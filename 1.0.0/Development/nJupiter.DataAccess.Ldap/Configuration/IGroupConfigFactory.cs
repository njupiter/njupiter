using nJupiter.Configuration;

namespace nJupiter.DataAccess.Ldap.Configuration {
	internal interface IGroupConfigFactory {
		IGroupsConfig Create(IConfig configSection);
	}
}