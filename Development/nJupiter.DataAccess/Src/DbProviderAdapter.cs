using System.Data;
using System.Data.Common;
using System.Security;
using System.Security.Permissions;

namespace nJupiter.DataAccess {
	public class DbProviderAdapter : IProvider {
		
		private readonly DbProviderFactory dbProviderFactory;

		public DbProviderAdapter(DbProviderFactory dbProviderFactory) {
			this.dbProviderFactory = dbProviderFactory; 
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

		public DbCommandBuilder CreateCommandBuilder() {
			return dbProviderFactory.CreateCommandBuilder();
		}

		public DbConnectionStringBuilder CreateConnectionStringBuilder() {
			return dbProviderFactory.CreateConnectionStringBuilder();
		}

		public DbDataSourceEnumerator CreateDataSourceEnumerator() {
			return dbProviderFactory.CreateDataSourceEnumerator();
		}

		public CodeAccessPermission CreatePermission(PermissionState state) {
			return dbProviderFactory.CreatePermission(state);
		}

	}
}
