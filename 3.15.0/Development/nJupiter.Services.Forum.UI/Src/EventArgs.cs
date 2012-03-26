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
using System.ComponentModel;

namespace nJupiter.Services.Forum.UI {
	public delegate void CategorySelectedEventHandler(object sender, EventArgs e);

	public delegate void AddPostEventHandler(object sender, AddPostEventArgs e);
	public delegate void ChangePostVisibleEventHandler(object sender, PostEventArgs e);
	public delegate void UpdatePostEventHandler(object sender, UpdatePostEventArgs e);
	public delegate void DeletePostEventHandler(object sender, PostEventArgs e);

	public delegate void PostAddingEventHandler(object sender, PostCancelEventArgs e);
	public delegate void PostAddedEventHandler(object sender, PostEventArgs e);
	public delegate void PostAdditionFailedEventHandler(object sender, PostFailureEventArgs e);
	public delegate void PostAdditionDiscardedEventHandler(object sender, EventArgs e);

	public delegate void PostUpdatingEventHandler(object sender, PostCancelEventArgs e);
	public delegate void PostUpdatedEventHandler(object sender, PostEventArgs e);
	public delegate void PostUpdateFailedEventHandler(object sender, PostFailureEventArgs e);
	public delegate void PostUpdateDiscardedEventHandler(object sender, EventArgs e);

	public delegate void PostDeletingEventHandler(object sender, PostCancelEventArgs e);
	public delegate void PostDeletedEventHandler(object sender, PostDeletedEventArgs e);

	public delegate void PostVisibleChangingEventHandler(object sender, PostCancelEventArgs e);
	public delegate void PostVisibleChangedEventHandler(object sender, PostEventArgs e);
	public delegate void PostVisibleChangeFailedEventHandler(object sender, PostFailureEventArgs e);
	
	public delegate void SearchEventHandler(object sender, SearchEventArgs e);
	public delegate void SearchDiscardedEventHandler(object sender, EventArgs e);

	public delegate void PostsSortingEventHandler(object sender, PostsSortEventArgs e);

	[Serializable]
	public sealed class AddPostEventArgs : EventArgs {
		#region Variables
		private readonly PostId		postId;
		private readonly CategoryId	categoryId;
		private readonly string		domain;
		private readonly string		categoryName;
		private readonly PostId		replySourcePostId;
		#endregion

		#region Properties
		public CategoryId CategoryId { get { return this.categoryId; } }
		public PostId PostId { get { return this.postId; } }
		public string Domain { get { return this.domain; } }
		public string CategoryName { get { return this.categoryName; } }
		public PostId ReplySourcePostId { get { return this.replySourcePostId; } }
		#endregion

		#region Constructors
		public AddPostEventArgs(PostId postId, PostId replySourcePostId) {
			this.postId			= postId;
			this.replySourcePostId	= replySourcePostId;
		}
		public AddPostEventArgs(PostId postId) : this(postId, postId) {}
		public AddPostEventArgs(CategoryId categoryId) {
			this.categoryId = categoryId;
		}
		public AddPostEventArgs(string domain, string categoryName) {
			this.domain = domain;
			this.categoryName = categoryName;
		}
		#endregion
	}

	[Serializable]
	public sealed class UpdatePostEventArgs : EventArgs {
		#region Variables
		private readonly PostId		postId;
		#endregion

		#region Properties
		public PostId PostId { get { return this.postId; } }
		#endregion

		#region Constructors
		public UpdatePostEventArgs(PostId postId) {
			this.postId = postId;
		}
		#endregion
	}

	[Serializable]
	public class PostEventArgs : EventArgs {
		#region Variables
		private readonly Post	post;
		#endregion

		#region Constructors
		public PostEventArgs(Post post) {
			this.post = post;
		}
		#endregion

		#region Properties
		public Post Post { get { return this.post; } }
		#endregion
	}

	[Serializable]
	public sealed class PostDeletedEventArgs : PostEventArgs {
		#region Variables
		private readonly int affectedPosts;
		#endregion

		#region Constructors
		public PostDeletedEventArgs(Post post, int affectedPosts) : base(post) {
			this.affectedPosts = affectedPosts;
		}
		#endregion

		#region Properties
		public int AffectedPosts { get { return this.affectedPosts; } }
		#endregion
	}

	[Serializable]
	public sealed class PostFailureEventArgs : PostEventArgs {
		#region Variables
		private readonly Exception	exception;
		#endregion

		#region Constructors
		public PostFailureEventArgs(Post post, Exception exception) : base(post) {
			this.exception = exception;
		}
		#endregion

		#region Properties
		public Exception Exception { get { return this.exception; } }
		#endregion
	}

	[Serializable]
	public class PostCancelEventArgs : CancelEventArgs {
		#region Variables
		private readonly Post	post;
		#endregion

		#region Constructors
		public PostCancelEventArgs(Post post) {
			this.post = post;
		}
		#endregion

		#region Properties
		public Post Post { get { return this.post; } }
		#endregion
	}

	[Serializable]
	public sealed class SearchEventArgs : EventArgs {
		#region Variables
		private readonly string	searchText;
		#endregion

		#region Constructors
		public SearchEventArgs(string searchText) {
			this.searchText = searchText;
		}
		#endregion

		#region Properties
		public string SearchText { get { return this.searchText; } }
		#endregion
	}

	[Serializable]
	public sealed class PostsSortEventArgs : EventArgs {
		#region Variables
		private Post.Property	property;
		private string			attributeName;
		private bool			ascending;
		#endregion

		#region Constructors
		public PostsSortEventArgs(Post.Property property, string attributeName, bool ascending) {
			this.property		= property;
			this.attributeName	= attributeName;
			this.ascending		= ascending;
		}
		#endregion

		#region Properties
		public Post.Property Property { get { return this.property; } set { this.property = value; } }
		public string AttributeName { get { return this.attributeName; } set { this.attributeName = value; } }
		public bool Ascending { get { return this.ascending; } set { this.ascending = value; } }
		#endregion
	}

}