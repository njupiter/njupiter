using System.Collections.Generic;

namespace nJupiter.Configuration {
	public class CompositeConfigLoader : List<IConfigLoader>, IConfigLoader  {
	
		public ConfigCollection LoadOnInit() {
			var configs = new ConfigCollection();
			InitializeCollection(configs);
			return configs;
		}

		public void InitializeCollection(ConfigCollection configs) {
			foreach(IConfigLoader loader in this) {
				loader.InitializeCollection(configs);
			}
		}

		public IConfig Load(string configKey) {
			foreach(IConfigLoader loader in this) {
				var config = loader.Load(configKey);
				if(config != null) {
					return config;
				}
			}
			return null;
		}
	}
}
