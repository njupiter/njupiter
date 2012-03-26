#region Copyright & License
/*
	Copyright (c) 2005-2010 nJupiter

	Permission is hereby granted, free of charge, to any person obtaining a copy
	of this software and associated documentation files (the "Software"), to deal
	in the Software without restriction, including without limitation the rights
	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	copies of the Software, and to permit persons to whom the Software is
	furnished to do so, subject to the following conditions:

	The above copyright notice and this permission notice shall be included in
	all copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
	THE SOFTWARE.
*/
#endregion

using System;
using System.Web;
using System.Web.Caching;

using log4net;

namespace nJupiter.DataAccess.Users {

	// TODO: Return a cloned instance, without it usercache isn't thread safe. should be configurable
	public class HttpRuntimeUserCache : IUserCache {

		#region Members
		private readonly UsersDAO	usersDao;
		private int					minutesInCache		= -1; // If zero, caching is turned off
		private bool?				slidingExpiration;
		private CacheItemPriority?	cacheItemPriority;
		#endregion

		#region Constructors
		public HttpRuntimeUserCache(UsersDAO usersDao) {
			if(usersDao == null) {
				throw new ArgumentNullException("usersDao");
			}
			this.usersDao = usersDao;
		}
		#endregion

		#region Static Members
		private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		#endregion

		#region Properties
		private int MinutesInCache {
			get {
				if(this.minutesInCache < 0){
					if(usersDao.Config != null && usersDao.Config.ContainsKey("cache", "minutesInCache")) {
						this.minutesInCache = usersDao.Config.GetIntValue("cache", "minutesInCache");
					} else {
						this.minutesInCache = 0;
					}
				}
				return this.minutesInCache;
			}
		}

		private bool SlidingExpiration {
			get {
				if(slidingExpiration == null){
					if(usersDao.Config != null && usersDao.Config.ContainsKey("cache", "slidingExpiration")) {
						this.slidingExpiration = usersDao.Config.GetBoolValue("cache", "slidingExpiration");
					} else {
						this.slidingExpiration = false;
					}
				}
				return (bool)this.slidingExpiration;
			}
		}

		private CacheItemPriority CachePriority {
			get {
				if(cacheItemPriority == null){
					if(usersDao.Config != null && usersDao.Config.ContainsKey("cache", "cachePriority")) {
						string configValue = usersDao.Config.GetValue("cache", "cachePriority");
						this.cacheItemPriority = (CacheItemPriority)Enum.Parse(typeof(CacheItemPriority), configValue, true);
					} else {
						this.cacheItemPriority = CacheItemPriority.Normal;
					}
				}
				return (CacheItemPriority)this.cacheItemPriority;
			}
		}
		#endregion
		
		#region Methods
		public User GetUserById(string userId){
			if(userId == null || this.MinutesInCache == 0)
				return null;
			UserIdCacheKey userIdCacheKey = new UserIdCacheKey(this.usersDao, userId);
			return HttpRuntime.Cache[userIdCacheKey.CacheKey] as User;
		}

		public User GetUserByUserName(string userName, string domain){
			if(userName == null || this.MinutesInCache == 0)
				return null;
			UsernameCacheKey usernameCacheKey = new UsernameCacheKey(this.usersDao, userName, domain);
			return HttpRuntime.Cache[usernameCacheKey.CacheKey] as User;
		}
		#endregion

		#region Protected Methods
		public void RemoveUserFromCache(User user){
			if(user != null){
				UsernameCacheKey usernameCacheKey = new UsernameCacheKey(this.usersDao, user.UserName, user.Domain);
				UserIdCacheKey userIdCacheKey =  new UserIdCacheKey(this.usersDao, user.Id);
				if(log.IsDebugEnabled) { log.Debug(string.Format("Removing user [{0}/{1}] from cache.", (user.Domain ?? string.Empty), user.UserName)); }
				HttpRuntime.Cache.Remove(usernameCacheKey.CacheKey);
				HttpRuntime.Cache.Remove(userIdCacheKey.CacheKey);
			}
		}

		public void RemoveUsersFromCache(UserCollection users){
			if(users != null){
				foreach(User user in users){
					this.RemoveUserFromCache(user);
				}
			}
		}

		public void AddUserToCache(User user){
			if(user != null && this.MinutesInCache > 0) {
				UsernameCacheKey usernameCacheKey = new UsernameCacheKey(this.usersDao, user.UserName, user.Domain);
				UserIdCacheKey userIdCacheKey =  new UserIdCacheKey(this.usersDao, user.Id);
				if(log.IsDebugEnabled) { log.Debug(string.Format("Adding user [{0}/{1}] to cache.", (user.Domain ?? string.Empty), user.UserName)); }
				if(this.SlidingExpiration) {
					TimeSpan expirationTime = new TimeSpan(0, 0, this.MinutesInCache, 0);
					HttpRuntime.Cache.Add(usernameCacheKey.CacheKey, user, null, Cache.NoAbsoluteExpiration, expirationTime, this.CachePriority, null);
					HttpRuntime.Cache.Add(userIdCacheKey.CacheKey, user, null, Cache.NoAbsoluteExpiration, expirationTime, this.CachePriority, null);
				} else {
					DateTime expirationTime = DateTime.Now.AddMinutes(this.MinutesInCache);
					HttpRuntime.Cache.Add(usernameCacheKey.CacheKey, user, null, expirationTime, Cache.NoSlidingExpiration, this.CachePriority, null);
					HttpRuntime.Cache.Add(userIdCacheKey.CacheKey, user, null, expirationTime, Cache.NoSlidingExpiration, this.CachePriority, null);
				}
			}
		}

		public void AddUsersToCache(UserCollection users){
			if(users != null && this.MinutesInCache > 0){
				foreach(User user in users){
					this.AddUserToCache(user);
				}
			}
		}
		#endregion

		#region Private Structs
		private struct UserIdCacheKey{
			
			private	readonly UsersDAO	usersDao;
			private	readonly string		userId;
			private	readonly int		hash;
			private	readonly string		cacheKey;
			
			public UserIdCacheKey(UsersDAO usersDao, string id){
				this.userId	= id ?? string.Empty;
				this.usersDao	= usersDao;

				int result = 17;
				result = (37*result) + this.userId.GetHashCode();
				result = (37*result) + this.usersDao.Name.GetHashCode();
				hash = result;
				cacheKey = string.Format("nJupiter.DataAccess.Users.UsersDao:{0}:UserIdCacheKey:{1}", usersDao.Name, hash);
			}

			public override bool Equals(object obj) {
				UserIdCacheKey map = (UserIdCacheKey)obj;
				if(map.userId == null)
					return false;
				return map.userId.Equals(this.userId) && map.usersDao.Name.Equals(this.usersDao.Name);
			}

			public string CacheKey {
				get {
					return cacheKey;
				}
			}

			public override int GetHashCode() {
				return hash;
			}
		}

		private struct UsernameCacheKey{
			
			private	readonly UsersDAO	usersDao;
			private	readonly string		userName;
			private	readonly string		domain;
			private	readonly int		hash;
			private	readonly string		cacheKey;
			
			public UsernameCacheKey(UsersDAO usersDao, string userName, string domain){
				this.userName	= userName;
				this.domain		= domain ?? string.Empty;
				this.usersDao	= usersDao;

				// Calculate a unique hash that will match all id:s with the same user name and domain
				int result = 17;
				result = (37*result) + this.userName.GetHashCode();
				result = (37*result) + this.domain.GetHashCode();
				result = (37*result) + this.usersDao.Name.GetHashCode();

				hash = result;
				cacheKey = string.Format("nJupiter.DataAccess.Users.UsersDao:{0}:UsernameCacheKey:{1}", this.usersDao.Name, hash);
			}

			public override bool Equals(object obj) {
				UsernameCacheKey map = (UsernameCacheKey)obj;
				if(map.userName == null)
					return false;
				return map.userName.Equals(this.userName) && map.domain.Equals(this.domain) && map.usersDao.Name.Equals(this.usersDao.Name);
			}

			public string CacheKey {
				get {
					return cacheKey;
				}
			}

			public override int GetHashCode() {
				return hash;
			}
		}
		#endregion

	}
}
