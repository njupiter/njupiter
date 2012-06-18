using System;
using System.Collections.Specialized;

namespace  nJupiter.DataAccess.Ldap.Configuration {
	internal static class ProviderConfigFactory {
		public static IProviderConfig Create<T>(string name, NameValueCollection config) {
			if(config == null) {
				throw new ArgumentNullException("config");
			}
			var appName = GetStringConfigValue(config, "applicationName", typeof(T).Name);
			var providerName = !string.IsNullOrEmpty(name) ? name : appName;
			var ldapServer = GetStringConfigValue(config, "ldapServer", string.Empty);
			var configuration = LdapConfigFactory.Instance.GetConfig(ldapServer);
			return new ProviderConfig(configuration, providerName, appName);
		}

		private static string GetStringConfigValue(NameValueCollection config, string configKey, string defaultValue) {
			if((config != null) && (config[configKey] != null)) {
				return config[configKey];
			}
			return defaultValue;
		}
	}
}