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
using System.Web.UI;

using nJupiter.Web.UI.ControlAdapters;
using nJupiter.Web.UI.Controls;

namespace nJupiter.Web.UI {
	public class ClientScriptRegistrator : IClientScriptRegistrator {

		private readonly HttpContextBase context;
		private readonly IHttpContextHandler contextHandler;

		private HttpContextBase CurrentContext { get { return context ?? contextHandler.Current; } }

		public ClientScriptRegistrator() : this(null, null) {}
		public ClientScriptRegistrator(HttpContextBase context) : this(null, context) {}
		public ClientScriptRegistrator(IHttpContextHandler contextHandler) : this(contextHandler, null) {}
		public ClientScriptRegistrator(Func<HttpContextBase> httpContextFactoryMethod) : this(new HttpContextHandler(httpContextFactoryMethod)) {}
		private ClientScriptRegistrator(IHttpContextHandler contextHandler, HttpContextBase context) {
			this.contextHandler = contextHandler ?? HttpContextHandler.Instance;
			this.context = context;
		}

		public void RegisterClientScriptBlock(Type type, string key, string script, RegisterTargetPreference targetPreference) {
			if(CurrentContext != null) {
				switch(targetPreference) {
					case RegisterTargetPreference.Auto:
					case RegisterTargetPreference.ScriptHolder:
						var webScriptHolder = CurrentContext.Items[typeof(WebScriptHolder)] as WebScriptHolder;
						if(webScriptHolder != null) {
							webScriptHolder.RegisterClientScriptBlock(type, key, script);
							break;
						}
						goto case RegisterTargetPreference.Head;
					case RegisterTargetPreference.Head:
						var htmlHeadAdapter = CurrentContext.Items[typeof(HtmlHeadAdapter)] as HtmlHeadAdapter;
						if(htmlHeadAdapter != null) {
							htmlHeadAdapter.RegisterClientScriptBlock(type, key, script);
							break;
						}
						var webHead = CurrentContext.Items[typeof(WebHead)] as WebHead;
						if(webHead != null) {
							webHead.RegisterClientScriptBlock(type, key, script);
							break;
						}
						goto case RegisterTargetPreference.Page;
					case RegisterTargetPreference.Page:
						var page = CurrentContext.CurrentHandler as Page;
						if(page != null)
							page.ClientScript.RegisterClientScriptBlock(type, key, script);
						break;
				}
			}
		}
		
		public void RegisterClientScriptBlock(Type type, string key, string script) {
			RegisterClientScriptBlock(type, key, script, RegisterTargetPreference.Auto);
		}
		
		public void RegisterClientScriptInclude(Type type, string key, string url, RegisterTargetPreference targetPreference) {
			RegisterClientScriptBlock(type, key, string.Format(CultureInfo.InvariantCulture, "<script type=\"text/javascript\" src=\"{0}\"></script>", HttpUtility.HtmlAttributeEncode(url)), targetPreference);
		}
		
		public void RegisterClientScriptInclude(Type type, string key, string url) {
			RegisterClientScriptInclude(type, key, url, RegisterTargetPreference.Auto);
		}
		
		public void RegisterClientScriptResource(Type type, string resourceName, RegisterTargetPreference targetPreference) {
			var page = CurrentContext.CurrentHandler as Page;
			if(page != null) {
				var url = page.ClientScript.GetWebResourceUrl(type, resourceName);
				RegisterClientScriptInclude(type, resourceName, url, targetPreference);
			}
		}
		
		public void RegisterClientScriptResource(Type type, string resourceName) {
			RegisterClientScriptResource(type, resourceName, RegisterTargetPreference.Auto);
		}

		public static IClientScriptRegistrator Instance { get { return NestedSingleton.instance; } }

		// thread safe Singleton implementation with fully lazy instantiation and with full performance
		private sealed class NestedSingleton {
			// ReSharper disable EmptyConstructor
			static NestedSingleton() {} // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
			// ReSharper restore EmptyConstructor
			internal static readonly IClientScriptRegistrator instance = new ClientScriptRegistrator();
		}


		
	}
}