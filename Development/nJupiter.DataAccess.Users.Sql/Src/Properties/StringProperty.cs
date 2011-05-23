using System;
using System.Globalization;

namespace nJupiter.DataAccess.Users.Sql {
	[Serializable]
	public class StringProperty : GenericProperty<string>, ISqlProperty {
		public StringProperty(string propertyName, Context context) : base(propertyName, context, CultureInfo.InvariantCulture) { }
		public bool SerializationPreservesOrder { get { return true; } }
	}
}