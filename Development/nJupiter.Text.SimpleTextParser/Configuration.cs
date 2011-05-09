using System;
using System.Collections.Generic;

using nJupiter.Configuration;

namespace nJupiter.Text.SimpleTextParser {
	internal class Configuration {

		private readonly string ruleSet;
		private IFormatter formatter;

		private static readonly Dictionary<string, Configuration> Configurations = new Dictionary<string, Configuration>();
		private static readonly object Padlock = new object();

		public IFormatter Formatter {
			get {
				return this.formatter;
			}
		}

		public static Configuration GetConfig() {
			return GetConfig(string.Empty);
		}

		public static Configuration GetConfig(string name) {
			if(!Configurations.ContainsKey(name)) {
				lock(Padlock) {
					if(!Configurations.ContainsKey(name)) {
						Configurations.Add(name, new Configuration(name));
					}
				}
			}
			return Configurations[name];
		}

		private Configuration(string ldapServer) {
			this.ruleSet = ldapServer;
			this.Configure(null, EventArgs.Empty);
		}

		private void Configure(object sender, EventArgs e) {
			lock(Padlock) {
				IConfig config = ConfigHandler.GetConfig();
				IConfig configSection = this.GetConfigSection(config);
				if(configSection != null) {
					this.formatter = FormatterFactory.GetFormatter(configSection);
				}
				// Auto reconfigure all values when this config object is disposed (droped from the cache)
				config.Disposed += this.Configure;
			}

		}

		private IConfig GetConfigSection(IConfig config) {
			IConfig configSection;
			if(string.IsNullOrEmpty(this.ruleSet)) {
				configSection = config.GetConfigSection("rules[@default='true']");
				if(configSection == null) {
					throw new ConfigurationException("No default rule set specified");
				}
			} else {
				configSection = config.GetConfigSection(string.Format("rules[@value='{0}']", this.ruleSet));
				if(configSection == null) {
					throw new ConfigurationException(string.Format("No rule set with name {0} specified", this.ruleSet));
				}
			}
			return configSection;
		}




	}
}
