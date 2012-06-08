using System;

namespace nJupiter.Abstraction.Logging {
	public abstract class AutoRegistratingLogManager : ILogManager {
		
		protected AutoRegistratingLogManager() {
			if(!LogManagerFactory.FactoryRegistered) {
				LogManagerFactory.RegisterFactory(() => this);
			}
		}

		public abstract ILog GetLogger(Type type);
		public abstract ILog GetLogger(string name);
	}
}
