using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices;

namespace nJupiter.DataAccess.Ldap {
	public class AttributeValueCollection : IEnumerable {
		private readonly List<object> attributeValueCollection;

		internal AttributeValueCollection(ResultPropertyValueCollection resultPropertyValueCollection) {
			attributeValueCollection = new List<object>();
			foreach(var propertyValue in resultPropertyValueCollection) {
				attributeValueCollection.Add(propertyValue);
			}

		}

		public object this[int index] {
			get {
				return attributeValueCollection[index];
			}
		}

		public int Count {
			get {
				return attributeValueCollection.Count;
			}
		}

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
