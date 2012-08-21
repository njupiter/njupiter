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

using System.Collections.Generic;
using System.Diagnostics;

using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.EnterpriseLibrary.Logging.Filters;

namespace nJupiter.Abstraction.Logging.EnterpriseLibrary {
	public class LoggerWrapper {
		
		public virtual LogWriter Writer { get { return Logger.Writer; } }

		public virtual void SetContextItem(object key, object value) {
			Logger.SetContextItem(key, value);
		}

		public virtual void FlushContextItems() {
			Logger.FlushContextItems();
		}

		public void Write(object message) {
			Logger.Write(message);
		}

		public virtual void Write(object message, string category) {
			Logger.Write(message, category);
		}

		public virtual void Write(object message, string category, int priority) {
			Logger.Write(message, category, priority);
		}

		public virtual void Write(object message, string category, int priority, int eventId) {
			Logger.Write(message, category, priority, eventId);
		}

		public virtual void Write(object message, string category, int priority, int eventId, TraceEventType severity) {
			Logger.Write(message, category, priority, eventId, severity);
		}

		public virtual void Write(object message, string category, int priority, int eventId, TraceEventType severity, string title) {
			Logger.Write(message, category, priority, eventId, severity, title);
		}

		public virtual void Write(object message, IDictionary<string, object> properties) {
			Logger.Write(message, properties);
		}

		public virtual void Write(object message, string category, IDictionary<string, object> properties) {
			Logger.Write(message, category, properties);
		}

		public virtual void Write(object message, string category, int priority, IDictionary<string, object> properties) {
			Logger.Write(message, category, priority, properties);
		}

		public virtual void Write(object message,
		                  string category,
		                  int priority,
		                  int eventId,
		                  TraceEventType severity,
		                  string title,
		                  IDictionary<string, object> properties) {
			Logger.Write(message, category, priority, eventId, severity, title, properties);
		}

		public virtual void Write(object message, ICollection<string> categories) {
			Logger.Write(message, categories);
		}

		public virtual void Write(object message, ICollection<string> categories, int priority) {
			Logger.Write(message, categories, priority);
		}

		public virtual void Write(object message, ICollection<string> categories, int priority, int eventId) {
			Logger.Write(message, categories, priority, eventId);
		}

		public virtual void Write(object message, ICollection<string> categories, int priority, int eventId, TraceEventType severity) {
			Logger.Write(message, categories, priority, eventId, severity);
		}

		public virtual void Write(object message,
		                  ICollection<string> categories,
		                  int priority,
		                  int eventId,
		                  TraceEventType severity,
		                  string title) {
			Logger.Write(message, categories, priority, eventId, severity, title);
		}

		public virtual void Write(object message, ICollection<string> categories, IDictionary<string, object> properties) {
			Logger.Write(message, categories, properties);
		}

		public virtual void Write(object message, ICollection<string> categories, int priority, IDictionary<string, object> properties) {
			Logger.Write(message, categories, priority, properties);
		}

		public virtual void Write(object message,
		                  ICollection<string> categories,
		                  int priority,
		                  int eventId,
		                  TraceEventType severity,
		                  string title,
		                  IDictionary<string, object> properties) {
			Logger.Write(message, categories, priority, eventId, severity, title, properties);
		}

		public virtual void Write(LogEntry log) {
			Logger.Write(log);
		}

		public virtual T GetFilter<T>() where T : class, ILogFilter {
			return Logger.GetFilter<T>();
		}

		public virtual T GetFilter<T>(string name) where T : class, ILogFilter {
			return Logger.GetFilter<T>(name);
		}

		public virtual ILogFilter GetFilter(string name) {
			return Logger.GetFilter(name);
		}

		public virtual bool IsLoggingEnabled() {
			return Logger.IsLoggingEnabled();
		}

		public virtual bool ShouldLog(LogEntry log) {
			return Logger.ShouldLog(log);
		}

		public virtual void Reset() {
			Logger.Reset();
		}
	}
}