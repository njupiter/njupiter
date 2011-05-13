using System;
using System.Data;

namespace nJupiter.DataAccess {
	public interface IDataSource {
		
		IProviderFactory DbProviderFactory { get; }

		/// <summary>
		/// Open the <see cref="IDbConnection" /> connection associated with the current data source.
		/// </summary>
		/// <returns><see cref="IDbConnection" /> connection associated with the current data source</returns>
		IDbConnection OpenConnection();

		/// <summary>
		/// Creates a command for the data source.
		/// </summary>
		/// <param name="command">The command string for the command.</param>
		/// <param name="transaction">The transaction associated with the command.</param>
		/// <param name="commandType">Type of command.</param>
		/// <param name="parameters">Parameters associated with the command.</param>
		/// <returns></returns>
		ICommand CreateCommand(string command, IDbTransaction transaction, CommandType commandType, params IDataParameter[] parameters);

		/// <summary>
		/// Creates a data parameter associated with the data source.
		/// </summary>
		/// <param name="name">The name of the paramter.</param>
		/// <param name="type">The type of the parameter.</param>
		/// <returns>An <see cref="IDataParameter" /></returns>
		IDataParameter CreateParameter(string name, DbType type);

		/// <summary>
		/// Creates a data parameter associated with the data source.
		/// </summary>
		/// <param name="name">The name of the paramter.</param>
		/// <param name="value">The value of the paramter.</param>
		/// <returns>An <see cref="IDataParameter"/></returns>
		IDataParameter CreateParameter(string name, object value);

		/// <summary>
		/// Creates a Stored Procedure command for the data source.
		/// </summary>
		/// <returns>A <see cref="Command" /> object.</returns>
		ICommand CreateSPCommand();

		/// <summary>
		/// Creates a Stored Procedure command for the data source.
		/// </summary>
		/// <param name="spName">Name of the stored procedure.</param>
		/// <returns>A <see cref="Command" /> object.</returns>
		ICommand CreateSPCommand(string spName);

		/// <summary>
		/// Creates a Stored Procedure command for the data source.
		/// </summary>
		/// <param name="spName">Name of the stored procedure.</param>
		/// <param name="transaction">The transaction that the command belongs to.</param>
		/// <returns>A <see cref="Command" /> object.</returns>
		ICommand CreateSPCommand(string spName, IDbTransaction transaction);

		/// <summary>
		/// Creates a Stored Procedure command for the data source.
		/// </summary>
		/// <param name="spName">Name of the stored procedure.</param>
		/// <param name="parameters">The parameters that shall be sent to the stored procedure.</param>
		/// <returns>A <see cref="Command" /> object.</returns>
		ICommand CreateSPCommand(string spName, params IDataParameter[] parameters);

		/// <summary>
		/// Creates a Stored Procedure command for the data source.
		/// </summary>
		/// <param name="spName">Name of the stored procedure.</param>
		/// <param name="transaction">The transaction that the command belongs to.</param>
		/// <param name="parameters">The parameters that shall be sent to the stored procedure.</param>
		/// <returns>A <see cref="Command" /> object.</returns>
		ICommand CreateSPCommand(string spName, IDbTransaction transaction, params IDataParameter[] parameters);

		/// <summary>
		/// Creates a text command for the data source.
		/// </summary>
		/// <returns>A <see cref="Command"/> object.</returns>
		ICommand CreateTextCommand();

		/// <summary>
		/// Creates a text command for the data source.
		/// </summary>
		/// <param name="command">The command string.</param>
		/// <returns>A <see cref="Command"/> object.</returns>
		ICommand CreateTextCommand(string command);

		/// <summary>
		/// Creates a text command for the data source.
		/// </summary>
		/// <param name="command">The command string.</param>
		/// <param name="transaction">The transaction that the command belongs to.</param>
		/// <returns>A <see cref="Command"/> object.</returns>
		ICommand CreateTextCommand(string command, IDbTransaction transaction);

		/// <summary>
		/// Creates a text command for the data source.
		/// </summary>
		/// <param name="command">The command string.</param>
		/// <param name="parameters">The parameters that shall be sent to the text command.</param>
		/// <returns>A <see cref="Command"/> object.</returns>
		ICommand CreateTextCommand(string command, params IDataParameter[] parameters);

		/// <summary>
		/// Creates a text command for the data source.
		/// </summary>
		/// <param name="command">The command string.</param>
		/// <param name="transaction">The transaction that the command belongs to.</param>
		/// <param name="parameters">The parameters that shall be sent to the text command.</param>
		/// <returns>A <see cref="Command" /> object.</returns>
		ICommand CreateTextCommand(string command, IDbTransaction transaction, params IDataParameter[] parameters);

		/// <summary>
		/// Gets the resulting data set for a command.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <param name="dataSet">The data set.</param>
		/// <returns>A data set filled with results from the command.</returns>
		DataSet GetDataSet(ICommand command, DataSet dataSet);

		/// <summary>
		/// Gets the resulting data set for a command.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <param name="dataSet">The data set.</param>
		/// <param name="table">The name of the resulting table.</param>
		/// <returns>
		/// A data set filled with results from the command.
		/// </returns>
		DataSet GetDataSet(ICommand command, DataSet dataSet, string table);

		/// <summary>
		/// Gets the resulting data set for a command.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <param name="dataSet">The data set.</param>
		/// <param name="tables">The name of the resulting tables.</param>
		/// <returns>
		/// A data set filled with results from the command.
		/// </returns>
		DataSet GetDataSet(ICommand command, DataSet dataSet, string[] tables);

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
		int UpdateDataSet(DataSet dataSet, ICommand insertCommand, ICommand updateCommand, ICommand deleteCommand, string tableName);

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
		int UpdateDataSet(DataSet dataSet, ICommand insertCommand, ICommand updateCommand, ICommand deleteCommand, string tableName, IDbTransaction transaction);

		/// <summary>
		/// Executes a command that results in a data set.
		/// </summary>
		/// <param name="command">The command text.</param>
		/// <param name="commandType">Type of the command.</param>
		/// <returns>The resulting data set.</returns>
		DataSet ExecuteDataSet(string command, CommandType commandType);

		/// <summary>
		/// Executes a command that results in a data set.
		/// </summary>
		/// <param name="command">The command text.</param>
		/// <param name="commandType">Type of the command.</param>
		/// <param name="parameters">The parameters that shall be used in the command.</param>
		/// <returns>The resulting data set.</returns>
		DataSet ExecuteDataSet(string command, CommandType commandType, params IDataParameter[] parameters);

		/// <summary>
		/// Executes a command that results in a data set.
		/// </summary>
		/// <param name="command">The command text.</param>
		/// <param name="commandType">Type of the command.</param>
		/// <param name="transaction">The transaction that the command belongs to.</param>
		/// <returns>The resulting data set.</returns>
		DataSet ExecuteDataSet(string command, CommandType commandType, IDbTransaction transaction);

		/// <summary>
		/// Executes a command that results in a data set.
		/// </summary>
		/// <param name="command">The command text.</param>
		/// <param name="commandType">Type of the command.</param>
		/// <param name="transaction">The transaction that the command belongs to.</param>
		/// <param name="parameters">The parameters that shall be used in the command.</param>
		/// <returns>The resulting data set.</returns>
		DataSet ExecuteDataSet(string command, CommandType commandType, IDbTransaction transaction, params IDataParameter[] parameters);

		/// <summary>
		/// Executes a command that results in a data set.
		/// </summary>
		/// <param name="spName">Name of the stored procedure that shall be executed.</param>
		/// <param name="parameters">The parameters that shall be used in the command.</param>
		/// <returns>The resulting data set.</returns>
		DataSet ExecuteDataSet(string spName, params IDataParameter[] parameters);

		/// <summary>
		/// Executes a command that results in a data set.
		/// </summary>
		/// <param name="spName">Name of the stored procedure that shall be executed.</param>
		/// <param name="transaction">The transaction that the command belongs to.</param>
		/// <param name="parameters">The parameters that shall be used in the command.</param>
		/// <returns>The resulting data set.</returns>
		DataSet ExecuteDataSet(string spName, IDbTransaction transaction, params IDataParameter[] parameters);

		/// <summary>
		/// Executes a command that results in a data set.
		/// </summary>
		/// <param name="spName">Name of the stored procedure that shall be executed.</param>
		/// <returns>The resulting data set.</returns>
		DataSet ExecuteDataSet(string spName);

		/// <summary>
		/// Executes a command that results in a data set.
		/// </summary>
		/// <param name="spName">Name of the stored procedure that shall be executed.</param>
		/// <param name="transaction">The transaction that the command belongs to.</param>
		/// <returns>The resulting data set.</returns>
		DataSet ExecuteDataSet(string spName, IDbTransaction transaction);

		/// <summary>
		/// Executes a command that results in a data set.
		/// </summary>
		/// <param name="command">The command that shall be executed.</param>
		/// <returns>The resulting data set.</returns>
		DataSet ExecuteDataSet(ICommand command);

		/// <summary>
		/// Runs the command without returning any results.
		/// </summary>
		/// <param name="command">The command text.</param>
		/// <param name="commandType">Type of the command.</param>
		/// <returns>The number of rows affected.</returns>
		int ExecuteNonQuery(string command, CommandType commandType);

		/// <summary>
		/// Runs the command without returning any results.
		/// </summary>
		/// <param name="command">The command text.</param>
		/// <param name="commandType">Type of the command.</param>
		/// <param name="parameters">The parameters that shall be used in the command.</param>
		/// <returns>The number of rows affected.</returns>
		int ExecuteNonQuery(string command, CommandType commandType, params IDataParameter[] parameters);

		/// <summary>
		/// Runs the command without returning any results.
		/// </summary>
		/// <param name="command">The command text.</param>
		/// <param name="commandType">Type of the command.</param>
		/// <param name="transaction">The transaction that the command belongs to.</param>
		/// <returns>The number of rows affected.</returns>
		int ExecuteNonQuery(string command, CommandType commandType, IDbTransaction transaction);

		/// <summary>
		/// Runs the command without returning any results.
		/// </summary>
		/// <param name="command">The command text.</param>
		/// <param name="commandType">Type of the command.</param>
		/// <param name="transaction">The transaction that the command belongs to.</param>
		/// <param name="parameters">The parameters that shall be used in the command.</param>
		/// <returns>The number of rows affected.</returns>
		int ExecuteNonQuery(string command, CommandType commandType, IDbTransaction transaction, params IDataParameter[] parameters);

		/// <summary>
		/// Runs the command without returning any results.
		/// </summary>
		/// <param name="spName">Name of the stored procedure to be executed.</param>
		/// <param name="parameters">The parameters that shall be used in the command.</param>
		/// <returns>The number of rows affected.</returns>
		int ExecuteNonQuery(string spName, params IDataParameter[] parameters);

		/// <summary>
		/// Runs the command without returning any results.
		/// </summary>
		/// <param name="spName">Name of the stored procedure to be executed.</param>
		/// <param name="transaction">The transaction that the command belongs to.</param>
		/// <param name="parameters">The parameters that shall be used in the command.</param>
		/// <returns>The number of rows affected.</returns>
		int ExecuteNonQuery(string spName, IDbTransaction transaction, params IDataParameter[] parameters);

		/// <summary>
		/// Runs the command without returning any results.
		/// </summary>
		/// <param name="spName">Name of the stored procedure to be executed.</param>
		/// <returns>The number of rows affected.</returns>
		int ExecuteNonQuery(string spName);

		/// <summary>
		/// Runs the command without returning any results.
		/// </summary>
		/// <param name="spName">Name of the stored procedure to be executed.</param>
		/// <param name="transaction">The transaction that the command belongs to.</param>
		/// <returns>The number of rows affected.</returns>
		int ExecuteNonQuery(string spName, IDbTransaction transaction);

		/// <summary>
		/// Runs the command without returning any results.
		/// </summary>
		/// <param name="command">The command to be executed.</param>
		/// <returns>The number of rows affected.</returns>
		int ExecuteNonQuery(ICommand command);

		/// <summary>
		/// Executes the query, and returns the first column of the first row in the resultset returned by the query. Extra columns or rows are ignored.
		/// </summary>
		/// <param name="command">The command to execute.</param>
		/// <param name="commandType">Type of the command.</param>
		/// <returns>The first column of the first row in the resultset.</returns>
		object ExecuteScalar(string command, CommandType commandType);

		/// <summary>
		/// Executes the query, and returns the first column of the first row in the resultset returned by the query. Extra columns or rows are ignored.
		/// </summary>
		/// <param name="command">The command to execute.</param>
		/// <param name="commandType">Type of the command.</param>
		/// <param name="parameters">The parameters that shall be used in the command.</param>
		/// <returns>
		/// The first column of the first row in the resultset.
		/// </returns>
		object ExecuteScalar(string command, CommandType commandType, params IDataParameter[] parameters);

		/// <summary>
		/// Executes the query, and returns the first column of the first row in the resultset returned by the query. Extra columns or rows are ignored.
		/// </summary>
		/// <param name="command">The command to execute.</param>
		/// <param name="commandType">Type of the command.</param>
		/// <param name="transaction">The transaction that the command belongs to.</param>
		/// <returns>
		/// The first column of the first row in the resultset.
		/// </returns>
		object ExecuteScalar(string command, CommandType commandType, IDbTransaction transaction);

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
		object ExecuteScalar(string command, CommandType commandType, IDbTransaction transaction, params IDataParameter[] parameters);

		/// <summary>
		/// Executes the query, and returns the first column of the first row in the resultset returned by the query. Extra columns or rows are ignored.
		/// </summary>
		/// <param name="spName">Name of the stored procedure to be executed.</param>
		/// <param name="parameters">The parameters that shall be used by the stored procedure.</param>
		/// <returns>
		/// The first column of the first row in the resultset.
		/// </returns>
		object ExecuteScalar(string spName, params IDataParameter[] parameters);

		/// <summary>
		/// Executes the query, and returns the first column of the first row in the resultset returned by the query. Extra columns or rows are ignored.
		/// </summary>
		/// <param name="spName">Name of the stored procedure to be executed.</param>
		/// <param name="transaction">The transaction that the command belongs to.</param>
		/// <param name="parameters">The parameters that shall be used by the stored procedure.</param>
		/// <returns>
		/// The first column of the first row in the resultset.
		/// </returns>
		object ExecuteScalar(string spName, IDbTransaction transaction, params IDataParameter[] parameters);

		/// <summary>
		/// Executes the query, and returns the first column of the first row in the resultset returned by the query. Extra columns or rows are ignored.
		/// </summary>
		/// <param name="spName">Name of the stored procedure to be executed.</param>
		/// <returns>
		/// The first column of the first row in the resultset.
		/// </returns>
		object ExecuteScalar(string spName);

		/// <summary>
		/// Executes the query, and returns the first column of the first row in the resultset returned by the query. Extra columns or rows are ignored.
		/// </summary>
		/// <param name="spName">Name of the stored procedure to be executed.</param>
		/// <param name="transaction">The transaction that the command belongs to.</param>
		/// <returns>
		/// The first column of the first row in the resultset.
		/// </returns>
		object ExecuteScalar(string spName, IDbTransaction transaction);

		/// <summary>
		/// Executes the query, and returns the first column of the first row in the resultset returned by the query. Extra columns or rows are ignored.
		/// </summary>
		/// <param name="command">The command to be executed.</param>
		/// <returns>
		/// The first column of the first row in the resultset.
		/// </returns>
		object ExecuteScalar(ICommand command);

		/// <summary>
		/// Executes the command and builds an IDataReader.
		/// </summary>
		/// <param name="command">The command to be executed.</param>
		/// <param name="commandType">Type of the command.</param>
		/// <param name="behavior">One of the <see cref="CommandBehavior" /> values.</param>
		/// <returns>An <see cref="IDataReader" /> object.</returns>
		IDataReader ExecuteReader(string command, CommandType commandType, CommandBehavior behavior);

		/// <summary>
		/// Executes the command and builds an IDataReader.
		/// </summary>
		/// <param name="command">The command to be executed.</param>
		/// <param name="commandType">Type of the command.</param>
		/// <param name="behavior">One of the <see cref="CommandBehavior"/> values.</param>
		/// <param name="parameters">The parameters that shall be used in the command.</param>
		/// <returns>An <see cref="IDataReader"/> object.</returns>
		IDataReader ExecuteReader(string command, CommandType commandType, CommandBehavior behavior, params IDataParameter[] parameters);

		/// <summary>
		/// Executes the command and builds an IDataReader.
		/// </summary>
		/// <param name="command">The command to be executed.</param>
		/// <param name="commandType">Type of the command.</param>
		/// <param name="transaction">The transaction that the command belongs to.</param>
		/// <param name="behavior">One of the <see cref="CommandBehavior"/> values.</param>
		/// <returns>An <see cref="IDataReader"/> object.</returns>
		IDataReader ExecuteReader(string command, CommandType commandType, IDbTransaction transaction, CommandBehavior behavior);

		/// <summary>
		/// Executes the command and builds an IDataReader.
		/// </summary>
		/// <param name="command">The command to be executed.</param>
		/// <param name="commandType">Type of the command.</param>
		/// <param name="transaction">The transaction that the command belongs to.</param>
		/// <param name="behavior">One of the <see cref="CommandBehavior"/> values.</param>
		/// <param name="parameters">The parameters that shall be used in the command.</param>
		/// <returns>An <see cref="IDataReader"/> object.</returns>
		IDataReader ExecuteReader(string command, CommandType commandType, IDbTransaction transaction, CommandBehavior behavior, params IDataParameter[] parameters);

		/// <summary>
		/// Executes the command and builds an IDataReader.
		/// </summary>
		/// <param name="spName">Name of the stored procedure to be executed.</param>
		/// <param name="behavior">One of the <see cref="CommandBehavior"/> values.</param>
		/// <param name="parameters">The parameters that shall be used by the stored procedure.</param>
		/// <returns>An <see cref="IDataReader"/> object.</returns>
		IDataReader ExecuteReader(string spName, CommandBehavior behavior, params IDataParameter[] parameters);

		/// <summary>
		/// Executes the command and builds an IDataReader.
		/// </summary>
		/// <param name="spName">Name of the stored procedure to be executed.</param>
		/// <param name="transaction">The transaction that the command belongs to.</param>
		/// <param name="behavior">One of the <see cref="CommandBehavior"/> values.</param>
		/// <param name="parameters">The parameters that shall be used by the stored procedure.</param>
		/// <returns>An <see cref="IDataReader"/> object.</returns>
		IDataReader ExecuteReader(string spName, IDbTransaction transaction, CommandBehavior behavior, params IDataParameter[] parameters);

		/// <summary>
		/// Executes the command and builds an IDataReader.
		/// </summary>
		/// <param name="spName">Name of the stored procedure to be executed.</param>
		/// <param name="behavior">One of the <see cref="CommandBehavior"/> values.</param>
		/// <returns>An <see cref="IDataReader"/> object.</returns>
		IDataReader ExecuteReader(string spName, CommandBehavior behavior);

		/// <summary>
		/// Executes the command and builds an IDataReader.
		/// </summary>
		/// <param name="spName">Name of the stored procedure to be executed.</param>
		/// <param name="transaction">The transaction that the command belongs to.</param>
		/// <param name="behavior">One of the <see cref="CommandBehavior"/> values.</param>
		/// <returns>An <see cref="IDataReader"/> object.</returns>
		IDataReader ExecuteReader(string spName, IDbTransaction transaction, CommandBehavior behavior);

		/// <summary>
		/// Executes the command and builds an IDataReader.
		/// </summary>
		/// <param name="command">The command to be executed.</param>
		/// <returns>An <see cref="IDataReader"/> object.</returns>
		IDataReader ExecuteReader(ICommand command);

		/// <summary>
		/// Executes the command and builds an IDataReader.
		/// </summary>
		/// <param name="command">The command to be executed.</param>
		/// <param name="behavior">One of the <see cref="CommandBehavior"/> values.</param>
		/// <returns>An <see cref="IDataReader"/> object.</returns>
		IDataReader ExecuteReader(ICommand command, CommandBehavior behavior);

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
		IDataParameter CreateInputParameter(string name, object value);

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
		IDataParameter CreateInputParameter(string name, DbType type, object value);

		/// <summary>
		/// Creates a IDataParameter of a specific type form a string
		/// </summary>
		/// <param name="name">The name of parameter.</param>
		/// <param name="type">The DbType of the parameter.</param>
		/// <param name="value">The value of parameter.</param>
		/// <returns>IDataParameter object.</returns>
		IDataParameter CreateStringInputParameter(string name, DbType type, string value);

		/// <summary>
		/// Creates a IDataParameter of a specific type form a string, with an option to create a null
		/// parameter if the string is empty.
		/// </summary>
		/// <param name="name">The name of parameter.</param>
		/// <param name="type">The DbType of the parameter.</param>
		/// <param name="value">The value of parameter.</param>
		/// <param name="useNullIfEmpty">true to create a parameter with a null value if the string is empty, otherwise false.</param>
		/// <returns>IDataParameter object.</returns>
		IDataParameter CreateStringInputParameter(string name, DbType type, string value, bool useNullIfEmpty);

		/// <summary>
		/// Creates an IDataParameter for a date datetype. If the DateTime structure has not been
		/// initialised, the parameter is set to a null value.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns>IDataParameter object.</returns>
		IDataParameter CreateDateInputParameter(string name, DateTime value);

		/// <summary>
		/// Creates an output parameter.
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="type">The DbType of the parameter.</param>
		/// <returns></returns>
		IDataParameter CreateOutputParameter(string name, DbType type);

		/// <summary>
		/// Creates an inputoutput parameter.
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="type">The DbType of the parameter.</param>
		/// <returns></returns>
		IDataParameter CreateInputOutputParameter(string name, DbType type);

		/// <summary>
		/// Creates an return parameter.
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="type">The DbType of the parameter.</param>
		/// <returns></returns>
		IDataParameter CreateReturnParameter(string name, DbType type);
	}
}