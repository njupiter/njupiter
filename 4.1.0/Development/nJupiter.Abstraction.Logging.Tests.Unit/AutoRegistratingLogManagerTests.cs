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

namespace nJupiter.Abstraction.Logging.Tests.Unit {
	
	[TestFixture]
	public class AutoRegistratingLogManagerTests {

		[TearDown]
		public void TearDown() {
			// Reset manager after every test
			LogManagerFactory.RegisterFactory(null);
		}

		[Test]
		public void Constructor_CreateInstanceWhenNoFacotryIsRegistered_LogManagerFactoryReturnsInstance() {
			LogManagerFactory.RegisterFactory(null);
			var manager = A.Fake<AutoRegistratingLogManager>();
			Assert.AreSame(manager, LogManagerFactory.GetLogManager());
		}
		 
	}

}