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

using nJupiter.Abstraction.Logging.NullLogger;
using nJupiter.Configuration;

namespace nJupiter.Abstraction.Logging {
	public class LogManagerFactory {

		private static readonly object PadLock = new object();
		private static Func<ILogManager> factoryMethod;
		private static readonly ILogManager nullLogManager = new NullLogManager();

		public static bool FactoryRegistered { get { return factoryMethod != null; } }

		public static void RegisterFactory(Func<ILogManager> factory) {
			factoryMethod = factory;
		}

		public static ILogManager GetLogManager() {
			Initialize();
			var result = factoryMethod();
			if(result == null) {
				return nullLogManager;
			}
			return result;
		}

		private static void Initialize() {
			if(factoryMethod == null) {
				lock(PadLock) {
					var result = factoryMethod;
					if(factoryMethod == null) {
						result = CreateDefaultFactoryMethod();
					}
					factoryMethod = result;
				}
			}
		}

		private static Func<ILogManager> CreateDefaultFactoryMethod() {
			var defaultManger = new ConfigurableLogManagerFactory(ConfigRepository.Instance);
			return defaultManger.GetInstance;
		}

	}
}