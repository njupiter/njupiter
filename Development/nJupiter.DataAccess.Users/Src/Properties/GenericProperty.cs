using System;
using System.ComponentModel;
using System.Globalization;

namespace nJupiter.DataAccess.Users {
	[Serializable]
	public class GenericProperty<T> : PropertyBase<T> {

		private readonly CultureInfo culture;
		private readonly TypeConverter converter;

		public GenericProperty(string name, IContext  context, CultureInfo culture) : base(name, context) {
			this.culture = culture;
			this.converter = TypeDescriptor.GetConverter(typeof(T));
			if(this.converter == null || !converter.CanConvertFrom(typeof(string))) {
				throw new NotSupportedException(string.Format("The specified type {0} has no TypeConverter and can therefor not be converted", typeof(T).Name));
			}
		}

		protected override bool SetDirtyOnTouch {
			get {
				return !this.Type.IsPrimitive && !this.Type.IsValueType && !this.Type.Equals(typeof(string));
			}
		}

		public override string ToSerializedString() {
			return converter.ConvertToString(null, culture, Value);
		}

		public override T DeserializePropertyValue(string value) {
			if(string.IsNullOrEmpty(value)) {
				return this.DefaultValue;
			}
			return (T)converter.ConvertFromString(null, culture, value);
		}

	}
}