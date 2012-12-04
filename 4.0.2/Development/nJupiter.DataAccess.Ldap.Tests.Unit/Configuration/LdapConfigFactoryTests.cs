#region Copyright & License
// 
// 	Copyright (c) 2005-2012 nJupiter
// 
// 	Permission is hereby granted, free of charge, to any person obtaining a copy
// 	of this software and associated documentation files (the "Software"), to deal
// 	in the Software without restriction, including without limitation the rights
// 	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// 	copies of the Software, and to permit persons to whom the Software is
// 	furnished to do so, subject to the following conditions:
// 
// 	The above copyright notice and this permission notice shall be included in
// 	all copies or substantial portions of the Software.
// 
// 	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// 	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// 	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// 	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// 	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// 	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// 	THE SOFTWARE.
// 
#endregion

using FakeItEasy;

using NUnit.Framework;

using nJupiter.Configuration;
using nJupiter.DataAccess.Ldap.Configuration;

namespace nJupiter.DataAccess.Ldap.Tests.Unit.Configuration {
	
	[TestFixture]
	public class LdapConfigFactoryTests {
		
		[Test]
		public void GetConfig_GetDefaultConfig_IsNotNull() {
			var factory = GetLdapConfigFactory();

			var config = factory.GetConfig();

			Assert.IsNotNull(config);
		}

		[Test]
		public void GetConfig_GetDefaultConfigTwice_InstancesAreSame() {
			var factory = GetLdapConfigFactory();

			var config1 = factory.GetConfig();
			var config2 = factory.GetConfig();

			Assert.AreSame(config1, config2);
		}


		[Test]
		public void GetConfig_GetSameConfigTwice_InstancesAreSame() {
			var factory = GetLdapConfigFactory();

			var config1 = factory.GetConfig("ladpServer1");
			var config2 = factory.GetConfig("ladpServer1");

			Assert.AreSame(config1, config2);
		}

		[Test]
		public void GetConfig_GetTwoDifferentConfigs_InstancesAreNotSame() {
			var factory = GetLdapConfigFactory();

			var config1 = factory.GetConfig("ladpServer1");
			var config2 = factory.GetConfig("ladpServer2");

			Assert.AreNotSame(config1, config2);
		}



		private static LdapConfigFactory GetLdapConfigFactory() {
			var configRepository = A.Fake<IConfigRepository>();
			var serverConfigFactory = A.Fake<IServerConfigFactory>();
			var usersConfigFactory = A.Fake<IUsersConfigFactory>();
			var groupsConfigFactory = A.Fake<IGroupConfigFactory>();
			var factory = new LdapConfigFactory(configRepository, serverConfigFactory, usersConfigFactory, groupsConfigFactory);
			return factory;
		}
	}
}