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

using System.Reflection;

namespace nJupiter.Configuration {

	/// <summary>
	/// The configuration handler class that server configurations
	/// </summary>
	public static class ConfigHandlerOld {

		#region Properties
		/// <summary>
		/// Gets the config key used for SystemConfig.
		/// </summary>
		/// <value>The system config key.</value>
		public static string SystemConfigKey { get { return ConfigHandler.SystemConfigKey; } }
		/// <summary>
		/// Gets all configuration objects.
		/// </summary>
		/// <value>The configurations.</value>
		public static ConfigCollection Configurations {
			get {
				return ConfigHandler.Configurations;
			}
		}
		#endregion

		#region Singleton Implementation
		/// <summary>
		/// Returns a ConfigHandler instance
		/// </summary>
		internal static IConfigHandler ConfigHandler { get { return NestedSingleton.instance; } }

		// thread safe Singleton implementation with fully lazy instantiation and with full performance
		private sealed class NestedSingleton {
			// Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
			// ReSharper disable EmptyConstructor
			static NestedSingleton() {
				// ReSharper restore EmptyConstructor
			}
			internal static readonly IConfigHandler instance = new ConfigHandler(null, "system", "app");
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Gets the system config object.
		/// </summary>
		/// <returns>The system config object</returns>
		/// <exception cref="ConfigurationException">The config does not exist.</exception>
		public static IConfig GetSystemConfig() {
			return ConfigHandler.GetConfig();
		}

		/// <summary>
		/// Gets the config object for the current assembly calling the config handler.
		/// </summary>
		/// <returns>The config object for the current assembly.</returns>
		/// <exception cref="ConfigurationException">The config does not exist.</exception>
		public static IConfig GetConfig() {
			return ConfigHandler.GetConfig();
		}

		/// <summary>
		/// Gets the config object for the current assembly calling the config handler.
		/// </summary>
		/// <param name="suppressMissingConfigException">if set to <c>true</c> suppress exception if config for the calling assembly is missing.</param>
		/// <returns>The config object for the current assembly.</returns>
		/// <exception cref="ConfigurationException">The config does not exist.</exception>
		public static IConfig GetConfig(bool suppressMissingConfigException) {
			return ConfigHandler.GetConfig(suppressMissingConfigException);
		}

		/// <summary>
		/// Gets the config object for a given assembly.
		/// </summary>
		/// <param name="assembly">The assembly.</param>
		/// <returns>A config object for the given assembly.</returns>
		/// <exception cref="ConfigurationException">The config does not exist.</exception>
		public static IConfig GetConfig(Assembly assembly) {
			return ConfigHandler.GetConfig(assembly);
		}

		/// <summary>
		/// Gets the config object for a given assembly.
		/// </summary>
		/// <param name="assembly">The assembly.</param>
		/// <param name="suppressMissingConfigException">if set to <c>true</c> suppress exception if config for the given assembly is missing.</param>
		/// <returns>A config object for the given assembly.</returns>
		/// <exception cref="ConfigurationException">The config does not exist.</exception>
		public static IConfig GetConfig(Assembly assembly, bool suppressMissingConfigException) {
			return ConfigHandler.GetConfig(assembly, suppressMissingConfigException);
		}

		/// <summary>
		///  Gets the config object with a given config key.
		/// </summary>
		/// <param name="configKey">The config key.</param>
		/// <returns>A config object with the given config key.</returns>
		/// <exception cref="ConfigurationException">The config does not exist.</exception>
		public static IConfig GetConfig(string configKey) {
			return ConfigHandler.GetConfig(configKey);
		}

		/// <summary>
		///  Gets the config object with a given config key.
		/// </summary>
		/// <param name="configKey">The config key.</param>
		/// <param name="suppressMissingConfigException">if set to <c>true</c> suppress exception if config for the given assembly is missing.</param>
		/// <returns>A config object with the given config key.</returns>
		/// <exception cref="ConfigurationException">The config does not exist.</exception>
		public static IConfig GetConfig(string configKey, bool suppressMissingConfigException) {
			return ConfigHandler.GetConfig(configKey, suppressMissingConfigException);
		}
		#endregion



	}
}
