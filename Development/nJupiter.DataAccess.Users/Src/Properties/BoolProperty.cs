using System;

namespace nJupiter.DataAccess.Users {
	[Serializable]
	public class BoolProperty : PropertyBase<bool> {
		[NonSerialized]
		private const bool Default = false;

		public BoolProperty(string propertyName, Context context) : base(propertyName, context) { }

		public override string ToSerializedString() {
			return this.Value.ToString();
		}

		public override bool DeserializePropertyValue(string value) {
			return value == null ? Default : bool.Parse(value);
		}

	}
}