#region Copyright & License
// 
// 	Copyright (c) 2005-2012 nJupiter
// 
// 	Permission is hereby granted, free of charge, to any person obtaining a copy
// 	of this software and associated documentation files (the "Software"), to deal
// 	in the Software without restriction, including without limitation the rights
// 	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// 	copies of the Software, and to permit persons to whom the Software is
// 	furnished to do so, subject to the following conditions:
// 
// 	The above copyright notice and this permission notice shall be included in
// 	all copies or substantial portions of the Software.
// 
// 	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// 	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// 	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// 	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// 	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// 	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// 	THE SOFTWARE.
// 
#endregion

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