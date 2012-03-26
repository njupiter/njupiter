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
	internal sealed class ContextualPropertyCollectionTable : ICollection {
		#region Members
		private		Hashtable	innerHash;
		#endregion
		
		#region Constructors
		internal ContextualPropertyCollectionTable() {
			this.innerHash = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
		}
		#endregion

		#region Indexers
		public PropertyCollection this[Context context] { get{ return (PropertyCollection) InnerHash[context]; } }
		#endregion

		#region Properties
		internal Hashtable InnerHash { get { return this.innerHash; } }
		#endregion

		#region Methods
		internal void Add(Context context, PropertyCollection pc) {
			InnerHash.Add(context, pc);
		}

		internal void Remove(Context context) {
			InnerHash.Remove(context);
		}
		
		internal bool Contains(Context context) {
			return(InnerHash.Contains(context));
		}

		public void CopyTo(Array array, int index) {
			InnerHash.Values.CopyTo(array, index);
		}

		public void CopyTo(Context[] array, int index) {
			InnerHash.Keys.CopyTo(array, index);
		}

		public static ContextualPropertyCollectionTable Synchronized(ContextualPropertyCollectionTable nonSync) {
			ContextualPropertyCollectionTable sync = new ContextualPropertyCollectionTable();
			sync.innerHash = Hashtable.Synchronized(nonSync.innerHash);
			return sync;
		}
		#endregion

		#region Implementation of IEnumerable
		IEnumerator IEnumerable.GetEnumerator() {
			return new ContextualPropertyCollectionTableEnumerator(this);
		}
		#endregion
		
		#region Implementation of ICollection
		public bool		IsSynchronized	{ get { return InnerHash.IsSynchronized; } }
		public int		Count			{ get { return InnerHash.Count; } }
		public object	SyncRoot		{ get { return this; } }
		#endregion
	}
	
	#region ContextualPropertyCollectionTableEnumerator
	[Serializable]
	internal class ContextualPropertyCollectionTableEnumerator : IEnumerator {
		private readonly IEnumerator innerEnumerator;
		
		internal ContextualPropertyCollectionTableEnumerator(ContextualPropertyCollectionTable enumerable) {
			innerEnumerator = enumerable.InnerHash.GetEnumerator();
		}
		
		#region Implementation of IEnumerator
		public void Reset() {
			innerEnumerator.Reset();
		}
		
		public bool MoveNext() {
			return innerEnumerator.MoveNext();
		}
		
		public object Current { get { return ((DictionaryEntry) innerEnumerator.Current).Value; } }
		#endregion
	}
	#endregion
}
