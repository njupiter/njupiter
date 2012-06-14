using System;
using System.Globalization;

namespace nJupiter.DataAccess.Users.Sql {
	[Serializable]
	public class StringProperty : Property<string>, ISqlProperty {
		public StringProperty(string propertyName, IContext context) : base(propertyName, context, CultureInfo.InvariantCulture) { }
		public bool SerializationPreservesOrder { get { return true; } }
	}
}