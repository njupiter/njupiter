using nJupiter.Configuration;
using nJupiter.DataAccess.Users.Caching;

namespace nJupiter.DataAccess.Users.Sql {
	public class UserRepositoryFactory : IUserRepositoryFactory {
		public IUserRepository Create(string name, IConfig config, IPredefinedNames predefinedNames, IUserCache cache) {
			return new UserRepository(name, config, predefinedNames, cache);
		}
	}
}
