#region Copyright & License
/*
	Copyright (c) 2005-2011 nJupiter

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
	public sealed class Post {
		#region Constants
		public enum Property {
			Id = 0,
			ParentId = 1,
			CategoryId = 2,
			UserIdentity = 3,
			Author = 4,
			Title = 5,
			Body = 6,
			TimePosted = 7,
			Visible = 8,
			PostCount = 9,
			TimeLastPost = 10,
			SearchRelevance = 11,
			EffectivelyVisible = 12,
			EffectiveCategoryId = 13,
			EffectiveCategoryName = 14,
			ConcurrencyIdentity = 15,
			RootPostId = 16,
			RootPostTitle = 17
		}
		public enum DateProperty {
			TimePosted,
			TimeLastPost
		}

		internal const Post.Property DefaultSortProperty = Post.Property.TimePosted;
		internal const bool DefaultSortAscending = true;
		#endregion

		#region Variables
		private readonly PostId id;
		private readonly PostId parentId;
		private readonly CategoryId categoryId;
		private readonly PostId rootPostId;
		private readonly string rootPostTitle;
		private readonly CategoryId effectiveCategoryId;
		private readonly string effectiveCategoryName;
		private string title;
		private string body;
		private string userIdentity;
		private string author;
		private readonly DateTime timePosted;
		private bool visible = true;
		private readonly bool effectivelyVisible;
		private readonly int postCount;
		private readonly DateTime timeLastPost;
		private readonly float searchRelevance;
		private readonly AttributeCollection attributes;
		private readonly PostCollection posts;
		private readonly string concurrencyIdentity;
		#endregion

		#region Constructors
		internal Post(PostId id, PostId parentId, CategoryId categoryId, PostId rootPostId, string rootPostTitle, CategoryId effectiveCategoryId, string effectiveCategoryName, AttributeCollection attributes, PostCollection posts, DateTime timePosted, DateTime timeLastPost, int postCount, float searchRelevance, bool effectivelyVisible, string concurrencyIdentity) {
			if(id == null) {
				throw new ArgumentNullException("id");
			}
			if(postCount < 0) {
				throw new ArgumentOutOfRangeException("postCount");
			}
			if(searchRelevance > 1 || searchRelevance < 0) {
				throw new ArgumentOutOfRangeException("searchRelevance");
			}
			this.id = id;
			this.parentId = parentId;
			this.categoryId = categoryId;
			this.rootPostId = rootPostId;
			this.rootPostTitle = rootPostTitle;
			this.effectiveCategoryId = effectiveCategoryId;
			this.effectiveCategoryName = effectiveCategoryName;
			this.attributes = attributes;
			this.posts = posts;
			this.timePosted = timePosted;
			this.timeLastPost = timeLastPost;
			this.postCount = postCount;
			this.searchRelevance = searchRelevance;
			this.effectivelyVisible = effectivelyVisible;
			this.concurrencyIdentity = concurrencyIdentity;
		}
		internal Post(PostId parentId, AttributeCollection attributes) : this(new PostId(), parentId, null, null, null, null, null, attributes, null, DateTime.MinValue, DateTime.MinValue, 0, 0, false, null) { }
		internal Post(CategoryId categoryId, AttributeCollection attributes) : this(new PostId(), null, categoryId, null, null, null, null, attributes, null, DateTime.MinValue, DateTime.MinValue, 0, 0, false, null) { }
		#endregion

		#region Properties
		public PostId Id { get { return this.id; } }
		public PostId ParentId { get { return this.parentId; } }
		public CategoryId CategoryId { get { return this.categoryId; } }
		public PostId RootPostId { get { return this.rootPostId; } }
		public string RootPostTitle { get { return this.rootPostTitle; } }
		public CategoryId EffectiveCategoryId { get { return this.effectiveCategoryId; } }
		public string EffectiveCategoryName { get { return this.effectiveCategoryName; } }
		public string Title {
			get { return this.title; }
			set {
				ValidateStringLength(value);
				this.title = value;
			}
		}
		public string Body { get { return this.body; } set { this.body = value; } }
		public string UserIdentity {
			get { return this.userIdentity; }
			set {
				ValidateStringLength(value);
				this.userIdentity = value;
			}
		}
		public string Author {
			get { return this.author; }
			set {
				ValidateStringLength(value);
				this.author = value;
			}
		}
		public bool Visible { get { return this.visible; } set { this.visible = value; } }
		public DateTime TimePosted { get { return this.timePosted; } }
		public PostCollection Posts { get { return this.posts; } }
		public AttributeCollection Attributes { get { return this.attributes; } }
		public int PostCount { get { return this.postCount; } }
		public DateTime TimeLastPost { get { return this.timeLastPost; } }
		public float SearchRelevance { get { return this.searchRelevance; } }
		public bool EffectivelyVisible { get { return this.effectivelyVisible; } }
		public string ConcurrencyIdentity { get { return this.concurrencyIdentity; } }
		#endregion

		#region Methods
		public override bool Equals(object obj) {
			Post post = obj as Post;
			return post != null &&
				this.id.Equals(post.id);
		}
		public override int GetHashCode() {
			return this.id.GetHashCode();
		}
		#endregion

		#region Helper Methods
		private static void ValidateStringLength(string value) {
			if(value != null && value.Length > 255) {
				throw new ArgumentOutOfRangeException("value");
			}
		}
		#endregion
	}

}