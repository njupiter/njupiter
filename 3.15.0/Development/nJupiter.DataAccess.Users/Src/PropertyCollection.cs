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
	// TODO: Implement clonable
	[Serializable]
	public sealed class PropertyCollection : ICollection {
		
		#region Members
		private				Hashtable			innerHash;
		private readonly	PropertySchemaTable	propertySchemaTable;
		#endregion
		
		#region Constructors
		internal PropertyCollection(PropertySchemaTable pst) {
			this.innerHash = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
			this.propertySchemaTable =  (pst ?? new PropertySchemaTable());
		}
		#endregion

		#region Indexers
		public AbstractProperty this[string propertyName]	{ get { return (AbstractProperty) InnerHash[propertyName]; } }
		#endregion

		#region Properties
		internal Hashtable InnerHash { get { return this.innerHash; } }
		internal PropertySchemaTable PropertySchemas  { get { return this.propertySchemaTable; } }
		#endregion

		#region Public Methods
		internal void Add(AbstractProperty property) {
			if(property == null)
				throw new ArgumentNullException("property");
			InnerHash.Add(property.Name, property);
		}

		public AbstractProperty[] ToArray() {
			return (AbstractProperty[])(new ArrayList(InnerHash.Values)).ToArray(typeof(AbstractProperty));
		}
		
		public bool Contains(AbstractProperty property) {
			return(InnerHash.ContainsValue(property));
		}
		
		public bool Contains(string propertyName) {
			return(InnerHash.Contains(propertyName));
		}

		public void CopyTo(Array array, int index) {
			InnerHash.Values.CopyTo(array, index);
		}

		public void CopyTo(AbstractProperty[] array, int index) {
			InnerHash.Values.CopyTo(array, index);
		}

		public static PropertyCollection Synchronized(PropertyCollection nonSync) {
			if(nonSync == null) {
				throw new ArgumentNullException("nonSync");
			}
			PropertyCollection sync = new PropertyCollection(nonSync.propertySchemaTable);
			sync.innerHash = Hashtable.Synchronized(nonSync.innerHash);
			return sync;
		}
		#endregion

		#region Implementation of IEnumerable
		IEnumerator IEnumerable.GetEnumerator() {
			return new PropertyCollectionEnumerator(this);
		}

		public PropertyCollectionEnumerator GetEnumerator() {
			return new PropertyCollectionEnumerator(this);
		}
		#endregion
		
		#region Implementation of ICollection
		public bool		IsSynchronized	{ get { return InnerHash.IsSynchronized; } }
		public int		Count			{ get { return InnerHash.Count; } }
		public object	SyncRoot		{ get { return this; } }
		#endregion

	}
	
	#region PropertyCollectionEnumerator
	[Serializable]
	public class PropertyCollectionEnumerator : IEnumerator {
		private readonly IEnumerator innerEnumerator;
		
		public PropertyCollectionEnumerator (PropertyCollection enumerable) {
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
		
		object IEnumerator.Current			{ get { return ((DictionaryEntry) innerEnumerator.Current).Value; } }
		public AbstractProperty Current		{ get { return (AbstractProperty)((DictionaryEntry) innerEnumerator.Current).Value; } }
		#endregion
	}
	#endregion
}
