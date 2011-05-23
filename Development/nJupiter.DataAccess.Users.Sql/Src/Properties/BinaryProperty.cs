using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace nJupiter.DataAccess.Users.Sql {
	[Serializable]
	public class BinaryProperty : PropertyBase<object> {
		[NonSerialized]
		private const object Default = null;

		public BinaryProperty(string propertyName, Context context) : base(propertyName, context) { }

		public override bool SerializationPreservesOrder { get { return false; } }

		public override string ToSerializedString() {
			if(this.Value == Default)
				return null;
			using(MemoryStream stream = new MemoryStream()) {
				new BinaryFormatter().Serialize(stream, this.Value);
				return Convert.ToBase64String(stream.ToArray());
			}
		}

		public override object DeserializePropertyValue(string value) {
			if(value == null)
				return null;
			using(MemoryStream stream = new MemoryStream(Convert.FromBase64String(value))) {
				return new BinaryFormatter().Deserialize(stream);
			}
		}
	}
}