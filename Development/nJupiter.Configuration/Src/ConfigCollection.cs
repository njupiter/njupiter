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
using System.Collections;
using System.Collections.Generic;

namespace nJupiter.Configuration {

	/// <summary>
	/// A collection that can contain <see cref="IConfig" /> objects.
	/// </summary>
	[Serializable]
	public sealed class ConfigCollection : IEnumerable<IConfig> {

		private readonly Dictionary<String, IConfig> innerDictionary;
		private readonly object padLock = new object();

		internal ConfigCollection() {
			this.innerDictionary = new Dictionary<String, IConfig>(StringComparer.InvariantCultureIgnoreCase);
		}

		/// <summary>
		/// Gets the <see cref="IConfig"/> with the specified config key.
		/// </summary>
		/// <value></value>
		public IConfig this[string configKey] {
			get {
				return this.InnerDictionary[configKey];
			}
		}

		internal Dictionary<String, IConfig> InnerDictionary { get { return this.innerDictionary; } }

		public void Add(IConfig config) {
			lock(this.padLock) {
				if(config.IsDiscarded) {
					throw new ConfigDiscardedException("The config object is discarded and can not be added to the collection.");
				}
				config.Discarded += ConfigDiscard;
				this.InnerDictionary.Add(config.ConfigKey, config);
			}
		}

		public void Remove(string configKey) {
			lock(padLock){
				if(this.Contains(configKey)){
					var config = this[configKey];
					config.Discarded -= this.ConfigDiscard;
				}
				this.InnerDictionary.Remove(configKey);
			}
		}

		private void ConfigDiscard(object sender, EventArgs e) {
			var config = sender as IConfig;
			if(config != null){
				this.Remove(config.ConfigKey);
			}
		}

		/// <summary>
		/// Determines whether the collection contains the specified config object.
		/// </summary>
		/// <param name="config">The <see cref="IConfig" /> to locate in the collection.</param>
		/// <returns>
		/// 	<c>true</c> if the collection contains the specified config; otherwise, <c>false</c>.
		/// </returns>
		public bool Contains(IConfig config) {
			return this.InnerDictionary.ContainsValue(config);
		}

		/// <summary>
		/// Determines whether the collection contains the specified config object with the specifed config key.
		/// </summary>
		/// <param name="configKey">The config key for the <see cref="IConfig" /> object to locate in the colleciton.</param>
		/// <returns>
		/// 	<c>true</c> if the collection contains the specified config; otherwise, <c>false</c>.
		/// </returns>
		public bool Contains(string configKey) {
			return this.InnerDictionary.ContainsKey(configKey);
		}

		public IEnumerator<IConfig> GetEnumerator() {
			return innerDictionary.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return this.GetEnumerator();
		}
	}
}
