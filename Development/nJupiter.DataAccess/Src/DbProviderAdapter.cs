using System.Data;
using System.Data.Common;

namespace nJupiter.DataAccess {
	internal class DbProviderAdapter : IProvider {
		
		private readonly DbProviderFactory dbProviderFactory;

		public DbProviderAdapter(DbProviderFactory dbProviderFactory) {
			this.dbProviderFactory = dbProviderFactory; 
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
			return dbProviderFactory.CreateConnection();
		}

		public IDbDataAdapter CreateDataAdapter() {
			return dbProviderFactory.CreateDataAdapter();
		}

		public IDataParameter CreateParameter() {
			return dbProviderFactory.CreateParameter();
		}

	}
}
