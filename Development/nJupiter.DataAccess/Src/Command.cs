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

	internal class Command : IDisposable, ICommand {

		private readonly IDbCommand dbCommand;
		private IDbTransaction transaction;
		private bool disposed;

		internal Command(IDbCommand dbCommand, IDbTransaction transaction, params IDataParameter[] parameters) {
			if(dbCommand == null) {
				throw new ArgumentNullException("dbCommand");
			}
			this.dbCommand = dbCommand;
			if(parameters != null){
				foreach(IDataParameter parameter in parameters){
					this.dbCommand.Parameters.Add(parameter);
				}
			}
			this.Transaction = transaction;

		}

		public IDbTransaction Transaction {
			get { return this.transaction; }
			set {
				this.transaction = value;
				this.DbCommand.Connection = this.transaction != null ? this.transaction.Connection : null;
				this.DbCommand.Transaction = this.transaction;
			}
		}
		
		public string CommandText { get { return this.DbCommand.CommandText; } set { this.DbCommand.CommandText = value; } }
		
		public IDbCommand DbCommand { get { return dbCommand; } }
		
		public int CommandTimeout {
			get { return dbCommand.CommandTimeout; }
			set { dbCommand.CommandTimeout = value; }
		}

		public void AddParameter(string name, DbType type, int size, ParameterDirection direction, byte precision, byte scale, string column, DataRowVersion rowVersion, object value) {
			var param = this.DbCommand.CreateParameter();

			param.ParameterName = name;
			param.DbType = type;
			param.Size = size;
			param.Scale = scale;
			param.Precision = precision;
			param.Direction = direction;
			param.SourceColumn = column;
			param.SourceVersion = rowVersion;
			param.Value = value;

			this.DbCommand.Parameters.Add(param);
		}

		public void AddOutParameter(string name, DbType type, int size) {
			this.AddParameter(name, type, size, ParameterDirection.Output, 0, 0, string.Empty, DataRowVersion.Default, DBNull.Value);
		}

		public void AddInParameter(string name, DbType type) {
			this.AddParameter(name, type, ParameterDirection.Input, string.Empty, DataRowVersion.Default, null);
		}

		public void AddInParameter(string name, DbType type, object value) {
			this.AddParameter(name, type, ParameterDirection.Input, string.Empty, DataRowVersion.Default, value);
		}

		public void AddInParameter(string name, DbType type, string column, DataRowVersion rowVersion) {
			this.AddParameter(name, type, 0, ParameterDirection.Input, 0, 0, column, rowVersion, null);
		}

		public void AddParameter(string name, DbType type, ParameterDirection direction, string column, DataRowVersion rowVersion, object value) {
			this.AddParameter(name, type, 0, direction, 0, 0, column, rowVersion, value);
		}

		private void Dispose(bool disposing) {
			if(!this.disposed) {
				this.disposed = true;
				dbCommand.Dispose();
				// Suppress finalization of this disposed instance.
				if(disposing)
					GC.SuppressFinalize(this);
			}
		}

		public void Dispose() {
			Dispose(true);
		}

		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="Command"/> is reclaimed by garbage collection.
		/// </summary>
		~Command() {
			Dispose(false);
		}
	}
}