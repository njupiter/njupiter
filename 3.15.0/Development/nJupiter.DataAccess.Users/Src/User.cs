#region Copyright & License
/*
	Copyright (c) 2005-2010 nJupiter

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

namespace nJupiter.DataAccess.Users {
	// TODO: Implement clonable
	[Serializable]
	public class User {

		#region Members
		private readonly	ContextualPropertyCollectionTable	contextProperties;
		private readonly	Properties							properties;
		private readonly	string								id;
		private readonly	string								domain;
		private	readonly	string								userName;
		private				PropertyCollection					globalProperties;
		private				Context[]							contexts;
		#endregion

		#region Constructors
		private User() {
			this.contextProperties = ContextualPropertyCollectionTable.Synchronized(new ContextualPropertyCollectionTable());
		}

		public User(string userId, string userName, string domain, PropertyCollection properties, CommonPropertyNames propertyNames) : this() {
			if(string.IsNullOrEmpty(userName))
				throw new UserNameEmptyException("User name can not be empty.");
			this.id							= userId;
			this.domain						= (domain ?? string.Empty);
			this.globalProperties			= properties;
			this.properties					= new Properties(this, propertyNames);
			this.properties.CreationDate	= this.properties.CreationDate > DateTime.MinValue ? this.properties.CreationDate : DateTime.UtcNow;
			this.userName					= userName;
		}
		#endregion

		#region Properties
		public	string				Id			{ get { return this.id; } }
		public	string				UserName	{ get { return this.userName; } }
		public	string				Domain		{ get { return this.domain; } }
		public	Properties			Properties	{ get { return this.properties; } }
		
		internal Context[] AttachedContexts {
			get{
				// We want the ContextualPropertyCollectionTable to be internal
				// so we copy the contexts to an array instead of make it public
				if(this.contexts == null){
					this.contexts = new Context[this.contextProperties.Count];
					this.contextProperties.CopyTo(this.contexts, 0);
				}
				return this.contexts;
			}
		}
		#endregion

		#region Internal Methods
		internal AbstractProperty GetProperty(string propertyName) {
			return GetProperty(propertyName, null);
		}

		internal AbstractProperty GetProperty(string propertyName, Context context) {
			if(context == null)
				return this.globalProperties[propertyName];
			if(this.contextProperties.Contains(context))
				return this.contextProperties[context][propertyName];
			return null;
		}

		public PropertyCollection GetProperties(){
			return this.globalProperties;
		}

		public PropertyCollection GetProperties(Context context){
			if(context == null)
				throw new ArgumentNullException("context");
			
			if(this.contextProperties.Contains(context))
				return this.contextProperties[context];
			return null;
		}
		#endregion

		#region Public Methods
		public bool ContainsPropertiesForContext(Context context){
			if(context == null)
				throw new ArgumentNullException("context");
			return this.contextProperties.Contains(context);
		}

		internal void AttachProperties(PropertyCollection properties) {

			if(properties == null)
				throw new ArgumentNullException("properties");
			
			if(properties.Count > 0) {
				PropertyCollection newProperties = new PropertyCollection(properties.PropertySchemas);
				Context context = null;
				foreach(AbstractProperty p in properties) {
					context = p.Context;
					break;
				}
				if(!properties.Count.Equals(properties.PropertySchemas.Count))
					throw new PropertyCollectionMismatchException("The attached PropertyCollection does not match the attached PropertySchemaTable.");
				foreach(PropertySchema pd in properties.PropertySchemas) {
					AbstractProperty newProperty = properties[pd.PropertyName];
					if(newProperty == null || newProperty.Context != context)
						throw new PropertyCollectionMismatchException("The attached PropertyCollection does not match the attached PropertySchemaTable.");
					newProperties.Add(newProperty);
				}
				if(context == null) {
					this.globalProperties = newProperties;
				} else {
					this.contextProperties.Remove(context);
					this.contextProperties.Add(context, newProperties);
					this.contexts = null; // Null array so it is rebuilt on next get
				}
			}
		}

		internal void UnattachProperties(Context context) {
			this.contextProperties.Remove(context);
			this.contexts = null;
		}

		public override int GetHashCode() {
			return this.Id.GetHashCode();
		}

		public override bool Equals(object obj) {
			User objUser = obj as User;
			return objUser != null && objUser.Id.Equals(this.Id);
		}
		#endregion
	}
}
