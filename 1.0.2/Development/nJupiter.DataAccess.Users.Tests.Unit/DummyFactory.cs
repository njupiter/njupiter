using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

using nJupiter.Configuration;
using nJupiter.DataAccess.Users;

namespace nJupiter.DataAccess.Users.Tests.Unit {
	public class DummyFactory{
		internal static IPropertyHandler GetPropertyHandler() {
			return GetPropertyHandler("username");
		}

		internal static IPropertyHandler GetPropertyHandler(string username) {
			var props = GetDefaultPropertyCollection();
			var names = GetDummyCommonNames();
			return new PropertyHandler(username, props, names);
		}

		internal static IPropertyCollection GetPropertyCollection(int count) {
			var propertyList = GetPropertyList(count);
			var schema = GetSchema(count);
			return new PropertyCollection(propertyList, schema);
		}

		internal static IPropertyCollection GetPropertyCollection(int count, IContext context) {
			var propertyList = GetPropertyList(count, context);
			var schema = GetSchema(count);
			return new PropertyCollection(propertyList, schema);
		}

		internal static IList<IProperty> GetPropertyList(int count) {
			return GetPropertyList(count, new Context("context"));
		}

		internal static IList<IProperty> GetPropertyList(int count, IContext context) {
			IList<IProperty> properties = new List<IProperty>();
			for(int i = 0; i < count; i++) {
				string name = String.Format("property{0}", i);
				var prop = new Property<string>(name, context, CultureInfo.InvariantCulture);
				prop.Value = String.Format("Value for {0}", name);
				properties.Add(prop);
			}
			return properties;
		}

		internal static ContextSchema GetSchema<T>(string namePrefix, int count) {
			IList<PropertyDefinition> propertyDefinitions = new List<PropertyDefinition>();
			for(int i = 0; i < count; i++) {
				var propDef = new PropertyDefinition(namePrefix + i, typeof(Property<T>));
				propertyDefinitions.Add(propDef);
			}
			return new ContextSchema(propertyDefinitions);
		}

		internal static ContextSchema GetSchema(int count) {
			return GetSchema<string>("property", count);
		}

		internal static IPropertyCollection GetDefaultPropertyCollection() {
			return new PropertyCollection(GetDefaultList(), GetDefaultSchema());
		}

		internal static IList<IProperty>  GetDefaultList() {
			IList<IProperty> properties = new List<IProperty>();
			var context = Context.DefaultContext;
			properties.Add(new Property<string>("UserName", context, CultureInfo.InvariantCulture));
			properties.Add(new Property<string>("FullName", context, CultureInfo.InvariantCulture));
			properties.Add(new Property<string>("FirstName", context, CultureInfo.InvariantCulture));
			properties.Add(new Property<string>("LastName", context, CultureInfo.InvariantCulture));
			properties.Add(new Property<string>("Description", context, CultureInfo.InvariantCulture));
			properties.Add(new Property<string>("Email", context, CultureInfo.InvariantCulture));
			properties.Add(new Property<string>("HomePage", context, CultureInfo.InvariantCulture));
			properties.Add(new Property<string>("Address", context, CultureInfo.InvariantCulture));
			properties.Add(new Property<string>("Company", context, CultureInfo.InvariantCulture));
			properties.Add(new Property<string>("Department", context, CultureInfo.InvariantCulture));
			properties.Add(new Property<string>("City", context, CultureInfo.InvariantCulture));
			properties.Add(new Property<string>("TelephoneNumber", context, CultureInfo.InvariantCulture));
			properties.Add(new Property<string>("Fax", context, CultureInfo.InvariantCulture));
			properties.Add(new Property<string>("HomeTelephone", context, CultureInfo.InvariantCulture));
			properties.Add(new Property<string>("MobileTelephone", context, CultureInfo.InvariantCulture));
			properties.Add(new Property<string>("PostOfficeBox", context, CultureInfo.InvariantCulture));
			properties.Add(new Property<string>("ZipCode", context, CultureInfo.InvariantCulture));
			properties.Add(new Property<string>("Country", context, CultureInfo.InvariantCulture));
			properties.Add(new Property<string>("Title", context, CultureInfo.InvariantCulture));
			properties.Add(new Property<bool>("Active", context, CultureInfo.InvariantCulture));
			properties.Add(new Property<string>("PasswordQuestion", context, CultureInfo.InvariantCulture));
			properties.Add(new Property<string>("PasswordAnswer", context, CultureInfo.InvariantCulture));
			properties.Add(new Property<DateTime>("LastActivityDate", context, CultureInfo.InvariantCulture));
			properties.Add(new Property<DateTime>("CreationDate", context, CultureInfo.InvariantCulture));
			properties.Add(new Property<DateTime>("LastLockoutDate", context, CultureInfo.InvariantCulture));
			properties.Add(new Property<DateTime>("LastLoginDate", context, CultureInfo.InvariantCulture));
			properties.Add(new Property<DateTime>("LastPasswordChangedDate", context, CultureInfo.InvariantCulture));
			properties.Add(new Property<bool>("Locked", context, CultureInfo.InvariantCulture));
			properties.Add(new Property<DateTime>("LastUpdatedDate", context, CultureInfo.InvariantCulture));
			properties.Add(new Property<bool>("IsAnonymous", context, CultureInfo.InvariantCulture));
			properties.Add(new Property<string>("Password", context, CultureInfo.InvariantCulture));
			properties.Add(new Property<string>("PasswordSalt", context, CultureInfo.InvariantCulture));
			return properties;			
		}

		internal static ContextSchema GetDefaultSchema() {
			IList<PropertyDefinition> propertyDefinitions = new List<PropertyDefinition>();
			propertyDefinitions.Add(new PropertyDefinition("UserName", typeof(Property<string>)));
			propertyDefinitions.Add(new PropertyDefinition("FullName", typeof(Property<string>)));				
			propertyDefinitions.Add(new PropertyDefinition("FirstName", typeof(Property<string>)));
			propertyDefinitions.Add(new PropertyDefinition("LastName", typeof(Property<string>)));				
			propertyDefinitions.Add(new PropertyDefinition("Description", typeof(Property<string>)));				
			propertyDefinitions.Add(new PropertyDefinition("Email", typeof(Property<string>)));					
			propertyDefinitions.Add(new PropertyDefinition("HomePage", typeof(Property<string>)));				
			propertyDefinitions.Add(new PropertyDefinition("Address", typeof(Property<string>)));			
			propertyDefinitions.Add(new PropertyDefinition("Company", typeof(Property<string>)));					
			propertyDefinitions.Add(new PropertyDefinition("Department", typeof(Property<string>)));				
			propertyDefinitions.Add(new PropertyDefinition("City", typeof(Property<string>)));					
			propertyDefinitions.Add(new PropertyDefinition("TelephoneNumber", typeof(Property<string>)));				
			propertyDefinitions.Add(new PropertyDefinition("Fax", typeof(Property<string>)));						
			propertyDefinitions.Add(new PropertyDefinition("HomeTelephone", typeof(Property<string>)));			
			propertyDefinitions.Add(new PropertyDefinition("MobileTelephone", typeof(Property<string>)));			
			propertyDefinitions.Add(new PropertyDefinition("PostOfficeBox", typeof(Property<string>)));			
			propertyDefinitions.Add(new PropertyDefinition("ZipCode", typeof(Property<string>)));				
			propertyDefinitions.Add(new PropertyDefinition("Country", typeof(Property<string>)));					
			propertyDefinitions.Add(new PropertyDefinition("Title", typeof(Property<string>)));					
			propertyDefinitions.Add(new PropertyDefinition("Active", typeof(Property<bool>)));					
			propertyDefinitions.Add(new PropertyDefinition("PasswordQuestion", typeof(Property<string>)));		
			propertyDefinitions.Add(new PropertyDefinition("PasswordAnswer", typeof(Property<string>)));			
			propertyDefinitions.Add(new PropertyDefinition("LastActivityDate", typeof(Property<DateTime>)));		
			propertyDefinitions.Add(new PropertyDefinition("CreationDate", typeof(Property<DateTime>)));			
			propertyDefinitions.Add(new PropertyDefinition("LastLockoutDate", typeof(Property<DateTime>)));			
			propertyDefinitions.Add(new PropertyDefinition("LastLoginDate", typeof(Property<DateTime>)));			
			propertyDefinitions.Add(new PropertyDefinition("LastPasswordChangedDate", typeof(Property<DateTime>)));	
			propertyDefinitions.Add(new PropertyDefinition("Locked", typeof(Property<bool>)));					
			propertyDefinitions.Add(new PropertyDefinition("LastUpdatedDate", typeof(Property<DateTime>)));			
			propertyDefinitions.Add(new PropertyDefinition("IsAnonymous", typeof(Property<bool>)));				
			propertyDefinitions.Add(new PropertyDefinition("Password", typeof(Property<string>)));				
			propertyDefinitions.Add(new PropertyDefinition("PasswordSalt", typeof(Property<string>)));			
			return new ContextSchema(propertyDefinitions);
		}

		internal static IPredefinedNames GetDummyCommonNames() {
			var settings =	@"<settings>
									<predefinedProperties>
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
									</predefinedProperties>			
								</settings>";

			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(settings);
			var config = ConfigFactory.Create("config", xmlDocument.DocumentElement);
			return PredefinedNamesFactory.Create(config);
		}
	}
}