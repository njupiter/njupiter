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

using NUnit.Framework;

using nJupiter.Abstraction.Logging.EnterpriseLibrary;

namespace nJupiter.Abstraction.Logging.Tests.Unit.EnterpriseLibrary {
	
	[TestFixture]
	public class LogManagerAdapterTests {

		[Test]
		public void GetLogger_SendInAnyName_ReturnsLogAdapter() {
			var adapter = new LogManagerAdapter();

			var result = adapter.GetLogger("Any name");

			Assert.IsInstanceOf<LogAdapter>(result);
		}

		[Test]
		public void GetLogger_SendInAnyType_ReturnsLogAdapter() {
			var adapter = new LogManagerAdapter();

			var result = adapter.GetLogger(GetType());

			Assert.IsInstanceOf<LogAdapter>(result);
		}

		[Test]
		public void GetLogger_SendInAnyName_ReturnsLogAdapterWithCorrectName() {
			var adapter = new LogManagerAdapter();

			var result = (LogAdapter)adapter.GetLogger("Any name");

			Assert.AreEqual("Any name", result.LoggerName);
		}


		[Test]
		public void GetLogger_SendInAnyType_ReturnsLogAdapterWithCorrectName() {
			var adapter = new LogManagerAdapter();

			var result = (LogAdapter)adapter.GetLogger(GetType());

			Assert.AreEqual(GetType().FullName, result.LoggerName);
		}

	}
}