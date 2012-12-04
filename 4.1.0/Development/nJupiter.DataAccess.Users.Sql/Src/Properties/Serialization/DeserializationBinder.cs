using System;
using System.Runtime.Serialization;

namespace nJupiter.DataAccess.Users.Sql.Serialization {
	class DeserializationBinder :  SerializationBinder {
		public DeserializationBinder(SurrogateSelector surrogateSelector) {}

		public override Type BindToType(string assemblyName, string typeName) {
			if(BinaryPropertyTypeFromOlderVersions(typeName) || BinaryPropertyFromDifferentVersion(typeName)) {
				return typeof(BinaryProperty);
			}
			if(XmlSerializedPropertyFromOldVersions(typeName) || XmlSerializedPropertyFromDifferentVersion(typeName)) {
				return typeof(XmlSerializedProperty.ValueWrapper);
			}
			return Type.GetType(String.Format("{0}, {1}", typeName, assemblyName));
		}

		private static bool BinaryPropertyTypeFromOlderVersions(string typeName) {
			return typeName.EndsWith(".DataAccess.Users.BinaryProperty", StringComparison.InvariantCultureIgnoreCase);
		}

		private static bool BinaryPropertyFromDifferentVersion(string typeName) {
			return string.Equals(typeName, "nJupiter.DataAccess.Users.Sql.BinaryProperty", StringComparison.InvariantCultureIgnoreCase);
		}

		private static bool XmlSerializedPropertyFromOldVersions(string typeName) {
			return typeName.EndsWith(".DataAccess.Users.XmlSerializedProperty+ValueWrapper", StringComparison.InvariantCultureIgnoreCase);
		}

		private static bool XmlSerializedPropertyFromDifferentVersion(string typeName) {
			return string.Equals(typeName, "nJupiter.DataAccess.Users.Sql.XmlSerializedProperty+ValueWrapper", StringComparison.InvariantCultureIgnoreCase);
		}

	}
}
