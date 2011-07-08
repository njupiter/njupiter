#region Copyright & License
/*
	Copyright (c) 2005-2011 nJupiter

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
using System.Collections;
using System.Collections.Generic;

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

		public GenericUserCache(IConfig config) : base(config){}

		private int MinutesInCache {
			get {
				if(this.minutesInCache < 0) {
					if(this.Config.ContainsKey("cache", "minutesToCacheUser"))
						this.minutesInCache = this.Config.GetValue<int>("cache", "minutesToCacheUser");
					else
						this.minutesInCache = 0;
				}
				return this.minutesInCache;
			}
		}

		private int MaxUsersInCache {
			get {
				if(this.maxUsersInCache < 0) {
					if(this.Config.ContainsKey("cache", "maxUsersInCache")) {
						this.maxUsersInCache = this.Config.GetValue<int>("cache", "maxUsersInCache");
						if(this.maxUsersInCache < MinimumCacheSize && this.maxUsersInCache != 0)
							this.maxUsersInCache = MinimumCacheSize;
					} else {
						this.maxUsersInCache = DefaultCacheSize;
					}
				}
				return 10000;
			}
		}

		private static int CacheTruncationFactor {
			get {
				return CacheTruncateFactor;
			}
		}

		public override IUser GetUserById(string userId) {
			if(userId == null || this.MinutesInCache == 0)
				return null;
			return GetUserFromCacheMap(userId);
		}

		public override IUser GetUserByUserName(string userName, string domain) {
			if(userName == null || this.MinutesInCache == 0)
				return null;
			return GetUserFromCacheMap(userName, domain);
		}

		public override void RemoveUserFromCache(IUser user) {
			if(user != null) {
				RemoveUserFromCache(user, GetCacheMapId(user));
			}
		}

		public override void RemoveUsersFromCache(IList<IUser> users) {
			if(users != null) {
				foreach(IUser user in users) {
					this.RemoveUserFromCache(user);
				}
			}
		}

		public override void AddUserToCache(IUser user) {
			if(user != null && this.MinutesInCache > 0) {
				lock(padlock) {
					if(this.cachedUsers.Contains(user))
						this.RemoveUserFromCache(user);

					// Truncate cache if it has grown out of size.
					if(this.MaxUsersInCache != 0 && this.cachedUsers.Count >= this.MaxUsersInCache)
						TruncateCache();

					user.MakeReadOnly();
					
					CacheMapId cacheMapId = new CacheMapId(user.UserName, user.Domain);
					CachedUser cachedUser = new CachedUser(user);
					this.cachedMap.Add(cacheMapId, cachedUser);
					this.cachedUsers.Add(user);
				}
			}
		}

		public override void AddUsersToCache(IList<IUser> users) {
			if(users != null && this.MinutesInCache > 0) {
				foreach(IUser user in users) {
					this.AddUserToCache(user);
				}
			}
		}

		private void RemoveUserFromCache(IUser user, CacheMapId cacheMapId) {
			if(user != null) {
				lock(padlock) {
					this.cachedUsers.Remove(user);
					this.cachedMap.Remove(cacheMapId);
				}
			}
		}

		private IUser GetUserFromCacheMap(string userId) {
			lock(padlock) {
				var user = GetUser(userId);
				if(user == null) {
					return null;
				}
				CacheMapId cacheMapId = new CacheMapId(user.UserName, user.Domain);
				return GetUserFromCacheMap(cacheMapId);
			}
		}

		private IUser GetUser(string userId) {
			foreach(IUser u in cachedUsers) {
				if(u.Id.Equals(userId)) {
					return u;
				}
			}
			return null;
		}

		private IUser GetUserFromCacheMap(string userName, string domain) {
			lock(padlock) {
				CacheMapId cacheMapId = new CacheMapId(userName, domain);
				return GetUserFromCacheMap(cacheMapId);
			}
		}

		private IUser GetUserFromCacheMap(CacheMapId cacheMapId) {
			lock(this.cachedMap.SyncRoot) {
				if(!this.cachedMap.Contains(cacheMapId))
					return null;

				CachedUser cachedUser = (CachedUser)this.cachedMap[cacheMapId];

				// Remove user from cache if the cache time has expired
				if(cachedUser.DateCreated > DateTime.Now.AddMinutes(this.MinutesInCache)) {
					this.RemoveUserFromCache(cachedUser.User, cacheMapId);
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

			ArrayList truncateList = new ArrayList(this.cachedMap);
			truncateList.Sort(CachedUserComparer.Instance);
			int itemsToRemove = ((this.MaxUsersInCache * CacheTruncationFactor) / 100) + (truncateList.Count - this.MaxUsersInCache);
			DateTime cacheTime = DateTime.Now.AddMinutes(this.MinutesInCache);

			// Remove users from cahce, remove atleast a factor of [CacheTruncationFactor] from the cache
			// If not enough users has a cache time that has expired the oldest cached objects will be removed
			foreach(DictionaryEntry dicEntry in truncateList) {
				CachedUser cachedUser = (CachedUser)dicEntry.Value;
				if(cachedUser.User != null) {
					if(itemsToRemove <= 0 && cachedUser.DateCreated < cacheTime)
						break;
					RemoveUserFromCache(cachedUser.User);
					itemsToRemove--;
				}
			}
		}

		// Sort cached users and put the oldest first
		private sealed class CachedUserComparer : IComparer {

			#region Members
			private static readonly CachedUserComparer instance = new CachedUserComparer();
			#endregion

			#region Constructors
			private CachedUserComparer() { }
			#endregion

			#region Singelton Instance
			public static CachedUserComparer Instance {
				get {
					return instance;
				}
			}
			#endregion

			#region IComparer Members
			public int Compare(object x, object y) {
				DictionaryEntry xEntry = (DictionaryEntry)x;
				DictionaryEntry yEntry = (DictionaryEntry)y;

				if(xEntry.Value == null || yEntry.Value == null)
					return 0;

				CachedUser xCachedUser = (CachedUser)xEntry.Value;
				CachedUser yCachedUser = (CachedUser)yEntry.Value;

				if(xCachedUser.User == null || yCachedUser.User == null)
					return 0;

				return xCachedUser.DateCreated.CompareTo(yCachedUser.DateCreated);
			}
			#endregion
		}

		private struct CachedUser {

			public readonly IUser User;
			public readonly DateTime DateCreated;

			public CachedUser(IUser user) {
				User = user;
				DateCreated = DateTime.Now;
			}
		}

		private struct CacheMapId {

			private const int InitialPrime = 17;
			private const int MultiplierPrime = 37;

			private readonly string userName;
			private readonly string domain;

			public CacheMapId(string userName, string domain) {
				this.userName = userName;
				this.domain = domain ?? string.Empty;
			}

			public override bool Equals(object obj) {
				CacheMapId map = (CacheMapId)obj;
				if(map.userName == null)
					return false;
				return map.userName.Equals(this.userName) && map.domain.Equals(this.domain);
			}

			public override int GetHashCode() {
				// Refer to Effective Java 1st ed page 34 for an good explanation of this hash code implementation
				int hash = InitialPrime;
				hash = (MultiplierPrime * hash) + this.userName.GetHashCode();
				hash = (MultiplierPrime * hash) + this.domain.GetHashCode();
				return hash;
			}
		}


	}
}
