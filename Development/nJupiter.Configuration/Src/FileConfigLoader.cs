#region Copyright & License
/*
	Copyright (c) 2005-2011 nJupiter

	Permission is hereby granted, free of charge, to any person obtaining a copy
	of this software and associated documentation files (the "Software"), to deal
	in the Software without restriction, including without limitation the rights
	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	copies of the Software, and to permit persons to whom the Software is
	furnished to do so, subject to the following conditions:

	The above copyright notice and this permission notice shall be included in
	all copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
	THE SOFTWARE.
*/
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace nJupiter.Configuration {
	internal class FileConfigLoader : IConfigLoader {
		
		private static readonly char[] IllegalPathCharacters = new[] { '\\', '/', '"', '?', '<', '>' };
		private readonly string configSuffix;
		private readonly bool addFileWatchers;
		private readonly bool loadAllConfigFilesOnInit;
		private readonly IEnumerable<string> configPaths;

		public FileConfigLoader(IConfig config) {
			this.configPaths = GetConfigPaths(config);
			this.configSuffix = GetConfigSuffix(config);
			this.addFileWatchers = ShallAddFileWatchers(config);
			this.loadAllConfigFilesOnInit = ShallLoadAllFilesOnInit(config);
		}

		public ConfigCollection LoadOnInit() {
			var configs = new ConfigCollection();
			InitializeCollection(configs);
			return configs;
		}

		public void InitializeCollection(ConfigCollection configs) {
			if(loadAllConfigFilesOnInit) {
				this.LoadConfigsIntoCollection("*", configs);
			}
		}

		public IConfig Load(string configKey) {
			var configs = new ConfigCollection();
			this.LoadConfigsIntoCollection(configKey, configs);
			if(configs.Contains(configKey)) {
				return configs[configKey];
			}
			return null;
		}

		private void LoadConfigsIntoCollection(string pattern, ConfigCollection configs) {
			try {
				this.LoadConfigsIntoCollectionFromFiles(pattern, configs);
			}catch(Exception ex){
				throw new ConfiguratorException(string.Format("Error while loading XML configuration for the config with pattern '{0}'", pattern), ex);
			}
		}

		private void LoadConfigsIntoCollectionFromFiles(string pattern, ConfigCollection configs) {
			if(pattern.IndexOfAny(IllegalPathCharacters) < 0) {
				var files = GetFiles(pattern);
				foreach(var file in files) {
					var config = FileConfigFactory.Create(file, addFileWatchers);
					configs.Add(config);
				}
			}
		}

		private IEnumerable<FileInfo> GetFiles(string pattern) {
			var files = new List<FileInfo>();
			foreach(string path in this.configPaths) {
				var dir = GetDirectory(path);
				if(dir.Exists) {
					var fileArray = this.GetFiles(pattern, dir);
					files.AddRange(fileArray);
				}
			}
			return files;
		}

		private IEnumerable<FileInfo> GetFiles(string pattern, DirectoryInfo dir) {
			try {
				return dir.GetFiles(string.Format("{0}{1}", pattern, this.configSuffix));
			} catch(IOException) {
				// Ignore IOException in case of incorrect syntax
			}
			return new FileInfo[0];
		}

		// Internal for test purposes
		internal static Stream OpenFile(FileInfo configFile) {
			Exception exception = null;
			for(int retries = 5; retries >= 0; retries--) {
				try {
					return configFile.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
				} catch(IOException ex) {
					exception = ex;
					System.Threading.Thread.Sleep(200);
				}
			}
			throw new ConfiguratorException(string.Format("Failed to open XML config file '{0}'.", configFile.Name), exception);
		}

		private static DirectoryInfo GetDirectory(string path) {
			if(HttpContext.Current != null && path.StartsWith("~")) {
				path = HttpContext.Current.Server.MapPath(path);
			}
			return new DirectoryInfo(path);
		}

		private static bool ShallAddFileWatchers(IConfig config) {
			if(config != null && config.ContainsAttribute("configDirectories", "enableFileWatching")) {
				return config.GetAttribute<bool>("configDirectories", "enableFileWatching");
			}
			return true;
		}

		private static bool ShallLoadAllFilesOnInit(IConfig config) {
			if(config != null && config.ContainsAttribute("configDirectories", "loadAllConfigFilesOnInit")) {
				return config.GetAttribute<bool>("configDirectories", "loadAllConfigFilesOnInit");
			}
			return false;
		}


		private static string GetConfigSuffix(IConfig config) {
			if(config != null && config.ContainsAttribute("configDirectories", "configSuffix")) {
				return config.GetAttribute("configDirectories", "configSuffix");
			}
			return ".config";
		}

		private static IEnumerable<string> GetConfigPaths(IConfig config) {
			string[] paths = config != null ? config.GetValueArray("configDirectories", "configDirectory") : null;
			if(paths == null || paths.Length == 0) {
				paths = Directory.GetDirectories(GetCurrentDirectory(), "*", SearchOption.AllDirectories);
			}
			return paths;
		}

		private static string GetCurrentDirectory() {
			return AppDomain.CurrentDomain.BaseDirectory;
		}
	}
}
