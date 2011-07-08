using System.Collections;
using System.Collections.Generic;

namespace nJupiter.DataAccess.Users {

	public class ContextSchema : IEnumerable<PropertyDefinition> {
		private readonly IList<PropertyDefinition> innerList;
		
		public ContextSchema(IList<PropertyDefinition> innerList) {
			this.innerList = innerList;
		}

		public int Count { get { return this.innerList.Count; } }

		public IEnumerator<PropertyDefinition> GetEnumerator() {
			return this.innerList.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return this.GetEnumerator();
		}


	}
}
