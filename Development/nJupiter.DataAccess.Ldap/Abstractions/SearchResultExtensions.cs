using System.Collections.Generic;
using System.DirectoryServices;

namespace nJupiter.DataAccess.Ldap.Abstractions {
	internal static class SearchResultExtensions {
		public static IEnumerable<ISearchResult> Wrap(this SearchResultCollection wrapped) {
			var result = new List<ISearchResult>();
			foreach(SearchResult searchResult in wrapped) {
				result.Add(searchResult.Wrap());
			}
			return result;
		}
		
		public static ISearchResult Wrap(this SearchResult wrapped) {
			if(wrapped == null) {
				return null;
			}
			return new SearchResultWrapper(wrapped);
		}	
		
	}
}