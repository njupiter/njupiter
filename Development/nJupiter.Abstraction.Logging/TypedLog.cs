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

namespace nJupiter.Abstraction.Logging {
	public class TypedLog<T> : ILog<T> {
		private readonly ILog log;

		public TypedLog(ILogManager logManager) {
			log = logManager.GetLogger(typeof(T));
		}

		public void Debug(object message, Exception exception) {
			log.Debug(message, exception);
		}

		public void Debug(object message) {
			log.Debug(message);
		}

		public void DebugFormat(IFormatProvider provider, string format, params object[] args) {
			log.DebugFormat(provider, format, args);
		}

		public void DebugFormat(string format, params object[] args) {
			log.DebugFormat(format, args);
		}

		public void Error(object message) {
			log.Error(message);
		}

		public void Error(object message, Exception exception) {
			log.Error(message, exception);
		}

		public void ErrorFormat(string format, params object[] args) {
			log.ErrorFormat(format, args);
		}

		public void ErrorFormat(IFormatProvider provider, string format, params object[] args) {
			log.ErrorFormat(provider, format, args);
		}

		public void Fatal(object message) {
			log.Fatal(message);
		}

		public void Fatal(object message, Exception exception) {
			log.Fatal(message, exception);
		}

		public void FatalFormat(IFormatProvider provider, string format, params object[] args) {
			log.FatalFormat(provider, format, args);
		}

		public void FatalFormat(string format, params object[] args) {
			log.FatalFormat(format, args);
		}

		public void Info(object message) {
			log.Info(message);
		}

		public void Info(object message, Exception exception) {
			log.Info(message, exception);
		}

		public void InfoFormat(string format, params object[] args) {
			log.InfoFormat(format, args);
		}

		public void InfoFormat(IFormatProvider provider, string format, params object[] args) {
			log.InfoFormat(provider, format, args);
		}

		public bool IsDebugEnabled { get { return log.IsDebugEnabled; } }

		public bool IsErrorEnabled { get { return log.IsErrorEnabled; } }

		public bool IsFatalEnabled { get { return log.IsFatalEnabled; } }

		public bool IsInfoEnabled { get { return log.IsInfoEnabled; } }

		public bool IsWarnEnabled { get { return log.IsWarnEnabled; } }

		public void Warn(object message) {
			log.Warn(message);
		}

		public void Warn(object message, Exception exception) {
			log.Warn(message, exception);
		}

		public void WarnFormat(IFormatProvider provider, string format, params object[] args) {
			log.WarnFormat(provider, format, args);
		}

		public void WarnFormat(string format, params object[] args) {
			log.WarnFormat(format, args);
		}

		public ILog BaseLog { get { return log; } }
	}
}