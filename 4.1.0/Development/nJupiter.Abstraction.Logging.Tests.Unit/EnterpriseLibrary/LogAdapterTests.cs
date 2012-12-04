#region Copyright & License
// 
// 	Copyright (c) 2005-2012 nJupiter
// 
// 	Permission is hereby granted, free of charge, to any person obtaining a copy
// 	of this software and associated documentation files (the "Software"), to deal
// 	in the Software without restriction, including without limitation the rights
// 	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// 	copies of the Software, and to permit persons to whom the Software is
// 	furnished to do so, subject to the following conditions:
// 
// 	The above copyright notice and this permission notice shall be included in
// 	all copies or substantial portions of the Software.
// 
// 	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// 	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// 	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// 	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// 	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// 	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// 	THE SOFTWARE.
// 
#endregion

using System;
using System.Diagnostics;
using System.Globalization;

using FakeItEasy;

using Microsoft.Practices.EnterpriseLibrary.Logging;

using NUnit.Framework;

using nJupiter.Abstraction.Logging.EnterpriseLibrary;

namespace nJupiter.Abstraction.Logging.Tests.Unit.EnterpriseLibrary {
	
	[TestFixture]
	public class LogAdapterTests {

		[Test]
		public void Debug_SendInAnyMessage_WriteVerboseEntryToUnderlyingClass() {
			var factory = A.Fake<ILogEntryFactory>();
			var logEntry = new LogEntry();
			A.CallTo(() => factory.Create(TraceEventType.Verbose, A<object[]>.Ignored)).Returns(logEntry);
			var adapter = new TestableLogAdapter(factory);

			adapter.Debug("Any message");

			Assert.AreSame(logEntry, adapter.WrittenLogEntry);
		}

		[Test]
		public void Debug_SendInAnyMessageAndException_WriteVerboseEntryToUnderlyingClass() {
			var factory = A.Fake<ILogEntryFactory>();
			var logEntry = new LogEntry();
			A.CallTo(() => factory.Create(TraceEventType.Verbose, A<object[]>.Ignored)).Returns(logEntry);
			var adapter = new TestableLogAdapter(factory);

			adapter.Debug("Any message", new Exception());

			Assert.AreSame(logEntry, adapter.WrittenLogEntry);
		}

		[Test]
		public void DebugFormat_SendInCurrentCultureAndFormatAndAnyMessageAndException_WriteVerboseEntryToUnderlyingClass() {
			var factory = A.Fake<ILogEntryFactory>();
			var logEntry = new LogEntry();
			A.CallTo(() => factory.Create(TraceEventType.Verbose, CultureInfo.CurrentCulture, "{0}", A<object[]>.Ignored)).Returns(logEntry);
			var adapter = new TestableLogAdapter(factory);

			adapter.DebugFormat(CultureInfo.CurrentCulture, "{0}", "Any message");

			Assert.AreSame(logEntry, adapter.WrittenLogEntry);
		}

		[Test]
		public void DebugFormat_SendInFormatAndAnyMessageAndException_WriteVerboseEntryToUnderlyingClass() {
			var factory = A.Fake<ILogEntryFactory>();
			var logEntry = new LogEntry();
			A.CallTo(() => factory.Create(TraceEventType.Verbose, "{0}", A<object[]>.Ignored)).Returns(logEntry);
			var adapter = new TestableLogAdapter(factory);

			adapter.DebugFormat("{0}", "Any message");

			Assert.AreSame(logEntry, adapter.WrittenLogEntry);
		}

		[Test]
		public void IsDebugEnabled_LoggingIsEnabledOnUnderlyingWrapper_ReturnsTrue() {
			var factory = A.Fake<ILogEntryFactory>();
			var adapter = new TestableLogAdapter(true, factory);
			Assert.IsTrue(adapter.IsDebugEnabled);
		}

		[Test]
		public void IsDebugEnabled_LoggingIsNotEnabledOnUnderlyingWrapper_ReturnsFalse() {
			var factory = A.Fake<ILogEntryFactory>();
			var adapter = new TestableLogAdapter(false, factory);
			Assert.IsFalse(adapter.IsDebugEnabled);
		}

		[Test]
		public void Info_SendInAnyMessage_WriteInformationEntryToUnderlyingClass() {
			var factory = A.Fake<ILogEntryFactory>();
			var logEntry = new LogEntry();
			A.CallTo(() => factory.Create(TraceEventType.Information, A<object[]>.Ignored)).Returns(logEntry);
			var adapter = new TestableLogAdapter(factory);

			adapter.Info("Any message");

			Assert.AreSame(logEntry, adapter.WrittenLogEntry);
		}

		[Test]
		public void Info_SendInAnyMessageAndException_WriteInformationEntryToUnderlyingClass() {
			var factory = A.Fake<ILogEntryFactory>();
			var logEntry = new LogEntry();
			A.CallTo(() => factory.Create(TraceEventType.Information, A<object[]>.Ignored)).Returns(logEntry);
			var adapter = new TestableLogAdapter(factory);

			adapter.Info("Any message", new Exception());

			Assert.AreSame(logEntry, adapter.WrittenLogEntry);
		}

		[Test]
		public void InfoFormat_SendInCurrentCultureAndFormatAndAnyMessageAndException_WriteInformationEntryToUnderlyingClass() {
			var factory = A.Fake<ILogEntryFactory>();
			var logEntry = new LogEntry();
			A.CallTo(() => factory.Create(TraceEventType.Information, CultureInfo.CurrentCulture, "{0}", A<object[]>.Ignored)).Returns(logEntry);
			var adapter = new TestableLogAdapter(factory);

			adapter.InfoFormat(CultureInfo.CurrentCulture, "{0}", "Any message");

			Assert.AreSame(logEntry, adapter.WrittenLogEntry);
		}

		[Test]
		public void InfoFormat_SendInFormatAndAnyMessageAndException_WriteInformationEntryToUnderlyingClass() {
			var factory = A.Fake<ILogEntryFactory>();
			var logEntry = new LogEntry();
			A.CallTo(() => factory.Create(TraceEventType.Information, "{0}", A<object[]>.Ignored)).Returns(logEntry);
			var adapter = new TestableLogAdapter(factory);

			adapter.InfoFormat("{0}", "Any message");

			Assert.AreSame(logEntry, adapter.WrittenLogEntry);
		}

		[Test]
		public void IsInfoEnabled_LoggingIsEnabledOnUnderlyingWrapper_ReturnsTrue() {
			var factory = A.Fake<ILogEntryFactory>();
			var adapter = new TestableLogAdapter(true, factory);
			Assert.IsTrue(adapter.IsInfoEnabled);
		}

		[Test]
		public void IsInfoEnabled_LoggingIsNotEnabledOnUnderlyingWrapper_ReturnsFalse() {
			var factory = A.Fake<ILogEntryFactory>();
			var adapter = new TestableLogAdapter(false, factory);
			Assert.IsFalse(adapter.IsInfoEnabled);
		}

		[Test]
		public void Warn_SendInAnyMessage_WriteWarningEntryToUnderlyingClass() {
			var factory = A.Fake<ILogEntryFactory>();
			var logEntry = new LogEntry();
			A.CallTo(() => factory.Create(TraceEventType.Warning, A<object[]>.Ignored)).Returns(logEntry);
			var adapter = new TestableLogAdapter(factory);

			adapter.Warn("Any message");

			Assert.AreSame(logEntry, adapter.WrittenLogEntry);
		}

		[Test]
		public void Warn_SendInAnyMessageAndException_WriteWarningEntryToUnderlyingClass() {
			var factory = A.Fake<ILogEntryFactory>();
			var logEntry = new LogEntry();
			A.CallTo(() => factory.Create(TraceEventType.Warning, A<object[]>.Ignored)).Returns(logEntry);
			var adapter = new TestableLogAdapter(factory);

			adapter.Warn("Any message", new Exception());

			Assert.AreSame(logEntry, adapter.WrittenLogEntry);
		}

		[Test]
		public void WarnFormat_SendInCurrentCultureAndFormatAndAnyMessageAndException_WriteWarningEntryToUnderlyingClass() {
			var factory = A.Fake<ILogEntryFactory>();
			var logEntry = new LogEntry();
			A.CallTo(() => factory.Create(TraceEventType.Warning, CultureInfo.CurrentCulture, "{0}", A<object[]>.Ignored)).Returns(logEntry);
			var adapter = new TestableLogAdapter(factory);

			adapter.WarnFormat(CultureInfo.CurrentCulture, "{0}", "Any message");

			Assert.AreSame(logEntry, adapter.WrittenLogEntry);
		}

		[Test]
		public void WarnFormat_SendInFormatAndAnyMessageAndException_WriteWarningEntryToUnderlyingClass() {
			var factory = A.Fake<ILogEntryFactory>();
			var logEntry = new LogEntry();
			A.CallTo(() => factory.Create(TraceEventType.Warning, "{0}", A<object[]>.Ignored)).Returns(logEntry);
			var adapter = new TestableLogAdapter(factory);

			adapter.WarnFormat("{0}", "Any message");

			Assert.AreSame(logEntry, adapter.WrittenLogEntry);
		}

		[Test]
		public void IsWarnEnabled_LoggingIsEnabledOnUnderlyingWrapper_ReturnsTrue() {
			var factory = A.Fake<ILogEntryFactory>();
			var adapter = new TestableLogAdapter(true, factory);
			Assert.IsTrue(adapter.IsWarnEnabled);
		}

		[Test]
		public void IsWarnEnabled_LoggingIsNotEnabledOnUnderlyingWrapper_ReturnsFalse() {
			var factory = A.Fake<ILogEntryFactory>();
			var adapter = new TestableLogAdapter(false, factory);
			Assert.IsFalse(adapter.IsWarnEnabled);
		}

		[Test]
		public void Error_SendInAnyMessage_WriteErrorEntryToUnderlyingClass() {
			var factory = A.Fake<ILogEntryFactory>();
			var logEntry = new LogEntry();
			A.CallTo(() => factory.Create(TraceEventType.Error, A<object[]>.Ignored)).Returns(logEntry);
			var adapter = new TestableLogAdapter(factory);

			adapter.Error("Any message");

			Assert.AreSame(logEntry, adapter.WrittenLogEntry);
		}

		[Test]
		public void Error_SendInAnyMessageAndException_WriteErrorEntryToUnderlyingClass() {
			var factory = A.Fake<ILogEntryFactory>();
			var logEntry = new LogEntry();
			A.CallTo(() => factory.Create(TraceEventType.Error, A<object[]>.Ignored)).Returns(logEntry);
			var adapter = new TestableLogAdapter(factory);

			adapter.Error("Any message", new Exception());

			Assert.AreSame(logEntry, adapter.WrittenLogEntry);
		}

		[Test]
		public void ErrorFormat_SendInCurrentCultureAndFormatAndAnyMessageAndException_WriteErrorEntryToUnderlyingClass() {
			var factory = A.Fake<ILogEntryFactory>();
			var logEntry = new LogEntry();
			A.CallTo(() => factory.Create(TraceEventType.Error, CultureInfo.CurrentCulture, "{0}", A<object[]>.Ignored)).Returns(logEntry);
			var adapter = new TestableLogAdapter(factory);

			adapter.ErrorFormat(CultureInfo.CurrentCulture, "{0}", "Any message");

			Assert.AreSame(logEntry, adapter.WrittenLogEntry);
		}

		[Test]
		public void ErrorFormat_SendInFormatAndAnyMessageAndException_WriteErrorEntryToUnderlyingClass() {
			var factory = A.Fake<ILogEntryFactory>();
			var logEntry = new LogEntry();
			A.CallTo(() => factory.Create(TraceEventType.Error, "{0}", A<object[]>.Ignored)).Returns(logEntry);
			var adapter = new TestableLogAdapter(factory);

			adapter.ErrorFormat("{0}", "Any message");

			Assert.AreSame(logEntry, adapter.WrittenLogEntry);
		}

		[Test]
		public void IsErrorEnabled_LoggingIsEnabledOnUnderlyingWrapper_ReturnsTrue() {
			var factory = A.Fake<ILogEntryFactory>();
			var adapter = new TestableLogAdapter(true, factory);
			Assert.IsTrue(adapter.IsErrorEnabled);
		}

		[Test]
		public void IsErrorEnabled_LoggingIsNotEnabledOnUnderlyingWrapper_ReturnsFalse() {
			var factory = A.Fake<ILogEntryFactory>();
			var adapter = new TestableLogAdapter(false, factory);
			Assert.IsFalse(adapter.IsErrorEnabled);
		}

		[Test]
		public void Fatal_SendInAnyMessage_WriteCriticalEntryToUnderlyingClass() {
			var factory = A.Fake<ILogEntryFactory>();
			var logEntry = new LogEntry();
			A.CallTo(() => factory.Create(TraceEventType.Critical, A<object[]>.Ignored)).Returns(logEntry);
			var adapter = new TestableLogAdapter(factory);

			adapter.Fatal("Any message");

			Assert.AreSame(logEntry, adapter.WrittenLogEntry);
		}

		[Test]
		public void Fatal_SendInAnyMessageAndException_WriteCriticalEntryToUnderlyingClass() {
			var factory = A.Fake<ILogEntryFactory>();
			var logEntry = new LogEntry();
			A.CallTo(() => factory.Create(TraceEventType.Critical, A<object[]>.Ignored)).Returns(logEntry);
			var adapter = new TestableLogAdapter(factory);

			adapter.Fatal("Any message", new Exception());

			Assert.AreSame(logEntry, adapter.WrittenLogEntry);
		}

		[Test]
		public void FatalFormat_SendInCurrentCultureAndFormatAndAnyMessageAndException_WriteCriticalEntryToUnderlyingClass() {
			var factory = A.Fake<ILogEntryFactory>();
			var logEntry = new LogEntry();
			A.CallTo(() => factory.Create(TraceEventType.Critical, CultureInfo.CurrentCulture, "{0}", A<object[]>.Ignored)).Returns(logEntry);
			var adapter = new TestableLogAdapter(factory);

			adapter.FatalFormat(CultureInfo.CurrentCulture, "{0}", "Any message");

			Assert.AreSame(logEntry, adapter.WrittenLogEntry);
		}

		[Test]
		public void FatalFormat_SendInFormatAndAnyMessageAndException_WriteCriticalEntryToUnderlyingClass() {
			var factory = A.Fake<ILogEntryFactory>();
			var logEntry = new LogEntry();
			A.CallTo(() => factory.Create(TraceEventType.Critical, "{0}", A<object[]>.Ignored)).Returns(logEntry);
			var adapter = new TestableLogAdapter(factory);

			adapter.FatalFormat("{0}", "Any message");

			Assert.AreSame(logEntry, adapter.WrittenLogEntry);
		}

		[Test]
		public void IsFatalEnabled_LoggingIsEnabledOnUnderlyingWrapper_ReturnsTrue() {
			var factory = A.Fake<ILogEntryFactory>();
			var adapter = new TestableLogAdapter(true, factory);
			Assert.IsTrue(adapter.IsFatalEnabled);
		}

		[Test]
		public void IsFatalEnabled_LoggingIsNotEnabledOnUnderlyingWrapper_ReturnsFalse() {
			var factory = A.Fake<ILogEntryFactory>();
			var adapter = new TestableLogAdapter(false, factory);
			Assert.IsFalse(adapter.IsFatalEnabled);
		}


		public class TestableLogAdapter : LogAdapter {
			private LogEntry writtenLogEntry;
			private readonly bool loggingEnabled;
			public TestableLogAdapter(ILogEntryFactory logEntryFactor) : this(true, logEntryFactor) {}
			public TestableLogAdapter(bool loggingEnabled, ILogEntryFactory logEntryFactor) : base(logEntryFactor) {
				this.loggingEnabled = loggingEnabled;
			}

			public LogEntry WrittenLogEntry { get { return writtenLogEntry; } }

			public override void Write(LogEntry log) {
				writtenLogEntry = log;
			}

			public override bool IsLoggingEnabled() {
				return loggingEnabled;
			}
		}
	}
}