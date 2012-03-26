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

namespace nJupiter.Services.Forum {
	
	[Serializable]
	public abstract class GuidId : IComparable {	
		#region Variables
		private Guid	value;
		#endregion

		#region Constructors
		internal GuidId(Guid value) {
			this.value = value;
		}
		internal GuidId() : this(Guid.NewGuid()) {}
		#endregion

		#region Properties
		public Guid Value { get { return this.value; } }
		#endregion

		#region IComparable Members
		public int CompareTo(object obj) {
			GuidId guidId = obj as GuidId;
			if(guidId == null) {
				throw new ArgumentException("'obj' is not the same type as this instance.", "obj");
			}
			return this.value.CompareTo(guidId.value);
		}
		#endregion

		#region Operator Overrides
		public static bool operator ==(GuidId gid1, GuidId gid2) {
			return gid1 as object == null ? gid2 as object == null : gid1.Equals(gid2);
		}
		public static bool operator !=(GuidId gid1, GuidId gid2) {
			return !(gid1 == gid2);
		}
		public static bool operator <(GuidId gid1, GuidId gid2) {
			return (gid1 == null ? gid2 == null ? 0 : -1 : gid1.CompareTo(gid2)) < 0;
		}
		public static bool operator >(GuidId gid1, GuidId gid2) {
			return (gid1 == null ? gid2 == null ? 0 : -1 : gid1.CompareTo(gid2)) > 0;
		}
		#endregion

		#region Methods
		public sealed override bool Equals(object obj) {
			GuidId guidId = obj as GuidId;
			return guidId != null &&
				this.value.Equals(guidId.value);
		}
		public sealed override int GetHashCode() {
			return this.value.GetHashCode();
		}
		public sealed override string ToString() {
			const string	format	= "N";

			return this.value.ToString(format);
		}
		#endregion
	}

}