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
using System.Globalization;
using System.Web;

namespace nJupiter.Web.UI {
	public class UserAgent : IUserAgent {
		
		private readonly string currentUserAgent;
		private readonly HttpContextBase context;
		private readonly IHttpContextHandler contextHandler;

		private HttpContextBase HttpContext { get { return context ?? contextHandler.Current; } }

		public UserAgent() : this(null, null, null) {}
		public UserAgent(HttpContextBase context) : this(null, null, context) {}
		public UserAgent(IHttpContextHandler contextHandler) : this(null, contextHandler, null) {}
		public UserAgent(Func<HttpContextBase> httpContextFactoryMethod) : this(new HttpContextHandler(httpContextFactoryMethod)) {}
		public UserAgent(string currentUserAgent) : this(currentUserAgent, null, null) {}

		private UserAgent(string currentUserAgent, IHttpContextHandler contextHandler, HttpContextBase context) {
			this.contextHandler = contextHandler ?? HttpContextHandler.Instance;
			this.context = context;
			this.currentUserAgent = currentUserAgent;
		}

		public bool IsIE { get { return !string.IsNullOrEmpty(CurrentUserAgent) && CurrentUserAgent.IndexOf("MSIE", StringComparison.Ordinal) > 0; } }

		public bool IsPreIE7 {
			get {
	
				if(!string.IsNullOrEmpty(CurrentUserAgent)) {
					var i = CurrentUserAgent.IndexOf("MSIE", StringComparison.Ordinal);
					if(i > 0) {
						i = i + 5;
						if(CurrentUserAgent.Length > i) {
							var versionString = CurrentUserAgent.Substring(i, 1);
							try {
								var version = int.Parse(versionString, NumberFormatInfo.InvariantInfo);
								if(version < 7) {
									return true;
								}
							} catch(FormatException) { }
						}
					}
				}
				return false;
			}
		}

		private string CurrentUserAgent {
			get {
				if(!string.IsNullOrEmpty(currentUserAgent)) {
					return currentUserAgent;
				}
				if(HttpContext != null && HttpContext.Request != null && HttpContext.Request.UserAgent != null) {
					return HttpContext.Request.UserAgent;
				}
				return string.Empty;
			}
		}

		public override string ToString() {
			return CurrentUserAgent;
		}

		public static IUserAgent Instance { get { return NestedSingleton.instance; } }

		// thread safe Singleton implementation with fully lazy instantiation and with full performance
		private sealed class NestedSingleton {
			// ReSharper disable EmptyConstructor
			static NestedSingleton() {} // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
			// ReSharper restore EmptyConstructor
			internal static readonly IUserAgent instance = new UserAgent();
		}
	}
}