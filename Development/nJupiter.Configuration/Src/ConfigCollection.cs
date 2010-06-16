#region Copyright & License
/*
	Copyright (c) 2005-2010 nJupiter

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
	/// A collection that can contain <see cref="Config" /> objects.
	/// </summary>
	[Serializable]
	public sealed class ConfigCollection : IEnumerable {
		#region Members
		private readonly Dictionary<String, Config> innerHash;
		private readonly object padLock = new object();
		#endregion

		#region Constructors
		internal ConfigCollection() {
			this.innerHash = new Dictionary<String, Config>(StringComparer.InvariantCultureIgnoreCase);
		}
		#endregion

		#region Indexers
		/// <summary>
		/// Gets the <see cref="Config"/> with the specified config key.
		/// </summary>
		/// <value></value>
		public Config this[string configKey] {
			get {
				return this.InnerHash[configKey];
			}
			internal set {
				Config config = this.InnerHash[configKey];
				if(config != null) {
					InnerHash[configKey] = value;
					config.OnRemovedFromCache(EventArgs.Empty);
				} else {
					throw new ArgumentOutOfRangeException("configKey");
				}
			}
		}
		#endregion

		#region Properties
		internal Dictionary<String, Config> InnerHash { get { return this.innerHash; } }
		#endregion

		#region Internal Methods
		internal void Add(Config config) {
			InnerHash.Add(config.ConfigKey, config);
		}

		internal void Remove(string configKey) {
			Config config = this.InnerHash[configKey];
			InnerHash.Remove(configKey);
			if(config != null)
				config.OnRemovedFromCache(EventArgs.Empty);
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Determines whether the collection contains the specified config object.
		/// </summary>
		/// <param name="config">The <see cref="Config" /> to locate in the collection.</param>
		/// <returns>
		/// 	<c>true</c> if the collection contains the specified config; otherwise, <c>false</c>.
		/// </returns>
		public bool Contains(Config config) {
			return (InnerHash.ContainsValue(config));
		}

		/// <summary>
		/// Determines whether the collection contains the specified config object with the specifed config key.
		/// </summary>
		/// <param name="configKey">The config key for the <see cref="Config" /> object to locate in the colleciton.</param>
		/// <returns>
		/// 	<c>true</c> if the collection contains the specified config; otherwise, <c>false</c>.
		/// </returns>
		public bool Contains(string configKey) {
			return (InnerHash.ContainsKey(configKey));
		}

		/// <summary>
		/// Copies the collection or a portion of it to a one-dimensional array.
		/// </summary>
		/// <param name="array">The one-dimensional <see cref="Array" /> that is the destination of the elements copied from collection. The Array must have zero-based indexing. </param>
		/// <param name="index">The zero-based index in array at which copying begins.</param>
		public void CopyTo(Config[] array, int index) {
			InnerHash.Values.CopyTo(array, index);
		}
		#endregion

		#region Implementation of IEnumerable
		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
		/// </returns>
		IEnumerator IEnumerable.GetEnumerator() {
			return new ConfigCollectionEnumerator(this);
		}
		#endregion

		#region Implementation of ICollection
		/// <summary>
		/// Gets the number of elements actually contained in the collection.
		/// </summary>
		/// <value>The number of elements actually contained in the collection.</value>
		public int Count { get { return InnerHash.Count; } }
		internal object SyncRoot { get { return this.padLock; } }
		#endregion
	}

	#region ConfigCollectionEnumerator
	[Serializable]
	internal class ConfigCollectionEnumerator : IEnumerator {
		private readonly IEnumerator innerEnumerator;

		internal ConfigCollectionEnumerator(ConfigCollection enumerable) {
			innerEnumerator = enumerable.InnerHash.GetEnumerator();
		}

		#region Implementation of IEnumerator
		/// <summary>
		/// Sets the enumerator to its initial position, which is before the first element in the collection.
		/// </summary>
		/// <exception cref="T:System.InvalidOperationException">
		/// The collection was modified after the enumerator was created.
		/// </exception>
		public void Reset() {
			innerEnumerator.Reset();
		}

		/// <summary>
		/// Advances the enumerator to the next element of the collection.
		/// </summary>
		/// <returns>
		/// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
		/// </returns>
		/// <exception cref="T:System.InvalidOperationException">
		/// The collection was modified after the enumerator was created.
		/// </exception>
		public bool MoveNext() {
			return innerEnumerator.MoveNext();
		}

		/// <summary>
		/// Gets the current element in the collection.
		/// </summary>
		/// <value></value>
		/// <returns>
		/// The current element in the collection.
		/// </returns>
		/// <exception cref="T:System.InvalidOperationException">
		/// The enumerator is positioned before the first element of the collection or after the last element.
		/// </exception>
		public object Current { get { return ((KeyValuePair<string, Config>)innerEnumerator.Current).Value; } }
		#endregion
	}
	#endregion
}
