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
using System.Collections;
using System.ComponentModel;
using System.DirectoryServices;
using System.Runtime.Remoting;

namespace nJupiter.DataAccess.Ldap.DirectoryServices.Abstraction {
	public class DirectoryEntryWrapper : IDirectoryEntry {
		private readonly DirectoryEntry directoryEntry;

		public IDirectoryEntry GetDirectoryEntry() {
			return this;
		}

		public DirectoryEntryWrapper(DirectoryEntry directoryEntry) {
			this.directoryEntry = directoryEntry;
		}

		public virtual DirectoryEntry WrappedEntry { get { return directoryEntry; } }

		public virtual AuthenticationTypes AuthenticationType { get { return directoryEntry.AuthenticationType; } set { directoryEntry.AuthenticationType = value; } }

		public virtual DirectoryEntries Children { get { return directoryEntry.Children; } }

		public virtual void Close() {
			directoryEntry.Close();
		}

		public virtual void CommitChanges() {
			directoryEntry.CommitChanges();
		}

		public virtual IContainer Container { get { return directoryEntry.Container; } }

		public virtual IEntry CopyTo(IEntry newParent) {
			return directoryEntry.CopyTo(newParent.UnWrap()).Wrap();
		}

		public virtual IEntry CopyTo(IEntry newParent, string newName) {
			return directoryEntry.CopyTo(newParent.UnWrap(), newName).Wrap();
		}

		public virtual ObjRef CreateObjRef(Type requestedType) {
			return directoryEntry.CreateObjRef(requestedType);
		}

		public virtual void DeleteTree() {
			directoryEntry.DeleteTree();
		}

		public virtual void Dispose() {
			directoryEntry.Dispose();
		}

		public virtual event EventHandler Disposed { add { directoryEntry.Disposed += value; } remove { directoryEntry.Disposed -= value; } }

		public virtual object GetLifetimeService() {
			return directoryEntry.GetLifetimeService();
		}

		public virtual Guid Guid { get { return directoryEntry.Guid; } }

		public virtual object InitializeLifetimeService() {
			return directoryEntry.InitializeLifetimeService();
		}

		public virtual object Invoke(string methodName, params object[] args) {
			return directoryEntry.Invoke(methodName, args);
		}

		public virtual object InvokeGet(string propertyName) {
			return directoryEntry.InvokeGet(propertyName);
		}

		public virtual void InvokeSet(string propertyName, params object[] args) {
			directoryEntry.InvokeSet(propertyName, args);
		}

		public virtual void MoveTo(IEntry newParent) {
			directoryEntry.MoveTo(newParent.UnWrap());
		}

		public virtual void MoveTo(IEntry newParent, string newName) {
			directoryEntry.MoveTo(newParent.UnWrap(), newName);
		}

		public virtual string Name { get { return directoryEntry.Name; } }

		public virtual string NativeGuid { get { return directoryEntry.NativeGuid; } }

		public virtual object NativeObject { get { return directoryEntry.NativeObject; } }

		public virtual ActiveDirectorySecurity ObjectSecurity { get { return directoryEntry.ObjectSecurity; } set { directoryEntry.ObjectSecurity = value; } }

		public virtual DirectoryEntryConfiguration Options { get { return directoryEntry.Options; } }

		public virtual IEntry Parent { get { return directoryEntry.Parent.Wrap(); } }

		public virtual string Password { set { directoryEntry.Password = value; } }

		public virtual string Path { get { return directoryEntry.Path; } set { directoryEntry.Path = value; } }

		public virtual IDictionary Properties { get { return directoryEntry.Properties; } }

		public virtual void RefreshCache() {
			directoryEntry.RefreshCache();
		}

		public virtual void RefreshCache(string[] propertyNames) {
			directoryEntry.RefreshCache(propertyNames);
		}

		public virtual void Rename(string newName) {
			directoryEntry.Rename(newName);
		}

		public virtual string SchemaClassName { get { return directoryEntry.SchemaClassName; } }

		public virtual IEntry SchemaEntry { get { return directoryEntry.SchemaEntry.Wrap(); } }

		public virtual ISite Site { get { return directoryEntry.Site; } set { directoryEntry.Site = value; } }

		public virtual bool UsePropertyCache { get { return directoryEntry.UsePropertyCache; } set { directoryEntry.UsePropertyCache = value; } }

		public virtual string Username { get { return directoryEntry.Username; } set { directoryEntry.Username = value; } }
	}
}