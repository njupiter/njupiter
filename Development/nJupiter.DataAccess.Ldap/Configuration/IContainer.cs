using nJupiter.Abstraction.Logging;
using nJupiter.DataAccess.Ldap.DirectoryServices;
using nJupiter.DataAccess.Ldap.DirectoryServices.Abstractions;
using nJupiter.DataAccess.Ldap.NameParser;

namespace nJupiter.DataAccess.Ldap.Configuration {
	internal interface IContainer {
		INameParser NameParser { get; }
		IDirectoryEntryFactory DirectoryEntryFactory { get; }
		ISearcherFactory SearcherFactory { get; }
		IFilterBuilder FilterBuilder { get; }
		IDirectoryEntryAdapter DirectoryEntryAdapter { get; }
		IUserEntryAdapter UserEntryAdapter { get; }
		IGroupEntryAdapter GroupEntryAdapter { get; }
		ILogManager LogManager { get; }

		void Build();
	}
}