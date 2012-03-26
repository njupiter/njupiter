using System;

namespace nJupiter.DataAccess.Users {
	[Serializable]
	public sealed class PredefinedNames : IPredefinedNames {
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
			
			this.userName					= username;
			this.fullName					= fullname;
			this.firstName					= firstname;
			this.lastName					= lastname;
			this.description				= description;
			this.email						= email;
			this.homePage					= homepage;
			this.streetAddress				= streetaddress;
			this.company					= company;
			this.department					= department;
			this.city						= city;
			this.telephone					= telephone;
			this.fax						= fax;
			this.homeTelephone				= hometelephone;
			this.mobileTelephone			= mobiletelephone;
			this.postOfficeBox				= postofficebox;
			this.postalCode					= postalcode;
			this.country					= country;
			this.title						= title;
			this.active						= active;
			this.passwordQuestion			= passwordQuestion;
			this.passwordAnswer				= passwordAnswer;
			this.lastActivityDate			= lastActivityDate;
			this.creationDate				= creationDate;
			this.lastLockoutDate			= lastLockoutDate;
			this.lastLoginDate				= lastLoginDate;
			this.lastPasswordChangedDate	= lastPasswordChangedDate;
			this.locked						= locked;
			this.lastUpdatedDate			= lastUpdatedDate;
			this.isAnonymous				= isAnonymous;
			this.password					= password;
			this.passwordSalt				= passwordSalt;
			this.contextNames				= contextNames;
		}

		public string		UserName				{ get { return this.userName;					} }
		public string		FullName				{ get { return this.fullName;					} }
		public string		FirstName				{ get { return this.firstName;					} }
		public string		LastName				{ get { return this.lastName;					} }
		public string		Description				{ get { return this.description;				} }
		public string		Email					{ get { return this.email;						} }
		public string		HomePage				{ get { return this.homePage;					} }
		public string		StreetAddress			{ get { return this.streetAddress;				} }
		public string		Company					{ get { return this.company;					} }
		public string		Department				{ get { return this.department;					} }
		public string		City					{ get { return this.city;						} }
		public string		Telephone				{ get { return this.telephone;					} }
		public string		Fax						{ get { return this.fax;						} }
		public string		HomeTelephone			{ get { return this.homeTelephone;				} }
		public string		MobileTelephone			{ get { return this.mobileTelephone;			} }
		public string		PostOfficeBox			{ get { return this.postOfficeBox;				} }
		public string		PostalCode				{ get { return this.postalCode;					} }
		public string		Country					{ get { return this.country;					} }
		public string		Title					{ get { return this.title;						} }
		public string		Active					{ get { return this.active;						} }
		public string		PasswordQuestion		{ get { return this.passwordQuestion;			} }
		public string		PasswordAnswer			{ get { return this.passwordAnswer;				} }
		public string		LastActivityDate		{ get { return this.lastActivityDate;			} }
		public string		CreationDate			{ get { return this.creationDate;				} }
		public string		LastLockoutDate			{ get { return this.lastLockoutDate;			} }
		public string		LastLoginDate			{ get { return this.lastLoginDate;				} }
		public string		LastPasswordChangedDate	{ get { return this.lastPasswordChangedDate;	} }
		public string		Locked					{ get { return this.locked;						} }
		public string		LastUpdatedDate			{ get { return this.lastUpdatedDate;			} }
		public string		IsAnonymous				{ get { return this.isAnonymous;				} }
		public string		Password				{ get { return this.password;					} }
		public string		PasswordSalt			{ get { return this.passwordSalt;				} }
		
		public IPredefinedNames	ContextNames		{ get { return this.contextNames;				} }

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
			if(this.ContextNames == null || name == null) {
				return Context.DefaultContext.Name;
			}
			if(AreEqual(name, this.UserName)) {
				return this.ContextNames.UserName;
			}
			if(AreEqual(name, this.FullName)) {
				return this.ContextNames.FullName;
			}
			if(AreEqual(name, this.FirstName)) {
				return this.ContextNames.FirstName;
			}
			if(AreEqual(name, this.LastName)) {
				return this.ContextNames.LastName;
			}
			if(AreEqual(name, this.Description)) {
				return this.ContextNames.Description;
			}
			if(AreEqual(name, this.Email)) {
				return this.ContextNames.Email;
			}
			if(AreEqual(name, this.HomePage)) {
				return this.ContextNames.HomePage;
			}
			if(AreEqual(name, this.StreetAddress)) {
				return this.ContextNames.StreetAddress;
			}
			if(AreEqual(name, this.Company)) {
				return this.ContextNames.Company;
			}
			if(AreEqual(name, this.Department)) {
				return this.ContextNames.Department;
			}
			if(AreEqual(name, this.City)) {
				return this.ContextNames.City;
			}
			if(AreEqual(name, this.Telephone)) {
				return this.ContextNames.Telephone;
			}
			if(AreEqual(name, this.Fax)) {
				return this.ContextNames.Fax;
			}
			if(AreEqual(name, this.HomeTelephone)) {
				return this.ContextNames.HomeTelephone;
			}
			if(AreEqual(name, this.MobileTelephone)) {
				return this.ContextNames.MobileTelephone;
			}
			if(AreEqual(name, this.PostOfficeBox)) {
				return this.ContextNames.PostOfficeBox;
			}
			if(AreEqual(name, this.PostalCode)) {
				return this.ContextNames.PostalCode;
			}
			if(AreEqual(name, this.Country)) {
				return this.ContextNames.Country;
			}
			if(AreEqual(name, this.Title)) {
				return this.ContextNames.Title;
			}
			if(AreEqual(name, this.Active)) {
				return this.ContextNames.Active;
			}
			if(AreEqual(name, this.PasswordQuestion)) {
				return this.ContextNames.PasswordQuestion;
			}
			if(AreEqual(name, this.PasswordAnswer)) {
				return this.ContextNames.PasswordAnswer;
			}
			if(AreEqual(name, this.LastActivityDate)) {
				return this.ContextNames.LastActivityDate;
			}
			if(AreEqual(name, this.CreationDate)) {
				return this.ContextNames.CreationDate;
			}
			if(AreEqual(name, this.LastLockoutDate)) {
				return this.ContextNames.LastLockoutDate;
			}
			if(AreEqual(name, this.LastLoginDate)) {
				return this.ContextNames.LastLoginDate;
			}
			if(AreEqual(name, this.LastPasswordChangedDate)) {
				return this.ContextNames.LastPasswordChangedDate;
			}
			if(AreEqual(name, this.Locked)) {
				return this.ContextNames.Locked;
			}
			if(AreEqual(name, this.Password)) {
				return this.ContextNames.Password;
			}
			if(AreEqual(name, this.PasswordSalt)) {
				return this.ContextNames.PasswordSalt;
			}
			if(AreEqual(name, this.LastUpdatedDate)) {
				return this.ContextNames.LastUpdatedDate;
			}
			if(AreEqual(name, this.IsAnonymous)) {
				return this.ContextNames.IsAnonymous;
			}
			return Context.DefaultContext.Name;
		}
	}
}