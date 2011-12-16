using System.Data;

namespace nJupiter.DataAccess {
	public static class TransactionFactory {
		/// <summary>
		/// Starts a database transaction.
		/// </summary>
		/// <param name="dataSource">The data source for the transaction.</param>
		/// <returns>The <see cref="Transaction" /> object</returns>
		public static ITransaction BeginTransaction(IDataSource dataSource) {
			return GetTransaction(dataSource, true);
		}

		/// <summary>
		/// Starts a database transaction.
		/// </summary>
		/// <param name="dataSource">The data source for the transaction.</param>
		/// <param name="isolationLevel">The isolation level for the transaction.</param>
		/// <returns>The <see cref="Transaction" /> object</returns>
		public static ITransaction BeginTransaction(IDataSource dataSource, IsolationLevel isolationLevel) {
			return GetTransaction(dataSource, isolationLevel, true);
		}

		internal static ITransaction GetTransaction(IDataSource dataSource, bool beginTransaction) {
			return GetTransaction(dataSource, IsolationLevel.ReadCommitted, beginTransaction);
		}

		internal static ITransaction GetTransaction(IDataSource dataSource, IsolationLevel isolationLevel, bool beginTransaction) {
			IDbConnection connection = dataSource.OpenConnection();
			Transaction transaction = new Transaction(connection, isolationLevel);
			if(beginTransaction) {
				transaction.Begin();
			}
			return transaction;
		}

	}
}
