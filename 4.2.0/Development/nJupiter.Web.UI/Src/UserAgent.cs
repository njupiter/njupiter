using System;
using System.Globalization;
using System.Web;

namespace nJupiter.Web.UI {
	public class UserAgent : IUserAgent {
		
		private readonly HttpContextBase httpContext;

		private readonly string currentUserAgent;
		private HttpContextBase HttpContext { get { return httpContext ?? HttpContextHandler.Instance.Current; } }

		public UserAgent() {}

		public UserAgent(string currentUserAgent) {
			this.currentUserAgent = currentUserAgent;
		}

		public UserAgent(HttpContextBase httpContext) {
			this.httpContext = httpContext;
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