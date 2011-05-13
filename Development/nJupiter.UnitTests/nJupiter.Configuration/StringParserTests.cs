using System;
using System.Globalization;

using nJupiter.Configuration;

using NUnit.Framework;

namespace nJupiter.UnitTests.Configuration {
	
	[TestFixture]
	public class StringParserTests {
		// This class is indirectly tested mainly in ConfigTests.cs
		// Just have some complementary tests here

		[Test]
		public void Parse_DateTimeString_ReturnsCorrectDateTime() {
			DateTime date = StringParser.Instance.Parse<DateTime>("2007-05-14T10:06:44.397+08:00", CultureInfo.InvariantCulture);
			Assert.AreEqual("2007-05-14 02:06:44Z", date.ToUniversalTime().ToString("u"));
		}

		[Test]
		public void Parse_UnknownType_ThrowsInvalidConfigTypeException() {
			Assert.Throws<InvalidConfigTypeException>(() => StringParser.Instance.Parse<MyDummyClass>("DummyValue", CultureInfo.InvariantCulture));
		}

		public class MyDummyClass{}

	}
}
