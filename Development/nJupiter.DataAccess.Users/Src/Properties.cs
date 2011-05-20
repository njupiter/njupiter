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
	public sealed class Properties {
		private readonly string username;
		private readonly ICommonNames propertyNames;
		private readonly IDictionary<Context, PropertyCollection> propertiesPerContext;

		public IProperty this[string propertyName] { get { return this.GetProperty(propertyName); } }
		public IProperty this[string propertyName, Context context] { get { return this.GetProperty(propertyName, context); } }

		public ICommonNames PropertyNames { get { return this.propertyNames; } }

		internal Properties(string username, PropertyCollection properties, ICommonNames propertyNames) {
			this.username = username;
			this.propertyNames = propertyNames;
			this.propertiesPerContext = new Dictionary<Context, PropertyCollection>();
			this.propertiesPerContext.Add(null, properties);
			if(this.CreationDate == DateTime.MinValue) {
				this.CreationDate = DateTime.UtcNow;
			}
		}

		public T GetProperty<T>(string propertyName, string contextName) {
			PropertyBase<T> ap = this.GetAbstractProperty<T>(propertyName, contextName);
			if(ap == null) {
				return default(T);
			}
			return ap.Value;
		}

		public void SetProperty<T>(string propertyName, string contextName, T value) {
			PropertyBase<T> property = this.GetAbstractProperty<T>(propertyName, contextName);
			if(property == null) {
				return;
			}
			property.Value = value;
		}

		private void SetPropertyByKey<T>(string key, T value) {
			if(this.propertyNames == null) {
				return;
			}
			string propertyName = this.propertyNames.GetName(key);
			string contextName = this.propertyNames.ContextNames.GetName(key);
			this.SetProperty(propertyName, contextName, value);
		}

		private T GetPropertyByKey<T>(string key) {
			if(this.propertyNames == null) {
				return default(T);
			}
			string propertyName = this.propertyNames.GetName(key);
			string contextName = this.propertyNames.ContextNames.GetName(key);
			return this.GetProperty<T>(propertyName, contextName);
		}

		private PropertyBase<T> GetAbstractProperty<T>(string propertyName, string contextName) {
			if(string.IsNullOrEmpty(propertyName)) {
				return null;
			}
			PropertyBase<T> property = null;

			if(contextName != null) {
				var context = this.AttachedContexts.FirstOrDefault(c => c.Name.Equals(contextName));
				if(context != null) {
					property = this.GetProperty<T>(propertyName, context);
				}
			} else {
				property = this.GetProperty<T>(propertyName);
			}

			if(property == null) {
				return null;
			}

			return property;
		}

		public IEnumerable<Context> AttachedContexts {
			get {
				return this.propertiesPerContext.Keys.Where(context => context != null);
			}
		}

		public IProperty GetProperty(string propertyName) {
			return GetProperty(propertyName, null);
		}

		public IProperty GetProperty(string propertyName, Context context) {
			if(this.propertiesPerContext.Keys.Contains(context))
				return this.propertiesPerContext[context].SingleOrDefault(p => p.Name == propertyName);
			return null;
		}

		public PropertyBase<T> GetProperty<T>(string propertyName) {
			return GetProperty<T>(propertyName, (Context)null);
		}

		public PropertyBase<T> GetProperty<T>(string propertyName, Context context) {
			if(this.propertiesPerContext.Keys.Contains(context))
				return this.propertiesPerContext[context].Where(p => p.Name == propertyName) as PropertyBase<T>;
			return null;
		}

		public PropertyCollection GetProperties() {
			return this.propertiesPerContext[null];
		}

		public PropertyCollection GetProperties(Context context) {
			if(context == null)
				throw new ArgumentNullException("context");

			if(this.propertiesPerContext.Keys.Contains(context))
				return this.propertiesPerContext[context];
			return null;
		}

		public bool ContainsPropertiesForContext(Context context) {
			if(context == null)
				throw new ArgumentNullException("context");
			return this.propertiesPerContext.Keys.Contains(context);
		}

		public void AttachProperties(PropertyCollection properties) {

			if(properties == null)
				throw new ArgumentNullException("properties");

			if(properties.Any()) {
				
				Context context = properties.First().Context;
				
				if(!properties.Count.Equals(properties.Schema.Count))
					throw new PropertyCollectionMismatchException("The attached PropertyCollection does not match the attached Schema.");

				var propertyList = new List<IProperty>();

				foreach(PropertyDefinition pd in properties.Schema) {
					IProperty newProperty = properties.FirstOrDefault(p => p.Name == pd.PropertyName);
					if(newProperty == null || newProperty.Context != context)
						throw new PropertyCollectionMismatchException("The attached PropertyCollection does not match the attached Schema.");
					propertyList.Add(newProperty);
				}
				var newProperties = new PropertyCollection(propertyList, properties.Schema);
				this.propertiesPerContext.Remove(context);
				this.propertiesPerContext.Add(context, newProperties);
			}
		}

		public string UserName {
			get {
				string userName = this.GetPropertyByKey<string>("UserName");
				if(string.IsNullOrEmpty(userName)) {
					return username;
				}
				return userName;
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
	}
}
