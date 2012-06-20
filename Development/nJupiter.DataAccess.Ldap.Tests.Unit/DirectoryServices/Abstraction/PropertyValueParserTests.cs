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

using NUnit.Framework;

using nJupiter.DataAccess.Ldap.DirectoryServices;

namespace nJupiter.DataAccess.Ldap.Tests.Unit.DirectoryServices.Abstraction {
	[TestFixture]
	public class PropertyValueParserTests {

		[Test]
		public void ParseString_SendInString_ReturnsCorrectString() {
			var result = PropertyValueParser.Parse<string>("hello world");
			Assert.AreEqual("hello world", result);
		}

		[Test]
		public void ParseString_SendInStringAsByteArray_ReturnsCorrectString() {
			var byteArray = System.Text.Encoding.UTF8.GetBytes("hello world");
			var result = PropertyValueParser.Parse<string>(byteArray);
			Assert.AreEqual("hello world", result);
		}

		[Test]
		public void ParseDateTime_SendInDateAsFileTime_ReturnsCorrectDateTime() {
			var date = new DateTime(2000, 1, 1);
			var result = PropertyValueParser.Parse<DateTime>(date.ToFileTime());
			Assert.AreEqual(date, result);

		}

		[Test]
		public void ParseDateTime_SendInDateAsFileTimeString_ReturnsCorrectDateTime() {
			var date = new DateTime(2000, 1, 1);
			var result = PropertyValueParser.Parse<DateTime>(date.ToFileTime().ToString());
			Assert.AreEqual(date, result);

		}
		 
	}
}