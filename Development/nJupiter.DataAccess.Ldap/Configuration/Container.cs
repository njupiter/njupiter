using nJupiter.Abstraction.Logging;
using nJupiter.DataAccess.Ldap.DirectoryServices;
using nJupiter.DataAccess.Ldap.DirectoryServices.Abstractions;
using nJupiter.DataAccess.Ldap.NameParser;

namespace nJupiter.DataAccess.Ldap.Configuration {
	internal class Container : IContainer {
		private readonly ILdapConfig configuration;
		private INameParser nameParser;
		private IDirectoryEntryFactory directoryEntryFactory;
		private ISearcherFactory searcherFactory;
		private IFilterBuilder filterBuilder;
		private IDirectoryEntryAdapter directoryEntryAdapter;
		private IUserEntryAdapter userEntryAdapter;
		private IGroupEntryAdapter groupEntryAdapter;

		public INameParser NameParser { get { return nameParser; } }
		public IDirectoryEntryFactory DirectoryEntryFactory { get { return directoryEntryFactory; } }
		public ISearcherFactory SearcherFactory { get { return searcherFactory; } }
		public IFilterBuilder FilterBuilder { get { return filterBuilder; } }
		public IDirectoryEntryAdapter DirectoryEntryAdapter { get { return directoryEntryAdapter; } }
		public IUserEntryAdapter UserEntryAdapter { get { return userEntryAdapter; } }
		public IGroupEntryAdapter GroupEntryAdapter { get { return groupEntryAdapter; } }
		public ILogManager LogManager { get { return LogManagerFactory.GetLogManager(); } }

		public Container(ILdapConfig configuration) {
			this.configuration = configuration;
		}

		public void Build() {
			nameParser = new NameParser.NameParser();
			directoryEntryFactory = new DirectoryEntryFactory();
			filterBuilder = new FilterBuilder(configuration);
			searcherFactory = new SearcherFactory(configuration);
			directoryEntryAdapter = new DirectoryEntryAdapter(configuration);
			userEntryAdapter = new UserEntryAdapter(configuration);
			groupEntryAdapter = new GroupEntryAdapter(configuration);
		}
	}
}