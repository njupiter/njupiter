using System;
using System.Collections;
using System.Collections.Generic;

namespace nJupiter.DataAccess.Ldap.DirectoryServices.Abstractions {
	internal interface IEntry : IDisposable {
		IDictionary Properties { get; }
		object NativeObject { get; }
		string Name { get; }
		string Path { get; }
		IDirectoryEntry GetDirectoryEntry();
	}

	internal interface IEntityCollection : IEnumerable<IEntry>, IDisposable {}


	internal class EntityCollection : IEntityCollection {
		
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
