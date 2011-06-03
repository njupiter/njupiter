using System.Configuration;
using System.Data;
using System.Data.Common;

namespace nJupiter.DataAccess {
	internal class DbProviderAdapter : IProvider {
		
		private readonly DbProviderFactory dbProviderFactory;
		private readonly string connectionString;

		public DbProviderAdapter(DbProviderFactory dbProviderFactory, string connectionString) {
			this.dbProviderFactory = dbProviderFactory;
			this.connectionString = connectionString;
		}

		public DbProviderFactory DbProvider {
			get {
				return dbProviderFactory;
			}
		}

		public bool CanCreateDataSourceEnumerator {
			get {
				return dbProviderFactory.CanCreateDataSourceEnumerator;
			}
		}

		public IDbCommand CreateCommand() {
			return dbProviderFactory.CreateCommand();
		}

		public IDbConnection CreateConnection() {
			var connection = dbProviderFactory.CreateConnection();
			connection.ConnectionString = this.connectionString;
			return connection;
		}

		public IDbDataAdapter CreateDataAdapter() {
			return dbProviderFactory.CreateDataAdapter();
		}

		public IDataParameter CreateParameter() {
			return dbProviderFactory.CreateParameter();
		}

	}
}
