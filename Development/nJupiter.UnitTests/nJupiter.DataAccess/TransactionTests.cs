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


	}
}
