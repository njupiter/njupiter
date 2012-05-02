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

#if DEBUG
using System;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Security.Principal;

using nJupiter.Web;

namespace nJupiter.Services.Forum.UI.Web {

	public class ForumTest : Page {
		#region UI Members
		protected FlatTabularPostList ctrlFlatTabularPostList;
		protected HierarchicalPostList ctrlHierarchicalPostList;
		protected AddPost ctrlAddPost;
		protected UpdatePost ctrlUpdatePost;
		protected CategorySelector ctrlCategorySelector;
		#endregion

		#region Event Handlers
		private static void ctrlFlatTabularPostList_PostsSorting(object sender, PostsSortEventArgs e) {
		}
		private void AddPost(object sender, AddPostEventArgs e) {
			ctrlFlatTabularPostList.AddPostButtonVisible = false;
			ctrlHierarchicalPostList.AddPostButtonVisibility = HierarchicalPostListVisibility.DoNotShowOnAnyPosts;
			ctrlAddPost.PostId = e.PostId;
			ctrlAddPost.CategoryId = e.CategoryId;
			ctrlAddPost.Domain = e.Domain;
			ctrlAddPost.CategoryName = e.CategoryName;
			ctrlAddPost.ReplySourcePostId = e.ReplySourcePostId;
			ctrlAddPost.UserIdentity = ctrlAddPost.Author = WindowsIdentity.GetCurrent().Name;
			ctrlAddPost.Visible = true;
		}
		private void UpdatePost(object sender, UpdatePostEventArgs e) {
			ctrlHierarchicalPostList.UpdatePostButtonVisibility = PostAdministrationVisibility.DoNotShowOnAnyPosts;
			ctrlUpdatePost.PostId = e.PostId;
			ctrlUpdatePost.Visible = true;
		}
		private static void ctrlHierarchicalPostList_PostVisibleChanged(object sender, PostEventArgs e) {
		}
		private static void ctrlHierarchicalPostList_PostVisibleChanging(object sender, PostCancelEventArgs e) {
		}
		private static void ctrlHierarchicalPostList_PostVisibleChangeFailed(object sender, PostFailureEventArgs e) {
		}
		private static void ctrlHierarchicalPostList_PostDeleted(object sender, PostDeletedEventArgs e) {
		}
		private static void ctrlHierarchicalPostList_PostDeleting(object sender, PostCancelEventArgs e) {
		}
		private void ctrlAddPost_PostAdded(object sender, PostEventArgs e) {
			ctrlHierarchicalPostList.AddPostButtonVisibility = HierarchicalPostListVisibility.ShowOnAllPosts;
			ctrlFlatTabularPostList.AddPostButtonVisible = true;
			ctrlAddPost.Visible = false;
		}
		private void ctrlAddPost_PostAdditionDiscarded(object sender, EventArgs e) {
			ctrlHierarchicalPostList.AddPostButtonVisibility = HierarchicalPostListVisibility.ShowOnAllPosts;
			ctrlFlatTabularPostList.AddPostButtonVisible = true;
			ctrlAddPost.Visible = false;
		}
		private void ctrlAddPost_PostAdditionFailed(object sender, PostFailureEventArgs e) {
			ctrlHierarchicalPostList.AddPostButtonVisibility = HierarchicalPostListVisibility.DoNotShowOnAnyPosts;
			ctrlFlatTabularPostList.AddPostButtonVisible = false;
		}
		private static void ctrlAddPost_PostAdding(object sender, PostCancelEventArgs e) {
			//e.Cancel = true;
		}
		private void ctrlUpdatePost_PostUpdated(object sender, PostEventArgs e) {
			ctrlHierarchicalPostList.UpdatePostButtonVisibility = PostAdministrationVisibility.ShowOnAllPosts;
			ctrlUpdatePost.Visible = false;
		}
		private void ctrlUpdatePost_PostUpdateDiscarded(object sender, EventArgs e) {
			ctrlHierarchicalPostList.UpdatePostButtonVisibility = PostAdministrationVisibility.ShowOnAllPosts;
			ctrlUpdatePost.Visible = false;
		}
		private void ctrlUpdatePost_PostUpdateFailed(object sender, PostFailureEventArgs e) {
			ctrlHierarchicalPostList.UpdatePostButtonVisibility = PostAdministrationVisibility.DoNotShowOnAnyPosts;
		}
		private static void ctrlUpdatePost_PostUpdating(object sender, PostCancelEventArgs e) {
			//e.Cancel = true;
		}
		private void ctrlCategorySelector_CategorySelected(object sender, EventArgs e) {
			ctrlFlatTabularPostList.CategoryId = ctrlCategorySelector.SelectedCategoryId;
		}
		#endregion

		#region Event Activators
		protected override void OnLoad(EventArgs e) {
			ctrlAddPost.UseTitleFromReplySource = true;
			//ctrlUpdatePost.AuthorTextBoxBeforeTitleAndBodyTextBoxes =
			//    ctrlAddPost.AuthorTextBoxBeforeTitleAndBodyTextBoxes = false;
			//ctrlUpdatePost.SubmitButtonBeforeCancelButton =
			//    ctrlAddPost.SubmitButtonBeforeCancelButton = false;
			if(Request.QueryString["id"] != null) {
				InactivateFlatTabularPostList();
				SetHierarchicalPostListProperties();
			} else {
				InactivateHierarchicalPostList();
				SetFlatTabularPostListProperties();
			}
			if(!this.Trace.IsEnabled) {
				var context = new HttpContextWrapper(HttpContext.Current);
				var responseHandler = new ResponseHandler(new MimeTypeHandler(context), context);
				responseHandler.PerformXhtmlContentNegotiation();
			}
			this.Response.ContentEncoding = Encoding.UTF8;
			base.OnLoad(e);
		}
		protected override void OnInit(EventArgs e) {
			ctrlHierarchicalPostList.AddPost += this.AddPost;
			ctrlHierarchicalPostList.UpdatePost += this.UpdatePost;
			ctrlHierarchicalPostList.PostVisibleChanging += ctrlHierarchicalPostList_PostVisibleChanging;
			ctrlHierarchicalPostList.PostVisibleChanged += ctrlHierarchicalPostList_PostVisibleChanged;
			ctrlHierarchicalPostList.PostVisibleChangeFailed += ctrlHierarchicalPostList_PostVisibleChangeFailed;
			ctrlHierarchicalPostList.PostDeleting += ctrlHierarchicalPostList_PostDeleting;
			ctrlHierarchicalPostList.PostDeleted += ctrlHierarchicalPostList_PostDeleted;
			ctrlFlatTabularPostList.AddPost += this.AddPost;
			ctrlFlatTabularPostList.PostsSorting += ctrlFlatTabularPostList_PostsSorting;
			ctrlAddPost.PostAdditionFailed += this.ctrlAddPost_PostAdditionFailed;
			ctrlAddPost.PostAdded += this.ctrlAddPost_PostAdded;
			ctrlAddPost.PostAdditionDiscarded += this.ctrlAddPost_PostAdditionDiscarded;
			ctrlAddPost.PostAdding += ctrlAddPost_PostAdding;
			ctrlUpdatePost.PostUpdated += this.ctrlUpdatePost_PostUpdated;
			ctrlUpdatePost.PostUpdateDiscarded += this.ctrlUpdatePost_PostUpdateDiscarded;
			ctrlUpdatePost.PostUpdateFailed += this.ctrlUpdatePost_PostUpdateFailed;
			ctrlUpdatePost.PostUpdating += ctrlUpdatePost_PostUpdating;
			ctrlCategorySelector.CategorySelected += this.ctrlCategorySelector_CategorySelected;
			base.OnInit(e);
		}
		#endregion

		#region Helper Methods
		private void InactivateHierarchicalPostList() {
			ctrlHierarchicalPostList.Visible =
				ctrlHierarchicalPostList.EnableViewState = false;
		}
		private void SetFlatTabularPostListProperties() {
			if(!this.IsPostBack) {
				if(Request.QueryString["domain"] != null) {
					ctrlCategorySelector.Domain = Request.QueryString["domain"];
				}
				if(Request.QueryString["categoryId"] != null) {
					ctrlCategorySelector.SelectedCategoryId = Request.QueryString["categoryId"].Length.Equals(0) ? null : CategoryId.Parse(Request.QueryString["categoryId"]);
				}
			}
			ctrlFlatTabularPostList.Visible = true;
			if(Request.QueryString["search"] != null) {
				ctrlFlatTabularPostList.SearchText = Request.QueryString["search"];
			}
			if(Request.QueryString["domain"] != null) {
				ctrlFlatTabularPostList.Domain = Request.QueryString["domain"];
			}
			if(Request.QueryString["categoryName"] != null) {
				ctrlFlatTabularPostList.CategoryName = Request.QueryString["categoryName"];
			}
			if(Request.QueryString["categoryId"] != null) {
				ctrlFlatTabularPostList.CategoryId = Request.QueryString["categoryId"].Length.Equals(0) ? null : CategoryId.Parse(Request.QueryString["categoryId"]);
			}
			ctrlFlatTabularPostList.IncludeHidden = true;
			ctrlFlatTabularPostList.UrlWithoutTrailingPostId = "/ForumTest.aspx?id=";
			ctrlFlatTabularPostList.UrlWithoutTrailingRootPostId = "/ForumTest.aspx?id=";
			ctrlCategorySelector.RedirectUrlWithoutTrailingCategoryId =
			ctrlFlatTabularPostList.UrlWithoutTrailingCategoryId = "/ForumTest.aspx?categoryId=";
			if(!Page.IsPostBack) {
				ctrlFlatTabularPostList.SortAscending = false;
			}
			//ctrlFlatTabularPostList.TableColumnEffectivelyVisibleVisible = true;
			//ctrlFlatTabularPostList.TableColumnVisibleVisible	= true;
			//StringAttribute ipAddress = new StringAttribute("IPAddress");
			//ipAddress.Value = "hej_hopp";
			//ctrlFlatTabularPostList.AttributeCriteria.Add(new AttributeCriterion(ipAddress, Comparison.Equal, true));
		}
		private void InactivateFlatTabularPostList() {
			ctrlCategorySelector.Visible =
				ctrlCategorySelector.EnableViewState =
				ctrlFlatTabularPostList.Visible =
				ctrlFlatTabularPostList.EnableViewState = false;
		}
		private void SetHierarchicalPostListProperties() {
			PostId postId = PostId.Parse(Request.QueryString["id"]);
			ctrlHierarchicalPostList.PostId = postId;
			ctrlHierarchicalPostList.IncludeHidden = true;
			ctrlHierarchicalPostList.SortAscending = true;
			ctrlHierarchicalPostList.UserIdentity = WindowsIdentity.GetCurrent().Name;
			ctrlHierarchicalPostList.AddPostButtonVisibility = HierarchicalPostListVisibility.DoNotShowOnAnyPosts;
			ctrlHierarchicalPostList.DescendantPostsLabelVisibility = HierarchicalPostListVisibility.DoNotShowOnAnyPosts;
			ctrlHierarchicalPostList.DescendantPostsInformationVisibility = HierarchicalPostListVisibility.ShowOnRootPostsOnly;
			ctrlHierarchicalPostList.UpdatePostButtonVisibility =
				ctrlHierarchicalPostList.ChangePostVisibleButtonVisibility =
				ctrlHierarchicalPostList.DeletePostButtonVisibility = PostAdministrationVisibility.DoNotShowOnAnyPosts;
			ctrlHierarchicalPostList.AddPostButtonPosition =
				ctrlHierarchicalPostList.UpdatePostButtonPosition =
				ctrlHierarchicalPostList.DeletePostButtonPosition =
				ctrlHierarchicalPostList.ChangePostVisibleButtonPosition =
				ctrlHierarchicalPostList.PostInformationPosition = Position.Below;
			ctrlHierarchicalPostList.AddPostReplySourceLocation = PostLocation.Current;
			ctrlHierarchicalPostList.AddPostTargetLocation = PostLocation.Root;
			//ctrlHierarchicalPostList.AddPostButtonTargetPostType				= TargetPostType.Specified;
			//ctrlHierarchicalPostList.AddPostButtonTargetPostId				= PostId.Parse("faf27fb39fb44da691413e2db60a3d62");
			//ctrlHierarchicalPostList.CategoryName								= "Til fædre";
			//ctrlHierarchicalPostList.Domain									= "da-DK";
			//ctrlHierarchicalPostList.Levels									= 1;
			//ctrlHierarchicalPostList.TitleVisibility							= HierarchicalPostListVisibility.ShowOnRootPostsOnly;
			//ctrlHierarchicalPostList.PostType									= PostType.RootPost;
			//ctrlHierarchicalPostList.DateFilterFrom								= new DateTime(2006, 9, 25);
			//ctrlHierarchicalPostList.DateFilterTo								= new DateTime(2006, 9, 26);
		}
		#endregion
	}

}
#endif