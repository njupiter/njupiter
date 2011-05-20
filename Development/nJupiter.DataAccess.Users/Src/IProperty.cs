using System;

namespace nJupiter.DataAccess.Users {
	public interface IProperty {
		bool IsEmpty();
		string ToSerializedString();
		Type GetPropertyValueType();
		string Name { get; }
		Context Context { get; }
		bool IsDirty { get; set; }
		bool SerializationPreservesOrder { get; }
		object DeserializePropertyValue(string value);
		object DefaultValue { get; }
		object Value { get; set; }
	}
}