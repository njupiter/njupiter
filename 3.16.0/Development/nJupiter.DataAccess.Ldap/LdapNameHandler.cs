#region Copyright & License
/*
	Copyright (c) 2005-2010 nJupiter

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
using System.DirectoryServices;

using nJupiter.DataAccess.Ldap.NameParser;

namespace nJupiter.DataAccess.Ldap {
	internal class LdapNameHandler {

		private readonly Configuration config;

		public static LdapNameHandler GetInstance(Configuration config) {
			if(config == null) {
				throw new ArgumentNullException("config");
			}

			return new LdapNameHandler(config);
		}

		private LdapNameHandler(Configuration config) {
			this.config = config;
		}

		public string GetUserNameFromEntry(DirectoryEntry entry) {
			return GetNameFromEntry(config.Users.NameType, entry);
		}

		public string GetUserName(string entryName) {
			return GetName(config.Users.NameType, entryName);
		}

		public string GetGroupNameFromEntry(DirectoryEntry entry) {
			return GetNameFromEntry(config.Groups.NameType, entry);
		}

		public string GetGroupName(string entryName) {
			return GetName(config.Groups.NameType, entryName);
		}

		private static string GetNameFromEntry(NameType nameType, DirectoryEntry entry) {
			return GetName(nameType, entry.Name);
		}

		private static string GetName(NameType nameType, string entryName) {
			switch(nameType) {
				case NameType.Cn:
				return DnParser.GetCn(entryName);

				case NameType.Rdn:
				return DnParser.GetRdn(entryName);

				default:
				return DnParser.GetDn(entryName);
			}
		}

		public bool GroupsEqual(string name, string nameToMatch) {
			return NamesEqual(name, nameToMatch, this.config.Groups.RdnAttribute, this.config.Groups.Base);
		}

		private static bool NamesEqual(string name, string nameToMatch, string attribute, string basePath) {
			name = DnParser.GetDn(name, attribute, basePath);
			nameToMatch = DnParser.GetDn(nameToMatch, attribute, basePath);
			return string.Equals(name, nameToMatch, StringComparison.InvariantCultureIgnoreCase);
		}
	}
}
