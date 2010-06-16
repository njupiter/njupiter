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
	[Serializable]
	public sealed class PropertySchemaTable : ICollection {

		#region Members
		private Hashtable innerHash;
		#endregion

		#region Constructors
		internal PropertySchemaTable() {
			this.innerHash = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
		}
		#endregion

		#region Indexers
		public PropertySchema this[string propertyName] { get { return (PropertySchema)InnerHash[propertyName]; } }
		#endregion

		#region Properties
		public ICollection Keys { get { return InnerHash.Keys; } }
		public ICollection Values { get { return InnerHash.Values; } }
		internal Hashtable InnerHash { get { return this.innerHash; } }
		#endregion

		#region Methods
		internal void Add(PropertySchema propertySchema) {
			if(propertySchema == null)
				throw new ArgumentNullException("propertySchema");
			InnerHash.Add(propertySchema.PropertyName, propertySchema);
		}

		public PropertySchemaTable FilterOnType(Type dataType) {
			PropertySchemaTable pdc = new PropertySchemaTable();
			foreach(PropertySchema pd in this) {
				if(pd.DataType.Equals(dataType)) {
					pdc.Add(pd);
				}
			}
			return pdc;
		}

		public PropertySchema[] ToArray() {
			return (PropertySchema[])(new ArrayList(InnerHash.Values)).ToArray(typeof(PropertySchema));
		}

		public bool Contains(PropertySchema propertySchema) {
			return (InnerHash.ContainsValue(propertySchema));
		}

		public bool Contains(string propertyName) {
			return (InnerHash.Contains(propertyName));
		}

		public void CopyTo(Array array, int index) {
			InnerHash.Values.CopyTo(array, index);
		}

		public void CopyTo(PropertySchema[] array, int index) {
			InnerHash.Values.CopyTo(array, index);
		}

		public static PropertySchemaTable Synchronized(PropertySchemaTable nonSync) {
			if(nonSync == null) {
				throw new ArgumentNullException("nonSync");
			}
			PropertySchemaTable sync = new PropertySchemaTable();
			sync.innerHash = Hashtable.Synchronized(nonSync.innerHash);
			return sync;
		}
		#endregion

		#region Implementation of IEnumerable
		IEnumerator IEnumerable.GetEnumerator() {
			return new PropertySchemaTableEnumerator(this);
		}

		public PropertySchemaTableEnumerator GetEnumerator() {
			return new PropertySchemaTableEnumerator(this);
		}
		#endregion

		#region Implementation of ICollection
		public bool IsSynchronized { get { return InnerHash.IsSynchronized; } }
		public int Count { get { return InnerHash.Count; } }
		public object SyncRoot { get { return this; } }
		#endregion
	}

	#region PropertySchemaTableEnumerator
	[Serializable]
	public class PropertySchemaTableEnumerator : IEnumerator {

		private readonly IEnumerator innerEnumerator;

		public PropertySchemaTableEnumerator(PropertySchemaTable enumerable) {
			if(enumerable == null)
				throw new ArgumentNullException("enumerable");

			innerEnumerator = enumerable.InnerHash.GetEnumerator();
		}

		#region Implementation of IEnumerator
		public void Reset() {
			innerEnumerator.Reset();
		}

		public bool MoveNext() {
			return innerEnumerator.MoveNext();
		}

		object IEnumerator.Current { get { return ((DictionaryEntry)innerEnumerator.Current).Value; } }
		public PropertySchema Current { get { return (PropertySchema)((DictionaryEntry)innerEnumerator.Current).Value; } }
		#endregion
	}
	#endregion
}
