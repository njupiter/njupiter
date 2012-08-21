using System;

namespace nJupiter.DataAccess.Users.Sql {
	internal class PropertyFactory {
		internal static IProperty Create(string propertyName, string serializedPropertyValue, Type propertyType, IContext context) {
			context = context ?? Context.DefaultContext;
			object[] constructorArgs = { propertyName, context };
			var property = (IProperty)Activator.CreateInstance(propertyType, constructorArgs);
			property.Value = property.DeserializePropertyValue(serializedPropertyValue);
			property.IsDirty = false;
			return property;
		}

	}
}