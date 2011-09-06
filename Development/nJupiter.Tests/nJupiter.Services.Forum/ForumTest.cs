using System.Threading;
using System.Security.Principal;

using NUnit.Framework;

namespace nJupiter.Services.Forum {

	[TestFixture, Ignore("Old tests depending on db")]
	public class ForumTest {
		private const string	Domain = "sv-SE";
		private ForumDao		forumDao;
		private CategoryId		categoryId;
		private PostId			postId;
		private PostId			childPostId;
		private PostId			leafChildPostId;

		[TestFixtureSetUp]
		public void Init() {
			this.forumDao = ForumDao.GetInstance();
		}

		[TestFixtureTearDown]
		public void Dispose() {
			if(this.categoryId != null) {
				this.forumDao.DeleteCategory(this.categoryId);
			}
		}

		[Test]
		public void Test_01_CreateCategory() {
			Category category = this.forumDao.CreateCategoryInstance("TestCategory", Domain);
			this.forumDao.SaveCategory(category);
			Category dbCategory = this.forumDao.GetCategory(category.Id);
			this.categoryId = dbCategory.Id;
			Assert.IsNotNull(dbCategory);
		}

		[Test]
		public void Test_02_CreatePost() {
			Assert.IsNotNull(this.categoryId, "You must run the CreateCategory method before running this method.");
			Post post = this.forumDao.CreatePostInstance(this.categoryId);
			post.Author = WindowsIdentity.GetCurrent().Name;
			post.Body = @"Test,
and this is on several rows in the database, 
which is nice, but this is not what this test is about, ok?";
			post.Title = "Meaningless subject";
			post.UserIdentity = "andyandy";
			this.forumDao.SavePost(post);
			Post dbPost = this.forumDao.GetPost(post.Id, PostType.This);
			this.postId = post.Id;
			Assert.IsNotNull(dbPost);
		}

		[Test]
		public void Test_03_CreateReply() {
			Assert.IsNotNull(this.postId, "You must run the CreatePost method before running this method.");
			Post post = this.forumDao.CreatePostInstance(this.postId);
			post.Author = WindowsIdentity.GetCurrent().Name;
			post.Body = @"Ok, then I know";
			post.Title = "Even more meaningless subject";
			post.UserIdentity = "pandypandy";
			this.forumDao.SavePost(post);	
			Post dbPost = this.forumDao.GetPost(post.Id, PostType.This);
			this.childPostId = post.Id;
			Assert.IsNotNull(dbPost);
		}

		[Test]
		public void Test_04_CreateReplyOnReply() {
			Assert.IsNotNull(this.postId, "You must run the CreatePost method before running this method.");
			Assert.IsNotNull(this.childPostId, "You must run the CreateReply method before running this method.");
			Post post = this.forumDao.CreatePostInstance(this.childPostId);
			post.Author = WindowsIdentity.GetCurrent().Name;
			post.Body = @"Ok, ok";
			post.Title = "Even more retarded subject";
			post.UserIdentity = "candylandy";
			this.forumDao.SavePost(post);	
			Post dbPost = this.forumDao.GetPost(post.Id, PostType.This);
			this.leafChildPostId = dbPost.Id;
			Assert.IsNotNull(dbPost);
		}

		[Test]
		public void Test_05_RetrievePostWithReply() {
			Assert.IsNotNull(this.postId, "You must run the CreatePost method before running this method.");
			Post dbPost = this.forumDao.GetPost(this.postId, PostType.This);
			Assert.IsTrue(dbPost.Posts.Count.Equals(1));
			Assert.IsTrue(dbPost.Posts[0].Posts.Count.Equals(1));
			Assert.IsTrue(dbPost.Posts[0].Posts[0].Posts.Count.Equals(0));
			Assert.IsTrue(dbPost.PostCount.Equals(2));
		}

		[Test]
		public void Test_06_SearchForPostsInCulture() {
			Thread.Sleep(20000);
			PostCollection posts = this.forumDao.SearchPosts(Domain, new SearchCriteria("Subject"));
			Assert.IsTrue(posts.Count.Equals(3));
		}
		[Test]
		public void Test_07_SearchForPostsInThread() {
			SearchCriteria searchCriteria = new SearchCriteria("Subject");
			PostCollection posts = this.forumDao.SearchPosts(this.postId, PostType.This, searchCriteria);
			PostCollection postsfromkid = this.forumDao.SearchPosts(this.childPostId, PostType.Parent, searchCriteria);
			PostCollection postsfromkidkid = this.forumDao.SearchPosts(this.leafChildPostId, PostType.Root, searchCriteria);
			Assert.IsTrue(posts.Count.Equals(3) && postsfromkid.Count.Equals(3) && postsfromkidkid.Count.Equals(3));
		}

		[Test]
		public void Test_08_SearchForPostsInThreadChild() {
			SearchCriteria searchCriteria = new SearchCriteria("Subject");
			PostCollection postsfromkid = this.forumDao.SearchPosts(this.childPostId, PostType.This, searchCriteria);
			PostCollection postsfromkidkid = this.forumDao.SearchPosts(this.leafChildPostId, PostType.Parent, searchCriteria);
			Assert.IsTrue(postsfromkid.Count.Equals(2) && postsfromkidkid.Count.Equals(2));
		}

		[Test]
		public void Test_09_SearchForPostsInLeafChild() {
			SearchCriteria searchCriteria = new SearchCriteria("Subject");
			PostCollection postsfromkidkid = this.forumDao.SearchPosts(this.leafChildPostId, PostType.This, searchCriteria);
			Assert.IsTrue(postsfromkidkid.Count.Equals(1));
		}

		[Test]
		public void Test_10_SearchForPostsInCategory() {
			SearchCriteria searchCriteria = new SearchCriteria("Subject");
			PostCollection posts = this.forumDao.SearchPosts(this.categoryId, searchCriteria);
			Assert.IsTrue(posts.Count.Equals(3));
		}

		[Test]
		public void Test_11_DeleteCategory() {
			Assert.IsNotNull(this.categoryId, "You must run the CreateCategory method before running this method.");
			this.forumDao.DeleteCategory(this.categoryId);
			Category dbCategory = this.forumDao.GetCategory(this.categoryId);
			Assert.IsNull(dbCategory);
		}

	}
}
