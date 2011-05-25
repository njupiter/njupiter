using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Xml.Serialization;

namespace nJupiter.DataAccess.Users.Sql {
	[Serializable]
	public class XmlSerializedProperty : PropertyBase<object>, ISqlProperty {

		public XmlSerializedProperty(string propertyName, Context context) : base(propertyName, context) { }
		
		protected override bool SetDirtyOnTouch { get { return true; } }

		public override string ToSerializedString() {
			if(this.IsEmpty())
				return null;
			Type type = this.Value.GetType();
			XmlSerializer serializer = new XmlSerializer(type);
			StringWriter stringwriter = new StringWriter(CultureInfo.InvariantCulture);
			XmlTextWriter xmlwriter = new XmlTextWriter(stringwriter);
			serializer.Serialize(xmlwriter, this.ValueUntouched);
			string xmlSerializedData = stringwriter.ToString();
			ValueWrapper valueWrapper = new ValueWrapper(type, xmlSerializedData);
			using(MemoryStream stream = new MemoryStream()) {
				new BinaryFormatter().Serialize(stream, valueWrapper);
				return Convert.ToBase64String(stream.ToArray());
			}
		}

		public override object DeserializePropertyValue(string value) {
			if(string.IsNullOrEmpty(value)) {
				return this.DefaultValue;	
			}
			ValueWrapper valueWrapper;
			using(MemoryStream stream = new MemoryStream(Convert.FromBase64String(value))) {
				valueWrapper = (new BinaryFormatter()).Deserialize(stream) as ValueWrapper;
			}
			if(valueWrapper == null || valueWrapper.Type == null || valueWrapper.Value == null)
				return this.DefaultValue;

			XmlSerializer serializer = new XmlSerializer(valueWrapper.Type);
			StringReader stringReader = new StringReader(valueWrapper.Value);
			XmlTextReader xmlReader = new XmlTextReader(stringReader);
			return serializer.Deserialize(xmlReader);
		}

		[Serializable]
		private sealed class ValueWrapper {
			private readonly Type type;
			private readonly string value;
			public ValueWrapper(Type type, string value) {
				this.type = type;
				this.value = value;
			}
			public Type Type { get { return this.type; } }
			public string Value { get { return this.value; } }
		}

		public bool SerializationPreservesOrder { get { return false; } }
	}
}