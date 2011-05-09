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
using System.Reflection;
using System.Globalization;

using nJupiter.Configuration;

namespace nJupiter.DataAccess.Users {

	public abstract class UsersDAO {

		#region Constants
		private const string UsersDAOSection = "userDAOs/userDAO";
		private const string UsersDAODefaultSection = UsersDAOSection + "[@default='true']";
		private const string UsersDAOSectionFormat = UsersDAOSection + "[@value='{0}']";
		private const string AssemblyPath = "assemblyPath";
		private const string Assembly = "assembly";
		private const string Type = "type";
		private const string Cache = "cache";
		#endregion

		#region Variables
		private static readonly Hashtable UsersDAOs = Hashtable.Synchronized(new Hashtable());

		private string name;
		private IConfig config;
		private CommonPropertyNames commonPropertyNames;
		private IUserCache userCache;
		#endregion

		#region Properties
		/// <summary>
		/// The name of the current UsersDAO instance.
		/// </summary>
		/// <value>The UsersDAO name.</value>
		public string Name { get { return this.name; } }
		/// <summary>
		/// Gets the config section that is associated with the current UsersDAO instance.
		/// </summary>
		/// <value>The config section that is associated with the current UsersDAO instance.</value>
		public IConfig Config { get { return this.config; } }
		/// <summary>
		/// Gets the common property names that is associated with the current UsersDAO instance.
		/// </summary>
		/// <value>The common property names that is associated with the current UsersDAO instance.</value>
		public CommonPropertyNames PropertyNames { get { return this.commonPropertyNames; } }

		protected IUserCache UserCache { get { return this.userCache; } }
		#endregion

		#region Methods
		/// <summary>
		/// Gets the default UsersDAO instance.
		/// </summary>
		/// <returns>The default UsersDAO instance.</returns>
		public static UsersDAO GetInstance() {
			return GetUserDAOFromSection(UsersDAODefaultSection);
		}

		/// <summary>
		/// Gets the UsersDAO instance with the name <paramref name="name"/>.
		/// </summary>
		/// <param name="name">The UsersDAO name to get.</param>
		/// <returns>The UsersDAO instance with the name <paramref name="name"/></returns>
		public static UsersDAO GetInstance(string name) {
			return GetUserDAOFromSection(string.Format(CultureInfo.InvariantCulture, UsersDAOSectionFormat, name));
		}
		#endregion

		#region Helper Methods
		private static UsersDAO GetUserDAOFromSection(string section) {

			string name = ConfigHandler.GetConfig().GetValue(section);

			if(UsersDAOs.ContainsKey(name))
				return (UsersDAO)UsersDAOs[name];

			lock(UsersDAOs.SyncRoot) {
				if(!UsersDAOs.ContainsKey(name)) {

					IConfig config = ConfigHandler.GetConfig();
					string assemblyPath = config.GetValue(section, AssemblyPath);
					string assemblyName = config.GetValue(section, Assembly);
					string assemblyType = config.GetValue(section, Type);

					object instance = CreateInstance(assemblyPath, assemblyName, assemblyType);
					UsersDAO userDAO = (UsersDAO)instance;
					if(userDAO == null)
						throw new ConfigurationException(string.Format("Could not load DataSource from {0} {1} {2}.", assemblyName, assemblyType, assemblyPath));

					userDAO.name = name;
					userDAO.config = ConfigHandler.GetConfig().GetConfigSection(section + "/settings");
					if(userDAO.config != null && userDAO.config.ContainsKey(Cache)) {
						if(userDAO.config.ContainsKey(Cache, Assembly)) {
							string cacheAssemblyName = userDAO.config.GetValue(Cache, Assembly);
							string cacheAssemblyPath = userDAO.config.GetValue(Cache, AssemblyPath);
							string cacheAssemblyType = userDAO.config.GetValue(Cache, Type);
							object[] constructorArgs = { userDAO };
							userDAO.userCache = CreateInstance(cacheAssemblyPath, cacheAssemblyName, cacheAssemblyType, constructorArgs) as IUserCache;
						}
					}
					if(userDAO.userCache == null) {
						userDAO.userCache = new GenericUserCache(userDAO);
					}

					userDAO.commonPropertyNames = PopulateCommonPropertyNames(userDAO.config);

					UsersDAOs.Add(name, instance);
					return userDAO;
				}
				return (UsersDAO)UsersDAOs[name];
			}
		}

		private static object CreateInstance(string assemblyPath, string assemblyName, string typeName) {
			return CreateInstance(assemblyPath, assemblyName, typeName, null);
		}

		private static object CreateInstance(string assemblyPath, string assemblyName, string typeName, object[] constructorArgs) {
			Assembly assembly;
			if(!string.IsNullOrEmpty(assemblyPath)) {
				assembly = System.Reflection.Assembly.LoadFrom(assemblyPath);
			} else if(assemblyName == null || assemblyName.Length.Equals(0) ||
				System.Reflection.Assembly.GetExecutingAssembly().GetName().Name.Equals(assemblyName)) {
				assembly = System.Reflection.Assembly.GetExecutingAssembly();	//Load current assembly
			} else {
				assembly = System.Reflection.Assembly.Load(assemblyName); // Late binding to an assembly on disk (current directory)
			}
			return assembly.CreateInstance(
				typeName, false,
				BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly |
				BindingFlags.Instance | BindingFlags.IgnoreCase,
				null, constructorArgs, null, null);
		}

		private static CommonPropertyNames PopulateCommonPropertyNames(IConfig config) {

			CommonPropertyNames cpn = new CommonPropertyNames(
																GetCommonPropertyKey("userName", config),
																GetCommonContextAttribute("userName", config),
																GetCommonPropertyKey("fullName", config),
																GetCommonContextAttribute("fullName", config),
																GetCommonPropertyKey("firstName", config),
																GetCommonContextAttribute("firstName", config),
																GetCommonPropertyKey("lastName", config),
																GetCommonContextAttribute("lastName", config),
																GetCommonPropertyKey("description", config),
																GetCommonContextAttribute("description", config),
																GetCommonPropertyKey("email", config),
																GetCommonContextAttribute("email", config),
																GetCommonPropertyKey("homePage", config),
																GetCommonContextAttribute("homePage", config),
																GetCommonPropertyKey("streetAddress", config),
																GetCommonContextAttribute("streetAddress", config),
																GetCommonPropertyKey("company", config),
																GetCommonContextAttribute("company", config),
																GetCommonPropertyKey("department", config),
																GetCommonContextAttribute("department", config),
																GetCommonPropertyKey("city", config),
																GetCommonContextAttribute("city", config),
																GetCommonPropertyKey("telephone", config),
																GetCommonContextAttribute("telephone", config),
																GetCommonPropertyKey("fax", config),
																GetCommonContextAttribute("fax", config),
																GetCommonPropertyKey("homeTelephone", config),
																GetCommonContextAttribute("homeTelephone", config),
																GetCommonPropertyKey("mobileTelephone", config),
																GetCommonContextAttribute("mobileTelephone", config),
																GetCommonPropertyKey("postOfficeBox", config),
																GetCommonContextAttribute("postOfficeBox", config),
																GetCommonPropertyKey("postalCode", config),
																GetCommonContextAttribute("postalCode", config),
																GetCommonPropertyKey("country", config),
																GetCommonContextAttribute("country", config),
																GetCommonPropertyKey("title", config),
																GetCommonContextAttribute("title", config),
																GetCommonPropertyKey("active", config),
																GetCommonContextAttribute("active", config),
																GetCommonPropertyKey("passwordQuestion", config),
																GetCommonContextAttribute("passwordQuestion", config),
																GetCommonPropertyKey("passwordAnswer", config),
																GetCommonContextAttribute("passwordAnswer", config),
																GetCommonPropertyKey("lastActivityDate", config),
																GetCommonContextAttribute("lastActivityDate", config),
																GetCommonPropertyKey("creationDate", config),
																GetCommonContextAttribute("creationDate", config),
																GetCommonPropertyKey("lastLockoutDate", config),
																GetCommonContextAttribute("lastLockoutDate", config),
																GetCommonPropertyKey("lastLoginDate", config),
																GetCommonContextAttribute("lastLoginDate", config),
																GetCommonPropertyKey("lastPasswordChangedDate", config),
																GetCommonContextAttribute("lastPasswordChangedDate", config),
																GetCommonPropertyKey("locked", config),
																GetCommonContextAttribute("locked", config),
																GetCommonPropertyKey("lastUpdatedDate", config),
																GetCommonContextAttribute("lastUpdatedDate", config),
																GetCommonPropertyKey("isAnonymous", config),
																GetCommonContextAttribute("isAnonymous", config),
																GetCommonPropertyKey("password", config),
																GetCommonContextAttribute("password", config),
																GetCommonPropertyKey("passwordSalt", config),
																GetCommonContextAttribute("passwordSalt", config));
			return cpn;
		}

		private static string GetCommonPropertyKey(string property, IConfig config) {
			string result = null;
			if(config.ContainsKey("commonProperties", property))
				result = config.GetValue("commonProperties", property);
			return result;
		}

		private static string GetCommonContextAttribute(string property, IConfig config) {
			string result = null;
			if(config.ContainsAttribute("commonProperties", property, "context"))
				result = config.GetAttribute("commonProperties", property, "context");
			return result;
		}
		#endregion

		#region Abstract Methods
		/// <summary>
		/// Gets a user by its Id
		/// </summary>
		/// <param name="userId">The Id for the user</param>
		/// <returns>The loaded user object</returns>
		public abstract User GetUserById(string userId);

		/// <summary>
		/// Gets a user for a specified domain by its user name
		/// </summary>
		/// <param name="userName">The user name</param>
		/// <param name="domain">The specified domain</param>
		/// <returns>A user object</returns>
		public abstract User GetUserByUserName(string userName, string domain);

		/// <summary>
		/// Gets a collection of users based on a set of search criteria
		/// </summary>
		/// <param name="searchCriteriaCollection">Search criteria collection</param>
		/// <returns>A collection of user objects</returns>
		public abstract UserCollection GetUsersBySearchCriteria(SearchCriteriaCollection searchCriteriaCollection);

		/// <summary>
		/// Gets a collection of users based on their domain
		/// </summary>
		/// <param name="domain">The domain</param>
		/// <returns>A collection of user objects</returns>
		public abstract UserCollection GetUsersByDomain(string domain);

		/// <summary>
		/// Gets all existing domains
		/// </summary>
		/// <returns>An array of domains</returns>
		public abstract string[] GetDomains();

		/// <summary>
		/// Creates a new user
		/// </summary>
		/// <param name="userName">The user name for the new user</param>
		/// <param name="domain">The specified domain</param>
		/// <returns>A user object with the new use</returns>
		/// <remarks>This method does not save any information to the database, use SaveUser to save user to the database.</remarks>
		public abstract User CreateUserInstance(string userName, string domain);

		/// <summary>
		/// Saves a user
		/// </summary>
		/// <param name="user">The user object to save</param>
		public abstract void SaveUser(User user);

		/// <summary>
		/// Saves a collection of users
		/// </summary>
		/// <param name="users">The collection of users to save</param>
		public abstract void SaveUsers(UserCollection users);


		/// <summary>
		/// Set password for user
		/// </summary>
		/// <param name="user">The user to change password for</param>
		/// <param name="password">The new password</param>
		public abstract void SetPassword(User user, string password);

		/// <summary>
		/// Check the password for a user
		/// </summary>
		/// <param name="user">The user to check password for</param>
		/// <param name="password">The password</param>
		/// <returns></returns>
		public abstract bool CheckPassword(User user, string password);

		/// <summary>
		/// Saves properties for a specific user
		/// </summary>
		/// <param name="user">The specified user</param>
		/// <param name="propertyCollection">PropertyCollection containing the properties</param>
		public abstract void SaveProperties(User user, PropertyCollection propertyCollection);

		/// <summary>
		/// Deletes a user
		/// </summary>
		/// <param name="user">The specified user</param>
		public abstract void DeleteUser(User user);

		/// <summary>
		/// Gets an collection with all properties empty
		/// </summary>
		/// <returns>A collection of properties</returns>
		public abstract PropertyCollection GetProperties();

		/// <summary>
		/// Gets a collection of empty properties depending on context
		/// </summary>
		/// <param name="context">The specified context</param>
		/// <returns>A collection of properties</returns>
		public abstract PropertyCollection GetProperties(Context context);

		/// <summary>
		/// Returns a collection of available contexts
		/// </summary>
		/// <returns>A collection containing all available contexts</returns>
		public abstract ContextCollection GetContexts();


		/// <summary>
		/// Get a specific context on name
		/// </summary>
		/// <param name="contextName">The name of the context</param>
		/// <returns>A context</returns>
		public abstract Context GetContext(string contextName);

		/// <summary>
		/// Create a context
		/// </summary>
		/// <param name="contextName">Context name</param>
		/// <param name="schemaTable">Schema table for the context</param>
		/// <returns></returns>
		public abstract Context CreateContext(string contextName, PropertySchemaTable schemaTable);

		/// <summary>
		/// Delete a context permanently
		/// </summary>
		/// <param name="context">The context to delete</param>
		public abstract void DeleteContext(Context context);

		/// <summary>
		/// Gets definitions for all properties
		/// </summary>
		/// <returns>PropertySchemaTable</returns>
		public abstract PropertySchemaTable GetPropertySchemas();
		#endregion

		#region Virtual Methods
		/// <summary>
		/// Gets a collection of all the users in the data source in pages of data.
		/// </summary>
		/// <param name="pageIndex">The index of the page of results to return. <paramref name="pageIndex"/> is zero-based.</param>
		/// <param name="pageSize">The size of the page of results to return.</param>
		/// <param name="totalRecords">The total number of matched users.</param>
		/// <returns>
		/// A <see cref="T:nJupiter.DataAccess.Users.UserCollection"/> collection that contains a page of <paramref name="pageSize"/><see cref="T:nJupiter.DataAccess.Users.User"/> objects beginning at the page specified by <paramref name="pageIndex"/>.
		/// </returns>
		public virtual UserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords) {
			UserCollection uc = this.GetUsersBySearchCriteria((SearchCriteria)null);
			totalRecords = uc.Count;
			UserCollection users = new UserCollection();
			for(int i = pageIndex * pageSize; (i < ((pageIndex * pageSize) + pageSize)) && (i < uc.Count); i++) {
				users.Add(uc[i]);
			}
			return users;
		}

		/// <summary>
		/// Gets a user by its Id
		/// </summary>
		/// <param name="userId">The Id for the user</param>
		/// <param name="loadAllContexts">If true, at the same time also load all properties for all contexts</param>
		/// <returns>The loaded user object</returns>
		public virtual User GetUserById(string userId, bool loadAllContexts) {
			User user = this.GetUserById(userId);
			if(loadAllContexts && user != null)
				this.LoadAllContextsForUser(user);
			return user;
		}

		/// <summary>
		/// Gets a user for a specified domain by its user name
		/// </summary>
		/// <param name="userName">The user name</param>
		/// <param name="domain">The specified domain</param>
		/// <param name="loadAllContexts">If true, at the same time also load all properties for all contexts</param>
		/// <returns>A user object</returns>
		public virtual User GetUserByUserName(string userName, string domain, bool loadAllContexts) {
			User user = this.GetUserByUserName(userName, domain);
			if(loadAllContexts && user != null)
				this.LoadAllContextsForUser(user);
			return user;
		}

		/// <summary>
		/// Gets a collection of users based on a set of search criteria
		/// </summary>
		/// <param name="searchCriteriaCollection">Search criteria collection</param>
		/// <param name="loadAllContexts">If true, at the same time also load all properties for all contexts</param>
		/// <returns>A collection of user objects</returns>
		public virtual UserCollection GetUsersBySearchCriteria(SearchCriteriaCollection searchCriteriaCollection, bool loadAllContexts) {
			if(loadAllContexts)
				return this.LoadAllContextsForUsers(GetUsersBySearchCriteria(searchCriteriaCollection));
			return this.GetUsersBySearchCriteria(searchCriteriaCollection);
		}

		/// <summary>
		/// Gets a collection of users based on their domain
		/// </summary>
		/// <param name="domain">The domain</param>
		/// <param name="loadAllContexts">If true, at the same time also load all properties for all contexts</param>
		/// <returns>A collection of user objects</returns>
		public virtual UserCollection GetUsersByDomain(string domain, bool loadAllContexts) {
			if(loadAllContexts)
				return this.LoadAllContextsForUsers(GetUsersByDomain(domain));
			return this.GetUsersByDomain(domain);
		}

		/// <summary>
		/// Creates a new user
		/// </summary>
		/// <param name="userName">The user name for the new use</param>
		/// <param name="domain">The specified domain</param>
		/// <param name="loadAllContexts">If true, at the same time also load all properties for all contexts</param>
		/// <returns>A user object with the new use</returns>
		/// <remarks>This method does not save any information to the data store, use SaveUser to save user to the data store.</remarks>
		public virtual User CreateUserInstance(string userName, string domain, bool loadAllContexts) {
			User user = this.CreateUserInstance(userName, domain);
			if(loadAllContexts && user != null)
				this.LoadAllContextsForUser(user);
			return user;
		}

		/// <summary>
		/// Gets a collection of users based on search criteria
		/// </summary>
		/// <param name="searchCriteria">Search criteria object</param>
		/// <returns>A collection of user objects</returns>
		public virtual UserCollection GetUsersBySearchCriteria(SearchCriteria searchCriteria) {
			SearchCriteriaCollection sc = new SearchCriteriaCollection();
			if(searchCriteria != null) {
				sc.Add(searchCriteria);
			}
			return GetUsersBySearchCriteria(sc);
		}

		/// <summary>
		/// Gets a collection of users based on search criteria
		/// </summary>
		/// <param name="searchCriteria">Search criteria object</param>
		/// <param name="loadAllContexts">If true, at the same time also load all properties for all contexts</param>
		/// <returns>A collection of user objects</returns>
		public virtual UserCollection GetUsersBySearchCriteria(SearchCriteria searchCriteria, bool loadAllContexts) {
			if(loadAllContexts)
				return this.LoadAllContextsForUsers(GetUsersBySearchCriteria(searchCriteria));
			return this.GetUsersBySearchCriteria(searchCriteria);
		}

		/// <summary>
		/// Gets context specific properties for a specific user
		/// </summary>
		/// <param name="user">The specified user</param>
		/// <param name="context">The specified context</param>
		/// <returns>A collection of properties</returns>
		public virtual PropertyCollection GetProperties(User user, Context context) {
			if(user == null)
				throw new ArgumentNullException("user");
			if(context == null)
				return user.GetProperties();
			return user.GetProperties(context);
		}

		/// <summary>
		/// Load context specific properties for a specific user if it is not already loaded
		/// </summary>
		/// <param name="user">The specified user</param>
		/// <param name="context">The specified context to load intro the user</param>
		/// <returns>Return a property collection if properties for the context exists, else returns null</returns>
		public virtual PropertyCollection LoadProperties(User user, Context context) {
			if(user == null)
				throw new ArgumentNullException("user");
			if(context == null)
				throw new ArgumentNullException("context");
			if(!user.ContainsPropertiesForContext(context)) {
				PropertyCollection pc = this.GetProperties(user, context);
				if(pc != null)
					AttachPropertiesToUser(user, pc);
				return pc;
			}
			return user.GetProperties(context);
		}

		/// <summary>
		/// Unloads context specific properties for a specific user
		/// </summary>
		/// <param name="user">The user.</param>
		/// <param name="context">The context.</param>
		public virtual void UnloadProperties(User user, Context context) {
			if(user == null)
				throw new ArgumentNullException("user");
			if(context == null)
				throw new ArgumentNullException("context");
			user.UnattachProperties(context);
		}
		#endregion

		#region Template Methods
		/// <summary>
		/// Gets a user for the current domain by its user name
		/// </summary>
		/// <param name="userName">The userName</param>
		/// <returns>A user object</returns>
		public User GetUserByUserName(string userName) {
			return GetUserByUserName(userName, null);
		}

		/// <summary>
		/// Gets a user for the current domain by its user name
		/// </summary>
		/// <param name="userName">The userName</param>
		/// <param name="loadAllContexts">If true, at the same time also load all properties for all contexts</param>
		/// <returns>A user object</returns>
		public User GetUserByUserName(string userName, bool loadAllContexts) {
			return this.GetUserByUserName(userName, null, loadAllContexts);
		}

		/// <summary>
		/// Creates a new user with blank password for current domain.
		/// </summary>
		/// <param name="userName">The user name for the new user</param>
		/// <returns>A user object with the new user</returns>
		/// <remarks>This method does not save any information to the data store, use SaveUser to save user to the data store.</remarks>
		public User CreateUserInstance(string userName) {
			return CreateUserInstance(userName, null);
		}

		/// <summary>
		/// Creates a new user with blank password for current domain.
		/// </summary>
		/// <param name="userName">The user name for the new user</param>
		/// <param name="loadAllContexts">If true, at the same time also load all properties for all contexts</param>
		/// <returns>A user object with the new user</returns>
		/// <remarks>This method does not save any information to the data store, use SaveUser to save user to the data store.</remarks>
		public User CreateUserInstance(string userName, bool loadAllContexts) {
			return CreateUserInstance(userName, null, loadAllContexts);
		}

		/// <summary>
		/// Gets global properties for a specific user
		/// </summary>
		/// <param name="user">The specified user</param>
		/// <returns>A collection of properties</returns>
		public PropertyCollection GetProperties(User user) {
			if(user == null)
				throw new ArgumentNullException("user");
			return user.GetProperties();
		}
		#endregion

		#region Protected Methods
		/// <summary>
		/// Creates a property collection
		/// </summary>
		/// <param name="propertySchemaTable">Schema bound to the collection</param>
		/// <returns>A property collection</returns>
		protected static PropertyCollection CreatePropertyCollectionInstance(PropertySchemaTable propertySchemaTable) {
			return new PropertyCollection(propertySchemaTable);
		}

		/// <summary>
		/// Adds a property to a property collection
		/// </summary>
		/// <param name="property">Property to add</param>
		/// <param name="propertyCollection">Destination collection</param>
		protected static void AddPropertyToCollection(AbstractProperty property, PropertyCollection propertyCollection) {
			if(property == null)
				throw new ArgumentNullException("property");
			if(propertyCollection == null)
				throw new ArgumentNullException("propertyCollection");

			propertyCollection.Add(property);
		}

		/// <summary>
		/// Creates a context instance
		/// </summary>
		/// <param name="name">The name of the context</param>
		/// <param name="propertySchemaTable">The property scheme table for the context</param>
		/// <returns>A context object</returns>
		protected static Context CreateContextInstance(string name, PropertySchemaTable propertySchemaTable) {
			return new Context(name, propertySchemaTable);
		}

		/// <summary>
		/// Creates an instance of a context collection
		/// </summary>
		/// <returns>A new context collection</returns>
		protected static ContextCollection CreateContextCollectionInstance() {
			return new ContextCollection();
		}

		/// <summary>
		/// Creates the context collection instance.
		/// </summary>
		/// <param name="contexts">The contexts that the collection shallc ontain.</param>
		/// <returns>A new context collection</returns>
		protected static ContextCollection CreateContextCollectionInstance(IEnumerable<Context> contexts) {
			if(contexts == null) {
				throw new ArgumentNullException("contexts");
			}
			ContextCollection contextCollection = new ContextCollection();
			foreach(Context context in contexts) {
				contextCollection.Add(context);
			}
			return contextCollection;
		}

		/// <summary>
		/// Adds a context to a sepcified context collection
		/// </summary>
		/// <param name="context">Context to add</param>
		/// <param name="contextCollection">The target collection</param>
		protected static void AddContextToCollection(Context context, ContextCollection contextCollection) {
			if(context == null)
				throw new ArgumentNullException("context");
			if(contextCollection == null)
				throw new ArgumentNullException("contextCollection");
			contextCollection.Add(context);
		}

		/// <summary>
		/// Removes a context from a specified context collection
		/// </summary>
		/// <param name="context">Context to remove</param>
		/// <param name="contextCollection">The target context</param>
		protected static void RemoveContextFromCollection(Context context, ContextCollection contextCollection) {
			if(context == null)
				throw new ArgumentNullException("context");
			if(contextCollection == null)
				throw new ArgumentNullException("contextCollection");

			contextCollection.Remove(context);
		}

		/// <summary>
		/// Creates a instance of PropertySchemaTable
		/// </summary>
		/// <returns>A PropertySchemaTable</returns>
		protected static PropertySchemaTable CreatePropertySchemaTableInstance() {
			return new PropertySchemaTable();
		}

		/// <summary>
		/// Creates an property instance of a specified type
		/// </summary>
		/// <param name="propertyName">The name of the property</param>
		/// <param name="serializedPropertyValue">The serilized value of the property</param>
		/// <param name="propertyType">The type of the property</param>
		/// <param name="context">The context that this property belongs to</param>
		/// <returns>The created property</returns>
		protected static AbstractProperty CreatePropertyInstance(string propertyName, string serializedPropertyValue, Type propertyType, Context context) {
			return AbstractProperty.Create(propertyName, serializedPropertyValue, propertyType, context);
		}

		/// <summary>
		/// Adds a property schema to a schema table
		/// </summary>
		/// <param name="propertySchema">Schema to add</param>
		/// <param name="propertySchemaTable">Destination table</param>
		protected static void AddPropertySchemaToTable(PropertySchema propertySchema, PropertySchemaTable propertySchemaTable) {
			if(propertySchema == null)
				throw new ArgumentNullException("propertySchema");
			if(propertySchemaTable == null)
				throw new ArgumentNullException("propertySchemaTable");

			propertySchemaTable.Add(propertySchema);
		}

		/// <summary>
		/// Creates a property schema instance
		/// </summary>
		/// <param name="propertyName">Property name</param>
		/// <param name="dataType">Property type</param>
		/// <returns>A property schema</returns>
		protected static PropertySchema CreatePropertySchemaInstance(string propertyName, Type dataType) {
			if(propertyName == null)
				throw new ArgumentNullException("propertyName");
			if(dataType == null)
				throw new ArgumentNullException("dataType");

			return new PropertySchema(propertyName, dataType);
		}

		/// <summary>
		/// Attach a property collection to a specified user
		/// </summary>
		/// <param name="user">The user</param>
		/// <param name="properties">The property collection to attach</param>
		protected static void AttachPropertiesToUser(User user, PropertyCollection properties) {
			if(user == null)
				throw new ArgumentNullException("user");
			user.AttachProperties(properties);
		}

		/// <summary>
		/// Gets an array of contexts that has been loaded for a specified user
		/// </summary>
		/// <param name="user">The user</param>
		/// <returns>An array of contexts</returns>
		protected static Context[] GetAttachedContextsToUser(User user) {
			if(user == null)
				throw new ArgumentNullException("user");

			return user.AttachedContexts;
		}

		/// <summary>
		/// Load properties for all context for a specified user
		/// </summary>
		/// <param name="user">The specified user</param>
		protected void LoadAllContextsForUser(User user) {
			foreach(Context context in this.GetContexts()) {
				this.LoadProperties(user, context);
			}
		}

		/// <summary>
		/// Load properties for all context for a collection of user
		/// </summary>
		/// <param name="users">The users to load context for</param>
		/// <returns>The user collection with the users with loaded contexts</returns>
		protected UserCollection LoadAllContextsForUsers(UserCollection users) {
			if(users == null)
				throw new ArgumentNullException("users");

			foreach(User user in users) {
				this.LoadAllContextsForUser(user);
			}
			return users;
		}
		#endregion

	}
}
