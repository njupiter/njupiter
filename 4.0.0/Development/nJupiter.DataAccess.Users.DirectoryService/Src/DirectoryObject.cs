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

namespace nJupiter.DataAccess.Users.DirectoryService {

	public class DirectoryObject {

		#region Members
		private readonly Hashtable innerHash = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
		#endregion

		#region Properties
		public string Id { get; set; }

		public string this[string propertyName] {
			get {
				CheckIfKey(propertyName);
				return ((Property)this.innerHash[propertyName]).Value;
			}
			set {
				CheckIfKey(propertyName);
				((Property)this.innerHash[propertyName]).Value = value;
			}
		}

		public Property[] Properties {
			get {
				Property[] properties = new Property[this.innerHash.Values.Count];
				this.innerHash.Values.CopyTo(properties, 0);
				return properties;
			}
			set {
				if(value == null) {
					throw new ArgumentNullException("value");
				}
				this.innerHash.Clear();
				foreach(Property property in value) {
					this.innerHash.Add(property.Name, property);
				}
			}
		}
		#endregion

		#region Methods
		public bool Contains(string propertyName) {
			return this.innerHash.ContainsKey(propertyName);
		}
		#endregion

		#region Private Methods
		private void CheckIfKey(string propertyName) {
			if(!this.Contains(propertyName)) {
				throw new ArgumentOutOfRangeException("propertyName");
			}
		}
		#endregion

	}
}