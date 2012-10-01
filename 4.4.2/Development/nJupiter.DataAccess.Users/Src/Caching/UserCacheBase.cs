using System;
using System.Collections.Generic;

using nJupiter.Configuration;

namespace nJupiter.DataAccess.Users.Caching {
	public abstract class UserCacheBase : IUserCache {

		protected IConfig Config { get; private set; }

		protected UserCacheBase(IConfig config) {
			if(config == null) {
				throw new ArgumentNullException("config");
			}
			this.Config = config;
		}

		public abstract IUser GetUserById(string userId);
		public abstract IUser GetUserByUserName(string userName, string domain);
		public abstract void RemoveUserFromCache(IUser user);
		public abstract void RemoveUsersFromCache(IList<IUser> users);
		public abstract void AddUserToCache(IUser user);
		public abstract void AddUsersToCache(IList<IUser> users);
	}
}
