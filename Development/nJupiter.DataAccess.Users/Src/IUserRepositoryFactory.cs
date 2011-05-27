using nJupiter.Configuration;

namespace nJupiter.DataAccess.Users {

	public interface IUserRepositoryFactory {
		IUserRepository Create(string name, IConfig config, IPredefinedNames predefinedNames, IUserCache cache);
	}

}