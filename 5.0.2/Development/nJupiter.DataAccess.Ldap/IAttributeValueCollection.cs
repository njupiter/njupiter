using System.Collections;

namespace nJupiter.DataAccess.Ldap {
	public interface IAttributeValueCollection : IEnumerable  {
		object this[int index] { get; }
		int Count { get; }
		bool Contains(object item);
		int GetHashCode();
		bool Equals(object obj);
	}
}