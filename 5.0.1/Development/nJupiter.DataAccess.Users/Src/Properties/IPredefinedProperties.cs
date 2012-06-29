using System;

namespace nJupiter.DataAccess.Users {
	public interface IPredefinedProperties {
		string UserName { get; }
		string DisplayName { get; }
		string FullName { get; set; }
		string FirstName { get; set; }
		string LastName { get; set; }
		string Description { get; set; }
		string Email { get; set; }
		string HomePage { get; set; }
		string StreetAddress { get; set; }
		string Company { get; set; }
		string Department { get; set; }
		string City { get; set; }
		string Telephone { get; set; }
		string Fax { get; set; }
		string HomeTelephone { get; set; }
		string MobileTelephone { get; set; }
		string PostOfficeBox { get; set; }
		string PostalCode { get; set; }
		string Country { get; set; }
		string Title { get; set; }
		bool Active { get; set; }
		string PasswordQuestion { get; set; }
		string PasswordAnswer { get; set; }
		DateTime LastActivityDate { get; set; }
		DateTime CreationDate { get; set; }
		DateTime LastLockoutDate { get; set; }
		DateTime LastLoginDate { get; set; }
		DateTime LastPasswordChangedDate { get; set; }
		bool Locked { get; set; }
		DateTime LastUpdatedDate { get; set; }
		bool IsAnonymous { get; set; }
	}
}