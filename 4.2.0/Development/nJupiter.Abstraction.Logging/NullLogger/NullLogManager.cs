using System;

namespace nJupiter.Abstraction.Logging.NullLogger {
	internal class NullLogManager : ILogManager {

		private readonly ILog log = new NullLog();

		public ILog GetLogger(Type type) {
			return log;
		}

		public ILog GetLogger(string name) {
			return log;
		}
	}
}