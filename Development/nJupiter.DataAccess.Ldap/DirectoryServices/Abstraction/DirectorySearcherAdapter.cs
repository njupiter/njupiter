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