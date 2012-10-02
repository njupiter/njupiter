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

namespace nJupiter.DataAccess.Users {
	[Serializable]
	public class PredefinedNames : IPredefinedNames {
		private readonly string userName;
		private readonly string fullName;
		private readonly string firstName;
		private readonly string lastName;
		private readonly string description;
		private readonly string email;
		private readonly string homePage;
		private readonly string streetAddress;
		private readonly string company;
		private readonly string department;
		private readonly string city;
		private readonly string telephone;
		private readonly string fax;
		private readonly string homeTelephone;
		private readonly string mobileTelephone;
		private readonly string postOfficeBox;
		private readonly string postalCode;
		private readonly string country;
		private readonly string title;
		private readonly string active;
		private readonly string passwordQuestion;
		private readonly string passwordAnswer;
		private readonly string lastActivityDate;
		private readonly string creationDate;
		private readonly string lastLockoutDate;
		private readonly string lastLoginDate;
		private readonly string lastPasswordChangedDate;
		private readonly string locked;
		private readonly string lastUpdatedDate;
		private readonly string isAnonymous;
		private readonly string password;
		private readonly string passwordSalt;

		private readonly IPredefinedNames contextNames;

		public PredefinedNames(
			string username,
			string fullname,
			string firstname,
			string lastname,
			string description,
			string email,
			string homepage,
			string streetaddress,
			string company,
			string department,
			string city,
			string telephone,
			string fax,
			string hometelephone,
			string mobiletelephone,
			string postofficebox,
			string postalcode,
			string country,
			string title,
			string active,
			string passwordQuestion,
			string passwordAnswer,
			string lastActivityDate,
			string creationDate,
			string lastLockoutDate,
			string lastLoginDate,
			string lastPasswordChangedDate,
			string locked,
			string lastUpdatedDate,
			string isAnonymous,
			string password,
			string passwordSalt,
			IPredefinedNames contextNames) {
			userName = username;
			fullName = fullname;
			firstName = firstname;
			lastName = lastname;
			this.description = description;
			this.email = email;
			homePage = homepage;
			streetAddress = streetaddress;
			this.company = company;
			this.department = department;
			this.city = city;
			this.telephone = telephone;
			this.fax = fax;
			homeTelephone = hometelephone;
			mobileTelephone = mobiletelephone;
			postOfficeBox = postofficebox;
			postalCode = postalcode;
			this.country = country;
			this.title = title;
			this.active = active;
			this.passwordQuestion = passwordQuestion;
			this.passwordAnswer = passwordAnswer;
			this.lastActivityDate = lastActivityDate;
			this.creationDate = creationDate;
			this.lastLockoutDate = lastLockoutDate;
			this.lastLoginDate = lastLoginDate;
			this.lastPasswordChangedDate = lastPasswordChangedDate;
			this.locked = locked;
			this.lastUpdatedDate = lastUpdatedDate;
			this.isAnonymous = isAnonymous;
			this.password = password;
			this.passwordSalt = passwordSalt;
			this.contextNames = contextNames;
		}

		public string UserName { get { return userName; } }
		public string FullName { get { return fullName; } }
		public string FirstName { get { return firstName; } }
		public string LastName { get { return lastName; } }
		public string Description { get { return description; } }
		public string Email { get { return email; } }
		public string HomePage { get { return homePage; } }
		public string StreetAddress { get { return streetAddress; } }
		public string Company { get { return company; } }
		public string Department { get { return department; } }
		public string City { get { return city; } }
		public string Telephone { get { return telephone; } }
		public string Fax { get { return fax; } }
		public string HomeTelephone { get { return homeTelephone; } }
		public string MobileTelephone { get { return mobileTelephone; } }
		public string PostOfficeBox { get { return postOfficeBox; } }
		public string PostalCode { get { return postalCode; } }
		public string Country { get { return country; } }
		public string Title { get { return title; } }
		public string Active { get { return active; } }
		public string PasswordQuestion { get { return passwordQuestion; } }
		public string PasswordAnswer { get { return passwordAnswer; } }
		public string LastActivityDate { get { return lastActivityDate; } }
		public string CreationDate { get { return creationDate; } }
		public string LastLockoutDate { get { return lastLockoutDate; } }
		public string LastLoginDate { get { return lastLoginDate; } }
		public string LastPasswordChangedDate { get { return lastPasswordChangedDate; } }
		public string Locked { get { return locked; } }
		public string LastUpdatedDate { get { return lastUpdatedDate; } }
		public string IsAnonymous { get { return isAnonymous; } }
		public string Password { get { return password; } }
		public string PasswordSalt { get { return passwordSalt; } }

		public IPredefinedNames ContextNames { get { return contextNames; } }

		public string GetName(string name) {
			var propertyInfo = typeof(IPredefinedNames).GetProperty(name);
			if(propertyInfo != null) {
				return propertyInfo.GetValue(this, null) as string;
			}
			throw new NotSupportedException(string.Format("Property with name {0} is not supported.", name));
		}

		private bool AreEqual(string value1, string value2) {
			return string.Equals(value1, value2, StringComparison.OrdinalIgnoreCase);
		}

		public string GetContextName(string name) {
			if(ContextNames == null || name == null) {
				return Context.DefaultContext.Name;
			}
			if(AreEqual(name, UserName)) {
				return ContextNames.UserName;
			}
			if(AreEqual(name, FullName)) {
				return ContextNames.FullName;
			}
			if(AreEqual(name, FirstName)) {
				return ContextNames.FirstName;
			}
			if(AreEqual(name, LastName)) {
				return ContextNames.LastName;
			}
			if(AreEqual(name, Description)) {
				return ContextNames.Description;
			}
			if(AreEqual(name, Email)) {
				return ContextNames.Email;
			}
			if(AreEqual(name, HomePage)) {
				return ContextNames.HomePage;
			}
			if(AreEqual(name, StreetAddress)) {
				return ContextNames.StreetAddress;
			}
			if(AreEqual(name, Company)) {
				return ContextNames.Company;
			}
			if(AreEqual(name, Department)) {
				return ContextNames.Department;
			}
			if(AreEqual(name, City)) {
				return ContextNames.City;
			}
			if(AreEqual(name, Telephone)) {
				return ContextNames.Telephone;
			}
			if(AreEqual(name, Fax)) {
				return ContextNames.Fax;
			}
			if(AreEqual(name, HomeTelephone)) {
				return ContextNames.HomeTelephone;
			}
			if(AreEqual(name, MobileTelephone)) {
				return ContextNames.MobileTelephone;
			}
			if(AreEqual(name, PostOfficeBox)) {
				return ContextNames.PostOfficeBox;
			}
			if(AreEqual(name, PostalCode)) {
				return ContextNames.PostalCode;
			}
			if(AreEqual(name, Country)) {
				return ContextNames.Country;
			}
			if(AreEqual(name, Title)) {
				return ContextNames.Title;
			}
			if(AreEqual(name, Active)) {
				return ContextNames.Active;
			}
			if(AreEqual(name, PasswordQuestion)) {
				return ContextNames.PasswordQuestion;
			}
			if(AreEqual(name, PasswordAnswer)) {
				return ContextNames.PasswordAnswer;
			}
			if(AreEqual(name, LastActivityDate)) {
				return ContextNames.LastActivityDate;
			}
			if(AreEqual(name, CreationDate)) {
				return ContextNames.CreationDate;
			}
			if(AreEqual(name, LastLockoutDate)) {
				return ContextNames.LastLockoutDate;
			}
			if(AreEqual(name, LastLoginDate)) {
				return ContextNames.LastLoginDate;
			}
			if(AreEqual(name, LastPasswordChangedDate)) {
				return ContextNames.LastPasswordChangedDate;
			}
			if(AreEqual(name, Locked)) {
				return ContextNames.Locked;
			}
			if(AreEqual(name, Password)) {
				return ContextNames.Password;
			}
			if(AreEqual(name, PasswordSalt)) {
				return ContextNames.PasswordSalt;
			}
			if(AreEqual(name, LastUpdatedDate)) {
				return ContextNames.LastUpdatedDate;
			}
			if(AreEqual(name, IsAnonymous)) {
				return ContextNames.IsAnonymous;
			}
			return Context.DefaultContext.Name;
		}
	}
}