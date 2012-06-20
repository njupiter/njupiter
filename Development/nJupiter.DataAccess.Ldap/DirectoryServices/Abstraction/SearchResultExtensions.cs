using System.DirectoryServices;

namespace nJupiter.DataAccess.Ldap.DirectoryServices.Abstraction {
	internal static class SearchResultExtensions {
		public static IEntryCollection Wrap(this SearchResultCollection wrapped) {
			var result = new EntryCollection();
			if(wrapped != null) {
				foreach(SearchResult searchResult in wrapped) {
					result.Add(searchResult.Wrap());
				}
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