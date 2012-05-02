#region Copyright & License
/*
	Copyright (c) 2005-2012 nJupiter

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

using System.Web;
using System.Web.Security;

namespace nJupiter.Web.Security {
	public interface IFormsAuthentication {
		string HashPasswordForStoringInConfigFile(string password, string passwordFormat);
		FormsAuthenticationTicket Decrypt(string encryptedTicket);
		string Encrypt(FormsAuthenticationTicket ticket);
		bool Authenticate(string name, string password);
		void SignOut();
		void SetAuthCookie(string userName, bool createPersistentCookie);
		void SetAuthCookie(string userName, bool createPersistentCookie, string strCookiePath);
		HttpCookie GetAuthCookie(string userName, bool createPersistentCookie);
		HttpCookie GetAuthCookie(string userName, bool createPersistentCookie, string strCookiePath);
		string GetRedirectUrl(string userName, bool createPersistentCookie);
		void RedirectFromLoginPage(string userName, bool createPersistentCookie);
		void RedirectFromLoginPage(string userName, bool createPersistentCookie, string strCookiePath);
		FormsAuthenticationTicket RenewTicketIfOld(FormsAuthenticationTicket tOld);
		string FormsCookieName { get; }
		string FormsCookiePath { get; }
		bool RequireSSL { get; }
		bool SlidingExpiration { get; }
		HttpCookieMode CookieMode { get; }
		string CookieDomain { get; }
		bool EnableCrossAppRedirects { get; }
		bool CookiesSupported { get; }
		string LoginUrl { get; }
		string DefaultUrl { get; }
		void RedirectToLoginPage();
		void RedirectToLoginPage(string extraQueryString);
	}
}