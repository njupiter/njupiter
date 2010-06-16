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
using System.Text;
using System.Collections;

namespace nJupiter.Net.Mail {

	public sealed class MailAddressCollection : ICollection {

		#region Members
		private ArrayList innerArray;
		#endregion

		#region Constructors
		public MailAddressCollection() {
			this.innerArray = new ArrayList();
		}

		internal MailAddressCollection(MailAddress mailAddress)
			: this() {
			this.innerArray.Add(mailAddress);
		}
		#endregion

		#region Indexers
		public MailAddress this[int index] { get { return (MailAddress)InnerArray[index]; } }
		#endregion

		#region Properties
		internal ArrayList InnerArray { get { return this.innerArray; } }
		#endregion

		#region Public Methods
		public void Add(MailAddress mailAddress) {
			InnerArray.Add(mailAddress);
		}

		public void Remove(MailAddress mailAddress) {
			InnerArray.Remove(mailAddress);
		}

		public MailAddress[] ToArray() {
			return (MailAddress[])InnerArray.ToArray(typeof(MailAddress));
		}

		public bool Contains(MailAddress mailAddress) {
			return (InnerArray.Contains(mailAddress));
		}

		public void CopyTo(Array array, int index) {
			InnerArray.CopyTo(array, index);
		}

		public void CopyTo(MailAddress[] array, int index) {
			InnerArray.CopyTo(array, index);
		}

		public void Clear() {
			InnerArray.Clear();
		}

		public void Sort() {
			InnerArray.Sort();
		}

		public void Sort(IComparer comparer) {
			InnerArray.Sort(comparer);
		}

		public void Sort(int indexer, int count, IComparer comparer) {
			InnerArray.Sort(indexer, count, comparer);
		}

		public static MailAddressCollection Synchronized(MailAddressCollection nonSync) {
			if(nonSync == null) {
				throw new ArgumentNullException("nonSync");
			}
			MailAddressCollection sync = new MailAddressCollection();
			sync.innerArray = ArrayList.Synchronized(nonSync.innerArray);
			return sync;
		}

		public override string ToString() {
			StringBuilder sb = new StringBuilder();
			foreach(MailAddress m in this) {
				if(sb.Length > 0) {
					sb.Append(";");
				}
				sb.Append(m.ToString());
			}
			return sb.ToString();
		}
		#endregion

		#region Implementation of IEnumerable
		IEnumerator IEnumerable.GetEnumerator() {
			return this.innerArray.GetEnumerator();
		}
		#endregion

		#region Implementation of ICollection
		public bool IsSynchronized { get { return InnerArray.IsSynchronized; } }
		public int Count { get { return InnerArray.Count; } }
		public object SyncRoot { get { return this; } }
		#endregion

	}
}
