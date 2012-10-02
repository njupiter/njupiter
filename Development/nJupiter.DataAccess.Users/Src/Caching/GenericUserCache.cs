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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using nJupiter.Configuration;

namespace nJupiter.DataAccess.Users.Caching {
	public class GenericUserCache : UserCacheBase {
		private const int DefaultCacheSize = 1000;
		private const int MinimumCacheSize = 100;
		private const int CacheTruncateFactor = 10; // The precentage to truncate the cache when max users has been reched

		private readonly IList<IUser> cachedUsers = new List<IUser>();
		private readonly Hashtable cachedMap = new Hashtable();
		private readonly object padlock = new object();
		private int minutesInCache = -1; // If zero, caching is turned off
		private int maxUsersInCache = -1; // If zero, then the cache can grow unrestrainedly

		public GenericUserCache(IConfig config) : base(config) {}

		private int MinutesInCache {
			get {
				if(minutesInCache < 0) {
					if(Config.ContainsKey("cache", "minutesToCacheUser")) {
						minutesInCache = Config.GetValue<int>("cache", "minutesToCacheUser");
					} else {
						minutesInCache = 0;
					}
				}
				return minutesInCache;
			}
		}

		private int MaxUsersInCache {
			get {
				if(maxUsersInCache < 0) {
					if(Config.ContainsKey("cache", "maxUsersInCache")) {
						maxUsersInCache = Config.GetValue<int>("cache", "maxUsersInCache");
						if(maxUsersInCache < MinimumCacheSize && maxUsersInCache != 0) {
							maxUsersInCache = MinimumCacheSize;
						}
					} else {
						maxUsersInCache = DefaultCacheSize;
					}
				}
				return 10000;
			}
		}

		private static int CacheTruncationFactor { get { return CacheTruncateFactor; } }

		public override IUser GetUserById(string userId) {
			if(userId == null || MinutesInCache == 0) {
				return null;
			}
			return GetUserFromCacheMap(userId);
		}

		public override IUser GetUserByUserName(string userName, string domain) {
			if(userName == null || MinutesInCache == 0) {
				return null;
			}
			return GetUserFromCacheMap(userName, domain);
		}

		public override void RemoveUserFromCache(IUser user) {
			if(user != null) {
				RemoveUserFromCache(user, GetCacheMapId(user));
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
				lock(padlock) {
					if(cachedUsers.Contains(user)) {
						RemoveUserFromCache(user);
					}

					// Truncate cache if it has grown out of size.
					if(MaxUsersInCache != 0 && cachedUsers.Count >= MaxUsersInCache) {
						TruncateCache();
					}

					user.MakeReadOnly();

					var cacheMapId = new CacheMapId(user.UserName, user.Domain);
					var cachedUser = new CachedUser(user);
					cachedMap.Add(cacheMapId, cachedUser);
					cachedUsers.Add(user);
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

		private void RemoveUserFromCache(IUser user, CacheMapId cacheMapId) {
			if(user != null) {
				lock(padlock) {
					cachedUsers.Remove(user);
					cachedMap.Remove(cacheMapId);
				}
			}
		}

		private IUser GetUserFromCacheMap(string userId) {
			lock(padlock) {
				var user = GetUser(userId);
				if(user == null) {
					return null;
				}
				var cacheMapId = new CacheMapId(user.UserName, user.Domain);
				return GetUserFromCacheMap(cacheMapId);
			}
		}

		private IUser GetUser(string userId) {
			return cachedUsers.FirstOrDefault(u => u.Id.Equals(userId));
		}

		private IUser GetUserFromCacheMap(string userName, string domain) {
			lock(padlock) {
				var cacheMapId = new CacheMapId(userName, domain);
				return GetUserFromCacheMap(cacheMapId);
			}
		}

		private IUser GetUserFromCacheMap(CacheMapId cacheMapId) {
			lock(cachedMap.SyncRoot) {
				if(!cachedMap.Contains(cacheMapId)) {
					return null;
				}

				var cachedUser = (CachedUser)cachedMap[cacheMapId];

				// Remove user from cache if the cache time has expired
				if(cachedUser.DateCreated > DateTime.Now.AddMinutes(MinutesInCache)) {
					RemoveUserFromCache(cachedUser.User, cacheMapId);
					return null;
				}
				return cachedUser.User;
			}
		}

		private static CacheMapId GetCacheMapId(IUser user) {
			return GetCacheMapId(user.UserName, user.Domain);
		}

		private static CacheMapId GetCacheMapId(string userName, string domain) {
			return new CacheMapId(userName, domain);
		}

		private void TruncateCache() {
			var truncateList = new ArrayList(cachedMap);
			truncateList.Sort(CachedUserComparer.Instance);
			int itemsToRemove = ((MaxUsersInCache * CacheTruncationFactor) / 100) + (truncateList.Count - MaxUsersInCache);
			var cacheTime = DateTime.Now.AddMinutes(MinutesInCache);

			// Remove users from cahce, remove atleast a factor of [CacheTruncationFactor] from the cache
			// If not enough users has a cache time that has expired the oldest cached objects will be removed
			foreach(DictionaryEntry dicEntry in truncateList) {
				var cachedUser = (CachedUser)dicEntry.Value;
				if(cachedUser.User != null) {
					if(itemsToRemove <= 0 && cachedUser.DateCreated < cacheTime) {
						break;
					}
					RemoveUserFromCache(cachedUser.User);
					itemsToRemove--;
				}
			}
		}

	}
}