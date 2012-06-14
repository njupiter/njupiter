using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;

using NUnit.Framework;

using nJupiter.Configuration;

namespace nJupiter.DataAccess.Ldap.Tests.Integration {
	
	[TestFixture]
	public class LdapMembershipProviderTests{

		[Test]
		public void Method_Scenario_ExprectedResult() {
			var config = new NameValueCollection();
			config.Add("applicationName", typeof(LdapMembershipProvider).FullName);
			config.Add("ldapServer", "ad");
			var provider = new LdapMembershipProvider();

			var fileLoader = new FileConfigLoader();
			var result = fileLoader.Load("nJupiter.DataAccess.Ldap");

			provider.Initialize("Integration Tests", config);

			var user = provider.GetUser("modhelius", false);
		}
	}


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
					var configFromFile = FileConfigFactory.Create(file, this.AddFileWatchers);
					if(!configs.Contains(configFromFile.ConfigKey)) {
						configs.Add(configFromFile);
					}
				}
			}
		}

		private IEnumerable<FileInfo> GetFiles(string pattern) {
			var files = new List<FileInfo>();
			foreach(var path in this.ConfigPaths) {
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
				return dir.GetFiles(string.Format("{0}{1}", pattern, this.ConfigSuffix));
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
					IEnumerable<string> paths = config != null ? config.GetValueArray("configDirectories", "configDirectory") : null;
					if(paths == null || !paths.Any()) {
						var defaultPaths = new List<string>();
						var result = Directory.GetDirectories(DefaultDirectory, DefaultDirectorySearchPattern, DefaultDirectorySearchOption);
						defaultPaths.AddRange(result);
						if(!defaultPaths.Contains(DefaultDirectory)) {
							defaultPaths.Add(DefaultDirectory);	
						}
						paths = defaultPaths;
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
