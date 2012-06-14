using System;
using System.Collections;

namespace nJupiter.DataAccess.Ldap.DirectoryServices.Abstractions {
	internal interface IEntry : IDisposable {
		IDictionary Properties { get; }
		object NativeObject { get; }
		string Name { get; }
		string Path { get; }
		IDirectoryEntry GetDirectoryEntry();
	}
}
