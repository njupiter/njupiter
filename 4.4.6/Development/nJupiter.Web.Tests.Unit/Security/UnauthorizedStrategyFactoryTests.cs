using System;
using System.Web;
using System.Web.Configuration;

using FakeItEasy;

using NUnit.Framework;

using nJupiter.Web.Security;

namespace nJupiter.Web.Tests.Unit.Security {
	
	[TestFixture]
	public class UnauthorizedStrategyFactoryTests {

		[Test]
		public void Create_FormsAuthenticationModeConfigured_ReturnsStrategyOfTypeFormsAuthenticationUnauthorizedStrategy() {
			var httpContextHandler = A.Fake<IHttpContextHandler>();
			var authenticationConfigurationLoader = A.Fake<IAuthenticationConfigurationLoader>(); 
			var authenticationConfiguration = A.Fake<IAuthenticationConfiguration>(); 
			A.CallTo(() => authenticationConfigurationLoader.Load()).Returns(authenticationConfiguration);
			A.CallTo(() => authenticationConfiguration.Mode).Returns(AuthenticationMode.Forms);
			
			var factory = new UnauthorizedStrategyFactory(httpContextHandler, authenticationConfigurationLoader);

			var result = factory.Create();

			Assert.IsInstanceOf<FormsAuthenticationUnauthorizedStrategy>(result);
		}

		[Test]
		public void Create_WindowsAuthenticationModeConfigured_ReturnsStrategyOfTypeGenericAuthenticationUnauthorizedStrategy() {
			var httpContextHandler = A.Fake<IHttpContextHandler>();
			var authenticationConfigurationLoader = A.Fake<IAuthenticationConfigurationLoader>(); 
			var authenticationConfiguration = A.Fake<IAuthenticationConfiguration>(); 
			A.CallTo(() => authenticationConfigurationLoader.Load()).Returns(authenticationConfiguration);
			A.CallTo(() => authenticationConfiguration.Mode).Returns(AuthenticationMode.Windows);
			
			var factory = new UnauthorizedStrategyFactory(httpContextHandler, authenticationConfigurationLoader);

			var result = factory.Create();

			Assert.IsInstanceOf<GenericUnauthorizedStrategy>(result);
		}

		[Test]
		public void Create_NoAuthenticationModeConfigured_ReturnsStrategyOfTypeGenericAuthenticationUnauthorizedStrategy() {
			var httpContextHandler = A.Fake<IHttpContextHandler>();
			var authenticationConfigurationLoader = A.Fake<IAuthenticationConfigurationLoader>(); 
			
			var factory = new UnauthorizedStrategyFactory(httpContextHandler, authenticationConfigurationLoader);

			var result = factory.Create();

			Assert.IsInstanceOf<GenericUnauthorizedStrategy>(result);
		}

	}
}
