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
	public class FileConfigLoader : IConfigLoader {
		
		private static readonly char[] IllegalPathCharacters = new[] { '\\', '/', '"', '?', '<', '>' };
		private readonly IConfig config;
		private readonly string defaultDirectory;
		private readonly string defaultDirectorySearchPattern;
		private readonly SearchOption defaultDirectorySearchOption;
		private bool? addFileWatchers;
		private bool? loadAllFilesOnInit;
		private string configSuffix;
		private IEnumerable<string> configPaths;

		public FileConfigLoader(string defaultDirectory, string defaultDirectorySearchPattern, SearchOption defaultDirectorySearchOption, string configSuffix) {
			this.configSuffix = configSuffix;
			this.defaultDirectory = defaultDirectory;
			this.defaultDirectorySearchPattern = defaultDirectorySearchPattern;
			this.defaultDirectorySearchOption = defaultDirectorySearchOption;
		}

		public FileConfigLoader(string defaultDirectory, string configSuffix) : this(defaultDirectory, "*", SearchOption.AllDirectories, configSuffix) {}
		public FileConfigLoader(string defaultDirectory) : this(defaultDirectory, null) {}
		public FileConfigLoader() : this(AppDomain.CurrentDomain.BaseDirectory) {}

		public FileConfigLoader(IConfig config) : this() {
			this.config = config;
		}

		public ConfigCollection LoadOnInit() {
			var configs = new ConfigCollection();
			InitializeCollection(configs);
			return configs;
		}

		public void InitializeCollection(ConfigCollection configs) {
			if(LoadAllFilesOnInit) {
				LoadConfigsIntoCollection("*", configs);
			}
		}

		public IConfig Load(string configKey) {
			var configs = new ConfigCollection();
			LoadConfigsIntoCollection(configKey, configs);
			if(configs.Contains(configKey)) {
				return configs[configKey];
			}
			return null;
		}

		private void LoadConfigsIntoCollection(string pattern, ConfigCollection configs) {
			try {
				LoadConfigsIntoCollectionFromFiles(pattern, configs);
			}catch(Exception ex){
				throw new ConfiguratorException(string.Format("Error while loading XML configuration for the config with pattern '{0}'", pattern), ex);
			}
		}

		private void LoadConfigsIntoCollectionFromFiles(string pattern, ConfigCollection configs) {
			if(pattern.IndexOfAny(IllegalPathCharacters) < 0) {
				var files = GetFiles(pattern);
				foreach(var file in files) {
					var configFromFile = FileConfigFactory.Create(file, AddFileWatchers);
					if(!configs.Contains(configFromFile.ConfigKey)) {
						configs.Add(configFromFile);
					}
				}
			}
		}

		private IEnumerable<FileInfo> GetFiles(string pattern) {
			var files = new List<FileInfo>();
			foreach(var path in ConfigPaths) {
				var dir = GetDirectory(path);
				if(dir.Exists) {
					var fileArray = GetFiles(pattern, dir);
					files.AddRange(fileArray);
				}
			}
			return files;
		}

		private IEnumerable<FileInfo> GetFiles(string pattern, DirectoryInfo dir) {
			try {
				return dir.GetFiles(string.Format("{0}{1}", pattern, ConfigSuffix));
			} catch(IOException) {
				// Ignore IOException in case of incorrect syntax
			}
			return new FileInfo[0];
		}

		private static DirectoryInfo GetDirectory(string path) {
			if(HttpContext.Current != null && path.StartsWith("~")) {
				path = HttpContext.Current.Server.MapPath(path);
			}
			return new DirectoryInfo(path);
		}

		protected virtual bool AddFileWatchers {
			get {
				if(addFileWatchers == null) {
					addFileWatchers = true;
					if(config != null && config.ContainsAttribute("configDirectories", "enableFileWatching")) {
						addFileWatchers = config.GetAttribute<bool>("configDirectories", "enableFileWatching");
					}
				}
				return (bool)addFileWatchers;
			}
		}

		protected virtual bool LoadAllFilesOnInit {
			get {
				if(loadAllFilesOnInit == null) {
					loadAllFilesOnInit = false;
					if(config != null && config.ContainsAttribute("configDirectories", "loadAllConfigFilesOnInit")) {
						loadAllFilesOnInit = config.GetAttribute<bool>("configDirectories", "loadAllConfigFilesOnInit");
					}
				}
				return (bool)loadAllFilesOnInit;
			}
		}


		protected virtual string ConfigSuffix {
			get {
				if(configSuffix == null) {
					configSuffix =  ".config";
					if(config != null && config.ContainsAttribute("configDirectories", "configSuffix")) {
						configSuffix = config.GetAttribute("configDirectories", "configSuffix");
					}
				}
				return configSuffix;
			}
		}

		
		protected virtual IEnumerable<string> ConfigPaths {
			get {
				if(configPaths == null) {
					var paths = config != null ? config.GetValueArray("configDirectories", "configDirectory") : null;
					if(paths == null || paths.Length == 0) {
						var defaultPaths = new List<string>();
						var result = Directory.GetDirectories(DefaultDirectory, DefaultDirectorySearchPattern, DefaultDirectorySearchOption);
						defaultPaths.AddRange(result);
						if(!defaultPaths.Contains(DefaultDirectory)) {
							defaultPaths.Add(DefaultDirectory);	
						}
						paths = defaultPaths.ToArray();
					}
					configPaths = paths;
				}
				return configPaths;
			}
		}

		protected virtual string DefaultDirectory { get { return defaultDirectory; } }
		protected virtual string DefaultDirectorySearchPattern { get { return defaultDirectorySearchPattern; } }
		protected virtual SearchOption DefaultDirectorySearchOption { get { return defaultDirectorySearchOption; } }
	}
}
