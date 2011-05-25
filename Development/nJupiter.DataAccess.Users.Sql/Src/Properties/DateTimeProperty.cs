using System;
using System.Globalization;

namespace nJupiter.DataAccess.Users.Sql {
	[Serializable]
	public class DateTimeProperty : PropertyBase<DateTime>, ISqlProperty {
		private const string Format = "D19";

		public DateTimeProperty(string propertyName, IContext context) : base(propertyName, context) { }

		public override string ToSerializedString() {
			return this.Value.Ticks.ToString(Format, NumberFormatInfo.InvariantInfo);
		}

		public override DateTime DeserializePropertyValue(string value) {
			return string.IsNullOrEmpty(value) ? this.DefaultValue : new DateTime(long.Parse(value, NumberFormatInfo.InvariantInfo));
		}

		public bool SerializationPreservesOrder { get { return true; } }
	}
}