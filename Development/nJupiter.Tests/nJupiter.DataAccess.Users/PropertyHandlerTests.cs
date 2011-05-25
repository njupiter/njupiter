using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

using FakeItEasy;

using nJupiter.Configuration;
using nJupiter.DataAccess.Users;

using NUnit.Framework;

using PropertyCollection = nJupiter.DataAccess.Users.PropertyCollection;

namespace nJupiter.Tests.DataAccess.Users {

	[TestFixture]
	public class PropertyHandlerTests {

		[Test]
		public void GetProperties_PassDefaultPropertyCollection_ReturnsSameCollection() {
			var defaultProperties = A.Fake<PropertyCollection>();
			var propertyHandler = new PropertyHandler("username", defaultProperties, null);
			Assert.AreEqual(defaultProperties, propertyHandler.GetProperties());
		}

		[Test]
		public void AttachProperties_AttachCollectionForContext_ContextAttached() {
			PropertyHandler prophandler = GetPropertyHandler();

			var context = new Context("context");
			PropertyCollection attachedProperties = GetPropertyCollection(12, context);

			prophandler.AttachProperties(attachedProperties);

			Assert.IsTrue(prophandler.AttachedContexts.Contains(context));
			Assert.AreEqual(attachedProperties, prophandler.GetProperties(context));
		}

		[Test]
		public void AttachProperties_AttachCollectionForContext_AnotherContextNotAttached() {
			PropertyHandler prophandler = GetPropertyHandler();

			var context = new Context("context");
			PropertyCollection attachedProperties = GetPropertyCollection(12, context);

			prophandler.AttachProperties(attachedProperties);
			var anotherContext = new Context("anotherContext");
			Assert.IsFalse(prophandler.AttachedContexts.Contains(anotherContext));
			Assert.IsNull(prophandler.GetProperties(anotherContext));
		}

		[Test]
		public void AttachProperties_PropertyCollectionWithMishmatchingSchemaCount_ThrowsArgumentException() {
			PropertyHandler prophandler = GetPropertyHandler();

			var propertyList = GetPropertyList(12);
			var schema = GetSchema(11);
			var attachedProperties = new PropertyCollection(propertyList, schema);

			Assert.Throws<ArgumentException>(() => prophandler.AttachProperties(attachedProperties));
		}

		[Test]
		public void AttachProperties_PropertyCollectionWithMishmatchingSchemaNames_ThrowsArgumentException() {
			PropertyHandler prophandler = GetPropertyHandler();

			var propertyList = GetPropertyList(12);
			var schema = GetSchema<string>("otherpropertyname", 12);
			var attachedProperties = new PropertyCollection(propertyList, schema);

			Assert.Throws<ArgumentException>(() => prophandler.AttachProperties(attachedProperties));
		}

		[Test]
		public void AttachProperties_PropertyCollectionWithMishmatchingSchemaTypes_ThrowsArgumentException() {
			PropertyHandler prophandler = GetPropertyHandler();

			var propertyList = GetPropertyList(12);
			var schema = GetSchema<int>("property", 12);
			var attachedProperties = new PropertyCollection(propertyList, schema);

			Assert.Throws<ArgumentException>(() => prophandler.AttachProperties(attachedProperties));
		}


		[Test]
		public void AttachProperties_PropertyCollectionWithMishmatchingContext_ThrowsArgumentException() {
			PropertyHandler prophandler = GetPropertyHandler();

			var propertyList = GetPropertyList(12);
			propertyList.Add(new GenericProperty<string>("property13", new Context("othercontext"), CultureInfo.InvariantCulture));
			var schema = GetSchema<int>("property", 13);
			var attachedProperties = new PropertyCollection(propertyList, schema);

			Assert.Throws<ArgumentException>(() => prophandler.AttachProperties(attachedProperties));
		}

		[Test]
		public void AttachProperties_AttachAlreadyAttachedContext_ThrowsArgumentException() {
			PropertyHandler prophandler = GetPropertyHandler();

			PropertyCollection attachedProperties = GetPropertyCollection(12);

			prophandler.AttachProperties(attachedProperties);
			Assert.Throws<ArgumentException>(() => prophandler.AttachProperties(attachedProperties));
		}

		[Test]
		public void AttachProperties_PassingNull_ThrowsArgumentNullException() {
			PropertyHandler prophandler = GetPropertyHandler();
			Assert.Throws<ArgumentNullException>(() => prophandler.AttachProperties(null));
		}

		[Test]
		public void GetProperties_PassingNull_ThrowsArgumentNullException() {
			PropertyHandler prophandler = GetPropertyHandler();
			Assert.Throws<ArgumentNullException>(() => prophandler.GetProperties(null));
		}

		[Test]
		public void GetProperty_PassingNullProperty_ThrowsArgumentNullException() {
			PropertyHandler prophandler = GetPropertyHandler();
			Assert.Throws<ArgumentNullException>(() => prophandler.GetProperty(null));
		}

		[Test]
		public void GetProperty_PassingNullContext_ThrowsArgumentNullException() {
			PropertyHandler prophandler = GetPropertyHandler();
			Assert.Throws<ArgumentNullException>(() => prophandler.GetProperty("MyProperty", (Context)null));
		}

		[Test]
		public void GetProperty_SetCommonName_ReturnsPropertyWithSetValue() {
			PropertyHandler prophandler = GetPropertyHandler();
			prophandler.Title = "Hello world";
			var prop = prophandler.GetProperty(prophandler.PropertyNames.Title, prophandler.PropertyNames.ContextNames.Title);
			Assert.NotNull(prop);
			Assert.AreEqual("Hello world", prop.Value);
			Assert.AreEqual(prop.Value, prophandler.Title);
		}

		[Test]
		public void SetProperty_SetPropertyInOtherContextAndReadTheCommonName_ReturnsSetValue() {
			PropertyHandler prophandler = GetPropertyHandler();
			var anotherContext = new Context("AnotherContext");
			PropertyCollection attachedProperties = GetPropertyCollection(12, anotherContext);
			prophandler.AttachProperties(attachedProperties);
			prophandler.SetProperty(prophandler.PropertyNames.Department, prophandler.PropertyNames.ContextNames.Department, "Hello World");
			Assert.AreEqual("Hello World", prophandler.Department);
		}


		[Test]
		public void SetProperty_PassingNonExistingPropertyName_ThrowsArgumentException() {
			PropertyHandler prophandler = GetPropertyHandler();
			Assert.Throws<ArgumentException>(() => prophandler.SetProperty("NoExisingPropertyName", null));
		}

		[Test]
		public void SetProperty_PassingNonExistingContext_ThrowsArgumentException() {
			PropertyHandler prophandler = GetPropertyHandler();
			Assert.Throws<ArgumentException>(() => prophandler.SetProperty(prophandler.FirstName, "NonExisgingContext", null));
		}

		[Test]
		public void GetValue_GetNonExistingProperty_ReturnsDefaultValue() {
			PropertyHandler prophandler = GetPropertyHandler();
			var result = prophandler.GetValue<DateTime>("NonExistantDate");
			Assert.AreEqual(DateTime.MinValue, result);
		}

		[Test]
		public void GetValue_SetTheValueInDefaultContext_ReturnsCorrectValue() {
			PropertyHandler prophandler = GetPropertyHandler();
			prophandler.SetProperty("Description", "Hello World");
			Assert.AreEqual("Hello World", prophandler.GetValue<string>("Description"));
		}

		[Test]
		public void GetProperty_SetTheValueInDefaultContext_ReturnsCorrectValue() {
			PropertyHandler prophandler = GetPropertyHandler();
			prophandler.SetProperty("Country", "Hello World");
			Assert.AreEqual("Hello World", prophandler.GetProperty("Country").Value);
		}

		[Test]
		public void Indexer_TryGetProperty8FromDefaultContext_ReturnsSameAsMethod() {
			PropertyHandler prophandler = GetPropertyHandler();
			var property = prophandler.GetProperty("FirstName");
			property.Value = "Hello world";
			Assert.AreEqual(property, prophandler["FirstName"]);
			Assert.AreEqual("Hello world", prophandler["FirstName"].Value);
		}

		[Test]
		public void Indexer_TryGetFirstPropertyFromContext_ReturnsProperty() {
			PropertyHandler prophandler = GetPropertyHandler();
			PropertyCollection attachedProperties = GetPropertyCollection(12);
			prophandler.AttachProperties(attachedProperties);
			var firstProperty = attachedProperties.First();
			Assert.AreEqual(firstProperty, prophandler[firstProperty.Name, firstProperty.Context]);

		}

		[Test]
		public void LastLockoutDate_PropertyNotDefiendInCommonNamesAndIsThereforNotSet_ReturnsDefaultValueAfterSet() {
			PropertyHandler prophandler = GetPropertyHandler();
			prophandler.LastLockoutDate = DateTime.UtcNow;
			Assert.AreEqual(DateTime.MinValue, prophandler.LastLockoutDate);
		}

		[Test]
		public void UserName_NoUserNameIsSet_ReturnsDefault() {
			PropertyHandler prophandler = GetPropertyHandler("username");
			Assert.AreEqual("username", prophandler.UserName);
		}

		[Test]
		public void UserName_SetUserName_ReturnsSetValue() {
			PropertyHandler prophandler = GetPropertyHandler("username");
			prophandler.SetProperty(prophandler.PropertyNames.UserName,
			                        prophandler.PropertyNames.ContextNames.UserName,
			                        "newusername");
			Assert.AreEqual("newusername", prophandler.UserName);
		}

		[Test]
		public void DisplayName_FullNameSet_ReturnsFullname() {
			PropertyHandler prophandler = GetPropertyHandler("modhelius");
			prophandler.FirstName = "Martin";
			prophandler.LastName = "Odhelius";
			prophandler.FullName = "Mr. Martin Odhelius";
			Assert.AreEqual("Mr. Martin Odhelius", prophandler.DisplayName);
		}

		[Test]
		public void DisplayName_FullNameNotSet_ReturnsFirstnameAndLastname() {
			PropertyHandler prophandler = GetPropertyHandler("modhelius");
			prophandler.FirstName = "Martin";
			prophandler.LastName = "Odhelius";
			Assert.AreEqual("Martin Odhelius", prophandler.DisplayName);
		}

		[Test]
		public void DisplayName_LastNameNotSet_ReturnsFirstnameAndUsername() {
			PropertyHandler prophandler = GetPropertyHandler("modhelius");
			prophandler.FirstName = "Martin";
			Assert.AreEqual("Martin (modhelius)", prophandler.DisplayName);
		}

		[Test]
		public void DisplayName_OnlyLastNameSet_ReturnsUsername() {
			PropertyHandler prophandler = GetPropertyHandler("modhelius");
			prophandler.LastName = "Odhelius";
			Assert.AreEqual("modhelius", prophandler.DisplayName);
		}

		[Test]
		public void MakeReadOnly_CheckObjectAndDefaultProperties_ReturnsTrue() {
			PropertyHandler prophandler = GetPropertyHandler("modhelius");
			prophandler.MakeReadOnly();
			Assert.IsTrue(prophandler.IsReadOnly);
			Assert.IsTrue(prophandler.GetProperties().IsReadOnly);
		}


		[Test]
		public void MakeReadOnly_SetAPropertyAfterReadOnly_ThrowsReadOnlyException() {
			PropertyHandler prophandler = GetPropertyHandler("modhelius");

			var anotherContext = new Context("AnotherContext");
			PropertyCollection attachedProperties = GetPropertyCollection(12, anotherContext);
			prophandler.AttachProperties(attachedProperties);

			prophandler.MakeReadOnly();
			Assert.Throws<ReadOnlyException>(() => prophandler.StreetAddress = "Other Address");
			Assert.Throws<ReadOnlyException>(() => prophandler.Department = "Other Department");
		}

		[Test]
		public void MakeReadOnly_AttachContextAfterReadonly_AttachContextBecomesReadonly() {
			PropertyHandler prophandler = GetPropertyHandler("modhelius");
			prophandler.MakeReadOnly();

			var anotherContext = new Context("AnotherContext");
			PropertyCollection attachedProperties = GetPropertyCollection(12, anotherContext);
			prophandler.AttachProperties(attachedProperties);

			Assert.Throws<ReadOnlyException>(() => prophandler.Department = "Other Department");
		}


		[Test]
		public void Clone_AttachContextAfterReadonly_AttachContextBecomesReadonly() {
			PropertyHandler prophandler = GetPropertyHandler("modhelius");
			prophandler.MakeReadOnly();

			var anotherContext = new Context("AnotherContext");
			PropertyCollection attachedProperties = GetPropertyCollection(12, anotherContext);
			prophandler.AttachProperties(attachedProperties);

			var newprophander = (PropertyHandler)prophandler.Clone();

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
			PropertyHandler prophandler = GetPropertyHandler("modhelius");
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

		private static PropertyHandler GetPropertyHandler() {
			return GetPropertyHandler("username");
		}

		private static PropertyHandler GetPropertyHandler(string username) {
			var props = GetDefaultPropertyCollection();
			var names = GetDummyCommonNames();
			return new PropertyHandler(username, props, names);
		}

		private static PropertyCollection GetPropertyCollection(int count) {
			var propertyList = GetPropertyList(count);
			var schema = GetSchema(count);
			return new PropertyCollection(propertyList, schema);
		}

		private static PropertyCollection GetPropertyCollection(int count, Context context) {
			var propertyList = GetPropertyList(count, context);
			var schema = GetSchema(count);
			return new PropertyCollection(propertyList, schema);
		}

		private static IList<IProperty> GetPropertyList(int count) {
			return GetPropertyList(count, new Context("context"));
		}

		private static IList<IProperty> GetPropertyList(int count, Context context) {
			IList<IProperty> properties = new List<IProperty>();
			for(int i = 0; i < count; i++) {
				string name = string.Format("property{0}", i);
				var prop = new GenericProperty<string>(name, context, CultureInfo.InvariantCulture);
				prop.Value = string.Format("Value for {0}", name);
				properties.Add(prop);
			}
			return properties;
		}

		private static ContextSchema GetSchema<T>(string namePrefix, int count) {
			IList<PropertyDefinition> propertyDefinitions = new List<PropertyDefinition>();
			for(int i = 0; i < count; i++) {
				var propDef = new PropertyDefinition(namePrefix + i, typeof(GenericProperty<T>));
				propertyDefinitions.Add(propDef);
			}
			return new ContextSchema(propertyDefinitions);
		}

		private static ContextSchema GetSchema(int count) {
			return GetSchema<string>("property", count);
		}

		private static PropertyCollection GetDefaultPropertyCollection() {
			return new PropertyCollection(GetDefaultList(), GetDefaultSchema());
		}

		private static IList<IProperty>  GetDefaultList() {
			IList<IProperty> properties = new List<IProperty>();
			var context = Context.DefaultContext;
			properties.Add(new GenericProperty<string>("UserName", context, CultureInfo.InvariantCulture));
			properties.Add(new GenericProperty<string>("FullName", context, CultureInfo.InvariantCulture));
			properties.Add(new GenericProperty<string>("FirstName", context, CultureInfo.InvariantCulture));
			properties.Add(new GenericProperty<string>("LastName", context, CultureInfo.InvariantCulture));
			properties.Add(new GenericProperty<string>("Description", context, CultureInfo.InvariantCulture));
			properties.Add(new GenericProperty<string>("Email", context, CultureInfo.InvariantCulture));
			properties.Add(new GenericProperty<string>("HomePage", context, CultureInfo.InvariantCulture));
			properties.Add(new GenericProperty<string>("Address", context, CultureInfo.InvariantCulture));
			properties.Add(new GenericProperty<string>("Company", context, CultureInfo.InvariantCulture));
			properties.Add(new GenericProperty<string>("Department", context, CultureInfo.InvariantCulture));
			properties.Add(new GenericProperty<string>("City", context, CultureInfo.InvariantCulture));
			properties.Add(new GenericProperty<string>("TelephoneNumber", context, CultureInfo.InvariantCulture));
			properties.Add(new GenericProperty<string>("Fax", context, CultureInfo.InvariantCulture));
			properties.Add(new GenericProperty<string>("HomeTelephone", context, CultureInfo.InvariantCulture));
			properties.Add(new GenericProperty<string>("MobileTelephone", context, CultureInfo.InvariantCulture));
			properties.Add(new GenericProperty<string>("PostOfficeBox", context, CultureInfo.InvariantCulture));
			properties.Add(new GenericProperty<string>("ZipCode", context, CultureInfo.InvariantCulture));
			properties.Add(new GenericProperty<string>("Country", context, CultureInfo.InvariantCulture));
			properties.Add(new GenericProperty<string>("Title", context, CultureInfo.InvariantCulture));
			properties.Add(new GenericProperty<bool>("Active", context, CultureInfo.InvariantCulture));
			properties.Add(new GenericProperty<string>("PasswordQuestion", context, CultureInfo.InvariantCulture));
			properties.Add(new GenericProperty<string>("PasswordAnswer", context, CultureInfo.InvariantCulture));
			properties.Add(new GenericProperty<DateTime>("LastActivityDate", context, CultureInfo.InvariantCulture));
			properties.Add(new GenericProperty<DateTime>("CreationDate", context, CultureInfo.InvariantCulture));
			properties.Add(new GenericProperty<DateTime>("LastLockoutDate", context, CultureInfo.InvariantCulture));
			properties.Add(new GenericProperty<DateTime>("LastLoginDate", context, CultureInfo.InvariantCulture));
			properties.Add(new GenericProperty<DateTime>("LastPasswordChangedDate", context, CultureInfo.InvariantCulture));
			properties.Add(new GenericProperty<bool>("Locked", context, CultureInfo.InvariantCulture));
			properties.Add(new GenericProperty<DateTime>("LastUpdatedDate", context, CultureInfo.InvariantCulture));
			properties.Add(new GenericProperty<bool>("IsAnonymous", context, CultureInfo.InvariantCulture));
			properties.Add(new GenericProperty<string>("Password", context, CultureInfo.InvariantCulture));
			properties.Add(new GenericProperty<string>("PasswordSalt", context, CultureInfo.InvariantCulture));
			return properties;			
		}

		private static ContextSchema GetDefaultSchema() {
			IList<PropertyDefinition> propertyDefinitions = new List<PropertyDefinition>();
			propertyDefinitions.Add(new PropertyDefinition("UserName", typeof(GenericProperty<string>)));
			propertyDefinitions.Add(new PropertyDefinition("FullName", typeof(GenericProperty<string>)));				
			propertyDefinitions.Add(new PropertyDefinition("FirstName", typeof(GenericProperty<string>)));
			propertyDefinitions.Add(new PropertyDefinition("LastName", typeof(GenericProperty<string>)));				
			propertyDefinitions.Add(new PropertyDefinition("Description", typeof(GenericProperty<string>)));				
			propertyDefinitions.Add(new PropertyDefinition("Email", typeof(GenericProperty<string>)));					
			propertyDefinitions.Add(new PropertyDefinition("HomePage", typeof(GenericProperty<string>)));				
			propertyDefinitions.Add(new PropertyDefinition("Address", typeof(GenericProperty<string>)));			
			propertyDefinitions.Add(new PropertyDefinition("Company", typeof(GenericProperty<string>)));					
			propertyDefinitions.Add(new PropertyDefinition("Department", typeof(GenericProperty<string>)));				
			propertyDefinitions.Add(new PropertyDefinition("City", typeof(GenericProperty<string>)));					
			propertyDefinitions.Add(new PropertyDefinition("TelephoneNumber", typeof(GenericProperty<string>)));				
			propertyDefinitions.Add(new PropertyDefinition("Fax", typeof(GenericProperty<string>)));						
			propertyDefinitions.Add(new PropertyDefinition("HomeTelephone", typeof(GenericProperty<string>)));			
			propertyDefinitions.Add(new PropertyDefinition("MobileTelephone", typeof(GenericProperty<string>)));			
			propertyDefinitions.Add(new PropertyDefinition("PostOfficeBox", typeof(GenericProperty<string>)));			
			propertyDefinitions.Add(new PropertyDefinition("ZipCode", typeof(GenericProperty<string>)));				
			propertyDefinitions.Add(new PropertyDefinition("Country", typeof(GenericProperty<string>)));					
			propertyDefinitions.Add(new PropertyDefinition("Title", typeof(GenericProperty<string>)));					
			propertyDefinitions.Add(new PropertyDefinition("Active", typeof(GenericProperty<bool>)));					
			propertyDefinitions.Add(new PropertyDefinition("PasswordQuestion", typeof(GenericProperty<string>)));		
			propertyDefinitions.Add(new PropertyDefinition("PasswordAnswer", typeof(GenericProperty<string>)));			
			propertyDefinitions.Add(new PropertyDefinition("LastActivityDate", typeof(GenericProperty<DateTime>)));		
			propertyDefinitions.Add(new PropertyDefinition("CreationDate", typeof(GenericProperty<DateTime>)));			
			propertyDefinitions.Add(new PropertyDefinition("LastLockoutDate", typeof(GenericProperty<DateTime>)));			
			propertyDefinitions.Add(new PropertyDefinition("LastLoginDate", typeof(GenericProperty<DateTime>)));			
			propertyDefinitions.Add(new PropertyDefinition("LastPasswordChangedDate", typeof(GenericProperty<DateTime>)));	
			propertyDefinitions.Add(new PropertyDefinition("Locked", typeof(GenericProperty<bool>)));					
			propertyDefinitions.Add(new PropertyDefinition("LastUpdatedDate", typeof(GenericProperty<DateTime>)));			
			propertyDefinitions.Add(new PropertyDefinition("IsAnonymous", typeof(GenericProperty<bool>)));				
			propertyDefinitions.Add(new PropertyDefinition("Password", typeof(GenericProperty<string>)));				
			propertyDefinitions.Add(new PropertyDefinition("PasswordSalt", typeof(GenericProperty<string>)));			
			return new ContextSchema(propertyDefinitions);
		}

		private static ICommonNames GetDummyCommonNames() {
				var settings =	@"<settings>
									<commonProperties>
										<userName value=""userName"" />
										<firstName value=""firstName"" />
										<fullName value=""fullName"" />
										<lastName value=""lastName"" />
										<description value=""description"" />
										<email value=""email"" />
										<homePage value=""homePage"" />
										<streetAddress value=""address"" />
										<company value=""company"" />
										<department value=""property1"" context=""anotherContext"" />
										<city value=""city"" />
										<telephone value=""telephoneNumber"" />
										<fax value=""fax"" />
										<homeTelephone value=""homeTelephone"" />
										<mobileTelephone value=""mobileTelephone"" />
										<postOfficeBox value=""postOfficeBox"" />
										<postalCode value=""zipCode"" />
										<country value=""country"" />
										<title value=""title"" />
										<passwordQuestion value=""passwordQuestion"" />
										<passwordAnswer value=""passwordAnswer"" />
										<lastActivityDate value=""lastActivityDate"" />
										<creationDate value=""creationDate"" />
										<lastLoginDate value=""lastLoginDate"" />
										<lastPasswordChangedDate value=""lastPasswordChangedDate"" />
										<locked value=""locked"" />
										<active value=""active"" />
										<lastUpdatedDate value=""lastUpdatedDate"" />
										<isAnonymous value=""isAnonymous"" />
									</commonProperties>			
								</settings>";

			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(settings);
			var config = ConfigFactory.Create("config", xmlDocument.DocumentElement);
			return CommonNamesFactory.Create(config);
		}
	}
}