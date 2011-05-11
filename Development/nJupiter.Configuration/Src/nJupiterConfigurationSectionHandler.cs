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

using System;
using System.Configuration;
using System.Reflection;
using System.Xml;

namespace nJupiter.Configuration {

	/// <summary>
	/// Configuration section handler for nJupiter.Configuration
	/// </summary>
	public class nJupiterConfigurationSectionHandler : IConfigurationSectionHandler {

		private const string ConfigElement = "nJupiterConfiguration";

		public static IConfig GetConfig() {
			try {
				return GetConfigInternal();
			} catch(System.Configuration.ConfigurationException confEx) {
				string configFile = AppDomain.CurrentDomain.GetData("APP_CONFIG_FILE").ToString();
				if(confEx.BareMessage.IndexOf("Unrecognized element") >= 0) {
					// Looks like the XML file is not valid
					throw new ConfiguratorException(string.Format("Failed to parse config file [{0}]. Check your .config file is well formed XML.", configFile), confEx);
				}
				// This exception is typically due to the assembly name not being correctly specified in the section type.
				string configSectionStr = string.Format("<section name=\"{0}\" type=\"nJupiter.Configuration.nJupiterConfigurationSectionHandler,{1}\" />", ConfigElement, Assembly.GetExecutingAssembly().FullName);
				throw new ConfiguratorException(string.Format("Failed to parse config file [{0}]. Is the <configSections> specified as: {1}", configFile, configSectionStr), confEx);
			}
		}

		private static IConfig GetConfigInternal() {
			XmlElement configElement = System.Configuration.ConfigurationManager.GetSection(ConfigElement) as XmlElement;
			if(configElement != null) {
				return ConfigFactory.Create(ConfigElement, configElement);
			}
			return null;			
		}

		#region Implementation of IConfigurationSectionHandler
		/// <summary>
		/// Creates a configuration section handler.
		/// </summary>
		/// <param name="parent">Parent object.</param>
		/// <param name="configContext">Configuration context object.</param>
		/// <param name="section">Section XML node.</param>
		/// <returns>The created section handler object.</returns>
		public object Create(object parent, object configContext, XmlNode section) {
			return section;
		}
		#endregion
	}

}
