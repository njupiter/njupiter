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

using NLog;

namespace nJupiter.Abstraction.Logging.NLog {
	public class LogAdapter : LogWrapper, ILog {
		public LogAdapter(Logger log) : base(log) {}

		public void Debug(object message, Exception exception) {
			log.DebugException(string.Format("{0}", message), exception);
		}

		public void DebugFormat(string format, params object[] args) {
			log.Debug(string.Format(format, args));
		}

		public void DebugFormat(IFormatProvider provider, string format, params object[] args) {
			log.Debug(string.Format(provider, format, args));
		}

		public void Info(object message, Exception exception) {
			log.InfoException(string.Format("{0}", message), exception);
		}

		public void InfoFormat(string format, params object[] args) {
			log.Info(string.Format(format, args));
		}

		public void InfoFormat(IFormatProvider provider, string format, params object[] args) {
			log.Info(string.Format(provider, format, args));
		}

		public void Warn(object message, Exception exception) {
			log.WarnException(string.Format("{0}", message), exception);
		}

		public void WarnFormat(string format, params object[] args) {
			log.Warn(string.Format(format, args));
		}

		public void WarnFormat(IFormatProvider provider, string format, params object[] args) {
			log.Warn(string.Format(provider, format, args));
		}

		public void Error(object message, Exception exception) {
			log.ErrorException(string.Format("{0}", message), exception);
		}

		public void ErrorFormat(string format, params object[] args) {
			log.Error(string.Format(format, args));
		}

		public void ErrorFormat(IFormatProvider provider, string format, params object[] args) {
			log.Error(string.Format(provider, format, args));
		}

		public void Fatal(object message, Exception exception) {
			log.FatalException(string.Format("{0}", message), exception);
		}

		public void FatalFormat(string format, params object[] args) {
			log.Fatal(string.Format(format, args));
		}

		public void FatalFormat(IFormatProvider provider, string format, params object[] args) {
			log.Fatal(string.Format(provider, format, args));
		}
	}
}