using System;

using nJupiter.DataAccess.Ldap.Configuration;

namespace nJupiter.DataAccess.Ldap {
	internal class LdapNameHandlerFactory {
		public static ILdapNameHandler GetInstance(ILdapConfig config) {
			if(config == null) {
				throw new ArgumentNullException("config");
			}

			return new LdapNameHandler(config);
		}		
	}
}