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
using System.Collections.Generic;

namespace nJupiter.Configuration {
	public class ConfigSource : IConfigSource {
		protected readonly List<object> configSources;

		public virtual IConfigSourceWatcher Watcher { get { return null; } }

		public ConfigSource() {
			configSources = new List<object>();
		}

		public ConfigSource(List<object> configSources) {
			if(configSources == null) {
				throw new ArgumentNullException("configSources");
			}
			this.configSources = configSources;
		}

		public ConfigSource(object configSource) : this() {
			if(configSource == null) {
				throw new ArgumentNullException("configSource");
			}
			configSources.Add(configSource);
		}

		public T GetConfigSource<T>() {
			return (T)configSources.Find(s => s.GetType() == typeof(T));
		}

	}
}