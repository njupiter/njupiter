using System;
using System.Data;
using System.Linq;
using System.Globalization;

using FakeItEasy;

using NUnit.Framework;

namespace nJupiter.DataAccess.Users.Tests.Unit {

	[TestFixture]
	public class PropertyHandlerTests {

		[Test]
		public void GetProperties_PassDefaultPropertyCollection_ReturnsSameCollection() {
			var defaultProperties = A.Fake<IPropertyCollection>();
			var propertyHandler = new PropertyHandler("username", defaultProperties, null);
			Assert.AreEqual(defaultProperties, propertyHandler.GetProperties());
		}

		[Test]
		public void AttachProperties_AttachCollectionForContext_ContextAttached() {
			var prophandler = DummyFactory.GetPropertyHandler();

			var context = new Context("context");
			var attachedProperties = DummyFactory.GetPropertyCollection(12, context);

			prophandler.AttachProperties(attachedProperties);

			Assert.IsTrue(prophandler.AttachedContexts.Contains(context));
			Assert.AreEqual(attachedProperties, prophandler.GetProperties(context));
		}

		[Test]
		public void AttachProperties_AttachCollectionForContext_AnotherContextNotAttached() {
			var prophandler = DummyFactory.GetPropertyHandler();

			var context = new Context("context");
			var attachedProperties = DummyFactory.GetPropertyCollection(12, context);

			prophandler.AttachProperties(attachedProperties);
			var anotherContext = new Context("anotherContext");
			Assert.IsFalse(prophandler.AttachedContexts.Contains(anotherContext));
			Assert.IsNull(prophandler.GetProperties(anotherContext));
		}

		[Test]
		public void AttachProperties_PropertyCollectionWithMishmatchingSchemaCount_ThrowsArgumentException() {
			var prophandler = DummyFactory.GetPropertyHandler();

			var propertyList = DummyFactory.GetPropertyList(12);
			var schema = DummyFactory.GetSchema(11);
			var attachedProperties = new PropertyCollection(propertyList, schema);

			Assert.Throws<ArgumentException>(() => prophandler.AttachProperties(attachedProperties));
		}

		[Test]
		public void AttachProperties_PropertyCollectionWithMishmatchingSchemaNames_ThrowsArgumentException() {
			var prophandler = DummyFactory.GetPropertyHandler();

			var propertyList = DummyFactory.GetPropertyList(12);
			var schema = DummyFactory.GetSchema<string>("otherpropertyname", 12);
			var attachedProperties = new PropertyCollection(propertyList, schema);

			Assert.Throws<ArgumentException>(() => prophandler.AttachProperties(attachedProperties));
		}

		[Test]
		public void AttachProperties_PropertyCollectionWithMishmatchingSchemaTypes_ThrowsArgumentException() {
			var prophandler = DummyFactory.GetPropertyHandler();

			var propertyList = DummyFactory.GetPropertyList(12);
			var schema = DummyFactory.GetSchema<int>("property", 12);
			var attachedProperties = new PropertyCollection(propertyList, schema);

			Assert.Throws<ArgumentException>(() => prophandler.AttachProperties(attachedProperties));
		}


		[Test]
		public void AttachProperties_PropertyCollectionWithMishmatchingContext_ThrowsArgumentException() {
			var prophandler = DummyFactory.GetPropertyHandler();

			var propertyList = DummyFactory.GetPropertyList(12);
			propertyList.Add(new Property<string>("property13", new Context("othercontext"), CultureInfo.InvariantCulture));
			var schema = DummyFactory.GetSchema<int>("property", 13);
			var attachedProperties = new PropertyCollection(propertyList, schema);

			Assert.Throws<ArgumentException>(() => prophandler.AttachProperties(attachedProperties));
		}

		[Test]
		public void AttachProperties_AttachAlreadyAttachedContext_ThrowsArgumentException() {
			var prophandler = DummyFactory.GetPropertyHandler();

			var attachedProperties = DummyFactory.GetPropertyCollection(12);

			prophandler.AttachProperties(attachedProperties);
			Assert.Throws<ArgumentException>(() => prophandler.AttachProperties(attachedProperties));
		}

		[Test]
		public void AttachProperties_PassingNull_ThrowsArgumentNullException() {
			var prophandler = DummyFactory.GetPropertyHandler();
			Assert.Throws<ArgumentNullException>(() => prophandler.AttachProperties(null));
		}

		[Test]
		public void GetProperties_PassingNull_ThrowsArgumentNullException() {
			var prophandler = DummyFactory.GetPropertyHandler();
			Assert.Throws<ArgumentNullException>(() => prophandler.GetProperties(null));
		}

		[Test]
		public void GetProperty_PassingNullProperty_ThrowsArgumentNullException() {
			var prophandler = DummyFactory.GetPropertyHandler();
			Assert.Throws<ArgumentNullException>(() => prophandler.GetProperty(null));
		}

		[Test]
		public void GetProperty_PassingNullContext_ThrowsArgumentNullException() {
			var prophandler = DummyFactory.GetPropertyHandler();
			Assert.Throws<ArgumentNullException>(() => prophandler.GetProperty("MyProperty", (IContext)null));
		}

		[Test]
		public void GetProperty_SetCommonName_ReturnsPropertyWithSetValue() {
			var prophandler = DummyFactory.GetPropertyHandler();
			prophandler.Title = "Hello world";
			var prop = prophandler.GetProperty(prophandler.PropertyNames.Title, prophandler.PropertyNames.ContextNames.Title);
			Assert.NotNull(prop);
			Assert.AreEqual("Hello world", prop.Value);
			Assert.AreEqual(prop.Value, prophandler.Title);
		}

		[Test]
		public void SetProperty_SetPropertyInOtherContextAndReadTheCommonName_ReturnsSetValue() {
			var prophandler = DummyFactory.GetPropertyHandler();
			var anotherContext = new Context("AnotherContext");
			var attachedProperties = DummyFactory.GetPropertyCollection(12, anotherContext);
			prophandler.AttachProperties(attachedProperties);
			prophandler.SetProperty(prophandler.PropertyNames.Department, prophandler.PropertyNames.ContextNames.Department, "Hello World");
			Assert.AreEqual("Hello World", prophandler.Department);
		}


		[Test]
		public void SetProperty_PassingNonExistingPropertyName_ThrowsArgumentException() {
			var prophandler = DummyFactory.GetPropertyHandler();
			Assert.Throws<ArgumentException>(() => prophandler.SetProperty("NoExisingPropertyName", null));
		}

		[Test]
		public void SetProperty_PassingNonExistingContext_ThrowsArgumentException() {
			var prophandler = DummyFactory.GetPropertyHandler();
			Assert.Throws<ArgumentException>(() => prophandler.SetProperty(prophandler.FirstName, "NonExisgingContext", null));
		}

		[Test]
		public void GetValue_GetNonExistingProperty_ReturnsDefaultValue() {
			var prophandler = DummyFactory.GetPropertyHandler();
			var result = prophandler.GetValue<DateTime>("NonExistantDate");
			Assert.AreEqual(DateTime.MinValue, result);
		}

		[Test]
		public void GetValue_SetTheValueInDefaultContext_ReturnsCorrectValue() {
			var prophandler = DummyFactory.GetPropertyHandler();
			prophandler.SetProperty("Description", "Hello World");
			Assert.AreEqual("Hello World", prophandler.GetValue<string>("Description"));
		}

		[Test]
		public void GetProperty_SetTheValueInDefaultContext_ReturnsCorrectValue() {
			var prophandler = DummyFactory.GetPropertyHandler();
			prophandler.SetProperty("Country", "Hello World");
			Assert.AreEqual("Hello World", prophandler.GetProperty("Country").Value);
		}

		[Test]
		public void Indexer_TryGetProperty8FromDefaultContext_ReturnsSameAsMethod() {
			var prophandler = DummyFactory.GetPropertyHandler();
			var property = prophandler.GetProperty("FirstName");
			property.Value = "Hello world";
			Assert.AreEqual(property, prophandler["FirstName"]);
			Assert.AreEqual("Hello world", prophandler["FirstName"].Value);
		}

		[Test]
		public void Indexer_TryGetFirstPropertyFromContext_ReturnsProperty() {
			var prophandler = DummyFactory.GetPropertyHandler();
			var attachedProperties = DummyFactory.GetPropertyCollection(12);
			prophandler.AttachProperties(attachedProperties);
			var firstProperty = attachedProperties.First();
			Assert.AreEqual(firstProperty, prophandler[firstProperty.Name, firstProperty.Context]);

		}

		[Test]
		public void LastLockoutDate_PropertyNotDefiendInCommonNamesAndIsThereforNotSet_ReturnsDefaultValueAfterSet() {
			var prophandler = DummyFactory.GetPropertyHandler();
			prophandler.LastLockoutDate = DateTime.UtcNow;
			Assert.AreEqual(DateTime.MinValue, prophandler.LastLockoutDate);
		}

		[Test]
		public void UserName_NoUserNameIsSet_ReturnsDefault() {
			var prophandler = DummyFactory.GetPropertyHandler("username");
			Assert.AreEqual("username", prophandler.UserName);
		}

		[Test]
		public void UserName_SetUserName_ReturnsSetValue() {
			var prophandler = DummyFactory.GetPropertyHandler("username");
			prophandler.SetProperty(prophandler.PropertyNames.UserName,
			                        prophandler.PropertyNames.ContextNames.UserName,
			                        "newusername");
			Assert.AreEqual("newusername", prophandler.UserName);
		}

		[Test]
		public void DisplayName_FullNameSet_ReturnsFullname() {
			var prophandler = DummyFactory.GetPropertyHandler("username");
			prophandler.FirstName = "John";
			prophandler.LastName = "Doe";
			prophandler.FullName = "Mr. John Doe";
			Assert.AreEqual("Mr. John Doe", prophandler.DisplayName);
		}

		[Test]
		public void DisplayName_FullNameNotSet_ReturnsFirstnameAndLastname() {
			var prophandler = DummyFactory.GetPropertyHandler("username");
			prophandler.FirstName = "John";
			prophandler.LastName = "Doe";
			Assert.AreEqual("John Doe", prophandler.DisplayName);
		}

		[Test]
		public void DisplayName_LastNameNotSet_ReturnsFirstnameAndUsername() {
			var prophandler = DummyFactory.GetPropertyHandler("username");
			prophandler.FirstName = "John";
			Assert.AreEqual("John (username)", prophandler.DisplayName);
		}

		[Test]
		public void DisplayName_OnlyLastNameSet_ReturnsUsername() {
			var prophandler = DummyFactory.GetPropertyHandler("username");
			prophandler.LastName = "Doe";
			Assert.AreEqual("username", prophandler.DisplayName);
		}

		[Test]
		public void MakeReadOnly_CheckObjectAndDefaultProperties_ReturnsTrue() {
			var prophandler = DummyFactory.GetPropertyHandler("username");
			prophandler.MakeReadOnly();
			Assert.IsTrue(prophandler.IsReadOnly);
			Assert.IsTrue(prophandler.GetProperties().IsReadOnly);
		}


		[Test]
		public void MakeReadOnly_SetAPropertyAfterReadOnly_ThrowsReadOnlyException() {
			var prophandler = DummyFactory.GetPropertyHandler("username");

			var anotherContext = new Context("AnotherContext");
			var attachedProperties = DummyFactory.GetPropertyCollection(12, anotherContext);
			prophandler.AttachProperties(attachedProperties);

			prophandler.MakeReadOnly();
			Assert.Throws<ReadOnlyException>(() => prophandler.StreetAddress = "Other Address");
			Assert.Throws<ReadOnlyException>(() => prophandler.Department = "Other Department");
		}

		[Test]
		public void MakeReadOnly_AttachContextAfterReadonly_AttachContextBecomesReadonly() {
			var prophandler = DummyFactory.GetPropertyHandler("username");
			prophandler.MakeReadOnly();

			var anotherContext = new Context("AnotherContext");
			var attachedProperties = DummyFactory.GetPropertyCollection(12, anotherContext);
			prophandler.AttachProperties(attachedProperties);

			Assert.Throws<ReadOnlyException>(() => prophandler.Department = "Other Department");
		}


		[Test]
		public void Clone_AttachContextAfterReadonly_AttachContextBecomesReadonly() {
			var prophandler = DummyFactory.GetPropertyHandler("username");
			prophandler.MakeReadOnly();

			var anotherContext = new Context("AnotherContext");
			var attachedProperties = DummyFactory.GetPropertyCollection(12, anotherContext);
			prophandler.AttachProperties(attachedProperties);

			var newprophander = (IPropertyHandler)prophandler.Clone();

			Assert.AreNotSame(prophandler, newprophander);

			newprophander.StreetAddress = "Other Address";
			newprophander.Department = "Other Department";
			
			Assert.AreEqual("Other Address", newprophander.StreetAddress);
			Assert.AreEqual("Other Department", newprophander.Department);
			Assert.AreNotEqual("Other Address", prophandler.StreetAddress);
			Assert.AreNotEqual("Other Department", prophandler.Department);

		}

		[Test]
		public void CreateWritable_AttachContextAfterReadonly_AttachContextBecomesReadonly() {
			var prophandler = DummyFactory.GetPropertyHandler("username");
			prophandler.MakeReadOnly();

			var anotherContext = new Context("AnotherContext");
			var attachedProperties = DummyFactory.GetPropertyCollection(12, anotherContext);
			prophandler.AttachProperties(attachedProperties);

			var newprophander = prophandler.CreateWritable();

			Assert.AreNotSame(prophandler, newprophander);

			newprophander.StreetAddress = "Other Address";
			newprophander.Department = "Other Department";
			
			Assert.AreEqual("Other Address", newprophander.StreetAddress);
			Assert.AreEqual("Other Department", newprophander.Department);
			Assert.AreNotEqual("Other Address", prophandler.StreetAddress);
			Assert.AreNotEqual("Other Department", prophandler.Department);

		}

		[Test]
		public void CommonNameProperties_SetAndGetSameValue() {
			var prophandler = DummyFactory.GetPropertyHandler("username");
			prophandler.FullName = "fullname";
			prophandler.FirstName = "firstname";
			prophandler.LastName = "lastname";
			prophandler.Description = "description";
			prophandler.Email = "email@test.org";
			prophandler.HomePage = "http://code.google.com/p/njupiter/";
			prophandler.StreetAddress = "street addres";
			prophandler.Company = "company";
			prophandler.City = "city";
			prophandler.Telephone = "+46123456";
			prophandler.Fax = "+416456789";
			prophandler.HomeTelephone = "+46789123";
			prophandler.MobileTelephone = "+46070123";
			prophandler.PostOfficeBox = "office box";
			prophandler.PostalCode = "12356";
			prophandler.Country = "sweden";
			prophandler.Title = "geek";
			prophandler.Active = true;
			prophandler.PasswordQuestion = "yeah what?";
			prophandler.PasswordAnswer = "31137";
			prophandler.LastActivityDate = DateTime.MaxValue;
			prophandler.CreationDate = DateTime.MaxValue;
			prophandler.LastLoginDate = DateTime.MaxValue;
			prophandler.LastPasswordChangedDate = DateTime.MaxValue;
			prophandler.Locked = true;
			prophandler.LastUpdatedDate = DateTime.MaxValue;
			prophandler.IsAnonymous = true;
			Assert.AreEqual("fullname", prophandler.FullName);
			Assert.AreEqual("firstname", prophandler.FirstName);
			Assert.AreEqual("lastname", prophandler.LastName);
			Assert.AreEqual("description", prophandler.Description);
			Assert.AreEqual("email@test.org", prophandler.Email);
			Assert.AreEqual("http://code.google.com/p/njupiter/", prophandler.HomePage);
			Assert.AreEqual("street addres", prophandler.StreetAddress);
			Assert.AreEqual("company", prophandler.Company);
			Assert.AreEqual("city", prophandler.City);
			Assert.AreEqual("+46123456", prophandler.Telephone);
			Assert.AreEqual("+416456789", prophandler.Fax);
			Assert.AreEqual("+46789123", prophandler.HomeTelephone);
			Assert.AreEqual("+46070123", prophandler.MobileTelephone);
			Assert.AreEqual("office box", prophandler.PostOfficeBox);
			Assert.AreEqual("12356", prophandler.PostalCode);
			Assert.AreEqual("sweden", prophandler.Country);
			Assert.AreEqual("geek", prophandler.Title);
			Assert.AreEqual(true, prophandler.Active);
			Assert.AreEqual("yeah what?", prophandler.PasswordQuestion);
			Assert.AreEqual("31137", prophandler.PasswordAnswer);
			Assert.AreEqual(DateTime.MaxValue, prophandler.LastActivityDate);
			Assert.AreEqual(DateTime.MaxValue, prophandler.CreationDate);
			Assert.AreEqual(DateTime.MaxValue, prophandler.LastLoginDate);
			Assert.AreEqual(DateTime.MaxValue, prophandler.LastPasswordChangedDate);
			Assert.AreEqual(DateTime.MaxValue, prophandler.LastUpdatedDate);
			Assert.AreEqual(true, prophandler.Locked);
			Assert.AreEqual(true, prophandler.IsAnonymous);
		}
	}
}