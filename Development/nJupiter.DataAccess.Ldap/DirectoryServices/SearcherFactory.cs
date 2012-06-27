#region Copyright & License
/*
	Copyright (c) 2005-2011 nJupiter

	Permission is hereby granted, free of charge, to any person obtaining a copy
	of this software and associated documentation files (the "Software"), to deal
	in the Software without restriction, including without limitation the rights
	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	copies of the Software, and to permit persons to whom the Software is
	furnished to do so, subject to the following conditions:

	The above copyright notice and this permission notice shall be included in
	all copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
	THE SOFTWARE.
*/
#endregion

using System.DirectoryServices;

using nJupiter.DataAccess.Ldap.Configuration;
using nJupiter.DataAccess.Ldap.DirectoryServices.Abstraction;

namespace nJupiter.DataAccess.Ldap.DirectoryServices {
	internal class SearcherFactory : ISearcherFactory {

		private readonly IServerConfig serverConfig;

		private readonly IFilterBuilder filterBuilder;

		public SearcherFactory(IServerConfig serverConfig, IFilterBuilder filterBuilder) {
			this.serverConfig = serverConfig;
			this.filterBuilder = filterBuilder;
		}

		public IDirectorySearcher CreateSearcher(IEntry entry, SearchScope searchScope, IEntryConfig entryConfig) {
			var searcher = CreateSearcher(entry, searchScope, entryConfig.RdnAttribute);
			searcher.PropertiesToLoad.Add(entryConfig.RdnAttribute);
			foreach(var attribute in entryConfig.Attributes) {
				searcher.PropertiesToLoad.Add(attribute.Name);
			}
			return searcher;
		}

		private IDirectorySearcher CreateSearcher(IEntry entry, SearchScope searchScope, string rdnAttribute) {
			var searcher = CreateSearcher(entry, searchScope);
			if(serverConfig.PropertySortingSupport) {
				searcher.Sort.PropertyName = rdnAttribute;
				searcher.Sort.Direction = SortDirection.Ascending;
			}
			return searcher;
		}

		public IDirectorySearcher CreateSearcher(IEntry entry, SearchScope searchScope) {
			var searcher = new DirectorySearcherAdapter(entry, filterBuilder);
			searcher.SearchRoot = entry.GetDirectoryEntry();
			searcher.SearchScope = searchScope;
			searcher.ServerTimeLimit = serverConfig.TimeLimit;
			SetPageSizeIfNotDefault(searcher);
			SetSizeLimitIfNotDefault(searcher);
			searcher.PropertiesToLoad.Clear();
			return searcher;
		}

		private void SetPageSizeIfNotDefault(DirectorySearcherAdapter searcher) {
			if(serverConfig.PageSize > 0) {
				searcher.PageSize = serverConfig.PageSize;
			}
		}

		private void SetSizeLimitIfNotDefault(DirectorySearcherAdapter searcher) {
			if(serverConfig.SizeLimit != 1000) {
				searcher.SizeLimit = serverConfig.SizeLimit;
			}
		}
	}
}
