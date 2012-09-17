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

namespace nJupiter.Web {
	public class HttpContextHandler : IHttpContextHandler {

		public Func<HttpContextBase> HttpContextFactoryMethod { get; set; }

		public HttpContextHandler() {}

		public HttpContextHandler(Func<HttpContextBase> httpContextFactoryMethod) {
			this.HttpContextFactoryMethod = httpContextFactoryMethod;
		}

		public virtual HttpContextBase Current {
			get {
				if(HttpContext.Current != null) {
					var currentContext = HttpContext.Current.Items[this] as HttpContextBase;
					if(currentContext == null) {
						currentContext = HttpContextFactoryMethod != null ? HttpContextFactoryMethod() : CreateHttpContextBase();
						HttpContext.Current.Items[this] = currentContext;
					}
					return currentContext;

				}
				return null;
			}
		}

		protected virtual HttpContextBase CreateHttpContextBase() {
			return new HttpContextWrapper(HttpContext.Current);
		}

		public static IHttpContextHandler Instance { get { return NestedSingleton.instance; } }

		// thread safe Singleton implementation with fully lazy instantiation and with full performance
		private sealed class NestedSingleton {
			// ReSharper disable EmptyConstructor
			static NestedSingleton() {} // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
			// ReSharper restore EmptyConstructor
			internal static readonly IHttpContextHandler instance = new HttpContextHandler();
		}
	}
}
