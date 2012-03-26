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
using System.Data;
using System.Globalization;

namespace nJupiter.DataAccess {

	internal class DataSource : IDataSource {

		private readonly IProvider provider;

		internal DataSource(IProvider provider) {
			this.provider = provider;
		}

		public IProvider Provider { get { return this.provider;  } }

		private IDbDataAdapter GetDataAdapter() {
			return this.Provider.CreateDataAdapter();
		}

		public ICommand CreateCommand(string command, ITransaction transaction, CommandType commandType, params IDataParameter[] parameters) {
			var dbCommand = this.Provider.CreateCommand();
			dbCommand.CommandText = command;
			dbCommand.CommandType = commandType;
			return new Command(dbCommand, transaction, parameters);
		}

		public IDataParameter CreateParameter(string name, DbType type) {
			var param = this.Provider.CreateParameter();
			param.ParameterName = name;
			param.DbType = type;
			return param;
		}

		public IDataParameter CreateParameter(string name, object value) {
			var param = this.Provider.CreateParameter();
			param.ParameterName = name;
			param.Value = value;
			return param;			
		}

		public IDbConnection OpenConnection() {
			var connection = this.Provider.CreateConnection();
			connection.Open();
			return connection;
		}

		public ICommand CreateSPCommand() {
			return CreateSPCommand(string.Empty, null, (IDataParameter[])null);
		}

		public ICommand CreateSPCommand(string spName) {
			return CreateSPCommand(spName, null, (IDataParameter[])null);
		}

		public ICommand CreateSPCommand(string spName, ITransaction transaction) {
			return CreateSPCommand(spName, transaction, null);
		}

		public ICommand CreateSPCommand(string spName, params IDataParameter[] parameters) {
			return CreateSPCommand(spName, null, parameters);
		}

		public ICommand CreateSPCommand(string spName, ITransaction transaction, params IDataParameter[] parameters) {
			return CreateCommand(spName, transaction, CommandType.StoredProcedure, parameters);
		}

		public ICommand CreateTextCommand() {
			return CreateTextCommand(string.Empty, null, (IDataParameter[])null);
		}

		public ICommand CreateTextCommand(string command) {
			return CreateTextCommand(command, null, (IDataParameter[])null);
		}

		public ICommand CreateTextCommand(string command, ITransaction transaction) {
			return CreateTextCommand(command, transaction, null);
		}

		public ICommand CreateTextCommand(string command, params IDataParameter[] parameters) {
			return CreateTextCommand(command, null, parameters);
		}

		public ICommand CreateTextCommand(string command, ITransaction transaction, params IDataParameter[] parameters) {
			return CreateCommand(command, transaction, CommandType.Text, parameters);
		}


		public DataSet GetDataSet(ICommand command, DataSet dataSet) {
			return GetDataSet(command, dataSet, "Table");
		}

		public DataSet GetDataSet(ICommand command, DataSet dataSet, string table) {
			return GetDataSet(command, dataSet, new[] { table });
		}

		public DataSet GetDataSet(ICommand command, DataSet dataSet, string[] tables) {
			if(command == null)
				throw new ArgumentNullException("command");

			if(command.Transaction == null) {
				using(ITransaction transaction = TransactionFactory.GetTransaction(this, false)) {
					command.Transaction = transaction;
					return GetDataSetInternal(command, dataSet, tables);
				}
			}
			return GetDataSetInternal(command, dataSet, tables);
		}

		private DataSet GetDataSetInternal(ICommand command, DataSet dataSet, string[] tables) {
			const string tableName = "Table";

			IDbDataAdapter adapter = this.GetDataAdapter();
 
			adapter.SelectCommand = command.DbCommand;
			if(tables != null){
				for(int i = 0; i < tables.Length; i++) {
					string name = i == 0 ? tableName : tableName + i;
					adapter.TableMappings.Add(name, tables[i]);
				}
			}
			adapter.Fill(dataSet);
			return dataSet;
		}

		public int UpdateDataSet(DataSet dataSet, ICommand insertCommand, ICommand updateCommand, ICommand deleteCommand) {
			return UpdateDataSet(dataSet, insertCommand, updateCommand, deleteCommand, null);
		}
		
		
		public int UpdateDataSet(DataSet dataSet, ICommand insertCommand, ICommand updateCommand, ICommand deleteCommand, string tableName) {
			using(ITransaction transaction = TransactionFactory.GetTransaction(this, false)) {
				return UpdateDataSet(dataSet, insertCommand, updateCommand, deleteCommand, tableName, transaction);
			}
		}

		public int UpdateDataSet(DataSet dataSet, ICommand insertCommand, ICommand updateCommand, ICommand deleteCommand, string tableName, ITransaction transaction) {
			if(transaction == null)
				throw new ArgumentNullException("transaction");
			if(dataSet == null)
				throw new ArgumentNullException("dataSet");

			IDbDataAdapter dbAdapter = this.GetDataAdapter();

			if(insertCommand != null) {
				insertCommand.Transaction = transaction;
				dbAdapter.InsertCommand = insertCommand.DbCommand;
			}
			if(updateCommand != null){
				updateCommand.Transaction = transaction;
				dbAdapter.UpdateCommand = updateCommand.DbCommand;
			}

			if(deleteCommand != null) {
				deleteCommand.Transaction = transaction;
				dbAdapter.DeleteCommand = deleteCommand.DbCommand;
			}

			if(tableName != null){
				dbAdapter.TableMappings.Add("Table", tableName);
			}

			return dbAdapter.Update(dataSet);
		}

		public DataSet ExecuteDataSet(string command, CommandType commandType) {
			using(ITransaction transaction = TransactionFactory.GetTransaction(this, false)) {
				return ExecuteDataSet(command, commandType, transaction);
			}
		}

		public DataSet ExecuteDataSet(string command, CommandType commandType, params IDataParameter[] parameters) {
			using(ITransaction transaction = TransactionFactory.GetTransaction(this, false)) {
				return ExecuteDataSet(command, commandType, transaction, parameters);
			}
		}

		public DataSet ExecuteDataSet(string command, CommandType commandType, ITransaction transaction) {
			return ExecuteDataSet(command, commandType, transaction, null);
		}

		public DataSet ExecuteDataSet(string command, CommandType commandType, ITransaction transaction, params IDataParameter[] parameters) {
			if(commandType == CommandType.StoredProcedure) {
				return this.ExecuteDataSet(command, transaction, parameters);
			}
			ICommand commandObj = this.CreateTextCommand(command, transaction, parameters);
			return this.ExecuteDataSet(commandObj);
		}

		public DataSet ExecuteDataSet(string spName, params IDataParameter[] parameters) {
			using(ITransaction transaction = TransactionFactory.GetTransaction(this, false)) {
				return this.ExecuteDataSet(spName, transaction, parameters);
			}
		}

		public DataSet ExecuteDataSet(string spName, ITransaction transaction, params IDataParameter[] parameters) {
			ICommand command = CreateSPCommand(spName, transaction, parameters);
			return this.ExecuteDataSet(command);
		}

		public DataSet ExecuteDataSet(string spName) {
			using(ITransaction transaction = TransactionFactory.GetTransaction(this, false)) {
				return this.ExecuteDataSet(spName, transaction);
			}
		}

		public DataSet ExecuteDataSet(string spName, ITransaction transaction) {
			ICommand command = CreateSPCommand(spName, transaction);
			return this.ExecuteDataSet(command);
		}

		public virtual DataSet ExecuteDataSet(ICommand command) {
			if(command == null)
				throw new ArgumentNullException("command");

			DataSet dataSet = new DataSet { Locale = CultureInfo.InvariantCulture };
			GetDataSet(command, dataSet);
			return dataSet;
		}

		public int ExecuteNonQuery(string command, CommandType commandType) {
			using(ITransaction transaction = TransactionFactory.GetTransaction(this, false)) {
				return ExecuteNonQuery(command, commandType, transaction);
			}
		}

		public int ExecuteNonQuery(string command, CommandType commandType, params IDataParameter[] parameters) {
			using(ITransaction transaction = TransactionFactory.GetTransaction(this, false)) {
				return ExecuteNonQuery(command, commandType, transaction, parameters);
			}
		}

		public int ExecuteNonQuery(string command, CommandType commandType, ITransaction transaction) {
			return ExecuteNonQuery(command, commandType, transaction, null);
		}

		public int ExecuteNonQuery(string command, CommandType commandType, ITransaction transaction, params IDataParameter[] parameters) {
			if(commandType == CommandType.StoredProcedure) {
				return this.ExecuteNonQuery(command, transaction, parameters);
			}
			ICommand commandObj = this.CreateTextCommand(command, transaction, parameters);
			return this.ExecuteNonQuery(commandObj);
		}

		public int ExecuteNonQuery(string spName, params IDataParameter[] parameters) {
			using(ITransaction transaction = TransactionFactory.GetTransaction(this, false)) {
				return this.ExecuteNonQuery(spName, transaction, parameters);
			}
		}

		public int ExecuteNonQuery(string spName, ITransaction transaction, params IDataParameter[] parameters) {
			ICommand command = CreateSPCommand(spName, transaction, parameters);
			return this.ExecuteNonQuery(command);
		}

		public int ExecuteNonQuery(string spName) {
			using(ITransaction transaction = TransactionFactory.GetTransaction(this, false)) {
				return this.ExecuteNonQuery(spName, transaction);
			}
		}

		public int ExecuteNonQuery(string spName, ITransaction transaction) {
			ICommand command = CreateSPCommand(spName, transaction);
			return this.ExecuteNonQuery(command);
		}

		public int ExecuteNonQuery(ICommand command) {
			if(command == null)
				throw new ArgumentNullException("command");

			if(command.Transaction == null) {
				using(ITransaction transaction = TransactionFactory.GetTransaction(this, false)) {
					command.Transaction = transaction;
					return command.DbCommand.ExecuteNonQuery();
				}
			}
			return command.DbCommand.ExecuteNonQuery();
		}

		public object ExecuteScalar(string command, CommandType commandType) {
			using(ITransaction transaction = TransactionFactory.GetTransaction(this, false)) {
				return ExecuteScalar(command, commandType, transaction);
			}
		}

		public object ExecuteScalar(string command, CommandType commandType, params IDataParameter[] parameters) {
			using(ITransaction transaction = TransactionFactory.GetTransaction(this, false)) {
				return ExecuteScalar(command, commandType, transaction, parameters);
			}
		}

		public object ExecuteScalar(string command, CommandType commandType, ITransaction transaction) {
			return ExecuteScalar(command, commandType, transaction, null);
		}

		public object ExecuteScalar(string command, CommandType commandType, ITransaction transaction, params IDataParameter[] parameters) {
			if(commandType == CommandType.StoredProcedure) {
				return this.ExecuteScalar(command, transaction, parameters);
			}
			ICommand commandObj = this.CreateTextCommand(command, transaction, parameters);
			return this.ExecuteScalar(commandObj);
		}

		public object ExecuteScalar(string spName, params IDataParameter[] parameters) {
			using(ITransaction transaction = TransactionFactory.GetTransaction(this, false)) {
				return this.ExecuteScalar(spName, transaction, parameters);
			}
		}

		public object ExecuteScalar(string spName, ITransaction transaction, params IDataParameter[] parameters) {
			ICommand command = CreateSPCommand(spName, transaction, parameters);
			return this.ExecuteScalar(command);
		}

		public object ExecuteScalar(string spName) {
			using(ITransaction transaction = TransactionFactory.GetTransaction(this, false)) {
				return this.ExecuteScalar(spName, transaction);
			}
		}

		public object ExecuteScalar(string spName, ITransaction transaction) {
			ICommand command = CreateSPCommand(spName, transaction);
			return this.ExecuteScalar(command);
		}

		public object ExecuteScalar(ICommand command) {
			if(command == null)
				throw new ArgumentNullException("command");
			if(command.Transaction == null) {
				using(ITransaction transaction = TransactionFactory.GetTransaction(this, false)) {
					command.Transaction = transaction;
					return command.DbCommand.ExecuteScalar();
				}
			}
			return command.DbCommand.ExecuteScalar();
		}

		public IDataReader ExecuteReader(string command, CommandType commandType, CommandBehavior behavior) {
			return ExecuteReader(command, commandType, behavior, null);
		}

		public IDataReader ExecuteReader(string command, CommandType commandType, CommandBehavior behavior, params IDataParameter[] parameters) {
			return ExecuteReader(command, commandType, null, behavior, parameters);
		}

		public IDataReader ExecuteReader(string command, CommandType commandType, ITransaction transaction, CommandBehavior behavior) {
			return ExecuteReader(command, commandType, transaction, behavior, null);
		}

		public IDataReader ExecuteReader(string command, CommandType commandType, ITransaction transaction, CommandBehavior behavior, params IDataParameter[] parameters) {
			if(commandType == CommandType.StoredProcedure) {
				return this.ExecuteReader(command, transaction, behavior, parameters);
			}
			var commandObj = this.CreateTextCommand(command, transaction, parameters);
			return this.ExecuteReader(commandObj, behavior);
		}

		public IDataReader ExecuteReader(string spName, CommandBehavior behavior, params IDataParameter[] parameters) {
			var command = CreateSPCommand(spName, parameters);
			return this.ExecuteReader(command, behavior);
		}

		public IDataReader ExecuteReader(string spName, ITransaction transaction, CommandBehavior behavior, params IDataParameter[] parameters) {
			var command = CreateSPCommand(spName, transaction, parameters);
			return this.ExecuteReader(command, behavior);
		}

		public IDataReader ExecuteReader(string spName, CommandBehavior behavior) {
			var command = CreateSPCommand(spName);
			return this.ExecuteReader(command, behavior);
		}

		public IDataReader ExecuteReader(string spName, ITransaction transaction, CommandBehavior behavior) {
			var command = CreateSPCommand(spName, transaction);
			return this.ExecuteReader(command, behavior);
		}

		public IDataReader ExecuteReader(ICommand command) {
			return ExecuteReader(command, CommandBehavior.CloseConnection);
		}


		public IDataReader ExecuteReader(ICommand command, CommandBehavior behavior) {
			if(command == null)
				throw new ArgumentNullException("command");
			if(command.DbCommand.Connection == null) {
				command.DbCommand.Connection = this.OpenConnection();
			}
			return command.DbCommand.ExecuteReader(behavior);
		}

		public IDataParameter CreateInputParameter(string name, object value) {
			if(value is DateTime){
				return CreateDateInputParameter(name, (DateTime)value);
			}
			if(value == null) {
				value = DBNull.Value;
			}
			var parameter = CreateParameter(name, value);
			parameter.Direction = ParameterDirection.Input;
			return parameter;
		}

		public IDataParameter CreateInputParameter(string name, DbType type, object value) {
			if(value is string){
				return CreateStringInputParameter(name, type, (string)value);
			}
			if(value == null)
				value = DBNull.Value;
			IDataParameter parameter = CreateParameter(name, type);
			parameter.Value = value;
			parameter.Direction = ParameterDirection.Input;
			return parameter;
		}

		public IDataParameter CreateStringInputParameter(string name, DbType type, string value) {
			return CreateStringInputParameter(name, type, value, false);
		}

		public IDataParameter CreateStringInputParameter(string name, DbType type, string value, bool useNullIfEmpty) {
			object pramVal;
			if(value == null || (useNullIfEmpty && value.Length.Equals(0))) {
				pramVal = DBNull.Value;
			} else {
				pramVal = value;
			}
			var parameter = CreateParameter(name, type);
			parameter.Value = pramVal;
			parameter.Direction = ParameterDirection.Input;
			return parameter;
		}

		public IDataParameter CreateDateInputParameter(string name, DateTime value) {
			var pramVal = value == DateTime.MinValue ? DBNull.Value : (object)value;
			var parameter = CreateParameter(name, pramVal);
			parameter.Direction = ParameterDirection.Input;
			return parameter;
		}

		public IDataParameter CreateOutputParameter(string name, DbType type) {
			var parameter = CreateParameter(name, type);
			parameter.Direction = ParameterDirection.Output;
			return parameter;
		}

		public IDataParameter CreateInputOutputParameter(string name, DbType type) {
			var parameter = CreateParameter(name, type);
			parameter.Direction = ParameterDirection.InputOutput;
			return parameter;
		}

		public IDataParameter CreateReturnParameter(string name, DbType type) {
			var parameter = CreateParameter(name, type);
			parameter.Direction = ParameterDirection.ReturnValue;
			return parameter;
		}

	}
}