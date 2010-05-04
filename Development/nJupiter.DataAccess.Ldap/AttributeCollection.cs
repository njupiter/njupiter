using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices;

namespace nJupiter.DataAccess.Ldap {
	public class AttributeCollection : IEnumerable {
		private readonly Dictionary<string, AttributeValueCollection> attributeCollection;

		internal AttributeCollection(ResultPropertyCollection resultPropertyCollection) {
			attributeCollection = new Dictionary<string, AttributeValueCollection>(StringComparer.InvariantCultureIgnoreCase);
			foreach(string key in resultPropertyCollection.PropertyNames) {
				ResultPropertyValueCollection values =  resultPropertyCollection[key];
				AttributeValueCollection attributeValueCollection = new AttributeValueCollection(values);
				attributeCollection.Add(key, attributeValueCollection);
			}
		}

		public AttributeValueCollection this[string attributeName] {
			get {
				return attributeCollection[attributeName];
			}
		}

		public int Count {
			get {
				return attributeCollection.Count;
			}
		}
	
		public bool ContainsKey(string key) {
			return attributeCollection.ContainsKey(key);
		}

		public bool ContainsValue(AttributeValueCollection value) {
			return attributeCollection.ContainsValue(value);
		}

		public IEnumerable<string> Keys {
			get {
				return attributeCollection.Keys;
			}
		}

		public IEnumerable<AttributeValueCollection> Values {
			get {
				return attributeCollection.Values;
			}
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return attributeCollection.GetEnumerator();
		}

		public override int GetHashCode() {
			return attributeCollection.GetHashCode();
		}

		public override bool Equals(object obj) {
			AttributeCollection collection = obj as AttributeCollection;
			if(collection == null) {
				return false;
			}
			return this.attributeCollection.Equals(collection.attributeCollection);
		}

	}
}
