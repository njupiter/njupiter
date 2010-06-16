#region Copyright & License
/*
	Copyright (c) 2005-2010 nJupiter

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
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Collections;
using System.Security.Cryptography;
using System.Globalization;

// TODO: Make use of typed dataset and batch update of properties etc

namespace nJupiter.DataAccess.Users {

	public class UsersDAOImplSQL : UsersDAO {
		#region Constants
		private const int AddStatusUsernameTaken = 1;
		#endregion

		#region Members
		private readonly object padlock = new object();
		private readonly Hashtable propertySchemaTables = new Hashtable();
		private ContextCollection contexts;
		private DataSource dataAccess;
		#endregion

		#region Properties
		private DataSource CurrentDB {
			get {
				if(this.dataAccess == null) {
					this.dataAccess = Config.ContainsKey("dataSource") ? DataSource.GetInstance(Config.GetValue("dataSource")) : DataSource.GetInstance();
				}
				return this.dataAccess;
			}
		}
		#endregion

		#region Overridden Methods
		public override UserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords) {
			IDataParameter returnParam = CurrentDB.CreateReturnParameter("@intPagingTotalNumber", DbType.Int32);
			DataSet dsUser;
			if(pageSize.Equals(int.MaxValue) || pageIndex.Equals(int.MaxValue)) {
				dsUser = CurrentDB.ExecuteDataSet("dbo.USER_FilterUsers", returnParam);
			} else {
				dsUser = CurrentDB.ExecuteDataSet("dbo.USER_FilterUsers",
					CurrentDB.CreateInputParameter("@intLimitPage", DbType.Int32, pageIndex + 1),
					CurrentDB.CreateInputParameter("@intLimitSize", DbType.Int32, pageSize),
					returnParam);
			}
			UserCollection uc = GetUsersFromDataSet(dsUser);
			this.UserCache.AddUsersToCache(uc);
			totalRecords = (int)returnParam.Value;
			return uc;
		}

		public override User GetUserById(string userId) {
			User user = this.UserCache.GetUserById(userId);
			if(user != null) {
				return user;
			}

			DataSet dsUser = CurrentDB.ExecuteDataSet("dbo.USER_GetByID",
				CurrentDB.CreateInputParameter("@guidUserID", DbType.Guid, new Guid(userId)));
			user = GetUserFromDataSet(dsUser);
			this.UserCache.AddUserToCache(user);
			return user;
		}

		public override User GetUserByUserName(string userName, string domain) {
			User user = this.UserCache.GetUserByUserName(userName, domain);
			if(user != null) {
				return user;
			}
			DataSet dsUser = CurrentDB.ExecuteDataSet("dbo.USER_GetByUsername",
				CurrentDB.CreateStringInputParameter("@chvnUsername", DbType.String, userName),
				CurrentDB.CreateStringInputParameter("@chvDomain", DbType.AnsiString, domain ?? string.Empty));
			user = GetUserFromDataSet(dsUser);
			this.UserCache.AddUserToCache(user);
			return user;
		}

		public override UserCollection GetUsersBySearchCriteria(SearchCriteriaCollection searchCriteriaCollection) {
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
			const string sqlBasicSubqueryStart = "EXISTS(SELECT * FROM dbo.USER_Property p JOIN dbo.USER_PropertySchema ps ON p.PropertyID = ps.PropertyID ";
			const string sqlBasicSubqueryEnd = sqlAnd + "u.UserID = p.UserID) ";
			const string sqlContextJoin = "JOIN dbo.USER_Context c ON p.ContextID = c.ContextID ";

			const string sqlCriteriaProperty = "ps.PropertyName = {0} ";
			const string sqlCriteriaContext = sqlAnd + "c.ContextName = {0} ";
			const string sqlCriteriaDomain = sqlAnd + "u.Domain = {0} ";

			const string sqlCriteriaCondNomatch = sqlAnd + "1 = 0 ";

			const string sqlCriteriaCondLike = sqlAnd + "(p.PropertyValue LIKE {0} OR p.ExtendedPropertyValue LIKE {0}) ";
			const string sqlCriteriaCondLikeOnlyext = sqlAnd + "p.ExtendedPropertyValue LIKE {0} ";
			const string sqlCriteriaCondNotlike = sqlAnd + "(p.PropertyValue NOT LIKE {0} OR p.ExtendedPropertyValue NOT LIKE {0}) ";
			const string sqlCriteriaCondNotlikeOnlyext = sqlAnd + "p.ExtendedPropertyValue NOT LIKE {0} ";

			const string sqlCriteriaCondEqual = sqlAnd + "p.PropertyValue = {0} ";
			const string sqlCriteriaCondEqualExt = sqlCriteriaCondLikeOnlyext;
			const string sqlCriteriaCondNotequal = sqlAnd + "p.PropertyValue <> {0} ";
			const string sqlCriteriaCondNotequalExt = sqlCriteriaCondNotlikeOnlyext;

			const string sqlCriteriaCondGreater = sqlAnd + "(p.PropertyValue > {0} OR CAST(p.ExtendedPropertyValue AS NVARCHAR(4000)) > {0})";
			const string sqlCriteriaCondGreaterequal = sqlAnd + "(p.PropertyValue >= {0} OR CAST(p.ExtendedPropertyValue AS NVARCHAR(4000)) >= {0})";
			const string sqlCriteriaCondLess = sqlAnd + "(p.PropertyValue < {0} OR CAST(p.ExtendedPropertyValue AS NVARCHAR(4000)) < {0})";
			const string sqlCriteriaCondLessequal = sqlAnd + "(p.PropertyValue <= {0} OR CAST(p.ExtendedPropertyValue AS NVARCHAR(4000)) <= {0})";

			const string sqlCriteriaCondContainsstartswith = sqlAnd + @"CONTAINS(p.*,N'""{0}*""') ";

			const string sqlParameter = "@param";

			Command command = CurrentDB.CreateTextCommand();
			StringBuilder queryBuilder = new StringBuilder(sqlBasicQuery);
			ArrayList requiredCriterias = new ArrayList();
			ArrayList notRequiredCriterias = new ArrayList();
			int parameterCount = 0;

			foreach(SearchCriteria sc in searchCriteriaCollection) {
				string serializedValue = sc.Property.ToSerializedString();
				string likeEscapedSerializedValue = EscapeForLikeClause(serializedValue);
				bool isDefaultValue = sc.Property.IsEmpty();

				string parameterNameName = sqlParameter + parameterCount++;
				string parameterNameValue = sqlParameter + parameterCount++;
				bool contextIsSet = sc.Property.Context != null && sc.Property.Context.Name != null;
				bool domainIsSet = !string.IsNullOrEmpty(sc.Domain);

				StringBuilder criteria = new StringBuilder(sqlBasicSubqueryStart);
				if(contextIsSet) {
					// Add context criteria to query if context is set
					criteria.Append(sqlContextJoin);
				}
				criteria.Append(sqlWhere).AppendFormat(CultureInfo.InvariantCulture, sqlCriteriaProperty, parameterNameName);
				if(contextIsSet) {
					// Add context criteria to query if context is set
					string parameterContext = sqlParameter + parameterCount++;
					criteria.AppendFormat(CultureInfo.InvariantCulture, sqlCriteriaContext, parameterContext);
					command.AddInParameter(parameterContext, DbType.String, sc.Property.Context.Name);
				}
				if(domainIsSet) {
					// Add domain criteria to query if domain is set
					string parameterDomain = sqlParameter + parameterCount++;
					criteria.AppendFormat(CultureInfo.InvariantCulture, sqlCriteriaDomain, parameterDomain);
					command.AddInParameter(parameterDomain, DbType.String, sc.Domain);
				}
				string basicSubQueryStart = criteria.ToString();
				switch(sc.Condition) {
					// If value is bigger than 4000 bytes, search in the extended value column
					case SearchCriteria.CompareCondition.ContainsStartsWith:
					if(!isDefaultValue) {
						criteria.AppendFormat(CultureInfo.InvariantCulture, sqlCriteriaCondContainsstartswith, serializedValue.Replace(sqlSinglequote, sqlSinglequoteEscaped));
					} else {
						//return all if defaultValue
						criteria.
							Append(sqlBasicSubqueryEnd).Append(sqlOr).Append(sqlNot).
							Append(basicSubQueryStart);
					}
					break;
					case SearchCriteria.CompareCondition.StartsWith:
					if(!isDefaultValue) {
						criteria.AppendFormat(CultureInfo.InvariantCulture, serializedValue.Length <= 4000 ? sqlCriteriaCondLike : sqlCriteriaCondLikeOnlyext, parameterNameValue);
						command.AddInParameter(parameterNameValue, DbType.String, likeEscapedSerializedValue + "%");
					} else {
						//return all if defaultValue
						criteria.
							Append(sqlBasicSubqueryEnd).Append(sqlOr).Append(sqlNot).
							Append(basicSubQueryStart);
					}
					break;
					case SearchCriteria.CompareCondition.NotStartsWith:
					if(!isDefaultValue) {
						criteria.
							AppendFormat(CultureInfo.InvariantCulture, serializedValue.Length <= 4000 ? sqlCriteriaCondNotlike : sqlCriteriaCondNotlikeOnlyext, parameterNameValue).
							Append(sqlBasicSubqueryEnd).Append(sqlOr).Append(sqlNot).
							Append(basicSubQueryStart);
					} else {
						//return none if defaultValue
						criteria.Append(sqlCriteriaCondNomatch);
					}
					command.AddInParameter(parameterNameValue, DbType.String, likeEscapedSerializedValue + "%");
					break;
					case SearchCriteria.CompareCondition.EndsWith:
					if(!isDefaultValue) {
						criteria.AppendFormat(CultureInfo.InvariantCulture, serializedValue.Length <= 4000 ? sqlCriteriaCondLike : sqlCriteriaCondLikeOnlyext, parameterNameValue);
						command.AddInParameter(parameterNameValue, DbType.String, "%" + likeEscapedSerializedValue);
					} else {
						//return all if defaultValue
						criteria.
							Append(sqlBasicSubqueryEnd).Append(sqlOr).Append(sqlNot).
							Append(basicSubQueryStart);
					}
					break;
					case SearchCriteria.CompareCondition.NotEndsWith:
					if(!isDefaultValue) {
						criteria.AppendFormat(CultureInfo.InvariantCulture, serializedValue.Length <= 4000 ? sqlCriteriaCondNotlike : sqlCriteriaCondNotlikeOnlyext, parameterNameValue).
							Append(sqlBasicSubqueryEnd).Append(sqlOr).Append(sqlNot).
							Append(basicSubQueryStart);
						command.AddInParameter(parameterNameValue, DbType.String, "%" + likeEscapedSerializedValue);
					} else {
						//return none if defaultValue
						criteria.Append(sqlCriteriaCondNomatch);
					}
					break;
					case SearchCriteria.CompareCondition.Contains:
					if(!isDefaultValue) {
						criteria.AppendFormat(CultureInfo.InvariantCulture, serializedValue.Length <= 4000 ? sqlCriteriaCondLike : sqlCriteriaCondLikeOnlyext, parameterNameValue);
						command.AddInParameter(parameterNameValue, DbType.String, "%" + likeEscapedSerializedValue + "%");
					} else {
						//return all if defaultValue
						criteria.
							Append(sqlBasicSubqueryEnd).Append(sqlOr).Append(sqlNot).
							Append(basicSubQueryStart);
					}
					break;
					case SearchCriteria.CompareCondition.NotContains:
					if(!isDefaultValue) {
						criteria.AppendFormat(CultureInfo.InvariantCulture, serializedValue.Length <= 4000 ? sqlCriteriaCondNotlike : sqlCriteriaCondNotlikeOnlyext, parameterNameValue).
							Append(sqlBasicSubqueryEnd).Append(sqlOr).Append(sqlNot).
							Append(basicSubQueryStart);
						command.AddInParameter(parameterNameValue, DbType.String, "%" + likeEscapedSerializedValue + "%");
					} else {
						//return none if defaultValue
						criteria.Append(sqlCriteriaCondNomatch);
					}
					break;
					case SearchCriteria.CompareCondition.NotEqual:
					if(!isDefaultValue) {
						criteria.AppendFormat(CultureInfo.InvariantCulture, serializedValue.Length <= 4000 ? sqlCriteriaCondNotequal : sqlCriteriaCondNotequalExt, parameterNameValue);
						command.AddInParameter(parameterNameValue, DbType.String, serializedValue.Length <= 4000 ? serializedValue : likeEscapedSerializedValue);
					}
					//return all that has a row if defaultValue
					break;
					case SearchCriteria.CompareCondition.GreaterThan:
					case SearchCriteria.CompareCondition.GreaterThanOrEqual:
					case SearchCriteria.CompareCondition.LessThan:
					case SearchCriteria.CompareCondition.LessThanOrEqual:
					if(!sc.Property.SerializationPreservesOrder) {
						throw new InvalidOperationException("Can not use inequality comparison on a property that does not maintain sort order of its underlying type in its serialized form.");
					}
					int comparedWithDefaultValue = sc.Property.Value == null ?
						sc.Property.DefaultValue == null ? 0 : -1 : ((IComparable)sc.Property.Value).CompareTo(sc.Property.DefaultValue);
					switch(sc.Condition) {
						case SearchCriteria.CompareCondition.GreaterThan:
						if(comparedWithDefaultValue < 0) {	// value > (<defaultValue)
							criteria.Insert(0, sqlNot).AppendFormat(CultureInfo.InvariantCulture, sqlCriteriaCondLessequal, parameterNameValue);
						} else {							// value > (>=defaultValue)
							criteria.AppendFormat(CultureInfo.InvariantCulture, sqlCriteriaCondGreater, parameterNameValue);
						}
						break;
						case SearchCriteria.CompareCondition.GreaterThanOrEqual:
						if(comparedWithDefaultValue > 0) {	//value >= (>defaultValue)
							criteria.AppendFormat(CultureInfo.InvariantCulture, sqlCriteriaCondGreaterequal, parameterNameValue);
						} else {							//value >= (<=defaultValue>
							criteria.Insert(0, sqlNot).AppendFormat(CultureInfo.InvariantCulture, sqlCriteriaCondLess, parameterNameValue);
						}
						break;
						case SearchCriteria.CompareCondition.LessThan:
						if(comparedWithDefaultValue > 0) {	//value < (>defaultValue>
							criteria.Insert(0, sqlNot).AppendFormat(CultureInfo.InvariantCulture, sqlCriteriaCondGreaterequal, parameterNameValue);
						} else {							//value < (<=defaultValue)
							criteria.AppendFormat(CultureInfo.InvariantCulture, sqlCriteriaCondLess, parameterNameValue);
						}
						break;
						case SearchCriteria.CompareCondition.LessThanOrEqual:
						if(comparedWithDefaultValue < 0) {	//value <= (<defaultValue)
							criteria.AppendFormat(CultureInfo.InvariantCulture, sqlCriteriaCondLessequal, parameterNameValue);
						} else {							//value <= (>=defaultValue>
							criteria.Insert(0, sqlNot).AppendFormat(CultureInfo.InvariantCulture, sqlCriteriaCondGreater, parameterNameValue);
						}
						break;
					}
					command.AddInParameter(parameterNameValue, DbType.String, serializedValue);
					break;
					default:
					if(!isDefaultValue) {
						criteria.AppendFormat(CultureInfo.InvariantCulture, serializedValue.Length <= 4000 ? sqlCriteriaCondEqual : sqlCriteriaCondEqualExt, parameterNameValue);
						command.AddInParameter(parameterNameValue, DbType.String, serializedValue.Length <= 4000 ? serializedValue : likeEscapedSerializedValue);
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
					for(int i = 0; i < requiredCriterias.Count; i++) {
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
					for(int i = 0; i < notRequiredCriterias.Count; i++) {
						queryBuilder.Append(notRequiredCriterias[i]);
						if(i != (notRequiredCriterias.Count - 1)) // If last criteria, do not add OR
							queryBuilder.Append(sqlOr);
					}
					queryBuilder.Append(") ");
				}
			}

			command.CommandText = queryBuilder.ToString();
			try {
				UserCollection uc = GetUsersFromDataSet(CurrentDB.ExecuteDataSet(command));
				this.UserCache.AddUsersToCache(uc);
				return uc;
			} catch(SqlException ex) {
				//the execution of a full-text operation failed because
				if(ex.Number.Equals(7603) ||	//full-text predicate was null or empty (SQL Server 2000)
					ex.Number.Equals(7619) ||	//the clause of the query contained only ignored words. (SQL Server 2000)
					ex.Number.Equals(7643) ||	//the search generated too many results
					ex.Number.Equals(7645)) {	//full-text predicate was null or empty (SQL Server 2005)
					//return empty user collection
					return new UserCollection();
				}
				throw;
			}
		}

		public override UserCollection GetUsersByDomain(string domain) {
			if(domain == null) {
				domain = string.Empty;
			}
			DataSet dsUser = CurrentDB.ExecuteDataSet("dbo.USER_GetUsersByDomain",
				CurrentDB.CreateStringInputParameter("@chvDomain", DbType.AnsiString, domain, false));
			UserCollection uc = GetUsersFromDataSet(dsUser);
			this.UserCache.AddUsersToCache(uc);
			return uc;
		}

		public override User CreateUserInstance(string userName, string domain) {
			if(domain == null)
				domain = string.Empty;

			string userId = Guid.NewGuid().ToString();
			User user = new User(userId, userName, domain, GetPropertiesByUserId(userId), this.PropertyNames);
			return user;
		}

		public override void SaveUser(User user) {
			using(Transaction transaction = Transaction.BeginTransaction(CurrentDB)) {
				this.SaveUser(user, transaction);
				transaction.Commit();
			}
		}

		public override void SaveUsers(UserCollection users) {
			if(users == null)
				throw new ArgumentNullException("users");

			using(Transaction transaction = Transaction.BeginTransaction(CurrentDB)) {
				foreach(User user in users) {
					this.SaveUser(user, transaction);
				}
				transaction.Commit();
			}
		}

		public override string[] GetDomains() {
			DataSet dsDomains = CurrentDB.ExecuteDataSet("dbo.USER_GetDomains");
			if(dsDomains.Tables.Count == 1 && dsDomains.Tables[0].Rows.Count > 0) {
				string[] result = new string[dsDomains.Tables[0].Rows.Count];
				for(int i = 0; i < result.Length; i++) {
					result[i] = dsDomains.Tables[0].Rows[i]["Domain"].ToString();
				}
				return result;
			}
			return new string[0];
		}

		public override PropertyCollection GetProperties() {
			return GetPropertiesFromDataRows(null, null, null);
		}

		public override PropertyCollection GetProperties(Context context) {
			return GetPropertiesFromDataRows(null, context, null);
		}

		public override PropertyCollection GetProperties(User user, Context context) {
			if(user == null)
				throw new ArgumentNullException("user");

			PropertyCollection pc = base.GetProperties(user, context);
			return pc ?? this.GetPropertiesByUserId(user.Id, context);
		}

		public override void SaveProperties(User user, PropertyCollection propertyCollection) {
			using(Transaction transaction = Transaction.BeginTransaction(CurrentDB)) {
				SaveProperties(user, propertyCollection, transaction);
				transaction.Commit();
			}
		}

		public override void DeleteUser(User user) {
			if(user == null) {
				throw new ArgumentNullException("user");
			}
			this.UserCache.RemoveUserFromCache(user);
			CurrentDB.ExecuteNonQuery("dbo.USER_Delete",
				CurrentDB.CreateInputParameter("@guidUserID", DbType.Guid, new Guid(user.Id)));
		}

		public override Context GetContext(string contextName) {
			if(this.GetContexts().Contains(contextName))
				return this.GetContexts()[contextName];
			lock(this.padlock) {
				if(!this.GetContexts().Contains(contextName))
					this.contexts = null; // If not found then clear the cache and read the contexts from database again to be sure it is not created on a different computer.
				if(!this.GetContexts().Contains(contextName))
					return null;
			}
			return this.GetContexts()[contextName];
		}
		public override ContextCollection GetContexts() {
			if(this.contexts != null)
				return this.contexts;
			lock(this.padlock) {
				if(this.contexts == null) {
					ContextCollection contextCollection = CreateContextCollectionInstance();
					DataSet dsFunction = CurrentDB.ExecuteDataSet("dbo.USER_GetContexts");
					if(dsFunction.Tables.Count > 0) {
						foreach(DataRow row in dsFunction.Tables[0].Rows) {
							string contextName = (string)row["ContextName"];
							Context uc = CreateContextInstance(contextName, this.GetPropertySchemas(contextName, null));
							AddContextToCollection(uc, contextCollection);
						}
					}
					this.contexts = contextCollection;
				}
			}
			return this.contexts;
		}

		public override Context CreateContext(string contextName, PropertySchemaTable schemaTable) {
			if(contextName == null)
				throw new ArgumentNullException("contextName");
			if(schemaTable == null)
				throw new ArgumentNullException("schemaTable");

			Context context;

			lock(this.contexts.SyncRoot) {
				if(this.GetContexts().Contains(contextName)) {
					throw new ContextAlreadyExistsException("A context with the name [" + contextName + "] already exists.");
				}
				using(Transaction transaction = Transaction.BeginTransaction(CurrentDB)) {
					CurrentDB.ExecuteNonQuery("dbo.USER_CreateContext", transaction,
						CurrentDB.CreateStringInputParameter("@chvContext", DbType.AnsiString, contextName));
					foreach(PropertySchema schema in schemaTable) {
						this.AddSchemaToContext(schema, contextName, transaction);
					}
					context = CreateContextInstance(contextName, this.GetPropertySchemas(contextName, transaction));
					transaction.Commit();
				}

				AddContextToCollection(context, this.contexts);
			}

			return context;
		}

		public override void DeleteContext(Context context) {
			if(context == null) {
				throw new ArgumentNullException("context");
			}
			lock(this.contexts.SyncRoot) {
				using(Transaction transaction = Transaction.BeginTransaction(CurrentDB)) {
					CurrentDB.ExecuteNonQuery("dbo.USER_DeleteContext", transaction,
						CurrentDB.CreateStringInputParameter("@chvContext", DbType.AnsiString, context.Name, true));
					transaction.Commit();
				}
				RemoveContextFromCollection(context, this.contexts);
			}
		}

		public override PropertySchemaTable GetPropertySchemas() {
			return GetPropertySchemas(string.Empty, null);
		}

		public override void SetPassword(User user, string password) {
			if(user == null)
				throw new ArgumentNullException("user");
			if(user.Properties[this.PropertyNames.Password] == null)
				throw new UsersException("Database does not contain a field for password.");
			if(user.Properties[this.PropertyNames.PasswordSalt] == null)
				throw new UsersException("Database does not contain a field for password salt.");

			if(this.Config.GetBoolValue("hashPassword")) {
				user.Properties[this.PropertyNames.PasswordSalt].Value = GenerateSalt(); // Generate new salt every time password is changed
				user.Properties[this.PropertyNames.Password].Value = MD5Hash(user.Properties["passwordSalt"].Value + password);
			} else {
				user.Properties[this.PropertyNames.PasswordSalt].Value = string.Empty;
				user.Properties[this.PropertyNames.Password].Value = password;
			}
		}

		public override bool CheckPassword(User user, string password) {
			if(user == null)
				throw new ArgumentNullException("user");
			if(user.Properties["password"] == null)
				throw new UsersException("Database does not contain a field for password.");
			if(user.Properties["passwordSalt"] == null)
				throw new UsersException("Database does not contain a field for password salt.");

			if(this.Config.GetBoolValue("hashPassword")) {
				return user.Properties["password"].Value.Equals(MD5Hash(user.Properties["passwordSalt"].Value + password));
			}
			return user.Properties["password"].Value.Equals(password);
		}
		#endregion

		#region Static Methods
		private static string EscapeForLikeClause(string value) {
			const char squarebracketOpening = '[';
			const char squarebracketClosing = ']';
			const char underscore = '_';
			const char percent = '%';

			StringBuilder escapedValue = new StringBuilder(value);
			for(int i = 0; i < escapedValue.Length; i++) {
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
			Random random = new Random();
			byte[] salt = new byte[10];
			for(int i = 0; i < 10; i++)
				salt[i] = (byte)random.Next(32, (126 - 32 + 1)); // Common 7bits ascii chars
			return System.Text.Encoding.ASCII.GetString(salt);
		}

		/// <summary>
		/// Runs MD5 Crypto on a string
		/// </summary>
		/// <param name="text">The string to hash</param>
		/// <returns>The hashed string as a Guid</returns>
		private static string MD5Hash(string text) {
			MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
			byte[] hashArray = md5.ComputeHash(Encoding.Unicode.GetBytes(text));
			return BitConverter.ToString(hashArray);
		}
		#endregion

		#region Private And Protected Methods
		private User GetUserFromDataSet(DataSet dsUser) {
			UserCollection uc = GetUsersFromDataSet(dsUser);
			return uc.Count > 0 ? uc[0] : null;
		}

		private UserCollection GetUsersFromDataSet(DataSet dsUser) {
			UserCollection users = new UserCollection();
			// translate dataset
			foreach(DataRow row in dsUser.Tables[0].Rows) {
				string domain = (string)row["Domain"];
				string userId = row["UserID"].ToString();

				User user = new User(userId, (string)row["Username"], domain, GetPropertiesByUserId(userId), this.PropertyNames);
				users.Add(user);
			}
			return users;
		}
		protected virtual void SaveUser(User user, Transaction transaction) {
			SaveUserInstance(user, transaction);
			SaveProperties(user, this.GetProperties(user), transaction);
			if(GetAttachedContextsToUser(user).Length > 0) {
				foreach(Context context in GetAttachedContextsToUser(user)) {
					SaveProperties(user, this.GetProperties(user, context), transaction);
				}
			}
		}
		protected virtual void SaveUserInstance(User user, Transaction transaction) {
			DataSet dsUser = CurrentDB.ExecuteDataSet("dbo.USER_Update", transaction,
				CurrentDB.CreateInputParameter("@guidUserId", DbType.Guid, new Guid(user.Id)),
				CurrentDB.CreateStringInputParameter("@chvnUsername", DbType.String, user.UserName, false),
				CurrentDB.CreateStringInputParameter("@chvDomain", DbType.AnsiString, user.Domain, false));
			int status = (int)dsUser.Tables[0].Rows[0]["STATUS"];
			if(status == AddStatusUsernameTaken) {
				throw new UserNameAlreadyExistsException("Cannot save user. User name already exists.");
			}
		}
		protected PropertyCollection GetPropertiesByUserId(string userId) {
			return GetPropertiesByUserId(userId, null);
		}
		protected virtual PropertyCollection GetPropertiesByUserId(string userId, Context context) {
			PropertyCollection upc = null;

			DataSet dsUser = CurrentDB.ExecuteDataSet("dbo.USER_GetProperties",
				CurrentDB.CreateInputParameter("@guidUserID", DbType.Guid, new Guid(userId)),
				CurrentDB.CreateStringInputParameter("@chvContext", DbType.AnsiString, context == null ? null : context.Name));
			// translate dataset
			if(dsUser.Tables.Count > 0) {
				dsUser.Tables[0].PrimaryKey = new[] { dsUser.Tables[0].Columns["PropertyName"] };
				DataRowCollection rows = dsUser.Tables[0].Rows;
				upc = GetPropertiesFromDataRows(rows, context, null);
			}
			return upc;
		}
		private PropertyCollection GetPropertiesFromDataRows(DataRowCollection rows, Context context, Transaction transaction) {
			PropertySchemaTable pdt = (context == null ? this.GetPropertySchemas() : this.GetPropertySchemas(context.Name, transaction));
			PropertyCollection upc = CreatePropertyCollectionInstance(pdt);

			foreach(PropertySchema pd in pdt) {
				string propertyValue = null;
				string propertyName = pd.PropertyName;
				Type propertyType = pd.DataType;

				DataRow currentField = (rows != null ? rows.Find(pd.PropertyName) : null);
				if(currentField != null) {
					if(!currentField.IsNull("PropertyValue"))
						propertyValue = (string)currentField["PropertyValue"];
					else
						propertyValue = (string)currentField["ExtendedPropertyValue"];
				}

				AbstractProperty property = CreatePropertyInstance(propertyName, propertyValue, propertyType, context);
				if(property != null)
					AddPropertyToCollection(property, upc);
			}

			return upc;
		}
		protected virtual void SaveProperty(User user, AbstractProperty property, Transaction transaction) {
			string propertyValue = string.Empty;

			if(!property.IsEmpty())
				propertyValue = property.ToSerializedString();

			if(propertyValue.Length > 0) {
				CurrentDB.ExecuteNonQuery("dbo.USER_UpdateProperty", transaction,
					CurrentDB.CreateInputParameter("@guidUserId", DbType.Guid, new Guid(user.Id)),
					CurrentDB.CreateStringInputParameter("@chvProperty", DbType.AnsiString, property.Name),
					CurrentDB.CreateStringInputParameter("@chvnPropertyValue", DbType.String, propertyValue.Length <= 4000 ? propertyValue : null),
					CurrentDB.CreateStringInputParameter("@txtnExtPropertyValue", DbType.String, propertyValue.Length > 4000 ? propertyValue : null),
					CurrentDB.CreateStringInputParameter("@chvContext", DbType.AnsiString, property.Context == null ? null : property.Context.Name));
			} else {
				CurrentDB.ExecuteNonQuery("dbo.USER_DeleteProperty", transaction,
					CurrentDB.CreateInputParameter("@guidUserId", DbType.Guid, new Guid(user.Id)),
					CurrentDB.CreateStringInputParameter("@chvPropertyName", DbType.AnsiString, property.Name),
					CurrentDB.CreateStringInputParameter("@chvContextName", DbType.AnsiString, property.Context == null ? null : property.Context.Name));
			}

			property.IsDirty = false;
		}
		protected virtual void SaveProperties(User user, PropertyCollection propertyCollection, Transaction transaction) {
			if(user == null)
				throw new ArgumentNullException("user");

			if(propertyCollection == null)
				throw new ArgumentNullException("propertyCollection");

			this.UserCache.RemoveUserFromCache(user);

			foreach(AbstractProperty property in propertyCollection) {
				SaveProperty(user, property, transaction);
			}
		}
		protected virtual PropertySchemaTable GetPropertySchemas(string contextName, Transaction transaction) {
			if(this.propertySchemaTables.Contains(contextName))
				return (PropertySchemaTable)this.propertySchemaTables[contextName];

			lock(padlock) {
				if(this.propertySchemaTables.Contains(contextName))
					return (PropertySchemaTable)this.propertySchemaTables[contextName];

				PropertySchemaTable pdt = CreatePropertySchemaTableInstance();

				IDataParameter[] prams = { CurrentDB.CreateStringInputParameter("@chvContext", DbType.AnsiString, contextName, true) };


				DataSet dsUser;
				if(transaction == null) {
					dsUser = CurrentDB.ExecuteDataSet("dbo.USER_GetPropertySchema", prams);
				} else {
					dsUser = CurrentDB.ExecuteDataSet("dbo.USER_GetPropertySchema", transaction, prams);
				}

				// translate dataset
				if(dsUser.Tables.Count > 0) {
					foreach(DataRow row in dsUser.Tables[0].Rows) {
						string propertyName = (string)row["PropertyName"];

						Type propertyType;
						if(!row.IsNull("AssemblyPath")) {
							Assembly assembly = Assembly.LoadFrom((string)row["AssemblyPath"]);
							propertyType = assembly.GetType((string)row["DataType"]);
						} else if(!row.IsNull("AssemblyName")) {
							Assembly assembly = Assembly.Load((string)row["AssemblyName"]);
							propertyType = assembly.GetType((string)row["DataType"]);
						} else {
							propertyType = Type.GetType((string)row["DataType"]);
						}
						if(propertyType == null)
							throw new UnsupportedTypeException("The given property has a type " + (string)row["DataType"] + " that can not be loaded.");

						PropertySchema pd = CreatePropertySchemaInstance(propertyName, propertyType);
						AddPropertySchemaToTable(pd, pdt);
					}
				}
				this.propertySchemaTables.Add(contextName, pdt);
				return pdt;
			}
		}
		protected virtual void AddSchemaToContext(PropertySchema schema, string contextName, Transaction transaction) {
			CurrentDB.ExecuteNonQuery("dbo.USER_AddContextualPropertySchema", transaction,
				CurrentDB.CreateStringInputParameter("@chvContext", DbType.AnsiString, contextName, true),
				CurrentDB.CreateStringInputParameter("@chvPropertyName", DbType.AnsiString, schema.PropertyName, true));
		}
		#endregion

	}
}
