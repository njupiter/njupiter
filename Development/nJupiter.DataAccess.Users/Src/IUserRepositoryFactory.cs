using nJupiter.Configuration;
using nJupiter.DataAccess.Users.Caching;

namespace nJupiter.DataAccess.Users {

	public interface IUserRepositoryFactory {
		IUserRepository Create(string name, IConfig config, IPredefinedNames predefinedNames, IUserCache cache);
	}

}