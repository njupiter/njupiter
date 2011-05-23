using System;
using System.Globalization;

namespace nJupiter.DataAccess.Users.Sql {
	[Serializable]
	public class BoolProperty : GenericProperty<bool>, ISqlProperty {
		public BoolProperty(string propertyName, Context context) : base(propertyName, context, CultureInfo.InvariantCulture) { }
		public bool SerializationPreservesOrder { get { return true; } }
	}
}