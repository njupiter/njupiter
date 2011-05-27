using System.Collections.Generic;

using FakeItEasy;

using nJupiter.DataAccess.Users;

using NUnit.Framework;

namespace nJupiter.Tests.DataAccess.Users {
	
	[TestFixture]
	public class UserRepositoryBaseTests {

		[Test]
		public void GetUserByUserName_CallMethod_CheckSoUnderlyingImplementationIsCalledWithDefaultDomain() {
			var innerProvider = A.Fake<IUserRepository>();
			var adapter = new UserRepositoryAdapter(innerProvider);
			adapter.GetUserByUserName("username");
			A.CallTo(() => innerProvider.GetUserByUserName("username", string.Empty)).MustHaveHappened(Repeated.Exactly.Once);
		}


		[Test]
		public void CreateUserInstance_CallMethod_CheckSoUnderlyingImplementationIsCalledWithDefaultDomain() {
			var innerProvider = A.Fake<IUserRepository>();
			var adapter = new UserRepositoryAdapter(innerProvider);
			adapter.CreateUserInstance("username");
			A.CallTo(() => innerProvider.CreateUserInstance("username", string.Empty)).MustHaveHappened(Repeated.Exactly.Once);
		}

		[Test]
		public void GetUsersBySearchCriteria_CallMethod_CheckSoUnderlyingImplementationIsCalled() {
			var innerProvider = A.Fake<IUserRepository>();
			var adapter = new UserRepositoryAdapter(innerProvider);
			var criteria = A.Fake<IEnumerable<SearchCriteria>>();
			adapter.GetUsersBySearchCriteria(criteria, true);
			A.CallTo(() => innerProvider.GetUsersBySearchCriteria(criteria)).MustHaveHappened(Repeated.Exactly.Once);
		}
		
		[Test]
		public void GetUsersBySearchCriteria_CallMethodWithOneCriteria_CheckSoUnderlyingImplementationIsCalled() {
			var innerProvider = A.Fake<IUserRepository>();
			var adapter = new UserRepositoryAdapter(innerProvider);
			var criteria = A.Fake<SearchCriteria>();
			adapter.GetUsersBySearchCriteria(criteria, true);
			A.CallTo(() => innerProvider.GetUsersBySearchCriteria(A<IEnumerable<SearchCriteria>>.That.Contains(criteria))).MustHaveHappened(Repeated.Exactly.Once);
		}

		[Test]
		public void GetUsersByDomain_CallMethod_CheckSoUnderlyingImplementationIsCalled() {
			var innerProvider = A.Fake<IUserRepository>();
			var adapter = new UserRepositoryAdapter(innerProvider);
			var contexts = A.CollectionOfFake<IContext>(12);
			A.CallTo(() => innerProvider.GetContexts()).Returns(contexts);
			A.CallTo(() => innerProvider.GetUsersByDomain("domain")).Returns(A.CollectionOfFake<IUser>(3));
			adapter.GetUsersByDomain("domain", true);
			A.CallTo(() => innerProvider.GetUsersByDomain("domain")).MustHaveHappened(Repeated.Exactly.Once);
			A.CallTo(() => innerProvider.GetProperties(A<IUser>.Ignored, A<IContext>.Ignored)).MustHaveHappened(Repeated.Exactly.Times(12 * 3));
		}

		[Test]
		public void CreateUserInstance_CallMethod_CheckSoUnderlyingImplementationIsCalled() {
			var innerProvider = A.Fake<IUserRepository>();
			var adapter = new UserRepositoryAdapter(innerProvider);
			adapter.CreateUserInstance("username", "domain", true);
			A.CallTo(() => innerProvider.CreateUserInstance("username", "domain")).MustHaveHappened(Repeated.Exactly.Once);
		}

		[Test]
		public void GetUserByUserName_CallMethod_CheckSoUnderlyingImplementationIsCalled() {
			var innerProvider = A.Fake<IUserRepository>();
			var adapter = new UserRepositoryAdapter(innerProvider);
			adapter.GetUserByUserName("username", "domain", true);
			A.CallTo(() => innerProvider.GetUserByUserName("username", "domain")).MustHaveHappened(Repeated.Exactly.Once);
		}


		[Test]
		public void GetUserById_GetUserAndLoadAllContextsAndCheckCalls_CallsExecuted() {
			var innerProvider = A.Fake<IUserRepository>();
			var contexts = A.CollectionOfFake<IContext>(12);
			A.CallTo(() => innerProvider.GetContexts()).Returns(contexts);
			var adapter = new UserRepositoryAdapter(innerProvider);
			var user = A.Fake<IUser>();
			A.CallTo(() => innerProvider.GetUserById("userid")).Returns(user);
			adapter.GetUserById("userid", true);
			A.CallTo(() => innerProvider.GetUserById("userid")).MustHaveHappened(Repeated.Exactly.Once);
			A.CallTo(() => innerProvider.GetProperties(user, A<IContext>.Ignored)).MustHaveHappened(Repeated.Exactly.Times(12));
		}


		[Test]
		public void GetAllUsers_PageUserCollection_ReturnTheCorrectUusers() {
			var innerProvider = A.Fake<IUserRepository>();
			var users = A.CollectionOfFake<IUser>(256);
			int i = 1;
			foreach(var user in users) {
				IUser u = user;
				A.CallTo(() => u.Id).Returns(i++.ToString());
			}
			A.CallTo(() => innerProvider.GetUsersBySearchCriteria(A<IEnumerable<SearchCriteria>>.That.IsEmpty())).Returns(users);
			var adapter = new UserRepositoryAdapter(innerProvider);
			int totalRecords;
			var pagedUsers = adapter.GetAllUsers(2, 10, out totalRecords);
			Assert.AreEqual(10, pagedUsers.Count);
			Assert.AreEqual("21", pagedUsers[0].Id);
			Assert.AreEqual("30", pagedUsers[9].Id);
			Assert.AreEqual(256, totalRecords);
		}

		[Test]
		public void GetAllUsers_PageUserCollectionInEnd_ReturnTheCorrectUusers() {
			var innerProvider = A.Fake<IUserRepository>();
			var users = A.CollectionOfFake<IUser>(256);
			int i = 1;
			foreach(var user in users) {
				IUser u = user;
				A.CallTo(() => u.Id).Returns(i++.ToString());
			}
			A.CallTo(() => innerProvider.GetUsersBySearchCriteria(A<IEnumerable<SearchCriteria>>.That.IsEmpty())).Returns(users);
			var adapter = new UserRepositoryAdapter(innerProvider);
			int totalRecords;
			var pagedUsers = adapter.GetAllUsers(25, 10, out totalRecords);
			Assert.AreEqual(6, pagedUsers.Count);
			Assert.AreEqual("251", pagedUsers[0].Id);
			Assert.AreEqual("256", pagedUsers[5].Id);
			Assert.AreEqual(256, totalRecords);
		}
	}
}
