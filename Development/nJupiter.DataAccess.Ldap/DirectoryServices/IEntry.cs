using System;
using System.Collections;

using nJupiter.DataAccess.Ldap.DirectoryServices.Abstraction;

namespace nJupiter.DataAccess.Ldap.DirectoryServices {
	internal interface IEntry : IDisposable {
		IDictionary Properties { get; }
		object NativeObject { get; }
		string Name { get; }
		string Path { get; }
		IDirectoryEntry GetDirectoryEntry();
	}
}
