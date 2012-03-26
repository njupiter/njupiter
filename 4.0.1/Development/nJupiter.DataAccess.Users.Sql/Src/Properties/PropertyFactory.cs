using System;

namespace nJupiter.DataAccess.Users.Sql {
	internal class PropertyFactory {
		internal static IProperty Create<T>(string propertyName, string serializedPropertyValue, IContext context) where T : IProperty {
			
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



		internal static IProperty Create(string propertyName, string serializedPropertyValue, Type propertyType, IContext context) {
			IProperty property;
			if(propertyType.Equals(typeof(string))) {
				property = new StringProperty(propertyName, context);
			} else if(propertyType.Equals(typeof(bool))) {
				property = new BoolProperty(propertyName, context);
			} else if(propertyType.Equals(typeof(int))) {
				property = new IntProperty(propertyName, context);
			} else if(propertyType.Equals(typeof(DateTime))) {
				property = new DateTimeProperty(propertyName, context);
			} else if(propertyType.IsSerializable) {
				property = new BinaryProperty(propertyName, context);
			} else {
				property = new XmlSerializedProperty(propertyName, context);
			}
			property.Value = property.DeserializePropertyValue(serializedPropertyValue);
			property.IsDirty = false;
			return property;
	
			/*
			object[] constructorArgs = { propertyName, context };
			IProperty property = (IProperty)Activator.CreateInstance(propertyType, constructorArgs);
			property.Value = property.DeserializePropertyValue(serializedPropertyValue);
			property.IsDirty = false;
			return property;*/
		}

	}
}