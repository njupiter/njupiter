using System;
using System.Globalization;

namespace nJupiter.DataAccess.Users.Sql {
	[Serializable]
	public class DateTimeProperty : PropertyBase<DateTime> {
		#region Constants
		private const string Format = "D19";
		#endregion

		[NonSerialized]
		private readonly DateTime defaultValue = DateTime.MinValue;

		public DateTimeProperty(string propertyName, Context context) : base(propertyName, context) { }

		public override string ToSerializedString() {
			return this.Value.Ticks.ToString(Format, NumberFormatInfo.InvariantInfo);
		}

		public override DateTime DeserializePropertyValue(string value) {
			return value == null ? defaultValue : new DateTime(long.Parse(value, NumberFormatInfo.InvariantInfo));
		}
	}
}