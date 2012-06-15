using nJupiter.Abstraction.Logging;
using nJupiter.DataAccess.Ldap.DirectoryServices;
using nJupiter.DataAccess.Ldap.DirectoryServices.Abstractions;
using nJupiter.DataAccess.Ldap.NameParser;

namespace nJupiter.DataAccess.Ldap.Configuration {
	internal class Container : IContainer {
		private readonly INameParser nameParser;
		private readonly IDirectoryEntryFactory directoryEntryFactory;
		private readonly ISearcherFactory searcherFactory;
		private readonly IFilterBuilder filterBuilder;
		private readonly IDirectoryEntryAdapter directoryEntryAdapter;
		private readonly IUserEntryAdapter userEntryAdapter;
		private readonly IGroupEntryAdapter groupEntryAdapter;

		public INameParser NameParser { get { return nameParser; } }
		public IDirectoryEntryFactory DirectoryEntryFactory { get { return directoryEntryFactory; } }
		public ISearcherFactory SearcherFactory { get { return searcherFactory; } }
		public IFilterBuilder FilterBuilder { get { return filterBuilder; } }
		public IDirectoryEntryAdapter DirectoryEntryAdapter { get { return directoryEntryAdapter; } }
		public IUserEntryAdapter UserEntryAdapter { get { return userEntryAdapter; } }
		public IGroupEntryAdapter GroupEntryAdapter { get { return groupEntryAdapter; } }
		public ILogManager LogManager { get { return LogManagerFactory.GetLogManager(); } }

		public Container(ILdapConfig configuration) {
			nameParser = new NameParser.NameParser();
			directoryEntryFactory = new DirectoryEntryFactory();
			filterBuilder = new FilterBuilder(configuration.Server);
			searcherFactory = new SearcherFactory(configuration.Server, filterBuilder);
			directoryEntryAdapter = new DirectoryEntryAdapter(configuration.Server, directoryEntryFactory, filterBuilder, nameParser);
			groupEntryAdapter = new GroupEntryAdapter(configuration.Groups, directoryEntryAdapter, searcherFactory, nameParser);
			userEntryAdapter = new UserEntryAdapter(configuration, directoryEntryAdapter, searcherFactory, filterBuilder, nameParser);
		}

	}
}