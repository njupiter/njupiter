#region Copyright & License
// 
// 	Copyright (c) 2005-2012 nJupiter
// 
// 	Permission is hereby granted, free of charge, to any person obtaining a copy
// 	of this software and associated documentation files (the "Software"), to deal
// 	in the Software without restriction, including without limitation the rights
// 	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// 	copies of the Software, and to permit persons to whom the Software is
// 	furnished to do so, subject to the following conditions:
// 
// 	The above copyright notice and this permission notice shall be included in
// 	all copies or substantial portions of the Software.
// 
// 	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// 	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// 	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// 	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// 	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// 	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// 	THE SOFTWARE.
// 
#endregion

using System;
using System.Collections.Generic;
using System.Linq;

namespace nJupiter.DataAccess.Users {
	[Serializable]
	public class PropertyHandler : IPropertyHandler {
		private readonly string username;
		private readonly IPredefinedNames propertyNames;
		private IDictionary<IContext, IPropertyCollection> propertiesPerContext;
		private readonly object padlock = new object();
		private bool isReadOnly;

		public IProperty this[string propertyName] { get { return GetProperty(propertyName); } }
		public IProperty this[string propertyName, IContext context] { get { return GetProperty(propertyName, context); } }

		public IPredefinedNames PropertyNames { get { return propertyNames; } }

		public IEnumerable<IContext> AttachedContexts { get { return propertiesPerContext.Keys.Where(context => !context.Equals(Context.DefaultContext)); } }

		internal PropertyHandler(string username, IPropertyCollection properties, IPredefinedNames propertyNames) {
			this.username = username;
			this.propertyNames = propertyNames;
			propertiesPerContext = new Dictionary<IContext, IPropertyCollection>();
			AttachProperties(properties);
			if(CreationDate == DateTime.MinValue) {
				CreationDate = DateTime.UtcNow;
			}
		}

		public IPropertyCollection GetProperties() {
			return GetProperties(Context.DefaultContext);
		}

		public IPropertyCollection GetProperties(IContext context) {
			if(context == null) {
				throw new ArgumentNullException("context");
			}
			if(propertiesPerContext.Keys.Contains(context)) {
				return propertiesPerContext[context];
			}
			return null;
		}

		public void AttachProperties(IPropertyCollection properties) {
			if(properties == null) {
				throw new ArgumentNullException("properties");
			}

			var context = GetContextFromPropertyCollection(properties);
			if(!ValidatePropertyCollection(properties, context)) {
				throw new ArgumentException("The attached PropertyCollection does not match the attached Schema.");
			}
			AttachProperties(properties, context);
		}

		public T GetValue<T>(string propertyName) {
			return GetValue<T>(propertyName, Context.DefaultContext);
		}

		public T GetValue<T>(string propertyName, IContext context) {
			var porperty = GetProperty(propertyName, context);
			return GetValue<T>(porperty);
		}

		public T GetValue<T>(string propertyName, string contextName) {
			var porperty = GetProperty(propertyName, contextName);
			return GetValue<T>(porperty);
		}

		public IProperty GetProperty(string propertyName) {
			return GetProperty(propertyName, Context.DefaultContext);
		}

		public IProperty GetProperty(string propertyName, string contextName) {
			var context = GetContext(contextName);
			return GetProperty(propertyName, context);
		}

		public IProperty GetProperty(string propertyName, IContext context) {
			if(propertyName == null) {
				throw new ArgumentNullException("propertyName");
			}
			if(context == null) {
				throw new ArgumentNullException("context");
			}
			return
				propertiesPerContext[context].FirstOrDefault(
				                                             p =>
				                                             propertyName.Equals(p.Name, StringComparison.InvariantCultureIgnoreCase));
		}

		public void SetProperty(string propertyName, object value) {
			SetProperty(propertyName, Context.DefaultContext, value);
		}

		public void SetProperty(string propertyName, string contextName, object value) {
			var context = GetContext(contextName);
			SetProperty(propertyName, context, value);
		}

		public void SetProperty(string propertyName, IContext context, object value) {
			var property = GetProperty(propertyName, context);
			if(property == null) {
				var incontext = Context.DefaultContext.Equals(context)
					                   ? "default context"
					                   : string.Format("context '{0}'", context.Name);
				throw new ArgumentException(string.Format(
				                                          "Property with name '{0}' in {1} is either not loader or does not exist.",
				                                          propertyName,
				                                          incontext));
			}
			property.Value = value;
		}

		public string UserName {
			get {
				var usernameFromProperty = GetPropertyByKey<string>("UserName");
				if(string.IsNullOrEmpty(usernameFromProperty)) {
					return username;
				}
				return usernameFromProperty;
			}
		}

		public string DisplayName {
			get {
				if(!string.IsNullOrEmpty(FullName)) {
					return FullName;
				}
				if(!string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName)) {
					return string.Format("{0} {1}", FirstName, LastName);
				}
				if(!string.IsNullOrEmpty(FirstName)) {
					return string.Format("{0} ({1})", FirstName, UserName);
				}
				return UserName;
			}
		}

		public string FullName { get { return GetPropertyByKey<string>("FullName"); } set { SetPropertyByKey("FullName", value); } }
		public string FirstName { get { return GetPropertyByKey<string>("FirstName"); } set { SetPropertyByKey("FirstName", value); } }
		public string LastName { get { return GetPropertyByKey<string>("LastName"); } set { SetPropertyByKey("LastName", value); } }
		public string Description { get { return GetPropertyByKey<string>("Description"); } set { SetPropertyByKey("Description", value); } }
		public string Email { get { return GetPropertyByKey<string>("Email"); } set { SetPropertyByKey("Email", value); } }
		public string HomePage { get { return GetPropertyByKey<string>("HomePage"); } set { SetPropertyByKey("HomePage", value); } }
		public string StreetAddress { get { return GetPropertyByKey<string>("StreetAddress"); } set { SetPropertyByKey("StreetAddress", value); } }
		public string Company { get { return GetPropertyByKey<string>("Company"); } set { SetPropertyByKey("Company", value); } }
		public string Department { get { return GetPropertyByKey<string>("Department"); } set { SetPropertyByKey("Department", value); } }
		public string City { get { return GetPropertyByKey<string>("City"); } set { SetPropertyByKey("City", value); } }
		public string Telephone { get { return GetPropertyByKey<string>("Telephone"); } set { SetPropertyByKey("Telephone", value); } }
		public string Fax { get { return GetPropertyByKey<string>("Fax"); } set { SetPropertyByKey("Fax", value); } }
		public string HomeTelephone { get { return GetPropertyByKey<string>("HomeTelephone"); } set { SetPropertyByKey("HomeTelephone", value); } }
		public string MobileTelephone { get { return GetPropertyByKey<string>("MobileTelephone"); } set { SetPropertyByKey("MobileTelephone", value); } }
		public string PostOfficeBox { get { return GetPropertyByKey<string>("PostOfficeBox"); } set { SetPropertyByKey("PostOfficeBox", value); } }
		public string PostalCode { get { return GetPropertyByKey<string>("PostalCode"); } set { SetPropertyByKey("PostalCode", value); } }
		public string Country { get { return GetPropertyByKey<string>("Country"); } set { SetPropertyByKey("Country", value); } }
		public string Title { get { return GetPropertyByKey<string>("Title"); } set { SetPropertyByKey("Title", value); } }
		public bool Active { get { return GetPropertyByKey<bool>("Active"); } set { SetPropertyByKey("Active", value); } }
		public string PasswordQuestion { get { return GetPropertyByKey<string>("PasswordQuestion"); } set { SetPropertyByKey("PasswordQuestion", value); } }
		public string PasswordAnswer { get { return GetPropertyByKey<string>("PasswordAnswer"); } set { SetPropertyByKey("PasswordAnswer", value); } }
		public DateTime LastActivityDate { get { return GetPropertyByKey<DateTime>("LastActivityDate"); } set { SetPropertyByKey("LastActivityDate", value); } }
		public DateTime CreationDate { get { return GetPropertyByKey<DateTime>("CreationDate"); } set { SetPropertyByKey("CreationDate", value); } }
		public DateTime LastLockoutDate { get { return GetPropertyByKey<DateTime>("LastLockoutDate"); } set { SetPropertyByKey("LastLockoutDate", value); } }
		public DateTime LastLoginDate { get { return GetPropertyByKey<DateTime>("LastLoginDate"); } set { SetPropertyByKey("LastLoginDate", value); } }
		public DateTime LastPasswordChangedDate { get { return GetPropertyByKey<DateTime>("LastPasswordChangedDate"); } set { SetPropertyByKey("LastPasswordChangedDate", value); } }
		public bool Locked { get { return GetPropertyByKey<bool>("Locked"); } set { SetPropertyByKey("Locked", value); } }
		public DateTime LastUpdatedDate { get { return GetPropertyByKey<DateTime>("LastUpdatedDate"); } set { SetPropertyByKey("LastUpdatedDate", value); } }
		public bool IsAnonymous { get { return GetPropertyByKey<bool>("IsAnonymous"); } set { SetPropertyByKey("IsAnonymous", value); } }

		private static bool ValidatePropertyCollection(IPropertyCollection properties, IContext context) {
			if(!properties.Count.Equals(properties.Schema.Count)) {
				return false;
			}
			if(!IsAllPropertiesInContext(properties, context)) {
				return false;
			}
			if(!IsPropertiesConsistantToSchema(properties)) {
				return false;
			}
			return true;
		}

		private void AttachProperties(IPropertyCollection properties, IContext context) {
			if(isReadOnly) {
				properties.MakeReadOnly();
			}
			lock(padlock) {
				if(propertiesPerContext.Keys.Contains(context)) {
					throw new ArgumentException("The context for the attached PropertyCollection is already attached.");
				}
				propertiesPerContext.Add(context, properties);
			}
		}

		private static T GetValue<T>(IProperty property) {
			if(property == null) {
				return default(T);
			}
			return (T)property.Value;
		}

		private static bool IsPropertiesConsistantToSchema(IPropertyCollection properties) {
			return properties.All(p => properties.Schema.Any(definition => PropertyConformDefinition(p, definition)));
		}

		private static bool PropertyConformDefinition(IProperty property, PropertyDefinition definition) {
			return definition.PropertyName.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase) &&
			       definition.PropertyType == property.GetType();
		}

		private static bool IsAllPropertiesInContext(IEnumerable<IProperty> properties, IContext context) {
			return properties.All(property => property.Context == context);
		}

		private IContext GetContext(string contextName) {
			if(string.IsNullOrEmpty(contextName)) {
				return Context.DefaultContext;
			}
			var context =
				AttachedContexts.FirstOrDefault(c => c.Name.Equals(contextName, StringComparison.InvariantCultureIgnoreCase));
			if(context == null) {
				throw new ArgumentException(string.Format("The context with name '{0}' is not attached.", contextName));
			}
			return context;
		}

		private static IContext GetContextFromPropertyCollection(IEnumerable<IProperty> properties) {
			var property = properties.FirstOrDefault();
			return property != null && property.Context != null ? property.Context : Context.DefaultContext;
		}

		private T GetPropertyByKey<T>(string key) {
			if(propertyNames == null) {
				return default(T);
			}
			var propertyName = propertyNames.GetName(key);
			var contextName = propertyNames.ContextNames.GetName(key);
			if(propertyName == null) {
				return default(T);
			}
			return GetValue<T>(propertyName, contextName);
		}

		private void SetPropertyByKey(string key, object value) {
			if(propertyNames == null) {
				return;
			}
			var propertyName = propertyNames.GetName(key);
			var contextName = propertyNames.ContextNames.GetName(key);
			if(propertyName != null) {
				var property = GetProperty(propertyName, contextName);
				if(property != null) {
					property.Value = value;
				}
			}
		}

		public IPropertyHandler CreateWritable() {
			var newPropertyHandler = (PropertyHandler)MemberwiseClone();
			var newPropertiesPerContext = new Dictionary<IContext, IPropertyCollection>();
			foreach(var pair in propertiesPerContext) {
				newPropertiesPerContext.Add(pair.Key, pair.Value.CreateWritable());
			}
			newPropertyHandler.propertiesPerContext = newPropertiesPerContext;
			newPropertyHandler.isReadOnly = false;
			return newPropertyHandler;
		}

		public object Clone() {
			return CreateWritable();
		}

		public void MakeReadOnly() {
			isReadOnly = true;
			foreach(var propertyCollection in propertiesPerContext.Values) {
				propertyCollection.MakeReadOnly();
			}
		}

		public bool IsReadOnly { get { return isReadOnly; } }
	}
}