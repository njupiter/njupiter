using System.Data;

namespace nJupiter.DataAccess {
	public interface ICommand {

		/// <summary>
		/// Gets or set the transaction associated with the command.
		/// </summary>
		/// <value>The transaction.</value>
		IDbTransaction Transaction { get; set; }
		/// <summary>
		/// Gets or sets the command text associated with the command.
		/// </summary>
		/// <value>The command text.</value>
		string CommandText { get; set; }
		/// <summary>
		/// Gets the <see cref="IDbCommand" /> associated with the command.
		/// </summary>
		/// <value>The <see cref="IDbCommand" /> associated with the command.</value>
		IDbCommand DbCommand { get; }
		/// <summary>
		/// Gets or sets the timeout for the command.
		/// </summary>
		/// <value>The command timeout.</value>
		int CommandTimeout { get; set; }

		/// <summary>
		/// Adds a parameter to the command.
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="type">The type of the parameter.</param>
		/// <param name="size">The size of the parameter.</param>
		/// <param name="direction">The direction of the parameter.</param>
		/// <param name="precision">The precision of the parameter.</param>
		/// <param name="scale">The scale of the parameter.</param>
		/// <param name="column">The column of the parameter.</param>
		/// <param name="rowVersion">The row version.</param>
		/// <param name="value">The value of the parameter.</param>
		void AddParameter(string name, DbType type, int size, ParameterDirection direction, byte precision, byte scale, string column, DataRowVersion rowVersion, object value);

		/// <summary>
		/// Adds a parameter to the command.
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="type">The type of the parameter.</param>
		/// <param name="size">The size of the parameter.</param>
		void AddOutParameter(string name, DbType type, int size);

		/// <summary>
		/// Adds a parameter to the command.
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="type">The type of the parameter.</param>
		void AddInParameter(string name, DbType type);

		/// <summary>
		/// Adds a parameter to the command.
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="type">The type of the parameter.</param>
		/// <param name="value">The value of the parameter.</param>
		void AddInParameter(string name, DbType type, object value);

		/// <summary>
		/// Adds a parameter to the command.
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="type">The type of the parameter.</param>
		/// <param name="column">The column of the parameter.</param>
		/// <param name="rowVersion">The row version.</param>
		void AddInParameter(string name, DbType type, string column, DataRowVersion rowVersion);

		/// <summary>
		/// Adds a parameter to the command.
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="type">The type of the parameter.</param>
		/// <param name="direction">The direction of the parameter.</param>
		/// <param name="column">The column of the parameter.</param>
		/// <param name="rowVersion">The row version.</param>
		/// <param name="value">The value of the parameter.</param>
		void AddParameter(string name, DbType type, ParameterDirection direction, string column, DataRowVersion rowVersion, object value);
	}
}