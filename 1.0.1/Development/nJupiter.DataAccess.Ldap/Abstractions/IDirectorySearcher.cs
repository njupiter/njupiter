using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.DirectoryServices;
using System.Runtime.Remoting;

namespace nJupiter.DataAccess.Ldap.Abstractions {
	internal interface IDirectorySearcher {
		bool Asynchronous { get; set; }
		string AttributeScopeQuery { get; set; }
		bool CacheResults { get; set; }
		TimeSpan ClientTimeout { get; set; }
		IContainer Container { get; }
		DereferenceAlias DerefAlias { get; set; }
		DirectorySynchronization DirectorySynchronization { get; set; }
		ExtendedDN ExtendedDN { get; set; }
		string Filter { get; set; }
		int PageSize { get; set; }
		StringCollection PropertiesToLoad { get; }
		bool PropertyNamesOnly { get; set; }
		ReferralChasingOption ReferralChasing { get; set; }
		IDirectoryEntry SearchRoot { get; set; }
		SearchScope SearchScope { get; set; }
		SecurityMasks SecurityMasks { get; set; }
		TimeSpan ServerPageTimeLimit { get; set; }
		TimeSpan ServerTimeLimit { get; set; }
		ISite Site { get; set; }
		int SizeLimit { get; set; }
		SortOption Sort { get; set; }
		bool Tombstone { get; set; }
		DirectoryVirtualListView VirtualListView { get; set; }
		ObjRef CreateObjRef(Type requestedType);
		void Dispose();
		event EventHandler Disposed;
		IEnumerable<ISearchResult> FindAll();
		ISearchResult FindOne();
		object GetLifetimeService();
		object InitializeLifetimeService();
	}
}