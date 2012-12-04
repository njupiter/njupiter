#region Copyright & License
// 
// 	Copyright (c) 2005-2012 nJupiter
// 
// 	Permission is hereby granted, free of charge, to any person obtaining a copy
// 	of this software and associated documentation files (the "Software"), to deal
// 	in the Software without restriction, including without limitation the rights
// 	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// 	copies of the Software, and to permit persons to whom the Software is
// 	furnished to do so, subject to the following conditions:
// 
// 	The above copyright notice and this permission notice shall be included in
// 	all copies or substantial portions of the Software.
// 
// 	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// 	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// 	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// 	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// 	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// 	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// 	THE SOFTWARE.
// 
#endregion

using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;

using nJupiter.Configuration;

namespace nJupiter.DataAccess.Users.Caching {
	public class HttpRuntimeUserCache : UserCacheBase {
		private int minutesInCache = -1; // If zero, caching is turned off
		private bool? slidingExpiration;
		private CacheItemPriority? cacheItemPriority;

		public HttpRuntimeUserCache(IConfig config) : base(config) {}

		private int MinutesInCache {
			get {
				if(minutesInCache < 0) {
					if(Config.ContainsKey("cache", "minutesInCache")) {
						minutesInCache = Config.GetValue<int>("cache", "minutesInCache");
					} else {
						minutesInCache = 0;
					}
				}
				return minutesInCache;
			}
		}

		private bool SlidingExpiration {
			get {
				if(slidingExpiration == null) {
					if(Config.ContainsKey("cache", "slidingExpiration")) {
						slidingExpiration = Config.GetValue<bool>("cache", "slidingExpiration");
					} else {
						slidingExpiration = false;
					}
				}
				return (bool)slidingExpiration;
			}
		}

		private CacheItemPriority CachePriority {
			get {
				if(cacheItemPriority == null) {
					if(Config.ContainsKey("cache", "cachePriority")) {
						var configValue = Config.GetValue("cache", "cachePriority");
						cacheItemPriority = (CacheItemPriority)Enum.Parse(typeof(CacheItemPriority), configValue, true);
					} else {
						cacheItemPriority = CacheItemPriority.Normal;
					}
				}
				return (CacheItemPriority)cacheItemPriority;
			}
		}

		public override IUser GetUserById(string userId) {
			if(userId == null || MinutesInCache == 0) {
				return null;
			}
			var userIdCacheKey = new UserIdCacheKey(Config.ConfigKey, userId);
			return HttpRuntime.Cache[userIdCacheKey.CacheKey] as IUser;
		}

		public override IUser GetUserByUserName(string userName, string domain) {
			if(userName == null || MinutesInCache == 0) {
				return null;
			}
			var usernameCacheKey = new UsernameCacheKey(Config.ConfigKey, userName, domain);
			return HttpRuntime.Cache[usernameCacheKey.CacheKey] as IUser;
		}

		public override void RemoveUserFromCache(IUser user) {
			if(user != null) {
				var usernameCacheKey = new UsernameCacheKey(Config.ConfigKey, user.UserName, user.Domain);
				var userIdCacheKey = new UserIdCacheKey(Config.ConfigKey, user.Id);
				HttpRuntime.Cache.Remove(usernameCacheKey.CacheKey);
				HttpRuntime.Cache.Remove(userIdCacheKey.CacheKey);
			}
		}

		public override void RemoveUsersFromCache(IList<IUser> users) {
			if(users != null) {
				foreach(var user in users) {
					RemoveUserFromCache(user);
				}
			}
		}

		public override void AddUserToCache(IUser user) {
			if(user != null && MinutesInCache > 0) {
				user.MakeReadOnly();
				var usernameCacheKey = new UsernameCacheKey(Config.ConfigKey, user.UserName, user.Domain);
				var userIdCacheKey = new UserIdCacheKey(Config.ConfigKey, user.Id);
				if(SlidingExpiration) {
					var expirationTime = new TimeSpan(0, 0, MinutesInCache, 0);
					HttpRuntime.Cache.Add(usernameCacheKey.CacheKey,
					                      user,
					                      null,
					                      Cache.NoAbsoluteExpiration,
					                      expirationTime,
					                      CachePriority,
					                      null);
					HttpRuntime.Cache.Add(userIdCacheKey.CacheKey,
					                      user,
					                      null,
					                      Cache.NoAbsoluteExpiration,
					                      expirationTime,
					                      CachePriority,
					                      null);
				} else {
					var expirationTime = DateTime.Now.AddMinutes(MinutesInCache);
					HttpRuntime.Cache.Add(usernameCacheKey.CacheKey,
					                      user,
					                      null,
					                      expirationTime,
					                      Cache.NoSlidingExpiration,
					                      CachePriority,
					                      null);
					HttpRuntime.Cache.Add(userIdCacheKey.CacheKey,
					                      user,
					                      null,
					                      expirationTime,
					                      Cache.NoSlidingExpiration,
					                      CachePriority,
					                      null);
				}
			}
		}

		public override void AddUsersToCache(IList<IUser> users) {
			if(users != null && MinutesInCache > 0) {
				foreach(var user in users) {
					AddUserToCache(user);
				}
			}
		}
	}
}