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
	public static class LogExtensions {

		public delegate void MessageHandler(object message);

		public delegate void ExcetpionMessageHandler(object message, Exception exception);

		public delegate void FormatMessageHandler(string format, params object[] args);

		public delegate void FormatProviderMessageHandler(IFormatProvider provider, string format, params object[] args);

		public static void Debug(this ILog log, Action<MessageHandler> callback) {
			if(log.IsDebugEnabled) {
				callback(log.Debug);
			}
		}

		public static void Debug(this ILog log, Action<ExcetpionMessageHandler> callback) {
			if(log.IsDebugEnabled) {
				callback(log.Debug);
			}
		}

		public static void DebugFormat(this ILog log, Action<FormatMessageHandler> callback) {
			if(log.IsDebugEnabled) {
				callback(log.DebugFormat);
			}
		}

		public static void DebugFormat(this ILog log, Action<FormatProviderMessageHandler> callback) {
			if(log.IsDebugEnabled) {
				callback(log.DebugFormat);
			}
		}

		public static void Info(this ILog log, Action<MessageHandler> callback) {
			if(log.IsInfoEnabled) {
				callback(log.Info);
			}
		}

		public static void Info(this ILog log, Action<ExcetpionMessageHandler> callback) {
			if(log.IsInfoEnabled) {
				callback(log.Info);
			}
		}

		public static void InfoFormat(this ILog log, Action<FormatMessageHandler> callback) {
			if(log.IsInfoEnabled) {
				callback(log.InfoFormat);
			}
		}

		public static void InfoFormat(this ILog log, Action<FormatProviderMessageHandler> callback) {
			if(log.IsInfoEnabled) {
				callback(log.InfoFormat);
			}
		}

		public static void Warn(this ILog log, Action<MessageHandler> callback) {
			if(log.IsWarnEnabled) {
				callback(log.Warn);
			}
		}

		public static void Warn(this ILog log, Action<ExcetpionMessageHandler> callback) {
			if(log.IsWarnEnabled) {
				callback(log.Warn);
			}
		}

		public static void WarnFormat(this ILog log, Action<FormatMessageHandler> callback) {
			if(log.IsWarnEnabled) {
				callback(log.WarnFormat);
			}
		}

		public static void WarnFormat(this ILog log, Action<FormatProviderMessageHandler> callback) {
			if(log.IsWarnEnabled) {
				callback(log.WarnFormat);
			}
		}

		public static void Error(this ILog log, Action<MessageHandler> callback) {
			if(log.IsErrorEnabled) {
				callback(log.Error);
			}
		}

		public static void Error(this ILog log, Action<ExcetpionMessageHandler> callback) {
			if(log.IsErrorEnabled) {
				callback(log.Error);
			}
		}

		public static void ErrorFormat(this ILog log, Action<FormatMessageHandler> callback) {
			if(log.IsErrorEnabled) {
				callback(log.ErrorFormat);
			}
		}

		public static void ErrorFormat(this ILog log, Action<FormatProviderMessageHandler> callback) {
			if(log.IsErrorEnabled) {
				callback(log.ErrorFormat);
			}
		}

		public static void Fatal(this ILog log, Action<MessageHandler> callback) {
			if(log.IsFatalEnabled) {
				callback(log.Fatal);
			}
		}

		public static void Fatal(this ILog log, Action<ExcetpionMessageHandler> callback) {
			if(log.IsFatalEnabled) {
				callback(log.Fatal);
			}
		}

		public static void FatalFormat(this ILog log, Action<FormatMessageHandler> callback) {
			if(log.IsFatalEnabled) {
				callback(log.FatalFormat);
			}
		}

		public static void FatalFormat(this ILog log, Action<FormatProviderMessageHandler> callback) {
			if(log.IsFatalEnabled) {
				callback(log.FatalFormat);
			}
		}
	}
}