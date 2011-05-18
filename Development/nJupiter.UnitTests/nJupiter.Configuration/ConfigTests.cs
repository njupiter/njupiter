using System;
using System.Xml;

using FakeItEasy;

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
			var config = GetTestConfig(string.Format(@"<element value=""{0}"" />", DummyString));
			Assert.AreEqual(DummyString, config.GetValue("element"));
		}

		[Test]
		public void GetValue_ElementValueAttributeIsNotDummyString_AreNotEqual() {
			var config = GetTestConfig(@"<element value=""Lorem ipsum dolor sit amet"" />");
			Assert.AreNotEqual(DummyString, config.GetValue("element"));
		}

		[Test]
		public void GetValue_ElementContentIsDummyString_AreEqual() {
			var config = GetTestConfig(string.Format(@"<section><element>{0}</element></section>", DummyString));
			Assert.AreEqual(DummyString, config.GetValue("section", "element"));
		}

		[Test]
		public void GetValue_ElementDoesNotExists_ThrowsConfigValueNotFoundException() {
			var config = GetTestConfig(@"<otherelement value=""DummyValue"" />");
			Assert.Throws<ConfigValueNotFoundException>(() => config.GetValue("element"));
		}

		[Test]
		public void GetValueDecimal_ElementIsInSwedishAndHasSwedishDecimalSeparator_ReturnsTrue() {
			var config = GetTestConfig(@"<element xml:lang=""sv"" value=""1,1"" />");
			Assert.AreEqual(1.1, config.GetValue<decimal>("element"));
		}

		[Test]
		public void GetValueDecimal_ElementIsInSwedishAndHasNotSwedishDecimalSeparator_ThrowsInvalidConfigValueException() {
			var config = GetTestConfig(@"<element xml:lang=""sv"" value=""1.1"" />");
			Assert.Throws<InvalidConfigValueException>(() => config.GetValue<decimal>("element"));
		}

		[Test]
		public void GetValueDecimal_InvariantElementAndHasEnglishDecimalSeparator_ReturnsTrue() {
			var config = GetTestConfig(@"<element value=""1.1"" />");
			Assert.AreEqual(1.1, config.GetValue<decimal>("element"));
		}

		[Test]
		public void GetValueDecimal_InvariantElementAndHasNotEnglishDecimalSeparator_ThrowsInvalidConfigValueException() {
			var config = GetTestConfig(@"<element value=""1,1"" />");
			Assert.Throws<InvalidConfigValueException>(() => config.GetValue<decimal>("element"));
		}

		[Test]
		public void GetValueBool_ElementValueAttributeIsTrue_ReturnsTrue() {
			var config = GetTestConfig(@"<element value=""true"" />");
			Assert.IsTrue(config.GetValue<bool>("element"));
		}

		[Test]
		public void GetValueBool_ElementValueAttributeIsFalse_ReturnsFalse() {
			var config = GetTestConfig(@"<element value=""FALSE"" />");
			Assert.IsFalse(config.GetValue<bool>("element"));
		}

		[Test]
		public void GetValueBool_ElementContentIsTrue_ReturnsTrue() {
			var config = GetTestConfig(@"<section><element>TRUE</element></section>");
			Assert.IsTrue(config.GetValue<bool>("section", "element"));
		}

		[Test]
		public void GetValueBool_ElementContentIsFalse_ReturnsFalse() {
			var config = GetTestConfig(@"<section><element>FALSE</element></section>");
			Assert.IsFalse(config.GetValue<bool>("section", "element"));
		}

		[Test]
		public void GetValueBool_ElementValueAttributeIsNoBoolean_ThrowsInvalidConfigValueException() {
			var config = GetTestConfig(@"<element value=""DummyValue"" />");
			Assert.Throws<InvalidConfigValueException>(() => config.GetValue<bool>("element"));
		}

		[Test]
		public void GetValueBool_ElementDoesNotExists_ThrowsConfigValueNotFoundException() {
			var config = GetTestConfig(@"<otherelement value=""DummyValue"" />");
			Assert.Throws<ConfigValueNotFoundException>(() => config.GetValue<bool>("element"));
		}

		[Test]
		public void GetValueInt_ElementValueAttributeIsCorrectInt_AreEqual() {
			var config = GetTestConfig(string.Format(@"<element value=""{0}"" />", CorrectInt));
			Assert.AreEqual(CorrectInt, config.GetValue<int>("element"));
		}

		[Test]
		public void GetValueInt_ElementValueAttributeIsNotCorrectInt_AreNotEqual() {
			var config = GetTestConfig(@"<element value=""51515"" />");
			Assert.AreNotEqual(CorrectInt, config.GetValue<int>("element"));
		}

		[Test]
		public void GetValueInt_ElementContentIsCorrectInt_AreEqual() {
			var config = GetTestConfig(string.Format(@"<section><element>{0}</element></section>", CorrectInt));
			Assert.AreEqual(CorrectInt, config.GetValue<int>("section", "element"));
		}

		[Test]
		public void GetValueInt_ElementContentIsNotInt_ThrowsInvalidConfigValueException() {
			var config = GetTestConfig(@"<section><element>NotAnInt!</element></section>");
			Assert.Throws<InvalidConfigValueException>(() => config.GetValue<int>("section", "element"));
		}

		[Test]
		public void GetValueInt_ElementDoesNotExists_ThrowsConfigValueNotFoundException() {
			var config = GetTestConfig(@"<otherelement value=""DummyValue"" />");
			Assert.Throws<ConfigValueNotFoundException>(() => config.GetValue<int>("element"));
		}

		[Test]
		public void GetValueChar_ElementValueAttributeIsCorrectChar_AreEqual() {
			var config = GetTestConfig(string.Format(@"<element value=""{0}"" />", CorrectChar));
			Assert.AreEqual(CorrectChar, config.GetValue<char>("element"));
		}

		[Test]
		public void GetValueChar_ElementValueAttributeIsNotCharCorrectChar_AreNotEqual() {
			var config = GetTestConfig(@"<element value=""f"" />");
			Assert.AreNotEqual(CorrectChar, config.GetValue<char>("element"));
		}

		[Test]
		public void GetValueChar_ElementContentCharCorrectChar_AreEqual() {
			var config = GetTestConfig(string.Format(@"<section><element>{0}</element></section>", CorrectChar));
			Assert.AreEqual(CorrectChar, config.GetValue<char>("section", "element"));
		}

		[Test]
		public void GetValueChar_ElementContentIsNotChar_ThrowsInvalidConfigValueException() {
			var config = GetTestConfig(@"<section><element>NotAnChar!</element></section>");
			Assert.Throws<InvalidConfigValueException>(() => config.GetValue<char>("section", "element"));
		}

		[Test]
		public void GetValueChar_ElementDoesNotExists_ThrowsConfigValueNotFoundException() {
			var config = GetTestConfig(@"<otherelement value=""DummyValue"" />");
			Assert.Throws<ConfigValueNotFoundException>(() => config.GetValue<char>("element"));
		}

		[Test]
		public void GetAttribute_AttributeIsDummyString_AreEqual() {
			var config = GetTestConfig(string.Format(@"<element attribute=""{0}"" />", DummyString));
			Assert.AreEqual(DummyString, config.GetAttribute("element", "attribute"));
		}

		[Test]
		public void GetAttribute_AttributeIsNotDummyString_AreNotEqual() {
			var config = GetTestConfig(@"<element attribute=""Lorem ipsum dolor sit amet"" />");
			Assert.AreNotEqual(DummyString, config.GetAttribute("element", "attribute"));
		}

		[Test]
		public void GetAttribute_AttributeInSectionIsDummyString_AreEqual() {
			var config = GetTestConfig(string.Format(@"<section><element attribute=""{0}"" /></section>", DummyString));
			Assert.AreEqual(DummyString, config.GetAttribute("section", "element", "attribute"));
		}

		[Test]
		public void GetAttribute_AttributeDoesNotExists_ThrowsConfigValueNotFoundException() {
			var config = GetTestConfig(@"<otherelement attribute=""DummyValue"" />");
			Assert.Throws<ConfigValueNotFoundException>(() => config.GetAttribute("element", "attribute"));
		}

		[Test]
		public void GetAttributeBool_AttributeIsTrue_ReturnsTrue() {
			var config = GetTestConfig(@"<element attribute=""true"" />");
			Assert.IsTrue(config.GetAttribute<bool>("element", "attribute"));
		}

		[Test]
		public void GetAttributeBool_AttributeIsFalse_ReturnsFalse() {
			var config = GetTestConfig(@"<element attribute=""FALSE"" />");
			Assert.IsFalse(config.GetAttribute<bool>("element", "attribute"));
		}

		[Test]
		public void GetAttributeBool_AttributeInSectionIsTrue_ReturnsTrue() {
			var config = GetTestConfig(@"<section><element attribute=""TRUE""></element></section>");
			Assert.IsTrue(config.GetAttribute<bool>("section", "element", "attribute"));
		}

		[Test]
		public void GetAttributeBool_AttributeInSectionIsFalse_ReturnsFalse() {
			var config = GetTestConfig(@"<section><element attribute=""FALSE"" /></section>");
			Assert.IsFalse(config.GetAttribute<bool>("section", "element", "attribute"));
		}

		[Test]
		public void GetAttributeBool_AttributeValueAttributeIsNoBoolean_ThrowsInvalidConfigValueException() {
			var config = GetTestConfig(@"<element attribute=""DummyValue"" />");
			Assert.Throws<InvalidConfigValueException>(() => config.GetAttribute<bool>("element", "attribute"));
		}

		[Test]
		public void GetAttributeBool_AttributeDoesNotExists_ThrowsConfigValueNotFoundException() {
			var config = GetTestConfig(@"<element anotherattribute=""DummyValue"" />");
			Assert.Throws<ConfigValueNotFoundException>(() => config.GetAttribute<bool>("element", "attribute"));
		}

		[Test]
		public void GetAttributeInt_AttributeIsCorrectInt_AreEqual() {
			var config = GetTestConfig(string.Format(@"<element attribute=""{0}"" />", CorrectInt));
			Assert.AreEqual(CorrectInt, config.GetAttribute<int>("element", "attribute"));
		}

		[Test]
		public void GetAttributeInt_AttributeIsNotCorrectInt_AreNotEqual() {
			var config = GetTestConfig(@"<element attribute=""51515"" />");
			Assert.AreNotEqual(CorrectInt, config.GetAttribute<int>("element", "attribute"));
		}

		[Test]
		public void GetAttributeInt_AttributeInSectionIsCorrectInt_AreEqual() {
			var config = GetTestConfig(string.Format(@"<section><element attribute=""{0}"" /></section>", CorrectInt));
			Assert.AreEqual(CorrectInt, config.GetAttribute<int>("section", "element", "attribute"));
		}

		[Test]
		public void GetAttributeInt_AttributeIsNotInt_ThrowsInvalidConfigValueException() {
			var config = GetTestConfig(@"<section><element attribute=""NotAnInt!"" /></section>");
			Assert.Throws<InvalidConfigValueException>(() => config.GetAttribute<int>("section", "element", "attribute"));
		}

		[Test]
		public void GetAttributeInt_AttributeDoesNotExists_ThrowsConfigValueNotFoundException() {
			var config = GetTestConfig(@"<element otherattribute=""DummyValue"" />");
			Assert.Throws<ConfigValueNotFoundException>(() => config.GetAttribute<int>("element", "attribute"));
		}

		[Test]
		public void GetAttributeChar_AttributeIsCorrectChar_AreEqual() {
			var config = GetTestConfig(string.Format(@"<element attribute=""{0}"" />", CorrectChar));
			Assert.AreEqual(CorrectChar, config.GetAttribute<char>("element", "attribute"));
		}

		[Test]
		public void GetAttributeChar_AttributeIsNotCharCorrectChar_AreNotEqual() {
			var config = GetTestConfig(@"<element attribute=""f"" />");
			Assert.AreNotEqual(CorrectChar, config.GetAttribute<char>("element", "attribute"));
		}

		[Test]
		public void GetAttributeChar_AttributeInSectionCharCorrectChar_AreEqual() {
			var config = GetTestConfig(string.Format(@"<section><element attribute=""{0}"" /></section>", CorrectChar));
			Assert.AreEqual(CorrectChar, config.GetAttribute<char>("section", "element", "attribute"));
		}

		[Test]
		public void GetAttributeChar_AttributeContentIsNotChar_ThrowsInvalidConfigValueException() {
			var config = GetTestConfig(@"<section><element attribute=""NotAnChar!""/></section>");
			Assert.Throws<InvalidConfigValueException>(() => config.GetAttribute<char>("section", "element", "attribute"));
		}

		[Test]
		public void GetAttributeChar_AttributeDoesNotExists_ThrowsConfigValueNotFoundException() {
			var config = GetTestConfig(@"<otherelement attribute=""DummyValue"" />");
			Assert.Throws<ConfigValueNotFoundException>(() => config.GetAttribute<char>("element", "attribute"));
		}

		[Test]
		public void GetValueArray_GetArrayWithTwoElements_ReturnsStringArrayWithLenghtTwo() {
			var config = GetTestConfig(@"<elements><element>value1</element><element>value2</element></elements>");
			string[] array = config.GetValueArray("elements", "element");
			Assert.IsTrue(array.Length == 2);
		}

		[Test]
		public void GetValueArray_GetArrayWithTwoElements_HasCorrectValuesInArray() {
			var config = GetTestConfig(@"<elements><element>value1</element><element>value2</element></elements>");
			string[] array = config.GetValueArray("elements", "element");
			Assert.AreEqual("value1", array[0]);
			Assert.AreEqual("value2", array[1]);
		}

		[Test]
		public void GetValueArray_GetNonExistingElement_ReturnsEmptyArray() {
			var config = GetTestConfig(@"<elements/>");
			string[] array = config.GetValueArray<string>("elements", "element");
			Assert.AreEqual(0, array.Length);
		}		
		
		[Test]
		public void GetAttributeArray_GetArrayWithTwoElements_ReturnsStringArrayWithLenghtTwo() {
			var config = GetTestConfig(@"<elements><element attribute=""value1"" /><element attribute=""value2"" /></elements>");
			string[] array = config.GetAttributeArray("elements", "element", "attribute");
			Assert.AreEqual("value1", array[0]);
			Assert.AreEqual("value2", array[1]);
		}

		[Test]
		public void GetAttributeArray_GetArrayWithTwoElements_HasCorrectValuesInArray() {
			var config = GetTestConfig(@"<elements><element attribute=""value1"" /><element attribute=""value2"" /></elements>");
			string[] array = config.GetAttributeArray("elements", "element", "attribute");
			Assert.IsTrue(array.Length == 2);
		}

		[Test]
		public void GetValueArray_GetEmptyArray_ReturnsEmptyArray() {
			var config = GetTestConfig(@"<elements />");
			string[] array = config.GetValueArray("elements", "element");
			Assert.IsTrue(array.Length == 0);
		}

		[Test]
		public void GetValueIntArray_GetArrayWithTwoElements_ReturnsStringArrayWithLenghtTwo() {
			var config = GetTestConfig(@"<elements><element>1</element><element>2</element></elements>");
			int[] array = config.GetValueArray<int>("elements", "element");
			Assert.IsTrue(array.Length == 2);
		}

		[Test]
		public void GetValueIntArray_GetArrayWithTwoElements_HasCorrectValuesInArray() {
			var config = GetTestConfig(@"<elements><element>1</element><element>2</element></elements>");
			int[] array = config.GetValueArray<int>("elements", "element");
			Assert.AreEqual(1, array[0]);
			Assert.AreEqual(2, array[1]);
		}

		[Test]
		public void GetValueIntArray_GetArrayWithNonIntValue_ThrowsInvalidConfigValueException() {
			var config = GetTestConfig(@"<elements><element value=""1"" /><element value=""value2"" /></elements>");
			Assert.Throws<InvalidConfigValueException>(() => config.GetValueArray<int>("elements", "element"));	
		}

		[Test]
		public void GetAttributeIntArray_GetArrayWithTwoElements_ReturnsStringArrayWithLenghtTwo() {
			var config = GetTestConfig(@"<elements><element attribute=""1"" /><element attribute=""2"" /></elements>");
			int[] array = config.GetAttributeArray<int>("elements", "element", "attribute");
			Assert.AreEqual(1, array[0]);
			Assert.AreEqual(2, array[1]);
		}

		[Test]
		public void GetAttributeIntArray_GetArrayWithTwoElements_HasCorrectValuesInArray() {
			var config = GetTestConfig(@"<elements><element attribute=""1"" /><element attribute=""2"" /></elements>");
			int[] array = config.GetAttributeArray<int>("elements", "element", "attribute");
			Assert.IsTrue(array.Length == 2);
		}

		[Test]
		public void GetAttributeIntArray_GetArrayWithNonIntValue_ThrowsInvalidConfigValueException() {
			var config = GetTestConfig(@"<elements><element attribute=""1"" /><element attribute=""value2"" /></elements>");
			Assert.Throws<InvalidConfigValueException>(() => config.GetAttributeArray<int>("elements", "element", "attribute"));	
		}


		[Test]
		public void GetValueIntArray_GetEmptyArray_ReturnsEmptyArray() {
			var config = GetTestConfig(@"<elements />");
			int[] array = config.GetValueArray<int>("elements", "element");
			Assert.IsTrue(array.Length == 0);
		}

		[Test]
		public void GetValueCharArray_GetArrayWithTwoElements_ReturnsStringArrayWithLenghtTwo() {
			var config = GetTestConfig(@"<elements><element>A</element><element>B</element></elements>");
			char[] array = config.GetValueArray<char>("elements", "element");
			Assert.IsTrue(array.Length == 2);
		}

		[Test]
		public void GetValueCharArray_GetArrayWithTwoElements_HasCorrectValuesInArray() {
			var config = GetTestConfig(@"<elements><element>A</element><element>B</element></elements>");
			char[] array = config.GetValueArray<char>("elements", "element");
			Assert.AreEqual('A', array[0]);
			Assert.AreEqual('B', array[1]);
		}

		[Test]
		public void GetValueCharArray_GetArrayWithNonCharValue_ThrowsInvalidConfigValueException() {
			var config = GetTestConfig(@"<elements><element value=""A"" /><element value=""valueB"" /></elements>");
			Assert.Throws<InvalidConfigValueException>(() => config.GetValueArray<char>("elements", "element"));	
		}

		[Test]
		public void GetAttributeCharArray_GetArrayWithTwoElements_ReturnsStringArrayWithLenghtTwo() {
			var config = GetTestConfig(@"<elements><element attribute=""A"" /><element attribute=""B"" /></elements>");
			char[] array = config.GetAttributeArray<char>("elements", "element", "attribute");
			Assert.AreEqual('A', array[0]);
			Assert.AreEqual('B', array[1]);
		}

		[Test]
		public void GetAttributeCharArray_GetArrayWithTwoElements_HasCorrectValuesInArray() {
			var config = GetTestConfig(@"<elements><element attribute=""A"" /><element attribute=""B"" /></elements>");
			char[] array = config.GetAttributeArray<char>("elements", "element", "attribute");
			Assert.IsTrue(array.Length == 2);
		}

		[Test]
		public void GetAttributeCharArray_GetArrayWithNonCharValue_ThrowsInvalidConfigValueException() {
			var config = GetTestConfig(@"<elements><element attribute=""A"" /><element attribute=""valueB"" /></elements>");
			Assert.Throws<InvalidConfigValueException>(() => config.GetAttributeArray<char>("elements", "element", "attribute"));	
		}


		[Test]
		public void GetValueCharArray_GetEmptyArray_ReturnsEmptyArray() {
			var config = GetTestConfig(@"<elements />");
			char[] array = config.GetValueArray<char>("elements", "element");
			Assert.IsTrue(array.Length == 0);
		}


		[Test]
		public void ContainsKey_KeyInSectionExists_ReturnsTrue() {
			var config = GetTestConfig(string.Format(@"<section><element value=""{0}"" /></section>", DummyString));
			Assert.IsTrue(config.ContainsKey("section", "element"));
		}

		[Test]
		public void ContainsKey_KeyExists_ReturnsTrue() {
			var config = GetTestConfig(string.Format(@"<element value=""{0}"" />", DummyString));
			Assert.IsTrue(config.ContainsKey("element"));
		}

		[Test]
		public void ContainsKey_KeyInSectionDoesNotExist_ReturnsFalse() {
			var config = GetTestConfig(string.Format(@"<section><anotherelement value=""{0}"" /></section>", DummyString));
			Assert.IsFalse(config.ContainsKey("section", "element"));
		}

		[Test]
		public void ContainsKey_KeyDoesNotExist_ReturnsFalse() {
			var config = GetTestConfig(string.Format(@"<anotherelement value=""{0}"" />", DummyString));
			Assert.IsFalse(config.ContainsKey("element"));
		}

//
		[Test]
		public void ContainsKey_AttributeInSectionExists_ReturnsTrue() {
			var config = GetTestConfig(string.Format(@"<section><element attribute=""{0}"" /></section>", DummyString));
			Assert.IsTrue(config.ContainsAttribute("section", "element", "attribute"));
		}

		[Test]
		public void ContainsKey_AttributeExists_ReturnsTrue() {
			var config = GetTestConfig(string.Format(@"<element attribute=""{0}"" />", DummyString));
			Assert.IsTrue(config.ContainsAttribute("element", "attribute"));
		}

		[Test]
		public void ContainsKey_AttributeInSectionDoesNotExist_ReturnsFalse() {
			var config = GetTestConfig(string.Format(@"<section><element otherattribute=""{0}"" /></section>", DummyString));
			Assert.IsFalse(config.ContainsAttribute("section", "element", "attribute"));
		}

		[Test]
		public void ContainsKey_AttributeDoesNotExist_ReturnsFalse() {
			var config = GetTestConfig(string.Format(@"<anotherelement attribute=""{0}"" />", DummyString));
			Assert.IsFalse(config.ContainsAttribute("element", "attribute"));
		}

		[Test]
		public void Create_ConfigWithInvalidXml_ThrowsXmlException() {
			Assert.Throws<XmlException>(() => GetTestConfig(@"<invalidxml>"));
		}

		[Test]
		public void GetConfigurationSectionHandler_CreatenJupiterConfiguration_ReturnsnJupiterConfiguration() {
			var config = GetTestConfig(@"<nJupiterConfiguration><configDirectories><configDirectory value=""~/Config""/></configDirectories></nJupiterConfiguration>");
			var configHandler = config.GetConfigurationSectionHandler("nJupiterConfiguration", typeof(nJupiterConfigurationSectionHandler));
			XmlNode node = configHandler as XmlNode;
			Assert.IsNotNull(node);
			Assert.AreEqual("~/Config", node.SelectSingleNode("./configDirectories/configDirectory").Attributes["value"].Value);
		}
		

		[Test]
		public void GetConfigurationSectionHandler_GetNonExistingSectionHandler_ReturnsNull() {
			var config = GetTestConfig(@"<myConfigHandler><config /></myConfigHandler>");
			var configHandler = config.GetConfigurationSectionHandler("myConfigHandler", this.GetType());
			Assert.IsNull(configHandler);
		}		


		[Test]
		public void GetConfigSection_GetChildSection_RetuernsConfigWithElementContainingTestValue() {
			var baseConfig = GetTestConfig(@"<baseSection><childSection><element value=""test"" /></childSection></baseSection>");
			var childConfig = baseConfig.GetConfigSection("baseSection/childSection");
			Assert.AreEqual("test", childConfig.GetValue("element"));
		}

		[Test]
		public void GetConfigSection_GetChildSectionTwoTime_RetuernsTheSameConfigObject() {
			var baseConfig = GetTestConfig(@"<baseSection><childSection><element value=""test"" /></childSection></baseSection>");
			var childConfig1 = baseConfig.GetConfigSection("baseSection/childSection");
			var childConfig2 = baseConfig.GetConfigSection("baseSection/childSection");
			Assert.AreEqual(childConfig1, childConfig2);
		}


		[Test]
		public void GetConfigSection_GetChildThatDoesNotExist_IsNull() {
			var baseConfig = GetTestConfig(@"<baseSection><childSection><element value=""test"" /></childSection></baseSection>");
			var childConfig = baseConfig.GetConfigSection("baseSection/anotherChildSection");
			Assert.IsNull(childConfig);
		}

		[Test]
		public void Discarded_CreateConfigWithoutDisposingIt_ReturnsNoCalls() {
			var config = GetTestConfig(@"<testconfig />");
			var disposer = new DiscardListener();
			config.Discarded += disposer.ConfigDiscarded;
			Assert.AreEqual(0, disposer.NumberOfCalled);
		}

		[Test]
		public void Discarded_CreateConfigDisposingIt_ReturnsOneCalls() {
			var config = GetTestConfig(@"<testconfig />");
			var disposer = new DiscardListener();
			config.Discarded += disposer.ConfigDiscarded;
			config.Discard();
			Assert.AreEqual(1, disposer.NumberOfCalled);
		}


		[Test]
		public void Discarded_CreateConfigDisposingItSeveralTimes_ReturnsOneCalls() {
			var config = GetTestConfig(@"<testconfig />");
			var disposeListener = new DiscardListener();
			config.Discarded += disposeListener.ConfigDiscarded;
			config.Discard();
			config.Discard();
			config.Discard();
			config.Discard();
			config.Discard();
			config.Discard();
			Assert.AreEqual(1, disposeListener.NumberOfCalled);
		}

		[Test]
		public void Discarded_WathcerAttachedWhenCreated_ReturnsConfigWithWatcher() {
			XmlElement configXmlElement = GetConfigXmlDocument(@"<testconfig />");
			var configSource = A.Fake<IConfigSource>();
			var watcher = new FakeWatcher();
			A.CallTo(() => configSource.Watcher).Returns(watcher);

			var config = ConfigFactory.Create("testConfig", configXmlElement, configSource);

			Assert.NotNull(config.ConfigSource.Watcher != null);
		}

		[Test]
		public void Discarded_WathcerAttachedWhenCreated_DiscaredIsCalledWhenWathcerChanged() {
			XmlElement configXmlElement = GetConfigXmlDocument(@"<testconfig />");
			var configSource = A.Fake<IConfigSource>();
			var watcher = new FakeWatcher();
			A.CallTo(() => configSource.Watcher).Returns(watcher);

			var config = ConfigFactory.Create("testConfig", configXmlElement, configSource);

			var listener = new DiscardListener();
			config.Discarded += listener.ConfigDiscarded;

			watcher.OnWatchedFileChange();

			Assert.NotNull(config.ConfigSource.Watcher != null);
			Assert.AreEqual(1, listener.NumberOfCalled);
		}
		
		[Test]
		public void Constructor_CallWithNullConifgKey_ThrowsArgumentNullException() {
			Assert.Throws<ArgumentNullException>(() => new Config(null,  GetConfigXmlDocument("<test />")));
		}
		
		[Test]
		public void Constructor_CallWithNullConifgElement_ThrowsArgumentNullException() {
			Assert.Throws<ArgumentNullException>(() => new Config("configKey",  null));
		}
				

		class FakeWatcher : IConfigSourceWatcher {
			public void OnWatchedFileChange() {
				if(this.ConfigSourceUpdated != null) {
					this.ConfigSourceUpdated(this, EventArgs.Empty);
				}
			}

			public event EventHandler ConfigSourceUpdated;
		}

		class DiscardListener {
			private int called;
			public void ConfigDiscarded(object sender, EventArgs e) {
				called++;
			}
			public int NumberOfCalled { get{ return called; } }
		}
		

		private static IConfig GetTestConfig(string innerXml) {
			XmlElement configXmlElement = GetConfigXmlDocument(innerXml);
			return ConfigFactory.Create("testConfig", configXmlElement);
		}

		private static XmlElement GetConfigXmlDocument(string innerXml) {
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(string.Format(@"<configuration>{0}</configuration>", innerXml));
			return xmlDocument.DocumentElement;
		}
	}

}
