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
using System.Collections.Generic;
using System.Linq;

namespace nJupiter.DataAccess.Users {

	public class PropertyCollection : IEnumerable<IProperty>, ILockable {
		
		private readonly IList<IProperty> innerList;
		private readonly ContextSchema schema;
		private bool isReadOnly;

		public ContextSchema Schema { get { return this.schema; } }

		public int Count { get { return innerList.Count; } }

		public PropertyCollection() : this(null, null) {}

		public PropertyCollection(IList<IProperty> innerList, ContextSchema schema) {
			this.innerList = innerList ?? new List<IProperty>();
			this.schema = schema ?? new ContextSchema(new List<PropertyDefinition>());
		}

		public IEnumerator<IProperty> GetEnumerator() {
			return innerList.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return innerList.GetEnumerator();
		}

		public object Clone() {
			var newList = this.innerList.Select(property => (IProperty)property.Clone()).ToList();
			return new PropertyCollection(newList, this.Schema) { isReadOnly = false };
		}

		public void MakeReadOnly() {
			isReadOnly = true;
			foreach(IProperty property in innerList) {
				property.MakeReadOnly();
			}
		}

		public bool IsReadOnly { get { return isReadOnly; } }
	}
}
