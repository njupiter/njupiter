using System;

namespace nJupiter.DataAccess.Users {
	[Serializable]
	public class StringProperty : PropertyBase<string> {
		[NonSerialized]
		private readonly string defaultValue = string.Empty;

		public StringProperty(string propertyName, Context context) : base(propertyName, context) { }
		public StringProperty(string propertyName, string value, Context context) : base(propertyName, context) {
			this.Value = value;
		}

		public override string ToSerializedString() {
			return this.Value;
		}

		public override string DeserializePropertyValue(string value) {
			return value ?? defaultValue;
		}

		public override bool IsEmpty() {
			return string.IsNullOrEmpty(this.Value);
		}
	}
}