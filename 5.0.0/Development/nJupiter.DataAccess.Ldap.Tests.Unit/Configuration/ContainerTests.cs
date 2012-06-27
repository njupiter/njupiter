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

using FakeItEasy;

using NUnit.Framework;

using nJupiter.Abstraction.Logging;
using nJupiter.DataAccess.Ldap.Configuration;

namespace nJupiter.DataAccess.Ldap.Tests.Unit.Configuration {
	
	[TestFixture]
	public class ContainerTests {

		[Test]
		public void Constructor_CreateInstance_NameParserIsNotNull() {
			var ldapConfig = A.Fake<ILdapConfig>();

			var container = new Container(ldapConfig);

			Assert.IsNotNull(container.NameParser);
		}

		[Test]
		public void Constructor_CreateInstance_DirectoryEntryFactoryIsNotNull() {
			var ldapConfig = A.Fake<ILdapConfig>();

			var container = new Container(ldapConfig);

			Assert.IsNotNull(container.DirectoryEntryFactory);
		}

		[Test]
		public void Constructor_CreateInstance_SearcherFactoryIsNotNull() {
			var ldapConfig = A.Fake<ILdapConfig>();

			var container = new Container(ldapConfig);

			Assert.IsNotNull(container.SearcherFactory);
		}

		[Test]
		public void Constructor_CreateInstance_FilterBuilderIsNotNull() {
			var ldapConfig = A.Fake<ILdapConfig>();

			var container = new Container(ldapConfig);

			Assert.IsNotNull(container.FilterBuilder);
		}

		[Test]
		public void Constructor_CreateInstance_DirectoryEntryAdapterIsNotNull() {
			var ldapConfig = A.Fake<ILdapConfig>();

			var container = new Container(ldapConfig);

			Assert.IsNotNull(container.DirectoryEntryAdapter);
		}

		[Test]
		public void Constructor_CreateInstance_UserEntryAdapterIsNotNull() {
			var ldapConfig = A.Fake<ILdapConfig>();

			var container = new Container(ldapConfig);

			Assert.IsNotNull(container.UserEntryAdapter);
		}


		[Test]
		public void Constructor_CreateInstance_GroupEntryAdapterIsNotNull() {
			var ldapConfig = A.Fake<ILdapConfig>();

			var container = new Container(ldapConfig);

			Assert.IsNotNull(container.GroupEntryAdapter);
		}

		[Test]
		public void Constructor_CreateInstance_LogManagerIsNotNull() {
			var ldapConfig = A.Fake<ILdapConfig>();

			var container = new Container(ldapConfig);

			Assert.IsNotNull(container.LogManager);
		}

		[Test]
		public void Constructor_CreateInstance_LogManagerIsSameAsFromFactory() {
			var ldapConfig = A.Fake<ILdapConfig>();

			var container = new Container(ldapConfig);

			Assert.AreSame(LogManagerFactory.GetLogManager(), container.LogManager);
		}
	}
}