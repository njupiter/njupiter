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
using System.Collections;
using System.Globalization;

namespace nJupiter.Services.Forum {

	[Serializable]
	public sealed class PostCollection : ICollection {
		#region Variables
		private readonly Hashtable innerHash;
		private readonly PostComparer comparer;
		[NonSerialized]
		private ArrayList innerList;
		[NonSerialized]
		private bool isSorted;
		#endregion

		#region Constructors
		internal PostCollection(Post.Property sortProperty, string sortAttributeName, bool sortAscending) : this(new PostComparer(sortProperty, sortAttributeName, sortAscending), new Hashtable()) { }
		private PostCollection(PostComparer comparer, Hashtable hashTable) {
			this.comparer = comparer;
			this.innerHash = hashTable;
		}
		#endregion

		#region Properties
		public Post this[PostId id] { get { return (Post)this.innerHash[id]; } }
		public Post this[int index] { get { return (Post)InnerList[index]; } }

		private ArrayList InnerList {
			get {
				if(!this.isSorted) {
					ArrayList list = new ArrayList(this.innerHash.Values);
					this.innerList = this.innerList != null && this.innerList.IsSynchronized ? ArrayList.Synchronized(list) : list;
					this.innerList.Sort(this.comparer);
					this.isSorted = true;
				}
				return this.innerList;
			}
		}
		#endregion

		#region Methods
		public Post[] ToArray() {
			Post[] posts = new Post[this.Count];
			CopyTo(posts, 0);
			return posts;
		}
		public bool Contains(Post post) {
			return post != null && this.innerHash.ContainsKey(post.Id);
		}
		void ICollection.CopyTo(Array array, int index) {
			InnerList.CopyTo(array, index);
		}
		public void CopyTo(Post[] array, int index) {
			InnerList.CopyTo(array, index);
		}
		public static PostCollection Synchronized(PostCollection nonSync) {
			if(nonSync == null) {
				throw new ArgumentNullException("nonSync");
			}
			return new PostCollection(nonSync.comparer, Hashtable.Synchronized(nonSync.innerHash));
		}
		#endregion

		#region Internal Methods
		internal void Add(Post post) {
			this.innerHash.Add(post.Id, post);
			this.isSorted = false;
		}
		#endregion

		#region Implementation of IEnumerable
		IEnumerator IEnumerable.GetEnumerator() {
			return InnerList.GetEnumerator();
		}
		#endregion

		#region Implementation of ICollection
		public bool IsSynchronized { get { return InnerList.IsSynchronized; } }
		public int Count { get { return this.innerHash.Count; } }
		public object SyncRoot { get { return this; } }
		#endregion

		#region Helper Classes
		[Serializable]
		private sealed class PostComparer : IComparer {
			#region Variables
			private readonly bool sortAscending;
			private readonly Post.Property sortProperty;
			private readonly string sortAttributeName;
			#endregion

			#region Constructors
			public PostComparer(Post.Property sortProperty, string sortAttributeName, bool sortAscending) {
				this.sortProperty = sortProperty;
				this.sortAttributeName = sortAttributeName;
				this.sortAscending = sortAscending;
			}
			#endregion

			#region IComparer Members
			public int Compare(object x, object y) {
				Post postx = (Post)x;
				Post posty = (Post)y;
				int compareValue;

				if(this.sortAttributeName == null) {
					switch(this.sortProperty) {
						case Post.Property.Author:
						compareValue = string.Compare(postx.Author, posty.Author, true, CultureInfo.CurrentCulture);
						break;
						case Post.Property.Body:
						compareValue = string.Compare(postx.Body, posty.Body, true, CultureInfo.CurrentCulture);
						break;
						case Post.Property.Id:
						compareValue = postx.Id.CompareTo(posty.Id);
						break;
						case Post.Property.ParentId:
						compareValue = postx.ParentId == null ? posty.ParentId == null ? 0 : -1 : posty.ParentId == null ? 1 : postx.ParentId.CompareTo(posty.ParentId);
						break;
						case Post.Property.CategoryId:
						compareValue = postx.CategoryId == null ? posty.CategoryId == null ? 0 : -1 : posty.CategoryId == null ? 1 : postx.CategoryId.CompareTo(posty.CategoryId);
						break;
						case Post.Property.PostCount:
						compareValue = postx.PostCount.CompareTo(posty.PostCount);
						break;
						case Post.Property.SearchRelevance:
						compareValue = postx.SearchRelevance.CompareTo(posty.SearchRelevance);
						break;
						case Post.Property.TimeLastPost:
						compareValue = postx.TimeLastPost.CompareTo(posty.TimeLastPost);
						break;
						case Post.Property.Title:
						compareValue = string.Compare(postx.Title, posty.Title, true, CultureInfo.CurrentCulture);
						break;
						case Post.Property.UserIdentity:
						compareValue = string.Compare(postx.UserIdentity, posty.UserIdentity, true, CultureInfo.CurrentCulture);
						break;
						case Post.Property.Visible:
						compareValue = postx.Visible.CompareTo(posty.Visible);
						break;
						case Post.Property.EffectivelyVisible:
						compareValue = postx.EffectivelyVisible.CompareTo(posty.EffectivelyVisible);
						break;
						case Post.Property.EffectiveCategoryId:
						compareValue = postx.EffectiveCategoryId.CompareTo(posty.EffectiveCategoryId);
						break;
						case Post.Property.EffectiveCategoryName:
						compareValue = string.Compare(postx.EffectiveCategoryName, posty.EffectiveCategoryName, true, CultureInfo.CurrentCulture);
						if(compareValue.Equals(0)) {
							goto case Post.Property.EffectiveCategoryId;
						}
						break;
						case Post.Property.ConcurrencyIdentity:
						compareValue = string.Compare(postx.ConcurrencyIdentity, posty.ConcurrencyIdentity, true, CultureInfo.CurrentCulture);
						break;
						case Post.Property.RootPostId:
						compareValue = postx.RootPostId.CompareTo(posty.RootPostId);
						break;
						case Post.Property.RootPostTitle:
						compareValue = string.Compare(postx.RootPostTitle, posty.RootPostTitle, true, CultureInfo.CurrentCulture);
						if(compareValue.Equals(0)) {
							goto case Post.Property.RootPostId;
						}
						break;
						default:
						compareValue = postx.TimePosted.CompareTo(posty.TimePosted);
						break;
					}
				} else {
					object valuex = postx.Attributes[this.sortAttributeName].Value;
					object valuey = posty.Attributes[this.sortAttributeName].Value;
					compareValue = valuex == null ? valuey == null ? 0 : -1 : ((IComparable)valuex).CompareTo(valuey);
				}
				return compareValue.Equals(0) && !this.sortProperty.Equals(Post.Property.TimePosted) ?
					postx.TimePosted.CompareTo(posty.TimePosted)
					: (this.sortAscending ? 1 : -1) * compareValue;
			}
			#endregion
		}
		#endregion
	}

}