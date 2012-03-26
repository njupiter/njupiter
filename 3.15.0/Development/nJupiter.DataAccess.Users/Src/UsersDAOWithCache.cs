using System;
using System.Collections;

using log4net;

namespace nJupiter.DataAccess.Users {

	// TODO: Return a cloned instance, without it usercache isn't thread safe. should be configurable
	public abstract class UsersDAOWithCache : UsersDAO {

		#region Constants
		private const int DefaultCacheSize		= 1000;
		private const int MinimumCacheSize		= 100;
		private const int CacheTruncateFactor	= 10; // The precentage to truncate the cache when max users has been reched
		#endregion
		
		#region Members
		private readonly	UserCollection	cachedUsers		= new UserCollection();
		private readonly	Hashtable		cachedMap		= new Hashtable();
		private				int				minutesInCache	= -1; // If zero, caching is turned off
		private				int				maxUsersInCache	= -1; // If zero, then the cache can grow unrestrainedly
		#endregion

		#region Static Members
		private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		#endregion

		#region Properties
		private int MinutesInCache {
			get {
				if(this.minutesInCache < 0){
					if(this.Config != null && this.Config.ContainsKey("cache", "minutesToCacheUser"))
						this.minutesInCache = this.Config.GetIntValue("cache", "minutesToCacheUser");
					else
						this.minutesInCache = 0;
				}
				return this.minutesInCache;
			}
		}

		private int MaxUsersInCache	{
			get {
				if(this.maxUsersInCache < 0){
					if(this.Config != null && this.Config.ContainsKey("cache", "maxUsersInCache")){
						this.maxUsersInCache = this.Config.GetIntValue("cache", "maxUsersInCache");
						if(this.maxUsersInCache < MinimumCacheSize && this.maxUsersInCache != 0)
							this.maxUsersInCache = MinimumCacheSize;
					}else{
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
		
		#region Overridden Methods
		public override User GetUserById(string userId){
			if(userId == null || this.MinutesInCache == 0)
				return null;
			return GetUserFromCacheMap(userId);
		}

		public override User GetUserByUserName(string userName, string domain){
			if(userName == null || this.MinutesInCache == 0)
				return null;
			return GetUserFromCacheMap(userName, domain);
		}
		#endregion

		#region Protected Methods
		protected void RemoveUserFromCache(User user){
			if(user != null){
				RemoveUserFromCache(user, GetCacheMapId(user));
			}
		}

		protected void RemoveUsersFromCache(UserCollection users){
			if(users != null){
				foreach(User user in users){
					this.RemoveUserFromCache(user);
				}
			}
		}

		protected void AddUserToCache(User user){
			if(user != null && this.MinutesInCache > 0){
				lock(this.cachedUsers.SyncRoot){
					lock(this.cachedMap.SyncRoot){
						if(this.cachedUsers.Contains(user))
							this.RemoveUserFromCache(user);

						// Truncate cache if it has grown out of size.
						if(this.MaxUsersInCache != 0 && this.cachedUsers.Count >= this.MaxUsersInCache)
							TruncateCache();

						if(log.IsDebugEnabled) { log.Debug(string.Format("Adding user user [{0}/{1}] to cache.", (user.Domain ?? string.Empty), user.UserName)); }

						CacheMapId cacheMapId = new CacheMapId(user.UserName, user.Domain);
						CachedUser cachedUser = new CachedUser(user);
						this.cachedMap.Add(cacheMapId, cachedUser);
						this.cachedUsers.Add(user);
					}
				}
			}
		}

		protected void AddUsersToCache(UserCollection users){
			if(users != null && this.MinutesInCache > 0){
				foreach(User user in users){
					this.AddUserToCache(user);
				}
			}
		}
		#endregion

		#region Private Methods
		private void RemoveUserFromCache(User user, CacheMapId cacheMapId){
			if(user != null){
				lock(this.cachedUsers.SyncRoot){
					lock(this.cachedMap.SyncRoot){
						if(log.IsDebugEnabled) { log.Debug(string.Format("Remove user [{0}/{1}] from cache.", (cacheMapId.Domain ?? string.Empty), cacheMapId.UserName)); }
						this.cachedUsers.Remove(user);
						this.cachedMap.Remove(cacheMapId);
					}
				}
			}
		}

		private User GetUserFromCacheMap(string userId){
			lock(this.cachedUsers.SyncRoot){
				if(!this.cachedUsers.Contains(userId))
					return null;
				User user = this.cachedUsers[userId];
				CacheMapId cacheMapId = new CacheMapId(user.UserName, user.Domain);
				return GetUserFromCacheMap(cacheMapId);
			}
		}

		private User GetUserFromCacheMap(string userName, string domain){
			lock(this.cachedUsers.SyncRoot){
				CacheMapId cacheMapId = new CacheMapId(userName, domain);
				return GetUserFromCacheMap(cacheMapId);
			}
		}

		private User GetUserFromCacheMap(CacheMapId cacheMapId){
			lock(this.cachedMap.SyncRoot){
				if(!this.cachedMap.Contains(cacheMapId))
					return null;
				
				CachedUser cachedUser = (CachedUser)this.cachedMap[cacheMapId];
				
				// Remove user from cache if the cache time has expired
				if (cachedUser.DateCreated > DateTime.Now.AddMinutes(this.MinutesInCache)){
					this.RemoveUserFromCache(cachedUser.User, cacheMapId);
					return null;
				}
				if(log.IsDebugEnabled) { log.Debug(string.Format("Get user [{0}/{1}] from cache.", (cacheMapId.Domain ?? string.Empty), cacheMapId.UserName)); }
				return cachedUser.User;
			}
		}

		private static CacheMapId GetCacheMapId(User user){
			return GetCacheMapId(user.UserName, user.Domain);
		}

		private static CacheMapId GetCacheMapId(string userName, string domain){
			return new CacheMapId(userName, domain);
		}

		private void TruncateCache(){
			if(log.IsDebugEnabled) { log.Debug("Truncating user cache"); }

			ArrayList truncateList = new ArrayList(this.cachedMap);
			truncateList.Sort(CachedUserComparer.Instance);
			int itemsToRemove =  ((this.MaxUsersInCache * CacheTruncationFactor) / 100) + (truncateList.Count - this.MaxUsersInCache);
			DateTime cacheTime = DateTime.Now.AddMinutes(this.MinutesInCache);
			
			// Remove users from cahce, remove atleast a factor of [CacheTruncationFactor] from the cache
			// If not enough users has a cache time that has expired the oldest cached objects will be removed
			foreach(DictionaryEntry dicEntry in truncateList){
				CachedUser cachedUser = (CachedUser)dicEntry.Value;
				if(cachedUser.User != null){
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
			private static readonly CachedUserComparer	instance = new CachedUserComparer();
			#endregion

			#region Constructors
			private CachedUserComparer() {}
			#endregion

			#region Singelton Instance
			public static CachedUserComparer Instance{
				get{
					return instance;
				}
			}
			#endregion

			#region IComparer Members
			public int Compare(object x, object y) {
				DictionaryEntry xEntry = (DictionaryEntry)x;
				DictionaryEntry yEntry = (DictionaryEntry)y;

				if (xEntry.Value == null || yEntry.Value == null)
					return 0;

				CachedUser xCachedUser = (CachedUser)xEntry.Value;
				CachedUser yCachedUser = (CachedUser)yEntry.Value;

				if (xCachedUser.User == null || yCachedUser.User == null)
					return 0;

				return xCachedUser.DateCreated.CompareTo(yCachedUser.DateCreated);
			}
			#endregion
		}
		#endregion

		#region Private Structs
		private struct CachedUser{
			
			public readonly User		User;
			public readonly DateTime	DateCreated;
			
			public CachedUser(User user){
				User		= user;
				DateCreated	= DateTime.Now;
			}
		}

		private struct CacheMapId{
			
			public	readonly string	UserName;
			public	readonly string	Domain;
			private	readonly int	hash;
			
			public CacheMapId(string userName, string domain){
				UserName	= userName;
				Domain		= domain ?? string.Empty;

				// Calculate a unique hash that will match all id:s with the same user name and domain
				int result = 17;
				result = (37*result) + UserName.GetHashCode();
				result = (37*result) + Domain.GetHashCode();

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
