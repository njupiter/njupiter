using System.Data;
using System.Data.Common;

namespace nJupiter.DataAccess {
	public interface IProvider {
		bool CanCreateDataSourceEnumerator { get; }
		string ConnectionString { get; }
		DbProviderFactory DbProvider { get; }
		IDbCommand CreateCommand();
		IDbConnection CreateConnection();
		IDbDataAdapter CreateDataAdapter();
		IDataParameter CreateParameter();
	}
}