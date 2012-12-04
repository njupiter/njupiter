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

using System;
using System.Web;

namespace nJupiter.Web.Security {
	public class FormsAuthenticationUnauthorizedStrategy : IUnauthorizedStrategy {
		
		private readonly IHttpContextHandler httpContextHandler;
		private readonly IFormsAuthentication formsAuthentication;

		public FormsAuthenticationUnauthorizedStrategy() : this((IHttpContextHandler)null) {}

		public FormsAuthenticationUnauthorizedStrategy(Func<HttpContextBase> httpContextFactoryMethod)
			: this(new HttpContextHandler(httpContextFactoryMethod), null) {
		}
		
		public FormsAuthenticationUnauthorizedStrategy(Func<HttpContextBase> httpContextFactoryMethod, IFormsAuthentication formsAuthentication)
			: this(new HttpContextHandler(httpContextFactoryMethod), formsAuthentication) {
		}
		
		public FormsAuthenticationUnauthorizedStrategy(IHttpContextHandler httpContextHandler)
			: this(httpContextHandler, null) {
		}

		public FormsAuthenticationUnauthorizedStrategy(IHttpContextHandler httpContextHandler, IFormsAuthentication formsAuthentication) {
			this.httpContextHandler = httpContextHandler ?? HttpContextHandler.Instance;
			this.formsAuthentication = formsAuthentication ?? FormsAuthenticationWrapper.Instance;
		}

		public void Execute() {
			var redirectUrl = GetRedirectUrl();
			httpContextHandler.Current.Response.Redirect(redirectUrl, true);
		}

		private string GetRedirectUrl() {
			var returnUrl = GetReturnUrl();
			return string.Format("{0}?ReturnUrl={1}", formsAuthentication.LoginUrl, returnUrl);
		}

		private string GetReturnUrl() {
			var returnUrl = GetPathAndQuery();
			return httpContextHandler.Current.Server.UrlEncode(returnUrl);
		}

		private string GetPathAndQuery() {
			if(	httpContextHandler.Current != null &&
				httpContextHandler.Current.Request.Url != null) {
				return httpContextHandler.Current.Request.Url.PathAndQuery;
			}
			return "/";
		}
	}
}