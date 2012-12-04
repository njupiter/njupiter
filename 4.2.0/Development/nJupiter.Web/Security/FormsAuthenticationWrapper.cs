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
	public class FormsAuthenticationWrapper : IFormsAuthentication {

		public string HashPasswordForStoringInConfigFile(string password, string passwordFormat) {
			return FormsAuthentication.HashPasswordForStoringInConfigFile(password, passwordFormat);
		}

		public FormsAuthenticationTicket Decrypt(string encryptedTicket) {
			return FormsAuthentication.Decrypt(encryptedTicket);
		}

		public string Encrypt(FormsAuthenticationTicket ticket) {
			return FormsAuthentication.Encrypt(ticket);
		}
		
		public bool Authenticate(string name, string password) {
			return FormsAuthentication.Authenticate(name, password);
		}
		
		public void SignOut() {
			FormsAuthentication.SignOut();
		}
		
		public void SetAuthCookie(string userName, bool createPersistentCookie) {
			FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
		}
		
		public void SetAuthCookie(string userName, bool createPersistentCookie, string strCookiePath) {
			FormsAuthentication.SetAuthCookie(userName, createPersistentCookie, strCookiePath);
		}
		
		public HttpCookie GetAuthCookie(string userName, bool createPersistentCookie) {
			return FormsAuthentication.GetAuthCookie(userName, createPersistentCookie);
		}

		public HttpCookie GetAuthCookie(string userName, bool createPersistentCookie, string strCookiePath) {
			return FormsAuthentication.GetAuthCookie(userName, createPersistentCookie, strCookiePath);
		}

		public string GetRedirectUrl(string userName, bool createPersistentCookie) {
			return FormsAuthentication.GetRedirectUrl(userName, createPersistentCookie);
		}
		
		public void RedirectFromLoginPage(string userName, bool createPersistentCookie) {
			FormsAuthentication.RedirectFromLoginPage(userName, createPersistentCookie);
		}
		
		public void RedirectFromLoginPage(string userName, bool createPersistentCookie, string strCookiePath) {
			FormsAuthentication.RedirectFromLoginPage(userName, createPersistentCookie, strCookiePath);
		}

		public FormsAuthenticationTicket RenewTicketIfOld(FormsAuthenticationTicket tOld) {
			return FormsAuthentication.RenewTicketIfOld(tOld);
		}

		public string FormsCookieName { get { return FormsAuthentication.FormsCookieName; }}
 
		public string FormsCookiePath { get { return FormsAuthentication.FormsCookiePath; }} 

		public bool   RequireSSL { get { return FormsAuthentication.RequireSSL; }} 

		public bool   SlidingExpiration { get { return FormsAuthentication.SlidingExpiration; }}

		public HttpCookieMode CookieMode { get { return FormsAuthentication.CookieMode; }} 

		public string CookieDomain { get { return FormsAuthentication.CookieDomain; } } 
 
		public bool EnableCrossAppRedirects { get { return FormsAuthentication.EnableCrossAppRedirects; } }
 
		public bool CookiesSupported { get { return FormsAuthentication.CookiesSupported; } } 

		public string LoginUrl { get { return FormsAuthentication.LoginUrl; } } 

		public string DefaultUrl { get { return FormsAuthentication.DefaultUrl; } }

		public void RedirectToLoginPage() { 
			FormsAuthentication.RedirectToLoginPage();
		}

		public void RedirectToLoginPage(string extraQueryString) {
			FormsAuthentication.RedirectToLoginPage(extraQueryString);
		}

		public static IFormsAuthentication Instance { get { return NestedSingleton.instance; } }

		// thread safe Singleton implementation with fully lazy instantiation and with full performance
		private sealed class NestedSingleton {
			// ReSharper disable EmptyConstructor
			static NestedSingleton() {} // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
			// ReSharper restore EmptyConstructor
			internal static readonly IFormsAuthentication instance = new FormsAuthenticationWrapper();
		}
	}
}