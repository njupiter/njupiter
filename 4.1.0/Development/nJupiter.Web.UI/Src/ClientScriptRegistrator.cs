using System;
using System.Globalization;
using System.Web;
using System.Web.UI;

using nJupiter.Web.UI.ControlAdapters;
using nJupiter.Web.UI.Controls;

namespace nJupiter.Web.UI {
	public class ClientScriptRegistrator : IClientScriptRegistrator {
		private readonly HttpContextBase httpContext;

		private HttpContextBase HttpContext { get { return httpContext ?? new HttpContextWrapper(System.Web.HttpContext.Current); } }

		public ClientScriptRegistrator() {}

		public ClientScriptRegistrator(HttpContextBase httpContext) {
			this.httpContext = httpContext;
		}

		public void RegisterClientScriptBlock(Type type, string key, string script, RegisterTargetPreference targetPreference) {
			if(HttpContext != null) {
				switch(targetPreference) {
					case RegisterTargetPreference.Auto:
					case RegisterTargetPreference.ScriptHolder:
						var webScriptHolder = HttpContext.Items[typeof(WebScriptHolder)] as WebScriptHolder;
						if(webScriptHolder != null) {
							webScriptHolder.RegisterClientScriptBlock(type, key, script);
							break;
						}
						goto case RegisterTargetPreference.Head;
					case RegisterTargetPreference.Head:
						var htmlHeadAdapter = HttpContext.Items[typeof(HtmlHeadAdapter)] as HtmlHeadAdapter;
						if(htmlHeadAdapter != null) {
							htmlHeadAdapter.RegisterClientScriptBlock(type, key, script);
							break;
						}
						var webHead = HttpContext.Items[typeof(WebHead)] as WebHead;
						if(webHead != null) {
							webHead.RegisterClientScriptBlock(type, key, script);
							break;
						}
						goto case RegisterTargetPreference.Page;
					case RegisterTargetPreference.Page:
						var page = HttpContext.CurrentHandler as Page;
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
			var page = HttpContext.CurrentHandler as Page;
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