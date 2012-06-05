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

using System;

using FakeItEasy;

using NUnit.Framework;

using nJupiter.Abstraction.Logging.NullLogger;
using nJupiter.Configuration;

namespace nJupiter.Abstraction.Logging.Tests.Unit {
	
	[TestFixture]
	public class ConfigurableLogManagerFactoryTests {
		[Test]
		public void GetInstance_NoConfigFound_ReturnsNullLogManager() {
			var configRepository = A.Fake<IConfigRepository>();
			A.CallTo(() => configRepository.GetConfig(true)).Returns(null);
			var facotry = new ConfigurableLogManagerFactory(configRepository);

			var result = facotry.GetInstance();

			Assert.IsInstanceOf<NullLogManager>(result);
		}

		[Test]
		public void GetInstance_ConfigForFakeLogManagerExists_ReturnsFakeLogManagerInstance() {
			var configRepository = A.Fake<IConfigRepository>();
			var config = A.Fake<IConfig>();
			A.CallTo(() => configRepository.GetConfig(true)).Returns(config);
			A.CallTo(() => config.GetAttribute("logManager", "qualifiedTypeName")).Returns(typeof(FakeLogManager).AssemblyQualifiedName);
			var facotry = new ConfigurableLogManagerFactory(configRepository);

			var result = facotry.GetInstance();

			Assert.IsInstanceOf<FakeLogManager>(result);
			
		}

		[Test]
		public void GetInstance_ConfigForFakeLogManagerNotImplementingILogManagerExists_ThrowsTypeLoadException() {
			var configRepository = A.Fake<IConfigRepository>();
			var config = A.Fake<IConfig>();
			A.CallTo(() => configRepository.GetConfig(true)).Returns(config);
			A.CallTo(() => config.GetAttribute("logManager", "qualifiedTypeName")).Returns(typeof(FakeLogManagerNotImplementingILogManager).AssemblyQualifiedName);
			var facotry = new ConfigurableLogManagerFactory(configRepository);

			Assert.Throws<TypeLoadException>(() => facotry.GetInstance());
		}

		[Test]
		public void GetInstance_ConfigForNonExistingLogManagerExists_ThrowsTypeLoadException() {
			var configRepository = A.Fake<IConfigRepository>();
			var config = A.Fake<IConfig>();
			A.CallTo(() => configRepository.GetConfig(true)).Returns(config);
			A.CallTo(() => config.GetAttribute("logManager", "qualifiedTypeName")).Returns("NotExisingLogManager");
			var facotry = new ConfigurableLogManagerFactory(configRepository);

			Assert.Throws<TypeLoadException>(() => facotry.GetInstance());
		}


		[Test]
		public void GetInstance_ConfigForFakeLogManagerExistsButIsDiscarded_ReturnsNullLogManagerAfterConfigDiscarded() {
			var configRepository = A.Fake<IConfigRepository>();
			var config = A.Fake<IConfig>();
			A.CallTo(() => configRepository.GetConfig(true)).Returns(config);
			A.CallTo(() => config.GetAttribute("logManager", "qualifiedTypeName")).Returns(typeof(FakeLogManager).AssemblyQualifiedName);
			var facotry = new ConfigurableLogManagerFactory(configRepository);

			var result = facotry.GetInstance();

			Assert.IsInstanceOf<FakeLogManager>(result);

			config.Discarded += Raise.With(config, EventArgs.Empty).Now;

			A.CallTo(() => configRepository.GetConfig(true)).Returns(null);

			result = facotry.GetInstance();

			Assert.IsInstanceOf<NullLogManager>(result);

			
		}
	}

	public class FakeLogManager : ILogManager {
		public ILog GetLogger(Type type) {
			throw new NotImplementedException();
		}

		public ILog GetLogger(string name) {
			throw new NotImplementedException();
		}
	}

	public class FakeLogManagerNotImplementingILogManager {}
}