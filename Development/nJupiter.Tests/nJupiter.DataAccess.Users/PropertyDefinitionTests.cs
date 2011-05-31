using System;

using nJupiter.DataAccess.Users;

using NUnit.Framework;

namespace nJupiter.Tests.nJupiter.DataAccess.Users {
	[TestFixture]
	public class PropertyDefinitionTests {

		[Test]
		public void Equals_TwoDifferentDefinitionsWithSameValues_ReturnsTrue() {
			var definition1 = new PropertyDefinition("propertyName", typeof(GenericProperty<string>));
			var definition2 = new PropertyDefinition("propertyName", typeof(GenericProperty<string>));
			Assert.IsTrue(definition1.Equals(definition2));
		}

		[Test]
		public void GetHashCode_TwoDifferentDefinitionsWithSameValues_ReturnsSameHashCode() {
			var definition1 = new PropertyDefinition("propertyName", typeof(GenericProperty<string>));
			var definition2 = new PropertyDefinition("propertyName", typeof(GenericProperty<string>));
			Assert.AreEqual(definition1.GetHashCode(), definition2.GetHashCode());
		}

		[Test]
		public void Constructor_PassingNullPropertyName_ThrowsArgumentNullException() {
			Assert.Throws<ArgumentNullException>(() => new PropertyDefinition(null, typeof(GenericProperty<string>)));
		}

		[Test]
		public void Constructor_PassingNullPropertyType_ThrowsArgumentNullException() {
			Assert.Throws<ArgumentNullException>(() => new PropertyDefinition("propertyName", null));
		}



	}
}
