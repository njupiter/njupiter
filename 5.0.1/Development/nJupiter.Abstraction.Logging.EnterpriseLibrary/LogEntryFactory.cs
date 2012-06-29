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

using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace nJupiter.Abstraction.Logging.EnterpriseLibrary {
	public class LogEntryFactory : ILogEntryFactory {
		private readonly string loggerName;

		public LogEntryFactory(string loggerName) {
			this.loggerName = loggerName;
		}

		public LogEntry Create(TraceEventType severity, IFormatProvider provider, string format, params object[] args) {
			var entry = new LogEntry();
			entry.Title = loggerName;
			entry.Severity = severity;
			entry.Message = string.Format(provider, format, args);
			return entry;
		}

		public LogEntry Create(TraceEventType severity, string format, params object[] args) {
			return Create(severity, null, format, args);
		}

		public LogEntry Create(TraceEventType severity, params object[] args) {
			return Create(severity, "{0}", args);
		}
	}
}