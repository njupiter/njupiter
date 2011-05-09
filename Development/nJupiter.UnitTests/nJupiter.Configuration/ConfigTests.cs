using System.Xml;

using nJupiter.Configuration;

using NUnit.Framework;

namespace nJupiter.UnitTests.Configuration {

	[TestFixture]
	public class ConfigTests {

		private const string DummyString = "The quick brown fox jumps over the lazy dog";
		private const char CorrectChar = 'F';
		private const int CorrectInt = 242;

		[Test]
		public void GetValue_ElementValueAttributeIsDummyString_AreEqual() {
			Config config = GetTestConfig(string.Format(@"<element value=""{0}"" />", DummyString));
			Assert.AreEqual(DummyString, config.GetValue("element"));
		}

		[Test]
		public void GetValue_ElementValueAttributeIsNotDummyString_AreNotEqual() {
			Config config = GetTestConfig(@"<element value=""Lorem ipsum dolor sit amet"" />");
			Assert.AreNotEqual(DummyString, config.GetValue("element"));
		}

		[Test]
		public void GetValue_ElementContentIsDummyString_AreEqual() {
			Config config = GetTestConfig(string.Format(@"<section><element>{0}</element></section>", DummyString));
			Assert.AreEqual(DummyString, config.GetValue("section", "element"));
		}

		[Test]
		public void GetValue_ElementDoesNotExists_ThrowsConfigValueNotFoundException() {
			Config config = GetTestConfig(@"<otherelement value=""DummyValue"" />");
			Assert.Throws<ConfigValueNotFoundException>(() => config.GetValue("element"));
		}

		[Test]
		public void GetBoolValue_ElementValueAttributeIsTrue_ReturnsTrue() {
			Config config = GetTestConfig(@"<element value=""true"" />");
			Assert.IsTrue(config.GetBoolValue("element"));
		}

		[Test]
		public void GetBoolValue_ElementValueAttributeIsFalse_ReturnsFalse() {
			Config config = GetTestConfig(@"<element value=""FALSE"" />");
			Assert.IsFalse(config.GetBoolValue("element"));
		}

		[Test]
		public void GetBoolValue_ElementContentIsTrue_ReturnsTrue() {
			Config config = GetTestConfig(@"<section><element>TRUE</element></section>");
			Assert.IsTrue(config.GetBoolValue("section", "element"));
		}

		[Test]
		public void GetBoolValue_ElementContentIsFalse_ReturnsFalse() {
			Config config = GetTestConfig(@"<section><element>FALSE</element></section>");
			Assert.IsFalse(config.GetBoolValue("section", "element"));
		}

		[Test]
		public void GetBoolValue_ElementValueAttributeIsNoBoolean_ReturnsFalse() {
			Config config = GetTestConfig(@"<element value=""DummyValue"" />");
			Assert.IsFalse(config.GetBoolValue("element"));
		}

		[Test]
		public void GetBoolValue_ElementDoesNotExists_ThrowsConfigValueNotFoundException() {
			Config config = GetTestConfig(@"<otherelement value=""DummyValue"" />");
			Assert.Throws<ConfigValueNotFoundException>(() => config.GetBoolValue("element"));
		}

		[Test]
		public void GetIntValue_ElementValueAttributeIsCorrectInt_AreEqual() {
			Config config = GetTestConfig(string.Format(@"<element value=""{0}"" />", CorrectInt));
			Assert.AreEqual(CorrectInt, config.GetIntValue("element"));
		}

		[Test]
		public void GetIntValue_ElementValueAttributeIsNotCorrectInt_AreNotEqual() {
			Config config = GetTestConfig(@"<element value=""51515"" />");
			Assert.AreNotEqual(CorrectInt, config.GetIntValue("element"));
		}

		[Test]
		public void GetIntValue_ElementContentIsCorrectInt_AreEqual() {
			Config config = GetTestConfig(string.Format(@"<section><element>{0}</element></section>", CorrectInt));
			Assert.AreEqual(CorrectInt, config.GetIntValue("section", "element"));
		}

		[Test]
		public void GetIntValue_ElementContentIsNotInt_ThrowsInvalidConfigValueException() {
			Config config = GetTestConfig(@"<section><element>NotAnInt!</element></section>");
			Assert.Throws<InvalidConfigValueException>(() => config.GetIntValue("section", "element"));
		}

		[Test]
		public void GetIntValue_ElementDoesNotExists_ThrowsConfigValueNotFoundException() {
			Config config = GetTestConfig(@"<otherelement value=""DummyValue"" />");
			Assert.Throws<ConfigValueNotFoundException>(() => config.GetIntValue("element"));
		}

		[Test]
		public void GetCharValue_ElementValueAttributeIsCorrectChar_AreEqual() {
			Config config = GetTestConfig(string.Format(@"<element value=""{0}"" />", CorrectChar));
			Assert.AreEqual(CorrectChar, config.GetCharValue("element"));
		}

		[Test]
		public void GetCharValue_ElementValueAttributeIsNotCharCorrectChar_AreNotEqual() {
			Config config = GetTestConfig(@"<element value=""f"" />");
			Assert.AreNotEqual(CorrectChar, config.GetCharValue("element"));
		}

		[Test]
		public void GetCharValue_ElementContentCharCorrectChar_AreEqual() {
			Config config = GetTestConfig(string.Format(@"<section><element>{0}</element></section>", CorrectChar));
			Assert.AreEqual(CorrectChar, config.GetCharValue("section", "element"));
		}

		[Test]
		public void GetCharValue_ElementContentIsNotChar_ThrowsInvalidConfigValueException() {
			Config config = GetTestConfig(@"<section><element>NotAnChar!</element></section>");
			Assert.Throws<InvalidConfigValueException>(() => config.GetCharValue("section", "element"));
		}

		[Test]
		public void GetCharValue_ElementDoesNotExists_ThrowsConfigValueNotFoundException() {
			Config config = GetTestConfig(@"<otherelement value=""DummyValue"" />");
			Assert.Throws<ConfigValueNotFoundException>(() => config.GetCharValue("element"));
		}

		[Test]
		public void GetAttribute_AttributeIsDummyString_AreEqual() {
			Config config = GetTestConfig(string.Format(@"<element attribute=""{0}"" />", DummyString));
			Assert.AreEqual(DummyString, config.GetAttribute("element", "attribute"));
		}

		[Test]
		public void GetAttribute_AttributeIsNotDummyString_AreNotEqual() {
			Config config = GetTestConfig(@"<element attribute=""Lorem ipsum dolor sit amet"" />");
			Assert.AreNotEqual(DummyString, config.GetAttribute("element", "attribute"));
		}

		[Test]
		public void GetAttribute_AttributeInSectionIsDummyString_AreEqual() {
			Config config = GetTestConfig(string.Format(@"<section><element attribute=""{0}"" /></section>", DummyString));
			Assert.AreEqual(DummyString, config.GetAttribute("section", "element", "attribute"));
		}

		[Test]
		public void GetAttribute_AttributeDoesNotExists_ThrowsConfigValueNotFoundException() {
			Config config = GetTestConfig(@"<otherelement attribute=""DummyValue"" />");
			Assert.Throws<ConfigValueNotFoundException>(() => config.GetAttribute("element", "attribute"));
		}

		[Test]
		public void GetBoolAttribute_AttributeIsTrue_ReturnsTrue() {
			Config config = GetTestConfig(@"<element attribute=""true"" />");
			Assert.IsTrue(config.GetBoolAttribute("element", "attribute"));
		}

		[Test]
		public void GetBoolAttribute_AttributeIsFalse_ReturnsFalse() {
			Config config = GetTestConfig(@"<element attribute=""FALSE"" />");
			Assert.IsFalse(config.GetBoolAttribute("element", "attribute"));
		}

		[Test]
		public void GetBoolAttribute_AttributeInSectionIsTrue_ReturnsTrue() {
			Config config = GetTestConfig(@"<section><element attribute=""TRUE""></element></section>");
			Assert.IsTrue(config.GetBoolAttribute("section", "element", "attribute"));
		}

		[Test]
		public void GetBoolAttribute_AttributeInSectionIsFalse_ReturnsFalse() {
			Config config = GetTestConfig(@"<section><element attribute=""FALSE"" /></section>");
			Assert.IsFalse(config.GetBoolAttribute("section", "element", "attribute"));
		}

		[Test]
		public void GetBoolAttribute_AttributeValueAttributeIsNoBoolean_ReturnsFalse() {
			Config config = GetTestConfig(@"<element attribute=""DummyValue"" />");
			Assert.IsFalse(config.GetBoolAttribute("element", "attribute"));
		}

		[Test]
		public void GetBoolAttribute_AttributeDoesNotExists_ThrowsConfigValueNotFoundException() {
			Config config = GetTestConfig(@"<element anotherattribute=""DummyValue"" />");
			Assert.Throws<ConfigValueNotFoundException>(() => config.GetBoolAttribute("element", "attribute"));
		}

		[Test]
		public void GetIntAttribute_AttributeIsCorrectInt_AreEqual() {
			Config config = GetTestConfig(string.Format(@"<element attribute=""{0}"" />", CorrectInt));
			Assert.AreEqual(CorrectInt, config.GetIntAttribute("element", "attribute"));
		}

		[Test]
		public void GetIntAttribute_AttributeIsNotCorrectInt_AreNotEqual() {
			Config config = GetTestConfig(@"<element attribute=""51515"" />");
			Assert.AreNotEqual(CorrectInt, config.GetIntAttribute("element", "attribute"));
		}

		[Test]
		public void GetIntAttribute_AttributeInSectionIsCorrectInt_AreEqual() {
			Config config = GetTestConfig(string.Format(@"<section><element attribute=""{0}"" /></section>", CorrectInt));
			Assert.AreEqual(CorrectInt, config.GetIntAttribute("section", "element", "attribute"));
		}

		[Test]
		public void GetIntAttribute_AttributeIsNotInt_ThrowsInvalidConfigValueException() {
			Config config = GetTestConfig(@"<section><element attribute=""NotAnInt!"" /></section>");
			Assert.Throws<InvalidConfigValueException>(() => config.GetIntAttribute("section", "element", "attribute"));
		}

		[Test]
		public void GetIntAttribute_AttributeDoesNotExists_ThrowsConfigValueNotFoundException() {
			Config config = GetTestConfig(@"<element otherattribute=""DummyValue"" />");
			Assert.Throws<ConfigValueNotFoundException>(() => config.GetIntAttribute("element", "attribute"));
		}

		[Test]
		public void GetCharAttribute_AttributeIsCorrectChar_AreEqual() {
			Config config = GetTestConfig(string.Format(@"<element attribute=""{0}"" />", CorrectChar));
			Assert.AreEqual(CorrectChar, config.GetCharAttribute("element", "attribute"));
		}

		[Test]
		public void GetCharAttribute_AttributeIsNotCharCorrectChar_AreNotEqual() {
			Config config = GetTestConfig(@"<element attribute=""f"" />");
			Assert.AreNotEqual(CorrectChar, config.GetCharAttribute("element", "attribute"));
		}

		[Test]
		public void GetCharAttribute_AttributeInSectionCharCorrectChar_AreEqual() {
			Config config = GetTestConfig(string.Format(@"<section><element attribute=""{0}"" /></section>", CorrectChar));
			Assert.AreEqual(CorrectChar, config.GetCharAttribute("section", "element", "attribute"));
		}

		[Test]
		public void GetCharAttribute_AttributeContentIsNotChar_ThrowsInvalidConfigValueException() {
			Config config = GetTestConfig(@"<section><element attribute=""NotAnChar!""/></section>");
			Assert.Throws<InvalidConfigValueException>(() => config.GetCharAttribute("section", "element", "attribute"));
		}

		[Test]
		public void GetCharAttribute_AttributeDoesNotExists_ThrowsConfigValueNotFoundException() {
			Config config = GetTestConfig(@"<otherelement attribute=""DummyValue"" />");
			Assert.Throws<ConfigValueNotFoundException>(() => config.GetCharAttribute("element", "attribute"));
		}

		[Test]
		public void GetValueArray_GetArrayWithTwoElements_ReturnsStringArrayWithLenghtTwo() {
			Config config = GetTestConfig(@"<elements><element>value1</element><element>value2</element></elements>");
			string[] array = config.GetValueArray("elements", "element");
			Assert.IsTrue(array.Length == 2);
		}

		[Test]
		public void GetValueArray_GetArrayWithTwoElements_HasCorrectValuesInArray() {
			Config config = GetTestConfig(@"<elements><element>value1</element><element>value2</element></elements>");
			string[] array = config.GetValueArray("elements", "element");
			Assert.AreEqual("value1", array[0]);
			Assert.AreEqual("value2", array[1]);
		}

		[Test]
		public void GetAttributeArray_GetArrayWithTwoElements_ReturnsStringArrayWithLenghtTwo() {
			Config config = GetTestConfig(@"<elements><element attribute=""value1"" /><element attribute=""value2"" /></elements>");
			string[] array = config.GetAttributeArray("elements", "element", "attribute");
			Assert.AreEqual("value1", array[0]);
			Assert.AreEqual("value2", array[1]);
		}

		[Test]
		public void GetAttributeArray_GetArrayWithTwoElements_HasCorrectValuesInArray() {
			Config config = GetTestConfig(@"<elements><element attribute=""value1"" /><element attribute=""value2"" /></elements>");
			string[] array = config.GetAttributeArray("elements", "element", "attribute");
			Assert.IsTrue(array.Length == 2);
		}

		[Test]
		public void GetValueArray_GetEmptyArray_ReturnsEmptyArray() {
			Config config = GetTestConfig(@"<elements />");
			string[] array = config.GetValueArray("elements", "element");
			Assert.IsTrue(array.Length == 0);
		}

		[Test]
		public void GetIntValueArray_GetArrayWithTwoElements_ReturnsStringArrayWithLenghtTwo() {
			Config config = GetTestConfig(@"<elements><element>1</element><element>2</element></elements>");
			int[] array = config.GetIntValueArray("elements", "element");
			Assert.IsTrue(array.Length == 2);
		}

		[Test]
		public void GetIntValueArray_GetArrayWithTwoElements_HasCorrectValuesInArray() {
			Config config = GetTestConfig(@"<elements><element>1</element><element>2</element></elements>");
			int[] array = config.GetIntValueArray("elements", "element");
			Assert.AreEqual(1, array[0]);
			Assert.AreEqual(2, array[1]);
		}

		[Test]
		public void GetIntValueArray_GetArrayWithNonIntValue_ThrowsInvalidConfigValueException() {
			Config config = GetTestConfig(@"<elements><element value=""1"" /><element value=""value2"" /></elements>");
			Assert.Throws<InvalidConfigValueException>(() => config.GetIntValueArray("elements", "element"));	
		}

		[Test]
		public void GetIntAttributeArray_GetArrayWithTwoElements_ReturnsStringArrayWithLenghtTwo() {
			Config config = GetTestConfig(@"<elements><element attribute=""1"" /><element attribute=""2"" /></elements>");
			int[] array = config.GetIntAttributeArray("elements", "element", "attribute");
			Assert.AreEqual(1, array[0]);
			Assert.AreEqual(2, array[1]);
		}

		[Test]
		public void GetIntAttributeArray_GetArrayWithTwoElements_HasCorrectValuesInArray() {
			Config config = GetTestConfig(@"<elements><element attribute=""1"" /><element attribute=""2"" /></elements>");
			int[] array = config.GetIntAttributeArray("elements", "element", "attribute");
			Assert.IsTrue(array.Length == 2);
		}

		[Test]
		public void GetIntAttributeArray_GetArrayWithNonIntValue_ThrowsInvalidConfigValueException() {
			Config config = GetTestConfig(@"<elements><element attribute=""1"" /><element attribute=""value2"" /></elements>");
			Assert.Throws<InvalidConfigValueException>(() => config.GetIntAttributeArray("elements", "element", "attribute"));	
		}


		[Test]
		public void GetIntValueArray_GetEmptyArray_ReturnsEmptyArray() {
			Config config = GetTestConfig(@"<elements />");
			int[] array = config.GetIntValueArray("elements", "element");
			Assert.IsTrue(array.Length == 0);
		}

		[Test]
		public void GetCharValueArray_GetArrayWithTwoElements_ReturnsStringArrayWithLenghtTwo() {
			Config config = GetTestConfig(@"<elements><element>A</element><element>B</element></elements>");
			char[] array = config.GetCharValueArray("elements", "element");
			Assert.IsTrue(array.Length == 2);
		}

		[Test]
		public void GetCharValueArray_GetArrayWithTwoElements_HasCorrectValuesInArray() {
			Config config = GetTestConfig(@"<elements><element>A</element><element>B</element></elements>");
			char[] array = config.GetCharValueArray("elements", "element");
			Assert.AreEqual('A', array[0]);
			Assert.AreEqual('B', array[1]);
		}

		[Test]
		public void GetCharValueArray_GetArrayWithNonCharValue_ThrowsInvalidConfigValueException() {
			Config config = GetTestConfig(@"<elements><element value=""A"" /><element value=""valueB"" /></elements>");
			Assert.Throws<InvalidConfigValueException>(() => config.GetCharValueArray("elements", "element"));	
		}

		[Test]
		public void GetCharAttributeArray_GetArrayWithTwoElements_ReturnsStringArrayWithLenghtTwo() {
			Config config = GetTestConfig(@"<elements><element attribute=""A"" /><element attribute=""B"" /></elements>");
			char[] array = config.GetCharAttributeArray("elements", "element", "attribute");
			Assert.AreEqual('A', array[0]);
			Assert.AreEqual('B', array[1]);
		}

		[Test]
		public void GetCharAttributeArray_GetArrayWithTwoElements_HasCorrectValuesInArray() {
			Config config = GetTestConfig(@"<elements><element attribute=""A"" /><element attribute=""B"" /></elements>");
			char[] array = config.GetCharAttributeArray("elements", "element", "attribute");
			Assert.IsTrue(array.Length == 2);
		}

		[Test]
		public void GetCharAttributeArray_GetArrayWithNonCharValue_ThrowsInvalidConfigValueException() {
			Config config = GetTestConfig(@"<elements><element attribute=""A"" /><element attribute=""valueB"" /></elements>");
			Assert.Throws<InvalidConfigValueException>(() => config.GetCharAttributeArray("elements", "element", "attribute"));	
		}

		[Test]
		public void GetCharValueArray_GetEmptyArray_ReturnsEmptyArray() {
			Config config = GetTestConfig(@"<elements />");
			char[] array = config.GetCharValueArray("elements", "element");
			Assert.IsTrue(array.Length == 0);
		}
		
		private static Config GetTestConfig(string innerXml) {
			XmlElement configXmlElement = GetConfigXmlDocument(innerXml);
			return new Config("testConfig", configXmlElement);
		}

		private static XmlElement GetConfigXmlDocument(string innerXml) {
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.InnerXml = string.Format(@"<configuration>{0}</configuration>", innerXml);
			return xmlDocument.DocumentElement;
		}
	}

}
