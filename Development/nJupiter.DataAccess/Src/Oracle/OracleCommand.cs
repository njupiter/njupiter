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
using OracleClient = System.Data.OracleClient;

namespace nJupiter.DataAccess.Oracle {

	/// <summary>
	/// Represents a Transact-SQL statement or stored procedure to execute against a Oracle database.
	/// </summary>
	public sealed class OracleCommand : Command {

		#region Members
		private readonly OracleClient.OracleCommand command;
		private bool disposed;
		#endregion

		#region Constructors
		internal OracleCommand(string command, CommandType commandType) {
			this.command = new OracleClient.OracleCommand { CommandText = command, CommandType = commandType };
		}

		internal OracleCommand(string command, CommandType commandType, object[] parameters)
			: this(command, commandType) {
			if(parameters != null) {
				this.command.Parameters.AddRange(parameters);
			}
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets the <see cref="IDbCommand"/> associated with the command.
		/// </summary>
		/// <value>The <see cref="IDbCommand"/> associated with the command.</value>
		public override IDbCommand DbCommand { get { return this.command; } }
		/// <summary>
		/// Gets or sets the timeout for the command.
		/// </summary>
		/// <value>The command timeout.</value>
		public override int CommandTimeout { get { return 0; } set { } }
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
		public override void AddParameter(string name, DbType type, int size, ParameterDirection direction, bool nullable, byte precision, byte scale, string column, DataRowVersion rowVersion, object value) {
			this.command.Parameters.Add(CreateParameter(name, type, size, direction, nullable, precision, scale, column, rowVersion, value));
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources
		/// </summary>
		/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		protected override void Dispose(bool disposing) {
			try {
				if(!this.disposed) {
					// Dispose managed resources.
					this.command.Dispose();
					this.disposed = true;
				}
			} finally {
				base.Dispose(disposing);
			}
		}
		#endregion

		#region Private Methods
		private OracleClient.OracleParameter CreateParameter(string name, DbType type, int size, ParameterDirection direction, bool nullable, byte precision, byte scale, string column, DataRowVersion rowVersion, object value) {
			OracleClient.OracleParameter param = this.command.CreateParameter();
			param.ParameterName = name;
			param.DbType = type;

			if(type == DbType.Guid) {
				param.Size = 16;
				param.OracleType = OracleClient.OracleType.Raw;
				param.Value = value != null ? (object)((Guid)value).ToByteArray() : DBNull.Value;
			} else {
				param.Size = size;
				param.Value = value ?? DBNull.Value;
			}

			param.Direction = direction;
			param.IsNullable = nullable;
#pragma warning disable 618,612
			param.Precision = precision;
			param.Scale = scale;
#pragma warning restore 618,612
			param.SourceColumn = column;
			param.SourceVersion = rowVersion;

			return param;
		}
		#endregion

	}
}
