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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

using nJupiter.Configuration;
using nJupiter.DataAccess.Users.Caching;

namespace nJupiter.DataAccess.Users.Sql {
	public class UserRepository : UserRepositoryBase {
		#region Constants
		private const int AddStatusUsernameTaken = 1;
		#endregion

		#region Members
		private readonly object padlock = new object();
		private readonly IDictionary<string, ContextSchema> contextSchema = new Dictionary<string, ContextSchema>();
		private IList<IContext> contexts;
		#endregion

		#region Properties
		protected IDataSource CurrentDB { get { return DataSourceFactory.Create(Config.GetValue("dataSource")); } }

		protected string PasswordPropertyName { get { return PropertyNames.Password ?? "password"; } }
		protected string PasswordSaltPropertyName { get { return PropertyNames.Password ?? "passwordSalt"; } }
		#endregion

		#region Overridden Methods
		public UserRepository(string name, IConfig config, IPredefinedNames predefinedNames, IUserCache cache)
			: base(name, config, predefinedNames, cache) {}

		public override IList<IUser> GetAllUsers(int pageIndex, int pageSize, out int totalRecords) {
			var returnParam = CurrentDB.CreateReturnParameter("@intPagingTotalNumber", DbType.Int32);
			DataSet dsUser;
			if(pageSize.Equals(int.MaxValue) || pageIndex.Equals(int.MaxValue)) {
				dsUser = CurrentDB.ExecuteDataSet("dbo.USER_FilterUsers", returnParam);
			} else {
				dsUser = CurrentDB.ExecuteDataSet("dbo.USER_FilterUsers",
				                                  CurrentDB.CreateInputParameter("@intLimitPage", DbType.Int32, pageIndex + 1),
				                                  CurrentDB.CreateInputParameter("@intLimitSize", DbType.Int32, pageSize),
				                                  returnParam);
			}
			var uc = GetUsersFromDataSet(dsUser);
			UserCache.AddUsersToCache(uc);
			totalRecords = (int)returnParam.Value;
			return uc;
		}

		public override IUser GetUserById(string userId) {
			var user = UserCache.GetUserById(userId);
			if(user != null) {
				return user;
			}

			var dsUser = CurrentDB.ExecuteDataSet("dbo.USER_GetByID",
			                                          CurrentDB.CreateInputParameter("@guidUserID", DbType.Guid, new Guid(userId)));
			user = GetUserFromDataSet(dsUser);
			UserCache.AddUserToCache(user);
			return user;
		}

		public override IUser GetUserByUserName(string userName, string domain) {
			var user = UserCache.GetUserByUserName(userName, domain);
			if(user != null) {
				return user;
			}
			var dsUser = CurrentDB.ExecuteDataSet("dbo.USER_GetByUsername",
			                                          CurrentDB.CreateStringInputParameter("@chvnUsername",
			                                                                               DbType.String,
			                                                                               userName),
			                                          CurrentDB.CreateStringInputParameter("@chvDomain",
			                                                                               DbType.AnsiString,
			                                                                               domain ?? string.Empty));
			user = GetUserFromDataSet(dsUser);
			UserCache.AddUserToCache(user);
			return user;
		}

		public override IList<IUser> GetUsersBySearchCriteria(IEnumerable<SearchCriteria> searchCriteriaCollection) {
			if(searchCriteriaCollection == null) {
				throw new ArgumentNullException("searchCriteriaCollection");
			}

			const string sqlSinglequote = "'";
			const string sqlSinglequoteEscaped = "''";
			const string sqlWhere = "WHERE ";
			const string sqlAnd = "AND ";
			const string sqlOr = "OR ";
			const string sqlNot = "NOT ";

			const string sqlBasicQuery = "SELECT u.UserID, u.Username, u.Domain FROM dbo.USER_User u ";
			const string sqlBasicSubqueryStart =
				"EXISTS(SELECT * FROM dbo.USER_Property p JOIN dbo.USER_PropertySchema ps ON p.PropertyID = ps.PropertyID ";
			const string sqlBasicSubqueryEnd = sqlAnd + "u.UserID = p.UserID) ";
			const string sqlContextJoin = "JOIN dbo.USER_Context c ON p.ContextID = c.ContextID ";

			const string sqlCriteriaProperty = "ps.PropertyName = {0} ";
			const string sqlCriteriaContext = sqlAnd + "c.ContextName = {0} ";
			const string sqlCriteriaDomain = sqlAnd + "u.Domain = {0} ";

			const string sqlCriteriaCondNomatch = sqlAnd + "1 = 0 ";

			const string sqlCriteriaCondLike = sqlAnd + "(p.PropertyValue LIKE {0} OR p.ExtendedPropertyValue LIKE {0}) ";
			const string sqlCriteriaCondLikeOnlyext = sqlAnd + "p.ExtendedPropertyValue LIKE {0} ";
			const string sqlCriteriaCondNotlike =
				sqlAnd + "(p.PropertyValue NOT LIKE {0} OR p.ExtendedPropertyValue NOT LIKE {0}) ";
			const string sqlCriteriaCondNotlikeOnlyext = sqlAnd + "p.ExtendedPropertyValue NOT LIKE {0} ";

			const string sqlCriteriaCondEqual = sqlAnd + "p.PropertyValue = {0} ";
			const string sqlCriteriaCondEqualExt = sqlCriteriaCondLikeOnlyext;
			const string sqlCriteriaCondNotequal = sqlAnd + "p.PropertyValue <> {0} ";
			const string sqlCriteriaCondNotequalExt = sqlCriteriaCondNotlikeOnlyext;

			const string sqlCriteriaCondGreater =
				sqlAnd + "(p.PropertyValue > {0} OR CAST(p.ExtendedPropertyValue AS NVARCHAR(4000)) > {0})";
			const string sqlCriteriaCondGreaterequal =
				sqlAnd + "(p.PropertyValue >= {0} OR CAST(p.ExtendedPropertyValue AS NVARCHAR(4000)) >= {0})";
			const string sqlCriteriaCondLess =
				sqlAnd + "(p.PropertyValue < {0} OR CAST(p.ExtendedPropertyValue AS NVARCHAR(4000)) < {0})";
			const string sqlCriteriaCondLessequal =
				sqlAnd + "(p.PropertyValue <= {0} OR CAST(p.ExtendedPropertyValue AS NVARCHAR(4000)) <= {0})";

			const string sqlCriteriaCondContainsstartswith = sqlAnd + @"CONTAINS(p.*,N'""{0}*""') ";

			const string sqlParameter = "@param";

			var command = CurrentDB.CreateTextCommand();
			var queryBuilder = new StringBuilder(sqlBasicQuery);
			var requiredCriterias = new ArrayList();
			var notRequiredCriterias = new ArrayList();
			var parameterCount = 0;

			foreach(var sc in searchCriteriaCollection) {
				var serializedValue = sc.Property.ToSerializedString();
				var likeEscapedSerializedValue = EscapeForLikeClause(serializedValue);
				var isDefaultValue = sc.Property.IsEmpty();

				var parameterNameName = sqlParameter + parameterCount++;
				var parameterNameValue = sqlParameter + parameterCount++;
				var contextIsSet = sc.Property.Context != null && !string.IsNullOrEmpty(sc.Property.Context.Name);
				var domainIsSet = !string.IsNullOrEmpty(sc.Domain);

				var criteria = new StringBuilder(sqlBasicSubqueryStart);
				if(contextIsSet) {
					// Add context criteria to query if context is set
					criteria.Append(sqlContextJoin);
				}
				criteria.Append(sqlWhere).AppendFormat(CultureInfo.InvariantCulture, sqlCriteriaProperty, parameterNameName);
				if(contextIsSet) {
					// Add context criteria to query if context is set
					var parameterContext = sqlParameter + parameterCount++;
					criteria.AppendFormat(CultureInfo.InvariantCulture, sqlCriteriaContext, parameterContext);
					command.AddInParameter(parameterContext, DbType.String, sc.Property.Context.Name);
				}
				if(domainIsSet) {
					// Add domain criteria to query if domain is set
					var parameterDomain = sqlParameter + parameterCount++;
					criteria.AppendFormat(CultureInfo.InvariantCulture, sqlCriteriaDomain, parameterDomain);
					command.AddInParameter(parameterDomain, DbType.String, sc.Domain);
				}
				var basicSubQueryStart = criteria.ToString();
				switch(sc.Condition) {
						// If value is bigger than 4000 bytes, search in the extended value column
					case CompareCondition.ContainsStartsWith:
						if(!isDefaultValue) {
							criteria.AppendFormat(CultureInfo.InvariantCulture,
							                      sqlCriteriaCondContainsstartswith,
							                      serializedValue.Replace(sqlSinglequote, sqlSinglequoteEscaped));
						} else {
							//return all if defaultValue
							criteria.
								Append(sqlBasicSubqueryEnd).Append(sqlOr).Append(sqlNot).
								Append(basicSubQueryStart);
						}
						break;
					case CompareCondition.StartsWith:
						if(!isDefaultValue) {
							criteria.AppendFormat(CultureInfo.InvariantCulture,
							                      serializedValue.Length <= 4000 ? sqlCriteriaCondLike : sqlCriteriaCondLikeOnlyext,
							                      parameterNameValue);
							command.AddInParameter(parameterNameValue, DbType.String, likeEscapedSerializedValue + "%");
						} else {
							//return all if defaultValue
							criteria.
								Append(sqlBasicSubqueryEnd).Append(sqlOr).Append(sqlNot).
								Append(basicSubQueryStart);
						}
						break;
					case CompareCondition.NotStartsWith:
						if(!isDefaultValue) {
							criteria.
								AppendFormat(CultureInfo.InvariantCulture,
								             serializedValue.Length <= 4000 ? sqlCriteriaCondNotlike : sqlCriteriaCondNotlikeOnlyext,
								             parameterNameValue).
								Append(sqlBasicSubqueryEnd).Append(sqlOr).Append(sqlNot).
								Append(basicSubQueryStart);
						} else {
							//return none if defaultValue
							criteria.Append(sqlCriteriaCondNomatch);
						}
						command.AddInParameter(parameterNameValue, DbType.String, likeEscapedSerializedValue + "%");
						break;
					case CompareCondition.EndsWith:
						if(!isDefaultValue) {
							criteria.AppendFormat(CultureInfo.InvariantCulture,
							                      serializedValue.Length <= 4000 ? sqlCriteriaCondLike : sqlCriteriaCondLikeOnlyext,
							                      parameterNameValue);
							command.AddInParameter(parameterNameValue, DbType.String, "%" + likeEscapedSerializedValue);
						} else {
							//return all if defaultValue
							criteria.
								Append(sqlBasicSubqueryEnd).Append(sqlOr).Append(sqlNot).
								Append(basicSubQueryStart);
						}
						break;
					case CompareCondition.NotEndsWith:
						if(!isDefaultValue) {
							criteria.AppendFormat(CultureInfo.InvariantCulture,
							                      serializedValue.Length <= 4000 ? sqlCriteriaCondNotlike : sqlCriteriaCondNotlikeOnlyext,
							                      parameterNameValue).
								Append(sqlBasicSubqueryEnd).Append(sqlOr).Append(sqlNot).
								Append(basicSubQueryStart);
							command.AddInParameter(parameterNameValue, DbType.String, "%" + likeEscapedSerializedValue);
						} else {
							//return none if defaultValue
							criteria.Append(sqlCriteriaCondNomatch);
						}
						break;
					case CompareCondition.Contains:
						if(!isDefaultValue) {
							criteria.AppendFormat(CultureInfo.InvariantCulture,
							                      serializedValue.Length <= 4000 ? sqlCriteriaCondLike : sqlCriteriaCondLikeOnlyext,
							                      parameterNameValue);
							command.AddInParameter(parameterNameValue, DbType.String, "%" + likeEscapedSerializedValue + "%");
						} else {
							//return all if defaultValue
							criteria.
								Append(sqlBasicSubqueryEnd).Append(sqlOr).Append(sqlNot).
								Append(basicSubQueryStart);
						}
						break;
					case CompareCondition.NotContains:
						if(!isDefaultValue) {
							criteria.AppendFormat(CultureInfo.InvariantCulture,
							                      serializedValue.Length <= 4000 ? sqlCriteriaCondNotlike : sqlCriteriaCondNotlikeOnlyext,
							                      parameterNameValue).
								Append(sqlBasicSubqueryEnd).Append(sqlOr).Append(sqlNot).
								Append(basicSubQueryStart);
							command.AddInParameter(parameterNameValue, DbType.String, "%" + likeEscapedSerializedValue + "%");
						} else {
							//return none if defaultValue
							criteria.Append(sqlCriteriaCondNomatch);
						}
						break;
					case CompareCondition.NotEqual:
						if(!isDefaultValue) {
							criteria.AppendFormat(CultureInfo.InvariantCulture,
							                      serializedValue.Length <= 4000 ? sqlCriteriaCondNotequal : sqlCriteriaCondNotequalExt,
							                      parameterNameValue);
							command.AddInParameter(parameterNameValue,
							                       DbType.String,
							                       serializedValue.Length <= 4000 ? serializedValue : likeEscapedSerializedValue);
						}
						//return all that has a row if defaultValue
						break;
					case CompareCondition.GreaterThan:
					case CompareCondition.GreaterThanOrEqual:
					case CompareCondition.LessThan:
					case CompareCondition.LessThanOrEqual:
						if(sc.Property is ISqlProperty && !((ISqlProperty)sc.Property).SerializationPreservesOrder) {
							throw new InvalidOperationException(
								"Can not use inequality comparison on a property that does not maintain sort order of its underlying type in its serialized form.");
						}
						var comparedWithDefaultValue = sc.Property.Value == null
							                               ? sc.Property.DefaultValue == null ? 0 : -1
							                               : ((IComparable)sc.Property.Value).CompareTo(sc.Property.DefaultValue);
						switch(sc.Condition) {
							case CompareCondition.GreaterThan:
								if(comparedWithDefaultValue < 0) { // value > (<defaultValue)
									criteria.Insert(0, sqlNot).AppendFormat(CultureInfo.InvariantCulture,
									                                        sqlCriteriaCondLessequal,
									                                        parameterNameValue);
								} else { // value > (>=defaultValue)
									criteria.AppendFormat(CultureInfo.InvariantCulture, sqlCriteriaCondGreater, parameterNameValue);
								}
								break;
							case CompareCondition.GreaterThanOrEqual:
								if(comparedWithDefaultValue > 0) { //value >= (>defaultValue)
									criteria.AppendFormat(CultureInfo.InvariantCulture, sqlCriteriaCondGreaterequal, parameterNameValue);
								} else { //value >= (<=defaultValue>
									criteria.Insert(0, sqlNot).AppendFormat(CultureInfo.InvariantCulture, sqlCriteriaCondLess, parameterNameValue);
								}
								break;
							case CompareCondition.LessThan:
								if(comparedWithDefaultValue > 0) { //value < (>defaultValue>
									criteria.Insert(0, sqlNot).AppendFormat(CultureInfo.InvariantCulture,
									                                        sqlCriteriaCondGreaterequal,
									                                        parameterNameValue);
								} else { //value < (<=defaultValue)
									criteria.AppendFormat(CultureInfo.InvariantCulture, sqlCriteriaCondLess, parameterNameValue);
								}
								break;
							case CompareCondition.LessThanOrEqual:
								if(comparedWithDefaultValue < 0) { //value <= (<defaultValue)
									criteria.AppendFormat(CultureInfo.InvariantCulture, sqlCriteriaCondLessequal, parameterNameValue);
								} else { //value <= (>=defaultValue>
									criteria.Insert(0, sqlNot).AppendFormat(CultureInfo.InvariantCulture,
									                                        sqlCriteriaCondGreater,
									                                        parameterNameValue);
								}
								break;
						}
						command.AddInParameter(parameterNameValue, DbType.String, serializedValue);
						break;
					default:
						if(!isDefaultValue) {
							criteria.AppendFormat(CultureInfo.InvariantCulture,
							                      serializedValue.Length <= 4000 ? sqlCriteriaCondEqual : sqlCriteriaCondEqualExt,
							                      parameterNameValue);
							command.AddInParameter(parameterNameValue,
							                       DbType.String,
							                       serializedValue.Length <= 4000 ? serializedValue : likeEscapedSerializedValue);
						} else {
							//return all that has no row if defaultValue
							criteria.Insert(0, sqlNot);
						}
						break;
				}
				criteria.Append(sqlBasicSubqueryEnd);
				(sc.Required ? requiredCriterias : notRequiredCriterias).Add(criteria.ToString());
				command.AddInParameter(parameterNameName, DbType.String, sc.Property.Name);
			}

			if(requiredCriterias.Count > 0 || notRequiredCriterias.Count > 0) {
				queryBuilder.Append(sqlWhere);
				// Populate required criterias
				if(requiredCriterias.Count > 0) {
					queryBuilder.Append("(");
					for(var i = 0; i < requiredCriterias.Count; i++) {
						queryBuilder.Append(requiredCriterias[i]);
						if(i != (requiredCriterias.Count - 1)) { // If last criteria, do not add AND
							queryBuilder.Append(sqlAnd);
						}
					}
					queryBuilder.Append(") ");
					if(notRequiredCriterias.Count > 0) {
						queryBuilder.Append(sqlAnd);
					}
				}

				// Populate non-required criterias
				if(notRequiredCriterias.Count > 0) {
					queryBuilder.Append("(");
					for(var i = 0; i < notRequiredCriterias.Count; i++) {
						queryBuilder.Append(notRequiredCriterias[i]);
						if(i != (notRequiredCriterias.Count - 1)) // If last criteria, do not add OR
						{
							queryBuilder.Append(sqlOr);
						}
					}
					queryBuilder.Append(") ");
				}
			}

			command.CommandText = queryBuilder.ToString();
			try {
				var uc = GetUsersFromDataSet(CurrentDB.ExecuteDataSet(command));
				UserCache.AddUsersToCache(uc);
				return uc;
			} catch(SqlException ex) {
				//the execution of a full-text operation failed because
				if(ex.Number.Equals(7603) || //full-text predicate was null or empty (SQL Server 2000)
				   ex.Number.Equals(7619) || //the clause of the query contained only ignored words. (SQL Server 2000)
				   ex.Number.Equals(7643) || //the search generated too many results
				   ex.Number.Equals(7645)) { //full-text predicate was null or empty (SQL Server 2005)
					//return empty user collection
					return new List<IUser>();
				}
				throw;
			}
		}

		public override IList<IUser> GetUsersByDomain(string domain) {
			if(domain == null) {
				domain = string.Empty;
			}
			var dsUser = CurrentDB.ExecuteDataSet("dbo.USER_GetUsersByDomain",
			                                          CurrentDB.CreateStringInputParameter("@chvDomain",
			                                                                               DbType.AnsiString,
			                                                                               domain,
			                                                                               false));
			var uc = GetUsersFromDataSet(dsUser);
			UserCache.AddUsersToCache(uc);
			return uc;
		}

		public override IUser CreateUserInstance(string userName, string domain) {
			if(domain == null) {
				domain = string.Empty;
			}

			var userId = Guid.NewGuid().ToString();
			var user = new User(userId, userName, domain, GetPropertiesByUserId(userId), PropertyNames);
			return user;
		}

		public override void SaveUser(IUser user) {
			using(var transaction = TransactionFactory.BeginTransaction(CurrentDB)) {
				SaveUser(user, transaction);
				transaction.Commit();
			}
		}

		public override void SaveUsers(IList<IUser> users) {
			if(users == null) {
				throw new ArgumentNullException("users");
			}

			using(var transaction = TransactionFactory.BeginTransaction(CurrentDB)) {
				foreach(var user in users) {
					SaveUser(user, transaction);
				}
				transaction.Commit();
			}
		}

		public override string[] GetDomains() {
			var dsDomains = CurrentDB.ExecuteDataSet("dbo.USER_GetDomains");
			if(dsDomains.Tables.Count == 1 && dsDomains.Tables[0].Rows.Count > 0) {
				var result = new string[dsDomains.Tables[0].Rows.Count];
				for(var i = 0; i < result.Length; i++) {
					result[i] = dsDomains.Tables[0].Rows[i]["Domain"].ToString();
				}
				return result;
			}
			return new string[0];
		}

		public override IPropertyCollection GetProperties() {
			return GetPropertiesFromDataRows(null, null, null);
		}

		public override IPropertyCollection GetProperties(IContext context) {
			return GetPropertiesFromDataRows(null, context, null);
		}

		public override IPropertyCollection GetProperties(IUser user, IContext context) {
			if(user == null) {
				throw new ArgumentNullException("user");
			}
			return GetPropertiesByUserId(user.Id, context);
		}

		public override void SaveProperties(IUser user, IPropertyCollection propertyCollection) {
			using(var transaction = TransactionFactory.BeginTransaction(CurrentDB)) {
				SaveProperties(user, propertyCollection, transaction);
				transaction.Commit();
			}
		}

		public override void DeleteUser(IUser user) {
			if(user == null) {
				throw new ArgumentNullException("user");
			}
			UserCache.RemoveUserFromCache(user);
			CurrentDB.ExecuteNonQuery("dbo.USER_Delete",
			                          CurrentDB.CreateInputParameter("@guidUserID", DbType.Guid, new Guid(user.Id)));
		}

		public override IContext GetContext(string contextName) {
			if(!ContextsContains(contextName)) {
				lock(padlock) {
					if(!ContextsContains(contextName)) {
						contexts = null;
							// If not found then clear the cache and read the contexts from database again to be sure it is not created on a different computer.
					}
					if(!ContextsContains(contextName)) {
						return null;
					}
				}
			}
			return
				GetContexts().SingleOrDefault(c => string.Equals(c.Name, contextName, StringComparison.InvariantCultureIgnoreCase));
		}

		private bool ContextsContains(string contextName) {
			return GetContexts().Any(c => string.Equals(c.Name, contextName, StringComparison.InvariantCultureIgnoreCase));
		}

		public override IEnumerable<IContext> GetContexts() {
			if(contexts != null) {
				return contexts;
			}
			lock(padlock) {
				if(contexts == null) {
					var contextCollection = new List<IContext>();
					var dsFunction = CurrentDB.ExecuteDataSet("dbo.USER_GetContexts");
					if(dsFunction.Tables.Count > 0) {
						foreach(DataRow row in dsFunction.Tables[0].Rows) {
							var contextName = (string)row["ContextName"];
							IContext uc = new Context(contextName);
							contextCollection.Add(uc);
						}
					}
					contexts = contextCollection;
				}
			}
			return contexts;
		}

		public override IContext CreateContext(string contextName, ContextSchema schema) {
			if(string.IsNullOrEmpty(contextName)) {
				throw new ArgumentNullException("contextName");
			}
			if(schema == null) {
				throw new ArgumentNullException("schema");
			}

			IContext context;

			lock(padlock) {
				if(ContextsContains(contextName)) {
					throw new ContextAlreadyExistsException("A context with the name [" + contextName + "] already exists.");
				}
				using(var transaction = TransactionFactory.BeginTransaction(CurrentDB)) {
					CurrentDB.ExecuteNonQuery("dbo.USER_CreateContext",
					                          transaction,
					                          CurrentDB.CreateStringInputParameter("@chvContext", DbType.AnsiString, contextName));
					foreach(var s in schema) {
						AddSchemaToContext(s, contextName, transaction);
					}
					context = new Context(contextName);
					transaction.Commit();
				}

				contexts.Add(context);
			}

			return context;
		}

		public override void DeleteContext(IContext context) {
			if(context == null) {
				throw new ArgumentNullException("context");
			}
			lock(padlock) {
				using(var transaction = TransactionFactory.BeginTransaction(CurrentDB)) {
					CurrentDB.ExecuteNonQuery("dbo.USER_DeleteContext",
					                          transaction,
					                          CurrentDB.CreateStringInputParameter("@chvContext", DbType.AnsiString, context.Name, true));
					transaction.Commit();
				}
				contexts.Remove(context);
			}
		}

		public override ContextSchema GetDefaultContextSchema() {
			return GetContextSchema(string.Empty, null);
		}

		public override void SetPassword(IUser user, string password) {
			if(user == null) {
				throw new ArgumentNullException("user");
			}
			if(user.Properties[PasswordPropertyName] == null) {
				throw new UsersException("Database does not contain a field for password.");
			}
			if(user.Properties[PasswordSaltPropertyName] == null) {
				throw new UsersException("Database does not contain a field for password salt.");
			}

			if(Config.GetValue<bool>("hashPassword")) {
				user.Properties[PasswordSaltPropertyName].Value = GenerateSalt();
					// Generate new salt every time password is changed
				user.Properties[PasswordPropertyName].Value = MD5Hash(user.Properties["passwordSalt"].Value + password);
			} else {
				user.Properties[PasswordSaltPropertyName].Value = string.Empty;
				user.Properties[PasswordPropertyName].Value = password;
			}
		}

		public override bool CheckPassword(IUser user, string password) {
			if(user == null) {
				throw new ArgumentNullException("user");
			}
			if(user.Properties[PasswordPropertyName] == null) {
				throw new UsersException("Database does not contain a field for password.");
			}
			if(user.Properties[PasswordSaltPropertyName] == null) {
				throw new UsersException("Database does not contain a field for password salt.");
			}

			if(Config.GetValue<bool>("hashPassword")) {
				return
					user.Properties[PasswordPropertyName].Value.Equals(
					                                                   MD5Hash(user.Properties[PasswordSaltPropertyName].Value +
					                                                           password));
			}
			return user.Properties[PasswordPropertyName].Value.Equals(password);
		}
		#endregion

		#region Static Methods
		private static string EscapeForLikeClause(string value) {
			const char squarebracketOpening = '[';
			const char squarebracketClosing = ']';
			const char underscore = '_';
			const char percent = '%';

			var escapedValue = new StringBuilder(value);
			for(var i = 0; i < escapedValue.Length; i++) {
				switch(escapedValue[i]) {
					case squarebracketOpening:
					case underscore:
					case percent:
						escapedValue.
							Insert(i, squarebracketOpening).
							Insert(i + 2, squarebracketClosing);
						i += 2;
						break;
				}
			}
			return escapedValue.ToString();
		}

		/// <summary>
		/// Generates salt to use with MD5 Crypto
		/// </summary>
		/// <returns>80 bit salt as string</returns>
		private static string GenerateSalt() {
			var random = new Random();
			var salt = new byte[10];
			for(var i = 0; i < 10; i++) {
				salt[i] = (byte)random.Next(32, (126 - 32 + 1)); // Common 7bits ascii chars
			}
			return Encoding.ASCII.GetString(salt);
		}

		/// <summary>
		/// Runs MD5 Crypto on a string
		/// </summary>
		/// <param name="text">The string to hash</param>
		/// <returns>The hashed string as a Guid</returns>
		private static string MD5Hash(string text) {
			var md5 = new MD5CryptoServiceProvider();
			var hashArray = md5.ComputeHash(Encoding.Unicode.GetBytes(text));
			return BitConverter.ToString(hashArray);
		}
		#endregion

		#region Private And Protected Methods
		private IUser GetUserFromDataSet(DataSet dsUser) {
			var uc = GetUsersFromDataSet(dsUser);
			return uc.Count > 0 ? uc[0] : null;
		}

		private IList<IUser> GetUsersFromDataSet(DataSet dsUser) {
			var users = new List<IUser>();
			// translate dataset
			foreach(DataRow row in dsUser.Tables[0].Rows) {
				var domain = (string)row["Domain"];
				var userId = row["UserID"].ToString();
				var properties = GetPropertiesByUserId(userId);
				var user = new User(userId, (string)row["Username"], domain, properties, PropertyNames);
				users.Add(user);
			}
			return users;
		}

		protected virtual void SaveUser(IUser user, ITransaction transaction) {
			SaveUserInstance(user, transaction);
			SaveProperties(user, user.Properties.GetProperties(), transaction);
			if(user.Properties.AttachedContexts.Any()) {
				foreach(var context in user.Properties.AttachedContexts) {
					SaveProperties(user, user.Properties.GetProperties(context), transaction);
				}
			}
		}

		protected virtual void SaveUserInstance(IUser user, ITransaction transaction) {
			var dsUser = CurrentDB.ExecuteDataSet("dbo.USER_Update",
			                                          transaction,
			                                          CurrentDB.CreateInputParameter("@guidUserId",
			                                                                         DbType.Guid,
			                                                                         new Guid(user.Id)),
			                                          CurrentDB.CreateStringInputParameter("@chvnUsername",
			                                                                               DbType.String,
			                                                                               user.UserName,
			                                                                               false),
			                                          CurrentDB.CreateStringInputParameter("@chvDomain",
			                                                                               DbType.AnsiString,
			                                                                               user.Domain,
			                                                                               false));
			var status = (int)dsUser.Tables[0].Rows[0]["STATUS"];
			if(status == AddStatusUsernameTaken) {
				throw new UserNameAlreadyExistsException("Cannot save user. User name already exists.");
			}
		}

		protected IPropertyCollection GetPropertiesByUserId(string userId) {
			return GetPropertiesByUserId(userId, null);
		}

		protected virtual IPropertyCollection GetPropertiesByUserId(string userId, IContext context) {
			IPropertyCollection upc = null;

			var dsUser = CurrentDB.ExecuteDataSet("dbo.USER_GetProperties",
			                                          CurrentDB.CreateInputParameter("@guidUserID", DbType.Guid, new Guid(userId)),
			                                          CurrentDB.CreateStringInputParameter("@chvContext",
			                                                                               DbType.AnsiString,
			                                                                               context == null ||
			                                                                               string.IsNullOrEmpty(context.Name)
				                                                                               ? null
				                                                                               : context.Name));
			// translate dataset
			if(dsUser.Tables.Count > 0) {
				dsUser.Tables[0].PrimaryKey = new[] { dsUser.Tables[0].Columns["PropertyName"] };
				var rows = dsUser.Tables[0].Rows;
				upc = GetPropertiesFromDataRows(rows, context, null);
			}
			return upc;
		}

		private IPropertyCollection GetPropertiesFromDataRows(DataRowCollection rows,
		                                                      IContext context,
		                                                      ITransaction transaction) {
			var schema = (context == null || string.IsNullOrEmpty(context.Name)
				              ? GetDefaultContextSchema()
				              : GetContextSchema(context.Name, transaction));
			var propertyList = new List<IProperty>();

			foreach(var pd in schema) {
				string propertyValue = null;
				var propertyName = pd.PropertyName;
				var propertyType = pd.PropertyType;

				var currentField = (rows != null ? rows.Find(pd.PropertyName) : null);
				if(currentField != null) {
					if(!currentField.IsNull("PropertyValue")) {
						propertyValue = (string)currentField["PropertyValue"];
					} else {
						propertyValue = (string)currentField["ExtendedPropertyValue"];
					}
				}

				var property = PropertyFactory.Create(propertyName, propertyValue, propertyType, context);
				if(property != null) {
					propertyList.Add(property);
				}
			}

			return new PropertyCollection(propertyList, schema);
		}

		protected virtual void SaveProperty(IUser user, IProperty property, ITransaction transaction) {
			var propertyValue = string.Empty;

			if(!property.IsEmpty()) {
				propertyValue = property.ToSerializedString();
			}

			if(propertyValue.Length > 0) {
				CurrentDB.ExecuteNonQuery("dbo.USER_UpdateProperty",
				                          transaction,
				                          CurrentDB.CreateInputParameter("@guidUserId", DbType.Guid, new Guid(user.Id)),
				                          CurrentDB.CreateStringInputParameter("@chvProperty", DbType.AnsiString, property.Name),
				                          CurrentDB.CreateStringInputParameter("@chvnPropertyValue",
				                                                               DbType.String,
				                                                               propertyValue.Length <= 4000 ? propertyValue : null),
				                          CurrentDB.CreateStringInputParameter("@txtnExtPropertyValue",
				                                                               DbType.String,
				                                                               propertyValue.Length > 4000 ? propertyValue : null),
				                          CurrentDB.CreateStringInputParameter("@chvContext",
				                                                               DbType.AnsiString,
				                                                               property.Context == null ||
				                                                               string.IsNullOrEmpty(property.Context.Name)
					                                                               ? null
					                                                               : property.Context.Name));
			} else {
				CurrentDB.ExecuteNonQuery("dbo.USER_DeleteProperty",
				                          transaction,
				                          CurrentDB.CreateInputParameter("@guidUserId", DbType.Guid, new Guid(user.Id)),
				                          CurrentDB.CreateStringInputParameter("@chvPropertyName", DbType.AnsiString, property.Name),
				                          CurrentDB.CreateStringInputParameter("@chvContextName",
				                                                               DbType.AnsiString,
				                                                               property.Context == null ||
				                                                               string.IsNullOrEmpty(property.Context.Name)
					                                                               ? null
					                                                               : property.Context.Name));
			}

			property.IsDirty = false;
		}

		protected virtual void SaveProperties(IUser user, IEnumerable<IProperty> propertyCollection, ITransaction transaction) {
			if(user == null) {
				throw new ArgumentNullException("user");
			}

			if(propertyCollection == null) {
				throw new ArgumentNullException("propertyCollection");
			}

			UserCache.RemoveUserFromCache(user);

			foreach(var property in propertyCollection) {
				SaveProperty(user, property, transaction);
			}
		}

		protected virtual ContextSchema GetContextSchema(string contextName, ITransaction transaction) {
			if(contextSchema.Keys.Contains(contextName)) {
				return contextSchema[contextName];
			}

			lock(padlock) {
				if(contextSchema.Keys.Contains(contextName)) {
					return contextSchema[contextName];
				}

				var pdt = new List<PropertyDefinition>();

				IDataParameter[] prams = {
					CurrentDB.CreateStringInputParameter("@chvContext", DbType.AnsiString, contextName, true)
				};

				DataSet dsUser;
				if(transaction == null) {
					dsUser = CurrentDB.ExecuteDataSet("dbo.USER_GetPropertySchema", prams);
				} else {
					dsUser = CurrentDB.ExecuteDataSet("dbo.USER_GetPropertySchema", transaction, prams);
				}

				// translate dataset
				if(dsUser.Tables.Count > 0) {
					foreach(DataRow row in dsUser.Tables[0].Rows) {
						var propertyName = (string)row["PropertyName"];

						Type propertyType;
						if(!row.IsNull("AssemblyPath")) {
							var assembly = Assembly.LoadFrom((string)row["AssemblyPath"]);
							propertyType = assembly.GetType((string)row["DataType"]);
						} else if(!row.IsNull("AssemblyName")) {
							var assembly = Assembly.Load((string)row["AssemblyName"]);
							propertyType = assembly.GetType((string)row["DataType"]);
						} else {
							propertyType = Type.GetType((string)row["DataType"]);
						}
						if(propertyType == null) {
							throw new NotSupportedException(string.Format("The given property has a type {0} that can not be loaded.",
							                                              row["DataType"]));
						}
						var pd = new PropertyDefinition(propertyName, propertyType);
						pdt.Add(pd);
					}
				}
				var schema = new ContextSchema(pdt);
				contextSchema.Add(contextName, schema);
				return schema;
			}
		}

		protected virtual void AddSchemaToContext(PropertyDefinition definition, string contextName, ITransaction transaction) {
			CurrentDB.ExecuteNonQuery("dbo.USER_AddContextualPropertySchema",
			                          transaction,
			                          CurrentDB.CreateStringInputParameter("@chvContext", DbType.AnsiString, contextName, true),
			                          CurrentDB.CreateStringInputParameter("@chvPropertyName",
			                                                               DbType.AnsiString,
			                                                               definition.PropertyName,
			                                                               true));
		}
		#endregion
	}
}