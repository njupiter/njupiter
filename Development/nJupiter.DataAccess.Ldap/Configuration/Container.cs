using nJupiter.DataAccess.Ldap.Abstractions;
using nJupiter.DataAccess.Ldap.NameParser;

namespace nJupiter.DataAccess.Ldap.Configuration {
	internal class Container : IContainer {
		private readonly IDnParser dnParser;
		private readonly IDirectoryEntryFactory directoryEntryFactory;
		private readonly ISearcher userSearcher;
		private readonly ISearcher groupSearcher;
		private readonly IFilterBuilder filterBuilder;
		private readonly ILdapNameHandler ldapNameHandler;
		private readonly IDirectoryEntryAdapter directoryEntryAdapter;

		public IDnParser DnParser { get { return dnParser; } }
		public IDirectoryEntryFactory DirectoryEntryFactory { get { return directoryEntryFactory; } }
		public ISearcher UserSearcher { get { return userSearcher; } }
		public ISearcher GroupSearcher { get { return groupSearcher; } }
		public IFilterBuilder FilterBuilder { get { return filterBuilder; } }
		public ILdapNameHandler LdapNameHandler { get { return ldapNameHandler; } }
		public IDirectoryEntryAdapter DirectoryEntryAdapter { get { return directoryEntryAdapter; } }

		public Container(ILdapConfig configuration) {
			dnParser = new DnParser();
			directoryEntryFactory = new DirectoryEntryFactory();
			userSearcher = new UserSearcher(configuration);
			groupSearcher = new GroupSearcher(configuration);
			filterBuilder = new FilterBuilder(configuration);
			ldapNameHandler = new LdapNameHandler(configuration);
			directoryEntryAdapter = new DirectoryEntryAdapter(configuration);
		}
	}
}