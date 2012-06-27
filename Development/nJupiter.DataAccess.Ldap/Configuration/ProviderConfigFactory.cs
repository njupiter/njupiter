#region Copyright & License
// 
// 	Copyright (c) 2005-2012 nJupiter
// 
// 	Permission is hereby granted, free of charge, to any person obtaining a copy
// 	of this software and associated documentation files (the "Software"), to deal
// 	in the Software without restriction, including without limitation the rights
// 	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// 	copies of the Software, and to permit persons to whom the Software is
// 	furnished to do so, subject to the following conditions:
// 
// 	The above copyright notice and this permission notice shall be included in
// 	all copies or substantial portions of the Software.
// 
// 	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// 	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// 	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// 	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// 	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// 	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// 	THE SOFTWARE.
// 
#endregion

using System.Collections.Specialized;

namespace nJupiter.DataAccess.Ldap.Configuration {
	public class ProviderConfigFactory : IProviderConfigFactory {
		private readonly ILdapConfigFactory ldapConfigFactory;

		internal ProviderConfigFactory(ILdapConfigFactory ldapConfigFactory) {
			this.ldapConfigFactory = ldapConfigFactory;
		}

		public virtual IProviderConfig Create<T>(string name, NameValueCollection config) {
			var appName = GetStringConfigValue(config, "applicationName", typeof(T).Name);
			var providerName = !string.IsNullOrEmpty(name) ? name : appName;
			var ldapServer = GetStringConfigValue(config, "ldapServer", string.Empty);
			var configuration = ldapConfigFactory.GetConfig(ldapServer);
			return new ProviderConfig(configuration, providerName, appName);
		}

		private string GetStringConfigValue(NameValueCollection config, string configKey, string defaultValue) {
			if((config != null) && (config[configKey] != null)) {
				return config[configKey];
			}
			return defaultValue;
		}

		public static IProviderConfigFactory Instance { get { return NestedSingleton.instance; } }

		private static class NestedSingleton {
			static NestedSingleton() {}
			internal static readonly ProviderConfigFactory instance = new ProviderConfigFactory(LdapConfigFactory.Instance);
		}
	}
}