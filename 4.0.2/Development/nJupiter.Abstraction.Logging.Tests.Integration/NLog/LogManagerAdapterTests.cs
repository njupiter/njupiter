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
using System.Globalization;

using NLog;
using NLog.Config;
using NLog.Targets;

using NUnit.Framework;

using nJupiter.Abstraction.Logging.NLog;

namespace nJupiter.Abstraction.Logging.Tests.Integration.NLog {
	[TestFixture]
	public class LogManagerAdapterTests {
		[Test]
		public void Debug_LogMessage_MessageWrittenToAdapter() {
			var memoryAppender = GetMemoryAppender("${message}");
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			logger.Debug("Any message");

			Assert.AreEqual("Any message", memoryAppender.Logs[0]);
		}

		[Test]
		public void Debug_LogMessageToCurrentClassAdapter_MessageWrittenCurrentClassLogger() {
			var memoryAppender = GetMemoryAppender("${logger}");
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			logger.Debug("Any message");

			Assert.AreEqual(typeof(LogManagerAdapterTests).FullName, memoryAppender.Logs[0]);
		}

		[Test]
		public void Debug_LogMessage_MessageLevelSetToDebug() {
			var memoryAppender = GetMemoryAppender("${level}");
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			logger.Debug("Any message");

			Assert.AreEqual("Debug", memoryAppender.Logs[0]);
		}

		[Test]
		public void Debug_LogMessageWithException_ExceptionLogged() {
			var memoryAppender = GetMemoryAppender("${exception}");
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			logger.Debug("Any message", new Exception("Any exception message"));

			Assert.AreEqual("Any exception message", memoryAppender.Logs[0]);
		}

		[Test]
		public void DebugFormat_LogMessageWithFormatProviderAndDateAndTellToFormatAsIso_DateFormatAfterIsoStandard() {
			var memoryAppender = GetMemoryAppender("${message}");
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			var date = new DateTime(2000, 1, 1);
			logger.DebugFormat(CultureInfo.InvariantCulture, "{0:s}", date);

			Assert.AreEqual("2000-01-01T00:00:00", memoryAppender.Logs[0]);
		}

		[Test]
		public void DebugFormat_LogMessageWithDateAndTellToFormatAsIso_DateFormatAfterIsoStandard() {
			var memoryAppender = GetMemoryAppender("${message}");
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			var date = new DateTime(2000, 1, 1);
			logger.DebugFormat("{0:s}", date);

			Assert.AreEqual("2000-01-01T00:00:00", memoryAppender.Logs[0]);
		}

		[Test]
		public void IsDebugEnabled_CreateAdapterWhereDebugIsEnabled_ReturnsTrue() {
			GetMemoryAppender("${message}", LogLevel.Debug);
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			Assert.IsTrue(logger.IsDebugEnabled);
		}

		[Test]
		public void IsDebugEnabled_CreateAdapterWhereDebugIsNotEnabled_ReturnsFalse() {
			GetMemoryAppender("${message}", LogLevel.Fatal);
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			Assert.IsFalse(logger.IsDebugEnabled);
		}

		[Test]
		public void Info_LogMessage_MessageWrittenToAdapter() {
			var memoryAppender = GetMemoryAppender("${message}");
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			logger.Info("Any message");

			Assert.AreEqual("Any message", memoryAppender.Logs[0]);
		}

		[Test]
		public void Info_LogMessageToCurrentClassAdapter_MessageWrittenCurrentClassLogger() {
			var memoryAppender = GetMemoryAppender("${logger}");
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			logger.Info("Any message");

			Assert.AreEqual(typeof(LogManagerAdapterTests).FullName, memoryAppender.Logs[0]);
		}

		[Test]
		public void Info_LogMessage_MessageLevelSetToInfo() {
			var memoryAppender = GetMemoryAppender("${level}");
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			logger.Info("Any message");

			Assert.AreEqual("Info", memoryAppender.Logs[0]);
		}

		[Test]
		public void Info_LogMessageWithException_ExceptionLogged() {
			var memoryAppender = GetMemoryAppender("${exception}");
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			logger.Info("Any message", new Exception("Any exception message"));

			Assert.AreEqual("Any exception message", memoryAppender.Logs[0]);
		}

		[Test]
		public void InfoFormat_LogMessageWithFormatProviderAndDateAndTellToFormatAsIso_DateFormatAfterIsoStandard() {
			var memoryAppender = GetMemoryAppender("${message}");
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			var date = new DateTime(2000, 1, 1);
			logger.InfoFormat(CultureInfo.InvariantCulture, "{0:s}", date);

			Assert.AreEqual("2000-01-01T00:00:00", memoryAppender.Logs[0]);
		}

		[Test]
		public void InfoFormat_LogMessageWithDateAndTellToFormatAsIso_DateFormatAfterIsoStandard() {
			var memoryAppender = GetMemoryAppender("${message}");
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			var date = new DateTime(2000, 1, 1);
			logger.InfoFormat("{0:s}", date);

			Assert.AreEqual("2000-01-01T00:00:00", memoryAppender.Logs[0]);
		}

		[Test]
		public void IsInfoEnabled_CreateAdapterWhereInfoIsEnabled_ReturnsTrue() {
			GetMemoryAppender("${message}", LogLevel.Info);
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			Assert.IsTrue(logger.IsInfoEnabled);
		}

		[Test]
		public void IsInfoEnabled_CreateAdapterWhereInfoIsNotEnabled_ReturnsFalse() {
			GetMemoryAppender("${message}", LogLevel.Fatal);
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			Assert.IsFalse(logger.IsInfoEnabled);
		}

		[Test]
		public void Warn_LogMessage_MessageWrittenToAdapter() {
			var memoryAppender = GetMemoryAppender("${message}");
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			logger.Warn("Any message");

			Assert.AreEqual("Any message", memoryAppender.Logs[0]);
		}

		[Test]
		public void Warn_LogMessageToCurrentClassAdapter_MessageWrittenCurrentClassLogger() {
			var memoryAppender = GetMemoryAppender("${logger}");
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			logger.Warn("Any message");

			Assert.AreEqual(typeof(LogManagerAdapterTests).FullName, memoryAppender.Logs[0]);
		}

		[Test]
		public void Warn_LogMessage_MessageLevelSetToWarn() {
			var memoryAppender = GetMemoryAppender("${level}");
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			logger.Warn("Any message");

			Assert.AreEqual("Warn", memoryAppender.Logs[0]);
		}

		[Test]
		public void Warn_LogMessageWithException_ExceptionLogged() {
			var memoryAppender = GetMemoryAppender("${exception}");
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			logger.Warn("Any message", new Exception("Any exception message"));

			Assert.AreEqual("Any exception message", memoryAppender.Logs[0]);
		}

		[Test]
		public void WarnFormat_LogMessageWithFormatProviderAndDateAndTellToFormatAsIso_DateFormatAfterIsoStandard() {
			var memoryAppender = GetMemoryAppender("${message}");
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			var date = new DateTime(2000, 1, 1);
			logger.WarnFormat(CultureInfo.InvariantCulture, "{0:s}", date);

			Assert.AreEqual("2000-01-01T00:00:00", memoryAppender.Logs[0]);
		}

		[Test]
		public void WarnFormat_LogMessageWithDateAndTellToFormatAsIso_DateFormatAfterIsoStandard() {
			var memoryAppender = GetMemoryAppender("${message}");
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			var date = new DateTime(2000, 1, 1);
			logger.WarnFormat("{0:s}", date);

			Assert.AreEqual("2000-01-01T00:00:00", memoryAppender.Logs[0]);
		}

		[Test]
		public void IsWarnEnabled_CreateAdapterWhereWarnIsEnabled_ReturnsTrue() {
			GetMemoryAppender("${message}", LogLevel.Warn);
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			Assert.IsTrue(logger.IsWarnEnabled);
		}

		[Test]
		public void IsWarnEnabled_CreateAdapterWhereWarnIsNotEnabled_ReturnsFalse() {
			GetMemoryAppender("${message}", LogLevel.Fatal);
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			Assert.IsFalse(logger.IsWarnEnabled);
		}

		[Test]
		public void Error_LogMessage_MessageWrittenToAdapter() {
			var memoryAppender = GetMemoryAppender("${message}");
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			logger.Error("Any message");

			Assert.AreEqual("Any message", memoryAppender.Logs[0]);
		}

		[Test]
		public void Error_LogMessageToCurrentClassAdapter_MessageWrittenCurrentClassLogger() {
			var memoryAppender = GetMemoryAppender("${logger}");
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			logger.Error("Any message");

			Assert.AreEqual(typeof(LogManagerAdapterTests).FullName, memoryAppender.Logs[0]);
		}

		[Test]
		public void Error_LogMessage_MessageLevelSetToError() {
			var memoryAppender = GetMemoryAppender("${level}");
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			logger.Error("Any message");

			Assert.AreEqual("Error", memoryAppender.Logs[0]);
		}

		[Test]
		public void Error_LogMessageWithException_ExceptionLogged() {
			var memoryAppender = GetMemoryAppender("${exception}");
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			logger.Error("Any message", new Exception("Any exception message"));

			Assert.AreEqual("Any exception message", memoryAppender.Logs[0]);
		}

		[Test]
		public void ErrorFormat_LogMessageWithFormatProviderAndDateAndTellToFormatAsIso_DateFormatAfterIsoStandard() {
			var memoryAppender = GetMemoryAppender("${message}");
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			var date = new DateTime(2000, 1, 1);
			logger.ErrorFormat(CultureInfo.InvariantCulture, "{0:s}", date);

			Assert.AreEqual("2000-01-01T00:00:00", memoryAppender.Logs[0]);
		}

		[Test]
		public void ErrorFormat_LogMessageWithDateAndTellToFormatAsIso_DateFormatAfterIsoStandard() {
			var memoryAppender = GetMemoryAppender("${message}");
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			var date = new DateTime(2000, 1, 1);
			logger.ErrorFormat("{0:s}", date);

			Assert.AreEqual("2000-01-01T00:00:00", memoryAppender.Logs[0]);
		}

		[Test]
		public void IsErrorEnabled_CreateAdapterWhereErrorIsEnabled_ReturnsTrue() {
			GetMemoryAppender("${message}", LogLevel.Error);
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			Assert.IsTrue(logger.IsErrorEnabled);
		}

		[Test]
		public void IsErrorEnabled_CreateAdapterWhereErrorIsNotEnabled_ReturnsFalse() {
			GetMemoryAppender("${message}", LogLevel.Fatal);
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			Assert.IsFalse(logger.IsErrorEnabled);
		}

		[Test]
		public void Fatal_LogMessage_MessageWrittenToAdapter() {
			var memoryAppender = GetMemoryAppender("${message}");
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			logger.Fatal("Any message");

			Assert.AreEqual("Any message", memoryAppender.Logs[0]);
		}

		[Test]
		public void Fatal_LogMessageToCurrentClassAdapter_MessageWrittenCurrentClassLogger() {
			var memoryAppender = GetMemoryAppender("${logger}");
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			logger.Fatal("Any message");

			Assert.AreEqual(typeof(LogManagerAdapterTests).FullName, memoryAppender.Logs[0]);
		}

		[Test]
		public void Fatal_LogMessage_MessageLevelSetToFatal() {
			var memoryAppender = GetMemoryAppender("${level}");
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			logger.Fatal("Any message");

			Assert.AreEqual("Fatal", memoryAppender.Logs[0]);
		}

		[Test]
		public void Fatal_LogMessageWithException_ExceptionLogged() {
			var memoryAppender = GetMemoryAppender("${exception}");
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			logger.Fatal("Any message", new Exception("Any exception message"));

			Assert.AreEqual("Any exception message", memoryAppender.Logs[0]);
		}

		[Test]
		public void FatalFormat_LogMessageWithFormatProviderAndDateAndTellToFormatAsIso_DateFormatAfterIsoStandard() {
			var memoryAppender = GetMemoryAppender("${message}");
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			var date = new DateTime(2000, 1, 1);
			logger.FatalFormat(CultureInfo.InvariantCulture, "{0:s}", date);

			Assert.AreEqual("2000-01-01T00:00:00", memoryAppender.Logs[0]);
		}

		[Test]
		public void FatalFormat_LogMessageWithDateAndTellToFormatAsIso_DateFormatAfterIsoStandard() {
			var memoryAppender = GetMemoryAppender("${message}");
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			var date = new DateTime(2000, 1, 1);
			logger.FatalFormat("{0:s}", date);

			Assert.AreEqual("2000-01-01T00:00:00", memoryAppender.Logs[0]);
		}

		[Test]
		public void IsFatalEnabled_CreateAdapterWhereFatalIsEnabled_ReturnsTrue() {
			GetMemoryAppender("${message}", LogLevel.Fatal);
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			Assert.IsTrue(logger.IsFatalEnabled);
		}

		private static MemoryTarget GetMemoryAppender(string layout) {
			return GetMemoryAppender(layout, LogLevel.Debug);
		}

		private static MemoryTarget GetMemoryAppender(string layout, LogLevel level) {
			var config = new LoggingConfiguration();
			var memoryTarget = new MemoryTarget();
			config.AddTarget("memoryT", memoryTarget);
			memoryTarget.Layout = layout;

			var rule1 = new LoggingRule("*", level, memoryTarget);
			config.LoggingRules.Add(rule1);
   
			LogManager.Configuration = config;
			return memoryTarget;
		}
	}
}