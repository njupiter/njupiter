using System;
using System.Web;

using FakeItEasy;

using NUnit.Framework;

using nJupiter.Web.Security;

namespace nJupiter.Web.Tests.Unit.Security {
	
	[TestFixture]
	public class FormsAuthenticationUnauthorizedStrategyTests {

		[Test]
		public void Execute_StrategyExecuted_Redirected() {
			var formsAuthentication = A.Fake<IFormsAuthentication>();
			var httpContextHandler = A.Fake<IHttpContextHandler>();

			A.CallTo(() => httpContextHandler.Current.Request.Url).Returns(new Uri("http://anyuri.org/"));
	
			var strategy = new FormsAuthenticationUnauthorizedStrategy(httpContextHandler, formsAuthentication);

			strategy.Execute();

			A.CallTo(() => httpContextHandler.Current.Response.Redirect(A<string>.Ignored, true)).MustHaveHappened(Repeated.Exactly.Once);
		}

		[Test]
		public void Execute_StrategyExecuted_RedirectedToFormsLoginUrlWithRedirectToCurrentUrl() {
			var formsAuthentication = A.Fake<IFormsAuthentication>();
			var httpContextHandler = A.Fake<IHttpContextHandler>();

			A.CallTo(() => formsAuthentication.LoginUrl).Returns("http://loginuri.org/");
			A.CallTo(() => httpContextHandler.Current.Request.Url).Returns(new Uri("http://anyuri.org/anypath/?anyquery=anyparam"));
			A.CallTo(() => httpContextHandler.Current.Server.UrlEncode(A<string>.Ignored)).ReturnsLazily(v => v.GetArgument<string>(0));
			
			var strategy = new FormsAuthenticationUnauthorizedStrategy(httpContextHandler, formsAuthentication);

			strategy.Execute();

			A.CallTo(() => httpContextHandler.Current.Response.Redirect("http://loginuri.org/?ReturnUrl=/anypath/?anyquery=anyparam", true)).MustHaveHappened(Repeated.Exactly.Once);
		}

		[Test]
		public void Execute_StrategyExecutedWithNoCurrentUrl_RedirectedToFormsLoginUrlWithRedirectToRoot() {
			var formsAuthentication = A.Fake<IFormsAuthentication>();
			var httpContextHandler = A.Fake<IHttpContextHandler>();

			A.CallTo(() => formsAuthentication.LoginUrl).Returns("http://loginuri.org/");
			A.CallTo(() => httpContextHandler.Current.Request.Url).Returns(null);
			A.CallTo(() => httpContextHandler.Current.Server.UrlEncode(A<string>.Ignored)).ReturnsLazily(v => v.GetArgument<string>(0));
			
			var strategy = new FormsAuthenticationUnauthorizedStrategy(httpContextHandler, formsAuthentication);

			strategy.Execute();

			A.CallTo(() => httpContextHandler.Current.Response.Redirect("http://loginuri.org/?ReturnUrl=/", true)).MustHaveHappened(Repeated.Exactly.Once);
		}

		[Test]
		public void Constructor_SendInAHttpContextBaseFactoryMethodAndStrategyExecuted_RedirectedExecutedOnHttpContextReturned() {
			var formsAuthentication = A.Fake<IFormsAuthentication>();
			var httpContextBase = A.Fake<HttpContextBase>();
			HttpContext.Current = new HttpContext(new HttpRequest(string.Empty, "http://anyuri.org/", string.Empty), new HttpResponse(null));

			A.CallTo(() => httpContextBase.Request.Url).Returns(new Uri("http://anyuri.org/"));
	
			var strategy = new FormsAuthenticationUnauthorizedStrategy(() => httpContextBase, formsAuthentication);

			strategy.Execute();

			A.CallTo(() => httpContextBase.Response.Redirect(A<string>.Ignored, true)).MustHaveHappened(Repeated.Exactly.Once);
		}

	}
}
