using System;

using nJupiter.DataAccess.Ldap.Abstractions;
using nJupiter.DataAccess.Ldap.Configuration;

namespace nJupiter.DataAccess.Ldap {
	internal class DirectoryEntryAdapterFactory {
		public static IDirectoryEntryAdapter GetInstance(ILdapConfig config, ISearcher userSearcher, ISearcher groupSearcher, IFilterBuilder filterBuilder) {
			if(config == null) {
				throw new ArgumentNullException("config");
			}
			if(userSearcher == null) {
				throw new ArgumentNullException("userSearcher");
			}
			if(groupSearcher == null) {
				throw new ArgumentNullException("groupSearcher");
			}
			if(filterBuilder == null) {
				throw new ArgumentNullException("filterBuilder");
			}
			return new DirectoryEntryAdapter(config, new DirectoryEntryFactory(), userSearcher, groupSearcher, filterBuilder);
		}
	}
}