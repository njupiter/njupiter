using System.Data;
using System.Data.Common;

using FakeItEasy;

using nJupiter.DataAccess;

using NUnit.Framework;

namespace nJupiter.UnitTests.DataAccess {
	
	[TestFixture]
	public class DataSourceTests {

		[Test]
		public void CreateCommand_CreateCommand_ChechThatCorrectCommandIsCreated() {
			var dbProviderFactory = A.Fake<IProvider>();
			var dataSource = new DataSource(dbProviderFactory);
			var transaction = A.Fake<IDbTransaction>();
			var parameter = A.Fake<IDataParameter>();

			var command = dataSource.CreateCommand("commandSting", transaction, CommandType.TableDirect, parameter);
			Assert.AreEqual("commandSting", command.DbCommand.CommandText);
			Assert.AreEqual(transaction.Connection, command.DbCommand.Connection);
			Assert.AreEqual(transaction, command.DbCommand.Transaction);
			Assert.AreEqual(CommandType.TableDirect, command.DbCommand.CommandType);
		}

		[Test]
		public void CreateParameter_CreateParameter_ChechThatCorrectParameterIsCreated() {
			var dbProviderFactory = A.Fake<IProvider>();
			var dataSource = new DataSource(dbProviderFactory);

			var parameter = dataSource.CreateParameter("myparameter", DbType.Guid);
			Assert.AreEqual("myparameter", parameter.ParameterName);
			Assert.AreEqual(DbType.Guid, parameter.DbType);
		}
	}
}
