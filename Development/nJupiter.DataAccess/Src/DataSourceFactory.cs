using System.Data.Common;

namespace nJupiter.DataAccess {
	public static class DataSourceFactory {
		/// <summary>
		/// Gets the instance with a given name. You can find the name of a configured data source in the value attribute for the dataSource element in nJupiter.DataAccess.config
		/// </summary>
		/// <param name="name">The name of the data source.</param>
		/// <returns>A <see cref="DataSource" /> instance.</returns>
		public static IDataSource Create(string name) {
			DbProviderFactory dbProviderFactory = DbProviderFactories.GetFactory(name);
			return Create(dbProviderFactory);
		}
		
		public static IDataSource Create(DbProviderFactory dbProviderFactory) {
			var wrapper = new DbProviderAdapter(dbProviderFactory);
			return new DataSource(wrapper);
		}

		

	}
}
