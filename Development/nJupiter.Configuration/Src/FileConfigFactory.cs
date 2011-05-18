using System;
using System.IO;

namespace nJupiter.Configuration {
	public static class FileConfigFactory {

		public static IConfig CreateWithWatcher(string path) {
			return Create(path, true);
		}

		public static IConfig CreateWithWatcher(string configKey, string path) {
			return Create(configKey, path, true);
		}

		public static IConfig CreateWithWatcher(FileInfo file) {
			return Create(file, true);
		}

		public static IConfig CreateWithWatcher(string configKey, FileInfo file) {
			return Create(configKey, file, true);
		}

		public static IConfig Create(string path) {
			FileInfo file = new FileInfo(path);
			return Create(null, file, ConfigSourceFactory.CreateConfigSource(file, false));
		}

		public static IConfig Create(string configKey, string path) {
			FileInfo file = new FileInfo(path);
			return Create(configKey, file, ConfigSourceFactory.CreateConfigSource(file, false));
		}

		public static IConfig Create(FileInfo file) {
			return Create(null, file, ConfigSourceFactory.CreateConfigSource(file, false));
		}

		public static IConfig Create(string configKey, FileInfo file) {
			return Create(configKey, file, ConfigSourceFactory.CreateConfigSource(file, false));
		}

		public static IConfig Create(string configKey, string path, IConfigSource configSource) {
			FileInfo file = new FileInfo(path);
			return Create(configKey, file, configSource);
		}

		public static IConfig Create(FileInfo file, IConfigSource configSource) {
			return Create(null, file, configSource);
		}

		public static IConfig Create(string configKey, FileInfo file, IConfigSource configSource) {
			if(file == null) {
				throw new ArgumentNullException("file");
			}
			using(Stream stream = file.OpenRead()) {
				return ConfigFactory.Create(GetConfigKey(file, configKey), stream, configSource);
			}
		}

		public static IConfig Create(string path, bool attachWatcher) {
			FileInfo file = new FileInfo(path);
			return Create(null, file, ConfigSourceFactory.CreateConfigSource(file, attachWatcher));
		}

		public static IConfig Create(string configKey, string path, bool attachWatcher) {
			FileInfo file = new FileInfo(path);
			return Create(configKey, file, ConfigSourceFactory.CreateConfigSource(file, attachWatcher));
		}

		public static IConfig Create(FileInfo file, bool attachWatcher) {
			return Create(null, file, ConfigSourceFactory.CreateConfigSource(file, attachWatcher));
		}

		public static IConfig Create(string configKey, FileInfo file, bool attachWatcher) {
			return Create(configKey, file, ConfigSourceFactory.CreateConfigSource(file, attachWatcher));
		}

		private static string GetConfigKey(FileInfo file, string configKey) {
			return configKey ?? file.Name.Substring(0, file.Name.Length - file.Extension.Length);
		}
	}
}
