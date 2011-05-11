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
using System.Reflection;
using System.Collections.Specialized;
using System.Globalization;

using nJupiter.Configuration;

namespace nJupiter.Services.Forum {

	public abstract class ForumDao {
		#region Constants
		private const PostType DefaultPostType = PostType.This;

		private const string ParamNamePosts = "posts";
		private const string ParamNamePost = "post";
		private const string ParamNameCategories = "categories";
		private const string ParamNameCategory = "category";
		private const string ParamNameAttributes = "attributes";
		private const string ParamNameAttribute = "attribute";
		#endregion

		#region Variables
		private readonly IConfig settings;
		#endregion

		#region Constructors
		protected ForumDao(string forumDaoName) {
			const string settingsXpathFormat = ForumDaoXpath + "[@value='{0}']/settings";

			string xPath = string.Format(CultureInfo.InvariantCulture, settingsXpathFormat, forumDaoName);
			if(Config.ContainsKey(xPath)) {
				this.settings = Config.GetConfigSection(xPath);
			}
		}
		#endregion

		#region Domain Methods
		public abstract string[] GetDomains();
		#endregion

		#region Category Methods
		public Category CreateCategoryInstance(string name, string domain) {
			return new Category(name, domain, this.CategoryAttributes.Clone());
		}

		public Category GetCategory(CategoryId id) {
			return GetCategory(id, null, null);
		}
		public Category GetCategory(CategoryId id, CategoryResultConfiguration resultConfiguration, ThreadedPostsResultConfiguration postsResultConfiguration) {
			CategoryWithPagedPostsResult categoryWithPagedPostsResult = GetCategory(id, resultConfiguration, postsResultConfiguration, null);
			return categoryWithPagedPostsResult == null ? null : categoryWithPagedPostsResult.Category;
		}
		public abstract CategoryWithPagedPostsResult GetCategory(CategoryId id, CategoryResultConfiguration resultConfiguration, ThreadedPostsResultConfiguration postsResultConfiguration, PagingConfiguration pagingConfiguration);

		public Category GetCategory(string domain, string name) {
			return GetCategory(domain, name, null, null);
		}
		public Category GetCategory(string domain, string name, CategoryResultConfiguration resultConfiguration, ThreadedPostsResultConfiguration postsResultConfiguration) {
			CategoryWithPagedPostsResult categoryWithPagedPostsResult = GetCategory(domain, name, resultConfiguration, postsResultConfiguration, null);
			return categoryWithPagedPostsResult == null ? null : categoryWithPagedPostsResult.Category;
		}
		public abstract CategoryWithPagedPostsResult GetCategory(string domain, string name, CategoryResultConfiguration resultConfiguration, ThreadedPostsResultConfiguration postsResultConfiguration, PagingConfiguration pagingConfiguration);

		public Category GetCategory(PostId id) {
			return GetCategory(id, null, null);
		}
		public Category GetCategory(PostId id, CategoryResultConfiguration resultConfiguration, ThreadedPostsResultConfiguration postsResultConfiguration) {
			CategoryWithPagedPostsResult categoryWithPagedPostsResult = GetCategory(id, resultConfiguration, postsResultConfiguration, null);
			return categoryWithPagedPostsResult == null ? null : categoryWithPagedPostsResult.Category;
		}
		public abstract CategoryWithPagedPostsResult GetCategory(PostId id, CategoryResultConfiguration resultConfiguration, ThreadedPostsResultConfiguration postsResultConfiguration, PagingConfiguration pagingConfiguration);

		public CategoryCollection GetCategories() {
			return GetCategories(null, null, null);
		}
		public CategoryCollection GetCategories(CategoriesResultConfiguration resultConfiguration, ThreadedPostsResultConfiguration postsResultConfiguration) {
			return GetCategories(resultConfiguration, postsResultConfiguration, PagingConfiguration.DefaultPageSize);
		}
		public abstract CategoryCollection GetCategories(CategoriesResultConfiguration resultConfiguration, ThreadedPostsResultConfiguration postsResultConfiguration, int postsLimitSize);

		public CategoryCollection GetCategories(string domain) {
			return GetCategories(domain, null, null);
		}
		public CategoryCollection GetCategories(string domain, CategoriesResultConfiguration resultConfiguration, ThreadedPostsResultConfiguration postsResultConfiguration) {
			return GetCategories(domain, resultConfiguration, postsResultConfiguration, PagingConfiguration.DefaultPageSize);
		}
		public abstract CategoryCollection GetCategories(string domain, CategoriesResultConfiguration resultConfiguration, ThreadedPostsResultConfiguration postsResultConfiguration, int postsLimitSize);

		public abstract void SaveCategory(Category category);

		public abstract bool DeleteCategory(CategoryId id);
		public abstract bool DeleteCategory(string domain, string name);

		public abstract int DeleteCategories();
		public abstract int DeleteCategories(string domain);
		#endregion

		#region Post Methods
		public Post CreatePostInstance(string domain, string categoryName) {
			if(domain == null) {
				throw new ArgumentNullException("domain");
			}
			if(domain == null) {
				throw new ArgumentNullException("categoryName");
			}
			Category category = GetCategory(domain, categoryName);
			if(category == null) {
				throw new CategoryNotFoundException("The specified category cannot be found.");
			}
			return new Post(category.Id, this.PostAttributes.Clone());
		}

		public Post CreatePostInstance(CategoryId categoryId) {
			if(categoryId == null) {
				throw new ArgumentNullException("categoryId");
			}
			return new Post(categoryId, this.PostAttributes.Clone());
		}

		public Post CreatePostInstance(PostId parentId) {
			if(parentId == null) {
				throw new ArgumentNullException("parentId");
			}
			return new Post(parentId, this.PostAttributes.Clone());
		}

		public Post GetPost(PostId id) {
			return GetPost(id, null);
		}
		public Post GetPost(PostId id, PostType postType) {
			return GetPost(id, postType, null);
		}
		public Post GetPost(PostId id, ThreadedPostsResultConfiguration resultConfiguration) {
			return GetPost(id, DefaultPostType, resultConfiguration);
		}
		public Post GetPost(PostId id, PostType postType, ThreadedPostsResultConfiguration resultConfiguration) {
			PostWithPagedPostsResult postWithPagedPostsResult = GetPost(id, postType, resultConfiguration, null);
			return postWithPagedPostsResult == null ? null : postWithPagedPostsResult.Post;
		}
		public PostWithPagedPostsResult GetPost(PostId id, ThreadedPostsResultConfiguration resultConfiguration, PagingConfiguration pagingConfiguration) {
			return GetPost(id, DefaultPostType, resultConfiguration, pagingConfiguration);
		}
		public abstract PostWithPagedPostsResult GetPost(PostId id, PostType postType, ThreadedPostsResultConfiguration resultConfiguration, PagingConfiguration pagingConfiguration);

		public PostCollection GetPosts() {
			return GetPosts((ThreadedPostsResultConfiguration)null);
		}
		public PostCollection GetPosts(ThreadedPostsResultConfiguration resultConfiguration) {
			return GetPosts(resultConfiguration, null).Posts;
		}
		public abstract PagedPostsResult GetPosts(ThreadedPostsResultConfiguration resultConfiguration, PagingConfiguration pagingConfiguration);

		public PostCollection GetPosts(string domain) {
			return GetPosts(domain, (ThreadedPostsResultConfiguration)null);
		}
		public PostCollection GetPosts(string domain, ThreadedPostsResultConfiguration resultConfiguration) {
			return GetPosts(domain, resultConfiguration, null).Posts;
		}
		public abstract PagedPostsResult GetPosts(string domain, ThreadedPostsResultConfiguration resultConfiguration, PagingConfiguration pagingConfiguration);

		public PostCollection GetPosts(CategoryId id) {
			return GetPosts(id, null);
		}
		public PostCollection GetPosts(CategoryId id, ThreadedPostsResultConfiguration resultConfiguration) {
			return GetPosts(id, resultConfiguration, null).Posts;
		}
		public abstract PagedPostsResult GetPosts(CategoryId id, ThreadedPostsResultConfiguration resultConfiguration, PagingConfiguration pagingConfiguration);

		public PostCollection GetPosts(string domain, string categoryName) {
			return GetPosts(domain, categoryName, null);
		}
		public PostCollection GetPosts(string domain, string categoryName, ThreadedPostsResultConfiguration resultConfiguration) {
			return GetPosts(domain, categoryName, resultConfiguration, null).Posts;
		}
		public abstract PagedPostsResult GetPosts(string domain, string categoryName, ThreadedPostsResultConfiguration resultConfiguration, PagingConfiguration pagingConfiguration);

		public PostCollection GetPosts(PostId id) {
			return GetPosts(id, null);
		}
		public PostCollection GetPosts(PostId id, PostType postType) {
			return GetPosts(id, postType, null);
		}
		public PostCollection GetPosts(PostId id, ThreadedPostsResultConfiguration resultConfiguration) {
			return GetPosts(id, resultConfiguration, null).Posts;
		}
		public PostCollection GetPosts(PostId id, PostType postType, ThreadedPostsResultConfiguration resultConfiguration) {
			return GetPosts(id, postType, resultConfiguration, null).Posts;
		}
		public PagedPostsResult GetPosts(PostId id, ThreadedPostsResultConfiguration resultConfiguration, PagingConfiguration pagingConfiguration) {
			return GetPosts(id, DefaultPostType, resultConfiguration, pagingConfiguration);
		}
		public abstract PagedPostsResult GetPosts(PostId id, PostType postType, ThreadedPostsResultConfiguration resultConfiguration, PagingConfiguration pagingConfiguration);

		public PostCollection GetNonThreadedPosts() {
			return GetNonThreadedPosts((PostsResultConfiguration)null);
		}
		public PostCollection GetNonThreadedPosts(PostsResultConfiguration resultConfiguration) {
			return this.GetNonThreadedPosts(resultConfiguration, null).Posts;
		}
		public abstract PagedPostsResult GetNonThreadedPosts(PostsResultConfiguration resultConfiguration, PagingConfiguration pagingConfiguration);

		public PostCollection GetNonThreadedPosts(string domain) {
			return GetNonThreadedPosts(domain, (PostsResultConfiguration)null);
		}
		public PostCollection GetNonThreadedPosts(string domain, PostsResultConfiguration resultConfiguration) {
			return GetNonThreadedPosts(domain, resultConfiguration, null).Posts;
		}
		public abstract PagedPostsResult GetNonThreadedPosts(string domain, PostsResultConfiguration resultConfiguration, PagingConfiguration pagingConfiguration);

		public PostCollection GetNonThreadedPosts(CategoryId id) {
			return GetNonThreadedPosts(id, null);
		}
		public PostCollection GetNonThreadedPosts(CategoryId id, PostsResultConfiguration resultConfiguration) {
			return GetNonThreadedPosts(id, resultConfiguration, null).Posts;
		}
		public abstract PagedPostsResult GetNonThreadedPosts(CategoryId id, PostsResultConfiguration resultConfiguration, PagingConfiguration pagingConfiguration);

		public PostCollection GetNonThreadedPosts(string domain, string categoryName) {
			return GetNonThreadedPosts(domain, categoryName, null);
		}
		public PostCollection GetNonThreadedPosts(string domain, string categoryName, PostsResultConfiguration resultConfiguration) {
			return GetNonThreadedPosts(domain, categoryName, resultConfiguration, null).Posts;
		}
		public abstract PagedPostsResult GetNonThreadedPosts(string domain, string categoryName, PostsResultConfiguration resultConfiguration, PagingConfiguration pagingConfiguration);

		public PostCollection GetNonThreadedPosts(PostId id) {
			return GetNonThreadedPosts(id, null);
		}
		public PostCollection GetNonThreadedPosts(PostId id, PostType postType) {
			return GetNonThreadedPosts(id, postType, null);
		}
		public PostCollection GetNonThreadedPosts(PostId id, PostsResultConfiguration resultConfiguration) {
			return GetNonThreadedPosts(id, resultConfiguration, null).Posts;
		}
		public PostCollection GetNonThreadedPosts(PostId id, PostType postType, PostsResultConfiguration resultConfiguration) {
			return GetNonThreadedPosts(id, postType, resultConfiguration, null).Posts;
		}
		public PagedPostsResult GetNonThreadedPosts(PostId id, PostsResultConfiguration resultConfiguration, PagingConfiguration pagingConfiguration) {
			return GetNonThreadedPosts(id, DefaultPostType, resultConfiguration, pagingConfiguration);
		}
		public abstract PagedPostsResult GetNonThreadedPosts(PostId id, PostType postType, PostsResultConfiguration resultConfiguration, PagingConfiguration pagingConfiguration);

		public int GetNumberOfPosts() {
			return GetNumberOfPosts((NumberOfPostsResultConfiguration)null);
		}
		public abstract int GetNumberOfPosts(NumberOfPostsResultConfiguration resultConfiguration);

		public int GetNumberOfPosts(string domain) {
			return GetNumberOfPosts(domain, (NumberOfPostsResultConfiguration)null);
		}
		public abstract int GetNumberOfPosts(string domain, NumberOfPostsResultConfiguration resultConfiguration);

		public int GetNumberOfPosts(CategoryId categoryId) {
			return GetNumberOfPosts(categoryId, null);
		}
		public abstract int GetNumberOfPosts(CategoryId categoryId, NumberOfPostsResultConfiguration resultConfiguration);

		public int GetNumberOfPosts(string domain, string categoryName) {
			return GetNumberOfPosts(domain, categoryName, null);
		}
		public abstract int GetNumberOfPosts(string domain, string categoryName, NumberOfPostsResultConfiguration resultConfiguration);

		public int GetNumberOfPosts(PostId postId) {
			return GetNumberOfPosts(postId, null);
		}
		public abstract int GetNumberOfPosts(PostId postId, NumberOfPostsResultConfiguration resultConfiguration);

		public PostCollection SearchPosts(SearchCriteria searchCriteria) {
			return SearchPosts(searchCriteria, null);
		}
		public PostCollection SearchPosts(SearchCriteria searchCriteria, PostsResultConfiguration resultConfiguration) {
			return SearchPosts(searchCriteria, resultConfiguration, null).Posts;
		}
		public abstract PagedPostsResult SearchPosts(SearchCriteria searchCriteria, PostsResultConfiguration resultConfiguration, PagingConfiguration pagingConfiguration);

		public PostCollection SearchPosts(string domain, SearchCriteria searchCriteria) {
			return SearchPosts(domain, searchCriteria, null);
		}
		public PostCollection SearchPosts(string domain, SearchCriteria searchCriteria, PostsResultConfiguration resultConfiguration) {
			return SearchPosts(domain, searchCriteria, resultConfiguration, null).Posts;
		}
		public abstract PagedPostsResult SearchPosts(string domain, SearchCriteria searchCriteria, PostsResultConfiguration resultConfiguration, PagingConfiguration pagingConfiguration);

		public PostCollection SearchPosts(CategoryId id, SearchCriteria searchCriteria) {
			return SearchPosts(id, searchCriteria, null);
		}
		public PostCollection SearchPosts(CategoryId id, SearchCriteria searchCriteria, PostsResultConfiguration resultConfiguration) {
			return SearchPosts(id, searchCriteria, resultConfiguration, null).Posts;
		}
		public abstract PagedPostsResult SearchPosts(CategoryId id, SearchCriteria searchCriteria, PostsResultConfiguration resultConfiguration, PagingConfiguration pagingConfiguration);

		public PostCollection SearchPosts(string domain, string categoryName, SearchCriteria searchCriteria) {
			return SearchPosts(domain, categoryName, searchCriteria, null);
		}
		public PostCollection SearchPosts(string domain, string categoryName, SearchCriteria searchCriteria, PostsResultConfiguration resultConfiguration) {
			return SearchPosts(domain, categoryName, searchCriteria, resultConfiguration, null).Posts;
		}
		public abstract PagedPostsResult SearchPosts(string domain, string categoryName, SearchCriteria searchCriteria, PostsResultConfiguration resultConfiguration, PagingConfiguration pagingConfiguration);

		public PostCollection SearchPosts(PostId id, SearchCriteria searchCriteria) {
			return SearchPosts(id, searchCriteria, null);
		}
		public PostCollection SearchPosts(PostId id, PostType postType, SearchCriteria searchCriteria) {
			return SearchPosts(id, postType, searchCriteria, null);
		}
		public PostCollection SearchPosts(PostId id, SearchCriteria searchCriteria, PostsResultConfiguration resultConfiguration) {
			return SearchPosts(id, searchCriteria, resultConfiguration, null).Posts;
		}
		public PostCollection SearchPosts(PostId id, PostType postType, SearchCriteria searchCriteria, PostsResultConfiguration resultConfiguration) {
			return SearchPosts(id, postType, searchCriteria, resultConfiguration, null).Posts;
		}
		public PagedPostsResult SearchPosts(PostId id, SearchCriteria searchCriteria, PostsResultConfiguration resultConfiguration, PagingConfiguration pagingConfiguration) {
			return SearchPosts(id, DefaultPostType, searchCriteria, resultConfiguration, pagingConfiguration);
		}
		public abstract PagedPostsResult SearchPosts(PostId id, PostType postType, SearchCriteria searchCriteria, PostsResultConfiguration resultConfiguration, PagingConfiguration pagingConfiguration);

		public abstract void SavePost(Post post);

		public abstract int MovePosts(string fromDomain, string fromCategoryName, string toDomain, string toCategoryName, DateTime until);
		public int MovePosts(string fromDomain, string fromCategoryName, string toDomain, string toCategoryName) {
			return MovePosts(fromDomain, fromCategoryName, toDomain, toCategoryName, DateTime.MinValue);
		}
		public abstract int MovePosts(string fromDomain, string fromCategoryName, CategoryId toCategoryId, DateTime until);
		public int MovePosts(string fromDomain, string fromCategoryName, CategoryId toCategoryId) {
			return MovePosts(fromDomain, fromCategoryName, toCategoryId, DateTime.MinValue);
		}
		public abstract int MovePosts(CategoryId fromCategoryId, string toDomain, string toCategoryName, DateTime until);
		public int MovePosts(CategoryId fromCategoryId, string toDomain, string toCategoryName) {
			return MovePosts(fromCategoryId, toDomain, toCategoryName, DateTime.MinValue);
		}
		public abstract int MovePosts(CategoryId fromCategoryId, CategoryId toCategoryId, DateTime until);
		public int MovePosts(CategoryId fromCategoryId, CategoryId toCategoryId) {
			return MovePosts(fromCategoryId, toCategoryId, DateTime.MinValue);
		}

		public abstract int DeletePost(PostId id);

		public abstract int DeletePosts(DateTime until);
		public int DeletePosts() {
			return DeletePosts(DateTime.MinValue);
		}
		public abstract int DeletePosts(string domain, DateTime until);
		public int DeletePosts(string domain) {
			return DeletePosts(domain, DateTime.MinValue);
		}
		public abstract int DeletePosts(CategoryId id, DateTime until);
		public int DeletePosts(CategoryId id) {
			return DeletePosts(id, DateTime.MinValue);
		}
		public abstract int DeletePosts(string domain, string categoryName, DateTime until);
		public int DeletePosts(string domain, string categoryName) {
			return DeletePosts(domain, categoryName, DateTime.MinValue);
		}
		public abstract int DeletePosts(PostId id, DateTime until);
		public int DeletePosts(PostId id) {
			return DeletePosts(id, DateTime.MinValue);
		}
		#endregion

		#region Protected Methods
		protected string GetSetting(string setting) {
			return this.settings == null || !this.settings.ContainsKey(setting) ? null : this.settings.GetValue(setting);
		}

		protected static Post CreatePostInstance(PostId id, PostId parentId, CategoryId categoryId, PostId rootPostId, string rootPostTitle, CategoryId effectiveCategoryId, string effectiveCategoryName, AttributeCollection attributes, PostCollection children, DateTime timePosted, DateTime timeLastPost, int postCount, float searchRelevance, bool effectivelyVisible, string concurrencyIdentity) {
			return new Post(id, parentId, categoryId, rootPostId, rootPostTitle, effectiveCategoryId, effectiveCategoryName, attributes, children, timePosted, timeLastPost, postCount, searchRelevance, effectivelyVisible, concurrencyIdentity);
		}
		protected static PostId CreatePostIdInstance(Guid value) {
			return new PostId(value);
		}
		protected static PostCollection CreatePostCollectionInstance(Post.Property sortProperty, string sortAttributeName, bool sortAscending) {
			return new PostCollection(sortProperty, sortAttributeName, sortAscending);
		}
		protected static void AddPostToPostCollection(PostCollection posts, Post post) {
			if(posts == null) {
				throw new ArgumentNullException(ParamNamePosts);
			}
			if(post == null) {
				throw new ArgumentNullException(ParamNamePost);
			}
			posts.Add(post);
		}
		protected static PagedPostsResult CreatePagedPostsResultInstance(PostCollection posts, int totalCount) {
			return new PagedPostsResult(posts, totalCount);
		}
		protected static PostWithPagedPostsResult CreatePostWithPagedPostsResultsInstance(Post post, int totalCount) {
			return new PostWithPagedPostsResult(post, totalCount);
		}

		protected static Category CreateCategoryInstance(CategoryId id, string domain, string name, AttributeCollection attributes, PostCollection posts, int rootPostCount, string concurrencyIdentity) {
			return new Category(id, domain, name, attributes, posts, rootPostCount, concurrencyIdentity);
		}
		protected static CategoryId CreateCategoryIdInstance(Guid value) {
			return new CategoryId(value);
		}
		protected static CategoryCollection CreateCategoryCollectionInstance(Category.Property sortProperty, string sortAttributeName, bool sortAscending) {
			return new CategoryCollection(sortProperty, sortAttributeName, sortAscending);
		}
		protected static void AddCategoryToCategoryCollection(CategoryCollection categories, Category category) {
			if(categories == null) {
				throw new ArgumentNullException(ParamNameCategories);
			}
			if(category == null) {
				throw new ArgumentNullException(ParamNameCategory);
			}
			categories.Add(category);
		}
		protected static CategoryWithPagedPostsResult CreateCategoryWithPagedPostsResultInstance(Category category, int totalCount) {
			return new CategoryWithPagedPostsResult(category, totalCount);
		}

		protected static Attribute CreateAttributeInstance(string name, Type type) {
			return Attribute.Create(name, type);
		}
		protected static AttributeCollection CreateAttributeCollectionInstance() {
			return new AttributeCollection();
		}
		/// <summary>
		/// Returns a new attribute collection containing shallow copies of the attributes contained in the supplied attribute collection.
		/// </summary>
		protected static AttributeCollection CloneAttributeCollection(AttributeCollection attributes) {
			if(attributes == null) {
				throw new ArgumentNullException(ParamNameAttributes);
			}
			return attributes.Clone();
		}
		protected static void AddAttributeToAttributeCollection(AttributeCollection attributes, Attribute attribute) {
			if(attributes == null) {
				throw new ArgumentNullException(ParamNameAttributes);
			}
			if(attribute == null) {
				throw new ArgumentNullException(ParamNameAttribute);
			}
			attributes.Add(attribute);
		}
		#endregion

		#region Protected Properties
		protected AttributeCollection PostAttributes { get; set; }
		protected AttributeCollection CategoryAttributes { get; set; }
		#endregion

		#region Abstract Factory
		#region Constants
		private const string ForumDaoXpath = "forumDAOs/forumDAO";
		private const string NamedForumDaoXpathFormat = ForumDaoXpath + "[@value='{0}']";
		#endregion

		#region Variables
		private static readonly ListDictionary ForumDaos = new ListDictionary();
		private static readonly IConfig Config = ConfigHandlerOld.GetConfig();
		private static ForumDao defaultInstance;
		private static readonly object LockObject = new object();
		#endregion

		#region Methods
		public static ForumDao GetInstance() {
			const string defaultForumDaoXpath = ForumDaoXpath + "[@default='true']";

			if(defaultInstance != null) {
				return defaultInstance;
			}
			lock(LockObject) {
				defaultInstance = GetForumDaoFromSection(defaultForumDaoXpath);
				return defaultInstance;
			}
		}
		public static ForumDao GetInstance(string name) {
			if(!ForumDaos.Contains(name)) {
				return GetForumDaoFromSection(string.Format(CultureInfo.InvariantCulture, NamedForumDaoXpathFormat, name));
			}
			return (ForumDao)ForumDaos[name];
		}
		#endregion

		#region Helper Methods
		private static ForumDao GetForumDaoFromSection(string section) {
			const string assemblyPathKey = "assemblyPath";
			const string assemblyKey = "assembly";
			const string typeKey = "type";

			string name = Config.GetValue(section);

			if(ForumDaos.Contains(name)) {
				return (ForumDao)ForumDaos[name];
			}
			lock(ForumDaos.SyncRoot) {
				object forumDao = GetInstance(
					Config.GetValue(section, assemblyPathKey),
					Config.GetValue(section, assemblyKey),
					Config.GetValue(section, typeKey),
					new object[] { name });
				ForumDaos.Add(name, forumDao);
				return (ForumDao)forumDao;
			}
		}
		private static object GetInstance(string assemblyPath, string assemblyName, string typeName, object[] prams) {
			Assembly assembly;
			if(!string.IsNullOrEmpty(assemblyPath)) {
				assembly = Assembly.LoadFrom(assemblyPath);
			} else if(assemblyName == null || assemblyName.Length.Equals(0) ||
				Assembly.GetExecutingAssembly().GetName().Name.Equals(assemblyName)) {
				assembly = Assembly.GetExecutingAssembly();
				//Load current assembly
			} else {
				assembly = Assembly.Load(assemblyName);
				// Late binding to an assembly on disk (current directory)
			}
			return assembly.CreateInstance(
				typeName, true,
				BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly |
				BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.ExactBinding,
				null, prams, null, null);
		}
		#endregion
		#endregion
	}

}