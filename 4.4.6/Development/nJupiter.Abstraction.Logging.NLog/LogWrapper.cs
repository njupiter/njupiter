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
	public class LogWrapper  {

		protected readonly Logger log;
		
		public LogWrapper(Logger log) {
			this.log = log;
		}

		public void Debug(string message, sbyte argument) {
			log.Debug(message, argument);
		}

		public void Debug(IFormatProvider formatProvider, string message, sbyte argument) {
			log.Debug(formatProvider, message, argument);
		}

		public void Debug(string message, uint argument) {
			log.Debug(message, argument);
		}

		public void Debug(IFormatProvider formatProvider, string message, uint argument) {
			log.Debug(formatProvider, message, argument);
		}

		public void Debug(string message, decimal argument) {
			log.Debug(message, argument);
		}

		public void Debug(object value) {
			log.Debug(value);
		}

		public void Debug(string message, object argument) {
			log.Debug(message, argument);
		}

		public void Debug(IFormatProvider formatProvider, string message, object argument) {
			log.Debug(formatProvider, message, argument);
		}

		public void Debug(IFormatProvider formatProvider, string message, ulong argument) {
			log.Debug(formatProvider, message, argument);
		}

		public void Debug<TArgument1, TArgument2>(IFormatProvider formatProvider, string message, TArgument1 argument1, TArgument2 argument2) {
			log.Debug(formatProvider, message, argument1, argument2);
		}

		public void Debug<TArgument1, TArgument2>(string message, TArgument1 argument1, TArgument2 argument2) {
			log.Debug(message, argument1, argument2);
		}

		public void Debug<TArgument>(IFormatProvider formatProvider, string message, TArgument argument) {
			log.Debug(formatProvider, message, argument);
		}

		public void Debug<TArgument>(string message, TArgument argument) {
			log.Debug(message, argument);
		}

		public void Debug(string message, double argument) {
			log.Debug(message, argument);
		}

		public void Debug(string message, ulong argument) {
			log.Debug(message, argument);
		}

		public void Debug<TArgument1, TArgument2, TArgument3>(IFormatProvider formatProvider, string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3) {
			log.Debug(formatProvider, message, argument1, argument2, argument3);
		}

		public void Debug<TArgument1, TArgument2, TArgument3>(string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3) {
			log.Debug(message, argument1, argument2, argument3);
		}

		public void Debug(IFormatProvider formatProvider, object value) {
			log.Debug(formatProvider, value);
		}

		public void Debug(string message, int argument) {
			log.Debug(message, argument);
		}

		public void Debug(IFormatProvider formatProvider, string message, long argument) {
			log.Debug(formatProvider, message, argument);
		}

		public void Debug(string message, string argument) {
			log.Debug(message, argument);
		}

		public void Debug(IFormatProvider formatProvider, string message, int argument) {
			log.Debug(formatProvider, message, argument);
		}

		public void Debug(string message, float argument) {
			log.Debug(message, argument);
		}

		public void Debug(IFormatProvider formatProvider, string message, double argument) {
			log.Debug(formatProvider, message, argument);
		}

		public void Debug(string message, long argument) {
			log.Debug(message, argument);
		}

		public void Debug(IFormatProvider formatProvider, string message, float argument) {
			log.Debug(formatProvider, message, argument);
		}

		public void Debug(IFormatProvider formatProvider, string message, string argument) {
			log.Debug(formatProvider, message, argument);
		}

		public void Debug(IFormatProvider formatProvider, string message, bool argument) {
			log.Debug(formatProvider, message, argument);
		}

		public void Debug(string message, bool argument) {
			log.Debug(message, argument);
		}

		public void Debug(IFormatProvider formatProvider, string message, decimal argument) {
			log.Debug(formatProvider, message, argument);
		}

		public void Debug(string message, object arg1, object arg2, object arg3) {
			log.Debug(message, arg1, arg2, arg3);
		}

		public void Debug(IFormatProvider formatProvider, string message, byte argument) {
			log.Debug(formatProvider, message, argument);
		}

		public void Debug(string message, byte argument) {
			log.Debug(message, argument);
		}

		public void Debug(IFormatProvider formatProvider, string message, char argument) {
			log.Debug(formatProvider, message, argument);
		}

		public void Debug(string message, char argument) {
			log.Debug(message, argument);
		}

		public void Debug(string message, params object[] args) {
			log.Debug(message, args);
		}

		public void Debug<T>(IFormatProvider formatProvider, T value) {
			log.Debug(formatProvider, value);
		}

		public void Debug<T>(T value) {
			log.Debug(value);
		}

		public void Debug(string message, object arg1, object arg2) {
			log.Debug(message, arg1, arg2);
		}

		public void Debug(LogMessageGenerator messageFunc) {
			log.Debug(messageFunc);
		}

		public void Debug(IFormatProvider formatProvider, string message, params object[] args) {
			log.Debug(formatProvider, message, args);
		}

		public void Debug(string message) {
			log.Debug(message);
		}

		public void DebugException(string message, Exception exception) {
			log.DebugException(message, exception);
		}

		public void Error(string message, double argument) {
			log.Error(message, argument);
		}

		public void Error(IFormatProvider formatProvider, string message, decimal argument) {
			log.Error(formatProvider, message, argument);
		}

		public void Error<TArgument1, TArgument2, TArgument3>(string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3) {
			log.Error(message, argument1, argument2, argument3);
		}

		public void Error(IFormatProvider formatProvider, string message, double argument) {
			log.Error(formatProvider, message, argument);
		}

		public void Error(string message, long argument) {
			log.Error(message, argument);
		}

		public void Error(IFormatProvider formatProvider, string message, float argument) {
			log.Error(formatProvider, message, argument);
		}

		public void Error(string message, float argument) {
			log.Error(message, argument);
		}

		public void Error<TArgument>(IFormatProvider formatProvider, string message, TArgument argument) {
			log.Error(formatProvider, message, argument);
		}

		public void Error(string message, params object[] args) {
			log.Error(message, args);
		}

		public void Error(string message) {
			log.Error(message);
		}

		public void Error<TArgument>(string message, TArgument argument) {
			log.Error(message, argument);
		}

		public void Error<TArgument1, TArgument2, TArgument3>(IFormatProvider formatProvider, string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3) {
			log.Error(formatProvider, message, argument1, argument2, argument3);
		}

		public void Error<TArgument1, TArgument2>(string message, TArgument1 argument1, TArgument2 argument2) {
			log.Error(message, argument1, argument2);
		}

		public void Error<TArgument1, TArgument2>(IFormatProvider formatProvider, string message, TArgument1 argument1, TArgument2 argument2) {
			log.Error(formatProvider, message, argument1, argument2);
		}

		public void Error(IFormatProvider formatProvider, string message, long argument) {
			log.Error(formatProvider, message, argument);
		}

		public void Error(IFormatProvider formatProvider, string message, bool argument) {
			log.Error(formatProvider, message, argument);
		}

		public void Error(string message, bool argument) {
			log.Error(message, argument);
		}

		public void Error(IFormatProvider formatProvider, string message, char argument) {
			log.Error(formatProvider, message, argument);
		}

		public void Error(string message, object arg1, object arg2, object arg3) {
			log.Error(message, arg1, arg2, arg3);
		}

		public void Error(object value) {
			log.Error(value);
		}

		public void Error(IFormatProvider formatProvider, object value) {
			log.Error(formatProvider, value);
		}

		public void Error(string message, object arg1, object arg2) {
			log.Error(message, arg1, arg2);
		}

		public void Error(string message, string argument) {
			log.Error(message, argument);
		}

		public void Error(IFormatProvider formatProvider, string message, int argument) {
			log.Error(formatProvider, message, argument);
		}

		public void Error(string message, int argument) {
			log.Error(message, argument);
		}

		public void Error(IFormatProvider formatProvider, string message, string argument) {
			log.Error(formatProvider, message, argument);
		}

		public void Error(string message, char argument) {
			log.Error(message, argument);
		}

		public void Error(IFormatProvider formatProvider, string message, byte argument) {
			log.Error(formatProvider, message, argument);
		}

		public void Error(string message, byte argument) {
			log.Error(message, argument);
		}

		public void Error(IFormatProvider formatProvider, string message, params object[] args) {
			log.Error(formatProvider, message, args);
		}

		public void Error(string message, sbyte argument) {
			log.Error(message, argument);
		}

		public void Error(IFormatProvider formatProvider, string message, sbyte argument) {
			log.Error(formatProvider, message, argument);
		}

		public void Error(string message, uint argument) {
			log.Error(message, argument);
		}

		public void Error(IFormatProvider formatProvider, string message, ulong argument) {
			log.Error(formatProvider, message, argument);
		}

		public void Error(string message, ulong argument) {
			log.Error(message, argument);
		}

		public void Error(IFormatProvider formatProvider, string message, uint argument) {
			log.Error(formatProvider, message, argument);
		}

		public void Error<T>(IFormatProvider formatProvider, T value) {
			log.Error(formatProvider, value);
		}

		public void Error(LogMessageGenerator messageFunc) {
			log.Error(messageFunc);
		}

		public void Error(string message, object argument) {
			log.Error(message, argument);
		}

		public void Error(IFormatProvider formatProvider, string message, object argument) {
			log.Error(formatProvider, message, argument);
		}

		public void Error(string message, decimal argument) {
			log.Error(message, argument);
		}

		public void Error<T>(T value) {
			log.Error(value);
		}

		public void ErrorException(string message, Exception exception) {
			log.ErrorException(message, exception);
		}

		public LogFactory Factory {
			get { return log.Factory; } }

		public void Fatal(IFormatProvider formatProvider, string message, long argument) {
			log.Fatal(formatProvider, message, argument);
		}

		public void Fatal(string message, int argument) {
			log.Fatal(message, argument);
		}

		public void Fatal(IFormatProvider formatProvider, string message, float argument) {
			log.Fatal(formatProvider, message, argument);
		}

		public void Fatal(string message, long argument) {
			log.Fatal(message, argument);
		}

		public void Fatal(IFormatProvider formatProvider, string message, char argument) {
			log.Fatal(formatProvider, message, argument);
		}

		public void Fatal(string message, char argument) {
			log.Fatal(message, argument);
		}

		public void Fatal(string message) {
			log.Fatal(message);
		}

		public void Fatal(string message, bool argument) {
			log.Fatal(message, argument);
		}

		public void Fatal(IFormatProvider formatProvider, string message, byte argument) {
			log.Fatal(formatProvider, message, argument);
		}

		public void Fatal(string message, string argument) {
			log.Fatal(message, argument);
		}

		public void Fatal(IFormatProvider formatProvider, string message, int argument) {
			log.Fatal(formatProvider, message, argument);
		}

		public void Fatal(string message, byte argument) {
			log.Fatal(message, argument);
		}

		public void Fatal(IFormatProvider formatProvider, string message, string argument) {
			log.Fatal(formatProvider, message, argument);
		}

		public void Fatal(IFormatProvider formatProvider, string message, uint argument) {
			log.Fatal(formatProvider, message, argument);
		}

		public void Fatal(string message, sbyte argument) {
			log.Fatal(message, argument);
		}

		public void Fatal(IFormatProvider formatProvider, string message, sbyte argument) {
			log.Fatal(formatProvider, message, argument);
		}

		public void Fatal(string message, ulong argument) {
			log.Fatal(message, argument);
		}

		public void Fatal(IFormatProvider formatProvider, string message, ulong argument) {
			log.Fatal(formatProvider, message, argument);
		}

		public void Fatal(string message, uint argument) {
			log.Fatal(message, argument);
		}

		public void Fatal(string message, object argument) {
			log.Fatal(message, argument);
		}

		public void Fatal(string message, double argument) {
			log.Fatal(message, argument);
		}

		public void Fatal(IFormatProvider formatProvider, string message, double argument) {
			log.Fatal(formatProvider, message, argument);
		}

		public void Fatal(string message, float argument) {
			log.Fatal(message, argument);
		}

		public void Fatal(IFormatProvider formatProvider, string message, object argument) {
			log.Fatal(formatProvider, message, argument);
		}

		public void Fatal(string message, decimal argument) {
			log.Fatal(message, argument);
		}

		public void Fatal(IFormatProvider formatProvider, string message, decimal argument) {
			log.Fatal(formatProvider, message, argument);
		}

		public void Fatal<TArgument1, TArgument2>(IFormatProvider formatProvider, string message, TArgument1 argument1, TArgument2 argument2) {
			log.Fatal(formatProvider, message, argument1, argument2);
		}

		public void Fatal<TArgument1, TArgument2>(string message, TArgument1 argument1, TArgument2 argument2) {
			log.Fatal(message, argument1, argument2);
		}

		public void Fatal<TArgument>(IFormatProvider formatProvider, string message, TArgument argument) {
			log.Fatal(formatProvider, message, argument);
		}

		public void Fatal<TArgument>(string message, TArgument argument) {
			log.Fatal(message, argument);
		}

		public void Fatal<TArgument1, TArgument2, TArgument3>(IFormatProvider formatProvider, string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3) {
			log.Fatal(formatProvider, message, argument1, argument2, argument3);
		}

		public void Fatal(string message, object arg1, object arg2) {
			log.Fatal(message, arg1, arg2);
		}

		public void Fatal(string message, object arg1, object arg2, object arg3) {
			log.Fatal(message, arg1, arg2, arg3);
		}

		public void Fatal(object value) {
			log.Fatal(value);
		}

		public void Fatal(IFormatProvider formatProvider, object value) {
			log.Fatal(formatProvider, value);
		}

		public void Fatal<T>(IFormatProvider formatProvider, T value) {
			log.Fatal(formatProvider, value);
		}

		public void Fatal(LogMessageGenerator messageFunc) {
			log.Fatal(messageFunc);
		}

		public void Fatal<TArgument1, TArgument2, TArgument3>(string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3) {
			log.Fatal(message, argument1, argument2, argument3);
		}

		public void Fatal<T>(T value) {
			log.Fatal(value);
		}

		public void Fatal(IFormatProvider formatProvider, string message, params object[] args) {
			log.Fatal(formatProvider, message, args);
		}

		public void Fatal(string message, params object[] args) {
			log.Fatal(message, args);
		}

		public void Fatal(IFormatProvider formatProvider, string message, bool argument) {
			log.Fatal(formatProvider, message, argument);
		}

		public void FatalException(string message, Exception exception) {
			log.FatalException(message, exception);
		}

		public void Info(string message) {
			log.Info(message);
		}

		public void Info<TArgument>(IFormatProvider formatProvider, string message, TArgument argument) {
			log.Info(formatProvider, message, argument);
		}

		public void Info(string message, params object[] args) {
			log.Info(message, args);
		}

		public void Info(object value) {
			log.Info(value);
		}

		public void Info(IFormatProvider formatProvider, object value) {
			log.Info(formatProvider, value);
		}

		public void Info(IFormatProvider formatProvider, string message, params object[] args) {
			log.Info(formatProvider, message, args);
		}

		public void Info<TArgument>(string message, TArgument argument) {
			log.Info(message, argument);
		}

		public void Info<TArgument1, TArgument2, TArgument3>(string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3) {
			log.Info(message, argument1, argument2, argument3);
		}

		public void Info(string message, decimal argument) {
			log.Info(message, argument);
		}

		public void Info<T>(T value) {
			log.Info(value);
		}

		public void Info<TArgument1, TArgument2>(IFormatProvider formatProvider, string message, TArgument1 argument1, TArgument2 argument2) {
			log.Info(formatProvider, message, argument1, argument2);
		}

		public void Info<TArgument1, TArgument2>(string message, TArgument1 argument1, TArgument2 argument2) {
			log.Info(message, argument1, argument2);
		}

		public void Info<TArgument1, TArgument2, TArgument3>(IFormatProvider formatProvider, string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3) {
			log.Info(formatProvider, message, argument1, argument2, argument3);
		}

		public void Info(IFormatProvider formatProvider, string message, object argument) {
			log.Info(formatProvider, message, argument);
		}

		public void Info(string message, object argument) {
			log.Info(message, argument);
		}

		public void Info(IFormatProvider formatProvider, string message, sbyte argument) {
			log.Info(formatProvider, message, argument);
		}

		public void Info(IFormatProvider formatProvider, string message, decimal argument) {
			log.Info(formatProvider, message, argument);
		}

		public void Info(string message, float argument) {
			log.Info(message, argument);
		}

		public void Info(IFormatProvider formatProvider, string message, double argument) {
			log.Info(formatProvider, message, argument);
		}

		public void Info(string message, double argument) {
			log.Info(message, argument);
		}

		public void Info(string message, ulong argument) {
			log.Info(message, argument);
		}

		public void Info<T>(IFormatProvider formatProvider, T value) {
			log.Info(formatProvider, value);
		}

		public void Info(LogMessageGenerator messageFunc) {
			log.Info(messageFunc);
		}

		public void Info(IFormatProvider formatProvider, string message, ulong argument) {
			log.Info(formatProvider, message, argument);
		}

		public void Info(string message, sbyte argument) {
			log.Info(message, argument);
		}

		public void Info(IFormatProvider formatProvider, string message, uint argument) {
			log.Info(formatProvider, message, argument);
		}

		public void Info(string message, uint argument) {
			log.Info(message, argument);
		}

		public void Info(IFormatProvider formatProvider, string message, float argument) {
			log.Info(formatProvider, message, argument);
		}

		public void Info(IFormatProvider formatProvider, string message, char argument) {
			log.Info(formatProvider, message, argument);
		}

		public void Info(string message, char argument) {
			log.Info(message, argument);
		}

		public void Info(IFormatProvider formatProvider, string message, byte argument) {
			log.Info(formatProvider, message, argument);
		}

		public void Info(string message, bool argument) {
			log.Info(message, argument);
		}

		public void Info(string message, object arg1, object arg2) {
			log.Info(message, arg1, arg2);
		}

		public void Info(string message, object arg1, object arg2, object arg3) {
			log.Info(message, arg1, arg2, arg3);
		}

		public void Info(IFormatProvider formatProvider, string message, bool argument) {
			log.Info(formatProvider, message, argument);
		}

		public void Info(string message, byte argument) {
			log.Info(message, argument);
		}

		public void Info(string message, int argument) {
			log.Info(message, argument);
		}

		public void Info(IFormatProvider formatProvider, string message, long argument) {
			log.Info(formatProvider, message, argument);
		}

		public void Info(string message, long argument) {
			log.Info(message, argument);
		}

		public void Info(IFormatProvider formatProvider, string message, int argument) {
			log.Info(formatProvider, message, argument);
		}

		public void Info(string message, string argument) {
			log.Info(message, argument);
		}

		public void Info(IFormatProvider formatProvider, string message, string argument) {
			log.Info(formatProvider, message, argument);
		}

		public void InfoException(string message, Exception exception) {
			log.InfoException(message, exception);
		}

		public bool IsDebugEnabled {
			get { return log.IsDebugEnabled; } }

		public bool IsEnabled(LogLevel level) {
			return log.IsEnabled(level);
		}

		public bool IsErrorEnabled {
			get { return log.IsErrorEnabled; } }

		public bool IsFatalEnabled {
			get { return log.IsFatalEnabled; } }

		public bool IsInfoEnabled {
			get { return log.IsInfoEnabled; } }

		public bool IsTraceEnabled {
			get { return log.IsTraceEnabled; } }

		public bool IsWarnEnabled {
			get { return log.IsWarnEnabled; } }

		public void Log(LogLevel level, IFormatProvider formatProvider, string message, sbyte argument) {
			log.Log(level, formatProvider, message, argument);
		}

		public void Log(LogLevel level, string message, sbyte argument) {
			log.Log(level, message, argument);
		}

		public void Log(LogLevel level, string message, object argument) {
			log.Log(level, message, argument);
		}

		public void Log(LogLevel level, string message, decimal argument) {
			log.Log(level, message, argument);
		}

		public void Log(LogLevel level, IFormatProvider formatProvider, string message, object argument) {
			log.Log(level, formatProvider, message, argument);
		}

		public void Log(LogLevel level, IFormatProvider formatProvider, string message, uint argument) {
			log.Log(level, formatProvider, message, argument);
		}

		public void Log(LogLevel level, string message, byte argument) {
			log.Log(level, message, argument);
		}

		public void Log(LogLevel level, IFormatProvider formatProvider, string message, byte argument) {
			log.Log(level, formatProvider, message, argument);
		}

		public void Log(LogLevel level, string message, ulong argument) {
			log.Log(level, message, argument);
		}

		public void Log(LogLevel level, string message, uint argument) {
			log.Log(level, message, argument);
		}

		public void Log(LogLevel level, IFormatProvider formatProvider, string message, ulong argument) {
			log.Log(level, formatProvider, message, argument);
		}

		public void Log(LogLevel level, string message, float argument) {
			log.Log(level, message, argument);
		}

		public void Log(LogLevel level, IFormatProvider formatProvider, string message, float argument) {
			log.Log(level, formatProvider, message, argument);
		}

		public void Log(LogLevel level, IFormatProvider formatProvider, string message, double argument) {
			log.Log(level, formatProvider, message, argument);
		}

		public void Log(LogLevel level, IFormatProvider formatProvider, string message, decimal argument) {
			log.Log(level, formatProvider, message, argument);
		}

		public void Log(LogLevel level, string message, double argument) {
			log.Log(level, message, argument);
		}

		public void Log(LogLevel level, string message, long argument) {
			log.Log(level, message, argument);
		}

		public void Log(LogLevel level, string message, string argument) {
			log.Log(level, message, argument);
		}

		public void Log(LogLevel level, IFormatProvider formatProvider, string message, string argument) {
			log.Log(level, formatProvider, message, argument);
		}

		public void Log(LogLevel level, IFormatProvider formatProvider, string message, int argument) {
			log.Log(level, formatProvider, message, argument);
		}

		public void Log(LogLevel level, IFormatProvider formatProvider, string message, long argument) {
			log.Log(level, formatProvider, message, argument);
		}

		public void Log(LogLevel level, string message, int argument) {
			log.Log(level, message, argument);
		}

		public void Log(LogLevel level, string message, char argument) {
			log.Log(level, message, argument);
		}

		public void Log(LogLevel level, string message) {
			log.Log(level, message);
		}

		public void Log(LogLevel level, IFormatProvider formatProvider, string message, params object[] args) {
			log.Log(level, formatProvider, message, args);
		}

		public void Log(LogLevel level, string message, params object[] args) {
			log.Log(level, message, args);
		}

		public void Log<TArgument>(LogLevel level, string message, TArgument argument) {
			log.Log(level, message, argument);
		}

		public void Log<TArgument>(LogLevel level, IFormatProvider formatProvider, string message, TArgument argument) {
			log.Log(level, formatProvider, message, argument);
		}

		public void Log(Type wrapperType, LogEventInfo logEvent) {
			log.Log(wrapperType, logEvent);
		}

		public void Log(LogEventInfo logEvent) {
			log.Log(logEvent);
		}

		public void Log<T>(LogLevel level, T value) {
			log.Log(level, value);
		}

		public void Log(LogLevel level, LogMessageGenerator messageFunc) {
			log.Log(level, messageFunc);
		}

		public void Log<T>(LogLevel level, IFormatProvider formatProvider, T value) {
			log.Log(level, formatProvider, value);
		}

		public void Log<TArgument1, TArgument2>(LogLevel level, IFormatProvider formatProvider, string message, TArgument1 argument1, TArgument2 argument2) {
			log.Log(level, formatProvider, message, argument1, argument2);
		}

		public void Log(LogLevel level, string message, object arg1, object arg2) {
			log.Log(level, message, arg1, arg2);
		}

		public void Log(LogLevel level, IFormatProvider formatProvider, string message, char argument) {
			log.Log(level, formatProvider, message, argument);
		}

		public void Log(LogLevel level, string message, object arg1, object arg2, object arg3) {
			log.Log(level, message, arg1, arg2, arg3);
		}

		public void Log(LogLevel level, string message, bool argument) {
			log.Log(level, message, argument);
		}

		public void Log(LogLevel level, IFormatProvider formatProvider, string message, bool argument) {
			log.Log(level, formatProvider, message, argument);
		}

		public void Log<TArgument1, TArgument2, TArgument3>(LogLevel level, IFormatProvider formatProvider, string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3) {
			log.Log(level, formatProvider, message, argument1, argument2, argument3);
		}

		public void Log<TArgument1, TArgument2>(LogLevel level, string message, TArgument1 argument1, TArgument2 argument2) {
			log.Log(level, message, argument1, argument2);
		}

		public void Log<TArgument1, TArgument2, TArgument3>(LogLevel level, string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3) {
			log.Log(level, message, argument1, argument2, argument3);
		}

		public void Log(LogLevel level, IFormatProvider formatProvider, object value) {
			log.Log(level, formatProvider, value);
		}

		public void Log(LogLevel level, object value) {
			log.Log(level, value);
		}

		public void LogException(LogLevel level, string message, Exception exception) {
			log.LogException(level, message, exception);
		}

		public event EventHandler<EventArgs> LoggerReconfigured { add { log.LoggerReconfigured += value; }
			remove { log.LoggerReconfigured -= value; } }

		public string Name {
			get { return log.Name; } }

		public void Trace(string message, object arg1, object arg2) {
			log.Trace(message, arg1, arg2);
		}

		public void Trace(IFormatProvider formatProvider, object value) {
			log.Trace(formatProvider, value);
		}

		public void Trace(string message, object arg1, object arg2, object arg3) {
			log.Trace(message, arg1, arg2, arg3);
		}

		public void Trace(IFormatProvider formatProvider, string message, bool argument) {
			log.Trace(formatProvider, message, argument);
		}

		public void Trace(object value) {
			log.Trace(value);
		}

		public void Trace(LogMessageGenerator messageFunc) {
			log.Trace(messageFunc);
		}

		public void Trace<T>(T value) {
			log.Trace(value);
		}

		public void Trace(IFormatProvider formatProvider, string message, params object[] args) {
			log.Trace(formatProvider, message, args);
		}

		public void Trace(string message, params object[] args) {
			log.Trace(message, args);
		}

		public void Trace(string message) {
			log.Trace(message);
		}

		public void Trace(string message, bool argument) {
			log.Trace(message, argument);
		}

		public void Trace<TArgument1, TArgument2>(IFormatProvider formatProvider, string message, TArgument1 argument1, TArgument2 argument2) {
			log.Trace(formatProvider, message, argument1, argument2);
		}

		public void Trace<TArgument>(string message, TArgument argument) {
			log.Trace(message, argument);
		}

		public void Trace<TArgument1, TArgument2>(string message, TArgument1 argument1, TArgument2 argument2) {
			log.Trace(message, argument1, argument2);
		}

		public void Trace<TArgument1, TArgument2, TArgument3>(string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3) {
			log.Trace(message, argument1, argument2, argument3);
		}

		public void Trace<TArgument1, TArgument2, TArgument3>(IFormatProvider formatProvider, string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3) {
			log.Trace(formatProvider, message, argument1, argument2, argument3);
		}

		public void Trace(string message, char argument) {
			log.Trace(message, argument);
		}

		public void Trace(IFormatProvider formatProvider, string message, char argument) {
			log.Trace(formatProvider, message, argument);
		}

		public void Trace(IFormatProvider formatProvider, string message, byte argument) {
			log.Trace(formatProvider, message, argument);
		}

		public void Trace<TArgument>(IFormatProvider formatProvider, string message, TArgument argument) {
			log.Trace(formatProvider, message, argument);
		}

		public void Trace(string message, byte argument) {
			log.Trace(message, argument);
		}

		public void Trace(string message, object argument) {
			log.Trace(message, argument);
		}

		public void Trace(IFormatProvider formatProvider, string message, sbyte argument) {
			log.Trace(formatProvider, message, argument);
		}

		public void Trace(IFormatProvider formatProvider, string message, object argument) {
			log.Trace(formatProvider, message, argument);
		}

		public void Trace(IFormatProvider formatProvider, string message, decimal argument) {
			log.Trace(formatProvider, message, argument);
		}

		public void Trace(string message, decimal argument) {
			log.Trace(message, argument);
		}

		public void Trace(string message, sbyte argument) {
			log.Trace(message, argument);
		}

		public void Trace(string message, ulong argument) {
			log.Trace(message, argument);
		}

		public void Trace<T>(IFormatProvider formatProvider, T value) {
			log.Trace(formatProvider, value);
		}

		public void Trace(IFormatProvider formatProvider, string message, ulong argument) {
			log.Trace(formatProvider, message, argument);
		}

		public void Trace(IFormatProvider formatProvider, string message, uint argument) {
			log.Trace(formatProvider, message, argument);
		}

		public void Trace(string message, uint argument) {
			log.Trace(message, argument);
		}

		public void Trace(string message, double argument) {
			log.Trace(message, argument);
		}

		public void Trace(IFormatProvider formatProvider, string message, int argument) {
			log.Trace(formatProvider, message, argument);
		}

		public void Trace(string message, int argument) {
			log.Trace(message, argument);
		}

		public void Trace(IFormatProvider formatProvider, string message, string argument) {
			log.Trace(formatProvider, message, argument);
		}

		public void Trace(string message, string argument) {
			log.Trace(message, argument);
		}

		public void Trace(IFormatProvider formatProvider, string message, long argument) {
			log.Trace(formatProvider, message, argument);
		}

		public void Trace(string message, float argument) {
			log.Trace(message, argument);
		}

		public void Trace(IFormatProvider formatProvider, string message, double argument) {
			log.Trace(formatProvider, message, argument);
		}

		public void Trace(string message, long argument) {
			log.Trace(message, argument);
		}

		public void Trace(IFormatProvider formatProvider, string message, float argument) {
			log.Trace(formatProvider, message, argument);
		}

		public void TraceException(string message, Exception exception) {
			log.TraceException(message, exception);
		}

		public void Warn<T>(IFormatProvider formatProvider, T value) {
			log.Warn(formatProvider, value);
		}

		public void Warn(LogMessageGenerator messageFunc) {
			log.Warn(messageFunc);
		}

		public void Warn(string message, ulong argument) {
			log.Warn(message, argument);
		}

		public void Warn<T>(T value) {
			log.Warn(value);
		}

		public void Warn(string message, sbyte argument) {
			log.Warn(message, argument);
		}

		public void Warn(IFormatProvider formatProvider, string message, uint argument) {
			log.Warn(formatProvider, message, argument);
		}

		public void Warn(string message, object argument) {
			log.Warn(message, argument);
		}

		public void Warn(IFormatProvider formatProvider, string message, sbyte argument) {
			log.Warn(formatProvider, message, argument);
		}

		public void Warn(string message, uint argument) {
			log.Warn(message, argument);
		}

		public void Warn(IFormatProvider formatProvider, string message, string argument) {
			log.Warn(formatProvider, message, argument);
		}

		public void Warn(IFormatProvider formatProvider, string message, int argument) {
			log.Warn(formatProvider, message, argument);
		}

		public void Warn(IFormatProvider formatProvider, string message, ulong argument) {
			log.Warn(formatProvider, message, argument);
		}

		public void Warn(string message, string argument) {
			log.Warn(message, argument);
		}

		public void Warn(IFormatProvider formatProvider, string message, object argument) {
			log.Warn(formatProvider, message, argument);
		}

		public void Warn(string message, long argument) {
			log.Warn(message, argument);
		}

		public void Warn(IFormatProvider formatProvider, string message, float argument) {
			log.Warn(formatProvider, message, argument);
		}

		public void Warn(string message, int argument) {
			log.Warn(message, argument);
		}

		public void Warn(IFormatProvider formatProvider, string message, long argument) {
			log.Warn(formatProvider, message, argument);
		}

		public void Warn(string message, float argument) {
			log.Warn(message, argument);
		}

		public void Warn(IFormatProvider formatProvider, string message, decimal argument) {
			log.Warn(formatProvider, message, argument);
		}

		public void Warn(string message, decimal argument) {
			log.Warn(message, argument);
		}

		public void Warn(IFormatProvider formatProvider, string message, double argument) {
			log.Warn(formatProvider, message, argument);
		}

		public void Warn(string message, double argument) {
			log.Warn(message, argument);
		}

		public void Warn<TArgument1, TArgument2>(IFormatProvider formatProvider, string message, TArgument1 argument1, TArgument2 argument2) {
			log.Warn(formatProvider, message, argument1, argument2);
		}

		public void Warn<TArgument>(string message, TArgument argument) {
			log.Warn(message, argument);
		}

		public void Warn<TArgument1, TArgument2, TArgument3>(IFormatProvider formatProvider, string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3) {
			log.Warn(formatProvider, message, argument1, argument2, argument3);
		}

		public void Warn<TArgument1, TArgument2>(string message, TArgument1 argument1, TArgument2 argument2) {
			log.Warn(message, argument1, argument2);
		}

		public void Warn<TArgument>(IFormatProvider formatProvider, string message, TArgument argument) {
			log.Warn(formatProvider, message, argument);
		}

		public void Warn(IFormatProvider formatProvider, string message, params object[] args) {
			log.Warn(formatProvider, message, args);
		}

		public void Warn(string message, byte argument) {
			log.Warn(message, argument);
		}

		public void Warn(string message, params object[] args) {
			log.Warn(message, args);
		}

		public void Warn(string message) {
			log.Warn(message);
		}

		public void Warn<TArgument1, TArgument2, TArgument3>(string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3) {
			log.Warn(message, argument1, argument2, argument3);
		}

		public void Warn(IFormatProvider formatProvider, string message, char argument) {
			log.Warn(formatProvider, message, argument);
		}

		public void Warn(string message, bool argument) {
			log.Warn(message, argument);
		}

		public void Warn(IFormatProvider formatProvider, string message, byte argument) {
			log.Warn(formatProvider, message, argument);
		}

		public void Warn(string message, char argument) {
			log.Warn(message, argument);
		}

		public void Warn(IFormatProvider formatProvider, string message, bool argument) {
			log.Warn(formatProvider, message, argument);
		}

		public void Warn(IFormatProvider formatProvider, object value) {
			log.Warn(formatProvider, value);
		}

		public void Warn(object value) {
			log.Warn(value);
		}

		public void Warn(string message, object arg1, object arg2, object arg3) {
			log.Warn(message, arg1, arg2, arg3);
		}

		public void Warn(string message, object arg1, object arg2) {
			log.Warn(message, arg1, arg2);
		}

		public void WarnException(string message, Exception exception) {
			log.WarnException(message, exception);
		}


	}
}