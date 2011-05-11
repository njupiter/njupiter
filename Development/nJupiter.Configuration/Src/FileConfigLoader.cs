using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace nJupiter.Configuration {
	class FileConfigLoader : IConfigLoader {
		
		private static readonly char[] IllegalPathCharacters = new[] { '\\', '/', '"', '*', '?', '<', '>' };
		private readonly string configSuffix;
		private readonly IEnumerable<string> configPaths;
		private readonly ConfigSourceFactory configSourceFactory;

		public FileConfigLoader(IConfig config, ConfigSourceFactory configSourceFactory) {
			this.configSourceFactory = configSourceFactory;
			this.configPaths = GetConfigPaths(config);
			this.configSuffix = GetConfigSuffix(config);
		}

		public ConfigCollection LoadAll() {
			return LoadConfigs("*");
		}

		public IConfig Load(string configKey) {
			ConfigCollection configs = LoadConfigs(configKey);
			if(configs.Contains(configKey)) {
				return configs[configKey];
			}
			return null;
		}

		private ConfigCollection LoadConfigs(string pattern) {
			ConfigCollection configs = new ConfigCollection();
			if(pattern.IndexOfAny(IllegalPathCharacters) < 0) {
				IEnumerable<FileInfo> files = GetFiles(pattern);
				foreach(FileInfo file in files) {
					string configKey = file.Name.Substring(0, file.Name.Length - configSuffix.Length);
					IConfig config = CreateConfigFromFile(configKey, file);
					configs.Insert(config);
				}
			}
			return configs;
		}

		private IEnumerable<FileInfo> GetFiles(string pattern) {
			List<FileInfo> files = new List<FileInfo>();
			foreach(string path in this.configPaths) {
				DirectoryInfo dir = GetDirectory(path);
				if(dir.Exists) {
					FileInfo[] fileArray = dir.GetFiles(string.Format("{0}{1}", pattern, this.configSuffix));
					files.AddRange(fileArray);
				}
			}
			return files;
		}

		private IConfig CreateConfigFromFile(string configKey, FileInfo configFile) {
			if(configFile == null) {
				throw new ArgumentNullException("configFile");
			}
			try {
				return CreateConfigFromFileWithoutErrorHandling(configKey, configFile);
			}catch(Exception ex){
				throw new ConfiguratorException(string.Format("Error while loading XML configuration for the config with key [{0}].", configKey), ex);
			}
		}

		private IConfig CreateConfigFromFileWithoutErrorHandling(string configKey, FileInfo configFile) {
			if(configFile.Name.StartsWith(configKey) && File.Exists(configFile.FullName)) {
				using(FileStream fs = OpenFile(configFile)) {
					IConfigSource source = this.configSourceFactory.CreateConfigSource(configFile);
					return ConfigFactory.Create(configKey, fs, source);
				}
			}
			return null;
		}

		private static FileStream OpenFile(FileInfo configFile) {
			for(int retry = 5; --retry >= 0; ) {
				try {
					return configFile.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
				} catch(IOException ex) {
					if(retry == 0) {
						throw new ConfiguratorException(string.Format("Failed to open XML config file [{0}].", configFile.Name), ex);
					}
					System.Threading.Thread.Sleep(250);
				}
			}
			return null;
		}

		private static DirectoryInfo GetDirectory(string path) {
			if(HttpContext.Current != null && path.StartsWith("~")) {
				path = HttpContext.Current.Server.MapPath(path);
			}
			return new DirectoryInfo(path);
		}

		private static string GetConfigSuffix(IConfig config) {
			if(config.ContainsAttribute("configDirectories", "configSuffix")) {
				return config.GetAttribute("configDirectories", "configSuffix");
			}
			return ".config";
		}

		private static IEnumerable<string> GetConfigPaths(IConfig config) {
			string[] paths = config.GetValueArray("configDirectories", "configDirectory");
			if(paths.Length == 0) {
				paths = Directory.GetDirectories(Environment.CurrentDirectory, "*", SearchOption.AllDirectories);
			}
			return paths;
		}
	}
}
