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

using nJupiter.DataAccess.Ldap.Abstractions;
using nJupiter.DataAccess.Ldap.Configuration;
using nJupiter.DataAccess.Ldap.NameParser;

namespace nJupiter.DataAccess.Ldap {
	internal class LdapNameHandler : ILdapNameHandler {

		private readonly ILdapConfig config;
		private readonly IDnParser dnParser;

		public LdapNameHandler(ILdapConfig config) {
			this.config = config;
			dnParser = config.Container.DnParser;
		}

		public string GetUserNameFromEntry(IDirectoryEntry entry) {
			return GetNameFromEntry(config.Users.NameType, entry);
		}

		public string GetUserName(string entryName) {
			return GetName(config.Users.NameType, entryName);
		}

		public string GetGroupNameFromEntry(IDirectoryEntry entry) {
			return GetNameFromEntry(config.Groups.NameType, entry);
		}

		public string GetGroupName(string entryName) {
			return GetName(config.Groups.NameType, entryName);
		}

		private string GetNameFromEntry(NameType nameType, IDirectoryEntry entry) {
			return GetName(nameType, entry.Name);
		}

		private string GetName(NameType nameType, string entryName) {
			switch(nameType) {
				case NameType.Cn:
				return dnParser.GetCn(entryName);

				case NameType.Rdn:
				return dnParser.GetRdn(entryName);

				default:
				return dnParser.GetDn(entryName);
			}
		}

		public bool GroupsEqual(string name, string nameToMatch) {
			return NamesEqual(name, nameToMatch, config.Groups.RdnAttribute, config.Groups.Base);
		}

		private bool NamesEqual(string name, string nameToMatch, string attribute, string basePath) {
			name = dnParser.GetDn(name, attribute, basePath);
			nameToMatch = dnParser.GetDn(nameToMatch, attribute, basePath);
			return string.Equals(name, nameToMatch, StringComparison.InvariantCultureIgnoreCase);
		}
	}
}
