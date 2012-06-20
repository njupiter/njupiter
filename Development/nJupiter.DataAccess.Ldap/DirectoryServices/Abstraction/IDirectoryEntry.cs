using System;
using System.ComponentModel;
using System.DirectoryServices;
using System.Runtime.Remoting;

namespace nJupiter.DataAccess.Ldap.DirectoryServices.Abstraction {
	internal interface IDirectoryEntry : IEntry {
		AuthenticationTypes AuthenticationType { get; set; }
		DirectoryEntries Children { get; }
		IContainer Container { get; }
		Guid Guid { get; }
		string NativeGuid { get; }
		ActiveDirectorySecurity ObjectSecurity { get; set; }
		DirectoryEntryConfiguration Options { get; }
		IEntry Parent { get; }
		string Password { set; }
		string SchemaClassName { get; }
		IEntry SchemaEntry { get; }
		ISite Site { get; set; }
		bool UsePropertyCache { get; set; }
		string Username { get; set; }
		void Close();
		void CommitChanges();
		IEntry CopyTo(IEntry newParent);
		IEntry CopyTo(IEntry newParent, string newName);
		ObjRef CreateObjRef(Type requestedType);
		void DeleteTree();
		event EventHandler Disposed;
		object GetLifetimeService();
		object InitializeLifetimeService();
		object Invoke(string methodName, params object[] args);
		object InvokeGet(string propertyName);
		void InvokeSet(string propertyName, params object[] args);
		void MoveTo(IEntry newParent);
		void MoveTo(IEntry newParent, string newName);
		void RefreshCache();
		void RefreshCache(string[] propertyNames);
		void Rename(string newName);
	}
}