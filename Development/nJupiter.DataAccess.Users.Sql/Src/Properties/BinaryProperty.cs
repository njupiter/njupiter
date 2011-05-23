using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace nJupiter.DataAccess.Users.Sql {
	[Serializable]
	public class BinaryProperty : PropertyBase<object>, ISqlProperty {

		public BinaryProperty(string name, Context context) : base(name, context) { }

		public bool SerializationPreservesOrder { get { return false; } }

		public override string ToSerializedString() {
			if(this.IsEmpty())
				return null;
			using(MemoryStream stream = new MemoryStream()) {
				new BinaryFormatter().Serialize(stream, this.Value);
				this.IsDirty = false;
				return Convert.ToBase64String(stream.ToArray());
			}
		}

		public override object DeserializePropertyValue(string value) {
			if(value == null)
				return this.DefaultValue;
			using(MemoryStream stream = new MemoryStream(Convert.FromBase64String(value))) {
				return new BinaryFormatter().Deserialize(stream);
			}
		}
	}
}