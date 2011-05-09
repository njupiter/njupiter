using System;
using System.IO;

namespace nJupiter.Configuration {
	internal class ConfigSourceFactory {

		private static readonly ConfigSourceFactory Instance = new ConfigSourceFactory();

		public static ConfigSourceFactory GetInstance() {
			return Instance;
		}

		public IConfigSource CreateConfigSource(object source) {
			FileInfo fileInfo = source as FileInfo;
			if(fileInfo != null) {
				return new FileConfigSource(fileInfo);
			}
			Uri uri = source as Uri;
			if(uri != null) {
				return new UriConfigSource(uri);
			}
			return new ConfigSource(source);
		}
	}
}
