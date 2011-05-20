using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

using nJupiter.Configuration;

namespace nJupiter.DataAccess.Users {

	public class UserProviderFactory {

		private static readonly IList<UserProviderBase> UserProviders = new List<UserProviderBase>();

		private const string UsersProviderSection = "usersProviders/usersProvider";
		private const string UsersProviderDefaultSection = UsersProviderSection + "[@default='true']";
		private const string UsersProviderSectionFormat = UsersProviderSection + "[@value='{0}']";
		private const string AssemblyPath = "assemblyPath";
		private const string Assembly = "assembly";
		private const string Type = "type";
		private const string Cache = "cache";

		private static readonly object Padlock = new object();

		private readonly IConfigHandler configHandler;

		public static UserProviderFactory Instance { get { return NestedSingleton.instance; } }

		public UserProviderFactory(IConfigHandler configHandler) {
			this.configHandler = configHandler;
		}

		/// <summary>
		/// Gets the default UserProvider instance.
		/// </summary>
		/// <returns>The default UserProvider instance.</returns>
		public IUserProvider CreateProvider() {
			return GetUserProviderFromSection(UsersProviderDefaultSection);
		}

		/// <summary>
		/// Gets the UserProvider instance with the name <paramref name="name"/>.
		/// </summary>
		/// <param name="name">The UserProvider name to get.</param>
		/// <returns>The UserProvider instance with the name <paramref name="name"/></returns>
		public IUserProvider CreateProvider(string name) {
			return GetUserProviderFromSection(string.Format(CultureInfo.InvariantCulture, UsersProviderSectionFormat, name));
		}


		private IUserProvider GetUserProviderFromSection(string section) {

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
					UserProviderBase userProvider = (UserProviderBase)instance;
					if(userProvider == null)
						throw new ConfigurationException(string.Format("Could not load DataSource from {0} {1} {2}.", assemblyName, assemblyType, assemblyPath));

					userProvider.Name = name;
					userProvider.Config = config.GetConfigSection(section + "/settings");
					if(userProvider.Config != null && userProvider.Config.ContainsKey(Cache)) {
						if(userProvider.Config.ContainsKey(Cache, Assembly)) {
							string cacheAssemblyName = userProvider.Config.GetValue(Cache, Assembly);
							string cacheAssemblyPath = userProvider.Config.GetValue(Cache, AssemblyPath);
							string cacheAssemblyType = userProvider.Config.GetValue(Cache, Type);
							object[] constructorArgs = { userProvider };
							userProvider.UserCache = CreateInstance(cacheAssemblyPath, cacheAssemblyName, cacheAssemblyType, constructorArgs) as IUserCache;
						}
					}
					if(userProvider.UserCache == null) {
						userProvider.UserCache = new GenericUserCache(userProvider);
					}

					var contextNames = CommonNamesFactory.CreateCommonContextNames(userProvider.Config);
					userProvider.PropertyNames = CommonNamesFactory.CreateCommonPropertyNames(userProvider.Config, contextNames);

					UserProviders.Add(userProvider);
					return userProvider;
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
			internal static readonly UserProviderFactory instance = new UserProviderFactory(ConfigHandler.Instance);
		}


	}
}
