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

using System.Linq;

using FakeItEasy;

using NUnit.Framework;

namespace nJupiter.DataAccess.Ldap.Tests.Unit {

	[TestFixture]
	public class AttributeValueCollectionTests {

		[Test]
		public void Index_AddCollectionWithOneObject_SameObjectFromIndex() {
			var list = A.CollectionOfFake<object>(1);
			var collection = new AttributeValueCollection(list);
			Assert.AreSame(list.First(), collection[0]);
		}

		[Test]
		public void Index_AddCollectionWithTenObjects_LastOjbectSameObjectAsLastIndex() {
			var list = A.CollectionOfFake<object>(10);
			var collection = new AttributeValueCollection(list);
			Assert.AreSame(list.Last(), collection[9]);
		}


		[Test]
		public void Count_AddCollectionWithTenObjects_ReturnsTen() {
			var list = A.CollectionOfFake<object>(10);
			var collection = new AttributeValueCollection(list);
			Assert.AreEqual(10, collection.Count);
		}


		[Test]
		public void GetEnumerator_AddCollectionWithTenObjects_AllObjectsAreInCollection() {
			var list = A.CollectionOfFake<object>(10);
			var collection = new AttributeValueCollection(list);
			foreach(var attribute in collection) {
				Assert.IsTrue(list.Contains(attribute));
			}
		}

		[Test]
		public void Contains_AddCollectionWithTenObjects_AllObjectsAreInCollection() {
			var list = A.CollectionOfFake<object>(10);
			var collection = new AttributeValueCollection(list);
			foreach(var attribute in list) {
				Assert.IsTrue(collection.Contains(attribute));
			}
		}

		[Test]
		public void Equals_CreateCollection_CollectionIsEqualsItself() {
			var list = A.CollectionOfFake<object>(10);
			var collection = new AttributeValueCollection(list);
			Assert.IsTrue(collection.Equals(collection));
		}

		[Test]
		public void Equals_CreateTwoIdenticallCollection_CollectionsAreNotEqual() {
			var list = A.CollectionOfFake<object>(10);
			var collection1 = new AttributeValueCollection(list);
			var collection2 = new AttributeValueCollection(list);
			Assert.IsFalse(collection1.Equals(collection2));
		}

		[Test]
		public void Equals_CreateCollection_CollectionIsNotEquaNull() {
			var list = A.CollectionOfFake<object>(10);
			var collection = new AttributeValueCollection(list);
			Assert.IsFalse(collection.Equals(null));
		}
		 
	}
}