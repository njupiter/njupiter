using nJupiter.Configuration;

namespace nJupiter.DataAccess.Ldap.Configuration {
	internal interface IServerConfigFactory {
		IServerConfig Create(IConfig configSection);
	}
}