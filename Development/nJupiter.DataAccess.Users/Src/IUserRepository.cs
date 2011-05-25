using System.Collections.Generic;

namespace nJupiter.DataAccess.Users {
	public interface IUserRepository {
		
        /// <summary>
        /// The name of the current userRepository instance.
        /// </summary>
        /// <value>The userRepository name.</value>
        string Name { get; }

	
		/// <summary>
        /// Gets the common property names that is associated with the current userRepository instance.
        /// </summary>
        /// <value>The common property names that is associated with the current userRepository instance.</value>
		IPredefinedNames PropertyNames { get; }
		
		/// <summary>
		/// Gets a user by its Id
		/// </summary>
		/// <param name="userId">The Id for the user</param>
		/// <returns>The loaded user object</returns>
		IUser GetUserById(string userId);

		/// <summary>
		/// Gets a user for a specified domain by its user name
		/// </summary>
		/// <param name="userName">The user name</param>
		/// <param name="domain">The specified domain</param>
		/// <returns>A user object</returns>
		IUser GetUserByUserName(string userName, string domain);

		/// <summary>
		/// Gets a collection of users based on a set of search criteria
		/// </summary>
		/// <param name="searchCriteriaCollection">Search criteria collection</param>
		/// <returns>A collection of user objects</returns>
		IList<IUser> GetUsersBySearchCriteria(IEnumerable<SearchCriteria> searchCriteriaCollection);

		/// <summary>
		/// Gets a collection of users based on their domain
		/// </summary>
		/// <param name="domain">The domain</param>
		/// <returns>A collection of user objects</returns>
		IList<IUser> GetUsersByDomain(string domain);

		/// <summary>
		/// Gets all existing domains
		/// </summary>
		/// <returns>An array of domains</returns>
		string[] GetDomains();

		/// <summary>
		/// Creates a new user
		/// </summary>
		/// <param name="userName">The user name for the new user</param>
		/// <param name="domain">The specified domain</param>
		/// <returns>A user object with the new use</returns>
		/// <remarks>This method does not save any information to the database, use SaveUser to save user to the database.</remarks>
		IUser CreateUserInstance(string userName, string domain);

		/// <summary>
		/// Saves a user
		/// </summary>
		/// <param name="user">The user object to save</param>
		void SaveUser(IUser user);

		/// <summary>
		/// Saves a collection of users
		/// </summary>
		/// <param name="users">The collection of users to save</param>
		void SaveUsers(IList<IUser> users);

		/// <summary>
		/// Set password for user
		/// </summary>
		/// <param name="user">The user to change password for</param>
		/// <param name="password">The new password</param>
		void SetPassword(IUser user, string password);

		/// <summary>
		/// Check the password for a user
		/// </summary>
		/// <param name="user">The user to check password for</param>
		/// <param name="password">The password</param>
		/// <returns></returns>
		bool CheckPassword(IUser user, string password);

		/// <summary>
		/// Saves properties for a specific user
		/// </summary>
		/// <param name="user">The specified user</param>
		/// <param name="propertyCollection">PropertyCollection containing the properties</param>
		void SaveProperties(IUser user, IPropertyCollection propertyCollection);

		/// <summary>
		/// Deletes a user
		/// </summary>
		/// <param name="user">The specified user</param>
		void DeleteUser(IUser user);

		/// <summary>
		/// Gets an collection with all properties empty
		/// </summary>
		/// <returns>A collection of properties</returns>
		IPropertyCollection GetProperties();

		/// <summary>
		/// Gets a collection of empty properties depending on context
		/// </summary>
		/// <param name="context">The specified context</param>
		/// <returns>A collection of properties</returns>
		IPropertyCollection GetProperties(IContext context);

		/// <summary>
		/// Returns a collection of available contexts
		/// </summary>
		/// <returns>A collection containing all available contexts</returns>
		IEnumerable<IContext> GetContexts();

		/// <summary>
		/// Get a specific context on name
		/// </summary>
		/// <param name="contextName">The name of the context</param>
		/// <returns>A context</returns>
		IContext GetContext(string contextName);

		/// <summary>
		/// Create a context
		/// </summary>
		/// <param name="contextName">Context name</param>
		/// <param name="schemaTable">Schema table for the context</param>
		/// <returns></returns>
		IContext CreateContext(string contextName, ContextSchema schemaTable);

		/// <summary>
		/// Delete a context permanently
		/// </summary>
		/// <param name="context">The context to delete</param>
		void DeleteContext(IContext context);

		/// <summary>
		/// Gets definitions for all properties
		/// </summary>
		/// <returns>Schema</returns>
		ContextSchema GetDefaultContextSchema();

		/// <summary>
		/// Gets a collection of all the users in the data source in pages of data.
		/// </summary>
		/// <param name="pageIndex">The index of the page of results to return. <paramref name="pageIndex"/> is zero-based.</param>
		/// <param name="pageSize">The size of the page of results to return.</param>
		/// <param name="totalRecords">The total number of matched users.</param>
		/// <returns>
		/// A <see cref="T:nJupiter.DataAccess.Users.UserCollection"/> collection that contains a page of <paramref name="pageSize"/><see cref="T:nJupiter.DataAccess.Users.User"/> objects beginning at the page specified by <paramref name="pageIndex"/>.
		/// </returns>
		IList<IUser> GetAllUsers(int pageIndex, int pageSize, out int totalRecords);

		/// <summary>
		/// Gets a user by its Id
		/// </summary>
		/// <param name="userId">The Id for the user</param>
		/// <param name="loadAllContexts">If true, at the same time also load all properties for all contexts</param>
		/// <returns>The loaded user object</returns>
		IUser GetUserById(string userId, bool loadAllContexts);

		/// <summary>
		/// Gets a user for a specified domain by its user name
		/// </summary>
		/// <param name="userName">The user name</param>
		/// <param name="domain">The specified domain</param>
		/// <param name="loadAllContexts">If true, at the same time also load all properties for all contexts</param>
		/// <returns>A user object</returns>
		IUser GetUserByUserName(string userName, string domain, bool loadAllContexts);

		/// <summary>
		/// Gets a collection of users based on a set of search criteria
		/// </summary>
		/// <param name="searchCriteriaCollection">Search criteria collection</param>
		/// <param name="loadAllContexts">If true, at the same time also load all properties for all contexts</param>
		/// <returns>A collection of user objects</returns>
		IList<IUser> GetUsersBySearchCriteria(IEnumerable<SearchCriteria> searchCriteriaCollection, bool loadAllContexts);

		/// <summary>
		/// Gets a collection of users based on their domain
		/// </summary>
		/// <param name="domain">The domain</param>
		/// <param name="loadAllContexts">If true, at the same time also load all properties for all contexts</param>
		/// <returns>A collection of user objects</returns>
		IList<IUser> GetUsersByDomain(string domain, bool loadAllContexts);

		/// <summary>
		/// Creates a new user
		/// </summary>
		/// <param name="userName">The user name for the new use</param>
		/// <param name="domain">The specified domain</param>
		/// <param name="loadAllContexts">If true, at the same time also load all properties for all contexts</param>
		/// <returns>A user object with the new use</returns>
		/// <remarks>This method does not save any information to the data store, use SaveUser to save user to the data store.</remarks>
		IUser CreateUserInstance(string userName, string domain, bool loadAllContexts);

		/// <summary>
		/// Gets a collection of users based on search criteria
		/// </summary>
		/// <param name="searchCriteria">Search criteria object</param>
		/// <returns>A collection of user objects</returns>
		IList<IUser> GetUsersBySearchCriteria(SearchCriteria searchCriteria);

		/// <summary>
		/// Gets a collection of users based on search criteria
		/// </summary>
		/// <param name="searchCriteria">Search criteria object</param>
		/// <param name="loadAllContexts">If true, at the same time also load all properties for all contexts</param>
		/// <returns>A collection of user objects</returns>
		IList<IUser> GetUsersBySearchCriteria(SearchCriteria searchCriteria, bool loadAllContexts);

		/// <summary>
		/// Load context specific properties for a specific user if it is not already loaded
		/// </summary>
		/// <param name="user">The specified user</param>
		/// <param name="context">The specified context to load intro the user</param>
		/// <returns>Return a property collection if properties for the context exists, else returns null</returns>
		IPropertyCollection GetProperties(IUser user, IContext context);

		/// <summary>
		/// Gets a user for the current domain by its user name
		/// </summary>
		/// <param name="userName">The userName</param>
		/// <returns>A user object</returns>
		IUser GetUserByUserName(string userName);

		/// <summary>
		/// Gets a user for the current domain by its user name
		/// </summary>
		/// <param name="userName">The userName</param>
		/// <param name="loadAllContexts">If true, at the same time also load all properties for all contexts</param>
		/// <returns>A user object</returns>
		IUser GetUserByUserName(string userName, bool loadAllContexts);

		/// <summary>
		/// Creates a new user with blank password for current domain.
		/// </summary>
		/// <param name="userName">The user name for the new user</param>
		/// <returns>A user object with the new user</returns>
		/// <remarks>This method does not save any information to the data store, use SaveUser to save user to the data store.</remarks>
		IUser CreateUserInstance(string userName);

		/// <summary>
		/// Creates a new user with blank password for current domain.
		/// </summary>
		/// <param name="userName">The user name for the new user</param>
		/// <param name="loadAllContexts">If true, at the same time also load all properties for all contexts</param>
		/// <returns>A user object with the new user</returns>
		/// <remarks>This method does not save any information to the data store, use SaveUser to save user to the data store.</remarks>
		IUser CreateUserInstance(string userName, bool loadAllContexts);

	}
}