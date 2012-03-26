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

namespace nJupiter.Web {
	
	public sealed class MimeTypeCollection : ICollection {
		#region Members
		private readonly Hashtable	innerHash;
		#endregion
		
		#region Constructors
		internal MimeTypeCollection() {
			this.innerHash  = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
		}
		#endregion

		#region Indexers
		public MimeType this[string mimeType] {
			get{
				return InnerHash[mimeType] as MimeType;
			}
		}
		#endregion

		#region Properties
		internal	Hashtable	InnerHash	{ get { return this.innerHash; } }
		#endregion

		#region Public Methods
		public void Add(MimeType mimeType) {
			if(mimeType == null) {
				throw new ArgumentNullException("mimeType");
			}
			this.innerHash[mimeType.ContentType] = mimeType;
		}

		public void Remove(MimeType mimeType) { 
			this.innerHash.Remove(mimeType);
		}

		public bool ContainsType(string mimeType) {
			MimeType mime = new MimeType(mimeType);
			foreach(MimeType m in this) {
				if(m.EqualsType(mime))
					return true;
			}
			return false;
		}

		public bool ContainsExactType(string mimeType) {
			MimeType mime = new MimeType(mimeType);
			foreach(MimeType m in this) {
				if(m.EqualsExactType(mime))
					return true;
			}
			return false;
		}

		public bool ContainsKey(string mimeTypeKey) {
			return InnerHash.ContainsKey(mimeTypeKey);
		}

		public bool Contains(MimeType mimeType) {
			return InnerHash.Contains(mimeType);
		}

		public MimeType GetHighestQuality(string mimeType) {
			return GetHighestQuality(mimeType, false);
		}

		public MimeType GetHighestQuality(string mimeType, bool exactType) {
			MimeType mime = new MimeType(mimeType);
			MimeType result = null;
			foreach(MimeType m in this) {
				if(((exactType && m.EqualsExactType(mime)) || m.EqualsType(mime)) && (result == null || result.Quality < m.Quality))
					result =  m;
			}
			return result;
		}
		void ICollection.CopyTo(Array array, int index) {
			InnerHash.Values.CopyTo(array, index);
		}
		public void CopyTo(MimeType[] array, int index) {
			InnerHash.Values.CopyTo(array, index);
		}
		#endregion

		#region Implementation of IEnumerable
		IEnumerator IEnumerable.GetEnumerator() {
			return new MimeTypeCollectionEnumerator(this);
		}
		#endregion
		
		#region Implementation of ICollection
		public bool		IsSynchronized	{ get { return InnerHash.IsSynchronized; } }
		public int		Count			{ get { return InnerHash.Count; } }
		public object	SyncRoot		{ get { return this; } }
		#endregion
	}
	
	#region MimeTypeCollectionEnumerator
	[Serializable]
	public class MimeTypeCollectionEnumerator : IEnumerator {
		private readonly IEnumerator innerEnumerator;
		
		internal MimeTypeCollectionEnumerator (MimeTypeCollection enumerable) {
			innerEnumerator = enumerable.InnerHash.GetEnumerator();
		}
	
		#region Implementation of IEnumerator
		public void Reset() {
			innerEnumerator.Reset();
		}
		
		public bool MoveNext() {
			return innerEnumerator.MoveNext();
		}
		
		object IEnumerator.Current { get { return ((DictionaryEntry) innerEnumerator.Current).Value; } }
		public MimeType Current { get { return (MimeType)((DictionaryEntry) innerEnumerator.Current).Value; } }
		#endregion
	}
	#endregion
}
