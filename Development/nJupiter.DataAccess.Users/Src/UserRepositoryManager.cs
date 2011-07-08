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
		private const string CacheTypeElement = "cache";
		private const string QualifiedNameAttribute = "qualifiedTypeName";

		private readonly object padlock = new object();

		private readonly IConfigRepository configRepository;
		private IConfig config;

		private IConfig Config {
			get {
				if(config == null) {
					lock(this.padlock) {
						if(config == null) {
							config = this.configRepository.GetConfig();
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

		public UserRepositoryManager(IConfigRepository configRepository) {
			this.configRepository = configRepository;
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
				throw new ApplicationException(string.Format("Error while creating UserRepository with section '{0}'", section), ex);
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
			var typeName = this.Config.GetAttribute(section, QualifiedNameAttribute);
			var repositoryConfig = this.Config.GetConfigSection(string.Format("{0}/settings", section));
			var predifinedNames = PredefinedNamesFactory.Create(repositoryConfig);
			var cache = GetUserCache(repositoryConfig);

			return CreateInstance(typeName, name, repositoryConfig, predifinedNames, cache) as IUserRepository;
		}

		private static IUserCache GetUserCache(IConfig settings) {
			IUserCache cache = null;
			if(settings.ContainsAttribute(CacheTypeElement, QualifiedNameAttribute)) {
				var typeName = settings.GetAttribute(CacheTypeElement, QualifiedNameAttribute);
				cache = (IUserCache)CreateInstance(typeName, settings);
			}
			return cache ?? new GenericUserCache(settings);
		}

		private static object CreateInstance(string typeName, params object[] constructorParameters) {
			Type userReporistoryType = System.Type.GetType(typeName, true, true);
			if(userReporistoryType == null) {
				throw new ApplicationException(string.Format("Type '{0}' not found", typeName));
			}
			return Activator.CreateInstance(userReporistoryType, constructorParameters);
		}

		// thread safe Singleton implementation with fully lazy instantiation and with full performance
		private sealed class NestedSingleton {
			// ReSharper disable EmptyConstructor
			static NestedSingleton() {} // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
			// ReSharper restore EmptyConstructor
			internal static readonly IUserRepositoryManager instance = new UserRepositoryManager(ConfigRepository.Instance);
		}


	}
}
