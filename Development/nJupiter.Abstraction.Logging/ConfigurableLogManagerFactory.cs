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

using nJupiter.Configuration;

namespace nJupiter.Abstraction.Logging {
	public class ConfigurableLogManagerFactory {
		private const string ConfigSection = "logManager";
		private const string QualifiedNameAttribute = "qualifiedTypeName";

		private ILogManager instance;

		private readonly object padlock = new object();

		private readonly IConfigRepository configRepository;

		public ConfigurableLogManagerFactory(IConfigRepository configRepository) {
			this.configRepository = configRepository;
		}

		public ILogManager GetInstance() {
			if(instance == null) {
				lock(padlock) {
					if(instance == null) {
						RegisterInstance(Create());
					}
				}
			}
			return instance;
		}

		private void RegisterInstance(ILogManager logManager) {
			instance = logManager;
		}

		private ILogManager Create() {
			var config = configRepository.GetConfig(true);
			if(config != null) {
				config.Discarded += ConfigDiscarded;
				return Create(config);
			}
			return null;
		}

		private ILogManager Create(IConfig config) {
			var typeName = config.GetAttribute(ConfigSection, QualifiedNameAttribute);
			var manager = Create(typeName) as ILogManager;
			if(manager == null) {
				throw new TypeLoadException(string.Format("Type '{0}' is not an instance of '{1}'", typeName, typeof(ILogManager).FullName));
			}
			return manager;
		}

		private object Create(string typeName) {
			var userReporistoryType = Type.GetType(typeName, true, true);
			return Activator.CreateInstance(userReporistoryType);
		}

		private void ConfigDiscarded(object sender, EventArgs e) {
			var config = sender as IConfig;
			if(config != null) {
				config.Discarded -= ConfigDiscarded;
			}
			instance = null;
		}
	}
}