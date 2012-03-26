using System;
using System.Runtime.Serialization;

namespace nJupiter.DataAccess.Users.Sql.Serialization {
	class DeserializationBinder :  SerializationBinder {
		public DeserializationBinder(SurrogateSelector surrogateSelector) {}

		public override Type BindToType(string assemblyName, string typeName) {
			if(typeName.EndsWith(".DataAccess.Users.BinaryProperty", StringComparison.InvariantCultureIgnoreCase)) {
				return typeof(BinaryProperty);
			}
			if(typeName.EndsWith(".DataAccess.Users.XmlSerializedProperty+ValueWrapper", StringComparison.InvariantCultureIgnoreCase)) {
				return typeof(XmlSerializedProperty.ValueWrapper);
			}
			return Type.GetType(String.Format("{0}, {1}", typeName, assemblyName));
		}
	}
}
