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

namespace nJupiter.DataAccess.Users {

	[Serializable]
	public sealed class CommonPropertyNames {

		private readonly string			userName;
		private readonly string			fullName;
		private readonly string			firstName;
		private readonly string			lastName;
		private readonly string			description;
		private readonly string			email;
		private readonly string			homePage;
		private readonly string			streetAddress;
		private readonly string			company;
		private readonly string			department;
		private readonly string			city;
		private readonly string			telephone;
		private readonly string			fax;
		private readonly string			homeTelephone;
		private readonly string			mobileTelephone;
		private readonly string			postOfficeBox;
		private readonly string			postalCode;
		private readonly string			country;
		private readonly string			title;
		private readonly string			active;
		private readonly string			passwordQuestion;
		private readonly string			passwordAnswer;
		private readonly string			lastActivityDate;
		private readonly string			creationDate;			
		private readonly string			lastLockoutDate;		
		private readonly string			lastLoginDate;			
		private readonly string			lastPasswordChangedDate;
		private readonly string			locked;
		private readonly string			password;
		private readonly string			passwordSalt;
		private readonly string			lastUpdatedDate;	
		private readonly string			isAnonymous;		

		private readonly ContextNames	contexts;

		internal CommonPropertyNames(
									string userName,
									string userNameContextName,
									string fullName,
									string fullNameContextName,
									string firstName,
									string firstNameContextName,
									string lastName,
									string lastNameContextName,
									string description,
									string descriptionContextName,
									string email,
									string emailContextName,
									string homepage,
									string homepageContextName,
									string streetaddress,
									string streetaddressContextName,
									string company,
									string companyContextName,
									string department,
									string departmentContextName,
									string city,
									string cityContextName,
									string telephone,
									string telephoneContextName,
									string fax,
									string faxContextName,
									string hometelephone,
									string hometelephoneContextName,
									string mobiletelephone,
									string mobiletelephoneContextName,
									string postofficebox,
									string postofficeboxContextName,
									string postalcode,
									string postalcodeContextName,
									string country,
									string countryContextName,
									string title,
									string titleContextName,
									string active,
									string activeContextName,
									string passwordQuestion,
									string passwordQuestionContextName,
									string passwordAnswer,
									string passwordAnswerContextName,
									string lastActivityDate,
									string lastActivityDateContextName,
									string creationDate,
									string creationDateContextName,
									string lastLockoutDate,
									string lastLockoutDateContextName,
									string lastLoginDate,
									string lastLoginDateContextName,
									string lastPasswordChangedDate,
									string lastPasswordChangedDateContextName,
									string locked,
									string lockedContextName,
									string lastUpdatedDate,
									string lastUpdatedDateContextName,
									string isAnonymous,
									string isAnonymousContextName,
									string password,
									string passwordContextName,
									string passwordSalt,
									string passwordSaltContextName) {
			
			this.userName					= userName;
			this.fullName					= fullName;
			this.firstName					= firstName;
			this.lastName					= lastName;
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
			this.password					= string.IsNullOrEmpty(password) ? "password" : password;
			this.passwordSalt				= string.IsNullOrEmpty(passwordSalt) ? "passwordSalt" : passwordSalt;
			this.contexts					= new ContextNames(
															userNameContextName,
															fullNameContextName,
															firstNameContextName,
															lastNameContextName,
															descriptionContextName,
															emailContextName,
															homepageContextName,
															streetaddressContextName,
															companyContextName,
															departmentContextName,
															cityContextName,
															telephoneContextName,
															faxContextName,
															hometelephoneContextName,
															mobiletelephoneContextName,
															postofficeboxContextName,
															postalcodeContextName,
															countryContextName,
															titleContextName,
															activeContextName,
															passwordQuestionContextName,
															passwordAnswerContextName,
															lastActivityDateContextName,
															creationDateContextName,
															lastLockoutDateContextName,
															lastLoginDateContextName,
															lastPasswordChangedDateContextName,
															lockedContextName,
															lastUpdatedDateContextName,
															isAnonymousContextName,
															passwordContextName,
															passwordSaltContextName);
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
		public string		Password				{ get { return this.password;					} }
		public string		PasswordSalt			{ get { return this.passwordSalt;				} }
		public string		LastUpdatedDate			{ get { return this.lastUpdatedDate;			} }
		public string		IsAnonymous				{ get { return this.isAnonymous;				} }
		public ContextNames	Contexts				{ get { return this.contexts;					} }

		public string GetContextName(string propertyName) {
			if(propertyName == null)
				throw new ArgumentNullException("propertyName");
			if(propertyName == this.UserName)
				return this.Contexts.UserName;
			if(propertyName == this.FullName)
				return this.Contexts.FullName;
			if(propertyName == this.FirstName)
				return this.Contexts.FirstName;
			if(propertyName == this.LastName)
				return this.Contexts.LastName;
			if(propertyName == this.Description)
				return this.Contexts.Description;
			if(propertyName == this.Email)
				return this.Contexts.Email;
			if(propertyName == this.HomePage)
				return this.Contexts.HomePage;
			if(propertyName == this.StreetAddress)
				return this.Contexts.StreetAddress;
			if(propertyName == this.Company)
				return this.Contexts.Company;
			if(propertyName == this.Department)
				return this.Contexts.Department;
			if(propertyName == this.City)
				return this.Contexts.City;
			if(propertyName == this.Telephone)
				return this.Contexts.Telephone;
			if(propertyName == this.Fax)
				return this.Contexts.Fax;
			if(propertyName == this.HomeTelephone)
				return this.Contexts.HomeTelephone;
			if(propertyName == this.MobileTelephone)
				return this.Contexts.MobileTelephone;
			if(propertyName == this.PostOfficeBox)
				return this.Contexts.PostOfficeBox;
			if(propertyName == this.PostalCode)
				return this.Contexts.PostalCode;
			if(propertyName == this.Country)
				return this.Contexts.Country;
			if(propertyName == this.Title)
				return this.Contexts.Title;
			if(propertyName == this.Active)
				return this.Contexts.Active;
			if(propertyName == this.PasswordQuestion)
				return this.Contexts.PasswordQuestion;
			if(propertyName == this.PasswordAnswer)
				return this.Contexts.PasswordAnswer;
			if(propertyName == this.LastActivityDate)
				return this.Contexts.LastActivityDate;
			if(propertyName == this.CreationDate)
				return this.Contexts.CreationDate;
			if(propertyName == this.LastLockoutDate)
				return this.Contexts.LastLockoutDate;
			if(propertyName == this.LastLoginDate)
				return this.Contexts.LastLoginDate;
			if(propertyName == this.LastPasswordChangedDate)
				return this.Contexts.LastPasswordChangedDate;
			if(propertyName == this.Locked)
				return this.Contexts.Locked;
			if(propertyName == this.Password)
				return this.Contexts.Password;
			if(propertyName == this.PasswordSalt)
				return this.Contexts.PasswordSalt;
			if(propertyName == this.LastUpdatedDate)
				return this.Contexts.LastUpdatedDate;
			if(propertyName == this.IsAnonymous)
				return this.Contexts.IsAnonymous;
			return null;
		}

		[Serializable]
		public sealed class ContextNames{
			private readonly string userNameContextName;
			private readonly string fullNameContextName;
			private readonly string firstNameContextName;
			private readonly string lastNameContextName;
			private readonly string descriptionContextName;
			private readonly string emailContextName;
			private readonly string homePageContextName;
			private readonly string streetAddressContextName;
			private readonly string companyContextName;
			private readonly string departmentContextName;
			private readonly string cityContextName;
			private readonly string telephoneContextName;
			private readonly string faxContextName;
			private readonly string homeTelephoneContextName;
			private readonly string mobileTelephoneContextName;
			private readonly string postOfficeBoxContextName;
			private readonly string postalCodeContextName;
			private readonly string countryContextName;
			private readonly string titleContextName;
			private readonly string activeContextName;
			private readonly string passwordQuestionContextName;
			private readonly string passwordAnswerContextName;
			private readonly string lastActivityDateContextName;
			private readonly string creationDateContextName;
			private readonly string lastLockoutDateContextName;
			private readonly string lastLoginDateContextName;
			private readonly string lastPasswordChangedDateContextName;
			private readonly string lockedContextName;
			private readonly string lastUpdatedDateContextName;
			private readonly string isAnonymousContextName;
			private readonly string passwordContextName;
			private readonly string passwordSaltContextName;

			internal ContextNames(
								string usernameContextName,
								string fullnameContextName,
								string firstnameContextName,
								string lastnameContextName,
								string descriptionContextName,
								string emailContextName,
								string homepageContextName,
								string streetaddressContextName,
								string companyContextName,
								string departmentContextName,
								string cityContextName,
								string telephoneContextName,
								string faxContextName,
								string hometelephoneContextName,
								string mobiletelephoneContextName,
								string postofficeboxContextName,
								string postalcodeContextName,
								string countryContextName,
								string titleContextName,
								string activeContextName,
								string passwordQuestionContextName,
								string passwordAnswerContextName,
								string lastActivityDateContextName,
								string creationDateContextName,
								string lastLockoutDateContextName,
								string lastLoginDateContextName,
								string lastPasswordChangedDateContextName,
								string lockedContextName,
								string lastUpdatedDateContextName,
								string isAnonymousContextName,
								string passwordContextName,
								string passwordSaltContextName) {
			
				this.userNameContextName					= usernameContextName;
				this.fullNameContextName					= fullnameContextName;
				this.firstNameContextName					= firstnameContextName;
				this.lastNameContextName					= lastnameContextName;
				this.descriptionContextName					= descriptionContextName;
				this.emailContextName						= emailContextName;
				this.homePageContextName					= homepageContextName;
				this.streetAddressContextName				= streetaddressContextName;
				this.companyContextName						= companyContextName;
				this.departmentContextName					= departmentContextName;
				this.cityContextName						= cityContextName;
				this.telephoneContextName					= telephoneContextName;
				this.faxContextName							= faxContextName;
				this.homeTelephoneContextName				= hometelephoneContextName;
				this.mobileTelephoneContextName				= mobiletelephoneContextName;
				this.postOfficeBoxContextName				= postofficeboxContextName;
				this.postalCodeContextName					= postalcodeContextName;
				this.countryContextName						= countryContextName;
				this.titleContextName						= titleContextName;
				this.activeContextName						= activeContextName;
				this.passwordQuestionContextName			= passwordQuestionContextName;
				this.passwordAnswerContextName				= passwordAnswerContextName;
				this.lastActivityDateContextName			= lastActivityDateContextName;
				this.creationDateContextName				= creationDateContextName;
				this.lastLockoutDateContextName				= lastLockoutDateContextName;
				this.lastLoginDateContextName				= lastLoginDateContextName;
				this.lastPasswordChangedDateContextName		= lastPasswordChangedDateContextName;
				this.lockedContextName						= lockedContextName;
				this.lastUpdatedDateContextName				= lastUpdatedDateContextName;
				this.isAnonymousContextName					= isAnonymousContextName;
				this.passwordContextName					= passwordContextName;
				this.passwordSaltContextName				= passwordSaltContextName;
			}

			public string	UserName				{ get { return this.userNameContextName;					} }
			public string	FullName				{ get { return this.fullNameContextName;					} }
			public string	FirstName				{ get { return this.firstNameContextName;					} }
			public string	LastName				{ get { return this.lastNameContextName;					} }
			public string	Description				{ get { return this.descriptionContextName;					} }
			public string	Email					{ get { return this.emailContextName;						} }
			public string	HomePage				{ get { return this.homePageContextName;					} }
			public string	StreetAddress			{ get { return this.streetAddressContextName;				} }
			public string	Company					{ get { return this.companyContextName;						} }
			public string	Department				{ get { return this.departmentContextName;					} }
			public string	City					{ get { return this.cityContextName;						} }
			public string	Telephone				{ get { return this.telephoneContextName;					} }
			public string	Fax						{ get { return this.faxContextName;							} }
			public string	HomeTelephone			{ get { return this.homeTelephoneContextName;				} }
			public string	MobileTelephone			{ get { return this.mobileTelephoneContextName;				} }
			public string	PostOfficeBox			{ get { return this.postOfficeBoxContextName;				} }
			public string	PostalCode				{ get { return this.postalCodeContextName;					} }
			public string	Country					{ get { return this.countryContextName;						} }
			public string	Title					{ get { return this.titleContextName;						} }
			public string	Active					{ get { return this.activeContextName;						} }
			public string	PasswordQuestion		{ get { return this.passwordQuestionContextName;			} }
			public string	PasswordAnswer			{ get { return this.passwordAnswerContextName;				} }
			public string	LastActivityDate		{ get { return this.lastActivityDateContextName;			} }
			public string	CreationDate			{ get { return this.creationDateContextName;				} }
			public string	LastLockoutDate			{ get { return this.lastLockoutDateContextName;				} }
			public string	LastLoginDate			{ get { return this.lastLoginDateContextName;				} }
			public string	LastPasswordChangedDate	{ get { return this.lastPasswordChangedDateContextName;		} }
			public string	Locked					{ get { return this.lockedContextName;						} }
			public string	LastUpdatedDate			{ get { return this.lastUpdatedDateContextName;				} }
			public string	IsAnonymous				{ get { return this.isAnonymousContextName;					} }
			public string	Password				{ get { return this.passwordContextName;					} }
			public string	PasswordSalt			{ get { return this.passwordSaltContextName;				} }
		}
	}
}
