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
using System.Collections;

namespace nJupiter.DataAccess.Users {

	public sealed class SearchCriteriaCollection : ICollection {
		#region Members
		private		ArrayList		innerArray;
		#endregion
		
		#region Constructors
		public SearchCriteriaCollection() {
			this.innerArray = new ArrayList();
		}
		#endregion

		#region Indexers
		public SearchCriteria this[int index] { get { return (SearchCriteria)InnerArray[index]; } }
		#endregion

		#region Properties
		internal ArrayList InnerArray { get { return this.innerArray; } }
		#endregion

		#region Public Methods
		public void Add(SearchCriteria searchCriteria) {
			InnerArray.Add(searchCriteria);
		}
		public void Remove(SearchCriteria searchCriteria) {
			InnerArray.Remove(searchCriteria);
		}
		public SearchCriteria[] ToArray() {
			return (SearchCriteria[])InnerArray.ToArray(typeof(SearchCriteria));
		}
		public bool Contains(SearchCriteria searchCriteria) {
			return(InnerArray.Contains(searchCriteria));
		}
		public void CopyTo(Array array, int index) {
			InnerArray.CopyTo(array, index);
		}
		public void CopyTo(SearchCriteria[] array, int index) {
			InnerArray.CopyTo(array, index);
		}
		public void Clear() {
			InnerArray.Clear();
		}
		public static SearchCriteriaCollection Synchronized(SearchCriteriaCollection nonSync) {
			if(nonSync == null) {
				throw new ArgumentNullException("nonSync");
			}
			SearchCriteriaCollection sync = new SearchCriteriaCollection();
			sync.innerArray = ArrayList.Synchronized(nonSync.innerArray);
			return sync;
		}
		#endregion

		#region Implementation of IEnumerable
		IEnumerator IEnumerable.GetEnumerator() {
			return this.innerArray.GetEnumerator();
		}
		#endregion
		
		#region Implementation of ICollection
		public bool		IsSynchronized	{ get { return InnerArray.IsSynchronized; } }
		public int		Count			{ get { return InnerArray.Count; } }
		public object	SyncRoot		{ get { return this; } }
		#endregion
	}
}
