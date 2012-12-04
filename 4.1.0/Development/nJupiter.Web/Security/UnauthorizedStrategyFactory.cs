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
using System.Web.Configuration;

namespace nJupiter.Web.Security {
	public class UnauthorizedStrategyFactory : IUnauthorizedStrategyFactory {
		private readonly IHttpContextHandler httpContextHandler;
		private readonly IAuthenticationConfigurationLoader authenticationConfigurationLoader;

		public UnauthorizedStrategyFactory() : this(null, null) {}
		public UnauthorizedStrategyFactory(Func<HttpContextBase> httpContextFactoryMethod) : this(new HttpContextHandler(httpContextFactoryMethod)) {}
		public UnauthorizedStrategyFactory(IHttpContextHandler httpContextHandler)
			: this(httpContextHandler, null) {
		}

		internal UnauthorizedStrategyFactory(	IHttpContextHandler httpContextHandler,
												IAuthenticationConfigurationLoader authenticationConfigurationLoader) {
			this.httpContextHandler = httpContextHandler ?? HttpContextHandler.Instance;
			this.authenticationConfigurationLoader = authenticationConfigurationLoader ?? new AuthenticationConfigurationLoader();
		}

		public IUnauthorizedStrategy Create() {
			var authenticationConfiguration = authenticationConfigurationLoader.Load();
			if(authenticationConfiguration.Mode == AuthenticationMode.Forms) {
				return new FormsAuthenticationUnauthorizedStrategy(httpContextHandler);
			}
			return new GenericUnauthorizedStrategy(httpContextHandler);
		}

	}
}