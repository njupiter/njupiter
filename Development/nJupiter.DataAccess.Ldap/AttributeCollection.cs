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

using System;
using System.Collections;
using System.Collections.Generic;

namespace nJupiter.DataAccess.Ldap {
	internal class AttributeCollection : IAttributeCollection {
		private readonly Dictionary<string, IAttributeValueCollection> attributeCollection;

		internal AttributeCollection(IDictionary propertyCollection) {
			attributeCollection = new Dictionary<string, IAttributeValueCollection>(StringComparer.InvariantCultureIgnoreCase);
			try {
				InitializeCollection(propertyCollection);
			} catch(InvalidCastException ex) {
				throw new ArgumentException(
					"IDictionary.Keys must be of type string and IDictionary.Values must be of type IEnumerable",
					"propertyCollection",
					ex);
			}
		}

		private void InitializeCollection(IDictionary propertyCollection) {
			foreach(string key in propertyCollection.Keys) {
				var values = (IEnumerable)propertyCollection[key];
				var attributeValueCollection = new AttributeValueCollection(values);
				attributeCollection.Add(key, attributeValueCollection);
			}
		}

		public IAttributeValueCollection this[string attributeName] { get { return attributeCollection[attributeName]; } }

		public int Count { get { return attributeCollection.Count; } }

		public bool ContainsKey(string key) {
			return attributeCollection.ContainsKey(key);
		}

		public bool ContainsValue(IAttributeValueCollection value) {
			return attributeCollection.ContainsValue(value);
		}

		public IEnumerable<string> Keys { get { return attributeCollection.Keys; } }

		public IEnumerable<IAttributeValueCollection> Values { get { return attributeCollection.Values; } }

		IEnumerator IEnumerable.GetEnumerator() {
			return attributeCollection.GetEnumerator();
		}

		public override int GetHashCode() {
			return attributeCollection.GetHashCode();
		}

		public IEnumerator<IAttributeValueCollection> GetEnumerator() {
			return attributeCollection.Values.GetEnumerator();
		}

		public override bool Equals(object obj) {
			var collection = obj as AttributeCollection;
			if(collection == null) {
				return false;
			}
			return attributeCollection.Equals(collection.attributeCollection);
		}
	}
}