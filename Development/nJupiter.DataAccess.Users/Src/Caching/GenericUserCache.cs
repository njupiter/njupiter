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

using log4net;

namespace nJupiter.DataAccess.Users {

	// TODO: Return a cloned instance, without it usercache isn't thread safe. should be configurable
	public class GenericUserCache : IUserCache {

		#region Constants
		private const int DefaultCacheSize = 1000;
		private const int MinimumCacheSize = 100;
		private const int CacheTruncateFactor = 10; // The precentage to truncate the cache when max users has been reched
		#endregion

		#region Members
		private readonly UserProviderBase userProvider;
		private readonly IList<IUser> cachedUsers = new List<IUser>();
		private readonly Hashtable cachedMap = new Hashtable();
		private readonly object padlock = new object();
		private int minutesInCache = -1; // If zero, caching is turned off
		private int maxUsersInCache = -1; // If zero, then the cache can grow unrestrainedly
		#endregion

		#region Static Members
		private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		#endregion

		#region Constructors
		public GenericUserCache(UserProviderBase userProvider) {
			if(userProvider == null) {
				throw new ArgumentNullException("userProvider");
			}
			this.userProvider = userProvider;
		}
		#endregion

		#region Properties
		private int MinutesInCache {
			get {
				if(this.minutesInCache < 0) {
					if(this.userProvider.Config != null && this.userProvider.Config.ContainsKey("cache", "minutesToCacheUser"))
						this.minutesInCache = this.userProvider.Config.GetValue<int>("cache", "minutesToCacheUser");
					else
						this.minutesInCache = 0;
				}
				return this.minutesInCache;
			}
		}

		private int MaxUsersInCache {
			get {
				if(this.maxUsersInCache < 0) {
					if(this.userProvider.Config != null && this.userProvider.Config.ContainsKey("cache", "maxUsersInCache")) {
						this.maxUsersInCache = this.userProvider.Config.GetValue<int>("cache", "maxUsersInCache");
						if(this.maxUsersInCache < MinimumCacheSize && this.maxUsersInCache != 0)
							this.maxUsersInCache = MinimumCacheSize;
					} else {
						this.maxUsersInCache = DefaultCacheSize;
					}
				}
				return 10;
			}
		}

		private static int CacheTruncationFactor {
			get {
				return CacheTruncateFactor;
			}
		}
		#endregion

		#region Methods
		public IUser GetUserById(string userId) {
			if(userId == null || this.MinutesInCache == 0)
				return null;
			return GetUserFromCacheMap(userId);
		}

		public IUser GetUserByUserName(string userName, string domain) {
			if(userName == null || this.MinutesInCache == 0)
				return null;
			return GetUserFromCacheMap(userName, domain);
		}

		public void RemoveUserFromCache(IUser user) {
			if(user != null) {
				RemoveUserFromCache(user, GetCacheMapId(user));
			}
		}

		public void RemoveUsersFromCache(IList<IUser> users) {
			if(users != null) {
				foreach(IUser user in users) {
					this.RemoveUserFromCache(user);
				}
			}
		}

		public void AddUserToCache(IUser user) {
			if(user != null && this.MinutesInCache > 0) {
				lock(padlock) {
					if(this.cachedUsers.Contains(user))
						this.RemoveUserFromCache(user);

					// Truncate cache if it has grown out of size.
					if(this.MaxUsersInCache != 0 && this.cachedUsers.Count >= this.MaxUsersInCache)
						TruncateCache();

					if(Log.IsDebugEnabled) { Log.Debug(string.Format("Adding user user [{0}/{1}] to cache.", (user.Domain ?? string.Empty), user.UserName)); }

					CacheMapId cacheMapId = new CacheMapId(user.UserName, user.Domain);
					CachedUser cachedUser = new CachedUser(user);
					this.cachedMap.Add(cacheMapId, cachedUser);
					this.cachedUsers.Add(user);
				}
			}
		}

		public void AddUsersToCache(IList<IUser> users) {
			if(users != null && this.MinutesInCache > 0) {
				foreach(IUser user in users) {
					this.AddUserToCache(user);
				}
			}
		}
		#endregion

		#region Private Methods
		private void RemoveUserFromCache(IUser user, CacheMapId cacheMapId) {
			if(user != null) {
				lock(padlock) {
					if(Log.IsDebugEnabled) { Log.Debug(string.Format("Remove user [{0}/{1}] from cache.", (cacheMapId.Domain ?? string.Empty), cacheMapId.UserName)); }
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
				if(Log.IsDebugEnabled) { Log.Debug(string.Format("Get user [{0}/{1}] from cache.", (cacheMapId.Domain ?? string.Empty), cacheMapId.UserName)); }
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
			if(Log.IsDebugEnabled) { Log.Debug("Truncating user cache"); }

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
		#endregion

		#region Private Structs
		private struct CachedUser {

			public readonly IUser User;
			public readonly DateTime DateCreated;

			public CachedUser(IUser user) {
				User = user;
				DateCreated = DateTime.Now;
			}
		}

		private struct CacheMapId {

			public readonly string UserName;
			public readonly string Domain;
			private readonly int hash;

			public CacheMapId(string userName, string domain) {
				UserName = userName;
				Domain = domain ?? string.Empty;

				// Calculate a unique hash that will match all id:s with the same user name and domain
				int result = 17;
				result = (37 * result) + UserName.GetHashCode();
				result = (37 * result) + Domain.GetHashCode();

				hash = result;
			}

			public override bool Equals(object obj) {
				CacheMapId map = (CacheMapId)obj;
				if(map.UserName == null)
					return false;
				return map.UserName.Equals(UserName) && map.Domain.Equals(Domain);
			}

			public override int GetHashCode() {
				return hash;
			}
		}
		#endregion

	}
}
