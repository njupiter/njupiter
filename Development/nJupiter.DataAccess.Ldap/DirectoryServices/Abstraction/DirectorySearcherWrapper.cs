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

		public bool Asynchronous { get { return directorySearcher.Asynchronous; } set { directorySearcher.Asynchronous = value; } }

		public string AttributeScopeQuery { get { return directorySearcher.AttributeScopeQuery; } set { directorySearcher.AttributeScopeQuery = value; } }

		public bool CacheResults { get { return directorySearcher.CacheResults; } set { directorySearcher.CacheResults = value; } }

		public TimeSpan ClientTimeout { get { return directorySearcher.ClientTimeout; } set { directorySearcher.ClientTimeout = value; } }

		public IContainer Container { get { return directorySearcher.Container; } }

		public ObjRef CreateObjRef(Type requestedType) {
			return directorySearcher.CreateObjRef(requestedType);
		}

		public DereferenceAlias DerefAlias { get { return directorySearcher.DerefAlias; } set { directorySearcher.DerefAlias = value; } }

		public DirectorySynchronization DirectorySynchronization { get { return directorySearcher.DirectorySynchronization; } set { directorySearcher.DirectorySynchronization = value; } }

		public void Dispose() {
			directorySearcher.Dispose();
		}

		public event EventHandler Disposed { add { directorySearcher.Disposed += value; } remove { directorySearcher.Disposed -= value; } }

		public ExtendedDN ExtendedDN { get { return directorySearcher.ExtendedDN; } set { directorySearcher.ExtendedDN = value; } }

		public string Filter { get { return directorySearcher.Filter; } set { directorySearcher.Filter = value; } }

		public IEntryCollection FindAll() {
			return directorySearcher.FindAll().Wrap();
		}

		public IEntry FindOne() {
			return directorySearcher.FindOne().Wrap();
		}

		public object GetLifetimeService() {
			return directorySearcher.GetLifetimeService();
		}

		public object InitializeLifetimeService() {
			return directorySearcher.InitializeLifetimeService();
		}

		public int PageSize { get { return directorySearcher.PageSize; } set { directorySearcher.PageSize = value; } }

		public StringCollection PropertiesToLoad { get { return directorySearcher.PropertiesToLoad; } }

		public bool PropertyNamesOnly { get { return directorySearcher.PropertyNamesOnly; } set { directorySearcher.PropertyNamesOnly = value; } }

		public ReferralChasingOption ReferralChasing { get { return directorySearcher.ReferralChasing; } set { directorySearcher.ReferralChasing = value; } }

		public IEntry SearchRoot { get { return directorySearcher.SearchRoot.Wrap(); } set { directorySearcher.SearchRoot = value.UnWrap(); } }

		public SearchScope SearchScope { get { return directorySearcher.SearchScope; } set { directorySearcher.SearchScope = value; } }

		public SecurityMasks SecurityMasks { get { return directorySearcher.SecurityMasks; } set { directorySearcher.SecurityMasks = value; } }

		public TimeSpan ServerPageTimeLimit { get { return directorySearcher.ServerPageTimeLimit; } set { directorySearcher.ServerPageTimeLimit = value; } }

		public TimeSpan ServerTimeLimit { get { return directorySearcher.ServerTimeLimit; } set { directorySearcher.ServerTimeLimit = value; } }

		public ISite Site { get { return directorySearcher.Site; } set { directorySearcher.Site = value; } }

		public int SizeLimit { get { return directorySearcher.SizeLimit; } set { directorySearcher.SizeLimit = value; } }

		public SortOption Sort { get { return directorySearcher.Sort; } set { directorySearcher.Sort = value; } }

		public bool Tombstone { get { return directorySearcher.Tombstone; } set { directorySearcher.Tombstone = value; } }

		public DirectoryVirtualListView VirtualListView { get { return directorySearcher.VirtualListView; } set { directorySearcher.VirtualListView = value; } }
	}
}