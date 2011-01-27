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

namespace nJupiter.Net.Mail {

	public sealed class MailCollection : ICollection {

		#region Members
		private ArrayList innerArray;
		#endregion

		#region Constructors
		public MailCollection() {
			this.innerArray = new ArrayList();
		}
		#endregion

		#region Indexers
		public Mail this[int index] { get { return (Mail)InnerArray[index]; } }
		#endregion

		#region Properties
		internal ArrayList InnerArray { get { return this.innerArray; } }
		#endregion

		#region Public Methods
		public void Add(Mail mail) {
			InnerArray.Add(mail);
		}

		public void Remove(Mail mail) {
			InnerArray.Remove(mail);
		}

		public Mail[] ToArray() {
			return (Mail[])InnerArray.ToArray(typeof(Mail));
		}

		public bool Contains(Mail mail) {
			return (InnerArray.Contains(mail));
		}

		public void CopyTo(Array array, int index) {
			InnerArray.CopyTo(array, index);
		}

		public void CopyTo(Mail[] array, int index) {
			InnerArray.CopyTo(array, index);
		}

		public void Clear() {
			InnerArray.Clear();
		}

		public static MailCollection Synchronized(MailCollection nonSync) {
			if(nonSync == null) {
				throw new ArgumentNullException("nonSync");
			}
			MailCollection sync = new MailCollection();
			sync.innerArray = ArrayList.Synchronized(nonSync.innerArray);
			return sync;
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
