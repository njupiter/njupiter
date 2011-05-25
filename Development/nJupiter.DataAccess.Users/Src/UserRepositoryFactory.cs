using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

using nJupiter.Configuration;

namespace nJupiter.DataAccess.Users {

	public class UserRepositoryFactory {

		private static readonly IList<UserRepositoryBase> UserProviders = new List<UserRepositoryBase>();

		private const string UsersRepositorySection = "usersRepositories/usersRepository";
		private const string UsersRepositoryDefaultSection = UsersRepositorySection + "[@default='true']";
		private const string UsersRepositorySectionFormat = UsersRepositorySection + "[@value='{0}']";
		private const string AssemblyPath = "assemblyPath";
		private const string Assembly = "assembly";
		private const string Type = "type";
		private const string Cache = "cache";

		private static readonly object Padlock = new object();

		private readonly IConfigHandler configHandler;

		public static UserRepositoryFactory Instance { get { return NestedSingleton.instance; } }

		public UserRepositoryFactory(IConfigHandler configHandler) {
			this.configHandler = configHandler;
		}

		/// <summary>
		/// Gets the default userRepository instance.
		/// </summary>
		/// <returns>The default userRepository instance.</returns>
		public IUserRepository CreateProvider() {
			return GetUserProviderFromSection(UsersRepositoryDefaultSection);
		}

		/// <summary>
		/// Gets the userRepository instance with the name <paramref name="name"/>.
		/// </summary>
		/// <param name="name">The userRepository name to get.</param>
		/// <returns>The userRepository instance with the name <paramref name="name"/></returns>
		public IUserRepository CreateProvider(string name) {
			return GetUserProviderFromSection(string.Format(CultureInfo.InvariantCulture, UsersRepositorySectionFormat, name));
		}


		private IUserRepository GetUserProviderFromSection(string section) {

			string name = ConfigHandler.Instance.GetConfig().GetValue(section);
			
			var provider = UserProviders.FirstOrDefault(userProvider => userProvider.Name == name);
			if(provider != null){
				return provider;
			}

			lock(Padlock) {
				if(!UserProviders.Any(userProvider => userProvider.Name == name)) {

					IConfig config = configHandler.GetConfig();

					string assemblyPath = config.GetValue(section, AssemblyPath);
					string assemblyName = config.GetValue(section, Assembly);
					string assemblyType = config.GetValue(section, Type);

					object instance = CreateInstance(assemblyPath, assemblyName, assemblyType);
					UserRepositoryBase userRepository = (UserRepositoryBase)instance;
					if(userRepository == null)
						throw new ConfigurationException(string.Format("Could not load DataSource from {0} {1} {2}.", assemblyName, assemblyType, assemblyPath));

					userRepository.Name = name;
					userRepository.Config = config.GetConfigSection(section + "/settings");
					if(userRepository.Config != null && userRepository.Config.ContainsKey(Cache)) {
						if(userRepository.Config.ContainsKey(Cache, Assembly)) {
							string cacheAssemblyName = userRepository.Config.GetValue(Cache, Assembly);
							string cacheAssemblyPath = userRepository.Config.GetValue(Cache, AssemblyPath);
							string cacheAssemblyType = userRepository.Config.GetValue(Cache, Type);
							object[] constructorArgs = { userRepository.Config };
							userRepository.UserCache = CreateInstance(cacheAssemblyPath, cacheAssemblyName, cacheAssemblyType, constructorArgs) as IUserCache;
						}
					}
					if(userRepository.UserCache == null) {
						userRepository.UserCache = new GenericUserCache(userRepository.Config);
					}

					userRepository.PropertyNames = PredefinedNamesFactory.Create(userRepository.Config);

					UserProviders.Add(userRepository);
					return userRepository;
				}
				return UserProviders.FirstOrDefault(userProvider => userProvider.Name == name);
			}
		}

		private static object CreateInstance(string assemblyPath, string assemblyName, string typeName) {
			return CreateInstance(assemblyPath, assemblyName, typeName, null);
		}

		private static object CreateInstance(string assemblyPath, string assemblyName, string typeName, object[] constructorArgs) {
			Assembly assembly;
			if(!string.IsNullOrEmpty(assemblyPath)) {
				assembly = System.Reflection.Assembly.LoadFrom(assemblyPath);
			} else if(assemblyName == null || assemblyName.Length.Equals(0) ||
				System.Reflection.Assembly.GetExecutingAssembly().GetName().Name.Equals(assemblyName)) {
				assembly = System.Reflection.Assembly.GetExecutingAssembly();	//Load current assembly
			} else {
				assembly = System.Reflection.Assembly.Load(assemblyName); // Late binding to an assembly on disk (current directory)
			}
			return assembly.CreateInstance(
				typeName, false,
				BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly |
				BindingFlags.Instance | BindingFlags.IgnoreCase,
				null, constructorArgs, null, null);
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
