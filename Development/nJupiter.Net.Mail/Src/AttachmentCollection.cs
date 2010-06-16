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
using System.IO;
using System.Collections;

namespace nJupiter.Net.Mail {

	public sealed class AttachmentCollection : ICloneable, IList, IDisposable {

		#region Members
		private ArrayList innerArray;
		private bool disposed;
		#endregion

		#region Constructors
		public AttachmentCollection() {
			this.innerArray = new ArrayList();
		}
		#endregion

		#region Properties
		private ArrayList InnerArray { get { return this.innerArray; } }
		#endregion

		#region Public Methods
		public Attachment this[int index] { get { return (Attachment)InnerArray[index]; } set { InnerArray[index] = value; } }

		public void Add(Attachment attachment) {
			InnerArray.Add(attachment);
		}

		public void Add(FileInfo file) {
			Attachment attachment = new Attachment(file);
			InnerArray.Add(attachment);
		}

		public void Add(AttachmentCollection attachmentCollection) {
			InnerArray.AddRange(attachmentCollection);
		}

		public bool Contains(Attachment attachment) {
			return InnerArray.Contains(attachment);
		}

		public void Remove(Attachment attachment) {
			InnerArray.Remove(attachment);
		}

		public void RemoveAt(int index) {
			InnerArray.RemoveAt(index);
		}

		public void RemoveRange(int index, int count) {
			InnerArray.RemoveRange(index, count);
		}

		public void Insert(int index, Attachment value) {
			InnerArray.Insert(index, value);
		}

		public void Clear() {
			InnerArray.Clear();
		}

		public int IndexOf(Attachment attachment) {
			return InnerArray.IndexOf(attachment);
		}

		public int LastIndexOf(Attachment attachment) {
			return InnerArray.LastIndexOf(attachment);
		}

		public void CopyTo(Array array, int index) {
			InnerArray.CopyTo(array, index);
		}

		public void CopyTo(Attachment[] array, int index) {
			InnerArray.CopyTo(array, index);
		}
		#endregion

		#region Implementation of ICloneable
		public AttachmentCollection Clone() {
			AttachmentCollection collection = new AttachmentCollection();
			collection.innerArray = (ArrayList)this.InnerArray.Clone();
			return collection;
		}

		object ICloneable.Clone() {
			return Clone();
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

		#region Implementation of IList
		public bool IsReadOnly { get { return InnerArray.IsReadOnly; } }

		object IList.this[int index] {
			get {
				return InnerArray[index];
			}
			set {
				if(!(value is Attachment))
					throw new InvalidCastException();
				InnerArray[index] = value;
			}
		}

		void IList.Insert(int index, object value) {
			if(!(value is Attachment))
				throw new InvalidCastException();
			InnerArray.Insert(index, value);
		}

		void IList.Remove(object value) {
			if(!(value is Attachment))
				throw new InvalidCastException();
			InnerArray.Remove(value);
		}

		bool IList.Contains(object value) {
			return InnerArray.Contains(value);
		}

		int IList.IndexOf(object value) {
			if(!(value is Attachment))
				throw new InvalidCastException();
			return InnerArray.IndexOf(value);
		}

		int IList.Add(object value) {
			if(!(value is Attachment))
				throw new InvalidCastException();
			return InnerArray.Add(value);
		}

		public bool IsFixedSize {
			get {
				return InnerArray.IsFixedSize;
			}
		}
		#endregion

		#region IEnumerable Members

		public IEnumerator GetEnumerator() {
			return InnerArray.GetEnumerator();
		}

		#endregion

		#region IDisposable Members
		public void Dispose() {
			if(!this.disposed) {
				foreach(Attachment attachment in this) {
					if(attachment != null)
						attachment.Dispose();
				}
				this.Clear();
				this.disposed = true;
			}
		}
		#endregion
	}
}
