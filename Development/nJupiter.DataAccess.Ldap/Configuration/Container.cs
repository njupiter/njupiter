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

using nJupiter.Abstraction.Logging;
using nJupiter.DataAccess.Ldap.DirectoryServices;
using nJupiter.DataAccess.Ldap.DirectoryServices.Abstraction;
using nJupiter.DataAccess.Ldap.DistinguishedNames;

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
			nameParser = new NameParser();
			directoryEntryFactory = new DirectoryEntryFactory();
			filterBuilder = new FilterBuilder(configuration.Server);
			searcherFactory = new SearcherFactory(configuration.Server, filterBuilder);
			directoryEntryAdapter = new DirectoryEntryAdapter(configuration.Server,
			                                                  directoryEntryFactory,
			                                                  filterBuilder,
			                                                  nameParser);
			groupEntryAdapter = new GroupEntryAdapter(configuration.Groups, directoryEntryAdapter, searcherFactory, nameParser);
			userEntryAdapter = new UserEntryAdapter(configuration,
			                                        directoryEntryAdapter,
			                                        searcherFactory,
			                                        filterBuilder,
			                                        nameParser);
		}
	}
}