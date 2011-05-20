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

namespace nJupiter.DataAccess.Users {
	
	public interface ICommonNames {
		string UserName { get; }
		string FullName { get; }
		string FirstName { get; }
		string LastName { get; }
		string Description { get; }
		string Email { get; }
		string HomePage { get; }
		string StreetAddress { get; }
		string Company { get; }
		string Department { get; }
		string City { get; }
		string Telephone { get; }
		string Fax { get; }
		string HomeTelephone { get; }
		string MobileTelephone { get; }
		string PostOfficeBox { get; }
		string PostalCode { get; }
		string Country { get; }
		string Title { get; }
		string Active { get; }
		string PasswordQuestion { get; }
		string PasswordAnswer { get; }
		string LastActivityDate { get; }
		string CreationDate { get; }
		string LastLockoutDate { get; }
		string LastLoginDate { get; }
		string LastPasswordChangedDate { get; }
		string Locked { get; }
		string Password { get; }
		string PasswordSalt { get; }
		string LastUpdatedDate { get; }
		string IsAnonymous { get; }
		ICommonNames ContextNames { get; }

		string GetName(string name);
		string GetContextName(string name);
		
	}
}
