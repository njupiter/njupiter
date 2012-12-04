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

using nJupiter.Configuration;

namespace nJupiter.DataAccess.Users.Caching {
	public abstract class UserCacheBase : IUserCache {
		protected IConfig Config { get; private set; }

		protected UserCacheBase(IConfig config) {
			if(config == null) {
				throw new ArgumentNullException("config");
			}
			Config = config;
		}

		public abstract IUser GetUserById(string userId);
		public abstract IUser GetUserByUserName(string userName, string domain);
		public abstract void RemoveUserFromCache(IUser user);
		public abstract void RemoveUsersFromCache(IList<IUser> users);
		public abstract void AddUserToCache(IUser user);
		public abstract void AddUsersToCache(IList<IUser> users);
	}
}