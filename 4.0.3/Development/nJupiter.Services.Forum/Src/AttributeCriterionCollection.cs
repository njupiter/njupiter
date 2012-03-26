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

using System;
using System.Collections;

namespace nJupiter.Services.Forum {

	public sealed class AttributeCriterionCollection : ICollection {
		#region Variables
		private readonly ArrayList innerList;
		#endregion

		#region Constructors
		public AttributeCriterionCollection() : this(new ArrayList()) { }
		private AttributeCriterionCollection(ArrayList arrayList) {
			this.innerList = arrayList;
		}
		#endregion

		#region Properties
		public AttributeCriterion this[int index] { get { return (AttributeCriterion)this.innerList[index]; } }
		#endregion

		#region Methods
		public void Add(AttributeCriterion attributeCriterion) {
			this.innerList.Add(attributeCriterion);
		}
		public void Remove(AttributeCriterion attributeCriterion) {
			this.innerList.Remove(attributeCriterion);
		}
		public void Clear() {
			this.innerList.Clear();
		}
		public AttributeCriterion[] ToArray() {
			AttributeCriterion[] attributeCriterions = new AttributeCriterion[this.Count];
			CopyTo(attributeCriterions, 0);
			return attributeCriterions;
		}
		public bool Contains(AttributeCriterion attributeCriterion) {
			return this.innerList.Contains(attributeCriterion);
		}
		void ICollection.CopyTo(Array array, int index) {
			this.innerList.CopyTo(array, index);
		}
		public void CopyTo(AttributeCriterion[] array, int index) {
			this.innerList.CopyTo(array, index);
		}
		public static AttributeCriterionCollection Synchronized(AttributeCriterionCollection nonSync) {
			if(nonSync == null) {
				throw new ArgumentNullException("nonSync");
			}
			return new AttributeCriterionCollection(ArrayList.Synchronized(nonSync.innerList));
		}
		#endregion

		#region Implementation of IEnumerable
		IEnumerator IEnumerable.GetEnumerator() {
			return this.innerList.GetEnumerator();
		}
		#endregion

		#region Implementation of ICollection
		public bool IsSynchronized { get { return this.innerList.IsSynchronized; } }
		public int Count { get { return this.innerList.Count; } }
		public object SyncRoot { get { return this; } }
		#endregion
	}

}