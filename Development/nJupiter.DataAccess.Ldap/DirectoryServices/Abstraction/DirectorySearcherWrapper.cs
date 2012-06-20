using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.DirectoryServices;
using System.Runtime.Remoting;

namespace nJupiter.DataAccess.Ldap.DirectoryServices.Abstraction {
	internal class DirectorySearcherWrapper {
		readonly DirectorySearcher directorySearcher;

		public DirectorySearcherWrapper(IEntry directoryEntry) {
			directorySearcher = new DirectorySearcher(directoryEntry.UnWrap());
		}

		public bool Asynchronous { get { return directorySearcher.Asynchronous; }
			set { directorySearcher.Asynchronous = value; } }

		public string AttributeScopeQuery { get { return directorySearcher.AttributeScopeQuery; }
			set { directorySearcher.AttributeScopeQuery = value; } }

		public bool CacheResults { get { return directorySearcher.CacheResults; }
			set { directorySearcher.CacheResults = value; } }

		public TimeSpan ClientTimeout { get { return directorySearcher.ClientTimeout; }
			set { directorySearcher.ClientTimeout = value; } }

		public IContainer Container {
			get { return directorySearcher.Container; } }

		public ObjRef CreateObjRef(Type requestedType) {
			return directorySearcher.CreateObjRef(requestedType);
		}

		public DereferenceAlias DerefAlias { get { return directorySearcher.DerefAlias; }
			set { directorySearcher.DerefAlias = value; } }

		public DirectorySynchronization DirectorySynchronization { get { return directorySearcher.DirectorySynchronization; }
			set { directorySearcher.DirectorySynchronization = value; } }

		public void Dispose() {
			directorySearcher.Dispose();
		}

		public event EventHandler Disposed { add { directorySearcher.Disposed += value; }
			remove { directorySearcher.Disposed -= value; } }

		public ExtendedDN ExtendedDN { get { return directorySearcher.ExtendedDN; }
			set { directorySearcher.ExtendedDN = value; } }

		public string Filter { get { return directorySearcher.Filter; }
			set { directorySearcher.Filter = value; } }

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

		public int PageSize { get { return directorySearcher.PageSize; }
			set { directorySearcher.PageSize = value; } }

		public StringCollection PropertiesToLoad {
			get { return directorySearcher.PropertiesToLoad; } }

		public bool PropertyNamesOnly { get { return directorySearcher.PropertyNamesOnly; }
			set { directorySearcher.PropertyNamesOnly = value; } }

		public ReferralChasingOption ReferralChasing { get { return directorySearcher.ReferralChasing; }
			set { directorySearcher.ReferralChasing = value; } }

		public IEntry SearchRoot { get { return directorySearcher.SearchRoot.Wrap(); }
			set { directorySearcher.SearchRoot = value.UnWrap(); } }

		public SearchScope SearchScope { get { return directorySearcher.SearchScope; }
			set { directorySearcher.SearchScope = value; } }

		public SecurityMasks SecurityMasks { get { return directorySearcher.SecurityMasks; }
			set { directorySearcher.SecurityMasks = value; } }

		public TimeSpan ServerPageTimeLimit { get { return directorySearcher.ServerPageTimeLimit; }
			set { directorySearcher.ServerPageTimeLimit = value; } }

		public TimeSpan ServerTimeLimit { get { return directorySearcher.ServerTimeLimit; }
			set { directorySearcher.ServerTimeLimit = value; } }

		public ISite Site { get { return directorySearcher.Site; }
			set { directorySearcher.Site = value; } }

		public int SizeLimit { get { return directorySearcher.SizeLimit; }
			set { directorySearcher.SizeLimit = value; } }

		public SortOption Sort { get { return directorySearcher.Sort; }
			set { directorySearcher.Sort = value; } }

		public bool Tombstone { get { return directorySearcher.Tombstone; }
			set { directorySearcher.Tombstone = value; } }

		public DirectoryVirtualListView VirtualListView { get { return directorySearcher.VirtualListView; }
			set { directorySearcher.VirtualListView = value; } }
	}
}