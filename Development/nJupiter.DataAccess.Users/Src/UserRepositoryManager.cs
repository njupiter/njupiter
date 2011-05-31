using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using nJupiter.Configuration;
using nJupiter.DataAccess.Users.Caching;

namespace nJupiter.DataAccess.Users {
	public class UserRepositoryManager : IUserRepositoryManager {

		private readonly IList<IUserRepository> userRepositories = new List<IUserRepository>();

		private const string UsersRepositorySection = "userRepositories/userRepository";
		private const string NameAttribute = "name";
		private const string UsersRepositoryDefaultSection = UsersRepositorySection + "[@default='true']";
		private const string UsersRepositorySectionFormat = UsersRepositorySection + "[@name='{0}']";
		private const string UsersRepositoryFactoryElement = "userRepositoryFactory";
		private const string CacheFactoryTypeElement = "cache/userCacheFactory";
		private const string QualifiedNameAttribute = "qualifiedTypeName";

		private readonly object padlock = new object();

		private readonly IConfigHandler configHandler;
		private IConfig config;

		private IConfig Config {
			get {
				if(config == null) {
					lock(this.padlock) {
						if(config == null) {
							config = configHandler.GetConfig();
							config.Discarded += this.ConfigDiscarded;
						}
					}
				}
				return config;
			}
		}

		private void ConfigDiscarded(object sender, EventArgs e) {
			lock(padlock) {
				config = null;
				userRepositories.Clear();
			}
		}

		public static IUserRepositoryManager Instance { get { return NestedSingleton.instance; } }

		public UserRepositoryManager(IConfigHandler configHandler) {
			this.configHandler = configHandler;
		}

		public IUserRepository GetRepository() {
			return this.GetRepositoryFromSection(UsersRepositoryDefaultSection);
		}

		public IUserRepository GetRepository(string name) {
			string section = UsersRepositoryDefaultSection;
			if(!string.IsNullOrEmpty(name)) {
				section = string.Format(CultureInfo.InvariantCulture, UsersRepositorySectionFormat, name);
			}
			return this.GetRepositoryFromSection(section);
		
		
		}

		private IUserRepository GetRepositoryFromSection(string section) {
			try {
				string name = this.Config.GetAttribute(section, NameAttribute);
				return this.GetRepositoryFromCacheOrCreate(section, name);
			}catch(Exception ex) {
				throw new ApplicationException(string.Format("Error while creating UserRepository with section [{0}]", section), ex);
			}
		}

		private IUserRepository GetRepositoryFromCacheOrCreate(string section, string name) {
			IUserRepository userRepository = GetRepositoryFromCache(name);
			if(userRepository == null) {
				lock(this.padlock) {
					userRepository = GetRepositoryFromCache(name);
					if(userRepository == null) {
						userRepository = this.CreateRepository(name, section);
						this.userRepositories.Add(userRepository);
					}
				}
			}
			return userRepository;
		}

		private IUserRepository GetRepositoryFromCache(string name) {
			return this.userRepositories.FirstOrDefault(userProvider => userProvider.Name == name);
		}

		private IUserRepository CreateRepository(string name, string section) {
			var config = this.Config;
			var typeName = this.Config.GetAttribute(section, UsersRepositoryFactoryElement, QualifiedNameAttribute);
			var settings = config.GetConfigSection(string.Format("{0}/settings", section));
			var predifinedNames = PredefinedNamesFactory.Create(settings);
			var cache = GetUserCache(settings);

			IUserRepositoryFactory userRepositoryFactory = CreateInstance(typeName) as IUserRepositoryFactory;
			return userRepositoryFactory.Create(name, settings, predifinedNames, cache);
		}

		private static IUserCache GetUserCache(IConfig settings) {
			IUserCache cache = null;
			if(settings.ContainsAttribute(CacheFactoryTypeElement, QualifiedNameAttribute)) {
				var typeName = settings.GetAttribute(CacheFactoryTypeElement, QualifiedNameAttribute);
				var cacheFactory = (IUserCacheFactory)CreateInstance(typeName);
				cache = cacheFactory.Create(settings);
			}
			if(cache == null) {
				cache = new GenericUserCache(settings);
			}
			return cache;
		}

		private static object CreateInstance(string typeName) {
			Type userReporistoryType = System.Type.GetType(typeName, true, true);
			return Activator.CreateInstance(userReporistoryType);
		}

		// thread safe Singleton implementation with fully lazy instantiation and with full performance
		private sealed class NestedSingleton {
			// ReSharper disable EmptyConstructor
			static NestedSingleton() {} // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
			// ReSharper restore EmptyConstructor
			internal static readonly IUserRepositoryManager instance = new UserRepositoryManager(ConfigHandler.Instance);
		}


	}
}
