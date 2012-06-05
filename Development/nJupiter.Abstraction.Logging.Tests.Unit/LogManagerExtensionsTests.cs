using System;

using FakeItEasy;

using NUnit.Framework;

namespace nJupiter.Abstraction.Logging.Tests.Unit {
	
	[TestFixture]
	public class LogManagerExtensionsTests {

		[Test]
		public void GetLogger_UseGenericMethod_ReturnsSameInstanceAsNonGenericMetod() {
			var logger = A.Fake<ILog>();
			var logManager = A.Fake<ILogManager>();
			A.CallTo(() => logManager.GetLogger(typeof(LogManagerExtensionsTests))).Returns(logger);
			
			var result = logManager.GetLogger<LogManagerExtensionsTests>();

			Assert.AreSame(logger, result);
		}

		[Test]
		public void GetCurrentClassLogger_GetLogger_ReturnsLoggerForThisClass() {
			var logger = A.Fake<ILog>();
			var logManager = A.Fake<ILogManager>();
			A.CallTo(() => logManager.GetLogger(GetType())).Returns(logger);
			
			var result = logManager.GetCurrentClassLogger();

			Assert.AreSame(logger, result);
		}

		[Test]
		public void GetLogger_LogManagerIsNull_ThrowsArgumentNullExeption() {
			ILogManager logManager = null;
			Assert.Throws<ArgumentNullException>(() => logManager.GetLogger<LogManagerExtensionsTests>());
		}

		[Test]
		public void GetCurrentClassLogger_LogManagerIsNull_ThrowsArgumentNullExeption() {
			ILogManager logManager = null;
			Assert.Throws<ArgumentNullException>(() => logManager.GetCurrentClassLogger());
		}

	}
}
