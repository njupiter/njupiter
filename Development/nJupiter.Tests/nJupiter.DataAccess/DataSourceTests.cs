using System;
using System.Data;
using System.Linq;

using FakeItEasy;

using nJupiter.DataAccess;

using NUnit.Framework;

namespace nJupiter.Tests.DataAccess {
	
	[TestFixture]
	public class DataSourceTests {

		[Test]
		public void CreateCommand_CreateCommand_CheckThatCorrectCommandIsCreated() {
			var provider = A.Fake<IProvider>();
			var dataSource = new DataSource(provider);
			var transaction = A.Fake<IDbTransaction>();
			var parameter = A.Fake<IDataParameter>();

			var command = dataSource.CreateCommand("commandSting", transaction, CommandType.TableDirect, parameter);
			Assert.AreEqual("commandSting", command.DbCommand.CommandText);
			Assert.AreEqual(transaction.Connection, command.DbCommand.Connection);
			Assert.AreEqual(transaction, command.DbCommand.Transaction);
			Assert.AreEqual(CommandType.TableDirect, command.DbCommand.CommandType);
		}

		[Test]
		public void CreateParameter_CreateParameter_CheckThatCorrectParameterIsCreated() {
			var provider = A.Fake<IProvider>();
			var dataSource = new DataSource(provider);

			var parameter = dataSource.CreateParameter("myparameter", DbType.Guid);
			Assert.AreEqual("myparameter", parameter.ParameterName);
			Assert.AreEqual(DbType.Guid, parameter.DbType);
		}

		[Test]
		public void CreateParameter_CreateParameterWithValue_CheckThatCorrectValueIsSet() {
			var provider = A.Fake<IProvider>();
			var dataSource = new DataSource(provider);
			var parameter = dataSource.CreateParameter("myparameter", "myvalue");
			Assert.AreEqual("myparameter", parameter.ParameterName);
			Assert.AreEqual("myvalue", parameter.Value);
		}

		[Test]
		public void OpenConnection_CreateProviderWithConnectionAndOpenIt_ReturnsUnderlyingOpenedConnection() {
			var provider = A.Fake<IProvider>();
			var underlyingConnection = A.Fake<IDbConnection>();
			A.CallTo(() => provider.CreateConnection()).Returns(underlyingConnection);
			var dataSource = new DataSource(provider);
			var connection = dataSource.OpenConnection();
			A.CallTo(() => underlyingConnection.Open()).MustHaveHappened(Repeated.Exactly.Once);
			Assert.AreEqual(underlyingConnection, connection);
		}


		[Test]
		public void CreateSPCommand_CreatePlainCommand_ReturnsCommandWithTypeStoredProcedure() {
			var provider = A.Fake<IProvider>();
			var dataSource = new DataSource(provider);
			var command = dataSource.CreateSPCommand();
			Assert.AreEqual(CommandType.StoredProcedure, command.DbCommand.CommandType);
		}

		[Test]
		public void CreateSPCommand_CreateCommandWithName_ReturnsCommandWithCorrectSpName() {
			var provider = A.Fake<IProvider>();
			var dataSource = new DataSource(provider);
			var command = dataSource.CreateSPCommand("command");
			Assert.AreEqual(CommandType.StoredProcedure, command.DbCommand.CommandType);
			Assert.AreEqual("command", command.DbCommand.CommandText);
		}

		[Test]
		public void CreateSPCommand_CreateSpCommandWithTransaction_ReturnsCommandWithCorrectTransaction() {
			var provider = A.Fake<IProvider>();
			var dataSource = new DataSource(provider);
			var trans = A.Fake<IDbTransaction>();
			var command = dataSource.CreateSPCommand("command", trans);
			Assert.AreEqual(CommandType.StoredProcedure, command.DbCommand.CommandType);
			Assert.AreEqual("command", command.DbCommand.CommandText);
			Assert.AreEqual(trans, command.DbCommand.Transaction);
		}

		[Test]
		public void CreateSPCommand_CreateSpCommandWithParameters_ReturnsCommandWithCorrectParameters() {
			var provider = A.Fake<IProvider>();
			var dbCommand = A.Fake<IDbCommand>();
			var parameterCollection = A.Fake<IDataParameterCollection>();
			
			A.CallTo(() => provider.CreateCommand()).Returns(dbCommand);
			A.CallTo(() => dbCommand.Parameters).Returns(parameterCollection);


			var dataSource = new DataSource(provider);
			var parameters = A.CollectionOfFake<IDataParameter>(12).ToArray();
			var command = dataSource.CreateSPCommand("command", parameters);

			A.CallTo(() => command.DbCommand.Parameters).Returns(parameterCollection);
			Assert.AreEqual(dbCommand, command.DbCommand);
			Assert.AreEqual(CommandType.StoredProcedure, command.DbCommand.CommandType);
			Assert.AreEqual("command", command.DbCommand.CommandText);
			A.CallTo(() => parameterCollection.Add(A<object>.Ignored)).MustHaveHappened(Repeated.Exactly.Times(12));
		}

		[Test]
		public void CreateTextCommand_CreatePlainCommand_ReturnsCommandWithTypeStoredProcedure() {
			var provider = A.Fake<IProvider>();
			var dataSource = new DataSource(provider);
			var command = dataSource.CreateTextCommand();
			Assert.AreEqual(CommandType.Text, command.DbCommand.CommandType);
		}

		[Test]
		public void CreateTextCommand_CreateTextCommandWithName_ReturnsCommandWithCorrectCommandText() {
			var provider = A.Fake<IProvider>();
			var dataSource = new DataSource(provider);
			var command = dataSource.CreateTextCommand("command");
			Assert.AreEqual(CommandType.Text, command.DbCommand.CommandType);
			Assert.AreEqual("command", command.DbCommand.CommandText);
		}

		[Test]
		public void CreateTextCommand_CreateTextCommandWithTransaction_ReturnsCommandWithCorrectTransaction() {
			var provider = A.Fake<IProvider>();
			var dataSource = new DataSource(provider);
			var trans = A.Fake<IDbTransaction>();
			var command = dataSource.CreateTextCommand("command", trans);
			Assert.AreEqual(CommandType.Text, command.DbCommand.CommandType);
			Assert.AreEqual("command", command.DbCommand.CommandText);
			Assert.AreEqual(trans, command.DbCommand.Transaction);
		}

		[Test]
		public void CreateTextCommand_CreateTextCommandWithParameters_ReturnsCommandWithCorrectParameters() {
			var provider = A.Fake<IProvider>();
			var dbCommand = A.Fake<IDbCommand>();
			var parameterCollection = A.Fake<IDataParameterCollection>();
			
			A.CallTo(() => provider.CreateCommand()).Returns(dbCommand);
			A.CallTo(() => dbCommand.Parameters).Returns(parameterCollection);


			var dataSource = new DataSource(provider);
			var parameters = A.CollectionOfFake<IDataParameter>(12).ToArray();
			var command = dataSource.CreateTextCommand("command", parameters);

			A.CallTo(() => command.DbCommand.Parameters).Returns(parameterCollection);
			Assert.AreEqual(dbCommand, command.DbCommand);
			Assert.AreEqual(CommandType.Text, command.DbCommand.CommandType);
			Assert.AreEqual("command", command.DbCommand.CommandText);
			A.CallTo(() => parameterCollection.Add(A<object>.Ignored)).MustHaveHappened(Repeated.Exactly.Times(12));
		}


		[Test]
		public void GetDataSet_PassDataset_DatasetFilled() {
			var provider = A.Fake<IProvider>();
			var dataAdapter = A.Fake<IDbDataAdapter>();
			var command = A.Fake<ICommand>();
			var dataSource = new DataSource(provider);
			A.CallTo(() => provider.CreateDataAdapter()).Returns(dataAdapter);
			A.CallTo(() => command.Transaction).Returns(null);
			var dataset = new DataSet();
			dataSource.GetDataSet(command, dataset);
			A.CallTo(() => dataAdapter.Fill(dataset)).MustHaveHappened(Repeated.Exactly.Once);
		}

		[Test]
		public void GetDataSet_PassDataset_DatasetFilledWithSeveralTabels() {
			var provider = A.Fake<IProvider>();
			var dataAdapter = A.Fake<IDbDataAdapter>();
			var command = A.Fake<ICommand>();
			var dataSource = new DataSource(provider);
			A.CallTo(() => provider.CreateDataAdapter()).Returns(dataAdapter);
			var dataset = new DataSet();
			dataSource.GetDataSet(command, dataset, new []{ "myTable1", "myTable2" });
			A.CallTo(() => dataAdapter.Fill(dataset)).MustHaveHappened(Repeated.Exactly.Once);
			A.CallTo(() => dataAdapter.TableMappings.Add(A<string>.Ignored, "myTable1")).MustHaveHappened(Repeated.Exactly.Once);
			A.CallTo(() => dataAdapter.TableMappings.Add(A<string>.Ignored, "myTable2")).MustHaveHappened(Repeated.Exactly.Once);
		}

		[Test]
		public void GetDataSet_PassNullCommand_ThrowsArgumentNullException() {
			var provider = A.Fake<IProvider>();
			var dataSource = new DataSource(provider);
			var dataset = new DataSet();
			Assert.Throws<ArgumentNullException>(() => dataSource.GetDataSet(null, dataset));
		}

		[Test]
		public void UpdateDataSet_PassAdapter_UpdateOnAdapterCalled() {
			var provider = A.Fake<IProvider>();
			var dataAdapter = A.Fake<IDbDataAdapter>();
			var dataSource = new DataSource(provider);
			A.CallTo(() => provider.CreateDataAdapter()).Returns(dataAdapter);

			var insertCommand = A.Fake<ICommand>();
			var updateCommand = A.Fake<ICommand>();
			var deleteCommand = A.Fake<ICommand>();

			var dataset = new DataSet();
			dataSource.UpdateDataSet(dataset, insertCommand, updateCommand, deleteCommand);

			A.CallTo(() => dataAdapter.Update(dataset)).MustHaveHappened(Repeated.Exactly.Once);
		}

		[Test]
		public void UpdateDataSet_PassAdapter_AddapterGetCommands() {
			var provider = A.Fake<IProvider>();
			var dataAdapter = A.Fake<IDbDataAdapter>();
			var dataSource = new DataSource(provider);
			A.CallTo(() => provider.CreateDataAdapter()).Returns(dataAdapter);

			var insertCommand = A.Fake<ICommand>();
			var updateCommand = A.Fake<ICommand>();
			var deleteCommand = A.Fake<ICommand>();

			var dataset = new DataSet();
			dataSource.UpdateDataSet(dataset, insertCommand, updateCommand, deleteCommand, "myTable");

			A.CallTo(() => dataAdapter.TableMappings.Add(A<string>.Ignored, "myTable")).MustHaveHappened(Repeated.Exactly.Once);
			Assert.AreEqual(insertCommand.DbCommand, dataAdapter.InsertCommand);
			Assert.AreEqual(updateCommand.DbCommand, dataAdapter.UpdateCommand);
			Assert.AreEqual(deleteCommand.DbCommand, dataAdapter.DeleteCommand);
		}

		[Test]
		public void UpdateDataSet_PassingNullTransaction_ThrowsArgumentNullExeption() {
			var provider = A.Fake<IProvider>();
			var dataSource = new DataSource(provider);

			var insertCommand = A.Fake<ICommand>();
			var updateCommand = A.Fake<ICommand>();
			var deleteCommand = A.Fake<ICommand>();

			var dataset = new DataSet();
			Assert.Throws<ArgumentNullException>(() => dataSource.UpdateDataSet(dataset, insertCommand, updateCommand, deleteCommand, "myTable", null));

		}


		[Test]
		public void UpdateDataSet_PassingNullDataSet_ThrowsArgumentNullExeption() {
			var provider = A.Fake<IProvider>();
			var dataSource = new DataSource(provider);

			var insertCommand = A.Fake<ICommand>();
			var updateCommand = A.Fake<ICommand>();
			var deleteCommand = A.Fake<ICommand>();

			Assert.Throws<ArgumentNullException>(() => dataSource.UpdateDataSet(null, insertCommand, updateCommand, deleteCommand));

		}

		[Test]
		public void ExecuteDataSet_ExecuteCommand_ReturnsDataset() {
			var provider = A.Fake<IProvider>();
			var dataSource = new DataSource(provider);

			var dataSet = dataSource.ExecuteDataSet("command", CommandType.StoredProcedure);
			
			Assert.IsNotNull(dataSet);
		}

		[Test]
		public void ExecuteDataSet_ExecuteCommandWithParameters_ReturnsDataset() {
			var provider = A.Fake<IProvider>();
			var dataAdapter = A.Fake<IDbDataAdapter>();
			var dataSource = new DataSource(provider);
			var dbCommand = A.Fake<IDbCommand>();

			A.CallTo(() => provider.CreateDataAdapter()).Returns(dataAdapter);
			A.CallTo(() => provider.CreateCommand()).Returns(dbCommand);

			var parameters = A.CollectionOfFake<IDataParameter>(12).ToArray();

			var result = dataSource.ExecuteDataSet("command", CommandType.Text, parameters);

			A.CallTo(() => dbCommand.Parameters.Add(A<object>.Ignored)).MustHaveHappened(Repeated.Exactly.Times(12));
			
			Assert.IsNotNull(result);
		}

		[Test]
		public void ExecuteDataSet_ExecuteSpWithParameters_ReturnsDataset() {
			var provider = A.Fake<IProvider>();
			var dataAdapter = A.Fake<IDbDataAdapter>();
			var dataSource = new DataSource(provider);
			var dbCommand = A.Fake<IDbCommand>();

			A.CallTo(() => provider.CreateDataAdapter()).Returns(dataAdapter);
			A.CallTo(() => provider.CreateCommand()).Returns(dbCommand);

			var parameters = A.CollectionOfFake<IDataParameter>(12).ToArray();

			var result = dataSource.ExecuteDataSet("spname", parameters);

			A.CallTo(() => dbCommand.Parameters.Add(A<object>.Ignored)).MustHaveHappened(Repeated.Exactly.Times(12));
			
			Assert.IsNotNull(result);
		}

		[Test]
		public void ExecuteDataSet_ExecuteSp_ReturnsDataset() {
			var provider = A.Fake<IProvider>();
			var dataSource = new DataSource(provider);

			var dataSet = dataSource.ExecuteDataSet("command");
			
			Assert.IsNotNull(dataSet);
		}

		[Test]
		public void ExecuteDataSet_PassingNullCommand_ThrowsArgumentNullException() {
			var provider = A.Fake<IProvider>();
			var dataSource = new DataSource(provider);

			Assert.Throws<ArgumentNullException>(() => dataSource.ExecuteDataSet((ICommand)null));
		}

		[Test]
		public void ExecuteNonQuery_ExecuteCommand_ReturnsInt() {
			var provider = A.Fake<IProvider>();
			var dataSource = new DataSource(provider);
			var dbCommand = A.Fake<IDbCommand>();
			A.CallTo(() => provider.CreateCommand()).Returns(dbCommand);
			A.CallTo(() => dbCommand.ExecuteNonQuery()).Returns(12);

			var result = dataSource.ExecuteNonQuery("command", CommandType.StoredProcedure);
			
			Assert.AreEqual(12, result);
		}

		[Test]
		public void ExecuteNonQuery_ExecuteCommandWithParameters_ReturnsDataset() {
			var provider = A.Fake<IProvider>();
			var dataAdapter = A.Fake<IDbDataAdapter>();
			var dataSource = new DataSource(provider);
			var dbCommand = A.Fake<IDbCommand>();

			A.CallTo(() => provider.CreateDataAdapter()).Returns(dataAdapter);
			A.CallTo(() => provider.CreateCommand()).Returns(dbCommand);

			var parameters = A.CollectionOfFake<IDataParameter>(12).ToArray();

			dataSource.ExecuteNonQuery("command", CommandType.Text, parameters);

			A.CallTo(() => dbCommand.Parameters.Add(A<object>.Ignored)).MustHaveHappened(Repeated.Exactly.Times(12));
			
		}

		[Test]
		public void ExecuteNonQuery_ExecuteSpWithParameters_ReturnsDataset() {
			var provider = A.Fake<IProvider>();
			var dataAdapter = A.Fake<IDbDataAdapter>();
			var dataSource = new DataSource(provider);
			var dbCommand = A.Fake<IDbCommand>();

			A.CallTo(() => provider.CreateDataAdapter()).Returns(dataAdapter);
			A.CallTo(() => provider.CreateCommand()).Returns(dbCommand);

			var parameters = A.CollectionOfFake<IDataParameter>(12).ToArray();

			dataSource.ExecuteNonQuery("spname", parameters);

			A.CallTo(() => dbCommand.Parameters.Add(A<object>.Ignored)).MustHaveHappened(Repeated.Exactly.Times(12));
			
		}

		[Test]
		public void ExecuteNonQuery_ExecuteSp_ReturnsDataset() {
			var provider = A.Fake<IProvider>();
			var dataSource = new DataSource(provider);
			var dbCommand = A.Fake<IDbCommand>();
			A.CallTo(() => provider.CreateCommand()).Returns(dbCommand);
			A.CallTo(() => dbCommand.ExecuteNonQuery()).Returns(242);

			var result = dataSource.ExecuteNonQuery("command");
			
			Assert.AreEqual(242, result);
		}

		[Test]
		public void ExecuteNonQuery_ExecuteCommandWithoutTransaction_TransactionCreatedAfterExecute() {
			var provider = A.Fake<IProvider>();
			var command = A.Fake<ICommand>();
			var dataSource = new DataSource(provider);
			command.Transaction = null;

			dataSource.ExecuteNonQuery(command);

			Assert.IsNotNull(command.Transaction);
		}

		[Test]
		public void ExecuteNonQuery_PassingNullCommand_ThrowsArgumentNullException() {
			var provider = A.Fake<IProvider>();
			var dataSource = new DataSource(provider);

			Assert.Throws<ArgumentNullException>(() => dataSource.ExecuteNonQuery((ICommand)null));
		}

		[Test]
		public void ExecuteScalar_ExecuteCommand_ReturnsInt() {
			var provider = A.Fake<IProvider>();
			var dataSource = new DataSource(provider);
			var dbCommand = A.Fake<IDbCommand>();
			var dummyObject = A.Fake<MyDummyClass>();

			A.CallTo(() => provider.CreateCommand()).Returns(dbCommand);
			A.CallTo(() => dbCommand.ExecuteScalar()).Returns(dummyObject);

			var result = dataSource.ExecuteScalar("command", CommandType.StoredProcedure);
			
			Assert.AreEqual(dummyObject, result);
		}

		[Test]
		public void ExecuteScalar_ExecuteCommandWithParameters_ReturnsDataset() {
			var provider = A.Fake<IProvider>();
			var dataAdapter = A.Fake<IDbDataAdapter>();
			var dataSource = new DataSource(provider);
			var dbCommand = A.Fake<IDbCommand>();

			A.CallTo(() => provider.CreateDataAdapter()).Returns(dataAdapter);
			A.CallTo(() => provider.CreateCommand()).Returns(dbCommand);

			var parameters = A.CollectionOfFake<IDataParameter>(12).ToArray();

			dataSource.ExecuteScalar("command", CommandType.Text, parameters);

			A.CallTo(() => dbCommand.Parameters.Add(A<object>.Ignored)).MustHaveHappened(Repeated.Exactly.Times(12));
			
		}

		[Test]
		public void ExecuteScalar_ExecuteSpWithParameters_ReturnsDataset() {
			var provider = A.Fake<IProvider>();
			var dataAdapter = A.Fake<IDbDataAdapter>();
			var dataSource = new DataSource(provider);
			var dbCommand = A.Fake<IDbCommand>();

			A.CallTo(() => provider.CreateDataAdapter()).Returns(dataAdapter);
			A.CallTo(() => provider.CreateCommand()).Returns(dbCommand);

			var parameters = A.CollectionOfFake<IDataParameter>(12).ToArray();

			dataSource.ExecuteScalar("spname", parameters);

			A.CallTo(() => dbCommand.Parameters.Add(A<object>.Ignored)).MustHaveHappened(Repeated.Exactly.Times(12));
			
		}

		[Test]
		public void ExecuteScalar_ExecuteSp_ReturnsDataset() {
			var provider = A.Fake<IProvider>();
			var dataSource = new DataSource(provider);
			var dbCommand = A.Fake<IDbCommand>();
			var dummyObject = A.Fake<MyDummyClass>();

			A.CallTo(() => provider.CreateCommand()).Returns(dbCommand);
			A.CallTo(() => dbCommand.ExecuteScalar()).Returns(dummyObject);

			var result = dataSource.ExecuteScalar("command");
			
			Assert.AreEqual(dummyObject, result);
		}

		[Test]
		public void ExecuteScalar_ExecuteCommandWithoutTransaction_TransactionCreatedAfterExecute() {
			var provider = A.Fake<IProvider>();
			var command = A.Fake<ICommand>();
			var dataSource = new DataSource(provider);
			command.Transaction = null;

			dataSource.ExecuteScalar(command);

			Assert.IsNotNull(command.Transaction);
		}

		[Test]
		public void ExecuteScalar_PassingNullCommand_ThrowsArgumentNullException() {
			var provider = A.Fake<IProvider>();
			var dataSource = new DataSource(provider);

			Assert.Throws<ArgumentNullException>(() => dataSource.ExecuteScalar((ICommand)null));
		}

		[Test]
		public void ExecuteReader_ExecuteCommand_ReturnsInt() {
			var provider = A.Fake<IProvider>();
			var dataSource = new DataSource(provider);
			var dbCommand = A.Fake<IDbCommand>();
			var dummyObject = A.Fake<IDataReader>();

			A.CallTo(() => provider.CreateCommand()).Returns(dbCommand);
			A.CallTo(() => dbCommand.ExecuteReader(CommandBehavior.SchemaOnly)).Returns(dummyObject);

			var result = dataSource.ExecuteReader("command", CommandType.StoredProcedure, CommandBehavior.SchemaOnly);
			
			Assert.AreEqual(dummyObject, result);
			A.CallTo(() => dbCommand.ExecuteReader(CommandBehavior.SchemaOnly)).MustHaveHappened(Repeated.Exactly.Once);
		}

		[Test]
		public void ExecuteReader_ExecuteCommandWithParameters_ReturnsDataset() {
			var provider = A.Fake<IProvider>();
			var dataAdapter = A.Fake<IDbDataAdapter>();
			var dataSource = new DataSource(provider);
			var dbCommand = A.Fake<IDbCommand>();

			A.CallTo(() => provider.CreateDataAdapter()).Returns(dataAdapter);
			A.CallTo(() => provider.CreateCommand()).Returns(dbCommand);

			var parameters = A.CollectionOfFake<IDataParameter>(12).ToArray();

			dataSource.ExecuteReader("command", CommandType.Text, CommandBehavior.SingleResult, parameters);

			A.CallTo(() => dbCommand.Parameters.Add(A<object>.Ignored)).MustHaveHappened(Repeated.Exactly.Times(12));
			A.CallTo(() => dbCommand.ExecuteReader(CommandBehavior.SingleResult)).MustHaveHappened(Repeated.Exactly.Once);
			
		}

		[Test]
		public void ExecuteReader_ExecuteSpWithParameters_ReturnsDataset() {
			var provider = A.Fake<IProvider>();
			var dataAdapter = A.Fake<IDbDataAdapter>();
			var dataSource = new DataSource(provider);
			var dbCommand = A.Fake<IDbCommand>();

			A.CallTo(() => provider.CreateDataAdapter()).Returns(dataAdapter);
			A.CallTo(() => provider.CreateCommand()).Returns(dbCommand);

			var parameters = A.CollectionOfFake<IDataParameter>(12).ToArray();

			dataSource.ExecuteReader("spname", CommandBehavior.Default, parameters);

			A.CallTo(() => dbCommand.Parameters.Add(A<object>.Ignored)).MustHaveHappened(Repeated.Exactly.Times(12));
			A.CallTo(() => dbCommand.ExecuteReader(CommandBehavior.Default)).MustHaveHappened(Repeated.Exactly.Once);
			
		}

		[Test]
		public void ExecuteReader_ExecuteSp_ReturnsDataset() {
			var provider = A.Fake<IProvider>();
			var dataSource = new DataSource(provider);
			var dbCommand = A.Fake<IDbCommand>();
			var dummyObject = A.Fake<IDataReader>();

			A.CallTo(() => provider.CreateCommand()).Returns(dbCommand);
			A.CallTo(() => dbCommand.ExecuteReader(CommandBehavior.KeyInfo)).Returns(dummyObject);

			var result = dataSource.ExecuteReader("command", CommandBehavior.KeyInfo);
			
			Assert.AreEqual(dummyObject, result);
			A.CallTo(() => dbCommand.ExecuteReader(CommandBehavior.KeyInfo)).MustHaveHappened(Repeated.Exactly.Once);
		}

		[Test]
		public void ExecuteReader_ExecuteCommandWithTransaction_TransactionNotCommited() {
			var provider = A.Fake<IProvider>();
			var command = A.Fake<ICommand>();
			var dataSource = new DataSource(provider);
			var trans = A.Fake<IDbTransaction>();

			command.Transaction = trans;

			dataSource.ExecuteReader(command);
			
			A.CallTo(() => trans.Commit()).MustNotHaveHappened();
		}

		[Test]
		public void ExecuteReader_ExecuteTextCommandPassingTransaction_TransactionSetOnCommand() {
			var provider = A.Fake<IProvider>();
			var dbCommand = A.Fake<IDbCommand>();
			var dataSource = new DataSource(provider);
			var trans = A.Fake<IDbTransaction>();

			A.CallTo(() => provider.CreateCommand()).Returns(dbCommand);

			dataSource.ExecuteReader("command", CommandType.Text, trans, CommandBehavior.KeyInfo);
			
			Assert.AreEqual("command", dbCommand.CommandText);
			Assert.AreEqual(trans, dbCommand.Transaction);
		}

		[Test]
		public void ExecuteReader_Passingnull() {
			var provider = A.Fake<IProvider>();
			var dataSource = new DataSource(provider);
			var trans = A.Fake<IDbTransaction>();

			dataSource.ExecuteReader(null, trans, CommandBehavior.KeyInfo);
		}


		[Test]
		public void ExecuteReader_ExecuteCommandWithoutTransaction_TransactionCreatedAfterExecute() {
			var provider = A.Fake<IProvider>();
			var command = A.Fake<ICommand>();
			var conn = A.Fake<IDbConnection>();
			var dataSource = new DataSource(provider);
			command.DbCommand.Connection = null;

			A.CallTo(() => provider.CreateConnection()).Returns(conn);

			dataSource.ExecuteReader(command);

			Assert.AreEqual(conn, command.DbCommand.Connection);
			A.CallTo(() => conn.Open()).MustHaveHappened(Repeated.Exactly.Once);
		}

		[Test]
		public void ExecuteReader_PassingNullCommand_ThrowsArgumentNullException() {
			var provider = A.Fake<IProvider>();
			var dataSource = new DataSource(provider);

			Assert.Throws<ArgumentNullException>(() => dataSource.ExecuteReader(null));
		}

		[Test]
		public void CreateInputParameter_PassingNullValue_ReturnsImputParameterWithDbNullAsValue() {
			var provider = A.Fake<IProvider>();
			var dataSource = new DataSource(provider);

			var parameter = dataSource.CreateInputParameter("myParamter", null);
			Assert.AreEqual("myParamter", parameter.ParameterName);
			Assert.AreEqual(DBNull.Value, parameter.Value);
			Assert.AreEqual(ParameterDirection.Input, parameter.Direction);
		}

		[Test]
		public void CreateInputParameter_PassingDateTimeMinValue_ReturnsImputParameterWithDbNullAsValue() {
			var provider = A.Fake<IProvider>();
			var dataSource = new DataSource(provider);

			var parameter = dataSource.CreateInputParameter("myParamter", DateTime.MinValue);
			Assert.AreEqual("myParamter", parameter.ParameterName);
			Assert.AreEqual(DBNull.Value, parameter.Value);
			Assert.AreEqual(ParameterDirection.Input, parameter.Direction);
		}

		[Test]
		public void CreateInputParameter_PassingAnsiStringNullValue_ReturnsImputParameterWithDbNullAsValue() {
			var provider = A.Fake<IProvider>();
			var dataSource = new DataSource(provider);

			string dummyString = null;
			var parameter = dataSource.CreateInputParameter("myParamter", DbType.AnsiString, dummyString);
			Assert.AreEqual("myParamter", parameter.ParameterName);
			Assert.AreEqual(DBNull.Value, parameter.Value);
			Assert.AreEqual(ParameterDirection.Input, parameter.Direction);
			Assert.AreEqual(DbType.AnsiString, parameter.DbType);
		}

		[Test]
		public void CreateInputParameter_PassingDummyObjectNullValue_ReturnsImputParameterWithDbNullAsValue() {
			var provider = A.Fake<IProvider>();
			var dataSource = new DataSource(provider);
			
			MyDummyClass myDummyObject = null;
			var parameter = dataSource.CreateInputParameter("myParamter", DbType.AnsiString, myDummyObject);
			Assert.AreEqual("myParamter", parameter.ParameterName);
			Assert.AreEqual(DBNull.Value, parameter.Value);
			Assert.AreEqual(ParameterDirection.Input, parameter.Direction);
			Assert.AreEqual(DbType.AnsiString, parameter.DbType);
		}

		[Test]
		public void CreateInputParameter_PassingAnsiStringValue_ReturnsImputParameterWithCorrectValue() {
			var provider = A.Fake<IProvider>();
			var dataSource = new DataSource(provider);

			const string dummyString = "myValue";
			var parameter = dataSource.CreateInputParameter("myParamter", DbType.AnsiString, dummyString);
			Assert.AreEqual("myParamter", parameter.ParameterName);
			Assert.AreEqual(dummyString, parameter.Value);
			Assert.AreEqual(ParameterDirection.Input, parameter.Direction);
			Assert.AreEqual(DbType.AnsiString, parameter.DbType);
		}


		[Test]
		public void CreateStringInputParameter_PassingAnsiStringEmptyValueAndUseNullIfEmpty_ReturnsImputParameterWithDbNullValue() {
			var provider = A.Fake<IProvider>();
			var dataSource = new DataSource(provider);

			string dummyString = string.Empty;
			var parameter = dataSource.CreateStringInputParameter("myParamter", DbType.AnsiString, dummyString, true);
			Assert.AreEqual("myParamter", parameter.ParameterName);
			Assert.AreEqual(DBNull.Value, parameter.Value);
			Assert.AreEqual(ParameterDirection.Input, parameter.Direction);
			Assert.AreEqual(DbType.AnsiString, parameter.DbType);
		}

		public class MyDummyClass{}

	}
}
