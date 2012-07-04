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
using System.Linq;

namespace nJupiter.DataAccess.Ldap.DistinguishedNames {
	public class NameParser : INameParser {

		public virtual string GetCn(string name) {
			var dn = GetDnObject(name);
			if(dn == null) {
				return name;
			}
			return dn.Rdns.First().Components.First().ComponentValue;
		}

		public virtual string GetRdn(string name) {
			var dn = GetDnObject(name);
			if(dn == null) {
				return null;
			}
			return dn.Rdns.First().ToString();
		}

		public virtual string GetDn(string name) {
			var dn = GetDnObject(name);
			if(dn == null) {
				return null;
			}
			return dn.ToString();
		}

		public virtual bool RdnInName(string name, string attribute, string basePath) {
			var dn = GetDnObject(name, attribute, basePath);
			var cnType = dn.Rdns.First().Components.First().ComponentType;
			return string.Equals(attribute, cnType, StringComparison.InvariantCultureIgnoreCase);
		}

		public virtual string GetDn(string name, string attribute, string basePath) {
			var dn = GetDnObject(name, attribute, basePath);
			return dn.ToString();
		}

		private IDn GetDnObject(string name, string attribute, string basePath) {
			var dn = GetDnObject(name);
			var type = GetNameType(dn);
			switch(type) {
				case NameType.Cn:
				dn = GetDnObject(String.Format("{0}={1},{2}", attribute, name, basePath));
				break;
				case NameType.Rdn:
				dn = GetDnObject(String.Format("{0},{1}", name, basePath));
				break;
			}
			return dn;
		}

		public virtual IDn GetDnObject(string name) {
			name = LdapPathHandler.GetDistinguishedNameFromPath(name);
			if(name.Contains("=")) {
				return CreateDnObject(name);
			}
			return null;
		}

		public virtual string GetName(NameType nameType, string entryName) {
			switch(nameType) {
				case NameType.Cn:
				return GetCn(entryName);

				case NameType.Rdn:
				return GetRdn(entryName);

				default:
				return GetDn(entryName);
			}
		}

		public virtual bool NamesEqual(string name, string nameToMatch, string attribute, string basePath) {
			name = GetDn(name, attribute, basePath);
			nameToMatch = GetDn(nameToMatch, attribute, basePath);
			return string.Equals(name, nameToMatch, StringComparison.InvariantCultureIgnoreCase);
		}

		protected virtual IDn CreateDnObject(string name) {
			return new Dn(name);
		}

		private NameType GetNameType(IDn dn) {
			if(dn == null) {
				return NameType.Cn;
			}
			return dn.Rdns.Count() > 1 ? NameType.Dn : NameType.Rdn;
		}
	}
}
