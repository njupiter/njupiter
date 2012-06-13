using System.Collections.Generic;
using System.DirectoryServices;

namespace nJupiter.DataAccess.Ldap.DirectoryServices.Abstractions {
	internal static class SearchResultExtensions {
		public static IEnumerable<IEntry> Wrap(this SearchResultCollection wrapped) {
			var result = new List<IEntry>();
			foreach(SearchResult searchResult in wrapped) {
				result.Add(searchResult.Wrap());
			}
			return result;
		}
		
		public static IEntry Wrap(this SearchResult wrapped) {
			if(wrapped == null) {
				return null;
			}
			return new SearchResultFacade(wrapped);
		}	
		
	}
}