using System;
using System.Configuration;
using System.Data.Common;

namespace nJupiter.DataAccess {
	public static class DataSourceFactory {
		public static IDataSource Create() {
			var connectionStringSettings = ConfigurationManager.ConnectionStrings[0];
			if(connectionStringSettings == null) {
				throw new ArgumentException("The default connection name was not found in the applications configuration or the connection string is empty");
			}
			DbProviderFactory dbProviderFactory = DbProviderFactories.GetFactory(connectionStringSettings.ProviderName);
			return Create(dbProviderFactory, connectionStringSettings.ConnectionString);
		}

		/// <summary>
		/// Gets the instance with a given name. You can find the name of a configured data source in the value attribute for the dataSource element in nJupiter.DataAccess.config
		/// </summary>
		/// <param name="name">The name of the data source.</param>
		/// <returns>A <see cref="DataSource" /> instance.</returns>
		public static IDataSource Create(string name) {
			var connectionStringSettings = ConfigurationManager.ConnectionStrings[name];
			if(connectionStringSettings == null) {
				throw new ArgumentException(string.Format("The connection name '{0}' was not found in the applications configuration or the connection string is empty", name), "name");
			}
			DbProviderFactory dbProviderFactory = DbProviderFactories.GetFactory(connectionStringSettings.ProviderName);
			return Create(dbProviderFactory, connectionStringSettings.ConnectionString);
		}
		
		public static IDataSource Create(DbProviderFactory dbProviderFactory, string connectionString) {
			var wrapper = new DbProviderAdapter(dbProviderFactory, connectionString);
			return new DataSource(wrapper);
		}

		

	}
}
