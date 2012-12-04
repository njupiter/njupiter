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
using System.Collections;
using System.Linq;

using FakeItEasy;

using NUnit.Framework;

namespace nJupiter.DataAccess.Ldap.Tests.Unit {

	[TestFixture]
	public class AttributeCollectionTests {

		[Test]
		public void Constructor_SendInDictionaryWithNonStringKey_ThrowsArgumentException() {
			var attributeValueCollection = A.Fake<IAttributeValueCollection>();
			var dictionary = new Hashtable();
			dictionary.Add(new object(), attributeValueCollection);

			Assert.Throws<ArgumentException>(() => new AttributeCollection(dictionary));
		}

		[Test]
		public void Constructor_SendInDictionaryWithNonIEnumerableValues_ThrowsArgumentException() {
			var dictionary = new Hashtable();
			dictionary.Add("key", new object());
			Assert.Throws<ArgumentException>(() => new AttributeCollection(dictionary));
		}

		[Test]
		public void Index_SendInDictionaryWithStringKeyAndObjectArrayValue_IndexWithKeyContainsValue() {
			var attributeValueCollection = A.CollectionOfFake<object>(1);
			var dictionary = new Hashtable();
			dictionary.Add("key", attributeValueCollection);

			var collection = new AttributeCollection(dictionary);

			Assert.AreEqual(attributeValueCollection, collection["key"]);
		}


		[Test]
		public void Count_SendInDictionaryWithOnePost_ReturnsOne() {
			var attributeValueCollection = A.CollectionOfFake<object>(1);
			var dictionary = new Hashtable();
			dictionary.Add("key", attributeValueCollection);

			var collection = new AttributeCollection(dictionary);

			Assert.AreEqual(1, collection.Count);
		}

		[Test]
		public void ContainsKey_SendInDictionaryWithStringKeyAndObjectArrayValueAndCheckForKey_ReturnsTrue() {
			var attributeValueCollection = A.CollectionOfFake<object>(1);
			var dictionary = new Hashtable();
			dictionary.Add("key", attributeValueCollection);

			var collection = new AttributeCollection(dictionary);

			Assert.IsTrue(collection.ContainsKey("key"));
		}

		[Test]
		public void ContainsKey_SendInDictionaryWithObjectArrayValueWitchIsRecivedThruIndexAndCompared_ReturnsTrue() {
			var attributeValueCollection = A.CollectionOfFake<object>(1);
			var dictionary = new Hashtable();
			dictionary.Add("key", attributeValueCollection);

			var collection = new AttributeCollection(dictionary);
			var result = collection["key"];

			Assert.IsTrue(collection.ContainsValue(result));
		}

		[Test]
		public void Keys_SendInDictionaryWithStringKeyAndObjectArrayValueAndCheckForKey_ContainsKey() {
			var attributeValueCollection = A.CollectionOfFake<object>(1);
			var dictionary = new Hashtable();
			dictionary.Add("key", attributeValueCollection);

			var collection = new AttributeCollection(dictionary);

			Assert.IsTrue(collection.Keys.Contains("key"));
		}
		[Test]
		public void Values_SendInDictionaryWithObjectArrayValueWitchIsRecivedThruIndexAndCompared_ContainsValue() {
			var attributeValueCollection = A.CollectionOfFake<object>(1);
			var dictionary = new Hashtable();
			dictionary.Add("key", attributeValueCollection);

			var collection = new AttributeCollection(dictionary);
			var result = collection["key"];

			Assert.IsTrue(collection.Values.Contains(result));
		}

		[Test]
		public void GetEnumerator_AddCollectionWithOneObjects_ObjectIsInCollection() {
			var valueCollection = A.CollectionOfFake<object>(1);
			var dictionary = new Hashtable();
			dictionary.Add("key", valueCollection);
	
			var collection = new AttributeCollection(dictionary);
			foreach(var attributeValueCollection in collection) {
				foreach(var attributeValue in attributeValueCollection) {
					Assert.IsTrue(valueCollection.Contains(attributeValue));	
				}
			}
		}

		[Test]
		public void Equals_CreateCollection_CollectionIsEqualsItself() {
			var attributeValueCollection = A.CollectionOfFake<object>(1);
			var dictionary = new Hashtable();
			dictionary.Add("key", attributeValueCollection);

			var collection = new AttributeCollection(dictionary);
			Assert.IsTrue(collection.Equals(collection));
		}

		[Test]
		public void Equals_CreateTwoIdenticallCollection_CollectionsAreNotEqual() {
			var attributeValueCollection = A.CollectionOfFake<object>(1);
			var dictionary = new Hashtable();
			dictionary.Add("key", attributeValueCollection);
	
			var collection1 = new AttributeCollection(dictionary);
			var collection2 = new AttributeCollection(dictionary);
			Assert.IsFalse(collection1.Equals(collection2));
		}

		[Test]
		public void Equals_CreateCollection_CollectionIsNotEquaNull() {
			var attributeValueCollection = A.CollectionOfFake<object>(1);
			var dictionary = new Hashtable();
			dictionary.Add("key", attributeValueCollection);

			var collection = new AttributeCollection(dictionary);
			Assert.IsFalse(collection.Equals(null));
		}



	}
}