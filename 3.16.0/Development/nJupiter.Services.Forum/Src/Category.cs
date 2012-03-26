#region Copyright & License
/*
	Copyright (c) 2005-2010 nJupiter

	Permission is hereby granted, free of charge, to any person obtaining a copy
	of this software and associated documentation files (the "Software"), to deal
	in the Software without restriction, including without limitation the rights
	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	copies of the Software, and to permit persons to whom the Software is
	furnished to do so, subject to the following conditions:

	The above copyright notice and this permission notice shall be included in
	all copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
	THE SOFTWARE.
*/
#endregion

using System;

namespace nJupiter.Services.Forum {

	[Serializable]
	public sealed class Category {
		#region Constants
		public enum Property {
			Id,
			Domain,
			Name,
			Visible,
			RootPostCount,
			ConcurrencyIdentity
		}

		internal const Category.Property DefaultSortProperty = Category.Property.Name;
		internal const bool DefaultSortAscending = true;
		#endregion

		#region Variables
		private readonly CategoryId id;
		private readonly string domain;
		private string name;
		private bool visible = true;
		private readonly PostCollection posts;
		private readonly AttributeCollection attributes;
		private readonly int rootPostCount;
		private readonly string concurrencyIdentity;
		#endregion

		#region Constructors
		internal Category(CategoryId id, string domain, string name, AttributeCollection attributes, PostCollection posts, int rootPostCount, string concurrencyIdentity) {
			if(id == null) {
				throw new ArgumentNullException("id");
			}
			if(rootPostCount < 0) {
				throw new ArgumentOutOfRangeException("rootPostCount");
			}
			this.id = id;
			this.domain = domain;
			this.Name = name;
			this.attributes = attributes;
			this.posts = posts;
			this.rootPostCount = rootPostCount;
			this.concurrencyIdentity = concurrencyIdentity;
		}
		internal Category(string name, string domain, AttributeCollection attributes) : this(new CategoryId(), domain, name, attributes, null, 0, null) { }
		#endregion

		#region Properties
		public CategoryId Id { get { return this.id; } }
		public string Domain { get { return this.domain; } }
		public string Name {
			get { return this.name; }
			set {
				if(value == null) {
					throw new ArgumentNullException("value");
				}
				if(value.Length > 100) {
					throw new ArgumentOutOfRangeException("value");
				}
				this.name = value;
			}
		}
		public bool Visible { get { return this.visible; } set { this.visible = value; } }
		public PostCollection Posts { get { return this.posts; } }
		public AttributeCollection Attributes { get { return this.attributes; } }
		public int RootPostCount { get { return this.rootPostCount; } }
		public string ConcurrencyIdentity { get { return this.concurrencyIdentity; } }
		#endregion

		#region Methods
		public override bool Equals(object obj) {
			Category category = obj as Category;
			return category != null &&
				this.id.Equals(category.id);
		}
		public override int GetHashCode() {
			return this.id.GetHashCode();
		}
		#endregion
	}

}