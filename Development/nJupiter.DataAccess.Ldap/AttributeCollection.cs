using System;
using System.Collections;
using System.Collections.Generic;

namespace nJupiter.DataAccess.Ldap {
	public class AttributeCollection : IAttributeCollection {
		private readonly Dictionary<string, IAttributeValueCollection> attributeCollection;

		internal AttributeCollection(IDictionary propertyCollection) {
			attributeCollection = new Dictionary<string, IAttributeValueCollection>(StringComparer.InvariantCultureIgnoreCase);
			try {
				InitializeCollection(propertyCollection);
			}catch(InvalidCastException ex){
				throw new ArgumentException("IDictionary.Keys must be of type string and IDictionary.Values must be of type IEnumerable", "propertyCollection", ex);
			}
		}

		private void InitializeCollection(IDictionary propertyCollection) {
			foreach(string key in propertyCollection.Keys) {
				var values = (IEnumerable)propertyCollection[key];
				var attributeValueCollection = new AttributeValueCollection(values);
				attributeCollection.Add(key, attributeValueCollection);
			}
		}

		public IAttributeValueCollection this[string attributeName] {
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

		public bool ContainsValue(IAttributeValueCollection value) {
			return attributeCollection.ContainsValue(value);
		}

		public IEnumerable<string> Keys {
			get {
				return attributeCollection.Keys;
			}
		}

		public IEnumerable<IAttributeValueCollection> Values {
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
