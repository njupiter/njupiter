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

using nJupiter.Abstraction.Logging.NullLogger;

namespace nJupiter.Abstraction.Logging.Tests.Unit {
	
	[TestFixture]
	public class LogManagerFactoryTests {

		[SetUp]
		public void SetUp() {
			// Reset manager before every test
			LogManagerFactory.RegisterFactory(null);
		}

		[TearDown]
		public void TearDown() {
			// Reset manager after every test
			LogManagerFactory.RegisterFactory(null);
		}

		[Test]
		public void GetLogManager_GetDefaultInstance_ReturnsNullLogManager() {
			var instance  = LogManagerFactory.GetLogManager();
			Assert.IsInstanceOf<NullLogManager>(instance);
		}


		[Test]
		public void GetLogManager_RegisterFactoryMethod_ReturnsInstanceFromFactory() {
			var manager = A.Fake<ILogManager>();
			LogManagerFactory.RegisterFactory(() => manager);

			var result  = LogManagerFactory.GetLogManager();

			Assert.AreSame(manager, result);
		}

		[Test]
		public void RegisterFactory_RegisterFactoryMethod_ReturnsTrue() {
			LogManagerFactory.RegisterFactory(A.Fake<ILogManager>);
			Assert.IsTrue(LogManagerFactory.FactoryRegistered);
		}

		[Test]
		public void RegisterFactory_RegisterNullFactoryMethod_ReturnsFalse() {
			LogManagerFactory.RegisterFactory(null);
			Assert.IsFalse(LogManagerFactory.FactoryRegistered);
		}


		 
	}
}