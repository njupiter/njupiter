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

		public Container(ILdapConfig configuration) {
			nameParser = new NameParser.NameParser();
			directoryEntryFactory = new DirectoryEntryFactory();
			searcherFactory = new SearcherFactory(configuration);
			filterBuilder = new FilterBuilder(configuration);
			directoryEntryAdapter = new DirectoryEntryAdapter(configuration);
			userEntryAdapter = new UserEntryAdapter(configuration);
			groupEntryAdapter = new GroupEntryAdapter(configuration);
		}
	}
}