using System;
using System.Globalization;

namespace nJupiter.DataAccess.Users.Sql {
	[Serializable]
	public class IntProperty : PropertyBase<int>, ISqlProperty {
		private const string Format = "D10";

		public IntProperty(string propertyName, Context context) : base(propertyName, context) { }

		public override string ToSerializedString() {
			return ToLexicographicallyFormat();
		}

		private string ToLexicographicallyFormat() {
			long longPositiveValue = (long)this.Value - int.MinValue;
			return longPositiveValue.ToString(Format, NumberFormatInfo.InvariantInfo);
		}

		public override int DeserializePropertyValue(string value) {
			return string.IsNullOrEmpty(value) ? this.DefaultValue : (int)(long.Parse(value, NumberFormatInfo.InvariantInfo) + int.MinValue);
		}

		public bool SerializationPreservesOrder { get { return true; } }
	}
}