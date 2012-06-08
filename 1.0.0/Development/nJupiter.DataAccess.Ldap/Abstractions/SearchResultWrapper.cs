using System.DirectoryServices;

namespace nJupiter.DataAccess.Ldap.Abstractions {
	internal class SearchResultWrapper : ISearchResult {
		readonly SearchResult searchResult;
		
		public SearchResultWrapper(SearchResult searchResult) {
			this.searchResult = searchResult;
		}

		public IDirectoryEntry GetDirectoryEntry() {
			return searchResult.GetDirectoryEntry().Wrap();
		}

		public string Path {
			get { return searchResult.Path; } }

		public ResultPropertyCollection Properties {
			get { return searchResult.Properties; } }
	}
}