using System;
using System.ComponentModel;
using System.DirectoryServices;
using System.Runtime.Remoting;

namespace nJupiter.DataAccess.Ldap.Abstractions {
	internal interface IDirectoryEntry : IDisposable {
		AuthenticationTypes AuthenticationType { get; set; }
		DirectoryEntries Children { get; }
		IContainer Container { get; }
		Guid Guid { get; }
		string Name { get; }
		string NativeGuid { get; }
		object NativeObject { get; }
		ActiveDirectorySecurity ObjectSecurity { get; set; }
		DirectoryEntryConfiguration Options { get; }
		IDirectoryEntry Parent { get; }
		string Password { set; }
		string Path { get; set; }
		PropertyCollection Properties { get; }
		string SchemaClassName { get; }
		IDirectoryEntry SchemaEntry { get; }
		ISite Site { get; set; }
		bool UsePropertyCache { get; set; }
		string Username { get; set; }
		void Close();
		void CommitChanges();
		IDirectoryEntry CopyTo(IDirectoryEntry newParent);
		IDirectoryEntry CopyTo(IDirectoryEntry newParent, string newName);
		ObjRef CreateObjRef(Type requestedType);
		void DeleteTree();
		event EventHandler Disposed;
		object GetLifetimeService();
		object InitializeLifetimeService();
		object Invoke(string methodName, params object[] args);
		object InvokeGet(string propertyName);
		void InvokeSet(string propertyName, params object[] args);
		void MoveTo(IDirectoryEntry newParent);
		void MoveTo(IDirectoryEntry newParent, string newName);
		void RefreshCache();
		void RefreshCache(string[] propertyNames);
		void Rename(string newName);
	}
}