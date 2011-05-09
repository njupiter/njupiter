using System;
using System.Collections.Generic;

namespace nJupiter.Configuration {
	public class ConfigSource : IConfigSource {
		protected readonly List<object> configSources;

		public ConfigSource() {
			configSources = new List<object>();
		}

		public ConfigSource(List<object> configSources) {
			if(configSources == null) {
				throw new ArgumentNullException("configSources");
			}
			this.configSources = configSources;
		}

		public ConfigSource(object configSource) : this() {
			if(configSource == null) {
				throw new ArgumentNullException("configSource");
			}
			configSources.Add(configSource);
		}

		public T GetConfigSource<T>() {
			return (T)configSources.Find(s => s.GetType() == typeof(T));
		}
	}
}