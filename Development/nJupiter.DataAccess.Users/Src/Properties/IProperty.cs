using System;

namespace nJupiter.DataAccess.Users {
	public interface IProperty: ILockable {
		bool IsEmpty();
		string Name { get; }
		Context Context { get; }
		bool IsDirty { get; set; }
		object DefaultValue { get; }
		object Value { get; set; }
		Type Type { get; }
		string ToSerializedString();
		object DeserializePropertyValue(string value);

	}
}