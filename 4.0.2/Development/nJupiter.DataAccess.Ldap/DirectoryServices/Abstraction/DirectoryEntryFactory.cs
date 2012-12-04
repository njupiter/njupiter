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

using System.DirectoryServices;

namespace nJupiter.DataAccess.Ldap.DirectoryServices.Abstraction {
	public class DirectoryEntryFactory : IDirectoryEntryFactory {

		private const string LdapScheme = "LDAP://";
		private const string LdapSchemeLoweCase = "ldap://";
		private const string IncorrectLdapScheme = "LDAP:///";

		public virtual IDirectoryEntry Create(string path, string username, string password, AuthenticationTypes authenticationTypes) {
			var entry = new DirectoryEntry(CorrectingScheme(path), username, password, authenticationTypes);
			return new DirectoryEntryWrapper(entry);
		}

		private static string CorrectingScheme(string path) {
			if(path.StartsWith(LdapSchemeLoweCase)) {
				// DirectoryEntry object needs ldap scheme in upper case bacause the path is case sensitive
				// http://msdn.microsoft.com/en-us/library/system.directoryservices.directoryentry.path%28VS.71%29.aspx
				path = path.Replace(LdapSchemeLoweCase, LdapScheme);
			}
			if(path.StartsWith(IncorrectLdapScheme)) {
			    // It seems like System.Uri can not handle serverless LDAP-paths, and return an invalid
			    // url with three slashes in the URI-scheme. So therefor we remove one slash if that happens.
			    path = path.Replace(IncorrectLdapScheme, LdapScheme);
			}
			return path;
		}
	}
}