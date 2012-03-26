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
using System.DirectoryServices;

using nJupiter.DataAccess.Ldap.NameParser;

namespace nJupiter.DataAccess.Ldap {
	internal class DirectoryEntryAdapter {

		private readonly Configuration config;
		private readonly Searcher userSearcher;
		private readonly Searcher groupSearcher;
		private readonly FilterBuilder filterBuilder;

		public static DirectoryEntryAdapter GetInstance(Configuration config, Searcher userSearcher, Searcher groupSearcher, FilterBuilder filterBuilder) {
			if(config == null) {
				throw new ArgumentNullException("config");
			}
			if(userSearcher == null) {
				throw new ArgumentNullException("userSearcher");
			}
			if(groupSearcher == null) {
				throw new ArgumentNullException("groupSearcher");
			}
			if(filterBuilder == null) {
				throw new ArgumentNullException("filterBuilder");
			}
			return new DirectoryEntryAdapter(config, userSearcher, groupSearcher, filterBuilder);
		}

		private DirectoryEntryAdapter(Configuration config, Searcher userSearcher, Searcher groupSearcher, FilterBuilder filterBuilder) {
			this.config = config;
			this.filterBuilder = filterBuilder;
			this.userSearcher = userSearcher;
			this.groupSearcher = groupSearcher;
		}

		public DirectoryEntry GetUserEntry(string username) {
			string userFilter = filterBuilder.CreateUserFilter();
			return GetEntry(config.Users.RdnAttribute, username, config.Users.Path, userFilter, userSearcher);
		}

		public DirectoryEntry GetUsersEntry() {
			return GetEntry(config.Users.Path);
		}

		public DirectoryEntry GetGroupEntry(string groupname) {
			string groupFilter = filterBuilder.CreateGroupFilter();
			return GetEntry(config.Groups.RdnAttribute, groupname, config.Groups.Path, groupFilter, groupSearcher);
		}

		public DirectoryEntry GetGroupsEntry() {
			return GetEntry(config.Groups.Path);
		}

		private DirectoryEntry GetEntry(string path) {
			return GetEntry(path, config.Server.Username, config.Server.Password);
		}

		private DirectoryEntry GetEntry(Uri uri) {
			return GetEntry(uri, config.Server.Username, config.Server.Password);
		}

		public DirectoryEntry GetEntry(Uri uri, string username, string password) {
			string path = LdapPathHandler.UriToPath(uri);
			return GetEntry(path, username, password);
		}

		private DirectoryEntry GetEntry(string path, string username, string password) {
			return new DirectoryEntry(path, username, password, config.Server.AuthenticationTypes);
		}

		private DirectoryEntry GetEntry(string attribute, string attributeValue, string path, string defaultFilter, Searcher searcher) {
			DirectorySearcher directorySearcher;
			DirectoryEntry directoryEntry = null;

			Dn dn = DnParser.GetDnObject(attributeValue);
			if(dn != null && dn.Rdns.Count > 1) {
				Uri uri = new Uri(config.Server.Url, dn.ToString());
				return GetEntry(uri);
			}

			DirectoryEntry entry = GetEntry(path);
			if(IsBound(entry)) {
				directorySearcher = searcher.Create(entry);
				if(dn != null) {
					directorySearcher.Filter = filterBuilder.AttachRdnFilter(attributeValue, defaultFilter);
				} else {
					directorySearcher.Filter = filterBuilder.AttachFilter(attribute, attributeValue, defaultFilter);
				}

				foreach(SearchResult result in directorySearcher.FindAll()) {
					if(directoryEntry != null) {
						throw new ProviderException(String.Format("More than one entry with value {0} for attribute {1} was found.", attributeValue, attribute));
					}
					directoryEntry = result.GetDirectoryEntry();
				}
			}
			return directoryEntry;
		}

		public static bool IsBound(DirectoryEntry entry) {
			return entry != null ? entry.NativeObject != null : false;
		}
	}
}
