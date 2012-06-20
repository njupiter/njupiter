using System.Collections;
using System.Collections.Generic;

namespace nJupiter.DataAccess.Ldap {
	public class AttributeValueCollection : IAttributeValueCollection {

		private readonly List<object> attributeValueCollection;

		internal AttributeValueCollection(IEnumerable propertyValueCollection) {
			attributeValueCollection = new List<object>();
			foreach(var propertyValue in propertyValueCollection) {
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
