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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.DirectoryServices;
using System.Runtime.Remoting;

namespace nJupiter.DataAccess.Ldap.DirectoryServices.Abstraction {
	public interface IDirectorySearcher {
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
		IEntry SearchRoot { get; set; }
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
		object GetLifetimeService();
		object InitializeLifetimeService();
		IEnumerable<T> GetPropertiesByRangedFilter<T>(string propertyName);
		IEntryCollection FindAll();
		IEntry FindOne();
		IEntry FindOne(string propertyToLoad);
	}
}