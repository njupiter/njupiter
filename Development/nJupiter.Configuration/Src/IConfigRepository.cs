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
	public interface IConfigRepository {
		
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