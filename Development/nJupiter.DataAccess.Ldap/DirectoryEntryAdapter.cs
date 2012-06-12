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
using System.Configuration.Provider;

using nJupiter.DataAccess.Ldap.Abstractions;
using nJupiter.DataAccess.Ldap.Configuration;
using nJupiter.DataAccess.Ldap.NameParser;

namespace nJupiter.DataAccess.Ldap {
	internal class DirectoryEntryAdapter : IDirectoryEntryAdapter {

		private readonly ILdapConfig config;
		private readonly ISearcher userSearcher;
		private readonly ISearcher groupSearcher;
		private readonly IFilterBuilder filterBuilder;
		private readonly IDnParser dnParser;
		private readonly IDirectoryEntryFactory directoryEntryFactory;

		public DirectoryEntryAdapter(ILdapConfig config) {
			this.config = config;
			directoryEntryFactory = config.Container.DirectoryEntryFactory;
			filterBuilder = config.Container.FilterBuilder;
			dnParser = config.Container.DnParser;
			userSearcher = config.Container.UserSearcher;
			groupSearcher = config.Container.GroupSearcher;
		}

		public IDirectoryEntry GetUserEntry(string username) {
			var userFilter = filterBuilder.CreateUserFilter();
			return GetEntry(config.Users.RdnAttribute, username, config.Users.Path, userFilter, userSearcher);
		}

		public IDirectoryEntry GetUsersEntry() {
			return GetEntry(config.Users.Path);
		}

		public IDirectoryEntry GetGroupEntry(string groupname) {
			var groupFilter = filterBuilder.CreateGroupFilter();
			return GetEntry(config.Groups.RdnAttribute, groupname, config.Groups.Path, groupFilter, groupSearcher);
		}

		public IDirectoryEntry GetGroupsEntry() {
			return GetEntry(config.Groups.Path);
		}

		private IDirectoryEntry GetEntry(string path) {
			return GetEntry(path, config.Server.Username, config.Server.Password);
		}

		private IDirectoryEntry GetEntry(Uri uri) {
			return GetEntry(uri, config.Server.Username, config.Server.Password);
		}

		public IDirectoryEntry GetEntry(Uri uri, string username, string password) {
			var path = LdapPathHandler.UriToPath(uri);
			return GetEntry(path, username, password);
		}

		private IDirectoryEntry GetEntry(string path, string username, string password) {
			return directoryEntryFactory.Create(path, username, password, config.Server.AuthenticationTypes);
		}

		private IDirectoryEntry GetEntry(string attribute, string attributeValue, string path, string defaultFilter, ISearcher searcher) {
			IDirectoryEntry directoryEntry = null;

			var dn = dnParser.GetDnObject(attributeValue)	;
			if(dn != null && dn.Rdns.Count > 1) {
				var uri = new Uri(config.Server.Url, dn.ToString());
				return GetEntry(uri);
			}

			var entry = GetEntry(path);
			if(entry.IsBound()) {
				var directorySearcher = searcher.Create(entry);
				if(dn != null) {
					directorySearcher.Filter = filterBuilder.AttachRdnFilter(attributeValue, defaultFilter);
				} else {
					directorySearcher.Filter = filterBuilder.AttachFilter(attribute, attributeValue, defaultFilter);
				}

				foreach(var result in directorySearcher.FindAll()) {
					if(directoryEntry != null) {
						throw new ProviderException(String.Format("More than one entry with value {0} for attribute {1} was found.", attributeValue, attribute));
					}
					directoryEntry = result.GetDirectoryEntry();
				}
			}
			return directoryEntry;
		}

	}
}
