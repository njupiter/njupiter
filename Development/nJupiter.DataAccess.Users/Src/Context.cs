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

namespace nJupiter.DataAccess.Users {
	[Serializable]
	public class Context {

		#region Members
		private readonly string name;
		private readonly PropertySchemaTable propertySchemaTable;
		#endregion

		#region Constructors
		private Context() {
		}

		internal Context(string contextName, PropertySchemaTable schemaTable)
			: this() {
			if(contextName == null)
				throw new ArgumentNullException("contextName");
			if(schemaTable == null)
				throw new ArgumentNullException("schemaTable");
			if(contextName.Length == 0)
				throw new ArgumentException("The context name can not be empty.", "contextName");
			this.name = contextName;
			this.propertySchemaTable = schemaTable;
		}
		#endregion

		#region Properties
		public string Name { get { return this.name; } }
		public PropertySchemaTable PropertySchemas { get { return this.propertySchemaTable; } }
		#endregion

		#region Methods
		public override int GetHashCode() {
			return this.Name.ToLowerInvariant().GetHashCode();
		}

		public override bool Equals(object obj) {
			Context objContext = obj as Context;
			return objContext != null && string.Equals(objContext.Name, this.Name, StringComparison.InvariantCultureIgnoreCase);
		}
		#endregion
	}
}
