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