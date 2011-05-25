using System;
using System.Collections.Generic;

using nJupiter.DataAccess.Users;

using NUnit.Framework;

namespace nJupiter.Tests.nJupiter.DataAccess.Users {
	
	[TestFixture]
	public class UserProviderBaseTests {
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

		public override IPropertyCollection GetProperties(Context context) {
			return provider.GetProperties(context);
		}

		public override IEnumerable<Context> GetContexts() {
			return provider.GetContexts();
		}

		public override Context GetContext(string contextName) {
			return provider.GetContext(contextName);
		}

		public override Context CreateContext(string contextName, ContextSchema schemaTable) {
			return provider.CreateContext(contextName, schemaTable);
		}

		public override void DeleteContext(Context context) {
			provider.DeleteContext(context);
		}

		public override ContextSchema GetDefaultContextSchema() {
			return provider.GetDefaultContextSchema();
		}


	}
}
