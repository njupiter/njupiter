using System.Collections.Generic;

namespace nJupiter.DataAccess.Ldap {
	public interface IAttributeCollection : IEnumerable<IAttributeValueCollection>  {
		IAttributeValueCollection this[string attributeName] { get; }
		int Count { get; }
		IEnumerable<string> Keys { get; }
		IEnumerable<IAttributeValueCollection> Values { get; }
		bool ContainsKey(string key);
		bool ContainsValue(IAttributeValueCollection value);
		int GetHashCode();
		bool Equals(object obj);
	}
}