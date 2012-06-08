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
using System.Xml;

using NUnit.Framework;

using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Repository.Hierarchy;

using nJupiter.Abstraction.Logging.Log4Net;

namespace nJupiter.Abstraction.Logging.Tests.Integration.Log4Net {
	[TestFixture]
	public class LogManagerAdapterTests {
		[Test]
		public void Debug_LogMessage_MessageWrittenToAdapter() {
			var memoryAppender = GetMemoryAppender();
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			logger.Debug("Any message");

			Assert.AreEqual("Any message", memoryAppender.GetEvents()[0].RenderedMessage);
		}

		[Test]
		public void Debug_LogMessageToCurrentClassAdapter_MessageWrittenCurrentClassLogger() {
			var memoryAppender = GetMemoryAppender();
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			logger.Debug("Any message");

			Assert.AreEqual(typeof(LogManagerAdapterTests).FullName, memoryAppender.GetEvents()[0].LoggerName);
		}

		[Test]
		public void Debug_LogMessage_MessageLevelSetToDebug() {
			var memoryAppender = GetMemoryAppender();
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			logger.Debug("Any message");

			Assert.AreEqual("DEBUG", memoryAppender.GetEvents()[0].Level.DisplayName);
		}

		[Test]
		public void Debug_LogMessageWithException_ExceptionLogged() {
			var memoryAppender = GetMemoryAppender();
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			logger.Debug("Any message", new Exception("Any exception message"));

			Assert.AreEqual("Any exception message", memoryAppender.GetEvents()[0].ExceptionObject.Message);
		}

		[Test]
		public void DebugFormat_LogMessageWithFormatProviderAndDateAndTellToFormatAsIso_DateFormatAfterIsoStandard() {
			var memoryAppender = GetMemoryAppender();
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			var date = new DateTime(2000, 1, 1);
			logger.DebugFormat(CultureInfo.InvariantCulture, "{0:s}", date);

			Assert.AreEqual("2000-01-01T00:00:00", memoryAppender.GetEvents()[0].RenderedMessage);
		}

		[Test]
		public void DebugFormat_LogMessageWithDateAndTellToFormatAsIso_DateFormatAfterIsoStandard() {
			var memoryAppender = GetMemoryAppender();
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			var date = new DateTime(2000, 1, 1);
			logger.DebugFormat("{0:s}", date);

			Assert.AreEqual("2000-01-01T00:00:00", memoryAppender.GetEvents()[0].RenderedMessage);
		}

		[Test]
		public void IsDebugEnabled_CreateAdapterWhereDebugIsEnabled_ReturnsTrue() {
			GetMemoryAppender("DEBUG");
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			Assert.IsTrue(logger.IsDebugEnabled);
		}

		[Test]
		public void IsDebugEnabled_CreateAdapterWhereDebugIsNotEnabled_ReturnsFalse() {
			GetMemoryAppender("FATAL");
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			Assert.IsFalse(logger.IsDebugEnabled);
		}

		[Test]
		public void Info_LogMessage_MessageWrittenToAdapter() {
			var memoryAppender = GetMemoryAppender();
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			logger.Info("Any message");

			Assert.AreEqual("Any message", memoryAppender.GetEvents()[0].RenderedMessage);
		}

		[Test]
		public void Info_LogMessageToCurrentClassAdapter_MessageWrittenCurrentClassLogger() {
			var memoryAppender = GetMemoryAppender();
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			logger.Info("Any message");

			Assert.AreEqual(typeof(LogManagerAdapterTests).FullName, memoryAppender.GetEvents()[0].LoggerName);
		}

		[Test]
		public void Info_LogMessage_MessageLevelSetToInfo() {
			var memoryAppender = GetMemoryAppender();
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			logger.Info("Any message");

			Assert.AreEqual("INFO", memoryAppender.GetEvents()[0].Level.DisplayName);
		}

		[Test]
		public void Info_LogMessageWithException_ExceptionLogged() {
			var memoryAppender = GetMemoryAppender();
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			logger.Info("Any message", new Exception("Any exception message"));

			Assert.AreEqual("Any exception message", memoryAppender.GetEvents()[0].ExceptionObject.Message);
		}

		[Test]
		public void InfoFormat_LogMessageWithFormatProviderAndDateAndTellToFormatAsIso_DateFormatAfterIsoStandard() {
			var memoryAppender = GetMemoryAppender();
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			var date = new DateTime(2000, 1, 1);
			logger.InfoFormat(CultureInfo.InvariantCulture, "{0:s}", date);

			Assert.AreEqual("2000-01-01T00:00:00", memoryAppender.GetEvents()[0].RenderedMessage);
		}

		[Test]
		public void InfoFormat_LogMessageWithDateAndTellToFormatAsIso_DateFormatAfterIsoStandard() {
			var memoryAppender = GetMemoryAppender();
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			var date = new DateTime(2000, 1, 1);
			logger.InfoFormat("{0:s}", date);

			Assert.AreEqual("2000-01-01T00:00:00", memoryAppender.GetEvents()[0].RenderedMessage);
		}

		[Test]
		public void IsInfoEnabled_CreateAdapterWhereInfoIsEnabled_ReturnsTrue() {
			GetMemoryAppender("INFO");
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			Assert.IsTrue(logger.IsInfoEnabled);
		}

		[Test]
		public void IsInfoEnabled_CreateAdapterWhereInfoIsNotEnabled_ReturnsFalse() {
			GetMemoryAppender("FATAL");
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			Assert.IsFalse(logger.IsInfoEnabled);
		}

		[Test]
		public void Warn_LogMessage_MessageWrittenToAdapter() {
			var memoryAppender = GetMemoryAppender();
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			logger.Warn("Any message");

			Assert.AreEqual("Any message", memoryAppender.GetEvents()[0].RenderedMessage);
		}

		[Test]
		public void Warn_LogMessageToCurrentClassAdapter_MessageWrittenCurrentClassLogger() {
			var memoryAppender = GetMemoryAppender();
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			logger.Warn("Any message");

			Assert.AreEqual(typeof(LogManagerAdapterTests).FullName, memoryAppender.GetEvents()[0].LoggerName);
		}

		[Test]
		public void Warn_LogMessage_MessageLevelSetToWarn() {
			var memoryAppender = GetMemoryAppender();
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			logger.Warn("Any message");

			Assert.AreEqual("WARN", memoryAppender.GetEvents()[0].Level.DisplayName);
		}

		[Test]
		public void Warn_LogMessageWithException_ExceptionLogged() {
			var memoryAppender = GetMemoryAppender();
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			logger.Warn("Any message", new Exception("Any exception message"));

			Assert.AreEqual("Any exception message", memoryAppender.GetEvents()[0].ExceptionObject.Message);
		}

		[Test]
		public void WarnFormat_LogMessageWithFormatProviderAndDateAndTellToFormatAsIso_DateFormatAfterIsoStandard() {
			var memoryAppender = GetMemoryAppender();
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			var date = new DateTime(2000, 1, 1);
			logger.WarnFormat(CultureInfo.InvariantCulture, "{0:s}", date);

			Assert.AreEqual("2000-01-01T00:00:00", memoryAppender.GetEvents()[0].RenderedMessage);
		}

		[Test]
		public void WarnFormat_LogMessageWithDateAndTellToFormatAsIso_DateFormatAfterIsoStandard() {
			var memoryAppender = GetMemoryAppender();
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			var date = new DateTime(2000, 1, 1);
			logger.WarnFormat("{0:s}", date);

			Assert.AreEqual("2000-01-01T00:00:00", memoryAppender.GetEvents()[0].RenderedMessage);
		}

		[Test]
		public void IsWarnEnabled_CreateAdapterWhereWarnIsEnabled_ReturnsTrue() {
			GetMemoryAppender("WARN");
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			Assert.IsTrue(logger.IsWarnEnabled);
		}

		[Test]
		public void IsWarnEnabled_CreateAdapterWhereWarnIsNotEnabled_ReturnsFalse() {
			GetMemoryAppender("FATAL");
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			Assert.IsFalse(logger.IsWarnEnabled);
		}

		[Test]
		public void Error_LogMessage_MessageWrittenToAdapter() {
			var memoryAppender = GetMemoryAppender();
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			logger.Error("Any message");

			Assert.AreEqual("Any message", memoryAppender.GetEvents()[0].RenderedMessage);
		}

		[Test]
		public void Error_LogMessageToCurrentClassAdapter_MessageWrittenCurrentClassLogger() {
			var memoryAppender = GetMemoryAppender();
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			logger.Error("Any message");

			Assert.AreEqual(typeof(LogManagerAdapterTests).FullName, memoryAppender.GetEvents()[0].LoggerName);
		}

		[Test]
		public void Error_LogMessage_MessageLevelSetToError() {
			var memoryAppender = GetMemoryAppender();
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			logger.Error("Any message");

			Assert.AreEqual("ERROR", memoryAppender.GetEvents()[0].Level.DisplayName);
		}

		[Test]
		public void Error_LogMessageWithException_ExceptionLogged() {
			var memoryAppender = GetMemoryAppender();
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			logger.Error("Any message", new Exception("Any exception message"));

			Assert.AreEqual("Any exception message", memoryAppender.GetEvents()[0].ExceptionObject.Message);
		}

		[Test]
		public void ErrorFormat_LogMessageWithFormatProviderAndDateAndTellToFormatAsIso_DateFormatAfterIsoStandard() {
			var memoryAppender = GetMemoryAppender();
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			var date = new DateTime(2000, 1, 1);
			logger.ErrorFormat(CultureInfo.InvariantCulture, "{0:s}", date);

			Assert.AreEqual("2000-01-01T00:00:00", memoryAppender.GetEvents()[0].RenderedMessage);
		}

		[Test]
		public void ErrorFormat_LogMessageWithDateAndTellToFormatAsIso_DateFormatAfterIsoStandard() {
			var memoryAppender = GetMemoryAppender();
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			var date = new DateTime(2000, 1, 1);
			logger.ErrorFormat("{0:s}", date);

			Assert.AreEqual("2000-01-01T00:00:00", memoryAppender.GetEvents()[0].RenderedMessage);
		}

		[Test]
		public void IsErrorEnabled_CreateAdapterWhereErrorIsEnabled_ReturnsTrue() {
			GetMemoryAppender("ERROR");
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			Assert.IsTrue(logger.IsErrorEnabled);
		}

		[Test]
		public void IsErrorEnabled_CreateAdapterWhereErrorIsNotEnabled_ReturnsFalse() {
			GetMemoryAppender("FATAL");
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			Assert.IsFalse(logger.IsErrorEnabled);
		}

		[Test]
		public void Fatal_LogMessage_MessageWrittenToAdapter() {
			var memoryAppender = GetMemoryAppender();
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			logger.Fatal("Any message");

			Assert.AreEqual("Any message", memoryAppender.GetEvents()[0].RenderedMessage);
		}

		[Test]
		public void Fatal_LogMessageToCurrentClassAdapter_MessageWrittenCurrentClassLogger() {
			var memoryAppender = GetMemoryAppender();
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			logger.Fatal("Any message");

			Assert.AreEqual(typeof(LogManagerAdapterTests).FullName, memoryAppender.GetEvents()[0].LoggerName);
		}

		[Test]
		public void Fatal_LogMessage_MessageLevelSetToFatal() {
			var memoryAppender = GetMemoryAppender();
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			logger.Fatal("Any message");

			Assert.AreEqual("FATAL", memoryAppender.GetEvents()[0].Level.DisplayName);
		}

		[Test]
		public void Fatal_LogMessageWithException_ExceptionLogged() {
			var memoryAppender = GetMemoryAppender();
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			logger.Fatal("Any message", new Exception("Any exception message"));

			Assert.AreEqual("Any exception message", memoryAppender.GetEvents()[0].ExceptionObject.Message);
		}

		[Test]
		public void FatalFormat_LogMessageWithFormatProviderAndDateAndTellToFormatAsIso_DateFormatAfterIsoStandard() {
			var memoryAppender = GetMemoryAppender();
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			var date = new DateTime(2000, 1, 1);
			logger.FatalFormat(CultureInfo.InvariantCulture, "{0:s}", date);

			Assert.AreEqual("2000-01-01T00:00:00", memoryAppender.GetEvents()[0].RenderedMessage);
		}

		[Test]
		public void FatalFormat_LogMessageWithDateAndTellToFormatAsIso_DateFormatAfterIsoStandard() {
			var memoryAppender = GetMemoryAppender();
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			var date = new DateTime(2000, 1, 1);
			logger.FatalFormat("{0:s}", date);

			Assert.AreEqual("2000-01-01T00:00:00", memoryAppender.GetEvents()[0].RenderedMessage);
		}

		[Test]
		public void IsFatalEnabled_CreateAdapterWhereFatalIsEnabled_ReturnsTrue() {
			GetMemoryAppender("FATAL");
			var log4netManagerAdapter = new LogManagerAdapter();
			var logger = log4netManagerAdapter.GetCurrentClassLogger();

			Assert.IsTrue(logger.IsFatalEnabled);
		}

		private static MemoryAppender GetMemoryAppender() {
			return GetMemoryAppender("DEBUG");
		}

		private static MemoryAppender GetMemoryAppender(string level) {
			var document = new XmlDocument();
			document.LoadXml(
			                 "<config>" +
			                 "	<log4net>" +
			                 "		<root>" +
			                 "			<appender-ref ref=\"MemoryAppender\" />" +
			                 "			<level value=\"" + level + "\" />" +
			                 "		</root>" +
			                 "		<appender name=\"MemoryAppender\" type=\"log4net.Appender.MemoryAppender\">" +
			                 "		</appender>" +
			                 "	</log4net>" +
			                 "</config>");
			XmlConfigurator.Configure((XmlElement)document.DocumentElement.SelectSingleNode("log4net"));
			var hierarchy = LogManager.GetRepository() as Hierarchy;
			var memoryAppender = hierarchy.Root.GetAppender("MemoryAppender") as MemoryAppender;
			return memoryAppender;
		}
	}
}