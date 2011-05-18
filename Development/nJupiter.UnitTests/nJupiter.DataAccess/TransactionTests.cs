using System;
using System.Data;

using FakeItEasy;

using nJupiter.DataAccess;

using NUnit.Framework;

namespace nJupiter.UnitTests.DataAccess {

	[TestFixture]
	public class TransactionTests {

		[Test]
		public void BeginTransaction_ReturnTransactionThatOpenConnection_CosingConncetionWhenTransactionIsOver() {
			var dataSource = A.Fake<IDataSource>();
			var connection = A.Fake<IDbConnection>();
			A.CallTo(() => dataSource.OpenConnection()).Returns(connection);
			using(IDbTransaction transaction = TransactionFactory.BeginTransaction(dataSource)) {
				A.CallTo(() => connection.State).Returns(ConnectionState.Open);
				A.CallTo(() => dataSource.OpenConnection()).MustHaveHappened(Repeated.Exactly.Once);
				A.CallTo(() => connection.BeginTransaction(IsolationLevel.ReadCommitted)).MustHaveHappened(Repeated.Exactly.Once);
				A.CallTo(() => connection.Close()).MustNotHaveHappened();
				transaction.Commit();
			}
			A.CallTo(() => connection.State).Returns(ConnectionState.Closed);
			A.CallTo(() => connection.Close()).MustHaveHappened(Repeated.Exactly.Once);
		}

		[Test]
		public void BeginTransaction_ReturnTransactionWithIsolationLeverlReadUncommitted_TestSoTransactionWithRightIsolationLevelIsStarted() {
			var dataSource = A.Fake<IDataSource>();
			var connection = A.Fake<IDbConnection>();
			A.CallTo(() => dataSource.OpenConnection()).Returns(connection);
			using(TransactionFactory.BeginTransaction(dataSource, IsolationLevel.ReadUncommitted)) {
				A.CallTo(() => dataSource.OpenConnection()).MustHaveHappened(Repeated.Exactly.Once);
				A.CallTo(() => connection.BeginTransaction(IsolationLevel.ReadUncommitted)).MustHaveHappened(Repeated.Exactly.Once);
			}
		}

		[Test]
		public void Rollback_RunTranactionAndRoleback_RollbackOnTheUnderlyingObjectIsCalled() {
			var underlyingTrans = A.Fake<IDbTransaction>();
			var connection = new FakeConnection(underlyingTrans);
			var dataSource = A.Fake<IDataSource>();
			A.CallTo(() => dataSource.OpenConnection()).Returns(connection);
			using(var trans = TransactionFactory.BeginTransaction(dataSource, IsolationLevel.Chaos)) {
				var myCommand = dataSource.CreateSPCommand("exec storedprocedure", trans);
				dataSource.ExecuteDataSet(myCommand);
				dataSource.ExecuteNonQuery("select * from table", CommandType.Text, trans);
				trans.Rollback();
				A.CallTo(() => underlyingTrans.Rollback()).MustHaveHappened(Repeated.Exactly.Once);
				Assert.AreEqual(IsolationLevel.Chaos, trans.IsolationLevel);
			}
		}

	}

	public class FakeConnection : IDbConnection {
		private readonly IDbTransaction transaction;

		public IDbTransaction Transaction { get { return transaction; } }

		public FakeConnection( IDbTransaction transaction) {
			this.transaction = transaction;
		}

		public void Dispose() {
		}

		public IDbTransaction BeginTransaction() {
			return transaction;
		}

		public IDbTransaction BeginTransaction(IsolationLevel il) {
			return transaction;
		}

		public void Close() {
		}

		public void ChangeDatabase(string databaseName) {
		}

		public IDbCommand CreateCommand() {
			return A.Fake<IDbCommand>();
		}

		public void Open() {
		}

		public string ConnectionString { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
		public int ConnectionTimeout { get { throw new NotImplementedException(); } }
		public string Database { get { throw new NotImplementedException(); } }
		public ConnectionState State { get { return ConnectionState.Open; } }
	}
}
