using System;

namespace nJupiter.DataAccess.Users.Sql {
	internal class PropertyFactory {
		internal static IProperty Create<T>(string propertyName, string serializedPropertyValue, Context context) where T : IProperty {
			
			IProperty property = null;
			if(typeof(T).Equals(typeof(StringProperty))) {
				property = new StringProperty(propertyName, context);
			} else if(typeof(T).Equals(typeof(BoolProperty))) {
				property = new BoolProperty(propertyName, context);
			} else if(typeof(T).Equals(typeof(IntProperty))) {
				property = new IntProperty(propertyName, context);
			} else if(typeof(T).Equals(typeof(DateTimeProperty))) {
				property = new DateTimeProperty(propertyName, context);
			} else if(typeof(T).Equals(typeof(BinaryProperty))) {
				property = new BinaryProperty(propertyName, context);
			} else if(typeof(T).Equals(typeof(XmlSerializedProperty))) {
				property = new XmlSerializedProperty(propertyName, context);
			}
			property.Value = property.DeserializePropertyValue(serializedPropertyValue);
			return property;
		}



		internal static IProperty Create(string propertyName, string serializedPropertyValue, Type propertyType, Context context) {
			object[] constructorArgs = { propertyName, context };
			IProperty property = (IProperty)Activator.CreateInstance(propertyType, constructorArgs);
			property.Value = property.DeserializePropertyValue(serializedPropertyValue);
			property.IsDirty = false;
			return property;
		}

	}
}