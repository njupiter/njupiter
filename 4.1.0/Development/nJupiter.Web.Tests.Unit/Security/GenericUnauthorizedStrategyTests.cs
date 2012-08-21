using System;
using System.Web;

using FakeItEasy;

using NUnit.Framework;

using nJupiter.Web.Security;

namespace nJupiter.Web.Tests.Unit.Security {
	
	[TestFixture]
	public class GenericUnauthorizedStrategyTests {
		[Test]
		public void Execute_StrategyExecuted_UnauthorizedHttpCodeSetOnContext() {
			var httpContextHandler = A.Fake<IHttpContextHandler>();
			var stategy = new GenericUnauthorizedStrategy(httpContextHandler);

			stategy.Execute();

			Assert.AreEqual("401 Unauthorized", httpContextHandler.Current.Response.Status);
			Assert.AreEqual(401, httpContextHandler.Current.Response.StatusCode);
		}

		[Test]
		public void Execute_StrategyExecuted_StatusFlushedToCurrentResponseAfterStatusHasBeenSet() {
			var httpContextHandler = A.Fake<IHttpContextHandler>();
			var stategy = new GenericUnauthorizedStrategy(httpContextHandler);

			var status = 0;
			A.CallTo(() => httpContextHandler.Current.Response.Flush()).Invokes(c => status = httpContextHandler.Current.Response.StatusCode);

			stategy.Execute();

			Assert.AreEqual(401, status);
		}

		[Test]
		public void Execute_StrategyExecuted_CurrentResponseCleared() {
			var httpContextHandler = A.Fake<IHttpContextHandler>();
			var stategy = new GenericUnauthorizedStrategy(httpContextHandler);

			stategy.Execute();

			A.CallTo(() => httpContextHandler.Current.Response.Clear()).MustHaveHappened(Repeated.Exactly.Once);
		}

		[Test]
		public void Execute_StrategyExecuted_CurrentResponseEnded() {
			var httpContextHandler = A.Fake<IHttpContextHandler>();
			var stategy = new GenericUnauthorizedStrategy(httpContextHandler);

			stategy.Execute();

			A.CallTo(() => httpContextHandler.Current.Response.End()).MustHaveHappened(Repeated.Exactly.Once);
		}

		[Test]
		public void Constructor_SendInAHttpContextBaseFactoryMethodAndStrategyExecuted_StatusSetOnHttpContextReturned() {
			var httpContextBase = A.Fake<HttpContextBase>();
			HttpContext.Current = new HttpContext(new HttpRequest(string.Empty, "http://anyuri.org/", string.Empty), new HttpResponse(null));

			A.CallTo(() => httpContextBase.Request.Url).Returns(new Uri("http://anyuri.org/"));

			var strategy = new GenericUnauthorizedStrategy(() => httpContextBase);

			strategy.Execute();

			Assert.AreEqual(401, httpContextBase.Response.StatusCode);
		}
	}
}
