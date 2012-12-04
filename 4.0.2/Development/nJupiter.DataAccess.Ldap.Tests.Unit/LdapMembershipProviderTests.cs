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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.Security;

using FakeItEasy;

using NUnit.Framework;

using nJupiter.Abstraction.Logging;
using nJupiter.DataAccess.Ldap.Configuration;
using nJupiter.DataAccess.Ldap.DirectoryServices;

namespace nJupiter.DataAccess.Ldap.Tests.Unit {

	[TestFixture]
	public class LdapMembershipProviderTests {

		LdapMembershipProvider provider;
		IProviderConfig providerConfig;
		IContainer container;
		ILog log;

		[SetUp]
		public void SetUp() {
			providerConfig = A.Fake<IProviderConfig>();
			container = providerConfig.LdapConfig.Container;
			log = A.Fake<ILog>();
			A.CallTo(() => container.LogManager.GetLogger(typeof(LdapMembershipProvider))).Returns(log);
			InitializeProvider();
		}


		[Test]
		public void ValidateUser_GetUserFromEntryAdapterThatContainRdnAttribute_ReturnsTrue() {
			var entry = A.Fake<IEntry>();
			A.CallTo(() => providerConfig.LdapConfig.Users.RdnAttribute).Returns("rdnattribute");
			A.CallTo(() => container.UserEntryAdapter.GetUserEntry("username", "correctpassword")).Returns(entry);

			var properties = new Dictionary<string, IEnumerable>();
			properties.Add("rdnattribute", A.CollectionOfFake<object>(1));
			
			A.CallTo(() => entry.Properties).Returns(properties);

			var result = provider.ValidateUser("username", "correctpassword");

			Assert.IsTrue(result);
		}


		[Test]
		public void ValidateUser_GetUserFromEntryAdapterThatThrowsExceptionOnGetUserEntry_ReturnsFalse() {
			A.CallTo(() => container.UserEntryAdapter.GetUserEntry(A<string>.Ignored, A<string>.Ignored)).Throws(new Exception());

			var result = provider.ValidateUser("username", "incorrectpassword");

			Assert.IsFalse(result);
		}


		[Test]
		public void ValidateUser_GetUserFromEntryAdapterThatDoesNotContainRdnAttribute_ReturnsFalse() {
			var entry = A.Fake<IEntry>();
			A.CallTo(() => container.UserEntryAdapter.GetUserEntry(A<string>.Ignored, A<string>.Ignored)).Returns(entry);

			var properties = new Dictionary<string, IEnumerable>();
			
			A.CallTo(() => entry.Properties).Returns(properties);

			var result = provider.ValidateUser("username", "incorrectpassword");

			Assert.IsFalse(result);
		}

		[Test]
		public void ValidateUser_GetUserFromEntryAdapterThatThrowsExceptionOnReadingRdnAttribute_ReturnsFalse() {
			var entry = A.Fake<IEntry>();
			A.CallTo(() => container.UserEntryAdapter.GetUserEntry(A<string>.Ignored, A<string>.Ignored)).Returns(entry);

			A.CallTo(() => entry.Properties).Throws(new Exception());

			var result = provider.ValidateUser("username", "incorrectpassword");

			Assert.IsFalse(result);
		}

		[Test]
		public void ValidateUser_GetUserFromEntryAdapterThatThrowsExceptionOnGetUserEntry_ExceptionIsLogged() {
			A.CallTo(() => log.IsDebugEnabled).Returns(true);
			var exception = new Exception();
			A.CallTo(() => container.UserEntryAdapter.GetUserEntry(A<string>.Ignored, A<string>.Ignored)).Throws(exception);

			provider.ValidateUser("username", "incorrectpassword");

			A.CallTo(() => log.Debug(A<object>.Ignored, exception)).MustHaveHappened(Repeated.Exactly.Once);

		}

		[Test]
		public void GetUser_UserEntryAdapterReturnsEntryForUserName_ReturnsMembershipUsersCreatedFromEntryByMembershipUserFactory() {
			var entry = A.Fake<IEntry>();
			A.CallTo(() => container.UserEntryAdapter.GetUserEntryAndLoadProperties("username")).Returns(entry);
			var user = new TestableMembershipUser();
			A.CallTo(() => providerConfig.MembershipUserFactory.Create(entry)).Returns(user);

			var result = provider.GetUser("username", false);

			Assert.AreSame(user, result);
		}


		[Test]
		public void GetUser_SendInUserNameAsObjectWhereUserEntryAdapterReturnsEntryForUserName_ReturnsMembershipUsersCreatedFromEntryByMembershipUserFactory() {
			var entry = A.Fake<IEntry>();
			A.CallTo(() => container.UserEntryAdapter.GetUserEntryAndLoadProperties("username")).Returns(entry);
			var user = new TestableMembershipUser();
			A.CallTo(() => providerConfig.MembershipUserFactory.Create(entry)).Returns(user);

			var result = provider.GetUser((object)"username", false);

			Assert.AreSame(user, result);
		}

		[Test]
		public void GetUserNameByEmail_GetUserEntryFromAdapterAndCreateMembershipUserFromEntry_RetrunsUserNameFromMemberhsipUser() {
			var entry = A.Fake<IEntry>();
			A.CallTo(() => container.UserEntryAdapter.GetUserEntryByEmail("email")).Returns(entry);
			var user = new TestableMembershipUser("username");
			A.CallTo(() => providerConfig.MembershipUserFactory.Create(entry)).Returns(user);

			var result = provider.GetUserNameByEmail("email");

			Assert.AreEqual("username", result);
		}

		[Test]
		public void GetUserNameByEmail_GetUserEntryFromAdapterButMembershipUserFactoryReturnsNull_RetrunsNull() {
			var entry = A.Fake<IEntry>();
			A.CallTo(() => container.UserEntryAdapter.GetUserEntryByEmail("email")).Returns(entry);
			A.CallTo(() => providerConfig.MembershipUserFactory.Create(entry)).Returns(null);

			provider.GetUserNameByEmail("email");

			Assert.IsNull(null);
		}

		[Test]
		public void GetAllUsers_GetAllUserEntriesFromAdapterAndCreateMembershipUsers_ReturnsMembershipUsersCreatedByMembershipUserFactory() {
			int totalRecords;
			var entries = A.Fake<IEntryCollection>();
			A.CallTo(() => container.UserEntryAdapter.GetAllUserEntries(1,1, out totalRecords)).Returns(entries);
			var members = new MembershipUserCollection();
			A.CallTo(() => providerConfig.MembershipUserFactory.CreateCollection(entries)).Returns(members);

			var result = provider.GetAllUsers(1,1, out totalRecords);

			Assert.AreSame(members, result);
		}

		[Test]
		public void FindUsersByName_GetUserEntriesFromAdapterAndCreateMembershipUsers_ReturnsMembershipUsersCreatedByMembershipUserFactory() {
			int totalRecords;
			var entries = A.Fake<IEntryCollection>();
			A.CallTo(() => container.UserEntryAdapter.FindUsersByName("username", 1,1, out totalRecords)).Returns(entries);
			var members = new MembershipUserCollection();
			A.CallTo(() => providerConfig.MembershipUserFactory.CreateCollection(entries)).Returns(members);

			var result = provider.FindUsersByName("username", 1,1, out totalRecords);

			Assert.AreSame(members, result);
		}

		[Test]
		public void FindUsersByEmail_GetUserEntriesFromAdapterAndCreateMembershipUsers_ReturnsMembershipUsersCreatedByMembershipUserFactory() {
			int totalRecords;
			var entries = A.Fake<IEntryCollection>();
			A.CallTo(() => container.UserEntryAdapter.FindUsersByEmail("email", 1,1, out totalRecords)).Returns(entries);
			var members = new MembershipUserCollection();
			A.CallTo(() => providerConfig.MembershipUserFactory.CreateCollection(entries)).Returns(members);

			var result = provider.FindUsersByEmail("email", 1,1, out totalRecords);

			Assert.AreSame(members, result);
		}

		[Test]
		public void ChangePassword_NotImplemented_ThrowsNotSupportedException() {
			Assert.Throws<NotSupportedException>(() => provider.ChangePassword("username", "oldpassword", "newpassword"));
		}

		[Test]
		public void UnlockUser_NotImplemented_ThrowsNotSupportedException() {
			Assert.Throws<NotSupportedException>(() => provider.UnlockUser("username"));
		}

		[Test]
		public void UpdateUser_NotImplemented_ThrowsNotSupportedException() {
			Assert.Throws<NotSupportedException>(() => provider.UpdateUser(null));
		}

		[Test]
		public void CreateUser_NotImplemented_ThrowsNotSupportedException() {
			MembershipCreateStatus status;
			Assert.Throws<NotSupportedException>(() => provider.CreateUser("username", "password", "email", "passwordQuestion", "passwordAnswer", true, null, out status));
		}

		[Test]
		public void ChangePasswordQuestionAndAnswer_NotImplemented_ThrowsNotSupportedException() {
			Assert.Throws<NotSupportedException>(() => provider.ChangePasswordQuestionAndAnswer("username", "password", "newpasswordquestion", "newpasswordanswer"));
		}

		[Test]
		public void GetPassword_NotImplemented_ThrowsNotSupportedException() {
			Assert.Throws<NotSupportedException>(() => provider.GetPassword("username", "answer"));
		}

		[Test]
		public void ResetPassword_NotImplemented_ThrowsNotSupportedException() {
			Assert.Throws<NotSupportedException>(() => provider.ResetPassword("username", "answer"));
		}

		[Test]
		public void DeleteUser_NotImplemented_ThrowsNotSupportedException() {
			Assert.Throws<NotSupportedException>(() => provider.DeleteUser("username", true));
		}

		[Test]
		public void GetNumberOfUsersOnline_NotImplemented_ThrowsNotSupportedException() {
			Assert.Throws<NotSupportedException>(() => provider.GetNumberOfUsersOnline());
		}

		[Test]
		public void Name_ProviderConfigReturnsProviderName_ReturnsNameFromProvider() {
			A.CallTo(() => providerConfig.Name).Returns("ProviderName");

			Assert.AreEqual("ProviderName", provider.Name);
		}

		[Test]
		public void ApplicationName_ProviderConfigReturnsApplicationName_ReturnsNameFromProvider() {
			A.CallTo(() => providerConfig.ApplicationName).Returns("ApplicationName");

			Assert.AreEqual("ApplicationName", provider.ApplicationName);
		}

		private void InitializeProvider() {
			const string name = "ProviderName";
			var factory = A.Fake<IProviderConfigFactory>();
			var config = new NameValueCollection();
		
			A.CallTo(() => providerConfig.Name).Returns(name);
			A.CallTo(() => factory.Create<LdapMembershipProvider>(name, config)).Returns(providerConfig);

			provider = new LdapMembershipProvider(factory);
			provider.Initialize(name, config);
		}

		public class TestableMembershipUser : MembershipUser {
			private readonly string username;

			public TestableMembershipUser() {}

			public TestableMembershipUser(string username) {
				this.username = username;
			}

			public override string UserName {
				get {
					return username;
				}
			}
		}
	}
}