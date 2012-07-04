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
using nJupiter.DataAccess.Users.Caching;

namespace nJupiter.DataAccess.Users {
	
	public abstract class UserRepositoryBase : IUserRepository {

		protected readonly string DefaultDomain = string.Empty;

		private readonly string name;
		private readonly IConfig config;
		private readonly IPredefinedNames predefinedNames;
		private readonly IUserCache cache;

		protected UserRepositoryBase() {}

		protected UserRepositoryBase(string name, IConfig config, IPredefinedNames predefinedNames, IUserCache cache) {
			if(name == null) {
				throw new ArgumentNullException("name");
			}
			if(config == null) {
				throw new ArgumentNullException("config");
			}
			this.name = name;
			this.config = config;
			this.predefinedNames = predefinedNames;
			this.cache = cache;
		}

		public virtual string Name { get { return name; } }
		public virtual IPredefinedNames PropertyNames { get { return predefinedNames; } }
		public virtual IConfig Config { get { return config; } }
		public virtual IUserCache UserCache { get { return cache; } }

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
		public abstract void SaveProperties(IUser user, IPropertyCollection propertyCollection);
		public abstract void DeleteUser(IUser user);
		public abstract IPropertyCollection GetProperties();
		public abstract IPropertyCollection GetProperties(IContext context);
		public abstract IPropertyCollection GetProperties(IUser user, IContext context);
		public abstract IEnumerable<IContext> GetContexts();
		public abstract IContext GetContext(string contextName);
		public abstract IContext CreateContext(string contextName, ContextSchema schemaTable);
		public abstract void DeleteContext(IContext context);
		public abstract ContextSchema GetDefaultContextSchema();

		public IUser GetUserByUserName(string userName) {
			return GetUserByUserName(userName, false);
		}

		public IUser GetUserByUserName(string userName, bool loadAllContexts) {
			return this.GetUserByUserName(userName, this.DefaultDomain, loadAllContexts);
		}

		public IUser CreateUserInstance(string userName) {
			return CreateUserInstance(userName, false);
		}

		public IUser CreateUserInstance(string userName, bool loadAllContexts) {
			return CreateUserInstance(userName, this.DefaultDomain, loadAllContexts);
		}

		public virtual IUser GetUserById(string userId, bool loadAllContexts) {
			var user = this.GetUserById(userId);
			if(loadAllContexts){
				this.LoadAndAttachAllContextsForUser(user);
			}
			return user;
		}

		public virtual IUser GetUserByUserName(string userName, string domain, bool loadAllContexts) {
			var user = this.GetUserByUserName(userName, domain);
			if(loadAllContexts){
				this.LoadAndAttachAllContextsForUser(user);
			}
			return user;
		}

		public virtual IList<IUser> GetUsersBySearchCriteria(IEnumerable<SearchCriteria> searchCriteriaCollection, bool loadAllContexts) {
			var users = GetUsersBySearchCriteria(searchCriteriaCollection);
			if(loadAllContexts){
				this.LoadAndAttachAllContextsForUsers(users);
			}
			return users;
		}

		public virtual IList<IUser> GetUsersByDomain(string domain, bool loadAllContexts) {
			var users = GetUsersByDomain(domain);
			if(loadAllContexts){
				this.LoadAndAttachAllContextsForUsers(users);
			}
			return users;
		}

		public virtual IList<IUser> GetAllUsers(int pageIndex, int pageSize, out int totalRecords) {
			var users = this.GetUsersBySearchCriteria((SearchCriteria)null);
			totalRecords = users.Count;
			var pagedUsers = new List<IUser>();
			for(int i = pageIndex * pageSize; (i < ((pageIndex * pageSize) + pageSize)) && (i < users.Count); i++) {
				pagedUsers.Add(users[i]);
			}
			return pagedUsers;
		}

		public virtual IUser CreateUserInstance(string userName, string domain, bool loadAllContexts) {
			var user = this.CreateUserInstance(userName, domain);
			if(loadAllContexts){
				this.LoadAndAttachAllContextsForUser(user);
			}
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
			var users = GetUsersBySearchCriteria(searchCriteria);
			if(loadAllContexts){
				this.LoadAndAttachAllContextsForUsers(users);
			}
			return users;
		}

		protected void LoadAndAttachAllContextsForUser(IUser user) {
			if(user != null){
				foreach(var context in this.GetContexts()) {
					if(!user.Properties.AttachedContexts.Contains(context)){
						var properties = this.GetProperties(user, context);
						user.Properties.AttachProperties(properties);
					}
				}
			}
		}

		protected void LoadAndAttachAllContextsForUsers(IList<IUser> users) {
			foreach(var user in users) {
				this.LoadAndAttachAllContextsForUser(user);
			}
		}

	}
}
