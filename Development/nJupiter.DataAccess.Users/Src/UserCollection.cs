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
	public sealed class UserCollection : IList {

		#region Members
		private ArrayList innerArray;
		#endregion

		#region Constructors
		public UserCollection() {
			this.innerArray = new ArrayList();
		}
		#endregion

		#region Indexers
		public User this[string userId] {
			get {
				foreach(User u in this) {
					if(u.Id.Equals(userId)) {
						return u;
					}
				}
				throw new ArgumentOutOfRangeException("userId");
			}
		}

		public User this[int index] { get { return (User)this.innerArray[index]; } set { this.innerArray[index] = value; } }
		#endregion

		#region Properties
		private ArrayList InnerArray { get { return this.innerArray; } }
		#endregion

		#region Public Methods
		public int Add(User user) {
			if(user == null)
				throw new ArgumentNullException("user");
			return InnerArray.Add(user);
		}

		public void AddRange(UserCollection users) {
			if(users == null)
				throw new ArgumentNullException("users");
			InnerArray.AddRange(users);
		}

		public void Remove(User user) {
			if(user == null)
				throw new ArgumentNullException("user");
			InnerArray.Remove(user);
		}

		public void RemoveRange(int index, int count) {
			InnerArray.RemoveRange(index, count);
		}

		public void RemoveRange(UserCollection users) {
			if(users == null)
				throw new ArgumentNullException("users");
			foreach(User user in users) {
				InnerArray.Remove(user);
			}
		}

		public User[] ToArray() {
			return (User[])InnerArray.ToArray(typeof(User));
		}

		public bool Contains(User user) {
			return (InnerArray.Contains(user));
		}

		public bool Contains(string userId) {
			foreach(User u in this) {
				if(u.Id.Equals(userId))
					return true;
			}
			return false;
		}

		public void Sort() {
			InnerArray.Sort();
		}

		public void Sort(IComparer comparer) {
			InnerArray.Sort(comparer);
		}

		public void Sort(int index, int count, IComparer comparer) {
			InnerArray.Sort(index, count, comparer);
		}

		public void CopyTo(Array array, int index) {
			InnerArray.CopyTo(array, index);
		}

		public void CopyTo(User[] array, int index) {
			InnerArray.CopyTo(array, index);
		}

		public void Clear() {
			InnerArray.Clear();
		}

		public static UserCollection Synchronized(UserCollection nonSync) {
			if(nonSync == null) {
				throw new ArgumentNullException("nonSync");
			}
			UserCollection sync = new UserCollection();
			sync.innerArray = ArrayList.Synchronized(nonSync.innerArray);
			return sync;
		}
		#endregion

		#region IList Members
		object System.Collections.IList.this[int index] {
			get {
				return InnerArray[index];
			}
			set {
				if(value is User)
					InnerArray[index] = value;
				else
					throw new InvalidCastException();
			}
		}

		public void RemoveAt(int index) {
			InnerArray.RemoveAt(index);
		}

		public void Insert(int index, User user) {
			InnerArray.Insert(index, user);
		}

		void System.Collections.IList.Insert(int index, object value) {
			if(value is User)
				InnerArray.Insert(index, value);
			else
				throw new InvalidCastException();
		}

		void System.Collections.IList.Remove(object value) {
			InnerArray.Remove(value);
		}

		bool System.Collections.IList.Contains(object value) {
			return InnerArray.Contains(value);
		}

		public int IndexOf(User user) {
			return InnerArray.IndexOf(user);
		}

		int System.Collections.IList.IndexOf(object value) {
			return InnerArray.IndexOf(value);
		}

		int System.Collections.IList.Add(object value) {
			if(value is User)
				return InnerArray.Add(value);
			throw new InvalidCastException();
		}

		public bool IsReadOnly {
			get {
				return InnerArray.IsReadOnly;
			}
		}

		public bool IsFixedSize {
			get {
				return InnerArray.IsFixedSize;
			}
		}
		#endregion

		#region Implementation of IEnumerable
		public IEnumerator GetEnumerator() {
			return InnerArray.GetEnumerator();
		}
		#endregion

		#region Implementation of ICollection
		public bool IsSynchronized { get { return InnerArray.IsSynchronized; } }
		public int Count { get { return InnerArray.Count; } }
		public object SyncRoot { get { return this; } }
		#endregion
	}


}