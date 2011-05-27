using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using nJupiter.Configuration;

namespace nJupiter.DataAccess.Users {
	public class UserRepositoryFactory {

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

		public static UserRepositoryFactory Instance { get { return NestedSingleton.instance; } }

		public UserRepositoryFactory(IConfigHandler configHandler) {
			this.configHandler = configHandler;
		}

		/// <summary>
		/// Gets the default userRepository instance.
		/// </summary>
		/// <returns>The default userRepository instance.</returns>
		public IUserRepository Create() {
			return this.GetUserRepository(UsersRepositoryDefaultSection);
		}

		/// <summary>
		/// Gets the userRepository instance with the name <paramref name="name"/>.
		/// </summary>
		/// <param name="name">The userRepository name to get.</param>
		/// <returns>The userRepository instance with the name <paramref name="name"/></returns>
		public IUserRepository Create(string name) {
			return this.GetUserRepository(string.Format(CultureInfo.InvariantCulture, UsersRepositorySectionFormat, name));
		
		
		}

		private IUserRepository GetUserRepository(string section) {
			try {
				return GetUserRepositoryFromSection(section);
			}catch(Exception ex) {
				throw new ApplicationException(string.Format("Error while creating UserRepository with section [{0}]", section), ex);
			}
		}

		private IUserRepository GetUserRepositoryFromSection(string section) {

			string name = this.Config.GetAttribute(section, NameAttribute);
			
			var provider = this.GetUserRepositoryFromCache(name);
			if(provider == null) {
				lock(this.padlock) {
					provider = this.GetUserRepositoryFromCache(name);
					if(provider == null) {
						if(!this.userRepositories.Any(userProvider => userProvider.Name == name)) {
							provider = this.CreateUserRepository(name, section);
							this.userRepositories.Add(provider);
						}
					}
				}
			}
			return provider;
		}

		private IUserRepository GetUserRepositoryFromCache(string name) {
			return this.userRepositories.FirstOrDefault(userProvider => userProvider.Name == name);
		}

		private IUserRepository CreateUserRepository(string name, string section) {
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
				cache = CreateInstance(typeName) as IUserCache;
				if(cache == null) {
					cache = new GenericUserCache(settings);
				}
			}
			return cache;
		}

		private static object CreateInstance(string typeName) {
			try {
				Type userReporistoryType = System.Type.GetType(typeName, true, true);
				return Activator.CreateInstance(userReporistoryType);
			}catch(Exception ex) {
				throw new ApplicationException(string.Format("Error while creating instance of [{0}]", typeName), ex);
			}
		}

		// thread safe Singleton implementation with fully lazy instantiation and with full performance
		private sealed class NestedSingleton {
			// ReSharper disable EmptyConstructor
			static NestedSingleton() {} // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
			// ReSharper restore EmptyConstructor
			internal static readonly UserRepositoryFactory instance = new UserRepositoryFactory(ConfigHandler.Instance);
		}


	}
}
