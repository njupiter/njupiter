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

namespace nJupiter.DataAccess {

	internal sealed class Transaction : IDbTransaction {
		#region Members
		private readonly IsolationLevel isolationLevel;
		private readonly IDbConnection connection;
		private IDbTransaction dbTransaction;
		private bool disposed;
		#endregion

		internal Transaction(IDbConnection connection, IsolationLevel isolationLevel) {
			this.connection = connection;
			this.isolationLevel = isolationLevel;
		}

		public IDbConnection Connection { get { return this.connection; } }

		public IsolationLevel IsolationLevel { get { return this.isolationLevel; } }

		public void Begin() {
			this.dbTransaction = this.connection.BeginTransaction(this.isolationLevel);
		}

		public void Commit() {
			this.dbTransaction.Commit();
		}

		public void Rollback() {
			this.dbTransaction.Rollback();
		}

		public void End() {
			this.Dispose();
		}

		public void Dispose() {
			Dispose(true);
		}

		private void Dispose(bool disposing) {
			if(!this.disposed) {
				if(this.connection != null && this.connection.State != ConnectionState.Closed) {
					this.connection.Close();
				}

				// Suppress finalization of this disposed instance.
				if(disposing)
					GC.SuppressFinalize(this);

				this.disposed = true;
			}
		}

		~Transaction() {
			Dispose(false);
		}
	}

}