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
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace nJupiter.Util.Collections {

	[Serializable]
	public class ReadOnlyHashtable : Hashtable, ISerializable	{

		#region Members
		private Hashtable	innerHash;
		#endregion
		
		#region Constructors
		protected ReadOnlyHashtable(SerializationInfo info, StreamingContext context) : base(info, context){
			if(info == null)
				throw new ArgumentNullException("info");
			this.innerHash = (Hashtable)info.GetValue("innerHash", typeof(Hashtable));
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.SerializationFormatter)]	
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context) {
			info.AddValue("innerHash", this.innerHash);
			base.GetObjectData(info, context);
		}

		private ReadOnlyHashtable(Hashtable hashtable) {
			this.innerHash = hashtable;
		}
		#endregion

		#region Indexers
		public override object this[object key] {
			get {
				return base[key];
			}
			set {
				throw new NotSupportedException("Collection is read-only.");
			}
		}
		#endregion

		#region Properties
		public override bool IsFixedSize {
			get {
				return true;
			}
		}

		public override bool IsReadOnly {
			get {
				return true;
			}
		}

		public override bool IsSynchronized {
			get {
				return this.innerHash.IsSynchronized;
			}
		}
				
		public override ICollection Keys {
			get {
				return this.innerHash.Keys;
			}
		}

		public override ICollection Values {
			get {
				return this.innerHash.Values;
			}
		}

		public override object SyncRoot {
			get {
				return this;
			}
		}

		public override int Count {
			get {
				return this.innerHash.Count;
			}
		}
		#endregion

		#region Simple Factory Method
		public static Hashtable ReadOnly(Hashtable hashtable){
			return new ReadOnlyHashtable(hashtable);
		}
		#endregion

		#region Methods
		public override void Clear() {
			this.innerHash.Clear();
		}

		public override object Clone() {
			ReadOnlyHashtable hashtable = new ReadOnlyHashtable(this.innerHash);
			hashtable.innerHash = (Hashtable)this.innerHash.Clone();
			return hashtable;
		}

		public override bool Contains(object key) {
			return this.innerHash.Contains(key);
		}

		public override bool ContainsKey(object key) {
			return this.innerHash.ContainsKey(key);
		}

		public override bool ContainsValue(object value) {
			return this.innerHash.ContainsValue (value);
		}

		public override void CopyTo(Array array, int index) {
			this.innerHash.CopyTo(array, index);
		}

		public override bool Equals(object obj) {
			return this.innerHash.Equals (obj);
		}

		public override IDictionaryEnumerator GetEnumerator() {
			return this.innerHash.GetEnumerator ();
		}

		public override int GetHashCode() {
			return this.innerHash.GetHashCode ();
		}

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter=true)]	
		public override void GetObjectData(SerializationInfo info, StreamingContext context) {
		    this.innerHash.GetObjectData(info, context);
		}

		public override void OnDeserialization(object sender) {
			this.innerHash.OnDeserialization(sender);
		}

		public override void Remove(object key) {
			 throw new NotSupportedException("Collection is read-only.");
		}

		public override void Add(object key, object value) {
			throw new NotSupportedException("Collection is read-only.");
		}

		public override string ToString() {
			return this.innerHash.ToString ();
		}
		#endregion
	}
}
