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
	public sealed class ContextCollection : ICollection {

		#region Members
		private Hashtable innerHash;
		#endregion

		#region Constructors
		internal ContextCollection() {
			this.innerHash = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
		}
		#endregion

		#region Indexers
		public Context this[string contextName] { get { return (Context)InnerHash[contextName]; } }
		#endregion

		#region Properties
		internal Hashtable InnerHash { get { return this.innerHash; } }
		#endregion

		#region Methods
		internal void Add(Context context) {
			if(context == null)
				throw new ArgumentNullException("context");
			InnerHash.Add(context.Name, context);
		}

		internal void Remove(Context context) {
			if(context == null)
				throw new ArgumentNullException("context");
			InnerHash.Remove(context.Name);
		}

		public Context[] ToArray() {
			return (Context[])(new ArrayList(InnerHash.Values).ToArray(typeof(Context)));
		}

		public bool Contains(Context context) {
			return (InnerHash.ContainsValue(context));
		}

		public bool Contains(string contextName) {
			return (InnerHash.Contains(contextName));
		}

		public void CopyTo(Array array, int index) {
			InnerHash.Values.CopyTo(array, index);
		}

		public void CopyTo(Context[] array, int index) {
			InnerHash.Values.CopyTo(array, index);
		}

		public static ContextCollection Synchronized(ContextCollection nonSync) {
			if(nonSync == null) {
				throw new ArgumentNullException("nonSync");
			}
			ContextCollection sync = new ContextCollection();
			sync.innerHash = Hashtable.Synchronized(nonSync.innerHash);
			return sync;
		}
		#endregion

		#region Implementation of IEnumerable
		IEnumerator IEnumerable.GetEnumerator() {
			return new ContextCollectionEnumerator(this);
		}

		public ContextCollectionEnumerator GetEnumerator() {
			return new ContextCollectionEnumerator(this);
		}
		#endregion

		#region Implementation of ICollection
		public bool IsSynchronized { get { return InnerHash.IsSynchronized; } }
		public int Count { get { return InnerHash.Count; } }
		public object SyncRoot { get { return this; } }
		#endregion
	}

	#region ContextCollectionEnumerator
	[Serializable]
	public class ContextCollectionEnumerator : IEnumerator {
		private readonly IEnumerator innerEnumerator;

		public ContextCollectionEnumerator(ContextCollection enumerable) {
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
		public Context Current { get { return (Context)((DictionaryEntry)innerEnumerator.Current).Value; } }
		#endregion
	}
	#endregion
}
