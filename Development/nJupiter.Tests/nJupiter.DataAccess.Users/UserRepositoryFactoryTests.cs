using System.Xml;

using FakeItEasy;

using nJupiter.Configuration;
using nJupiter.DataAccess.Users;

using NUnit.Framework;

namespace nJupiter.Tests.nJupiter.DataAccess.Users {
	
	[TestFixture]
	public class UserRepositoryFactoryTests {

		[Test]
		public void Create_CreateDefaultInstanceFromConfig_ReturnsRepositorySetToDefault() {
			UserRepositoryFactory userRepositoryFactory = GetUserRepositoryFactory();
			var repository = userRepositoryFactory.Create();
			Assert.AreEqual("SQLRepository", repository.Name);
		}

		private static UserRepositoryFactory GetUserRepositoryFactory() {
			var configHandler = A.Fake<IConfigHandler>();
			var config = GetConfig();
			A.CallTo(() => configHandler.GetConfig()).Returns(config);
			return new UserRepositoryFactory(configHandler);
		}

		private static IConfig GetConfig() {
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(string.Format(testConfig));
			return ConfigFactory.Create("testConfig", xmlDocument.DocumentElement);
		}

		private const string testConfig =
			@"<configuration>
				<userRepositories>
					<userRepository name=""SQLRepository"" default=""true"">
						<userRepositoryFactory
							qualifiedTypeName=""nJupiter.DataAccess.Users.Sql.UserRepositoryFactory, nJupiter.DataAccess.Users.Sql""/>
						<settings>
							<dataSource value=""SQLAdapter"" />
							<hashPassword value=""true"" />
							<cache>
								<userCacheFactory
									qualifiedTypeName=""nJupiter.DataAccess.Users.Caching.HttpRuntimeUserCacheFactory, nJupiter.DataAccess.Users""/>
								<minutesInCache value=""60"" />
							</cache>
							<predefinedProperties>
								<firstName value=""firstName"" />
								<lastName value=""lastName"" />
							</predefinedProperties>
						</settings>
					</userRepository>
					<userRepository name=""TestRepository"" default=""true"">
						<userRepositoryFactory
							qualifiedTypeName=""nJupiter.Tests.DataAccess.Users.UserRepositoryAdapter, nJupiter.Tests""/>
						<settings>
							<someSettings value=""test"" />
						</settings>
					</userRepository>
				</userRepositories>
			</configuration>";

	}
}
