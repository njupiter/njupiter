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
	internal struct UsernameCacheKey {
		private readonly string userProvider;
		private readonly string userName;
		private readonly string domain;
		private readonly int hash;
		private readonly string cacheKey;

		public UsernameCacheKey(string userProvider, string userName, string domain) {
			this.userName = userName;
			this.domain = domain ?? String.Empty;
			this.userProvider = userProvider;

			// Calculate a unique hash that will match all id:s with the same user name and domain
			var result = 17;
			result = (37 * result) + this.userName.GetHashCode();
			result = (37 * result) + this.domain.GetHashCode();
			result = (37 * result) + this.userProvider.GetHashCode();

			hash = result;
			cacheKey = String.Format("nJupiter.DataAccess.Users.userRepository:{0}:UsernameCacheKey:{1}", this.userProvider, hash);
		}

		public override bool Equals(object obj) {
			var map = (UsernameCacheKey)obj;
			if(map.userName == null) {
				return false;
			}
			return map.userName.Equals(userName) && map.domain.Equals(domain) && map.userProvider.Equals(userProvider);
		}

		public string CacheKey { get { return cacheKey; } }

		public override int GetHashCode() {
			return hash;
		}
	}
}