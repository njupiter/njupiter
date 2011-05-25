using System.Collections.Generic;

using FakeItEasy;

using nJupiter.DataAccess.Users;

using NUnit.Framework;

namespace nJupiter.Tests.DataAccess.Users {
	
	[TestFixture]
	public class UserProviderBaseTests {

		[Test]
		public void GetUserByUserName_CallMethod_CheckSoUnderlyingImplementationIsCalledWithDefaultDomain() {
			var innerProvider = A.Fake<IUserProvider>();
			var adapter = new UserProviderAdapter(innerProvider);
			adapter.GetUserByUserName("username");
			A.CallTo(() => innerProvider.GetUserByUserName("username", string.Empty)).MustHaveHappened(Repeated.Exactly.Once);
		}


		[Test]
		public void CreateUserInstance_CallMethod_CheckSoUnderlyingImplementationIsCalledWithDefaultDomain() {
			var innerProvider = A.Fake<IUserProvider>();
			var adapter = new UserProviderAdapter(innerProvider);
			adapter.CreateUserInstance("username");
			A.CallTo(() => innerProvider.CreateUserInstance("username", string.Empty)).MustHaveHappened(Repeated.Exactly.Once);
		}

		[Test]
		public void GetUsersBySearchCriteria_CallMethod_CheckSoUnderlyingImplementationIsCalled() {
			var innerProvider = A.Fake<IUserProvider>();
			var adapter = new UserProviderAdapter(innerProvider);
			var criteria = A.Fake<IEnumerable<SearchCriteria>>();
			adapter.GetUsersBySearchCriteria(criteria, true);
			A.CallTo(() => innerProvider.GetUsersBySearchCriteria(criteria)).MustHaveHappened(Repeated.Exactly.Once);
		}
		
		[Test]
		public void GetUsersBySearchCriteria_CallMethodWithOneCriteria_CheckSoUnderlyingImplementationIsCalled() {
			var innerProvider = A.Fake<IUserProvider>();
			var adapter = new UserProviderAdapter(innerProvider);
			var criteria = A.Fake<SearchCriteria>();
			adapter.GetUsersBySearchCriteria(criteria, true);
			A.CallTo(() => innerProvider.GetUsersBySearchCriteria(A<IEnumerable<SearchCriteria>>.That.Contains(criteria))).MustHaveHappened(Repeated.Exactly.Once);
		}

		[Test]
		public void GetUsersByDomain_CallMethod_CheckSoUnderlyingImplementationIsCalled() {
			var innerProvider = A.Fake<IUserProvider>();
			var adapter = new UserProviderAdapter(innerProvider);
			var contexts = A.CollectionOfFake<IContext>(12);
			A.CallTo(() => innerProvider.GetContexts()).Returns(contexts);
			A.CallTo(() => innerProvider.GetUsersByDomain("domain")).Returns(A.CollectionOfFake<IUser>(3));
			adapter.GetUsersByDomain("domain", true);
			A.CallTo(() => innerProvider.GetUsersByDomain("domain")).MustHaveHappened(Repeated.Exactly.Once);
			A.CallTo(() => innerProvider.GetProperties(A<IUser>.Ignored, A<IContext>.Ignored)).MustHaveHappened(Repeated.Exactly.Times(12 * 3));
		}

		[Test]
		public void CreateUserInstance_CallMethod_CheckSoUnderlyingImplementationIsCalled() {
			var innerProvider = A.Fake<IUserProvider>();
			var adapter = new UserProviderAdapter(innerProvider);
			adapter.CreateUserInstance("username", "domain", true);
			A.CallTo(() => innerProvider.CreateUserInstance("username", "domain")).MustHaveHappened(Repeated.Exactly.Once);
		}

		[Test]
		public void GetUserByUserName_CallMethod_CheckSoUnderlyingImplementationIsCalled() {
			var innerProvider = A.Fake<IUserProvider>();
			var adapter = new UserProviderAdapter(innerProvider);
			adapter.GetUserByUserName("username", "domain", true);
			A.CallTo(() => innerProvider.GetUserByUserName("username", "domain")).MustHaveHappened(Repeated.Exactly.Once);
		}


		[Test]
		public void GetUserById_GetUserAndLoadAllContextsAndCheckCalls_CallsExecuted() {
			var innerProvider = A.Fake<IUserProvider>();
			var contexts = A.CollectionOfFake<IContext>(12);
			A.CallTo(() => innerProvider.GetContexts()).Returns(contexts);
			var adapter = new UserProviderAdapter(innerProvider);
			var user = A.Fake<IUser>();
			A.CallTo(() => innerProvider.GetUserById("userid")).Returns(user);
			adapter.GetUserById("userid", true);
			A.CallTo(() => innerProvider.GetUserById("userid")).MustHaveHappened(Repeated.Exactly.Once);
			A.CallTo(() => innerProvider.GetProperties(user, A<IContext>.Ignored)).MustHaveHappened(Repeated.Exactly.Times(12));
		}


		[Test]
		public void GetAllUsers_PageUserCollection_ReturnTheCorrectUusers() {
			var innerProvider = A.Fake<IUserProvider>();
			var users = A.CollectionOfFake<IUser>(256);
			int i = 1;
			foreach(var user in users) {
				IUser u = user;
				A.CallTo(() => u.Id).Returns(i++.ToString());
			}
			A.CallTo(() => innerProvider.GetUsersBySearchCriteria(A<IEnumerable<SearchCriteria>>.That.IsEmpty())).Returns(users);
			var adapter = new UserProviderAdapter(innerProvider);
			int totalRecords;
			var pagedUsers = adapter.GetAllUsers(2, 10, out totalRecords);
			Assert.AreEqual(10, pagedUsers.Count);
			Assert.AreEqual("21", pagedUsers[0].Id);
			Assert.AreEqual("30", pagedUsers[9].Id);
			Assert.AreEqual(256, totalRecords);
		}

		[Test]
		public void GetAllUsers_PageUserCollectionInEnd_ReturnTheCorrectUusers() {
			var innerProvider = A.Fake<IUserProvider>();
			var users = A.CollectionOfFake<IUser>(256);
			int i = 1;
			foreach(var user in users) {
				IUser u = user;
				A.CallTo(() => u.Id).Returns(i++.ToString());
			}
			A.CallTo(() => innerProvider.GetUsersBySearchCriteria(A<IEnumerable<SearchCriteria>>.That.IsEmpty())).Returns(users);
			var adapter = new UserProviderAdapter(innerProvider);
			int totalRecords;
			var pagedUsers = adapter.GetAllUsers(25, 10, out totalRecords);
			Assert.AreEqual(6, pagedUsers.Count);
			Assert.AreEqual("251", pagedUsers[0].Id);
			Assert.AreEqual("256", pagedUsers[5].Id);
			Assert.AreEqual(256, totalRecords);
		}
	}



	public class UserProviderAdapter : UserProviderBase {
		private readonly IUserProvider provider;

		public UserProviderAdapter(IUserProvider provider) {
			this.provider = provider;
		}

		public override IUser GetUserById(string userId) {
			return provider.GetUserById(userId); 
		}

		public override IUser GetUserByUserName(string userName, string domain) {
			return provider.GetUserByUserName(userName, domain);
		}

		public override IList<IUser> GetUsersBySearchCriteria(IEnumerable<SearchCriteria> searchCriteriaCollection) {
			return provider.GetUsersBySearchCriteria(searchCriteriaCollection);
		}

		public override IList<IUser> GetUsersByDomain(string domain) {
			return provider.GetUsersByDomain(domain);
		}

		public override string[] GetDomains() {
			return provider.GetDomains();
		}

		public override IUser CreateUserInstance(string userName, string domain) {
			return provider.CreateUserInstance(userName, domain);
		}

		public override void SaveUser(IUser user) {
			provider.SaveUser(user);
		}

		public override void SaveUsers(IList<IUser> users) {
			provider.SaveUsers(users);
		}

		public override void SetPassword(IUser user, string password) {
			provider.SetPassword(user, password);
		}

		public override bool CheckPassword(IUser user, string password) {
			return provider.CheckPassword(user, password);
		}

		public override void SaveProperties(IUser user, IPropertyCollection propertyCollection) {
			provider.SaveProperties(user, propertyCollection);
		}

		public override void DeleteUser(IUser user) {
			provider.DeleteUser(user);
		}

		public override IPropertyCollection GetProperties() {
			return provider.GetProperties();
		}

		public override IPropertyCollection GetProperties(IContext context) {
			return provider.GetProperties(context);
		}

		public override IEnumerable<IContext> GetContexts() {
			return provider.GetContexts();
		}

		public override IContext GetContext(string contextName) {
			return provider.GetContext(contextName);
		}

		public override IContext CreateContext(string contextName, ContextSchema schemaTable) {
			return provider.CreateContext(contextName, schemaTable);
		}

		public override void DeleteContext(IContext context) {
			provider.DeleteContext(context);
		}

		public override ContextSchema GetDefaultContextSchema() {
			return provider.GetDefaultContextSchema();
		}

		public override IPropertyCollection GetProperties(IUser user, IContext context) {
			return provider.GetProperties(user, context);
		}
	}
}
