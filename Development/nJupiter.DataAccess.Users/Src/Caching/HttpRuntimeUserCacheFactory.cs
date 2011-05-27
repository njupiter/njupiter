using nJupiter.Configuration;

namespace nJupiter.DataAccess.Users.Caching {
	class HttpRuntimeUserCacheFactory : IUserCacheFactory {
		public IUserCache Create(IConfig config) {
			return new HttpRuntimeUserCache(config);
		}
	}
}
