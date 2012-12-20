#region Copyright & License
// 
// 	Copyright (c) 2005-2012 nJupiter
// 
// 	Permission is hereby granted, free of charge, to any person obtaining a copy
// 	of this software and associated documentation files (the "Software"), to deal
// 	in the Software without restriction, including without limitation the rights
// 	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// 	copies of the Software, and to permit persons to whom the Software is
// 	furnished to do so, subject to the following conditions:
// 
// 	The above copyright notice and this permission notice shall be included in
// 	all copies or substantial portions of the Software.
// 
// 	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// 	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// 	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// 	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// 	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// 	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// 	THE SOFTWARE.
// 
#endregion

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
					lock(padlock) {
						config = config ?? GetConfig();
					}
				}
				return config;
			}
		}

		private IConfig GetConfig() {
			var c = configRepository.GetConfig();
			c.Discarded += ConfigDiscarded;
			return c;
		}

		private void ConfigDiscarded(object sender, EventArgs e) {
			lock(padlock) {
				var c = sender as IConfig;
				if(c != null) {
					c.Discarded -= ConfigDiscarded;
				}
				config = null;
				userRepositories.Clear();
			}
		}

		public static IUserRepositoryManager Instance { get { return NestedSingleton.instance; } }

		public UserRepositoryManager(IConfigRepository configRepository) {
			this.configRepository = configRepository;
		}

		public IUserRepository GetRepository() {
			return GetRepositoryFromSection(UsersRepositoryDefaultSection);
		}

		public IUserRepository GetRepository(string name) {
			string section = UsersRepositoryDefaultSection;
			if(!string.IsNullOrEmpty(name)) {
				section = string.Format(CultureInfo.InvariantCulture, UsersRepositorySectionFormat, name);
			}
			return GetRepositoryFromSection(section);
		}

		private IUserRepository GetRepositoryFromSection(string section) {
			try {
				string name = Config.GetAttribute(section, NameAttribute);
				return GetRepositoryFromCacheOrCreate(section, name);
			} catch(Exception ex) {
				throw new ApplicationException(string.Format("Error while creating UserRepository with section '{0}'", section), ex);
			}
		}

		private IUserRepository GetRepositoryFromCacheOrCreate(string section, string name) {
			var userRepository = GetRepositoryFromCache(name);
			if(userRepository == null) {
				lock(padlock) {
					userRepository = GetRepositoryFromCache(name);
					if(userRepository == null) {
						userRepository = CreateRepository(name, section);
						userRepositories.Add(userRepository);
					}
				}
			}
			return userRepository;
		}

		private IUserRepository GetRepositoryFromCache(string name) {
			return userRepositories.FirstOrDefault(userProvider => userProvider.Name == name);
		}

		private IUserRepository CreateRepository(string name, string section) {
			var typeName = Config.GetAttribute(section, QualifiedNameAttribute);
			var repositoryConfig = Config.GetConfigSection(string.Format("{0}/settings", section));
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
			var userReporistoryType = Type.GetType(typeName, true, true);
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