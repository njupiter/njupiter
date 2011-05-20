using System;

namespace nJupiter.DataAccess.Users {
	internal class PropertyFactory {
		internal static IProperty Create(string propertyName, string serializedPropertyValue, Type propertyType, Context context) {
			object[] constructorArgs = { propertyName, context };
			IProperty property = (IProperty)Activator.CreateInstance(propertyType, constructorArgs);
			property.Value = property.DeserializePropertyValue(serializedPropertyValue);
			property.IsDirty = false;
			return property;
		}

	}
}