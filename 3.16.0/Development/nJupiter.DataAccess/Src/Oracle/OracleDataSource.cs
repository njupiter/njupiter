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

using System.Data;
using System.Data.Common;
using System.Data.OracleClient;

namespace nJupiter.DataAccess.Oracle {

	/// <summary>
	/// Represents a Oracle database data source.
	/// </summary>
	public class OracleDataSource : DataSource {

		#region Constructors
		internal OracleDataSource() { }
		#endregion

		#region Methods
		/// <summary>
		/// Initializes a new instance of the <see cref="DbDataAdapter"/> for the data source.
		/// </summary>
		/// <param name="connection">A <see cref="IDbConnection"/> that represents the connection.</param>
		/// <returns>
		/// A <see cref="DbDataAdapter"/> for the data source
		/// </returns>
		protected override DbDataAdapter GetDataAdapter(IDbConnection connection) {
			return new OracleDataAdapter(string.Empty, (OracleConnection)connection);
		}

		/// <summary>
		/// Gets the <see cref="IDbConnection"/> connection associated with the current data source.
		/// </summary>
		/// <returns>
		/// 	<see cref="IDbConnection"/> connection associated with the current data source
		/// </returns>
		public override IDbConnection GetConnection() {
			return new OracleConnection(this.ConnectionString);
		}

		/// <summary>
		/// Creates a command for the data source.
		/// </summary>
		/// <param name="command">The command string for the command.</param>
		/// <param name="transaction">The transaction associated with the command.</param>
		/// <param name="commandType">Type of command.</param>
		/// <param name="parameters">Parameters associated with the command.</param>
		/// <returns></returns>
		public override Command CreateCommand(string command, Transaction transaction, CommandType commandType, params object[] parameters) {
			OracleCommand commandObj = new OracleCommand(command, commandType, parameters);
			if(transaction != null) {
				commandObj.DbCommand.Connection = transaction.Connection;
				commandObj.Transaction = transaction;
			}
			return commandObj;
		}

		/// <summary>
		/// Creates a data parameter associated with the data source.
		/// </summary>
		/// <param name="name">The name of the paramter.</param>
		/// <param name="value">The value of the paramter.</param>
		/// <returns>An <see cref="IDataParameter"/></returns>
		public override IDataParameter CreateParameter(string name, object value) {
			return new OracleParameter(name, value);
		}

		/// <summary>
		/// Creates a data parameter associated with the data source.
		/// </summary>
		/// <param name="name">The name of the paramter.</param>
		/// <param name="type">The type of the parameter.</param>
		/// <returns>An <see cref="IDataParameter"/></returns>
		public override IDataParameter CreateParameter(string name, DbType type) {
			OracleParameter param = new OracleParameter();
			param.ParameterName = name;
			param.DbType = type;
			if(type == DbType.Guid) {
				param.Size = 16;
				param.OracleType = OracleType.Raw;
			}
			return param;
		}
		#endregion
	}
}
