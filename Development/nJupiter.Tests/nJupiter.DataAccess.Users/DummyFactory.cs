using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

using nJupiter.Configuration;
using nJupiter.DataAccess.Users;

namespace nJupiter.Tests.DataAccess.Users {
	public class DummyFactory{
		internal static PropertyHandler GetPropertyHandler() {
			return GetPropertyHandler("username");
		}

		internal static PropertyHandler GetPropertyHandler(string username) {
			var props = GetDefaultPropertyCollection();
			var names = GetDummyCommonNames();
			return new PropertyHandler(username, props, names);
		}

		internal static IPropertyCollection GetPropertyCollection(int count) {
			var propertyList = GetPropertyList(count);
			var schema = GetSchema(count);
			return new PropertyCollection(propertyList, schema);
		}

		internal static IPropertyCollection GetPropertyCollection(int count, Context context) {
			var propertyList = GetPropertyList(count, context);
			var schema = GetSchema(count);
			return new PropertyCollection(propertyList, schema);
		}

		internal static IList<IProperty> GetPropertyList(int count) {
			return GetPropertyList(count, new Context("context"));
		}

		internal static IList<IProperty> GetPropertyList(int count, Context context) {
			IList<IProperty> properties = new List<IProperty>();
			for(int i = 0; i < count; i++) {
				string name = String.Format("property{0}", i);
				var prop = new GenericProperty<string>(name, context, CultureInfo.InvariantCulture);
				prop.Value = String.Format("Value for {0}", name);
				properties.Add(prop);
			}
			return properties;
		}

		internal static ContextSchema GetSchema<T>(string namePrefix, int count) {
			IList<PropertyDefinition> propertyDefinitions = new List<PropertyDefinition>();
			for(int i = 0; i < count; i++) {
				var propDef = new PropertyDefinition(namePrefix + i, typeof(GenericProperty<T>));
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

		internal static ContextSchema GetDefaultSchema() {
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

		internal static ICommonNames GetDummyCommonNames() {
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