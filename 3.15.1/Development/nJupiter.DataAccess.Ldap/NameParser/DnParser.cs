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

namespace nJupiter.DataAccess.Ldap.NameParser {
	internal static class DnParser {
		
		public static string GetCn(string name) {
			Dn dn = GetDnObject(name);
			if(dn == null) {
				return name;
			}
			return dn.Rdns[0].Components[0].ComponentValue;
		}

		public static string GetRdn(string name) {
			Dn dn = GetDnObject(name);
			if(dn == null) {
				return null;
			}
			return dn.Rdns[0].ToString();
		}

		public static string GetDn(string name) {
			Dn dn = GetDnObject(name);
			if(dn == null) {
				return null;
			}
			return dn.ToString();
		}

		public static string GetDn(string name, string attribute, string basePath) {
			Dn dn = GetDnObject(name);
			NameType type = GetNameType(dn);
			switch(type) {
				case NameType.Cn:
					dn = GetDnObject(String.Format("{0}={1},{2}", attribute, name, basePath));
					break;
				case NameType.Rdn:
					dn = GetDnObject(String.Format("{0},{1}", name, basePath));
					break;
				default:
					break;
			}
			return dn.ToString();
		}

		public static Dn GetDnObject(string name) {
			name = LdapPathHandler.GetDistinguishedNameFromPath(name);
			if(name.Contains("=")) {
				return new Dn(name);
			}
			return null;
		}

		private static NameType GetNameType(Dn dn) {
			if(dn == null) {
				return NameType.Cn;
			}
			return dn.Rdns.Count > 1 ? NameType.Dn : NameType.Rdn;
		}
	}
}
