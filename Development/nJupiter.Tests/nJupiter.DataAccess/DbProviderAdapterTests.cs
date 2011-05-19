using System.Data.Common;

using FakeItEasy;

using nJupiter.DataAccess;

using NUnit.Framework;

namespace nJupiter.Tests.nJupiter.DataAccess {
	
	[TestFixture]
	public class DbProviderAdapterTests {

		[Test]
		public void CanCreateDataSourceEnumerator_CreateAdapter_WrappedObjectHasSameValue() {
			var providerFactory = A.Fake<DbProviderFactory>();
			var provider = new DbProviderAdapter(providerFactory);
			Assert.AreEqual(provider.CanCreateDataSourceEnumerator, providerFactory.CanCreateDataSourceEnumerator);
		}

		[Test]
		public void DbProvider_CreateAdapter_RetusnsWrappedObject() {
			var providerFactory = A.Fake<DbProviderFactory>();
			var provider = new DbProviderAdapter(providerFactory);
			Assert.AreEqual(provider.DbProvider, providerFactory);
		}

		[Test]
		public void CreateCommand_CreateAdapter_WrappedObjectHasBeenCalled() {
			var providerFactory = A.Fake<DbProviderFactory>();
			var provider = new DbProviderAdapter(providerFactory);
			provider.CreateCommand();
			A.CallTo(() => providerFactory.CreateCommand()).MustHaveHappened(Repeated.Exactly.Once);
		}

		[Test]
		public void CreateConnection_CreateAdapter_WrappedObjectHasBeenCalled() {
			var providerFactory = A.Fake<DbProviderFactory>();
			var provider = new DbProviderAdapter(providerFactory);
			provider.CreateConnection();
			A.CallTo(() => providerFactory.CreateConnection()).MustHaveHappened(Repeated.Exactly.Once);
		}

		[Test]
		public void CreateDataAdapter_CreateAdapter_WrappedObjectHasBeenCalled() {
			var providerFactory = A.Fake<DbProviderFactory>();
			var provider = new DbProviderAdapter(providerFactory);
			provider.CreateDataAdapter();
			A.CallTo(() => providerFactory.CreateDataAdapter()).MustHaveHappened(Repeated.Exactly.Once);
		}

		[Test]
		public void CreateParameter_CreateAdapter_WrappedObjectHasBeenCalled() {
			var providerFactory = A.Fake<DbProviderFactory>();
			var provider = new DbProviderAdapter(providerFactory);
			provider.CreateParameter();
			A.CallTo(() => providerFactory.CreateParameter()).MustHaveHappened(Repeated.Exactly.Once);
		}

	}
}
