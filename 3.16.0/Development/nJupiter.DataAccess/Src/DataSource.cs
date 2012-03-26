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
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Collections;
using System.Globalization;

using nJupiter.Configuration;

using log4net;

using ConfigurationException = nJupiter.Configuration.ConfigurationException;

namespace nJupiter.DataAccess {

	/// <summary>
	/// Represents an abstract data source. Also works as the abstract factory for different data sources.
	/// </summary>
	public abstract class DataSource {
		#region Constants
		private const string DataaccessSection = "dataSources/dataSource";
		#endregion

		#region Static Members
		private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		private static readonly Hashtable DataSourceAdapters = Hashtable.Synchronized(new Hashtable());
		#endregion

		#region Members
		private string connectionString;
		private Config config;
		#endregion

		#region Properties
		/// <summary>
		/// Gets the config object associated with the data source.
		/// </summary>
		/// <value>The config.</value>
		protected Config Config { get { return this.config; } }
		/// <summary>
		/// Gets the connection string associated with the data source.
		/// </summary>
		/// <value>The connection string.</value>
		public string ConnectionString {
			get {
				if(this.connectionString == null) {
					this.connectionString = this.Config.GetValue("connectionString");
					if(ConfigurationManager.ConnectionStrings[this.connectionString] != null) {
						this.connectionString = ConfigurationManager.ConnectionStrings[this.connectionString].ConnectionString;

					}
				}
				return this.connectionString;
			}
		}
		#endregion

		#region Singleton Methods
		/// <summary>
		/// Gets the default data source instance.
		/// </summary>
		/// <returns></returns>
		public static DataSource GetInstance() {
			if(Log.IsDebugEnabled) { Log.Debug("Getting default instance"); }
			const string section = DataaccessSection + "[@default='true']";
			return GetDataSourceFromSection(section);
		}

		/// <summary>
		/// Gets the instance with a given name. You can find the name of a configured data source in the value attribute for the dataSource element in nJupiter.DataAccess.config
		/// </summary>
		/// <param name="name">The name of the data source.</param>
		/// <returns>A <see cref="DataSource" /> instance.</returns>
		public static DataSource GetInstance(string name) {
			if(name == null)
				throw new ArgumentNullException("name");
			if(Log.IsDebugEnabled) { Log.Debug("Getting instance with name " + name); }
			const string sectionFormat = DataaccessSection + "[@value='{0}']";
			return GetDataSourceFromSection(string.Format(CultureInfo.InvariantCulture, sectionFormat, name));
		}

		#region Helper Methods
		private static DataSource GetDataSourceFromSection(string section) {
			const string settings = "settings";
			const string assemblypath = "assemblyPath";
			const string assembly = "assembly";
			const string type = "type";

			string name = ConfigHandler.GetConfig().GetValue(section);
			if(DataSourceAdapters.ContainsKey(name))
				return (DataSource)DataSourceAdapters[name];

			lock(DataSourceAdapters.SyncRoot) {
				DataSource dataSource;
				if(!DataSourceAdapters.ContainsKey(name)) {
					Config config = ConfigHandler.GetConfig();
					string assemblyPath = config.GetValue(section, assemblypath);
					string assemblyName = config.GetValue(section, assembly);
					string assemblyType = config.GetValue(section, type);

					object instance = CreateInstance(assemblyPath, assemblyName, assemblyType);
					dataSource = (DataSource)instance;
					if(dataSource == null)
						throw new ConfigurationException(string.Format("Could not load DataSource from {0} {1} {2}.", assemblyName, assemblyType, assemblyPath));
					dataSource.config = config.GetConfigSection(section + "/" + settings);
					DataSourceAdapters.Add(name, dataSource);
				} else {
					dataSource = (DataSource)DataSourceAdapters[name];
				}
				return dataSource;
			}
		}

		private static object CreateInstance(string assemblyPath, string assemblyName, string typeName) {
			Assembly assembly;
			if(!string.IsNullOrEmpty(assemblyPath)) {
				assembly = Assembly.LoadFrom(assemblyPath);
			} else if(assemblyName == null || assemblyName.Length.Equals(0) ||
				Assembly.GetExecutingAssembly().GetName().Name.Equals(assemblyName)) {
				assembly = Assembly.GetExecutingAssembly();	//Load current assembly
			} else {
				assembly = Assembly.Load(assemblyName); // Late binding to an assembly on disk (current directory)
			}
			return assembly.CreateInstance(
				typeName, false,
				BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly |
				BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.ExactBinding,
				null, null, null, null);
		}
		#endregion
		#endregion

		#region Abstract Methods
		/// <summary>
		/// Gets the <see cref="IDbConnection" /> connection associated with the current data source.
		/// </summary>
		/// <returns><see cref="IDbConnection" /> connection associated with the current data source</returns>
		public abstract IDbConnection GetConnection();
		/// <summary>
		/// Initializes a new instance of the <see cref="DbDataAdapter" /> for the data source.
		/// </summary>
		/// <param name="connection">A <see cref="IDbConnection" /> that represents the connection.</param>
		/// <returns>A <see cref="DbDataAdapter" /> for the data source</returns>
		protected abstract DbDataAdapter GetDataAdapter(IDbConnection connection);
		/// <summary>
		/// Creates a command for the data source.
		/// </summary>
		/// <param name="command">The command string for the command.</param>
		/// <param name="transaction">The transaction associated with the command.</param>
		/// <param name="commandType">Type of command.</param>
		/// <param name="parameters">Parameters associated with the command.</param>
		/// <returns></returns>
		public abstract Command CreateCommand(string command, Transaction transaction, CommandType commandType, params object[] parameters);
		/// <summary>
		/// Creates a data parameter associated with the data source.
		/// </summary>
		/// <param name="name">The name of the paramter.</param>
		/// <param name="type">The type of the parameter.</param>
		/// <returns>An <see cref="IDataParameter" /></returns>
		public abstract IDataParameter CreateParameter(string name, DbType type);
		/// <summary>
		/// Creates a data parameter associated with the data source.
		/// </summary>
		/// <param name="name">The name of the paramter.</param>
		/// <param name="value">The value of the paramter.</param>
		/// <returns>An <see cref="IDataParameter"/></returns>
		public abstract IDataParameter CreateParameter(string name, object value);
		#endregion

		#region Internal Methods
		internal IDbConnection OpenConnection() {
			if(Log.IsDebugEnabled) { Log.Debug("Open Connection"); }
			IDbConnection connection = this.GetConnection();
			connection.Open();
			return connection;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Creates a Stored Procedure command for the data source.
		/// </summary>
		/// <returns>A <see cref="Command" /> object.</returns>
		public Command CreateSPCommand() {
			return CreateSPCommand(string.Empty, null, null);
		}

		/// <summary>
		/// Creates a Stored Procedure command for the data source.
		/// </summary>
		/// <param name="spName">Name of the stored procedure.</param>
		/// <returns>A <see cref="Command" /> object.</returns>
		public Command CreateSPCommand(string spName) {
			return CreateSPCommand(spName, null, null);
		}

		/// <summary>
		/// Creates a Stored Procedure command for the data source.
		/// </summary>
		/// <param name="spName">Name of the stored procedure.</param>
		/// <param name="transaction">The transaction that the command belongs to.</param>
		/// <returns>A <see cref="Command" /> object.</returns>
		public Command CreateSPCommand(string spName, Transaction transaction) {
			return CreateSPCommand(spName, transaction, null);
		}

		/// <summary>
		/// Creates a Stored Procedure command for the data source.
		/// </summary>
		/// <param name="spName">Name of the stored procedure.</param>
		/// <param name="parameters">The parameters that shall be sent to the stored procedure.</param>
		/// <returns>A <see cref="Command" /> object.</returns>
		public Command CreateSPCommand(string spName, params object[] parameters) {
			return CreateSPCommand(spName, null, parameters);
		}

		/// <summary>
		/// Creates a Stored Procedure command for the data source.
		/// </summary>
		/// <param name="spName">Name of the stored procedure.</param>
		/// <param name="transaction">The transaction that the command belongs to.</param>
		/// <param name="parameters">The parameters that shall be sent to the stored procedure.</param>
		/// <returns>A <see cref="Command" /> object.</returns>
		public Command CreateSPCommand(string spName, Transaction transaction, params object[] parameters) {
			return CreateCommand(spName, transaction, CommandType.StoredProcedure, parameters);
		}

		/// <summary>
		/// Creates a text command for the data source.
		/// </summary>
		/// <returns>A <see cref="Command"/> object.</returns>
		public Command CreateTextCommand() {
			return CreateTextCommand(string.Empty, null, null);
		}

		/// <summary>
		/// Creates a text command for the data source.
		/// </summary>
		/// <param name="command">The command string.</param>
		/// <returns>A <see cref="Command"/> object.</returns>
		public Command CreateTextCommand(string command) {
			return CreateTextCommand(command, null, null);
		}

		/// <summary>
		/// Creates a text command for the data source.
		/// </summary>
		/// <param name="command">The command string.</param>
		/// <param name="transaction">The transaction that the command belongs to.</param>
		/// <returns>A <see cref="Command"/> object.</returns>
		public Command CreateTextCommand(string command, Transaction transaction) {
			return CreateTextCommand(command, transaction, null);
		}

		/// <summary>
		/// Creates a text command for the data source.
		/// </summary>
		/// <param name="command">The command string.</param>
		/// <param name="parameters">The parameters that shall be sent to the text command.</param>
		/// <returns>A <see cref="Command"/> object.</returns>
		public Command CreateTextCommand(string command, params object[] parameters) {
			return CreateTextCommand(command, null, parameters);
		}

		/// <summary>
		/// Creates a text command for the data source.
		/// </summary>
		/// <param name="command">The command string.</param>
		/// <param name="transaction">The transaction that the command belongs to.</param>
		/// <param name="parameters">The parameters that shall be sent to the text command.</param>
		/// <returns>A <see cref="Command" /> object.</returns>
		public Command CreateTextCommand(string command, Transaction transaction, params object[] parameters) {
			return CreateCommand(command, transaction, CommandType.Text, parameters);
		}

		/// <summary>
		/// Gets the resulting data set for a command.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <param name="dataSet">The data set.</param>
		/// <returns>A data set filled with results from the command.</returns>
		public DataSet GetDataSet(Command command, DataSet dataSet) {
			return GetDataSet(command, dataSet, "Table");
		}

		/// <summary>
		/// Gets the resulting data set for a command.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <param name="dataSet">The data set.</param>
		/// <param name="table">The name of the resulting table.</param>
		/// <returns>
		/// A data set filled with results from the command.
		/// </returns>
		public DataSet GetDataSet(Command command, DataSet dataSet, string table) {
			return GetDataSet(command, dataSet, new[] { table });
		}

		/// <summary>
		/// Gets the resulting data set for a command.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <param name="dataSet">The data set.</param>
		/// <param name="tables">The name of the resulting tables.</param>
		/// <returns>
		/// A data set filled with results from the command.
		/// </returns>
		public DataSet GetDataSet(Command command, DataSet dataSet, string[] tables) {
			if(command == null)
				throw new ArgumentNullException("command");

			if(command.Transaction == null) {
				using(Transaction transaction = Transaction.GetTransaction(this, false)) {
					command.Transaction = transaction;
					return GetDataSetInternal(command, dataSet, tables);
				}
			}
			return GetDataSetInternal(command, dataSet, tables);
		}

		private DataSet GetDataSetInternal(Command command, DataSet dataSet, string[] tables) {
			const string tableName = "Table";

			using(DbDataAdapter adapter = this.GetDataAdapter(command.DbCommand.Connection)) {
				((IDbDataAdapter)adapter).SelectCommand = command.DbCommand;

				if(tables == null || tables.Length == 0) {
					adapter.Fill(dataSet, tableName);
				} else {
					for(int i = 0; i < tables.Length; i++) {
						string name = i == 0 ? tableName : tableName + i;
						adapter.TableMappings.Add(name, tables[i]);
					}
					adapter.Fill(dataSet);
				}
			}
			return dataSet;
		}

		/// <summary>
		/// Calls the respective INSERT, UPDATE, or DELETE statements for each inserted, updated, or deleted row in the specified <see cref="DataSet"/> from a <see cref="DataTable"/> named <paramref name="tableName"/>.
		/// </summary>
		/// <param name="dataSet">The data set.</param>
		/// <param name="insertCommand">The insert command.</param>
		/// <param name="updateCommand">The update command.</param>
		/// <param name="deleteCommand">The delete command.</param>
		/// <param name="tableName">Name of the table in the dataset that shall be updated.</param>
		/// <returns>
		/// The number of rows successfully updated from the <see cref="DataSet"/>.
		/// </returns>
		public int UpdateDataSet(DataSet dataSet, Command insertCommand, Command updateCommand, Command deleteCommand, string tableName) {
			using(Transaction transaction = Transaction.GetTransaction(this, false)) {
				return UpdateDataSet(dataSet, insertCommand, updateCommand, deleteCommand, tableName, transaction);
			}
		}

		/// <summary>
		/// Calls the respective INSERT, UPDATE, or DELETE statements for each inserted, updated, or deleted row in the specified <see cref="DataSet" /> from a <see cref="DataTable" /> named <paramref name="tableName"/>. 
		/// </summary>
		/// <param name="dataSet">The data set.</param>
		/// <param name="insertCommand">The insert command.</param>
		/// <param name="updateCommand">The update command.</param>
		/// <param name="deleteCommand">The delete command.</param>
		/// <param name="tableName">Name of the table in the dataset that shall be updated.</param>
		/// <param name="transaction">The transaction that the update belongs to.</param>
		/// <returns>The number of rows successfully updated from the <see cref="DataSet" />.</returns>
		public int UpdateDataSet(DataSet dataSet, Command insertCommand, Command updateCommand, Command deleteCommand, string tableName, Transaction transaction) {
			if(transaction == null)
				throw new ArgumentNullException("transaction");
			if(dataSet == null)
				throw new ArgumentNullException("dataSet");

			using(DbDataAdapter adapter = this.GetDataAdapter(transaction.Connection)) {

				IDbDataAdapter dbAdapter = adapter;

				if(insertCommand != null)
					dbAdapter.InsertCommand = insertCommand.DbCommand;
				if(updateCommand != null)
					dbAdapter.UpdateCommand = updateCommand.DbCommand;
				if(deleteCommand != null)
					dbAdapter.DeleteCommand = deleteCommand.DbCommand;

				if(tableName == null)
					return adapter.Update(dataSet);
				return adapter.Update(dataSet.Tables[tableName]);
			}
		}

		/// <summary>
		/// Executes a command that results in a data set.
		/// </summary>
		/// <param name="command">The command text.</param>
		/// <param name="commandType">Type of the command.</param>
		/// <returns>The resulting data set.</returns>
		public DataSet ExecuteDataSet(string command, CommandType commandType) {
			using(Transaction transaction = Transaction.GetTransaction(this, false)) {
				return ExecuteDataSet(command, commandType, transaction);
			}
		}

		/// <summary>
		/// Executes a command that results in a data set.
		/// </summary>
		/// <param name="command">The command text.</param>
		/// <param name="commandType">Type of the command.</param>
		/// <param name="parameters">The parameters that shall be used in the command.</param>
		/// <returns>The resulting data set.</returns>
		public DataSet ExecuteDataSet(string command, CommandType commandType, params object[] parameters) {
			using(Transaction transaction = Transaction.GetTransaction(this, false)) {
				return ExecuteDataSet(command, commandType, transaction, parameters);
			}
		}

		/// <summary>
		/// Executes a command that results in a data set.
		/// </summary>
		/// <param name="command">The command text.</param>
		/// <param name="commandType">Type of the command.</param>
		/// <param name="transaction">The transaction that the command belongs to.</param>
		/// <returns>The resulting data set.</returns>
		public DataSet ExecuteDataSet(string command, CommandType commandType, Transaction transaction) {
			return ExecuteDataSet(command, commandType, transaction, null);
		}

		/// <summary>
		/// Executes a command that results in a data set.
		/// </summary>
		/// <param name="command">The command text.</param>
		/// <param name="commandType">Type of the command.</param>
		/// <param name="transaction">The transaction that the command belongs to.</param>
		/// <param name="parameters">The parameters that shall be used in the command.</param>
		/// <returns>The resulting data set.</returns>
		public DataSet ExecuteDataSet(string command, CommandType commandType, Transaction transaction, params object[] parameters) {
			if(commandType == CommandType.StoredProcedure) {
				return this.ExecuteDataSet(command, transaction, parameters);
			}
			Command commandObj = this.CreateTextCommand(command, transaction, parameters);
			return this.ExecuteDataSet(commandObj);
		}

		/// <summary>
		/// Executes a command that results in a data set.
		/// </summary>
		/// <param name="spName">Name of the stored procedure that shall be executed.</param>
		/// <param name="parameters">The parameters that shall be used in the command.</param>
		/// <returns>The resulting data set.</returns>
		public DataSet ExecuteDataSet(string spName, params object[] parameters) {
			using(Transaction transaction = Transaction.GetTransaction(this, false)) {
				return this.ExecuteDataSet(spName, transaction, parameters);
			}
		}

		/// <summary>
		/// Executes a command that results in a data set.
		/// </summary>
		/// <param name="spName">Name of the stored procedure that shall be executed.</param>
		/// <param name="transaction">The transaction that the command belongs to.</param>
		/// <param name="parameters">The parameters that shall be used in the command.</param>
		/// <returns>The resulting data set.</returns>
		public DataSet ExecuteDataSet(string spName, Transaction transaction, params object[] parameters) {
			Command command = CreateSPCommand(spName, transaction, parameters);
			return this.ExecuteDataSet(command);
		}

		/// <summary>
		/// Executes a command that results in a data set.
		/// </summary>
		/// <param name="spName">Name of the stored procedure that shall be executed.</param>
		/// <returns>The resulting data set.</returns>
		public DataSet ExecuteDataSet(string spName) {
			using(Transaction transaction = Transaction.GetTransaction(this, false)) {
				return this.ExecuteDataSet(spName, transaction);
			}
		}

		/// <summary>
		/// Executes a command that results in a data set.
		/// </summary>
		/// <param name="spName">Name of the stored procedure that shall be executed.</param>
		/// <param name="transaction">The transaction that the command belongs to.</param>
		/// <returns>The resulting data set.</returns>
		public DataSet ExecuteDataSet(string spName, Transaction transaction) {
			Command command = CreateSPCommand(spName, transaction);
			return this.ExecuteDataSet(command);
		}

		/// <summary>
		/// Executes a command that results in a data set.
		/// </summary>
		/// <param name="command">The command that shall be executed.</param>
		/// <returns>The resulting data set.</returns>
		public virtual DataSet ExecuteDataSet(Command command) {
			if(command == null)
				throw new ArgumentNullException("command");

			DataSet dataSet = new DataSet { Locale = CultureInfo.InvariantCulture };
			GetDataSet(command, dataSet);
			return dataSet;
		}

		/// <summary>
		/// Runs the command without returning any results.
		/// </summary>
		/// <param name="command">The command text.</param>
		/// <param name="commandType">Type of the command.</param>
		/// <returns>The number of rows affected.</returns>
		public int ExecuteNonQuery(string command, CommandType commandType) {
			using(Transaction transaction = Transaction.GetTransaction(this, false)) {
				return ExecuteNonQuery(command, commandType, transaction);
			}
		}

		/// <summary>
		/// Runs the command without returning any results.
		/// </summary>
		/// <param name="command">The command text.</param>
		/// <param name="commandType">Type of the command.</param>
		/// <param name="parameters">The parameters that shall be used in the command.</param>
		/// <returns>The number of rows affected.</returns>
		public int ExecuteNonQuery(string command, CommandType commandType, params object[] parameters) {
			using(Transaction transaction = Transaction.GetTransaction(this, false)) {
				return ExecuteNonQuery(command, commandType, transaction, parameters);
			}
		}

		/// <summary>
		/// Runs the command without returning any results.
		/// </summary>
		/// <param name="command">The command text.</param>
		/// <param name="commandType">Type of the command.</param>
		/// <param name="transaction">The transaction that the command belongs to.</param>
		/// <returns>The number of rows affected.</returns>
		public int ExecuteNonQuery(string command, CommandType commandType, Transaction transaction) {
			return ExecuteNonQuery(command, commandType, transaction, null);
		}

		/// <summary>
		/// Runs the command without returning any results.
		/// </summary>
		/// <param name="command">The command text.</param>
		/// <param name="commandType">Type of the command.</param>
		/// <param name="transaction">The transaction that the command belongs to.</param>
		/// <param name="parameters">The parameters that shall be used in the command.</param>
		/// <returns>The number of rows affected.</returns>
		public int ExecuteNonQuery(string command, CommandType commandType, Transaction transaction, params object[] parameters) {
			if(commandType == CommandType.StoredProcedure) {
				return this.ExecuteNonQuery(command, transaction, parameters);
			}
			Command commandObj = this.CreateTextCommand(command, transaction, parameters);
			return this.ExecuteNonQuery(commandObj);
		}

		/// <summary>
		/// Runs the command without returning any results.
		/// </summary>
		/// <param name="spName">Name of the stored procedure to be executed.</param>
		/// <param name="parameters">The parameters that shall be used in the command.</param>
		/// <returns>The number of rows affected.</returns>
		public int ExecuteNonQuery(string spName, params object[] parameters) {
			using(Transaction transaction = Transaction.GetTransaction(this, false)) {
				return this.ExecuteNonQuery(spName, transaction, parameters);
			}
		}

		/// <summary>
		/// Runs the command without returning any results.
		/// </summary>
		/// <param name="spName">Name of the stored procedure to be executed.</param>
		/// <param name="transaction">The transaction that the command belongs to.</param>
		/// <param name="parameters">The parameters that shall be used in the command.</param>
		/// <returns>The number of rows affected.</returns>
		public int ExecuteNonQuery(string spName, Transaction transaction, params object[] parameters) {
			Command command = CreateSPCommand(spName, transaction, parameters);
			return this.ExecuteNonQuery(command);
		}

		/// <summary>
		/// Runs the command without returning any results.
		/// </summary>
		/// <param name="spName">Name of the stored procedure to be executed.</param>
		/// <returns>The number of rows affected.</returns>
		public int ExecuteNonQuery(string spName) {
			using(Transaction transaction = Transaction.GetTransaction(this, false)) {
				return this.ExecuteNonQuery(spName, transaction);
			}
		}

		/// <summary>
		/// Runs the command without returning any results.
		/// </summary>
		/// <param name="spName">Name of the stored procedure to be executed.</param>
		/// <param name="transaction">The transaction that the command belongs to.</param>
		/// <returns>The number of rows affected.</returns>
		public int ExecuteNonQuery(string spName, Transaction transaction) {
			Command command = CreateSPCommand(spName, transaction);
			return this.ExecuteNonQuery(command);
		}

		/// <summary>
		/// Runs the command without returning any results.
		/// </summary>
		/// <param name="command">The command to be executed.</param>
		/// <returns>The number of rows affected.</returns>
		public int ExecuteNonQuery(Command command) {
			if(command == null)
				throw new ArgumentNullException("command");

			if(command.Transaction == null) {
				using(Transaction transaction = Transaction.GetTransaction(this, false)) {
					command.Transaction = transaction;
					return command.DbCommand.ExecuteNonQuery();
				}
			}
			return command.DbCommand.ExecuteNonQuery();
		}

		/// <summary>
		/// Executes the query, and returns the first column of the first row in the resultset returned by the query. Extra columns or rows are ignored.
		/// </summary>
		/// <param name="command">The command to execute.</param>
		/// <param name="commandType">Type of the command.</param>
		/// <returns>The first column of the first row in the resultset.</returns>
		public object ExecuteScalar(string command, CommandType commandType) {
			using(Transaction transaction = Transaction.GetTransaction(this, false)) {
				return ExecuteScalar(command, commandType, transaction);
			}
		}

		/// <summary>
		/// Executes the query, and returns the first column of the first row in the resultset returned by the query. Extra columns or rows are ignored.
		/// </summary>
		/// <param name="command">The command to execute.</param>
		/// <param name="commandType">Type of the command.</param>
		/// <param name="parameters">The parameters that shall be used in the command.</param>
		/// <returns>
		/// The first column of the first row in the resultset.
		/// </returns>
		public object ExecuteScalar(string command, CommandType commandType, params object[] parameters) {
			using(Transaction transaction = Transaction.GetTransaction(this, false)) {
				return ExecuteScalar(command, commandType, transaction, parameters);
			}
		}

		/// <summary>
		/// Executes the query, and returns the first column of the first row in the resultset returned by the query. Extra columns or rows are ignored.
		/// </summary>
		/// <param name="command">The command to execute.</param>
		/// <param name="commandType">Type of the command.</param>
		/// <param name="transaction">The transaction that the command belongs to.</param>
		/// <returns>
		/// The first column of the first row in the resultset.
		/// </returns>
		public object ExecuteScalar(string command, CommandType commandType, Transaction transaction) {
			if(command == null)
				throw new ArgumentNullException("command");

			return ExecuteScalar(command, commandType, transaction, null);
		}

		/// <summary>
		/// Executes the query, and returns the first column of the first row in the resultset returned by the query. Extra columns or rows are ignored.
		/// </summary>
		/// <param name="command">The command to execute.</param>
		/// <param name="commandType">Type of the command.</param>
		/// <param name="transaction">The transaction that the command belongs to.</param>
		/// <param name="parameters">The parameters that shall be used in the command.</param>
		/// <returns>
		/// The first column of the first row in the resultset.
		/// </returns>
		public object ExecuteScalar(string command, CommandType commandType, Transaction transaction, params object[] parameters) {
			if(commandType == CommandType.StoredProcedure) {
				return this.ExecuteScalar(command, transaction, parameters);
			}
			Command commandObj = this.CreateTextCommand(command, transaction, parameters);
			return this.ExecuteScalar(commandObj);
		}

		/// <summary>
		/// Executes the query, and returns the first column of the first row in the resultset returned by the query. Extra columns or rows are ignored.
		/// </summary>
		/// <param name="spName">Name of the stored procedure to be executed.</param>
		/// <param name="parameters">The parameters that shall be used by the stored procedure.</param>
		/// <returns>
		/// The first column of the first row in the resultset.
		/// </returns>
		public object ExecuteScalar(string spName, params object[] parameters) {
			using(Transaction transaction = Transaction.GetTransaction(this, false)) {
				return this.ExecuteScalar(spName, transaction, parameters);
			}
		}

		/// <summary>
		/// Executes the query, and returns the first column of the first row in the resultset returned by the query. Extra columns or rows are ignored.
		/// </summary>
		/// <param name="spName">Name of the stored procedure to be executed.</param>
		/// <param name="transaction">The transaction that the command belongs to.</param>
		/// <param name="parameters">The parameters that shall be used by the stored procedure.</param>
		/// <returns>
		/// The first column of the first row in the resultset.
		/// </returns>
		public object ExecuteScalar(string spName, Transaction transaction, params object[] parameters) {
			Command command = CreateSPCommand(spName, transaction, parameters);
			return this.ExecuteScalar(command);
		}

		/// <summary>
		/// Executes the query, and returns the first column of the first row in the resultset returned by the query. Extra columns or rows are ignored.
		/// </summary>
		/// <param name="spName">Name of the stored procedure to be executed.</param>
		/// <returns>
		/// The first column of the first row in the resultset.
		/// </returns>
		public object ExecuteScalar(string spName) {
			using(Transaction transaction = Transaction.GetTransaction(this, false)) {
				return this.ExecuteScalar(spName, transaction);
			}
		}

		/// <summary>
		/// Executes the query, and returns the first column of the first row in the resultset returned by the query. Extra columns or rows are ignored.
		/// </summary>
		/// <param name="spName">Name of the stored procedure to be executed.</param>
		/// <param name="transaction">The transaction that the command belongs to.</param>
		/// <returns>
		/// The first column of the first row in the resultset.
		/// </returns>
		public object ExecuteScalar(string spName, Transaction transaction) {
			Command command = CreateSPCommand(spName, transaction);
			return this.ExecuteScalar(command);
		}

		/// <summary>
		/// Executes the query, and returns the first column of the first row in the resultset returned by the query. Extra columns or rows are ignored.
		/// </summary>
		/// <param name="command">The command to be executed.</param>
		/// <returns>
		/// The first column of the first row in the resultset.
		/// </returns>
		public object ExecuteScalar(Command command) {
			if(command == null)
				throw new ArgumentNullException("command");
			if(command.Transaction == null) {
				using(Transaction transaction = Transaction.GetTransaction(this, false)) {
					command.Transaction = transaction;
					return command.DbCommand.ExecuteScalar();
				}
			}
			return command.DbCommand.ExecuteScalar();
		}

		/// <summary>
		/// Executes the command and builds an IDataReader.
		/// </summary>
		/// <param name="command">The command to be executed.</param>
		/// <param name="commandType">Type of the command.</param>
		/// <param name="behavior">One of the <see cref="CommandBehavior" /> values.</param>
		/// <returns>An <see cref="IDataReader" /> object.</returns>
		public IDataReader ExecuteReader(string command, CommandType commandType, CommandBehavior behavior) {
			return ExecuteReader(command, commandType, behavior);
		}

		/// <summary>
		/// Executes the command and builds an IDataReader.
		/// </summary>
		/// <param name="command">The command to be executed.</param>
		/// <param name="commandType">Type of the command.</param>
		/// <param name="behavior">One of the <see cref="CommandBehavior"/> values.</param>
		/// <param name="parameters">The parameters that shall be used in the command.</param>
		/// <returns>An <see cref="IDataReader"/> object.</returns>
		public IDataReader ExecuteReader(string command, CommandType commandType, CommandBehavior behavior, params object[] parameters) {
			return ExecuteReader(command, commandType, null, behavior, parameters);
		}

		/// <summary>
		/// Executes the command and builds an IDataReader.
		/// </summary>
		/// <param name="command">The command to be executed.</param>
		/// <param name="commandType">Type of the command.</param>
		/// <param name="transaction">The transaction that the command belongs to.</param>
		/// <param name="behavior">One of the <see cref="CommandBehavior"/> values.</param>
		/// <returns>An <see cref="IDataReader"/> object.</returns>
		public IDataReader ExecuteReader(string command, CommandType commandType, Transaction transaction, CommandBehavior behavior) {
			return ExecuteReader(command, commandType, transaction, behavior, null);
		}

		/// <summary>
		/// Executes the command and builds an IDataReader.
		/// </summary>
		/// <param name="command">The command to be executed.</param>
		/// <param name="commandType">Type of the command.</param>
		/// <param name="transaction">The transaction that the command belongs to.</param>
		/// <param name="behavior">One of the <see cref="CommandBehavior"/> values.</param>
		/// <param name="parameters">The parameters that shall be used in the command.</param>
		/// <returns>An <see cref="IDataReader"/> object.</returns>
		public IDataReader ExecuteReader(string command, CommandType commandType, Transaction transaction, CommandBehavior behavior, params object[] parameters) {
			if(commandType == CommandType.StoredProcedure) {
				return this.ExecuteReader(command, transaction, behavior, parameters);
			}
			Command commandObj = this.CreateTextCommand(command, transaction, parameters);
			return this.ExecuteReader(commandObj, behavior);
		}

		/// <summary>
		/// Executes the command and builds an IDataReader.
		/// </summary>
		/// <param name="spName">Name of the stored procedure to be executed.</param>
		/// <param name="behavior">One of the <see cref="CommandBehavior"/> values.</param>
		/// <param name="parameters">The parameters that shall be used by the stored procedure.</param>
		/// <returns>An <see cref="IDataReader"/> object.</returns>
		public IDataReader ExecuteReader(string spName, CommandBehavior behavior, params object[] parameters) {
			Command command = CreateSPCommand(spName, parameters);
			return this.ExecuteReader(command, behavior);
		}

		/// <summary>
		/// Executes the command and builds an IDataReader.
		/// </summary>
		/// <param name="spName">Name of the stored procedure to be executed.</param>
		/// <param name="transaction">The transaction that the command belongs to.</param>
		/// <param name="behavior">One of the <see cref="CommandBehavior"/> values.</param>
		/// <param name="parameters">The parameters that shall be used by the stored procedure.</param>
		/// <returns>An <see cref="IDataReader"/> object.</returns>
		public IDataReader ExecuteReader(string spName, Transaction transaction, CommandBehavior behavior, params object[] parameters) {
			Command command = CreateSPCommand(spName, transaction, parameters);
			return this.ExecuteReader(command, behavior);
		}

		/// <summary>
		/// Executes the command and builds an IDataReader.
		/// </summary>
		/// <param name="spName">Name of the stored procedure to be executed.</param>
		/// <param name="behavior">One of the <see cref="CommandBehavior"/> values.</param>
		/// <returns>An <see cref="IDataReader"/> object.</returns>
		public IDataReader ExecuteReader(string spName, CommandBehavior behavior) {
			Command command = CreateSPCommand(spName);
			return this.ExecuteReader(command, behavior);
		}

		/// <summary>
		/// Executes the command and builds an IDataReader.
		/// </summary>
		/// <param name="spName">Name of the stored procedure to be executed.</param>
		/// <param name="transaction">The transaction that the command belongs to.</param>
		/// <param name="behavior">One of the <see cref="CommandBehavior"/> values.</param>
		/// <returns>An <see cref="IDataReader"/> object.</returns>
		public IDataReader ExecuteReader(string spName, Transaction transaction, CommandBehavior behavior) {
			Command command = CreateSPCommand(spName, transaction);
			return this.ExecuteReader(command, behavior);
		}

		/// <summary>
		/// Executes the command and builds an IDataReader.
		/// </summary>
		/// <param name="command">The command to be executed.</param>
		/// <returns>An <see cref="IDataReader"/> object.</returns>
		public IDataReader ExecuteReader(Command command) {
			return ExecuteReader(command, CommandBehavior.CloseConnection);
		}

		/// <summary>
		/// Executes the command and builds an IDataReader.
		/// </summary>
		/// <param name="command">The command to be executed.</param>
		/// <param name="behavior">One of the <see cref="CommandBehavior"/> values.</param>
		/// <returns>An <see cref="IDataReader"/> object.</returns>
		public IDataReader ExecuteReader(Command command, CommandBehavior behavior) {
			if(command == null)
				throw new ArgumentNullException("command");
			if(command.DbCommand.Connection == null) {
				command.DbCommand.Connection = this.OpenConnection();
			}
			return command.DbCommand.ExecuteReader(behavior);
		}
		#endregion

		#region Parameter Methods
		/// <summary>
		/// Create an IDataParameter, the DbType is automatically generated.
		/// </summary>
		/// <param name="name">The name of parameter.</param>
		/// <param name="value">The value of parameter.</param>
		/// <returns>IDataParameter object.</returns>
		/// <remarks>
		/// This method should not be used to create date parameters, use
		/// CreateDateInputParameter instead.
		/// </remarks>
		public IDataParameter CreateInputParameter(string name, object value) {
			if(value is DateTime)
				throw new ArgumentException("Date Parameters should be created with the CreateDateInputParameter method.", "value");
			//Check for null parameters (Required?);
			if(value == null) {
				value = DBNull.Value;
			}
			return CreateParameter(name, value);
		}
		/// <summary>
		/// Create an IDataParameter of a specific DbType.
		/// </summary>
		/// <param name="name">The name of parameter.</param>
		/// <param name="type">The DbType of the parameter.</param>
		/// <param name="value">The value of parameter.</param>
		/// <returns>IDataParameter object.</returns>'
		/// <remarks>
		/// This method should not be used to create string parameters, use
		/// CreateStringInputParameter instead.
		/// </remarks>		
		public IDataParameter CreateInputParameter(string name, DbType type, object value) {
			if(value is string)
				throw new ArgumentException("String Parameters should be created with the CreateStringInputParameter method.", "value");
			if(value == null)
				value = DBNull.Value;
			IDataParameter parameter = CreateParameter(name, type);
			parameter.Value = value;
			return parameter;
		}
		/// <summary>
		/// Creates a IDataParameter of a specific type form a string
		/// </summary>
		/// <param name="name">The name of parameter.</param>
		/// <param name="type">The DbType of the parameter.</param>
		/// <param name="value">The value of parameter.</param>
		/// <returns>IDataParameter object.</returns>
		public IDataParameter CreateStringInputParameter(string name, DbType type, string value) {
			return CreateStringInputParameter(name, type, value, false);
		}
		/// <summary>
		/// Creates a IDataParameter of a specific type form a string, with an option to create a null
		/// parameter if the string is empty.
		/// </summary>
		/// <param name="name">The name of parameter.</param>
		/// <param name="type">The DbType of the parameter.</param>
		/// <param name="value">The value of parameter.</param>
		/// <param name="useNullIfEmpty">true to create a parameter with a null value if the string is empty, otherwise false.</param>
		/// <returns>IDataParameter object.</returns>
		public IDataParameter CreateStringInputParameter(string name, DbType type, string value, bool useNullIfEmpty) {
			object pramVal;
			if(value == null || (useNullIfEmpty && value.Length.Equals(0))) {
				pramVal = DBNull.Value;
			} else {
				pramVal = value;
			}
			IDataParameter parameter = CreateParameter(name, type);
			parameter.Value = pramVal;
			return parameter;
		}
		/// <summary>
		/// Creates an IDataParameter for a date datetype. If the DateTime structure has not been
		/// initialised, the parameter is set to a null value.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns>IDataParameter object.</returns>
		public IDataParameter CreateDateInputParameter(string name, DateTime value) {
			object pramVal = value == DateTime.MinValue ? DBNull.Value : (object)value;
			return CreateParameter(name, pramVal);
		}

		/// <summary>
		/// Creates an output parameter.
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="type">The DbType of the parameter.</param>
		/// <returns></returns>
		public IDataParameter CreateOutputParameter(string name, DbType type) {
			IDataParameter parameter = CreateParameter(name, type);
			parameter.Direction = ParameterDirection.Output;
			return parameter;
		}
		/// <summary>
		/// Creates an inputoutput parameter.
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="type">The DbType of the parameter.</param>
		/// <returns></returns>
		public IDataParameter CreateInputOutputParameter(string name, DbType type) {
			IDataParameter parameter = CreateParameter(name, type);
			parameter.Direction = ParameterDirection.InputOutput;
			return parameter;
		}
		/// <summary>
		/// Creates an return parameter.
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="type">The DbType of the parameter.</param>
		/// <returns></returns>
		public IDataParameter CreateReturnParameter(string name, DbType type) {
			IDataParameter parameter = CreateParameter(name, type);
			parameter.Direction = ParameterDirection.ReturnValue;
			return parameter;
		}
		#endregion
	}
}