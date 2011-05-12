using FakeItEasy;

using nJupiter.Configuration;

using NUnit.Framework;

namespace nJupiter.UnitTests.Configuration {

	[TestFixture]
	public class CompositeConfigLoaderTests {

		[Test]
		public void Load_AddToLoadersWhereConfigKeyExistsInBoth_ReturnResultFromFirstLoader() {
			const string configKey = "myConfigKey";
			var fakeLoder1 = A.Fake<IConfigLoader>();
			var fakeConfig1 = A.Fake<IConfig>();
			var fakeLoder2 = A.Fake<IConfigLoader>();
			var fakeConfig2 = A.Fake<IConfig>();

			A.CallTo(() => fakeLoder1.Load(configKey)).Returns(fakeConfig1);
			A.CallTo(() => fakeLoder2.Load(configKey)).Returns(fakeConfig2);

			var compositeLoader = new CompositeConfigLoader();
			compositeLoader.Add(fakeLoder1);
			compositeLoader.Add(fakeLoder2);

			var config = compositeLoader.Load(configKey);
			A.CallTo(() => fakeLoder1.Load(configKey)).MustHaveHappened(Repeated.Exactly.Once);
			A.CallTo(() => fakeLoder2.Load(configKey)).MustNotHaveHappened();
			Assert.AreEqual(fakeConfig1, config);
		}

		[Test]
		public void Load_AddToLoadersAndLoadConfigExistningInNon_ReturnNull() {
			const string configKey = "myConfigKey";
			var fakeLoder1 = A.Fake<IConfigLoader>();
			var fakeLoder2 = A.Fake<IConfigLoader>();
			
			var compositeLoader = new CompositeConfigLoader();
			compositeLoader.Add(fakeLoder1);
			compositeLoader.Add(fakeLoder2);

			A.CallTo(() => fakeLoder1.Load(configKey)).Returns(null);
			A.CallTo(() => fakeLoder2.Load(configKey)).Returns(null);

			var config = compositeLoader.Load(configKey);
			
			A.CallTo(() => fakeLoder1.Load(configKey)).MustHaveHappened(Repeated.Exactly.Once);
			A.CallTo(() => fakeLoder2.Load(configKey)).MustHaveHappened(Repeated.Exactly.Once);
			Assert.IsNull(config);
		}

		[Test]
		public void LoadOnInit_AddToLoaders_InitializeCollctionInBothLoaders() {
			var fakeLoder1 = A.Fake<IConfigLoader>();
			var fakeLoder2 = A.Fake<IConfigLoader>();
			
			var compositeLoader = new CompositeConfigLoader();
			compositeLoader.Add(fakeLoder1);
			compositeLoader.Add(fakeLoder2);

			compositeLoader.LoadOnInit();
			
			A.CallTo(() => fakeLoder1.InitializeCollection(null)).WithAnyArguments().MustHaveHappened(Repeated.Exactly.Once);
			A.CallTo(() => fakeLoder2.InitializeCollection(null)).WithAnyArguments().MustHaveHappened(Repeated.Exactly.Once);
			
		}

	}

}
