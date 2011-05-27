using nJupiter.Configuration;

namespace nJupiter.DataAccess.Users.Caching {
	public class GenericUserCacheFactory : IUserCacheFactory {
		public IUserCache Create(IConfig config) {
			return new GenericUserCache(config);
		}
	}
}
