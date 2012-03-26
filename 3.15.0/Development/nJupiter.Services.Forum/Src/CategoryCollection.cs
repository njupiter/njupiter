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
using System.Globalization;

namespace nJupiter.Services.Forum {

	[Serializable]
	public sealed class CategoryCollection : ICollection {
		#region Variables
		private readonly Hashtable			innerHash;
		private readonly CategoryComparer	comparer;
		[NonSerialized]
		private ArrayList					innerList;
		[NonSerialized]
		private bool						isSorted;
		#endregion
		
		#region Constructors
		internal CategoryCollection(Category.Property sortProperty, string sortAttributeName, bool sortAscending) : this(new CategoryComparer(sortProperty, sortAttributeName, sortAscending), new Hashtable()) {}
		private CategoryCollection(CategoryComparer comparer, Hashtable hashTable) {
			this.comparer		= comparer;
			this.innerHash		= hashTable;
		}
		#endregion

		#region Properties
		public Category this[CategoryId id] { get { return (Category)this.innerHash[id]; } }
		public Category this[int index] { get { return (Category)InnerList[index]; } }

		private ArrayList InnerList { 
			get { 
				if(!this.isSorted) {
					ArrayList list = new ArrayList(this.innerHash.Values);
					this.innerList = this.innerList != null && this.innerList.IsSynchronized ? ArrayList.Synchronized(list) : list;
					this.innerList.Sort(this.comparer);
					this.isSorted = true;
				}
				return this.innerList; 
			} 
		}
		#endregion

		#region Methods
		public Category[] ToArray() {
			Category[] categories = new Category[this.Count];
			CopyTo(categories, 0);
			return categories;
		}
		public bool Contains(Category category) {
			return category != null && this.innerHash.ContainsKey(category.Id);
		}
		void ICollection.CopyTo(Array array, int index) {
			InnerList.CopyTo(array, index);
		}
		public void CopyTo(Category[] array, int index) {
			InnerList.CopyTo(array, index);
		}
		public static CategoryCollection Synchronized(CategoryCollection nonSync) {
			if(nonSync == null) {
				throw new ArgumentNullException("nonSync");
			}
			return new CategoryCollection(nonSync.comparer, Hashtable.Synchronized(nonSync.innerHash));
		}
		#endregion

		#region Internal Methods
		internal void Add(Category category) {
			this.innerHash.Add(category.Id, category);
			this.isSorted = false;
		}
		#endregion

		#region Implementation of IEnumerable
		IEnumerator IEnumerable.GetEnumerator() {
			return InnerList.GetEnumerator();
		}
		#endregion
		
		#region Implementation of ICollection
		public bool IsSynchronized { get { return InnerList.IsSynchronized; } }
		public int Count { get { return this.innerHash.Count; } }
		public object SyncRoot { get { return this; } }
		#endregion

		#region Helper Classes
		[Serializable]
		private sealed class CategoryComparer : IComparer {
			#region Variables
			private readonly bool				sortAscending;
			private readonly Category.Property	sortProperty;
			private readonly string				sortAttributeName;
			#endregion

			#region Constructors
			public CategoryComparer(Category.Property sortProperty, string sortAttributeName, bool sortAscending) {
				this.sortProperty		= sortProperty;
				this.sortAttributeName	= sortAttributeName;
				this.sortAscending		= sortAscending;
			}
			#endregion
			
			#region IComparer Members
			public int Compare(object x, object y) {
				Category categoryx = (Category)x;
				Category categoryy = (Category)y;
				int compareValue;

				if(this.sortAttributeName == null) {
					switch(this.sortProperty) {
						case Category.Property.Id:
							compareValue = categoryx.Id.CompareTo(categoryy.Id);
							break;
						case Category.Property.Domain:
							compareValue = string.Compare(categoryx.Domain, categoryy.Domain, true, CultureInfo.CurrentCulture);
							break;
						case Category.Property.RootPostCount:
							compareValue = categoryx.RootPostCount.CompareTo(categoryy.RootPostCount);
							break;
						case Category.Property.Visible:
							compareValue = categoryx.Visible.CompareTo(categoryy.Visible);
							break;
						case Category.Property.ConcurrencyIdentity:
							compareValue = string.Compare(categoryx.ConcurrencyIdentity, categoryy.ConcurrencyIdentity, true, CultureInfo.CurrentCulture);
							break;
						default:
							compareValue = string.Compare(categoryx.Name, categoryy.Name, true, CultureInfo.CurrentCulture);
							break;
					}
				} else {
					object valuex	= categoryx.Attributes[this.sortAttributeName].Value;
					object valuey	= categoryy.Attributes[this.sortAttributeName].Value;
					compareValue	= valuex == null ? valuey == null ? 0 : -1 : ((IComparable)valuex).CompareTo(valuey);
				}
				return (this.sortAscending ? 1 : -1) * compareValue;
			}
			#endregion
		}
		#endregion
	}

}