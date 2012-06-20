using System.Collections.Generic;

namespace nJupiter.DataAccess.Ldap.DirectoryServices.Abstraction {
	internal class DirectorySearcherAdapter : DirectorySearcherWrapper, IDirectorySearcher {
		private readonly IFilterBuilder filterBuilder;

		public DirectorySearcherAdapter(IEntry directoryEntry, IFilterBuilder filterBuilder) : base(directoryEntry) {
			this.filterBuilder = filterBuilder;
		}

		public IEnumerable<T> GetPropertiesByRangedFilter<T>(string propertyName) {
			uint rangeLow = 0;
			var rangeHigh = rangeLow;
			var isLastQuery = false;
			var endOfRange = false;
			do {
				var rangeFilter = filterBuilder.CreatePropertyRangeFilter(propertyName, rangeLow, rangeHigh, isLastQuery);
				var searchResult = FindOne(rangeFilter);
				if(searchResult.ContainsProperty(rangeFilter)) {
					foreach(var property in searchResult.GetProperties<T>(rangeFilter)) {
						yield return property;
					}
					endOfRange = isLastQuery;
				} else {
					isLastQuery = true;
				}
				if(!isLastQuery) {
					rangeLow = rangeHigh + 1;
					rangeHigh = rangeLow;
				}
			} while(!endOfRange);
		}

		public IEntry FindOne(string propertyToLoad) {
			NewPropertyToLoad(propertyToLoad);
			return FindOne();
		}

		private void NewPropertyToLoad(string propertyName) {
			PropertiesToLoad.Clear();
			PropertiesToLoad.Add(propertyName);
		}
	}
}