using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using nJupiter.DataAccess.Users.Sql.Serialization;

namespace nJupiter.DataAccess.Users.Sql {
	[Serializable]
	public class BinaryProperty : PropertyBase<object>, ISqlProperty {

		public BinaryProperty(string name, IContext context) : base(name, context) { }

		public bool SerializationPreservesOrder { get { return false; } }
		protected override bool SetDirtyOnTouch { get { return true; } }

		public override string ToSerializedString() {
			if(this.IsEmpty())
				return null;
			using(MemoryStream stream = new MemoryStream()) {
				new BinaryFormatter().Serialize(stream, this.ValueUntouched);
				return Convert.ToBase64String(stream.ToArray());
			}
		}

		public override object DeserializePropertyValue(string value) {
			if(value == null)
				return this.DefaultValue;
			using(MemoryStream stream = new MemoryStream(Convert.FromBase64String(value))) {
				BinaryFormatter formatter = new BinaryFormatter();
				var surrogateSelector = new SurrogateSelector();
				formatter.SurrogateSelector = surrogateSelector;
				var deserializationBinder = new DeserializationBinder(surrogateSelector);
				formatter.Binder = deserializationBinder;
				return formatter.Deserialize(stream);
			}
		}
	}
}