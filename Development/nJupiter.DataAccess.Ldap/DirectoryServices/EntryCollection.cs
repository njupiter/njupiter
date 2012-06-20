using System.Collections;
using System.Collections.Generic;

namespace nJupiter.DataAccess.Ldap.DirectoryServices {
	internal class EntryCollection : IEntryCollection {
		
		private readonly List<IEntry> innerCollection = new List<IEntry>();

		public IEnumerator<IEntry> GetEnumerator() {
			return innerCollection.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		public void Dispose() {
			foreach(var entry in this) {
				entry.Dispose();
			}
		}

		public void Add(IEntry entry) {
			innerCollection.Add(entry);
		}
	}
}