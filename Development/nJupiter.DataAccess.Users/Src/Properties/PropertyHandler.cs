#region Copyright & License
/*
	Copyright (c) 2005-2011 nJupiter

	Permission is hereby granted, free of charge, to any person obtaining a copy
	of this software and associated documentation files (the "Software"), to deal
	in the Software without restriction, including without limitation the rights
	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	copies of the Software, and to permit persons to whom the Software is
	furnished to do so, subject to the following conditions:

	The above copyright notice and this permission notice shall be included in
	all copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
	THE SOFTWARE.
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;

namespace nJupiter.DataAccess.Users {
	[Serializable]
	public sealed class PropertyHandler : IPropertyHandler {
		private readonly string username;
		private readonly IPredefinedNames propertyNames;
		private IDictionary<IContext, IPropertyCollection> propertiesPerContext;
		private readonly object padlock = new object();
		private bool isReadOnly;

		public IProperty this[string propertyName] { get { return this.GetProperty(propertyName); } }
		public IProperty this[string propertyName, IContext context] { get { return this.GetProperty(propertyName, context); } }

		public IPredefinedNames PropertyNames { get { return this.propertyNames; } }

		public IEnumerable<IContext> AttachedContexts {
			get {
				return this.propertiesPerContext.Keys.Where(context => !context.Equals(Context.DefaultContext));
			}
		}

		internal PropertyHandler(string username, IPropertyCollection properties, IPredefinedNames propertyNames) {
			this.username = username;
			this.propertyNames = propertyNames;
			this.propertiesPerContext = new Dictionary<IContext, IPropertyCollection>();
			this.AttachProperties(properties);
			if(this.CreationDate == DateTime.MinValue) {
				this.CreationDate = DateTime.UtcNow;
			}
		}

		public IPropertyCollection GetProperties() {
			return GetProperties(Context.DefaultContext);
		}

		public IPropertyCollection GetProperties(IContext context) {
			if(context == null){
				throw new ArgumentNullException("context");
			}
			if(this.propertiesPerContext.Keys.Contains(context)){
				return this.propertiesPerContext[context];
			}
			return null;
		}

		public void AttachProperties(IPropertyCollection properties) {
			if(properties == null){
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
			var porperty = this.GetProperty(propertyName, context);
			return GetValue<T>(porperty);
		}

		public T GetValue<T>(string propertyName, string contextName) {
			var porperty = this.GetProperty(propertyName, contextName);
			return GetValue<T>(porperty);
		}

		public IProperty GetProperty(string propertyName) {
			return GetProperty(propertyName, Context.DefaultContext);
		}

		public IProperty GetProperty(string propertyName, string contextName) {
			var context = GetContext(contextName);
			return this.GetProperty(propertyName, context);
		}

		public IProperty GetProperty(string propertyName, IContext context) {
			if(propertyName == null) {
				throw new ArgumentNullException("propertyName");
			}
			if(context == null) {
				throw new ArgumentNullException("context");
			}
			return propertiesPerContext[context].FirstOrDefault(p => propertyName.Equals(p.Name, StringComparison.InvariantCultureIgnoreCase));
		}

		public void SetProperty(string propertyName, object value) {
			SetProperty(propertyName, Context.DefaultContext, value);
		}

		public void SetProperty(string propertyName, string contextName, object value) {
			var context = GetContext(contextName);
			SetProperty(propertyName, context, value);
		}

		public void SetProperty(string propertyName, IContext context, object value) {
			var property = this.GetProperty(propertyName, context);
			if(property == null) {
				string incontext = Context.DefaultContext.Equals(context) ? "default context" : string.Format("context '{0}'", context.Name);
				throw new ArgumentException(string.Format("Property with name '{0}' in {1} is either not loader or does not exist.", propertyName, incontext));
			}
			property.Value = value;
		}

		public string UserName {
			get {
				string usernameFromProperty = this.GetPropertyByKey<string>("UserName");
				if(string.IsNullOrEmpty(usernameFromProperty)) {
					return username;
				}
				return usernameFromProperty;
			}
		}

		public string DisplayName {
			get {
				if(!string.IsNullOrEmpty(this.FullName)) {
					return this.FullName;
				}
				if(!string.IsNullOrEmpty(this.FirstName) && !string.IsNullOrEmpty(this.LastName)) {
					return string.Format("{0} {1}", this.FirstName, this.LastName);
				}
				if(!string.IsNullOrEmpty(this.FirstName)) {
					return string.Format("{0} ({1})", this.FirstName, this.UserName);
				}
				return this.UserName;
			}
		}

		public string FullName { get { return this.GetPropertyByKey<string>("FullName"); } set { this.SetPropertyByKey("FullName", value); } }
		public string FirstName { get { return this.GetPropertyByKey<string>("FirstName"); } set { this.SetPropertyByKey("FirstName", value); } }
		public string LastName { get { return this.GetPropertyByKey<string>("LastName"); } set { this.SetPropertyByKey("LastName", value); } }
		public string Description { get { return this.GetPropertyByKey<string>("Description"); } set { this.SetPropertyByKey("Description", value); } }
		public string Email { get { return this.GetPropertyByKey<string>("Email"); } set { this.SetPropertyByKey("Email", value); } }
		public string HomePage { get { return this.GetPropertyByKey<string>("HomePage"); } set { this.SetPropertyByKey("HomePage", value); } }
		public string StreetAddress { get { return this.GetPropertyByKey<string>("StreetAddress"); } set { this.SetPropertyByKey("StreetAddress", value); } }
		public string Company { get { return this.GetPropertyByKey<string>("Company"); } set { this.SetPropertyByKey("Company", value); } }
		public string Department { get { return this.GetPropertyByKey<string>("Department"); } set { this.SetPropertyByKey("Department", value); } }
		public string City { get { return this.GetPropertyByKey<string>("City"); } set { this.SetPropertyByKey("City", value); } }
		public string Telephone { get { return this.GetPropertyByKey<string>("Telephone"); } set { this.SetPropertyByKey("Telephone", value); } }
		public string Fax { get { return this.GetPropertyByKey<string>("Fax"); } set { this.SetPropertyByKey("Fax", value); } }
		public string HomeTelephone { get { return this.GetPropertyByKey<string>("HomeTelephone"); } set { this.SetPropertyByKey("HomeTelephone", value); } }
		public string MobileTelephone { get { return this.GetPropertyByKey<string>("MobileTelephone"); } set { this.SetPropertyByKey("MobileTelephone", value); } }
		public string PostOfficeBox { get { return this.GetPropertyByKey<string>("PostOfficeBox"); } set { this.SetPropertyByKey("PostOfficeBox", value); } }
		public string PostalCode { get { return this.GetPropertyByKey<string>("PostalCode"); } set { this.SetPropertyByKey("PostalCode", value); } }
		public string Country { get { return this.GetPropertyByKey<string>("Country"); } set { this.SetPropertyByKey("Country", value); } }
		public string Title { get { return this.GetPropertyByKey<string>("Title"); } set { this.SetPropertyByKey("Title", value); } }
		public bool Active { get { return this.GetPropertyByKey<bool>("Active"); } set { this.SetPropertyByKey("Active", value); } }
		public string PasswordQuestion { get { return this.GetPropertyByKey<string>("PasswordQuestion"); } set { this.SetPropertyByKey("PasswordQuestion", value); } }
		public string PasswordAnswer { get { return this.GetPropertyByKey<string>("PasswordAnswer"); } set { this.SetPropertyByKey("PasswordAnswer", value); } }
		public DateTime LastActivityDate { get { return this.GetPropertyByKey<DateTime>("LastActivityDate"); } set { this.SetPropertyByKey("LastActivityDate", value); } }
		public DateTime CreationDate { get { return this.GetPropertyByKey<DateTime>("CreationDate"); } set { this.SetPropertyByKey("CreationDate", value); } }
		public DateTime LastLockoutDate { get { return this.GetPropertyByKey<DateTime>("LastLockoutDate"); } set { this.SetPropertyByKey("LastLockoutDate", value); } }
		public DateTime LastLoginDate { get { return this.GetPropertyByKey<DateTime>("LastLoginDate"); } set { this.SetPropertyByKey("LastLoginDate", value); } }
		public DateTime LastPasswordChangedDate { get { return this.GetPropertyByKey<DateTime>("LastPasswordChangedDate"); } set { this.SetPropertyByKey("LastPasswordChangedDate", value); } }
		public bool Locked { get { return this.GetPropertyByKey<bool>("Locked"); } set { this.SetPropertyByKey("Locked", value); } }
		public DateTime LastUpdatedDate { get { return this.GetPropertyByKey<DateTime>("LastUpdatedDate"); } set { this.SetPropertyByKey("LastUpdatedDate", value); } }
		public bool IsAnonymous { get { return this.GetPropertyByKey<bool>("IsAnonymous"); } set { this.SetPropertyByKey("IsAnonymous", value); } }

		private static bool ValidatePropertyCollection(IPropertyCollection properties, IContext context) {
			if(!properties.Count.Equals(properties.Schema.Count)){
				return false;
			}
			if(!IsAllPropertiesInContext(properties, context)){
				return false;
			}
			if(!IsPropertiesConsistantToSchema(properties)) {
				return false;
			}
			return true;
		}

		private void AttachProperties(IPropertyCollection properties, IContext context) {
			if(this.isReadOnly) {
				properties.MakeReadOnly();
			}
			lock(this.padlock) {
				if(this.propertiesPerContext.Keys.Contains(context)) {
					throw new ArgumentException("The context for the attached PropertyCollection is already attached.");
				}
				this.propertiesPerContext.Add(context, properties);
			}
		}

		private static T GetValue<T>(IProperty porperty) {
			if(porperty == null) {
				return default(T);
			}
			return (T)porperty.Value;
		}

		private static bool IsPropertiesConsistantToSchema(IPropertyCollection properties) {
			return properties.All(p => properties.Schema.Any(definition => PropertyConformDefinition(p, definition)));
		}

		private static bool PropertyConformDefinition(IProperty property, PropertyDefinition definition) {
			return definition.PropertyName.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase) && definition.PropertyType.Equals(property.GetType());
		}

		private static bool IsAllPropertiesInContext(IEnumerable<IProperty> properties, IContext context) {
			return properties.All(property => property.Context == context);
		}

		private IContext GetContext(string contextName) {
			if(string.IsNullOrEmpty(contextName)) {
				return Context.DefaultContext;
			}
			var context = this.AttachedContexts.FirstOrDefault(c => c.Name.Equals(contextName, StringComparison.InvariantCultureIgnoreCase));
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
			if(this.propertyNames == null) {
				return default(T);
			}
			string propertyName = this.propertyNames.GetName(key);
			string contextName = this.propertyNames.ContextNames.GetName(key);
			if(propertyName == null) {
				return default(T);
			}
			return this.GetValue<T>(propertyName, contextName);
		}

		private void SetPropertyByKey(string key, object value) {
			if(this.propertyNames == null) {
				return;
			}
			string propertyName = this.propertyNames.GetName(key);
			string contextName = this.propertyNames.ContextNames.GetName(key);
			if(propertyName != null) {
				var property = this.GetProperty(propertyName, contextName);
				if(property != null) {
					property.Value = value;
				}
			}
		}
		
		public object Clone() {
			var newPropertyHandler = (PropertyHandler)this.MemberwiseClone();
			var newPropertiesPerContext = new Dictionary<IContext, IPropertyCollection>();
			foreach(var pair in propertiesPerContext) {
				newPropertiesPerContext.Add(pair.Key, (IPropertyCollection)pair.Value.Clone());
			}
			newPropertyHandler.propertiesPerContext = newPropertiesPerContext;
			newPropertyHandler.isReadOnly = false;
			return newPropertyHandler;
			
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
