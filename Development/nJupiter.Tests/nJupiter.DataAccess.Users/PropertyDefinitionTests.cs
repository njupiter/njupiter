using System;

using nJupiter.DataAccess.Users;

using NUnit.Framework;

namespace nJupiter.Tests.DataAccess.Users {
	[TestFixture]
	public class PropertyDefinitionTests {

		[Test]
		public void Equals_TwoDifferentDefinitionsWithSameValues_ReturnsTrue() {
			var definition1 = new PropertyDefinition("propertyName", typeof(Property<string>));
			var definition2 = new PropertyDefinition("propertyName", typeof(Property<string>));
			Assert.IsTrue(definition1.Equals(definition2));
		}

		[Test]
		public void GetHashCode_TwoDifferentDefinitionsWithSameValues_ReturnsSameHashCode() {
			var definition1 = new PropertyDefinition("propertyName", typeof(Property<string>));
			var definition2 = new PropertyDefinition("propertyName", typeof(Property<string>));
			Assert.AreEqual(definition1.GetHashCode(), definition2.GetHashCode());
		}

		[Test]
		public void Constructor_PassingNullPropertyName_ThrowsArgumentNullException() {
			Assert.Throws<ArgumentNullException>(() => new PropertyDefinition(null, typeof(Property<string>)));
		}

		[Test]
		public void Constructor_PassingNullPropertyType_ThrowsArgumentNullException() {
			Assert.Throws<ArgumentNullException>(() => new PropertyDefinition("propertyName", null));
		}



	}
}
