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

using System.Collections;
using System.DirectoryServices;

namespace nJupiter.DataAccess.Ldap.DirectoryServices.Abstraction {
	public class SearchResultFacade : IEntry {
		private readonly SearchResult searchResult;
		private IDirectoryEntry internalEntry;

		public SearchResultFacade(SearchResult searchResult) {
			this.searchResult = searchResult;
		}

		public virtual IDirectoryEntry GetDirectoryEntry() {
			return internalEntry ?? (internalEntry = searchResult.GetDirectoryEntry().Wrap());
		}

		public virtual string Path { get { return searchResult.Path; } }

		public virtual IDictionary Properties { get { return searchResult.Properties; } }

		public virtual object NativeObject { get { return GetDirectoryEntry().NativeObject; } }
		public virtual string Name { get { return GetDirectoryEntry().Name; } }

		public virtual void Dispose() {
			if(internalEntry != null) {
				internalEntry.Dispose();
			}
		}
	}
}