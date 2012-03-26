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
using System.Collections.Generic;

namespace nJupiter.Collections.Generics {

	[Serializable]
	public class SortableCollection<T> : IList<T>, IList {

		[NonSerialized]
		private object				syncRoot;
		private readonly List<T>	items;

		public SortableCollection() {
			this.items = new List<T>();
		}

		public SortableCollection(IList<T> list) {
			if(list == null)
				throw new ArgumentNullException("list");
			List<T> innerList = list as List<T>;
			if(innerList == null)
				innerList = new List<T>(list);
			this.items = innerList;
		}

		public void Add(T item) {
			if(this.Items.IsReadOnly)
				throw new NotSupportedException("The collection is read only");
			int count = this.items.Count;
			this.InsertItem(count, item);
		}

		public void AddRange(IEnumerable<T> collection) {
			this.items.InsertRange(this.Count, collection);
		}

		public void InsertRange(int index, IEnumerable<T> collection) {
			this.items.InsertRange(index, collection);
		}

		public void InsertRange(int index, int count) {
			this.items.RemoveRange(index, count);
		}

		public IList<T> GetRange(int index, int count) {
			return this.items.GetRange(index, count);
		}

		public void Clear() {
			if(this.Items.IsReadOnly)
				throw new NotSupportedException("The collection is read only");
			this.ClearItems();
		}

		protected virtual void ClearItems() {
			this.items.Clear();
		}

		public bool Contains(T item) {
			return this.items.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex) {
			this.items.CopyTo(array, arrayIndex);
		}

		public IEnumerator<T> GetEnumerator() {
			return this.items.GetEnumerator();
		}

		public int IndexOf(T item) {
			return this.items.IndexOf(item);
		}

		public void Insert(int index, T item) {
			if((index < 0) || (index > this.items.Count))
				throw new ArgumentOutOfRangeException();
			this.InsertItem(index, item);
		}

		protected virtual void InsertItem(int index, T item) {
			this.items.Insert(index, item);
		}

		private static bool IsCompatibleObject(object value) {
			if(!(value is T) && ((value != null) || typeof(T).IsValueType)) {
				return false;
			}
			return true;
		}

		public bool Remove(T item) {
			if(this.Items.IsReadOnly)
				throw new NotSupportedException("The collection is read only");
			int index = this.items.IndexOf(item);
			if(index < 0) {
				return false;
			}
			this.RemoveItem(index);
			return true;
		}

		public void RemoveAt(int index) {
			if(this.Items.IsReadOnly)
				throw new NotSupportedException("The collection is read only");
			if((index < 0) || (index >= this.items.Count))
				throw new ArgumentOutOfRangeException();
			this.RemoveItem(index);
		}

		protected virtual void RemoveItem(int index) {
			this.items.RemoveAt(index);
		}

		protected virtual void SetItem(int index, T item) {
			this.items[index] = item;
		}

		void ICollection.CopyTo(Array array, int index) {
			if(array == null)
				throw new ArgumentNullException("array");
			if(array.Rank != 1)
				throw new ArgumentException("Multi-dimentional rank not supported");
			if(array.GetLowerBound(0) != 0)
				throw new ArgumentException("Lower bound is non zero");
			if(index < 0)
				throw new ArgumentOutOfRangeException("index", "Index can not be negative");
			if((array.Length - index) < this.Count)
				throw new ArgumentException("Array length plus offset is too small");
			T[] localArray = array as T[];
			if(localArray != null) {
				this.items.CopyTo(localArray, index);
			} else {
				Type elementType = array.GetType().GetElementType();
				Type c = typeof(T);
				if(!elementType.IsAssignableFrom(c) && !c.IsAssignableFrom(elementType))
					throw new ArgumentException("Invalid array type");
				object[] objArray = array as object[];
				if(objArray == null)
					throw new ArgumentException("Invalid array type");
				int count = this.items.Count;
				try {
					for(int i = 0; i < count; i++) {
						objArray[index++] = this.items[i];
					}
				} catch(ArrayTypeMismatchException) {
					throw new ArgumentException("Invalid array type");
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return this.items.GetEnumerator();
		}

		int IList.Add(object value) {
			if(this.Items.IsReadOnly)
				throw new NotSupportedException("The collection is read only");
			SortableCollection<T>.VerifyValueType(value);
			this.Add((T)value);
			return (this.Count - 1);
		}

		bool IList.Contains(object value) {
			if(SortableCollection<T>.IsCompatibleObject(value)) {
				return this.Contains((T)value);
			}
			return false;
		}

		int IList.IndexOf(object value) {
			if(SortableCollection<T>.IsCompatibleObject(value)) {
				return this.IndexOf((T)value);
			}
			return -1;
		}

		void IList.Insert(int index, object value) {
			if(this.Items.IsReadOnly)
				throw new NotSupportedException("The collection is read only");
			SortableCollection<T>.VerifyValueType(value);
			this.Insert(index, (T)value);
		}

		void IList.Remove(object value) {
			if(this.Items.IsReadOnly)
				throw new NotSupportedException("The collection is read only");
			if(SortableCollection<T>.IsCompatibleObject(value)) {
				this.Remove((T)value);
			}
		}

		private static void VerifyValueType(object value) {
			if(!SortableCollection<T>.IsCompatibleObject(value)) {
				throw new ArgumentException(string.Format("value {0} has to be of type {1}", value, typeof(T).FullName));
			}
		}

		public int Count {
			get {
				return this.items.Count;
			}
		}

		public T this[int index] {
			get {
				return this.items[index];
			}
			set {
				if(this.Items.IsReadOnly)
					throw new NotSupportedException("The collection is read only");
				if((index < 0) || (index >= this.items.Count))
					throw new ArgumentOutOfRangeException();
				this.SetItem(index, value);
			}
		}

		protected IList<T> Items {
			get {
				return this.items;
			}
		}

		public T[] ToArray() {
			return this.items.ToArray();
		}

		bool ICollection<T>.IsReadOnly {
			get {
				return this.Items.IsReadOnly;
			}
		}

		public bool IsSynchronized {
			get {
				return false;
			}
		}

		public object SyncRoot {
			get {
				if(this.syncRoot == null) {
					ICollection collection = this.items;
					if(collection != null) {
						this.syncRoot = collection.SyncRoot;
					} else {
						System.Threading.Interlocked.CompareExchange(ref this.syncRoot, new object(), null);
					}
				}
				return this.syncRoot;
			}
		}

		public bool IsFixedSize {
			get {
				IList collection = this.items;
				if(collection != null) {
					return collection.IsFixedSize;
				}
				return false;
			}
		}

		public bool IsReadOnly {
			get {
				return this.Items.IsReadOnly;
			}
		}

		object IList.this[int index] {
			get {
				return this.items[index];
			}
			set {
				SortableCollection<T>.VerifyValueType(value);
				this[index] = (T)value;
			}
		}


		public void Sort() {
			this.Sort(0, this.Count, null);
		}

		public void Sort(IComparer<T> comparer) {
			this.Sort(0, this.Count, comparer);
		}

		public void Sort(Comparison<T> comparison) {
			if(comparison == null)
				throw new ArgumentNullException("comparison");
			this.items.Sort(comparison);	
		}

		public void Sort(int index, int count, IComparer<T> comparer) {
			if((index < 0) || (count < 0))
				throw new ArgumentOutOfRangeException("index", "Index and cound can not be negative");
			if((this.Count - index) < count)
				throw new ArgumentException("Invalid offset length");
			this.items.Sort(index, count, comparer);
		}
	}

}
