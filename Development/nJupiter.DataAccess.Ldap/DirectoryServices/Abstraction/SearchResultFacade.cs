using System.Collections;
using System.DirectoryServices;

namespace nJupiter.DataAccess.Ldap.DirectoryServices.Abstraction {
	internal class SearchResultFacade : IEntry {
		private readonly SearchResult searchResult;
		private IDirectoryEntry internalEntry;
		
		public SearchResultFacade(SearchResult searchResult) {
			this.searchResult = searchResult;
		}

		public IDirectoryEntry GetDirectoryEntry() {
			return internalEntry ?? (internalEntry = searchResult.GetDirectoryEntry().Wrap());
		}

		public string Path { get { return searchResult.Path; } }

		public IDictionary Properties {
			get { return searchResult.Properties; } }

		public object NativeObject { get { return GetDirectoryEntry().NativeObject; } }
		public string Name { get { return GetDirectoryEntry().Name; } }

		public void Dispose() {
			if(internalEntry != null) {
				internalEntry.Dispose();
			}
		}
	}
}