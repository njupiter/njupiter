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

namespace nJupiter.DataAccess.Users.Caching {
	internal struct UserIdCacheKey {
		private const int InitialPrime = 17;
		private const int MultiplierPrime = 37;

		private readonly string userProvider;
		private readonly string userId;
		private readonly string cacheKey;

		public UserIdCacheKey(string userProvider, string id) {
			userId = id ?? String.Empty;
			this.userProvider = userProvider;

			cacheKey = String.Format("nJupiter.DataAccess.Users.userRepository:{0}:userId:{1}", userProvider, id);
		}

		public override bool Equals(object obj) {
			var map = (UserIdCacheKey)obj;
			if(map.userId == null) {
				return false;
			}
			return map.userId.Equals(userId) && map.userProvider.Equals(userProvider);
		}

		public string CacheKey { get { return cacheKey; } }

		public override int GetHashCode() {
			// Refer to Effective Java 1st ed page 34 for an good explanation of this hash code implementation
			var hash = InitialPrime;
			hash = (MultiplierPrime * hash) + userId.GetHashCode();
			hash = (MultiplierPrime * hash) + userProvider.GetHashCode();
			return hash;
		}
	}
}