using System;

namespace nJupiter.DataAccess.Users {
	public interface IProperty: ILockable {
		bool IsEmpty();
		string Name { get; }
		Context Context { get; }
		bool IsDirty { get; set; }
		object DefaultValue { get; }
		object Value { get; set; }
		Type GetPropertyValueType();
		string ToSerializedString();
		bool SerializationPreservesOrder { get; }
		object DeserializePropertyValue(string value);

	}
}