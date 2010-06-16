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

namespace nJupiter.Services.Forum {

	public sealed class SearchCriteria {
		#region Constants
		private const FullTextSearchType DefaultFulltextSearchType = FullTextSearchType.Normal;
		#endregion

		#region Variables
		private readonly AttributeCriterionCollection attributeCriteria;
		#endregion

		#region Constructors
		public SearchCriteria(string fullTextSearchText, FullTextSearchType fullTextSearchType, string userIdentity, AttributeCriterionCollection attributeCriteria) {
			this.FullTextSearchText = fullTextSearchText;
			this.FullTextSearchType = fullTextSearchType;
			this.UserIdentity = userIdentity;
			this.attributeCriteria = attributeCriteria ?? new AttributeCriterionCollection();
		}
		public SearchCriteria(string fullTextSearchText, string userIdentity, AttributeCriterionCollection attributeCriteria) : this(fullTextSearchText, DefaultFulltextSearchType, userIdentity, attributeCriteria) { }
		public SearchCriteria() : this(null, DefaultFulltextSearchType, null, null) { }
		public SearchCriteria(string fullTextSearchText, FullTextSearchType fullTextSearchType) : this(fullTextSearchText, fullTextSearchType, null, null) { }
		public SearchCriteria(string fullTextSearchText) : this(fullTextSearchText, DefaultFulltextSearchType) { }
		public SearchCriteria(string fullTextSearchText, AttributeCriterionCollection attributeCriteria) : this(fullTextSearchText, DefaultFulltextSearchType, null, attributeCriteria) { }
		public SearchCriteria(AttributeCriterionCollection attributeCriteria) : this(null, attributeCriteria) { }
		#endregion

		#region Properties
		public string FullTextSearchText { get; set; }
		public FullTextSearchType FullTextSearchType { get; set; }
		public string UserIdentity { get; set; }
		public AttributeCriterionCollection AttributeCriteria { get { return this.attributeCriteria; } }
		#endregion
	}

}
