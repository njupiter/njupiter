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

using NUnit.Framework;

namespace nJupiter.DataAccess.Ldap.Tests.Unit {
	
	[TestFixture]
	public class LdapPathHandlerTests {

		[Test]
		public void UriToPath_SendInUriWithLowerCaseLdapScheme_ReturnsPathWithUpperCaseLdapScheme() {
			var uri = new Uri("ldap://host/");
			var path = LdapPathHandler.UriToPath(uri);

			Assert.AreEqual("LDAP://host/", path);
		}

		[Test]
		public void UriToPath_SendInUriWithUpperCaseLdapScheme_ReturnsPathWithoutModifications() {
			var uri = new Uri("LDAP://host/");
			var path = LdapPathHandler.UriToPath(uri);

			Assert.AreEqual("LDAP://host/", path);
		}

		[Test]
		public void GetDistinguishedNameFromPath_SendInFullUrl_ReturnsDnFromPath() {
			const string fullUrl = "ldap://co.int:389/CN=user,OU=Test,DC=org";
			var path = LdapPathHandler.GetDistinguishedNameFromPath(fullUrl);

			Assert.AreEqual("CN=user,OU=Test,DC=org", path);
		}


		[Test]
		public void GetDistinguishedNameFromPath_SendJustPath_ReturnsDnFromPath() {
			const string fullUrl = "/CN=user,OU=Test,DC=org";
			var path = LdapPathHandler.GetDistinguishedNameFromPath(fullUrl);

			Assert.AreEqual("CN=user,OU=Test,DC=org", path);
		}

		[Test]
		public void GetDistinguishedNameFromPath_SendDn_ReturnsDn() {
			const string fullUrl = "CN=user,OU=Test,DC=org";
			var path = LdapPathHandler.GetDistinguishedNameFromPath(fullUrl);

			Assert.AreEqual("CN=user,OU=Test,DC=org", path);
		}
		 
	}
}