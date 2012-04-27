using System.Web;

namespace nJupiter.Web {
	public class HttpContextHandler : IHttpContextHandler {

		public HttpContextBase Current {
			get {
				if(HttpContext.Current != null) {
					var currentContext = HttpContext.Current.Items[typeof(HttpContextHandler)] as HttpContextBase;
					if(currentContext == null) {
						currentContext = new HttpContextWrapper(HttpContext.Current);
						HttpContext.Current.Items[typeof(HttpContextHandler)] = currentContext;
					}
					return currentContext;

				}
				return null;
			}
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
