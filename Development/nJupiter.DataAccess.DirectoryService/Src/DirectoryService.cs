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

using System.Collections;
using System.Reflection;
using System.Globalization;

using nJupiter.Configuration;

namespace nJupiter.DataAccess.DirectoryService {

	public abstract class DirectoryService {
		#region Constants
		private const string DirectoryServicesSection = "directoryServices";
		private const string DirectoryServiceSection = DirectoryServicesSection + "/directoryService";
		private const string DirectoryServiceSectionFormat = DirectoryServiceSection + "[@value='{0}']";
		private const string SettingsSectionFormat = DirectoryServiceSectionFormat + "/settings";
		#endregion

		#region Static Members
		private static readonly Hashtable DirectoryServices = Hashtable.Synchronized(new Hashtable());
		#endregion

		#region Members
		private IConfig settings;
		#endregion

		#region Methods
		public abstract DirectoryObject GetDirectoryObjectById(string id);

		public abstract DirectoryObject[] GetDirectoryObjectsBySearchCriteria(SearchCriteria[] searchCriteria);

		public abstract void SaveDirectoryObject(DirectoryObject directoryObject);

		public abstract DirectoryObject CreateDirectoryObjectInstance();
		#endregion

		#region Simple Factory Methods
		public static DirectoryService GetInstance() {
			const string section = DirectoryServiceSection + "[@default='true']";
			return GetDirectoryServiceFromSection(section);
		}

		public static DirectoryService GetInstance(string name) {
			const string sectionFormat = DirectoryServiceSection + "[@value='{0}']";
			return GetDirectoryServiceFromSection(string.Format(CultureInfo.InvariantCulture, sectionFormat, name));
		}

		private static DirectoryService GetDirectoryServiceFromSection(string section) {
			const string assemblyPathKey = "assemblyPath";
			const string assemblyKey = "assembly";
			const string typKey = "type";

			IConfig config = ConfigHandler.Instance.GetConfig();
			string name = config.GetValue(section);
			if(DirectoryServices.ContainsKey(name))
				return (DirectoryService)DirectoryServices[name];

			lock(DirectoryServices.SyncRoot) {
				if(!DirectoryServices.ContainsKey(name)) {

					string assemblyPath = config.GetValue(section, assemblyPathKey);
					string assemblyName = config.GetValue(section, assemblyKey);
					string assemblyType = config.GetValue(section, typKey);

					object instance = CreateInstance(assemblyPath, assemblyName, assemblyType);
					DirectoryService directoryService = (DirectoryService)instance;
					if(directoryService == null)
						throw new ConfigurationException("Could not load DirectoryService from " + assemblyName + " " + assemblyType + " " + assemblyPath + ".");

					directoryService.settings = config.GetConfigSection(string.Format(CultureInfo.InvariantCulture, SettingsSectionFormat, name));

					DirectoryServices.Add(name, directoryService);
					return directoryService;
				}
				return (DirectoryService)DirectoryServices[name];
			}
		}

		private static object CreateInstance(string assemblyPath, string assemblyName, string typeName) {
			Assembly assembly;
			if(!string.IsNullOrEmpty(assemblyPath)) {
				assembly = Assembly.LoadFrom(assemblyPath);
			} else if(assemblyName == null || assemblyName.Length.Equals(0) ||
				Assembly.GetExecutingAssembly().GetName().Name.Equals(assemblyName)) {
				assembly = Assembly.GetExecutingAssembly();	//Load current assembly
			} else {
				assembly = Assembly.Load(assemblyName); // Late binding to an assembly on disk (current directory)
			}
			return assembly.CreateInstance(
				typeName, false,
				BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly |
				BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.ExactBinding,
				null, null, null, null);
		}
		#endregion

		#region Protected Properties
		protected IConfig Settings { get { return this.settings; } }
		#endregion
	}
}
