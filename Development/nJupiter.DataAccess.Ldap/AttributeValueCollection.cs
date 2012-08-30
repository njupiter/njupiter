#region Copyright & License
// 
// 	Copyright (c) 2005-2012 nJupiter
// 
// 	Permission is hereby granted, free of charge, to any person obtaining a copy
// 	of this software and associated documentation files (the "Software"), to deal
// 	in the Software without restriction, including without limitation the rights
// 	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// 	copies of the Software, and to permit persons to whom the Software is
// 	furnished to do so, subject to the following conditions:
// 
// 	The above copyright notice and this permission notice shall be included in
// 	all copies or substantial portions of the Software.
// 
// 	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// 	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// 	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// 	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// 	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// 	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// 	THE SOFTWARE.
// 
#endregion

using System.Collections;
using System.Collections.Generic;

namespace nJupiter.DataAccess.Ldap {
	internal class AttributeValueCollection : IAttributeValueCollection {
		private readonly List<object> attributeValueCollection;

		internal AttributeValueCollection(IEnumerable propertyValueCollection) {
			attributeValueCollection = new List<object>();
			foreach(var propertyValue in propertyValueCollection) {
				attributeValueCollection.Add(propertyValue);
			}
		}

		public object this[int index] { get { return attributeValueCollection[index]; } }

		public int Count { get { return attributeValueCollection.Count; } }

		public bool Contains(object item) {
			return attributeValueCollection.Contains(item);
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return attributeValueCollection.GetEnumerator();
		}

		public override int GetHashCode() {
			return attributeValueCollection.GetHashCode();
		}

		public override bool Equals(object obj) {
			var collection = obj as AttributeValueCollection;
			if(collection == null) {
				return false;
			}
			return attributeValueCollection.Equals(collection.attributeValueCollection);
		}
	}
}