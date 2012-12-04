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
using System.Collections.Specialized;
using System.ComponentModel;
using System.DirectoryServices;
using System.Runtime.Remoting;

namespace nJupiter.DataAccess.Ldap.DirectoryServices.Abstraction {
	public class DirectorySearcherWrapper {
		private readonly DirectorySearcher directorySearcher;

		public DirectorySearcherWrapper(IEntry directoryEntry) {
			directorySearcher = new DirectorySearcher(directoryEntry.UnWrap());
		}

		public virtual bool Asynchronous { get { return directorySearcher.Asynchronous; } set { directorySearcher.Asynchronous = value; } }

		public virtual string AttributeScopeQuery { get { return directorySearcher.AttributeScopeQuery; } set { directorySearcher.AttributeScopeQuery = value; } }

		public virtual bool CacheResults { get { return directorySearcher.CacheResults; } set { directorySearcher.CacheResults = value; } }

		public virtual TimeSpan ClientTimeout { get { return directorySearcher.ClientTimeout; } set { directorySearcher.ClientTimeout = value; } }

		public virtual IContainer Container { get { return directorySearcher.Container; } }

		public virtual ObjRef CreateObjRef(Type requestedType) {
			return directorySearcher.CreateObjRef(requestedType);
		}

		public virtual DereferenceAlias DerefAlias { get { return directorySearcher.DerefAlias; } set { directorySearcher.DerefAlias = value; } }

		public virtual DirectorySynchronization DirectorySynchronization { get { return directorySearcher.DirectorySynchronization; } set { directorySearcher.DirectorySynchronization = value; } }

		public virtual void Dispose() {
			directorySearcher.Dispose();
		}

		public virtual event EventHandler Disposed { add { directorySearcher.Disposed += value; } remove { directorySearcher.Disposed -= value; } }

		public virtual ExtendedDN ExtendedDN { get { return directorySearcher.ExtendedDN; } set { directorySearcher.ExtendedDN = value; } }

		public virtual string Filter { get { return directorySearcher.Filter; } set { directorySearcher.Filter = value; } }

		public virtual IEntryCollection FindAll() {
			return directorySearcher.FindAll().Wrap();
		}

		public virtual IEntry FindOne() {
			return directorySearcher.FindOne().Wrap();
		}

		public virtual object GetLifetimeService() {
			return directorySearcher.GetLifetimeService();
		}

		public virtual object InitializeLifetimeService() {
			return directorySearcher.InitializeLifetimeService();
		}

		public virtual int PageSize { get { return directorySearcher.PageSize; } set { directorySearcher.PageSize = value; } }

		public virtual StringCollection PropertiesToLoad { get { return directorySearcher.PropertiesToLoad; } }

		public virtual bool PropertyNamesOnly { get { return directorySearcher.PropertyNamesOnly; } set { directorySearcher.PropertyNamesOnly = value; } }

		public virtual ReferralChasingOption ReferralChasing { get { return directorySearcher.ReferralChasing; } set { directorySearcher.ReferralChasing = value; } }

		public virtual IEntry SearchRoot { get { return directorySearcher.SearchRoot.Wrap(); } set { directorySearcher.SearchRoot = value.UnWrap(); } }

		public virtual SearchScope SearchScope { get { return directorySearcher.SearchScope; } set { directorySearcher.SearchScope = value; } }

		public virtual SecurityMasks SecurityMasks { get { return directorySearcher.SecurityMasks; } set { directorySearcher.SecurityMasks = value; } }

		public virtual TimeSpan ServerPageTimeLimit { get { return directorySearcher.ServerPageTimeLimit; } set { directorySearcher.ServerPageTimeLimit = value; } }

		public virtual TimeSpan ServerTimeLimit { get { return directorySearcher.ServerTimeLimit; } set { directorySearcher.ServerTimeLimit = value; } }

		public virtual ISite Site { get { return directorySearcher.Site; } set { directorySearcher.Site = value; } }

		public virtual int SizeLimit { get { return directorySearcher.SizeLimit; } set { directorySearcher.SizeLimit = value; } }

		public virtual SortOption Sort { get { return directorySearcher.Sort; } set { directorySearcher.Sort = value; } }

		public virtual bool Tombstone { get { return directorySearcher.Tombstone; } set { directorySearcher.Tombstone = value; } }

		public virtual DirectoryVirtualListView VirtualListView { get { return directorySearcher.VirtualListView; } set { directorySearcher.VirtualListView = value; } }
	}
}