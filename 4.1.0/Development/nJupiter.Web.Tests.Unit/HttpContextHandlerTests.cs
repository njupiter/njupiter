using System.Web;

using FakeItEasy;

using NUnit.Framework;

namespace nJupiter.Web.Tests.Unit {
	
	[TestFixture]
	public class HttpContextHandlerTests {

		[Test]
		public void Current_HandlerCreatedWithDefaultConstructor_CurrentIsAHttpContextWrapper() {
			HttpContext.Current = new HttpContext(new HttpRequest(string.Empty, "http://anyuri.org/", string.Empty), new HttpResponse(null));

			var httpContextHandler = new HttpContextHandler();
			
			Assert.IsInstanceOf<HttpContextWrapper>(httpContextHandler.Current);
		}

		[Test]
		public void Current_HandlerCreatedWithDefaultConstructor_WrapperCreatedWithCurrentContext() {
			HttpContext.Current = new HttpContext(new HttpRequest(string.Empty, "http://anyuri.org/", string.Empty), new HttpResponse(null));

			var httpContextHandler = new HttpContextHandler();
			
			Assert.AreEqual("http://anyuri.org/", httpContextHandler.Current.Request.Url.OriginalString);
		}


		[Test]
		public void Current_HandlerCreatedWithDefaultConstructor_CurrentRequestStoresCurrentContext() {
			HttpContext.Current = new HttpContext(new HttpRequest(string.Empty, "http://anyuri.org/", string.Empty), new HttpResponse(null));

			var httpContextHandler = new HttpContextHandler();
			
			Assert.AreEqual(httpContextHandler.Current, HttpContext.Current.Items[httpContextHandler]);
		}


		[Test]
		public void Current_CurrentHttpContextIsNull_ReturnsNull() {
			HttpContext.Current = null;

			var httpContextHandler = new HttpContextHandler();
			
			Assert.IsNull(httpContextHandler.Current);
		}

		[Test]
		public void Constructor_SendInAHttpContextFactoryMethod_CurrentContextCreatedByFactoryMethod() {
			HttpContext.Current = new HttpContext(new HttpRequest(string.Empty, "http://anyuri.org/", string.Empty), new HttpResponse(null));
			var context = A.Fake<HttpContextBase>();

			var httpContextHandler = new HttpContextHandler(() => context);
			
			Assert.AreSame(context, httpContextHandler.Current);
		}
	}
}
