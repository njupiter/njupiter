using System;
using System.ComponentModel;
using System.Data;
using System.Globalization;

using nJupiter.DataAccess.Users;

using NUnit.Framework;

namespace nJupiter.Tests.DataAccess.Users {
	
	[TestFixture]
	public class PropertyTests {

		[Test]
		public void Name_SetName_ReturnsPropertyWithSameName() {
			var context = new Context("context");
			var prop = new GenericProperty<string>("propertyname", context, CultureInfo.InvariantCulture);
			Assert.AreEqual("propertyname", prop.Name);
		}

		[Test]
		public void Context_SetContext_ReturnsPropertyWithSameContext() {
			var context = new Context("context");
			var prop = new GenericProperty<string>("propertyname", context, CultureInfo.InvariantCulture);
			Assert.AreEqual(context, prop.Context);
		}

		[Test]
		public void Type_SetType_ReturnsPropertyWithSameType() {
			var context = new Context("context");
			var prop = new GenericProperty<bool>("propertyname", context, CultureInfo.InvariantCulture);
			Assert.AreEqual(typeof(bool), prop.Type);
		}

		[Test]
		public void IsDirty_SetStringToSameValue_IsNotDirty() {
			var context = new Context("context");
			var prop = new GenericProperty<string>("propertyname", context, CultureInfo.InvariantCulture);
			prop.Value = "Value 1";
			Assert.IsTrue(prop.IsDirty);
			prop.IsDirty = false;
			prop.Value = "Value 1";
			Assert.IsFalse(prop.IsDirty);
		}

		[Test]
		public void IsDirty_SetStringToAnotherValue_IsDirty() {
			var context = new Context("context");
			var prop = new GenericProperty<string>("propertyname", context, CultureInfo.InvariantCulture);
			prop.Value = "Value 1";
			Assert.IsTrue(prop.IsDirty);
			prop.IsDirty = false;
			prop.Value = "Value 2";
			Assert.IsTrue(prop.IsDirty);
		}

		[Test]
		public void IsDirty_GetValueOfNoPrimitiveType_IsDirtyOnTouch() {
			var context = new Context("context");
			var prop = new GenericProperty<MyDummyClass1>("propertyname", context, CultureInfo.InvariantCulture);
			var dummyObject = new MyDummyClass1();
			prop.Value = dummyObject;
			Assert.IsTrue(prop.IsDirty);
			prop.IsDirty = false;
			var tochTheValueByReadingIt = prop.Value;
			Assert.AreEqual(tochTheValueByReadingIt, dummyObject);
			Assert.IsTrue(prop.IsDirty);
		}

		[Test]
		public void Context_SetType_ReturnsCorrectDefaultValue() {
			var context = new Context("context");
			IProperty prop = new GenericProperty<bool>("propertyname", context, CultureInfo.InvariantCulture);
			Assert.AreEqual(false, prop.DefaultValue);
		}

		[Test]
		public void IsEmpty_SetValueToDefault_ReturnsTrue() {
			var context = new Context("context");
			var prop = new GenericProperty<bool>("propertyname", context, CultureInfo.InvariantCulture);
			prop.Value = false;
			Assert.IsTrue(prop.IsEmpty());
		}

		[Test]
		public void IsEmpty_SetNoValue_ReturnsTrue() {
			var context = new Context("context");
			var prop = new GenericProperty<bool>("propertyname", context, CultureInfo.InvariantCulture);
			Assert.IsTrue(prop.IsEmpty());
		}

		[Test]
		public void IsEmpty_SetValueToContent_ReturnsFalse() {
			var context = new Context("context");
			var prop = new GenericProperty<string>("propertyname", context, CultureInfo.InvariantCulture);
			prop.Value = "Hello world";
			Assert.IsFalse(prop.IsEmpty());
		}


		[Test]
		public void IsEmpty_SetStringToEmpty_ReturnsTrue() {
			var context = new Context("context");
			var prop = new GenericProperty<string>("propertyname", context, CultureInfo.InvariantCulture);
			prop.Value = string.Empty;
			Assert.IsTrue(prop.IsEmpty());
		}


		[Test]
		public void Value_SetPropertyToReadOnlyAndTryToSetIt_ThrowsReadOnlyException() {
			var context = new Context("context");
			var prop = new GenericProperty<string>("propertyname", context, CultureInfo.InvariantCulture);
			prop.MakeReadOnly();
			Assert.IsTrue(prop.IsReadOnly);
			Assert.Throws<ReadOnlyException>(() => prop.Value = "Hello world");
		}

		[Test]
		public void Name_SetValueToIncorrectType_ThrowsInvalidCastException() {
			var context = new Context("context");
			IProperty prop = new GenericProperty<string>("propertyname", context, CultureInfo.InvariantCulture);
			Assert.Throws<InvalidCastException>(() => prop.Value = true);
		}

		[Test]
		public void Clone_MakeClone_ReturnsNewObjectWithSameValue() {
			var context = new Context("context");
			var prop = new GenericProperty<MyDummyClass1>("propertyname", context, CultureInfo.InvariantCulture);
			var dummyObject = new MyDummyClass1("my dummy object");
			prop.Value = dummyObject;
			var newProp = (GenericProperty<MyDummyClass1>)prop.Clone();
			Assert.AreNotSame(prop, newProp);
			Assert.AreEqual("my dummy object", newProp.Value.MyString);
		}

		[Test]
		public void Constructor_PassingTypeWithoutTypeConverter_ThrowsNotSupportedException() {
			var context = new Context("context");
			Assert.Throws<NotSupportedException>(() => new GenericProperty<MyDummyClass2>("propertyname", context, CultureInfo.InvariantCulture));
		}

		[Test]
		public void ToSerializedString_CreateObjectWithTypeConverter_ReturnCorrectValueFromTypeConverter() {
			var context = new Context("context");
			var prop = new GenericProperty<MyDummyClass1>("propertyname", context, CultureInfo.InvariantCulture);
			prop.Value = new MyDummyClass1("my dummy object");
			Assert.AreEqual("my dummy object", prop.ToSerializedString()) ;
		}

		[Test]
		public void DeserializePropertyValue_CreateValueWithDeserializer_ReturnCorrectValue() {
			var context = new Context("context");
			IProperty prop = new GenericProperty<MyDummyClass1>("propertyname", context, CultureInfo.InvariantCulture);
			prop.Value = prop.DeserializePropertyValue("my dummy object");
			Assert.AreEqual("my dummy object", ((MyDummyClass1)prop.Value).MyString) ;
		}

		[Test]
		public void DeserializePropertyValue_PassNull_ReturnDefaultValue() {
			var context = new Context("context");
			IProperty prop = new GenericProperty<bool>("propertyname", context, CultureInfo.InvariantCulture);
			prop.Value = prop.DeserializePropertyValue(null);
			Assert.AreEqual(false, prop.Value) ;
		}


		[TypeConverter(typeof(MyTypeConverter))]
		public class MyDummyClass1 {
			private readonly string myString;

			public string MyString { get { return myString; }  }

			public MyDummyClass1() {}

			public MyDummyClass1(string myString) {
				this.myString = myString;
			}

			class MyTypeConverter : TypeConverter {
				public override bool CanConvertFrom(ITypeDescriptorContext context, System.Type sourceType) {
					return true;
				}
				public override bool CanConvertTo(ITypeDescriptorContext context, System.Type destinationType) {
					return true;
				}

				public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
					var myString = value as string;
					if(myString != null) {
						return new MyDummyClass1(myString);
					}
					return base.ConvertFrom(context, culture, value);
				}

				public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, System.Type destinationType) {
					var dummy = value as MyDummyClass1;
					if(dummy != null) {
						return dummy.MyString;
					}
					return base.ConvertTo(context, culture, value, destinationType);
				}
			}
		}

		public class MyDummyClass2 { }
	}
}
