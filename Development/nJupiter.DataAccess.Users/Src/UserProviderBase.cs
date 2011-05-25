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
using System.Collections.Generic;
using System.Linq;

using nJupiter.Configuration;

namespace nJupiter.DataAccess.Users {
	
	public abstract class UserProviderBase : IUserProvider {

		public string Name { get; internal set; }
		public IConfig Config { get; internal set; }
		public ICommonNames PropertyNames { get; internal set; }
		protected internal IUserCache UserCache { get; internal set; }

		public abstract IUser GetUserById(string userId);
		public abstract IUser GetUserByUserName(string userName, string domain);
		public abstract IList<IUser> GetUsersBySearchCriteria(IEnumerable<SearchCriteria> searchCriteriaCollection);
		public abstract IList<IUser> GetUsersByDomain(string domain);
		public abstract string[] GetDomains();
		public abstract IUser CreateUserInstance(string userName, string domain);
		public abstract void SaveUser(IUser user);
		public abstract void SaveUsers(IList<IUser> users);
		public abstract void SetPassword(IUser user, string password);
		public abstract bool CheckPassword(IUser user, string password);
		public abstract void SaveProperties(IUser user, PropertyCollection propertyCollection);
		public abstract void DeleteUser(IUser user);
		public abstract PropertyCollection GetProperties();
		public abstract PropertyCollection GetProperties(Context context);
		public abstract IEnumerable<Context> GetContexts();
		public abstract Context GetContext(string contextName);
		public abstract Context CreateContext(string contextName, ContextSchema schemaTable);
		public abstract void DeleteContext(Context context);
		public abstract ContextSchema GetDefaultContextSchema();

		public virtual IList<IUser> GetAllUsers(int pageIndex, int pageSize, out int totalRecords) {
			var uc = this.GetUsersBySearchCriteria((SearchCriteria)null);
			totalRecords = uc.Count;
			var users = new List<IUser>();
			for(int i = pageIndex * pageSize; (i < ((pageIndex * pageSize) + pageSize)) && (i < uc.Count); i++) {
				users.Add(uc[i]);
			}
			return users;
		}

		public virtual IUser GetUserById(string userId, bool loadAllContexts) {
			IUser user = this.GetUserById(userId);
			if(loadAllContexts && user != null)
				this.LoadAllContextsForUser(user);
			return user;
		}

		public virtual IUser GetUserByUserName(string userName, string domain, bool loadAllContexts) {
			IUser user = this.GetUserByUserName(userName, domain);
			if(loadAllContexts && user != null)
				this.LoadAllContextsForUser(user);
			return user;
		}

		public virtual IList<IUser> GetUsersBySearchCriteria(IEnumerable<SearchCriteria> searchCriteriaCollection, bool loadAllContexts) {
			if(loadAllContexts)
				return this.LoadAllContextsForUsers(GetUsersBySearchCriteria(searchCriteriaCollection));
			return this.GetUsersBySearchCriteria(searchCriteriaCollection);
		}

		public virtual IList<IUser> GetUsersByDomain(string domain, bool loadAllContexts) {
			if(loadAllContexts)
				return this.LoadAllContextsForUsers(GetUsersByDomain(domain));
			return this.GetUsersByDomain(domain);
		}

		public virtual IUser CreateUserInstance(string userName, string domain, bool loadAllContexts) {
			IUser user = this.CreateUserInstance(userName, domain);
			if(loadAllContexts && user != null)
				this.LoadAllContextsForUser(user);
			return user;
		}

		public virtual IList<IUser> GetUsersBySearchCriteria(SearchCriteria searchCriteria) {
			var sc = new List<SearchCriteria>();
			if(searchCriteria != null) {
				sc.Add(searchCriteria);
			}
			return GetUsersBySearchCriteria(sc);
		}

		public virtual IList<IUser> GetUsersBySearchCriteria(SearchCriteria searchCriteria, bool loadAllContexts) {
			if(loadAllContexts)
				return this.LoadAllContextsForUsers(GetUsersBySearchCriteria(searchCriteria));
			return this.GetUsersBySearchCriteria(searchCriteria);
		}

		public virtual PropertyCollection GetProperties(IUser user, Context context) {
			if(user == null)
				throw new ArgumentNullException("user");
			if(context == null)
				return user.Properties.GetProperties();
			return user.Properties.GetProperties(context);
		}

		public virtual PropertyCollection LoadProperties(IUser user, Context context) {
			if(user == null)
				throw new ArgumentNullException("user");
			if(context == null)
				throw new ArgumentNullException("context");
			if(!user.Properties.AttachedContexts.Contains(context)) {
				var pc = this.GetProperties(user, context);
				if(pc != null) {
					user.Properties.AttachProperties(pc);
				}
				return pc;
			}
			return user.Properties.GetProperties(context);
		}

		public IUser GetUserByUserName(string userName) {
			return GetUserByUserName(userName, null);
		}

		public IUser GetUserByUserName(string userName, bool loadAllContexts) {
			return this.GetUserByUserName(userName, null, loadAllContexts);
		}

		public IUser CreateUserInstance(string userName) {
			return CreateUserInstance(userName, null);
		}

		public IUser CreateUserInstance(string userName, bool loadAllContexts) {
			return CreateUserInstance(userName, null, loadAllContexts);
		}

		protected void LoadAllContextsForUser(IUser user) {
			foreach(Context context in this.GetContexts()) {
				this.LoadProperties(user, context);
			}
		}

		protected IList<IUser> LoadAllContextsForUsers(IList<IUser> users) {
			if(users == null)
				throw new ArgumentNullException("users");

			foreach(IUser user in users) {
				this.LoadAllContextsForUser(user);
			}
			return users;
		}

	}
}
