using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Xml.Serialization;

using nJupiter.DataAccess.Users.Sql.Serialization;

namespace nJupiter.DataAccess.Users.Sql {
	[Serializable]
	public class XmlSerializedProperty : PropertyBase<object>, ISqlProperty {

		public XmlSerializedProperty(string propertyName, IContext context) : base(propertyName, context) { }
		
		protected override bool SetDirtyOnTouch { get { return true; } }

		public override string ToSerializedString() {
			if(this.IsEmpty())
				return null;
			var type = this.Value.GetType();
			var serializer = new XmlSerializer(type);
			var stringwriter = new StringWriter(CultureInfo.InvariantCulture);
			var xmlwriter = new XmlTextWriter(stringwriter);
			serializer.Serialize(xmlwriter, this.ValueUntouched);
			var xmlSerializedData = stringwriter.ToString();
			var valueWrapper = new ValueWrapper(type, xmlSerializedData);
			using(var stream = new MemoryStream()) {
				new BinaryFormatter().Serialize(stream, valueWrapper);
				return Convert.ToBase64String(stream.ToArray());
			}
		}

		public override object DeserializePropertyValue(string value) {
			if(string.IsNullOrEmpty(value)) {
				return this.DefaultValue;	
			}
			ValueWrapper valueWrapper;
			using(var stream = new MemoryStream(Convert.FromBase64String(value))) {
				var formatter = new BinaryFormatter();
				var surrogateSelector = new SurrogateSelector();
				formatter.SurrogateSelector = surrogateSelector;
				var deserializationBinder = new DeserializationBinder(surrogateSelector);
				formatter.Binder = deserializationBinder;
				valueWrapper = formatter.Deserialize(stream) as ValueWrapper;
			}
			if(valueWrapper == null || valueWrapper.Type == null || valueWrapper.Value == null)
				return this.DefaultValue;

			var serializer = new XmlSerializer(valueWrapper.Type);
			var stringReader = new StringReader(valueWrapper.Value);
			var xmlReader = new XmlTextReader(stringReader);
			return serializer.Deserialize(xmlReader);
		}

		[Serializable]
		internal sealed class ValueWrapper {
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