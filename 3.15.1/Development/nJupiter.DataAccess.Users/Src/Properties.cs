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

	[Serializable]
	public sealed class Properties {

		#region Members
		private readonly User					user;
		private readonly CommonPropertyNames	propertyNames;
		#endregion

		#region Indexers
		public AbstractProperty this[string propertyName] { get{ return this.user.GetProperty(propertyName); } }
		public AbstractProperty this[string propertyName, Context context] { get{ return this.user.GetProperty(propertyName, context); } }
		#endregion

		#region Properties
		public CommonPropertyNames PropertyNames { get { return this.propertyNames; } }

		internal Properties(User user, CommonPropertyNames propertyNames) {
			this.user = user;
			this.propertyNames = propertyNames;
		}

		public string UserName{
			get{
				if(this.propertyNames == null)
					return this.user.UserName;
				string userName = this.GetPropertyFromKey(this.propertyNames.UserName, this.propertyNames.Contexts.UserName);
				if(string.IsNullOrEmpty(userName))
					return this.user.UserName;
				return userName;
			}
		}

		public string DisplayName{
			get {
				if(!string.IsNullOrEmpty(this.FullName))
					return this.FullName;
				if(!string.IsNullOrEmpty(this.FirstName) && !string.IsNullOrEmpty(this.LastName))
					return string.Format("{0} {1}", this.FirstName, this.LastName);
				if(!string.IsNullOrEmpty(this.FirstName))
					return string.Format("{0} ({1})", this.FirstName, this.UserName);
				return this.UserName;
			}
		}
		
		public string FullName{
			get{
				if(this.propertyNames == null)
					return null;
				return this.GetPropertyFromKey(this.propertyNames.FullName, this.propertyNames.Contexts.FullName);
			}
			set{
				if(this.propertyNames == null)
					return;
				this.SetPropertyFromKey(value, this.propertyNames.FullName, this.propertyNames.Contexts.FullName);
			}
		}

		public string FirstName{
			get{
				if(this.propertyNames == null)
					return null;
				return this.GetPropertyFromKey(this.propertyNames.FirstName, this.propertyNames.Contexts.FirstName);
			}
			set{
				if(this.propertyNames == null)
					return;
				this.SetPropertyFromKey(value, this.propertyNames.FirstName, this.propertyNames.Contexts.FirstName);
			}
		}

		public string LastName{
			get{
				if(this.propertyNames == null)
					return null;
				return this.GetPropertyFromKey(this.propertyNames.LastName, this.propertyNames.Contexts.LastName);
			}
			set{
				if(this.propertyNames == null)
					return;
				this.SetPropertyFromKey(value, this.propertyNames.LastName, this.propertyNames.Contexts.LastName);
			}

		}

		public string Description{
			get{
				if(this.propertyNames == null)
					return null;
				return this.GetPropertyFromKey(this.propertyNames.Description, this.propertyNames.Contexts.Description);
			}
			set{
				if(this.propertyNames == null)
					return;
				this.SetPropertyFromKey(value, this.propertyNames.Description, this.propertyNames.Contexts.Description);
			}
		}

		public string Email{
			get{
				if(this.propertyNames == null)
					return null;
				return this.GetPropertyFromKey(this.propertyNames.Email, this.propertyNames.Contexts.Email);
			}
			set{
				if(this.propertyNames == null)
					return;
				this.SetPropertyFromKey(value, this.propertyNames.Email, this.propertyNames.Contexts.Email);
			}
		}

		public string HomePage{
			get{
				if(this.propertyNames == null)
					return null;
				return this.GetPropertyFromKey(this.propertyNames.HomePage, this.propertyNames.Contexts.HomePage);
			}
			set{
				if(this.propertyNames == null)
					return;
				this.SetPropertyFromKey(value, this.propertyNames.HomePage, this.propertyNames.Contexts.HomePage);
			}
		}

		public string StreetAddress{
			get{
				if(this.propertyNames == null)
					return null;
				return this.GetPropertyFromKey(this.propertyNames.StreetAddress, this.propertyNames.Contexts.StreetAddress);
			}
			set{
				if(this.propertyNames == null)
					return;
				this.SetPropertyFromKey(value, this.propertyNames.StreetAddress, this.propertyNames.Contexts.StreetAddress);
			}

		}

		public string Company{
			get{
				if(this.propertyNames == null)
					return null;
				return this.GetPropertyFromKey(this.propertyNames.Company, this.propertyNames.Contexts.Company);
			}
			set{
				if(this.propertyNames == null)
					return;
				this.SetPropertyFromKey(value, this.propertyNames.Company, this.propertyNames.Contexts.Company);
			}
		}
		
		public string Department{
			get{
				if(this.propertyNames == null)
					return null;
				return this.GetPropertyFromKey(this.propertyNames.Department, this.propertyNames.Contexts.Department);
			}
			set{
				if(this.propertyNames == null)
					return;
				this.SetPropertyFromKey(value, this.propertyNames.Department, this.propertyNames.Contexts.Department);
			}
		}

		public string City{
			get{
				if(this.propertyNames == null)
					return null;
				return this.GetPropertyFromKey(this.propertyNames.City, this.propertyNames.Contexts.City);
			}
			set{
				if(this.propertyNames == null)
					return;
				this.SetPropertyFromKey(value, this.propertyNames.City, this.propertyNames.Contexts.City);
			}
		}

		public string Telephone{
			get{
				if(this.propertyNames == null)
					return null;
				return this.GetPropertyFromKey(this.propertyNames.Telephone, this.propertyNames.Contexts.Telephone);
			}
			set{
				if(this.propertyNames == null)
					return;
				this.SetPropertyFromKey(value, this.propertyNames.Telephone, this.propertyNames.Contexts.Telephone);
			}
		}

		public string Fax{
			get{
				if(this.propertyNames == null)
					return null;
				return this.GetPropertyFromKey(this.propertyNames.Fax, this.propertyNames.Contexts.Fax);
			}
			set{
				if(this.propertyNames == null)
					return;
				this.SetPropertyFromKey(value, this.propertyNames.Fax, this.propertyNames.Contexts.Fax);
			}
		}


		public string HomeTelephone{
			get{
				if(this.propertyNames == null)
					return null;
				return this.GetPropertyFromKey(this.propertyNames.HomeTelephone, this.propertyNames.Contexts.HomeTelephone);
			}
			set{
				if(this.propertyNames == null)
					return;
				this.SetPropertyFromKey(value, this.propertyNames.HomeTelephone, this.propertyNames.Contexts.HomeTelephone);
			}
		}

		public string MobileTelephone{
			get{
				if(this.propertyNames == null)
					return null;
				return this.GetPropertyFromKey(this.propertyNames.MobileTelephone, this.propertyNames.Contexts.MobileTelephone);
			}
			set{
				if(this.propertyNames == null)
					return;
				this.SetPropertyFromKey(value, this.propertyNames.MobileTelephone, this.propertyNames.Contexts.MobileTelephone);
			}
		}
		
		public string PostOfficeBox{
			get{
				if(this.propertyNames == null)
					return null;
				return this.GetPropertyFromKey(this.propertyNames.PostOfficeBox, this.propertyNames.Contexts.PostOfficeBox);
			}
			set{
				if(this.propertyNames == null)
					return;
				this.SetPropertyFromKey(value, this.propertyNames.PostOfficeBox, this.propertyNames.Contexts.PostOfficeBox);
			}
		}
		
		public string PostalCode{
			get{
				if(this.propertyNames == null)
					return null;
				return this.GetPropertyFromKey(this.propertyNames.PostalCode, this.propertyNames.Contexts.PostalCode);
			}
			set{
				if(this.propertyNames == null)
					return;
				this.SetPropertyFromKey(value, this.propertyNames.PostalCode, this.propertyNames.Contexts.PostalCode);
			}
		}
		
		public string Country{
			get{
				if(this.propertyNames == null)
					return null;
				return this.GetPropertyFromKey(this.propertyNames.Country, this.propertyNames.Contexts.Country);
			}
			set{
				if(this.propertyNames == null)
					return;
				this.SetPropertyFromKey(value, this.propertyNames.Country, this.propertyNames.Contexts.Country);
			}
		}

		public string Title{
			get{
				if(this.propertyNames == null)
					return null;
				return this.GetPropertyFromKey(this.propertyNames.Title, this.propertyNames.Contexts.Title);
			}
			set{
				if(this.propertyNames == null)
					return;
				this.SetPropertyFromKey(value, this.propertyNames.Title, this.propertyNames.Contexts.Title);
			}
		}

		public bool Active{
			get{
				if(this.propertyNames == null)
					return false;
				AbstractProperty ap = this.GetAbstractPropertyFromKey(this.propertyNames.Active, this.propertyNames.Contexts.Active);
				if(ap == null || ap.Value == null)
					return false;
				return (bool)ap.Value;
			}
			set{
				if(this.propertyNames == null)
					return;
				this.SetPropertyFromKey(value, this.propertyNames.Active, this.propertyNames.Contexts.Active);
			}
		}
		
		public string PasswordQuestion {
			get{
				if(this.propertyNames == null)
					return null;
				return this.GetPropertyFromKey(this.propertyNames.PasswordQuestion, this.propertyNames.Contexts.PasswordQuestion);
			}
			set{
				if(this.propertyNames == null)
					return;
				this.SetPropertyFromKey(value, this.propertyNames.PasswordQuestion, this.propertyNames.Contexts.PasswordQuestion);
			}
		}
		
		public string PasswordAnswer{
			get{
				if(this.propertyNames == null)
					return null;
				return this.GetPropertyFromKey(this.propertyNames.PasswordAnswer, this.propertyNames.Contexts.PasswordAnswer);
			}
			set{
				if(this.propertyNames == null)
					return;
				this.SetPropertyFromKey(value, this.propertyNames.PasswordAnswer, this.propertyNames.Contexts.PasswordAnswer);
			}
		}

		public DateTime LastActivityDate{
			get{
				if(this.propertyNames == null)
					return DateTime.MinValue;
				AbstractProperty ap = this.GetAbstractPropertyFromKey(this.propertyNames.LastActivityDate, this.propertyNames.Contexts.LastActivityDate);
				if(ap == null || ap.Value == null)
					return DateTime.MinValue;
				return (DateTime)ap.Value;
			}
			set{
				if(this.propertyNames == null)
					return;
				this.SetPropertyFromKey(value, this.propertyNames.LastActivityDate, this.propertyNames.Contexts.LastActivityDate);
			}
		}

		public DateTime CreationDate {
			get{
				if(this.propertyNames == null)
					return DateTime.MinValue;
				AbstractProperty ap = this.GetAbstractPropertyFromKey(this.propertyNames.CreationDate, this.propertyNames.Contexts.CreationDate);
				if(ap == null || ap.Value == null)
					return DateTime.MinValue;
				return (DateTime)ap.Value;
			}
			set{
				if(this.propertyNames == null)
					return;
				this.SetPropertyFromKey(value, this.propertyNames.CreationDate, this.propertyNames.Contexts.CreationDate);
			}
		}

		public DateTime LastLockoutDate {
			get{
				if(this.propertyNames == null)
					return DateTime.MinValue;
				AbstractProperty ap = this.GetAbstractPropertyFromKey(this.propertyNames.LastLockoutDate, this.propertyNames.Contexts.LastLockoutDate);
				if(ap == null || ap.Value == null)
					return DateTime.MinValue;
				return (DateTime)ap.Value;
			}
			set{
				if(this.propertyNames == null)
					return;
				this.SetPropertyFromKey(value, this.propertyNames.LastLockoutDate, this.propertyNames.Contexts.LastLockoutDate);
			}
		}

		public DateTime LastLoginDate {
			get{
				if(this.propertyNames == null)
					return DateTime.MinValue;
				AbstractProperty ap = this.GetAbstractPropertyFromKey(this.propertyNames.LastLoginDate, this.propertyNames.Contexts.LastLoginDate);
				if(ap == null || ap.Value == null)
					return DateTime.MinValue;
				return (DateTime)ap.Value;
			}
			set{
				if(this.propertyNames == null)
					return;
				this.SetPropertyFromKey(value, this.propertyNames.LastLoginDate, this.propertyNames.Contexts.LastLoginDate);
			}
		}

		public DateTime LastPasswordChangedDate {
			get{
				if(this.propertyNames == null)
					return DateTime.MinValue;
				AbstractProperty ap = this.GetAbstractPropertyFromKey(this.propertyNames.LastPasswordChangedDate, this.propertyNames.Contexts.LastPasswordChangedDate);
				if(ap == null || ap.Value == null)
					return DateTime.MinValue;
				return (DateTime)ap.Value;
			}
			set{
				if(this.propertyNames == null)
					return;
				this.SetPropertyFromKey(value, this.propertyNames.LastPasswordChangedDate, this.propertyNames.Contexts.LastPasswordChangedDate);
			}
		}

		public bool Locked {
			get{
				if(this.propertyNames == null)
					return false;
				AbstractProperty ap = this.GetAbstractPropertyFromKey(this.propertyNames.Locked, this.propertyNames.Contexts.Locked);
				if(ap == null || ap.Value == null)
					return false;
				return (bool)ap.Value;
			}
			set{
				if(this.propertyNames == null)
					return;
				this.SetPropertyFromKey(value, this.propertyNames.Locked, this.propertyNames.Contexts.Locked);
			}
		}

		public DateTime LastUpdatedDate {
			get{
				if(this.propertyNames == null)
					return DateTime.MinValue;
				AbstractProperty ap = this.GetAbstractPropertyFromKey(this.propertyNames.LastUpdatedDate, this.propertyNames.Contexts.LastUpdatedDate);
				if(ap == null || ap.Value == null)
					return DateTime.MinValue;
				return (DateTime)ap.Value;
			}
			set{
				if(this.propertyNames == null)
					return;
				this.SetPropertyFromKey(value, this.propertyNames.LastUpdatedDate, this.propertyNames.Contexts.LastUpdatedDate);
			}
		}

		public bool IsAnonymous {
			get{
				if(this.propertyNames == null)
					return false;
				AbstractProperty ap = this.GetAbstractPropertyFromKey(this.propertyNames.IsAnonymous, this.propertyNames.Contexts.IsAnonymous);
				if(ap == null || ap.Value == null)
					return false;
				return (bool)ap.Value;
			}
			set{
				if(this.propertyNames == null)
					return;
				this.SetPropertyFromKey(value, this.propertyNames.IsAnonymous, this.propertyNames.Contexts.IsAnonymous);
			}
		}

		private AbstractProperty GetAbstractPropertyFromKey(string key, string contextKey) {
			if(string.IsNullOrEmpty(key))
				return null;
			AbstractProperty property = null;
			
			if(contextKey != null){
				Context context = null;
				foreach(Context c in this.user.AttachedContexts){
					if(c.Name.Equals(contextKey)){
						context = c;
						break;
					} 
				}
				if(context != null){
					property = this.user.Properties[key, context];
				}
			}else{
				property = this.user.Properties[key];
			}
			
			if(property == null || property.Value == null)
				return null;
			
			return property;
		}

		private string GetPropertyFromKey(string key, string contextKey){
			AbstractProperty property = this.GetAbstractPropertyFromKey(key, contextKey);
			if(property == null || property.Value == null)
				return null;
			return property.Value.ToString();
		}

		private void SetPropertyFromKey(object value, string key, string contextKey){
			AbstractProperty property = this.GetAbstractPropertyFromKey(key, contextKey);
			if(property == null || property.Value == null)
				return;
			property.Value = value;
		}
		#endregion

	}
}
