using System;
using System.Diagnostics;

using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace nJupiter.Abstraction.Logging.EnterpriseLibrary {
	public interface ILogEntryFactory {
		LogEntry Create(TraceEventType severity, IFormatProvider provider, string format, params object[] args);
		LogEntry Create(TraceEventType severity, string format, params object[] args);
		LogEntry Create(TraceEventType severity, params object[] args);
	}
}