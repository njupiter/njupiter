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

namespace nJupiter.Services.Forum {

	public sealed class CategoriesResultConfiguration {
		#region Constants
		private const bool DefaultIncludeHidden = false;
		private const bool DefaultLoadAttributes = true;
		#endregion

		#region Variables
		#endregion

		#region Constructors
		public CategoriesResultConfiguration() : this(DefaultIncludeHidden, DefaultLoadAttributes, Category.DefaultSortProperty, null, Category.DefaultSortAscending) { }
		public CategoriesResultConfiguration(bool includeHidden, string sortAttributeName, bool sortAscending) : this(includeHidden, DefaultLoadAttributes, Category.DefaultSortProperty, sortAttributeName, sortAscending) { }
		public CategoriesResultConfiguration(bool includeHidden, bool loadAttributes, Category.Property sortProperty, bool sortAscending) : this(includeHidden, loadAttributes, sortProperty, null, sortAscending) { }
		public CategoriesResultConfiguration(bool includeHidden, bool loadAttributes, Category.Property sortProperty, string sortAttributeName, bool sortAscending) {
			this.IncludeHidden = includeHidden;
			this.LoadAttributes = loadAttributes;
			this.SortProperty = sortProperty;
			this.SortAttributeName = sortAttributeName;
			this.SortAscending = sortAscending;
		}
		#endregion

		#region Properties
		public bool IncludeHidden { get; set; }
		public bool LoadAttributes { get; set; }
		public Category.Property SortProperty { get; set; }
		public string SortAttributeName { get; set; }
		public bool SortAscending { get; set; }
		#endregion
	}

}
