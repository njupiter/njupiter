#region Copyright & License
/*
	Copyright (c) 2005-2011 nJupiter

	Permission is hereby granted, free of charge, to any person obtaining a copy
	of this software and associated documentation files (the "Software"), to deal
	in the Software without restriction, including without limitation the rights
	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	copies of the Software, and to permit persons to whom the Software is
	furnished to do so, subject to the following conditions:

	The above copyright notice and this permission notice shall be included in
	all copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
	THE SOFTWARE.
*/
#endregion

using System;

using nJupiter.DataAccess.Ldap.Configuration;
using nJupiter.DataAccess.Ldap.DirectoryServices.Abstraction;
using nJupiter.DataAccess.Ldap.DistinguishedNames;

namespace nJupiter.DataAccess.Ldap.DirectoryServices {
	
	internal class DirectoryEntryAdapter : IDirectoryEntryAdapter {

		private readonly IServerConfig serverConfig;
		private readonly IFilterBuilder filterBuilder;
		private readonly INameParser nameParser;
		private readonly IDirectoryEntryFactory directoryEntryFactory;

		public DirectoryEntryAdapter(	IServerConfig serverConfig,
										IDirectoryEntryFactory directoryEntryFactory,
										IFilterBuilder filterBuilder,
										INameParser nameParser) {
			this.serverConfig = serverConfig;
			this.directoryEntryFactory = directoryEntryFactory;
			this.filterBuilder = filterBuilder;
			this.nameParser = nameParser;
		}

		public IDirectoryEntry GetEntry(string path) {
			return GetEntry(path, serverConfig.Username, serverConfig.Password);
		}

		public IDirectoryEntry GetEntry(Uri uri, string username, string password) {
			var path = LdapPathHandler.UriToPath(uri);
			return GetEntry(path, username, password);
		}

		public IDirectoryEntry GetEntry(string attribute, string attributeValue, string path, string defaultFilter, Func<IEntry, IDirectorySearcher> searcherFactory) {
			IDirectoryEntry directoryEntry = null;
			if(attributeValue == null) {
				return null;
			}

			var dn = nameParser.GetDnObject(attributeValue);
			var dnValue = dn != null;
			if(dnValue && dn.Rdns.Count > 1) {
				var uri = new Uri(new Uri(path), dn.ToString());
				return GetEntry(uri, serverConfig.Username, serverConfig.Password);
			}

			var entry = GetEntry(path);
			if(entry.IsBound()) {
				var directorySearcher = searcherFactory(entry);
				directorySearcher.Filter = CreateFilter(dnValue, attribute, attributeValue, defaultFilter);
				directoryEntry = GetEntry(directorySearcher);
			}
			return directoryEntry;
		}

		private string CreateFilter(bool dnValue, string attribute, string attributeValue, string defaultFilter) {
			if(dnValue) {
				return filterBuilder.AttachRdnFilter(attributeValue, defaultFilter);
			}
			return filterBuilder.AttachFilter(attribute, attributeValue, defaultFilter);
		}

		private IDirectoryEntry GetEntry(IDirectorySearcher directorySearcher) {
			var result = directorySearcher.FindOne();
			return result != null ? result.GetDirectoryEntry() : null;			
		}

		private IDirectoryEntry GetEntry(string path, string username, string password) {
			return directoryEntryFactory.Create(path, username, password, serverConfig.AuthenticationTypes);
		}
	}
}
