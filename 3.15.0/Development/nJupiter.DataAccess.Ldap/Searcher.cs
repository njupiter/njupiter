#region Copyright & License
/*
	Copyright (c) 2005-2010 nJupiter

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

using System;
using System.DirectoryServices;

namespace nJupiter.DataAccess.Ldap {
	internal abstract class Searcher {

		private readonly Configuration config;

		protected Searcher(Configuration config) {
			if(config == null) {
				throw new ArgumentNullException("config");
			}
			this.config = config;
		}

		protected Configuration Config { get { return this.config; } }

		public DirectorySearcher Create(DirectoryEntry entry) {
			return this.Create(entry, SearchScope.Subtree);
		}

		public abstract DirectorySearcher Create(DirectoryEntry entry, SearchScope searchScope);

		protected DirectorySearcher CreateSearcher(DirectoryEntry entry, SearchScope searchScope, string rdnAttribute, string[] otherAttributes) {
			DirectorySearcher searcher = CreateSearcher(entry, searchScope, rdnAttribute);
			searcher.PropertiesToLoad.Clear();
			searcher.PropertiesToLoad.Add(rdnAttribute);
			foreach(string attribute in otherAttributes) {
				searcher.PropertiesToLoad.Add(attribute);
			}
			return searcher;
		}

		private DirectorySearcher CreateSearcher(DirectoryEntry entry, SearchScope searchScope, string rdnAttribute) {
			DirectorySearcher searcher = CreateSearcher(entry, searchScope);
			searcher.Sort.PropertyName = rdnAttribute;
			searcher.Sort.Direction = SortDirection.Ascending;
			return searcher;
		}

		public DirectorySearcher CreateSearcher(DirectoryEntry entry, SearchScope searchScope) {
			DirectorySearcher searcher = new DirectorySearcher(entry);
			searcher.SearchRoot = entry;
			searcher.SearchScope = searchScope;
			searcher.ServerTimeLimit = this.Config.Server.TimeLimit;
			searcher.PageSize = this.Config.Server.PageSize;
			searcher.PropertiesToLoad.Clear();
			return searcher;
		}
	}
}
