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

namespace nJupiter.Abstraction.Logging.EnterpriseLibrary {
	public class LogAdapter : LoggerWrapper, ILog {

		private readonly ILogEntryFactory logEntryFactory;
		private readonly string loggerName;

		public LogAdapter(string loggerName) : this(new LogEntryFactory(loggerName)) {
			this.loggerName = loggerName;
		}

		public LogAdapter(ILogEntryFactory logEntryFactory) {
			this.logEntryFactory = logEntryFactory;
		}

		public string LoggerName { get { return loggerName; } }

		public void Debug(object message) {
			Write(logEntryFactory.Create(TraceEventType.Verbose, message));
		}

		public void Debug(object message, Exception exception) {
			Write(logEntryFactory.Create(TraceEventType.Verbose, message, exception));
		}

		public void DebugFormat(string format, params object[] args) {
			Write(logEntryFactory.Create(TraceEventType.Verbose, format, args));
		}

		public void DebugFormat(IFormatProvider provider, string format, params object[] args) {
			Write(logEntryFactory.Create(TraceEventType.Verbose, provider, format, args));
		}

		public void Info(object message) {
			Write(logEntryFactory.Create(TraceEventType.Information, message));
		}

		public void Info(object message, Exception exception) {
			Write(logEntryFactory.Create(TraceEventType.Information, message, exception));
		}

		public void InfoFormat(string format, params object[] args) {
			Write(logEntryFactory.Create(TraceEventType.Information, format, args));
		}

		public void InfoFormat(IFormatProvider provider, string format, params object[] args) {
			Write(logEntryFactory.Create(TraceEventType.Information, provider, format, args));
		}

		public void Warn(object message) {
			Write(logEntryFactory.Create(TraceEventType.Warning, message));
		}

		public void Warn(object message, Exception exception) {
			Write(logEntryFactory.Create(TraceEventType.Warning, message, exception));
		}

		public void WarnFormat(string format, params object[] args) {
			Write(logEntryFactory.Create(TraceEventType.Warning, format, args));
		}

		public void WarnFormat(IFormatProvider provider, string format, params object[] args) {
			Write(logEntryFactory.Create(TraceEventType.Warning, provider, format, args));
		}

		public void Error(object message) {
			Write(logEntryFactory.Create(TraceEventType.Error, message));
		}

		public void Error(object message, Exception exception) {
			Write(logEntryFactory.Create(TraceEventType.Error, message, exception));
		}

		public void ErrorFormat(string format, params object[] args) {
			Write(logEntryFactory.Create(TraceEventType.Error, format, args));
		}

		public void ErrorFormat(IFormatProvider provider, string format, params object[] args) {
			Write(logEntryFactory.Create(TraceEventType.Error, provider, format, args));
		}

		public void Fatal(object message) {
			Write(logEntryFactory.Create(TraceEventType.Critical, message));
		}

		public void Fatal(object message, Exception exception) {
			Write(logEntryFactory.Create(TraceEventType.Critical, message, exception));
		}

		public void FatalFormat(string format, params object[] args) {
			Write(logEntryFactory.Create(TraceEventType.Critical, format, args));
		}

		public void FatalFormat(IFormatProvider provider, string format, params object[] args) {
			Write(logEntryFactory.Create(TraceEventType.Critical, provider, format, args));
		}

		public bool IsDebugEnabled { get { return IsLoggingEnabled(); } }

		public bool IsInfoEnabled { get { return IsLoggingEnabled(); } }

		public bool IsWarnEnabled { get { return IsLoggingEnabled(); } }

		public bool IsErrorEnabled { get { return IsLoggingEnabled(); } }

		public bool IsFatalEnabled { get { return IsLoggingEnabled(); } }
	}
}