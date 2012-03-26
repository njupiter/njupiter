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
using System.Data;

using log4net;

namespace nJupiter.DataAccess {

	/// <summary>
	/// Represents a transaction to be performed at a data source.
	/// </summary>
	public sealed class Transaction : IDbTransaction {
		#region Members
		private readonly	IsolationLevel	isolationLevel;
		private readonly	DataSource		dataAccess;
		private				IDbConnection	connection;
		private				IDbTransaction	dbTransaction;
		private bool						disposed;
		#endregion

		#region Static Members
		private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="Transaction"/> class.
		/// </summary>
		/// <param name="dataSource">The data source associated with the transaction.</param>
		/// <param name="isolationLevel">The isolation level of the transaction.</param>
		private Transaction(DataSource dataSource, IsolationLevel isolationLevel) {
			if(log.IsDebugEnabled) { log.Debug("Creating transaction"); }
			this.dataAccess		=	dataSource;
			this.isolationLevel	=	isolationLevel;
		}
		#endregion

		#region Public Properties
		/// <summary>
		/// Specifies the Connection object to associate with the transaction.
		/// </summary>
		/// <value></value>
		/// <returns>
		/// The Connection object to associate with the transaction.
		/// </returns>
		public IDbConnection	Connection		{ get { return this.connection; } }
		/// <summary>
		/// Specifies the <see cref="T:System.Data.IsolationLevel"/> for this transaction.
		/// </summary>
		/// <value></value>
		/// <returns>
		/// The <see cref="T:System.Data.IsolationLevel"/> for this transaction. The default is ReadCommitted.
		/// </returns>
		public IsolationLevel	IsolationLevel	{ get { return this.isolationLevel; } }
		/// <summary>
		/// Gets the <see cref="IDbTransaction" /> associated with the transaction.
		/// </summary>
		/// <value>The <see cref="IDbTransaction" /> associated with the transaction.</value>
		public IDbTransaction	DbTransaction	{ get { return this.dbTransaction; } }
		#endregion

		#region Simple Factory Methods
		/// <summary>
		/// Starts a database transaction.
		/// </summary>
		/// <param name="dataSource">The data source for the transaction.</param>
		/// <returns>The <see cref="Transaction" /> object</returns>
		public static Transaction BeginTransaction(DataSource dataSource) {
			return GetTransaction(dataSource, true);
		}

		/// <summary>
		/// Starts a database transaction.
		/// </summary>
		/// <param name="dataSource">The data source for the transaction.</param>
		/// <param name="isolationLevel">The isolation level for the transaction.</param>
		/// <returns>The <see cref="Transaction" /> object</returns>
		public static Transaction BeginTransaction(DataSource dataSource, IsolationLevel isolationLevel) {
			return GetTransaction(dataSource, isolationLevel, true);
		}

		internal static Transaction GetTransaction(DataSource dataSource, bool beginTransaction) {
			return GetTransaction(dataSource, IsolationLevel.ReadCommitted, beginTransaction);
		}

		internal static Transaction GetTransaction(DataSource dataSource, IsolationLevel isolationLevel, bool beginTransaction) {
			if(log.IsDebugEnabled) { log.Debug("Getting transaction"); }
			Transaction transaction = new Transaction(dataSource, isolationLevel);
			transaction.connection = transaction.dataAccess.OpenConnection();
			if(beginTransaction) {
				if(log.IsDebugEnabled) { log.Debug("Beginning transaction"); }
				transaction.Begin();
			}
			return transaction;
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Start the database transaction.
		/// </summary>
		public void Begin() {
			if(log.IsDebugEnabled) { log.Debug("Beginning transaction"); }
			this.dbTransaction =  this.connection.BeginTransaction(this.isolationLevel);
		}

		/// <summary>
		/// Commits the database transaction.
		/// </summary>
		/// <exception cref="T:System.Exception">
		/// An error occurred while trying to commit the transaction.
		/// </exception>
		/// <exception cref="T:System.InvalidOperationException">
		/// The transaction has already been committed or rolled back.
		/// -or-
		/// The connection is broken.
		/// </exception>
		public void Commit() {
			if(log.IsDebugEnabled) { log.Debug("Commiting transaction"); }
			this.dbTransaction.Commit();
		}

		/// <summary>
		/// Rolls back a transaction from a pending state.
		/// </summary>
		/// <exception cref="T:System.Exception">
		/// An error occurred while trying to commit the transaction.
		/// </exception>
		/// <exception cref="T:System.InvalidOperationException">
		/// The transaction has already been committed or rolled back.
		/// -or-
		/// The connection is broken.
		/// </exception>
		public void Rollback() {
			if(log.IsDebugEnabled) { log.Debug("Rollback transaction"); }
			this.dbTransaction.Rollback();
		}

		/// <summary>
		/// Ends the database transaction.
		/// </summary>
		public void End() {
			this.Dispose();
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose() {
			if(log.IsDebugEnabled) { log.Debug("Ending transaction"); }
			Dispose(true);
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources
		/// </summary>
		/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		private void Dispose(bool disposing) {
			if(!this.disposed) {
				if(this.connection != null && this.connection.State != ConnectionState.Closed)
					this.connection.Close();

				// Suppress finalization of this disposed instance.
				if (disposing) 
					GC.SuppressFinalize(this);

				this.disposed = true;
			}
		}

		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="Transaction"/> is reclaimed by garbage collection.
		/// </summary>
		~Transaction(){
			Dispose(false);
		}
		#endregion
	}

}