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

namespace nJupiter.DataAccess {

	/// <summary>
	/// Abstract class that represent a command that can be executed by a <see cref="DataSource" />.
	/// </summary>
	public abstract class Command : IDisposable {
		#region Members
		private Transaction transaction;
		private bool		disposed;
		#endregion
		
		#region Properties
		/// <summary>
		/// Gets or sets the transaction associated with the command.
		/// </summary>
		/// <value>The transaction.</value>
		public Transaction Transaction {
			get { return this.transaction; }
			set {
				this.transaction = value;
				this.DbCommand.Connection = this.transaction.Connection;
				this.DbCommand.Transaction = this.transaction.DbTransaction;
			}
		}
		/// <summary>
		/// Gets or sets the command text associated with the command.
		/// </summary>
		/// <value>The command text.</value>
		public string CommandText { get { return this.DbCommand.CommandText; } set { this.DbCommand.CommandText = value; } }
		/// <summary>
		/// Gets the <see cref="IDbCommand" /> associated with the command.
		/// </summary>
		/// <value>The <see cref="IDbCommand" /> associated with the command.</value>
		public abstract IDbCommand DbCommand { get; }
		/// <summary>
		/// Gets or sets the timeout for the command.
		/// </summary>
		/// <value>The command timeout.</value>
		public abstract int CommandTimeout { get; set; }
		#endregion

		#region Methods
		/// <summary>
		/// Adds a parameter to the command.
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="type">The type of the parameter.</param>
		/// <param name="size">The size of the parameter.</param>
		/// <param name="direction">The direction of the parameter.</param>
		/// <param name="nullable">if set to <c>true</c> the value can be null.</param>
		/// <param name="precision">The precision of the parameter.</param>
		/// <param name="scale">The scale of the parameter.</param>
		/// <param name="column">The column of the parameter.</param>
		/// <param name="rowVersion">The row version.</param>
		/// <param name="value">The value of the parameter.</param>
		public abstract void AddParameter(string name, DbType type, int size, ParameterDirection direction, bool nullable, byte precision, byte scale, string column, DataRowVersion rowVersion, object value);
		/// <summary>
		/// Adds a parameter to the command.
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="type">The type of the parameter.</param>
		/// <param name="size">The size of the parameter.</param>
		public void AddOutParameter(string name, DbType type, int size){
			this.AddParameter(name, type, size, ParameterDirection.Output, true, 0, 0, string.Empty, DataRowVersion.Default, DBNull.Value);
		}
		/// <summary>
		/// Adds a parameter to the command.
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="type">The type of the parameter.</param>
		public void AddInParameter(string name, DbType type){
			this.AddParameter(name, type, ParameterDirection.Input, string.Empty, DataRowVersion.Default, null);
		}
		/// <summary>
		/// Adds a parameter to the command.
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="type">The type of the parameter.</param>
		/// <param name="value">The value of the parameter.</param>
		public void AddInParameter(string name, DbType type, object value){
			this.AddParameter(name, type, ParameterDirection.Input, string.Empty, DataRowVersion.Default, value);
		}
		/// <summary>
		/// Adds a parameter to the command.
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="type">The type of the parameter.</param>
		/// <param name="column">The column of the parameter.</param>
		/// <param name="rowVersion">The row version.</param>
		public void AddInParameter(string name, DbType type, string column, DataRowVersion rowVersion){
			this.AddParameter(name, type, 0, ParameterDirection.Input, true, 0, 0, column, rowVersion, null);
		}
		/// <summary>
		/// Adds a parameter to the command.
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="type">The type of the parameter.</param>
		/// <param name="direction">The direction of the parameter.</param>
		/// <param name="column">The column of the parameter.</param>
		/// <param name="rowVersion">The row version.</param>
		/// <param name="value">The value of the parameter.</param>
		public void AddParameter(string name, DbType type, ParameterDirection direction, string column, DataRowVersion rowVersion, object value) {
			this.AddParameter(name, type, 0, direction, false, 0, 0, column, rowVersion, value);
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources
		/// </summary>
		/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing) {
			if (!this.disposed) {
				this.disposed = true;
   
				// Suppress finalization of this disposed instance.
				if (disposing) 
					GC.SuppressFinalize(this);
			}
		}
		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
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
		#endregion
	}
}