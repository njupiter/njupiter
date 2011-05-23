using System;
using System.Globalization;

namespace nJupiter.DataAccess.Users.Sql {
	[Serializable]
	public class IntProperty : PropertyBase<int> {
		private const string Format = "D10";

		[NonSerialized]
		private const int Default = 0;

		public IntProperty(string propertyName, Context context) : base(propertyName, context) { }

		public override string ToSerializedString() {
			//convert to a non negative value for being able to sort lexicographically
			long longPositiveValue = (long)this.Value - int.MinValue;
			return longPositiveValue.ToString(Format, NumberFormatInfo.InvariantInfo);
		}

		public override int DeserializePropertyValue(string value) {
			return value == null ? Default : (int)(long.Parse(value, NumberFormatInfo.InvariantInfo) + int.MinValue);
		}

	}
}