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

namespace nJupiter.Abstraction.Logging.NullLogger {
	internal class NullLog : ILog {
		public void Debug(object message) {}
		public void Debug(object message, Exception exception) {}
		public void DebugFormat(string format, params object[] args) {}
		public void DebugFormat(IFormatProvider provider, string format, params object[] args) {}
		public void Info(object message) {}
		public void Info(object message, Exception exception) {}
		public void InfoFormat(string format, params object[] args) {}
		public void InfoFormat(IFormatProvider provider, string format, params object[] args) {}
		public void Warn(object message) {}
		public void Warn(object message, Exception exception) {}
		public void WarnFormat(string format, params object[] args) {}
		public void WarnFormat(IFormatProvider provider, string format, params object[] args) {}
		public void Error(object message) {}
		public void Error(object message, Exception exception) {}
		public void ErrorFormat(string format, params object[] args) {}
		public void ErrorFormat(IFormatProvider provider, string format, params object[] args) {}
		public void Fatal(object message) {}
		public void Fatal(object message, Exception exception) {}
		public void FatalFormat(string format, params object[] args) {}
		public void FatalFormat(IFormatProvider provider, string format, params object[] args) {}

		public bool IsDebugEnabled { get { return false; } }
		public bool IsInfoEnabled { get { return false; } }
		public bool IsWarnEnabled { get { return false; } }
		public bool IsErrorEnabled { get { return false; } }
		public bool IsFatalEnabled { get { return false; } }
	}
}