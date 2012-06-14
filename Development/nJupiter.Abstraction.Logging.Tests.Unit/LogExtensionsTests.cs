using System;

using FakeItEasy;

using NUnit.Framework;

namespace nJupiter.Abstraction.Logging.Tests.Unit {
	
	[TestFixture]
	public class LogExtensionsTests {

		[Test]
		public void DebugCallback_SendInMessageInCallbackWhileDebugLoggingIsEnabled_MessageIsLoged() {
			var log = A.Fake<ILog>();
			A.CallTo(() => log.IsDebugEnabled).Returns(true);

			log.Debug(m => m("Hello world"));

			A.CallTo(() => log.Debug("Hello world")).MustHaveHappened(Repeated.Exactly.Once);
		}

		[Test]
		public void DebugCallback_SendInMessageInCallbackWhileDebugLoggingIsDisabled_MessageIsNotLoged() {
			var log = A.Fake<ILog>();
			A.CallTo(() => log.IsDebugEnabled).Returns(false);

			log.Debug(m => m("Hello world"));

			A.CallTo(() => log.Debug("Hello world")).MustNotHaveHappened();
		}

		[Test]
		public void DebugCallback_SendInMessageAndExceptionInCallbackWhileDebugLoggingIsEnabled_MessageIsLoged() {
			var log = A.Fake<ILog>();
			A.CallTo(() => log.IsDebugEnabled).Returns(true);

			var exception = new Exception();
			log.Debug(m => m("Hello world", exception));

			A.CallTo(() => log.Debug("Hello world", exception)).MustHaveHappened(Repeated.Exactly.Once);
		}

		[Test]
		public void DebugCallback_SendInMessageAndExceptionInCallbackWhileDebugLoggingIsDisabled_MessageIsNotLoged() {
			var log = A.Fake<ILog>();
			A.CallTo(() => log.IsDebugEnabled).Returns(false);

			var exception = new Exception();
			log.Debug(m => m("Hello world", exception));

			A.CallTo(() => log.Debug("Hello world", exception)).MustNotHaveHappened();
		}


		[Test]
		public void DebugCallback_SendInFormatMessageInCallbackWhileDebugLoggingIsEnabled_MessageIsLoged() {
			var log = A.Fake<ILog>();
			A.CallTo(() => log.IsDebugEnabled).Returns(true);
			
			var args = new object[] { "Message" };

			log.DebugFormat(m => m("Format", args));

			A.CallTo(() => log.DebugFormat("Format", A<object[]>.That.IsSameSequenceAs(args))).MustHaveHappened(Repeated.Exactly.Once);
		}

		[Test]
		public void DebugCallback_SendInFormatMessageInCallbackWhileDebugLoggingIsDisabled_MessageIsNotLoged() {
			var log = A.Fake<ILog>();
			A.CallTo(() => log.IsDebugEnabled).Returns(false);

			var args = new object[] { "Message" };

			log.DebugFormat(m => m("Format", args));

			A.CallTo(() => log.DebugFormat("Format", A<object[]>.That.IsSameSequenceAs(args))).MustNotHaveHappened();
		}


		[Test]
		public void DebugCallback_SendInFormatProviderMessageInCallbackWhileDebugLoggingIsEnabled_MessageIsLoged() {
			var log = A.Fake<ILog>();
			A.CallTo(() => log.IsDebugEnabled).Returns(true);

			var args = new object[] { "Message" };
			var provider = A.Fake<IFormatProvider>();
			log.DebugFormat(m => m(provider, "Format", args));

			A.CallTo(() => log.DebugFormat(provider, "Format",  A<object[]>.That.IsSameSequenceAs(args))).MustHaveHappened(Repeated.Exactly.Once);
		}

		[Test]
		public void DebugCallback_SendInFormatProviderMessageInCallbackWhileDebugLoggingIsDisabled_MessageIsNotLoged() {
			var log = A.Fake<ILog>();
			A.CallTo(() => log.IsDebugEnabled).Returns(false);

			var args = new object[] { "Message" };
			var provider = A.Fake<IFormatProvider>();
			log.DebugFormat(m => m("Format", "Message"));

			A.CallTo(() => log.DebugFormat(provider, "Format",  A<object[]>.That.IsSameSequenceAs(args))).MustNotHaveHappened();
		}

		[Test]
		public void InfoCallback_SendInMessageInCallbackWhileInfoLoggingIsEnabled_MessageIsLoged() {
			var log = A.Fake<ILog>();
			A.CallTo(() => log.IsInfoEnabled).Returns(true);

			log.Info(m => m("Hello world"));

			A.CallTo(() => log.Info("Hello world")).MustHaveHappened(Repeated.Exactly.Once);
		}

		[Test]
		public void InfoCallback_SendInMessageInCallbackWhileInfoLoggingIsDisabled_MessageIsNotLoged() {
			var log = A.Fake<ILog>();
			A.CallTo(() => log.IsInfoEnabled).Returns(false);

			log.Info(m => m("Hello world"));

			A.CallTo(() => log.Info("Hello world")).MustNotHaveHappened();
		}

		[Test]
		public void InfoCallback_SendInMessageAndExceptionInCallbackWhileInfoLoggingIsEnabled_MessageIsLoged() {
			var log = A.Fake<ILog>();
			A.CallTo(() => log.IsInfoEnabled).Returns(true);

			var exception = new Exception();
			log.Info(m => m("Hello world", exception));

			A.CallTo(() => log.Info("Hello world", exception)).MustHaveHappened(Repeated.Exactly.Once);
		}

		[Test]
		public void InfoCallback_SendInMessageAndExceptionInCallbackWhileInfoLoggingIsDisabled_MessageIsNotLoged() {
			var log = A.Fake<ILog>();
			A.CallTo(() => log.IsInfoEnabled).Returns(false);

			var exception = new Exception();
			log.Info(m => m("Hello world", exception));

			A.CallTo(() => log.Info("Hello world", exception)).MustNotHaveHappened();
		}


		[Test]
		public void InfoCallback_SendInFormatMessageInCallbackWhileInfoLoggingIsEnabled_MessageIsLoged() {
			var log = A.Fake<ILog>();
			A.CallTo(() => log.IsInfoEnabled).Returns(true);
			
			var args = new object[] { "Message" };

			log.InfoFormat(m => m("Format", args));

			A.CallTo(() => log.InfoFormat("Format", A<object[]>.That.IsSameSequenceAs(args))).MustHaveHappened(Repeated.Exactly.Once);
		}

		[Test]
		public void InfoCallback_SendInFormatMessageInCallbackWhileInfoLoggingIsDisabled_MessageIsNotLoged() {
			var log = A.Fake<ILog>();
			A.CallTo(() => log.IsInfoEnabled).Returns(false);

			var args = new object[] { "Message" };

			log.InfoFormat(m => m("Format", args));

			A.CallTo(() => log.InfoFormat("Format", A<object[]>.That.IsSameSequenceAs(args))).MustNotHaveHappened();
		}


		[Test]
		public void InfoCallback_SendInFormatProviderMessageInCallbackWhileInfoLoggingIsEnabled_MessageIsLoged() {
			var log = A.Fake<ILog>();
			A.CallTo(() => log.IsInfoEnabled).Returns(true);

			var args = new object[] { "Message" };
			var provider = A.Fake<IFormatProvider>();
			log.InfoFormat(m => m(provider, "Format", args));

			A.CallTo(() => log.InfoFormat(provider, "Format",  A<object[]>.That.IsSameSequenceAs(args))).MustHaveHappened(Repeated.Exactly.Once);
		}

		[Test]
		public void InfoCallback_SendInFormatProviderMessageInCallbackWhileInfoLoggingIsDisabled_MessageIsNotLoged() {
			var log = A.Fake<ILog>();
			A.CallTo(() => log.IsInfoEnabled).Returns(false);

			var args = new object[] { "Message" };
			var provider = A.Fake<IFormatProvider>();
			log.InfoFormat(m => m("Format", "Message"));

			A.CallTo(() => log.InfoFormat(provider, "Format",  A<object[]>.That.IsSameSequenceAs(args))).MustNotHaveHappened();
		}

		[Test]
		public void WarnCallback_SendInMessageInCallbackWhileWarnLoggingIsEnabled_MessageIsLoged() {
			var log = A.Fake<ILog>();
			A.CallTo(() => log.IsWarnEnabled).Returns(true);

			log.Warn(m => m("Hello world"));

			A.CallTo(() => log.Warn("Hello world")).MustHaveHappened(Repeated.Exactly.Once);
		}

		[Test]
		public void WarnCallback_SendInMessageInCallbackWhileWarnLoggingIsDisabled_MessageIsNotLoged() {
			var log = A.Fake<ILog>();
			A.CallTo(() => log.IsWarnEnabled).Returns(false);

			log.Warn(m => m("Hello world"));

			A.CallTo(() => log.Warn("Hello world")).MustNotHaveHappened();
		}

		[Test]
		public void WarnCallback_SendInMessageAndExceptionInCallbackWhileWarnLoggingIsEnabled_MessageIsLoged() {
			var log = A.Fake<ILog>();
			A.CallTo(() => log.IsWarnEnabled).Returns(true);

			var exception = new Exception();
			log.Warn(m => m("Hello world", exception));

			A.CallTo(() => log.Warn("Hello world", exception)).MustHaveHappened(Repeated.Exactly.Once);
		}

		[Test]
		public void WarnCallback_SendInMessageAndExceptionInCallbackWhileWarnLoggingIsDisabled_MessageIsNotLoged() {
			var log = A.Fake<ILog>();
			A.CallTo(() => log.IsWarnEnabled).Returns(false);

			var exception = new Exception();
			log.Warn(m => m("Hello world", exception));

			A.CallTo(() => log.Warn("Hello world", exception)).MustNotHaveHappened();
		}


		[Test]
		public void WarnCallback_SendInFormatMessageInCallbackWhileWarnLoggingIsEnabled_MessageIsLoged() {
			var log = A.Fake<ILog>();
			A.CallTo(() => log.IsWarnEnabled).Returns(true);
			
			var args = new object[] { "Message" };

			log.WarnFormat(m => m("Format", args));

			A.CallTo(() => log.WarnFormat("Format", A<object[]>.That.IsSameSequenceAs(args))).MustHaveHappened(Repeated.Exactly.Once);
		}

		[Test]
		public void WarnCallback_SendInFormatMessageInCallbackWhileWarnLoggingIsDisabled_MessageIsNotLoged() {
			var log = A.Fake<ILog>();
			A.CallTo(() => log.IsWarnEnabled).Returns(false);

			var args = new object[] { "Message" };

			log.WarnFormat(m => m("Format", args));

			A.CallTo(() => log.WarnFormat("Format", A<object[]>.That.IsSameSequenceAs(args))).MustNotHaveHappened();
		}


		[Test]
		public void WarnCallback_SendInFormatProviderMessageInCallbackWhileWarnLoggingIsEnabled_MessageIsLoged() {
			var log = A.Fake<ILog>();
			A.CallTo(() => log.IsWarnEnabled).Returns(true);

			var args = new object[] { "Message" };
			var provider = A.Fake<IFormatProvider>();
			log.WarnFormat(m => m(provider, "Format", args));

			A.CallTo(() => log.WarnFormat(provider, "Format",  A<object[]>.That.IsSameSequenceAs(args))).MustHaveHappened(Repeated.Exactly.Once);
		}

		[Test]
		public void WarnCallback_SendInFormatProviderMessageInCallbackWhileWarnLoggingIsDisabled_MessageIsNotLoged() {
			var log = A.Fake<ILog>();
			A.CallTo(() => log.IsWarnEnabled).Returns(false);

			var args = new object[] { "Message" };
			var provider = A.Fake<IFormatProvider>();
			log.WarnFormat(m => m("Format", "Message"));

			A.CallTo(() => log.WarnFormat(provider, "Format",  A<object[]>.That.IsSameSequenceAs(args))).MustNotHaveHappened();
		}

		[Test]
		public void ErrorCallback_SendInMessageInCallbackWhileErrorLoggingIsEnabled_MessageIsLoged() {
			var log = A.Fake<ILog>();
			A.CallTo(() => log.IsErrorEnabled).Returns(true);

			log.Error(m => m("Hello world"));

			A.CallTo(() => log.Error("Hello world")).MustHaveHappened(Repeated.Exactly.Once);
		}

		[Test]
		public void ErrorCallback_SendInMessageInCallbackWhileErrorLoggingIsDisabled_MessageIsNotLoged() {
			var log = A.Fake<ILog>();
			A.CallTo(() => log.IsErrorEnabled).Returns(false);

			log.Error(m => m("Hello world"));

			A.CallTo(() => log.Error("Hello world")).MustNotHaveHappened();
		}

		[Test]
		public void ErrorCallback_SendInMessageAndExceptionInCallbackWhileErrorLoggingIsEnabled_MessageIsLoged() {
			var log = A.Fake<ILog>();
			A.CallTo(() => log.IsErrorEnabled).Returns(true);

			var exception = new Exception();
			log.Error(m => m("Hello world", exception));

			A.CallTo(() => log.Error("Hello world", exception)).MustHaveHappened(Repeated.Exactly.Once);
		}

		[Test]
		public void ErrorCallback_SendInMessageAndExceptionInCallbackWhileErrorLoggingIsDisabled_MessageIsNotLoged() {
			var log = A.Fake<ILog>();
			A.CallTo(() => log.IsErrorEnabled).Returns(false);

			var exception = new Exception();
			log.Error(m => m("Hello world", exception));

			A.CallTo(() => log.Error("Hello world", exception)).MustNotHaveHappened();
		}


		[Test]
		public void ErrorCallback_SendInFormatMessageInCallbackWhileErrorLoggingIsEnabled_MessageIsLoged() {
			var log = A.Fake<ILog>();
			A.CallTo(() => log.IsErrorEnabled).Returns(true);
			
			var args = new object[] { "Message" };

			log.ErrorFormat(m => m("Format", args));

			A.CallTo(() => log.ErrorFormat("Format", A<object[]>.That.IsSameSequenceAs(args))).MustHaveHappened(Repeated.Exactly.Once);
		}

		[Test]
		public void ErrorCallback_SendInFormatMessageInCallbackWhileErrorLoggingIsDisabled_MessageIsNotLoged() {
			var log = A.Fake<ILog>();
			A.CallTo(() => log.IsErrorEnabled).Returns(false);

			var args = new object[] { "Message" };

			log.ErrorFormat(m => m("Format", args));

			A.CallTo(() => log.ErrorFormat("Format", A<object[]>.That.IsSameSequenceAs(args))).MustNotHaveHappened();
		}


		[Test]
		public void ErrorCallback_SendInFormatProviderMessageInCallbackWhileErrorLoggingIsEnabled_MessageIsLoged() {
			var log = A.Fake<ILog>();
			A.CallTo(() => log.IsErrorEnabled).Returns(true);

			var args = new object[] { "Message" };
			var provider = A.Fake<IFormatProvider>();
			log.ErrorFormat(m => m(provider, "Format", args));

			A.CallTo(() => log.ErrorFormat(provider, "Format",  A<object[]>.That.IsSameSequenceAs(args))).MustHaveHappened(Repeated.Exactly.Once);
		}

		[Test]
		public void ErrorCallback_SendInFormatProviderMessageInCallbackWhileErrorLoggingIsDisabled_MessageIsNotLoged() {
			var log = A.Fake<ILog>();
			A.CallTo(() => log.IsErrorEnabled).Returns(false);

			var args = new object[] { "Message" };
			var provider = A.Fake<IFormatProvider>();
			log.ErrorFormat(m => m("Format", "Message"));

			A.CallTo(() => log.ErrorFormat(provider, "Format",  A<object[]>.That.IsSameSequenceAs(args))).MustNotHaveHappened();
		}

		[Test]
		public void FatalCallback_SendInMessageInCallbackWhileFatalLoggingIsEnabled_MessageIsLoged() {
			var log = A.Fake<ILog>();
			A.CallTo(() => log.IsFatalEnabled).Returns(true);

			log.Fatal(m => m("Hello world"));

			A.CallTo(() => log.Fatal("Hello world")).MustHaveHappened(Repeated.Exactly.Once);
		}

		[Test]
		public void FatalCallback_SendInMessageInCallbackWhileFatalLoggingIsDisabled_MessageIsNotLoged() {
			var log = A.Fake<ILog>();
			A.CallTo(() => log.IsFatalEnabled).Returns(false);

			log.Fatal(m => m("Hello world"));

			A.CallTo(() => log.Fatal("Hello world")).MustNotHaveHappened();
		}

		[Test]
		public void FatalCallback_SendInMessageAndExceptionInCallbackWhileFatalLoggingIsEnabled_MessageIsLoged() {
			var log = A.Fake<ILog>();
			A.CallTo(() => log.IsFatalEnabled).Returns(true);

			var exception = new Exception();
			log.Fatal(m => m("Hello world", exception));

			A.CallTo(() => log.Fatal("Hello world", exception)).MustHaveHappened(Repeated.Exactly.Once);
		}

		[Test]
		public void FatalCallback_SendInMessageAndExceptionInCallbackWhileFatalLoggingIsDisabled_MessageIsNotLoged() {
			var log = A.Fake<ILog>();
			A.CallTo(() => log.IsFatalEnabled).Returns(false);

			var exception = new Exception();
			log.Fatal(m => m("Hello world", exception));

			A.CallTo(() => log.Fatal("Hello world", exception)).MustNotHaveHappened();
		}


		[Test]
		public void FatalCallback_SendInFormatMessageInCallbackWhileFatalLoggingIsEnabled_MessageIsLoged() {
			var log = A.Fake<ILog>();
			A.CallTo(() => log.IsFatalEnabled).Returns(true);
			
			var args = new object[] { "Message" };

			log.FatalFormat(m => m("Format", args));

			A.CallTo(() => log.FatalFormat("Format", A<object[]>.That.IsSameSequenceAs(args))).MustHaveHappened(Repeated.Exactly.Once);
		}

		[Test]
		public void FatalCallback_SendInFormatMessageInCallbackWhileFatalLoggingIsDisabled_MessageIsNotLoged() {
			var log = A.Fake<ILog>();
			A.CallTo(() => log.IsFatalEnabled).Returns(false);

			var args = new object[] { "Message" };

			log.FatalFormat(m => m("Format", args));

			A.CallTo(() => log.FatalFormat("Format", A<object[]>.That.IsSameSequenceAs(args))).MustNotHaveHappened();
		}


		[Test]
		public void FatalCallback_SendInFormatProviderMessageInCallbackWhileFatalLoggingIsEnabled_MessageIsLoged() {
			var log = A.Fake<ILog>();
			A.CallTo(() => log.IsFatalEnabled).Returns(true);

			var args = new object[] { "Message" };
			var provider = A.Fake<IFormatProvider>();
			log.FatalFormat(m => m(provider, "Format", args));

			A.CallTo(() => log.FatalFormat(provider, "Format",  A<object[]>.That.IsSameSequenceAs(args))).MustHaveHappened(Repeated.Exactly.Once);
		}

		[Test]
		public void FatalCallback_SendInFormatProviderMessageInCallbackWhileFatalLoggingIsDisabled_MessageIsNotLoged() {
			var log = A.Fake<ILog>();
			A.CallTo(() => log.IsFatalEnabled).Returns(false);

			var args = new object[] { "Message" };
			var provider = A.Fake<IFormatProvider>();
			log.FatalFormat(m => m("Format", "Message"));

			A.CallTo(() => log.FatalFormat(provider, "Format",  A<object[]>.That.IsSameSequenceAs(args))).MustNotHaveHappened();
		}
	}
}
