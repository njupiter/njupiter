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

namespace nJupiter.Services.Forum {

	[Serializable]
	public sealed class AttributeCollection : ICollection {
		#region Variables
		private readonly Hashtable innerHash;
		#endregion

		#region Constructors
		internal AttributeCollection() : this(new Hashtable()) { }
		private AttributeCollection(Hashtable hashTable) {
			this.innerHash = hashTable;
		}
		#endregion

		#region Properties
		public Attribute this[string name] { get { return (Attribute)this.innerHash[name]; } }
		#endregion

		#region Methods
		public Attribute[] ToArray() {
			Attribute[] attributes = new Attribute[this.Count];
			CopyTo(attributes, 0);
			return attributes;
		}
		public bool Contains(Attribute attribute) {
			return this.innerHash.ContainsValue(attribute);
		}
		public bool Contains(string attributeName) {
			return this.innerHash.ContainsKey(attributeName);
		}
		void ICollection.CopyTo(Array array, int index) {
			this.innerHash.Values.CopyTo(array, index);
		}
		public void CopyTo(Attribute[] array, int index) {
			this.innerHash.Values.CopyTo(array, index);
		}
		public static AttributeCollection Synchronized(AttributeCollection nonSync) {
			if(nonSync == null) {
				throw new ArgumentNullException("nonSync");
			}
			return new AttributeCollection(Hashtable.Synchronized(nonSync.innerHash));
		}
		#endregion

		#region Internal Methods
		internal AttributeCollection Clone() {
			AttributeCollection attributes = new AttributeCollection();
			foreach(Attribute attribute in this.innerHash.Values) {
				attributes.Add(attribute.Clone());
			}
			return attributes;
		}
		internal void Add(Attribute attribute) {
			this.innerHash.Add(attribute.Name, attribute);
		}
		#endregion

		#region Implementation of IEnumerable
		IEnumerator IEnumerable.GetEnumerator() {
			return this.innerHash.Values.GetEnumerator();
		}
		#endregion

		#region Implementation of ICollection
		public bool IsSynchronized { get { return this.innerHash.IsSynchronized; } }
		public int Count { get { return this.innerHash.Count; } }
		public object SyncRoot { get { return this; } }
		#endregion
	}

}