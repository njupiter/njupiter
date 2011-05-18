using System.Data;
using System.Data.Common;

namespace nJupiter.DataAccess {
	public interface IProvider {
		bool CanCreateDataSourceEnumerator { get; }
		DbProviderFactory DbProvider { get; }
		IDbCommand CreateCommand();
		IDbConnection CreateConnection();
		IDbDataAdapter CreateDataAdapter();
		IDataParameter CreateParameter();
	}
}