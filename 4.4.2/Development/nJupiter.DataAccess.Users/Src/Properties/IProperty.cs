using System;

namespace nJupiter.DataAccess.Users {
	public interface IProperty: ILockable<IProperty> {
		bool IsEmpty();
		string Name { get; }
		IContext Context { get; }
		bool IsDirty { get; set; }
		object DefaultValue { get; }
		object Value { get; set; }
		Type Type { get; }
		string ToSerializedString();
		object DeserializePropertyValue(string value);

	}
}