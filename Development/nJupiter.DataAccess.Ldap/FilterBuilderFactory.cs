using System;

using nJupiter.DataAccess.Ldap.Configuration;

namespace nJupiter.DataAccess.Ldap {
	internal class FilterBuilderFactory {
		public static IFilterBuilder GetInstance(ILdapConfig config) {
			if(config == null) {
				throw new ArgumentNullException("config");
			}
			return new FilterBuilder(config);
		}

	}
}