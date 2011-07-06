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
using System.IO;
using System.Web;
using System.Web.UI;
using System.ComponentModel;
using System.Globalization;

using nJupiter.Configuration;
using nJupiter.Web.UI.Controls;
using nJupiter.Web.UI.Controls.Listings;

namespace nJupiter.Services.Forum.UI.Web {

	public class HierarchicalPostListItem : ListItemBase {
		#region Constants
		private const string SettingPostlisttemplate = "hierarchicalPostListTemplate";
		private const string DefaultPostlisttemplate = "HierarchicalPostList.ascx";
		private const string CssclassOdd = "odd";
		private const string CssclassEven = "even";
		private const string CssclassHidden = "hidden";
		private const string CssclassHiddenbyhidden = "hidden-by-hidden";
		#endregion

		#region Variables
		private Post currentPost;
		private HierarchicalPostList hierarchicalPostList;

		private HierarchicalPostListVisibility messageNoDescendantPostsVisibility;

		private static readonly object EventAddPost = new object();
		private static readonly object EventUpdatePost = new object();
		private static readonly object EventChangePostVisible = new object();
		private static readonly object EventDeletePost = new object();
		private static readonly object EventPostVisibleChanging = new object();
		private static readonly object EventPostVisibleChanged = new object();
		private static readonly object EventPostVisibleChangeFailed = new object();
		private static readonly object EventPostDeleting = new object();
		private static readonly object EventPostDeleted = new object();
		#endregion

		#region Events
		public event AddPostEventHandler AddPost {
			add { base.Events.AddHandler(EventAddPost, value); }
			remove { base.Events.RemoveHandler(EventAddPost, value); }
		}
		public event UpdatePostEventHandler UpdatePost {
			add { base.Events.AddHandler(EventUpdatePost, value); }
			remove { base.Events.RemoveHandler(EventUpdatePost, value); }
		}
		public event ChangePostVisibleEventHandler ChangePostVisible {
			add { base.Events.AddHandler(EventChangePostVisible, value); }
			remove { base.Events.RemoveHandler(EventChangePostVisible, value); }
		}
		public event DeletePostEventHandler DeletePost {
			add { base.Events.AddHandler(EventDeletePost, value); }
			remove { base.Events.RemoveHandler(EventDeletePost, value); }
		}
		public event PostVisibleChangingEventHandler PostVisibleChanging {
			add { base.Events.AddHandler(EventPostVisibleChanging, value); }
			remove { base.Events.RemoveHandler(EventPostVisibleChanging, value); }
		}
		public event PostVisibleChangedEventHandler PostVisibleChanged {
			add { base.Events.AddHandler(EventPostVisibleChanged, value); }
			remove { base.Events.RemoveHandler(EventPostVisibleChanged, value); }
		}
		public event PostVisibleChangeFailedEventHandler PostVisibleChangeFailed {
			add { base.Events.AddHandler(EventPostVisibleChangeFailed, value); }
			remove { base.Events.RemoveHandler(EventPostVisibleChangeFailed, value); }
		}
		public event PostDeletingEventHandler PostDeleting {
			add { base.Events.AddHandler(EventPostDeleting, value); }
			remove { base.Events.RemoveHandler(EventPostDeleting, value); }
		}
		public event PostDeletedEventHandler PostDeleted {
			add { base.Events.AddHandler(EventPostDeleted, value); }
			remove { base.Events.RemoveHandler(EventPostDeleted, value); }
		}
		#endregion

		#region UI Members
		protected WebGenericControl ctrlPostDivision;
		protected WebButton btnHeaderAddPost;
		protected WebButton btnHeaderUpdatePost;
		protected WebButton btnHeaderChangePostVisible;
		protected WebButton btnHeaderDeletePost;
		protected WebGenericControl ctrlHeaderPostInformation;
		protected WebGenericControl ctrlHeaderAuthorLabel;
		protected WebAnchor ctrlHeaderAuthorAnchor;
		protected WebGenericControl ctrlHeaderTimePostedLabel;
		protected WebGenericControl ctrlHeaderTimePosted;
		protected WebPlaceHolder ctrlHeaderDescendantPostsInformation;
		protected WebGenericControl ctrlHeaderPostCountLabel;
		protected WebGenericControl ctrlHeaderPostCount;
		protected WebGenericControl ctrlHeaderTimeLastPostLabel;
		protected WebGenericControl ctrlHeaderTimeLastPost;
		protected WebHeading ctrlTitle;
		protected WebParagraph ctrlBody;
		protected WebGenericControl ctrlFooterPostInformation;
		protected WebGenericControl ctrlFooterAuthorLabel;
		protected WebAnchor ctrlFooterAuthorAnchor;
		protected WebGenericControl ctrlFooterTimePostedLabel;
		protected WebGenericControl ctrlFooterTimePosted;
		protected WebPlaceHolder ctrlFooterDescendantPostsInformation;
		protected WebGenericControl ctrlFooterPostCountLabel;
		protected WebGenericControl ctrlFooterPostCount;
		protected WebGenericControl ctrlFooterTimeLastPostLabel;
		protected WebGenericControl ctrlFooterTimeLastPost;
		protected WebButton btnFooterAddPost;
		protected WebButton btnFooterUpdatePost;
		protected WebButton btnFooterChangePostVisible;
		protected WebButton btnFooterDeletePost;
		protected WebGenericControl ctrlDescendantPostsDivision;
		protected WebGenericControl ctrlDescendantsPostsLabel;
		protected WebParagraph ctrlMessageNoDescendantPosts;
		protected WebPlaceHolder ctrlDescendantPosts;
		#endregion

		#region Properties
		protected Post CurrentPost { get { return this.currentPost ?? (this.currentPost = (Post)this.ListItem); } }
		protected HierarchicalPostList HierarchicalPostList {
			get {
				if(this.hierarchicalPostList == null &&
					this.CurrentPost.Posts != null &&
					!this.CurrentPost.Posts.Count.Equals(0)) {
					IConfig config = ConfigRepository.Instance.GetConfig(true);
					this.hierarchicalPostList = (HierarchicalPostList)this.LoadControl(config != null && config.ContainsKey(SettingPostlisttemplate) ? config.GetValue(SettingPostlisttemplate) : Path.Combine(this.TemplateSourceDirectory, DefaultPostlisttemplate));
				}
				return this.hierarchicalPostList;
			}
		}

		public PostId AddPostTargetPostId { get; set; }
		public PostId AddPostReplySourcePostId { get; set; }
		public PostLocation AddPostTargetLocation { get; set; }
		public PostLocation AddPostReplySourceLocation { get; set; }
		public AuthorResolver AuthorResolver { get; set; }
		public DateFormatter DateFormatter { get; set; }
		public string PostHyperlinkPrefix { get; set; }
		public TextFormatter TextFormatter { get; set; }
		public string UrlWithoutTrailingUserIdentity { get; set; }
		public string UserIdentity { get; set; }

		public HierarchicalPostListVisibility TitleVisibility { get; set; }
		public HierarchicalPostListVisibility DescendantPostsInformationVisibility { get; set; }
		public HierarchicalPostListVisibility DescendantPostsLabelVisibility { get; set; }
		public HierarchicalPostListVisibility MessageNoDescendantPostsVisibility {
			get { return this.messageNoDescendantPostsVisibility; }
			set {
				switch(value) {
					case HierarchicalPostListVisibility.ShowOnParentPostsOnly:
					case HierarchicalPostListVisibility.ShowOnRootAndParentPostsOnly:
					throw new InvalidEnumArgumentException("value", (int)value, typeof(HierarchicalPostListVisibility));
					default:
					this.messageNoDescendantPostsVisibility = value;
					break;
				}
			}
		}
		public HierarchicalPostListVisibility AddPostButtonVisibility { get; set; }
		public PostAdministrationVisibility UpdatePostButtonVisibility { get; set; }
		public PostAdministrationVisibility ChangePostVisibleButtonVisibility { get; set; }
		public PostAdministrationVisibility DeletePostButtonVisibility { get; set; }
		public Position AddPostButtonPosition { get; set; }
		public Position UpdatePostButtonPosition { get; set; }
		public Position ChangePostVisibleButtonPosition { get; set; }
		public Position DeletePostButtonPosition { get; set; }
		public Position PostInformationPosition { get; set; }

		public string AddPostButtonText { get { return btnHeaderAddPost.InnerText; } set { btnHeaderAddPost.InnerText = btnFooterAddPost.InnerText = value; } }
		public string UpdatePostButtonText { get { return btnHeaderUpdatePost.InnerText; } set { btnHeaderUpdatePost.InnerText = btnFooterUpdatePost.InnerText = value; } }
		public string ChangePostVisibleButtonHideText { get; set; }
		public string ChangePostVisibleButtonUnhideText { get; set; }
		public string DeletePostButtonText { get { return btnHeaderDeletePost.InnerText; } set { btnHeaderDeletePost.InnerText = btnFooterDeletePost.InnerText = value; } }
		public string AuthorLabelText { get { return ctrlHeaderAuthorLabel.InnerText; } set { ctrlHeaderAuthorLabel.InnerText = ctrlFooterAuthorLabel.InnerText = value; } }
		public string TimePostedLabelText { get { return ctrlHeaderTimePostedLabel.InnerText; } set { ctrlHeaderTimePostedLabel.InnerText = ctrlFooterTimePostedLabel.InnerText = value; } }
		public string PostCountLabelText { get { return ctrlHeaderPostCountLabel.InnerText; } set { ctrlHeaderPostCountLabel.InnerText = ctrlFooterPostCountLabel.InnerText = value; } }
		public string TimeLastPostLabelText { get { return ctrlHeaderTimeLastPostLabel.InnerText; } set { ctrlHeaderTimeLastPostLabel.InnerText = ctrlFooterTimeLastPostLabel.InnerText = value; } }
		public string DescendantPostsLabelText { get { return ctrlDescendantsPostsLabel.InnerText; } set { ctrlDescendantsPostsLabel.InnerText = value; } }
		public string MessageNoDescendantPostsText { get { return ctrlMessageNoDescendantPosts.InnerText; } set { ctrlMessageNoDescendantPosts.InnerText = value; } }
		#endregion

		#region Event Handlers
		private void AddPostClick(object sender, EventArgs e) {
			OnAddPost(new AddPostEventArgs(
				this.AddPostTargetLocation.Equals(PostLocation.Specified) && this.AddPostTargetPostId != null ? this.AddPostTargetPostId : this.CurrentPost.Id,
				this.AddPostReplySourceLocation.Equals(PostLocation.Specified) && this.AddPostReplySourcePostId != null ? this.AddPostReplySourcePostId : this.CurrentPost.Id));
		}
		private void UpdatePostClick(object sender, EventArgs e) {
			OnUpdatePost(new UpdatePostEventArgs(this.CurrentPost.Id));
		}
		private void ChangePostVisibleClick(object sender, EventArgs e) {
			OnChangePostVisible(new PostEventArgs(this.CurrentPost));
		}
		private void DeletePostClick(object sender, EventArgs e) {
			OnDeletePost(new PostEventArgs(this.CurrentPost));
		}
		private void HierarchicalPostListAddPost(object sender, AddPostEventArgs e) {
			OnAddPost(e);
		}
		private void HierarchicalPostListUpdatePost(object sender, UpdatePostEventArgs e) {
			OnUpdatePost(e);
		}
		private void HierarchicalPostListPostVisibleChanging(object sender, PostCancelEventArgs e) {
			OnPostVisibleChanging(e);
		}
		private void HierarchicalPostListPostVisibleChanged(object sender, PostEventArgs e) {
			OnPostVisibleChanged(e);
		}
		private void HierarchicalPostListPostVisibleChangeFailed(object sender, PostFailureEventArgs e) {
			OnPostVisibleChangeFailed(e);
		}
		private void HierarchicalPostListPostDeleting(object sender, PostCancelEventArgs e) {
			OnPostDeleting(e);
		}
		private void HierarchicalPostListPostDeleted(object sender, PostDeletedEventArgs e) {
			OnPostDeleted(e);
		}
		#endregion

		#region Event Activators
		protected virtual void OnAddPost(AddPostEventArgs e) {
			AddPostEventHandler eventHandler = base.Events[EventAddPost] as AddPostEventHandler;
			if(eventHandler != null) {
				eventHandler(this, e);
			}
		}
		protected virtual void OnUpdatePost(UpdatePostEventArgs e) {
			UpdatePostEventHandler eventHandler = base.Events[EventUpdatePost] as UpdatePostEventHandler;
			if(eventHandler != null) {
				eventHandler(this, e);
			}
		}
		protected virtual void OnChangePostVisible(PostEventArgs e) {
			ChangePostVisibleEventHandler eventHandler = base.Events[EventChangePostVisible] as ChangePostVisibleEventHandler;
			if(eventHandler != null) {
				eventHandler(this, e);
			}
		}
		protected virtual void OnDeletePost(PostEventArgs e) {
			DeletePostEventHandler eventHandler = base.Events[EventDeletePost] as DeletePostEventHandler;
			if(eventHandler != null) {
				eventHandler(this, e);
			}
		}
		protected virtual void OnPostVisibleChanging(PostCancelEventArgs e) {
			PostVisibleChangingEventHandler eventHandler = base.Events[EventPostVisibleChanging] as PostVisibleChangingEventHandler;
			if(eventHandler != null) {
				eventHandler(this, e);
			}
		}
		protected virtual void OnPostVisibleChanged(PostEventArgs e) {
			PostVisibleChangedEventHandler eventHandler = base.Events[EventPostVisibleChanged] as PostVisibleChangedEventHandler;
			if(eventHandler != null) {
				eventHandler(this, e);
			}
		}
		protected virtual void OnPostVisibleChangeFailed(PostFailureEventArgs e) {
			PostVisibleChangeFailedEventHandler eventHandler = base.Events[EventPostVisibleChangeFailed] as PostVisibleChangeFailedEventHandler;
			if(eventHandler != null) {
				eventHandler(this, e);
			}
		}
		protected virtual void OnPostDeleting(PostCancelEventArgs e) {
			PostDeletingEventHandler eventHandler = base.Events[EventPostDeleting] as PostDeletingEventHandler;
			if(eventHandler != null) {
				eventHandler(this, e);
			}
		}
		protected virtual void OnPostDeleted(PostDeletedEventArgs e) {
			PostDeletedEventHandler eventHandler = base.Events[EventPostDeleted] as PostDeletedEventHandler;
			if(eventHandler != null) {
				eventHandler(this, e);
			}
		}
		#endregion

		#region Overridden Methods
		protected override void OnInit(EventArgs e) {
			EventHandler addPostEventHandler = this.AddPostClick;
			EventHandler updatePostEventHandler = this.UpdatePostClick;
			EventHandler changePostVisibleEventHandler = this.ChangePostVisibleClick;
			EventHandler deletePostEventHandler = this.DeletePostClick;
			btnHeaderAddPost.Click += addPostEventHandler;
			btnFooterAddPost.Click += addPostEventHandler;
			btnHeaderUpdatePost.Click += updatePostEventHandler;
			btnFooterUpdatePost.Click += updatePostEventHandler;
			btnHeaderChangePostVisible.Click += changePostVisibleEventHandler;
			btnFooterChangePostVisible.Click += changePostVisibleEventHandler;
			btnHeaderDeletePost.Click += deletePostEventHandler;
			btnFooterDeletePost.Click += deletePostEventHandler;
			base.OnInit(e);
		}
		public override void DataBind() {
			bool descendantsNull = this.CurrentPost.Posts == null;
			bool hasDescendants = !descendantsNull && !this.CurrentPost.Posts.Count.Equals(0);
			string author;
			ctrlHeaderAuthorAnchor.Text =
				ctrlFooterAuthorAnchor.Text = this.CurrentPost.UserIdentity != null && this.AuthorResolver != null && (author = this.AuthorResolver.GetAuthor(this.CurrentPost.UserIdentity)) != null ? author : this.CurrentPost.Author;
			if(!(ctrlHeaderAuthorAnchor.NoLink = ctrlFooterAuthorAnchor.NoLink = this.UrlWithoutTrailingUserIdentity == null || this.CurrentPost.UserIdentity == null)) {
				ctrlHeaderAuthorAnchor.NavigateUrl =
					ctrlFooterAuthorAnchor.NavigateUrl = this.UrlWithoutTrailingUserIdentity + HttpUtility.UrlEncode(this.CurrentPost.UserIdentity);
			}
			ctrlHeaderTimePosted.InnerText =
				ctrlFooterTimePosted.InnerText = this.DateFormatter.FormatDate(this.CurrentPost.TimePosted);
			ctrlTitle.InnerHtml = this.TextFormatter.FormatText(this.CurrentPost.Title);
			ctrlBody.InnerHtml = this.TextFormatter.FormatText(this.CurrentPost.Body);
			SetVisibleFromVisibleAndPosition(true, this.PostInformationPosition, ctrlHeaderPostInformation, ctrlFooterPostInformation);
			bool realDescendantPostsInformationVisible = IsVisible(this.DescendantPostsInformationVisibility);
			ctrlTitle.Visible = IsVisible(this.TitleVisibility);
			ctrlHeaderDescendantPostsInformation.Visible = realDescendantPostsInformationVisible;
			ctrlFooterDescendantPostsInformation.Visible = realDescendantPostsInformationVisible;
			if(realDescendantPostsInformationVisible) {
				ctrlHeaderPostCount.InnerText =
					ctrlFooterPostCount.InnerText = this.CurrentPost.PostCount.ToString(NumberFormatInfo.CurrentInfo);
				ctrlHeaderTimeLastPost.InnerText =
					ctrlFooterTimeLastPost.InnerText = this.DateFormatter.FormatDate(this.CurrentPost.TimeLastPost);
			}
			if(descendantsNull) { //do not show if we are not interested in descendants
				ctrlDescendantPostsDivision.Visible = false;
			} else {
				bool realDescendantPostLabelVisible = IsVisible(this.DescendantPostsLabelVisibility);
				bool realMessageNoDescendantsPostsVisible = !hasDescendants && !this.MessageNoDescendantPostsVisibility.Equals(HierarchicalPostListVisibility.DoNotShowOnAnyPosts);
				ctrlDescendantPostsDivision.Visible = hasDescendants || realDescendantPostLabelVisible || realMessageNoDescendantsPostsVisible;
				ctrlDescendantsPostsLabel.Visible = realDescendantPostLabelVisible;
				ctrlMessageNoDescendantPosts.Visible = realMessageNoDescendantsPostsVisible;
			}
			SetVisibleFromVisibleAndPosition(IsVisible(this.AddPostButtonVisibility), this.AddPostButtonPosition, btnHeaderAddPost, btnFooterAddPost);
			SetVisibleFromVisibleAndPosition(IsVisible(this.DeletePostButtonVisibility), this.DeletePostButtonPosition, btnHeaderDeletePost, btnFooterDeletePost);
			SetVisibleFromVisibleAndPosition(IsVisible(this.UpdatePostButtonVisibility), this.UpdatePostButtonPosition, btnHeaderUpdatePost, btnFooterUpdatePost);
			bool realChangePostVisibleButtonVisible = IsVisible(this.ChangePostVisibleButtonVisibility);
			if(realChangePostVisibleButtonVisible) {
				btnHeaderChangePostVisible.InnerText = btnFooterChangePostVisible.InnerText = this.CurrentPost.Visible ? this.ChangePostVisibleButtonHideText : this.ChangePostVisibleButtonUnhideText;
			}
			SetVisibleFromVisibleAndPosition(realChangePostVisibleButtonVisible, this.ChangePostVisibleButtonPosition, btnHeaderChangePostVisible, btnFooterChangePostVisible);
			if(this.HierarchicalPostList != null) {
				this.HierarchicalPostList.PostCollection = this.CurrentPost.Posts;
				this.HierarchicalPostList.AddPostReplySourcePostId = GetLocationPostId(this.AddPostReplySourceLocation, this.AddPostReplySourcePostId);
				this.HierarchicalPostList.AddPostReplySourceLocation = PostLocation.Specified;
				this.HierarchicalPostList.AddPostTargetPostId = GetLocationPostId(this.AddPostTargetLocation, this.AddPostTargetPostId);
				this.HierarchicalPostList.AddPostTargetLocation = PostLocation.Specified;
				this.HierarchicalPostList.AuthorResolver = this.AuthorResolver;
				this.HierarchicalPostList.DateFormatter = this.DateFormatter;
				this.HierarchicalPostList.PostHyperlinkPrefix = this.PostHyperlinkPrefix;
				this.HierarchicalPostList.TextFormatter = this.TextFormatter;
				this.HierarchicalPostList.UserIdentity = this.UserIdentity;
				this.HierarchicalPostList.AddPostButtonVisibility = GetDescendantVisibility(this.AddPostButtonVisibility);
				this.HierarchicalPostList.UpdatePostButtonVisibility = this.UpdatePostButtonVisibility;
				this.HierarchicalPostList.ChangePostVisibleButtonVisibility = this.ChangePostVisibleButtonVisibility;
				this.HierarchicalPostList.DeletePostButtonVisibility = this.DeletePostButtonVisibility;
				this.HierarchicalPostList.TitleVisibility = GetDescendantVisibility(this.TitleVisibility);
				this.HierarchicalPostList.DescendantPostsLabelVisibility = GetDescendantVisibility(this.DescendantPostsLabelVisibility);
				this.HierarchicalPostList.DescendantPostsInformationVisibility = GetDescendantVisibility(this.DescendantPostsInformationVisibility);
				this.HierarchicalPostList.MessageNoDescendantPostsVisibility = GetDescendantVisibility(this.MessageNoDescendantPostsVisibility);
				this.HierarchicalPostList.AddPostButtonPosition = this.AddPostButtonPosition;
				this.HierarchicalPostList.UpdatePostButtonPosition = this.UpdatePostButtonPosition;
				this.HierarchicalPostList.ChangePostVisibleButtonPosition = this.ChangePostVisibleButtonPosition;
				this.HierarchicalPostList.DeletePostButtonPosition = this.DeletePostButtonPosition;
				this.HierarchicalPostList.PostInformationPosition = this.PostInformationPosition;
				this.HierarchicalPostList.AddPostButtonText = this.AddPostButtonText;
				this.HierarchicalPostList.UpdatePostButtonText = this.UpdatePostButtonText;
				this.HierarchicalPostList.ChangePostVisibleButtonHideText = this.ChangePostVisibleButtonHideText;
				this.HierarchicalPostList.ChangePostVisibleButtonUnhideText = this.ChangePostVisibleButtonUnhideText;
				this.HierarchicalPostList.DeletePostButtonText = this.DeletePostButtonText;
				this.HierarchicalPostList.AuthorLabelText = this.AuthorLabelText;
				this.HierarchicalPostList.TimePostedLabelText = this.TimePostedLabelText;
				this.HierarchicalPostList.PostCountLabelText = this.PostCountLabelText;
				this.HierarchicalPostList.TimeLastPostLabelText = this.TimeLastPostLabelText;
				this.HierarchicalPostList.DescendantPostsLabelText = this.DescendantPostsLabelText;
				this.HierarchicalPostList.MessageNoDescendantPostsText = this.MessageNoDescendantPostsText;
				ctrlDescendantPosts.Controls.Add(this.HierarchicalPostList);
				this.HierarchicalPostList.EnableViewState = false;
				this.HierarchicalPostList.AddPost += this.HierarchicalPostListAddPost;
				this.HierarchicalPostList.UpdatePost += this.HierarchicalPostListUpdatePost;
				this.HierarchicalPostList.PostVisibleChanging += this.HierarchicalPostListPostVisibleChanging;
				this.HierarchicalPostList.PostVisibleChanged += this.HierarchicalPostListPostVisibleChanged;
				this.HierarchicalPostList.PostVisibleChangeFailed += this.HierarchicalPostListPostVisibleChangeFailed;
				this.HierarchicalPostList.PostDeleting += this.HierarchicalPostListPostDeleting;
				this.HierarchicalPostList.PostDeleted += this.HierarchicalPostListPostDeleted;
				this.HierarchicalPostList.DataBind();
			}
			AddCssClass(ctrlPostDivision, this.AlternatingItem ? CssclassEven : CssclassOdd);
			if(!this.CurrentPost.EffectivelyVisible) {
				AddCssClass(ctrlPostDivision, this.CurrentPost.Visible ? CssclassHiddenbyhidden : CssclassHidden);
			}
			ctrlPostDivision.ID = this.PostHyperlinkPrefix + this.CurrentPost.Id;
			ctrlPostDivision.RenderId = true;
		}
		#endregion

		#region Helper Methods
		private static void AddCssClass(WebGenericControl control, string cssClass) {
			const string SPACE = " ";
			if(control.CssClass.Length > 0) {
				control.CssClass += SPACE;
			}
			control.CssClass += cssClass;
		}
		private static HierarchicalPostListVisibility GetDescendantVisibility(HierarchicalPostListVisibility visibility) {
			switch(visibility) {
				case HierarchicalPostListVisibility.ShowOnAllPosts:
				case HierarchicalPostListVisibility.ShowOnParentPostsOnly:
				return visibility;
				case HierarchicalPostListVisibility.ShowOnRootAndParentPostsOnly:
				return HierarchicalPostListVisibility.ShowOnParentPostsOnly;
				default:
				return HierarchicalPostListVisibility.DoNotShowOnAnyPosts;
			}
		}
		private PostId GetLocationPostId(PostLocation locationPostType, PostId locationPostId) {
			switch(locationPostType) {
				case PostLocation.Specified:
				return locationPostId;
				case PostLocation.Root:
				return this.CurrentPost.Id;
				default:
				return null;
			}
		}
		private bool IsVisible(HierarchicalPostListVisibility visibility) {
			switch(visibility) {
				case HierarchicalPostListVisibility.ShowOnParentPostsOnly:
				return this.CurrentPost.Posts != null && !this.CurrentPost.Posts.Count.Equals(0);
				case HierarchicalPostListVisibility.DoNotShowOnAnyPosts:
				return false;
				default:
				return true;
			}
		}
		private bool IsVisible(PostAdministrationVisibility visibility) {
			switch(visibility) {
				case PostAdministrationVisibility.ShowOnAllPosts:
				return true;
				case PostAdministrationVisibility.ShowOnPostsWithMatchingUserIdentity:
				return string.Compare(this.CurrentPost.UserIdentity, this.UserIdentity, true, CultureInfo.InvariantCulture).Equals(0);
				default:
				return false;
			}
		}
		private static void SetVisibleFromVisibleAndPosition(bool visible, Position position, Control headerControl, Control footerControl) {
			if(visible) {
				switch(position) {
					case Position.Above:
					footerControl.Visible = !(headerControl.Visible = true);
					break;
					case Position.Below:
					footerControl.Visible = !(headerControl.Visible = false);
					break;
					case Position.AboveAndBelow:
					footerControl.Visible = headerControl.Visible = true;
					break;
				}
			} else {
				footerControl.Visible = headerControl.Visible = false;
			}
		}
		#endregion
	}

}