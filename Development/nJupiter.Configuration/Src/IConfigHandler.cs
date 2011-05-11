using System.Reflection;

namespace nJupiter.Configuration {
	public interface IConfigHandler {
		/// <summary>
		/// Gets the config key used for AppConfig.
		/// </summary>
		/// <value>The system config key.</value>
		string AppConfigKey { get; }
		/// <summary>
		/// Gets the config key used for SystemConfig.
		/// </summary>
		/// <value>The system config key.</value>
		string SystemConfigKey { get; }
		/// <summary>
		/// Gets all configuration objects.
		/// </summary>
		/// <value>The configurations.</value>
		ConfigCollection Configurations { get; }

		/// <summary>
		/// Gets the app config object.
		/// </summary>
		/// <returns>The app config object</returns>
		/// <exception cref="ConfigurationException">The config does not exist.</exception>
		IConfig GetAppConfig();

		/// <summary>
		/// Gets the system config object.
		/// </summary>
		/// <returns>The system config object</returns>
		/// <exception cref="ConfigurationException">The config does not exist.</exception>
		IConfig GetSystemConfig();

		/// <summary>
		/// Gets the config object for the current assembly calling the config handler.
		/// </summary>
		/// <returns>The config object for the current assembly.</returns>
		/// <exception cref="ConfigurationException">The config does not exist.</exception>
		IConfig GetConfig();

		/// <summary>
		/// Gets the config object for the current assembly calling the config handler.
		/// </summary>
		/// <param name="suppressMissingConfigException">if set to <c>true</c> suppress exception if config for the calling assembly is missing.</param>
		/// <returns>The config object for the current assembly.</returns>
		/// <exception cref="ConfigurationException">The config does not exist.</exception>
		IConfig GetConfig(bool suppressMissingConfigException);

		/// <summary>
		/// Gets the config object for a given assembly.
		/// </summary>
		/// <param name="assembly">The assembly.</param>
		/// <returns>A config object for the given assembly.</returns>
		/// <exception cref="ConfigurationException">The config does not exist.</exception>
		IConfig GetConfig(Assembly assembly);

		/// <summary>
		/// Gets the config object for a given assembly.
		/// </summary>
		/// <param name="assembly">The assembly.</param>
		/// <param name="suppressMissingConfigException">if set to <c>true</c> suppress exception if config for the given assembly is missing.</param>
		/// <returns>A config object for the given assembly.</returns>
		/// <exception cref="ConfigurationException">The config does not exist.</exception>
		IConfig GetConfig(Assembly assembly, bool suppressMissingConfigException);

		/// <summary>
		///  Gets the config object with a given config key.
		/// </summary>
		/// <param name="configKey">The config key.</param>
		/// <returns>A config object with the given config key.</returns>
		/// <exception cref="ConfigurationException">The config does not exist.</exception>
		IConfig GetConfig(string configKey);

		/// <summary>
		///  Gets the config object with a given config key.
		/// </summary>
		/// <param name="configKey">The config key.</param>
		/// <param name="suppressMissingConfigException">if set to <c>true</c> suppress exception if config for the given assembly is missing.</param>
		/// <returns>A config object with the given config key.</returns>
		/// <exception cref="ConfigurationException">The config does not exist.</exception>
		IConfig GetConfig(string configKey, bool suppressMissingConfigException);
	}
}