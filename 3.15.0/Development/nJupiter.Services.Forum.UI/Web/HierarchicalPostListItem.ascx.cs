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
		private const string	SETTING_POSTLISTTEMPLATE		= "hierarchicalPostListTemplate";
		private const string	DEFAULT_POSTLISTTEMPLATE		= "HierarchicalPostList.ascx";
		private const string	CSSCLASS_ODD					= "odd";
		private const string	CSSCLASS_EVEN					= "even";
		private const string	CSSCLASS_HIDDEN					= "hidden";
		private const string	CSSCLASS_HIDDENBYHIDDEN			= "hidden-by-hidden";
		#endregion

		#region Variables
		private Post							m_CurrentPost;
		private HierarchicalPostList			m_HierarchicalPostList;

		private PostId							m_AddPostReplySourcePostId;
		private PostId							m_AddPostTargetPostId;
		private PostLocation					m_AddPostReplySourceLocation;
		private PostLocation					m_AddPostTargetLocation;
		private AuthorResolver					m_AuthorResolver;
		private DateFormatter					m_DateFormatter;
		private string							m_PostHyperlinkPrefix;
		private TextFormatter					m_TextFormatter;
		private string							m_UrlWithoutTrailingUserIdentity;
		private string							m_UserIdentity;

		private HierarchicalPostListVisibility	m_TitleVisibility;
		private HierarchicalPostListVisibility	m_DescendantPostsInformationVisibility;
		private HierarchicalPostListVisibility	m_DescendantPostsLabelVisibility;
		private HierarchicalPostListVisibility	m_MessageNoDescendantPostsVisibility;
		private HierarchicalPostListVisibility	m_AddPostButtonVisibility;
		private PostAdministrationVisibility	m_UpdatePostButtonVisibility;
		private PostAdministrationVisibility	m_ChangePostVisibleButtonVisibility;
		private PostAdministrationVisibility	m_DeletePostButtonVisibility;
		private Position						m_AddPostButtonPosition;
		private Position						m_UpdatePostButtonPosition;
		private Position						m_ChangePostVisibleButtonPosition;
		private Position						m_DeletePostButtonPosition;
		private Position						m_PostInformationPosition;

		private string							m_ChangePostVisibleButtonHideText;
		private string							m_ChangePostVisibleButtonUnhideText;

		private static readonly object			s_EventAddPost							= new object();
		private static readonly object			s_EventUpdatePost						= new object();
		private static readonly object			s_EventChangePostVisible				= new object();
		private static readonly object			s_EventDeletePost						= new object();
		private static readonly object			s_EventPostVisibleChanging				= new object();
		private static readonly object			s_EventPostVisibleChanged				= new object();
		private static readonly object			s_EventPostVisibleChangeFailed			= new object();
		private static readonly object			s_EventPostDeleting						= new object();
		private static readonly object			s_EventPostDeleted						= new object();
		#endregion

		#region Events
		public event AddPostEventHandler AddPost { 
			add { base.Events.AddHandler(s_EventAddPost, value); } 
			remove { base.Events.RemoveHandler(s_EventAddPost, value); } 
		}
		public event UpdatePostEventHandler UpdatePost { 
			add { base.Events.AddHandler(s_EventUpdatePost, value); } 
			remove { base.Events.RemoveHandler(s_EventUpdatePost, value); } 
		}
		public event ChangePostVisibleEventHandler ChangePostVisible { 
			add { base.Events.AddHandler(s_EventChangePostVisible, value); } 
			remove { base.Events.RemoveHandler(s_EventChangePostVisible, value); } 
		}
		public event DeletePostEventHandler DeletePost { 
			add { base.Events.AddHandler(s_EventDeletePost, value); } 
			remove { base.Events.RemoveHandler(s_EventDeletePost, value); } 
		}
		public event PostVisibleChangingEventHandler PostVisibleChanging { 
			add { base.Events.AddHandler(s_EventPostVisibleChanging, value); } 
			remove { base.Events.RemoveHandler(s_EventPostVisibleChanging, value); } 
		}
		public event PostVisibleChangedEventHandler PostVisibleChanged { 
			add { base.Events.AddHandler(s_EventPostVisibleChanged, value); } 
			remove { base.Events.RemoveHandler(s_EventPostVisibleChanged, value); } 
		}
		public event PostVisibleChangeFailedEventHandler PostVisibleChangeFailed { 
			add { base.Events.AddHandler(s_EventPostVisibleChangeFailed, value); } 
			remove { base.Events.RemoveHandler(s_EventPostVisibleChangeFailed, value); } 
		}
		public event PostDeletingEventHandler PostDeleting { 
			add { base.Events.AddHandler(s_EventPostDeleting, value); } 
			remove { base.Events.RemoveHandler(s_EventPostDeleting, value); } 
		}
		public event PostDeletedEventHandler PostDeleted { 
			add { base.Events.AddHandler(s_EventPostDeleted, value); } 
			remove { base.Events.RemoveHandler(s_EventPostDeleted, value); } 
		}
		#endregion

		#region UI Members
		protected WebGenericControl		ctrlPostDivision;
		protected WebButton				btnHeaderAddPost;
		protected WebButton				btnHeaderUpdatePost;
		protected WebButton				btnHeaderChangePostVisible;
		protected WebButton				btnHeaderDeletePost;
		protected WebGenericControl		ctrlHeaderPostInformation;
		protected WebGenericControl		ctrlHeaderAuthorLabel;
		protected WebAnchor				ctrlHeaderAuthorAnchor;
		protected WebGenericControl		ctrlHeaderTimePostedLabel;
		protected WebGenericControl		ctrlHeaderTimePosted;
		protected WebPlaceHolder		ctrlHeaderDescendantPostsInformation;
		protected WebGenericControl		ctrlHeaderPostCountLabel;
		protected WebGenericControl		ctrlHeaderPostCount;
		protected WebGenericControl		ctrlHeaderTimeLastPostLabel;
		protected WebGenericControl		ctrlHeaderTimeLastPost;
		protected WebHeading			ctrlTitle;
		protected WebParagraph			ctrlBody;
		protected WebGenericControl		ctrlFooterPostInformation;
		protected WebGenericControl		ctrlFooterAuthorLabel;
		protected WebAnchor				ctrlFooterAuthorAnchor;
		protected WebGenericControl		ctrlFooterTimePostedLabel;
		protected WebGenericControl		ctrlFooterTimePosted;
		protected WebPlaceHolder		ctrlFooterDescendantPostsInformation;
		protected WebGenericControl		ctrlFooterPostCountLabel;
		protected WebGenericControl		ctrlFooterPostCount;
		protected WebGenericControl		ctrlFooterTimeLastPostLabel;
		protected WebGenericControl		ctrlFooterTimeLastPost;
		protected WebButton				btnFooterAddPost;
		protected WebButton				btnFooterUpdatePost;
		protected WebButton				btnFooterChangePostVisible;
		protected WebButton				btnFooterDeletePost;
		protected WebGenericControl		ctrlDescendantPostsDivision;
		protected WebGenericControl		ctrlDescendantsPostsLabel;
		protected WebParagraph			ctrlMessageNoDescendantPosts;
		protected WebPlaceHolder		ctrlDescendantPosts;
		#endregion

		#region Properties
		protected Post CurrentPost { get { return this.m_CurrentPost ?? (this.m_CurrentPost = (Post)this.ListItem); } }
		protected HierarchicalPostList HierarchicalPostList {
			get {
				if(m_HierarchicalPostList == null &&
					this.CurrentPost.Posts != null &&
					!this.CurrentPost.Posts.Count.Equals(0)) {
					Config config			= ConfigHandler.GetConfig(true);
					m_HierarchicalPostList	= (HierarchicalPostList)this.LoadControl(config != null && config.ContainsKey(SETTING_POSTLISTTEMPLATE) ? config.GetValue(SETTING_POSTLISTTEMPLATE) : Path.Combine(this.TemplateSourceDirectory, DEFAULT_POSTLISTTEMPLATE));
				}
				return m_HierarchicalPostList;
			}
		}

		public PostId AddPostTargetPostId { get { return m_AddPostTargetPostId; } set { m_AddPostTargetPostId = value; } }
		public PostId AddPostReplySourcePostId { get { return m_AddPostReplySourcePostId; } set { m_AddPostReplySourcePostId = value; } }
		public PostLocation AddPostTargetLocation { get { return m_AddPostTargetLocation; } set { m_AddPostTargetLocation = value; } }
		public PostLocation AddPostReplySourceLocation { get { return m_AddPostReplySourceLocation; } set { m_AddPostReplySourceLocation = value; } }
		public AuthorResolver AuthorResolver { get { return m_AuthorResolver; } set { m_AuthorResolver = value; } }
		public DateFormatter DateFormatter { get { return m_DateFormatter; } set { m_DateFormatter = value; } }
		public string PostHyperlinkPrefix { get { return m_PostHyperlinkPrefix; } set { m_PostHyperlinkPrefix = value; } }
		public TextFormatter TextFormatter { get { return m_TextFormatter; } set { m_TextFormatter = value; } }
		public string UrlWithoutTrailingUserIdentity { get { return m_UrlWithoutTrailingUserIdentity; } set { m_UrlWithoutTrailingUserIdentity = value; } }
		public string UserIdentity { get { return m_UserIdentity; } set { m_UserIdentity = value; } }

		public HierarchicalPostListVisibility TitleVisibility { get { return m_TitleVisibility; } set { m_TitleVisibility = value; } }
		public HierarchicalPostListVisibility DescendantPostsInformationVisibility { get { return m_DescendantPostsInformationVisibility; } set { m_DescendantPostsInformationVisibility = value; } }
		public HierarchicalPostListVisibility DescendantPostsLabelVisibility { get { return m_DescendantPostsLabelVisibility; } set { m_DescendantPostsLabelVisibility = value; } }
		public HierarchicalPostListVisibility MessageNoDescendantPostsVisibility { 
			get { return m_MessageNoDescendantPostsVisibility; }
			set {
				switch(value) {
					case HierarchicalPostListVisibility.ShowOnParentPostsOnly:
					case HierarchicalPostListVisibility.ShowOnRootAndParentPostsOnly:
						throw new InvalidEnumArgumentException("value", (int)value, typeof(HierarchicalPostListVisibility));
					default:
						m_MessageNoDescendantPostsVisibility = value; 
						break;
				}
			} 
		}
		public HierarchicalPostListVisibility AddPostButtonVisibility { get { return m_AddPostButtonVisibility; } set { m_AddPostButtonVisibility = value; } }
		public PostAdministrationVisibility UpdatePostButtonVisibility { get { return m_UpdatePostButtonVisibility; } set { m_UpdatePostButtonVisibility = value; } }
		public PostAdministrationVisibility ChangePostVisibleButtonVisibility { get { return m_ChangePostVisibleButtonVisibility; } set { m_ChangePostVisibleButtonVisibility = value; } }
		public PostAdministrationVisibility DeletePostButtonVisibility { get { return m_DeletePostButtonVisibility; } set { m_DeletePostButtonVisibility = value; } }
		public Position AddPostButtonPosition { get { return m_AddPostButtonPosition; } set { m_AddPostButtonPosition = value; } }
		public Position UpdatePostButtonPosition { get { return m_UpdatePostButtonPosition; } set { m_UpdatePostButtonPosition = value; } }
		public Position ChangePostVisibleButtonPosition { get { return m_ChangePostVisibleButtonPosition; } set { m_ChangePostVisibleButtonPosition = value; } }
		public Position DeletePostButtonPosition { get { return m_DeletePostButtonPosition; } set { m_DeletePostButtonPosition = value; } }
		public Position PostInformationPosition { get { return m_PostInformationPosition; } set { m_PostInformationPosition = value; } }

		public string AddPostButtonText { get { return btnHeaderAddPost.InnerText; } set { btnHeaderAddPost.InnerText = btnFooterAddPost.InnerText = value; } }
		public string UpdatePostButtonText { get { return btnHeaderUpdatePost.InnerText; } set { btnHeaderUpdatePost.InnerText = btnFooterUpdatePost.InnerText = value; } }
		public string ChangePostVisibleButtonHideText { get { return m_ChangePostVisibleButtonHideText; } set { m_ChangePostVisibleButtonHideText = value; } }
		public string ChangePostVisibleButtonUnhideText { get { return m_ChangePostVisibleButtonUnhideText; } set { m_ChangePostVisibleButtonUnhideText = value; } }
		public string DeletePostButtonText { get { return btnHeaderDeletePost.InnerText; } set { btnHeaderDeletePost.InnerText = btnFooterDeletePost.InnerText = value; } }
		public string AuthorLabelText { get { return ctrlHeaderAuthorLabel.InnerText; } set { ctrlHeaderAuthorLabel.InnerText = ctrlFooterAuthorLabel.InnerText = value; } }
		public string TimePostedLabelText { get { return ctrlHeaderTimePostedLabel.InnerText; } set { ctrlHeaderTimePostedLabel.InnerText = ctrlFooterTimePostedLabel.InnerText = value; } }
		public string PostCountLabelText { get { return ctrlHeaderPostCountLabel.InnerText; } set { ctrlHeaderPostCountLabel.InnerText = ctrlFooterPostCountLabel.InnerText = value; } }
		public string TimeLastPostLabelText { get { return ctrlHeaderTimeLastPostLabel.InnerText; } set { ctrlHeaderTimeLastPostLabel.InnerText = ctrlFooterTimeLastPostLabel.InnerText = value; } }
		public string DescendantPostsLabelText { get { return ctrlDescendantsPostsLabel.InnerText; } set { ctrlDescendantsPostsLabel.InnerText = value; } }
		public string MessageNoDescendantPostsText { get { return ctrlMessageNoDescendantPosts.InnerText; } set { ctrlMessageNoDescendantPosts.InnerText = value; } }
		#endregion

		#region Event Handlers
		private void btnAddPost_Click(object sender, EventArgs e) {
			OnAddPost(new AddPostEventArgs(
				this.AddPostTargetLocation.Equals(PostLocation.Specified) && this.AddPostTargetPostId != null ? this.AddPostTargetPostId : this.CurrentPost.Id,
				this.AddPostReplySourceLocation.Equals(PostLocation.Specified) && this.AddPostReplySourcePostId != null ? this.AddPostReplySourcePostId : this.CurrentPost.Id));
		}
		private void btnUpdatePost_Click(object sender, EventArgs e) {
			OnUpdatePost(new UpdatePostEventArgs(this.CurrentPost.Id));
		}
		private void btnChangePostVisible_Click(object sender, EventArgs e) {
			OnChangePostVisible(new PostEventArgs(this.CurrentPost));
		}
		private void btnDeletePost_Click(object sender, EventArgs e) {
			OnDeletePost(new PostEventArgs(this.CurrentPost));
		}
		private void ctrlHierarchicalPostList_AddPost(object sender, AddPostEventArgs e) {
			OnAddPost(e);
		}
		private void ctrlHierarchicalPostList_UpdatePost(object sender, UpdatePostEventArgs e) {
			OnUpdatePost(e);
		}
		private void ctrlHierarchicalPostList_PostVisibleChanging(object sender, PostCancelEventArgs e) {
			OnPostVisibleChanging(e);
		}
		private void ctrlHierarchicalPostList_PostVisibleChanged(object sender, PostEventArgs e) {
			OnPostVisibleChanged(e);
		}
		private void ctrlHierarchicalPostList_PostVisibleChangeFailed(object sender, PostFailureEventArgs e) {
			OnPostVisibleChangeFailed(e);
		}
		private void ctrlHierarchicalPostList_PostDeleting(object sender, PostCancelEventArgs e) {
			OnPostDeleting(e);
		}
		private void ctrlHierarchicalPostList_PostDeleted(object sender, PostDeletedEventArgs e) {
			OnPostDeleted(e);
		}
		#endregion

		#region Event Activators
		protected virtual void OnAddPost(AddPostEventArgs e) {
			AddPostEventHandler eventHandler = base.Events[s_EventAddPost] as AddPostEventHandler;
			if(eventHandler != null) {
				eventHandler(this, e);
			}
		}
		protected virtual void OnUpdatePost(UpdatePostEventArgs e) {
			UpdatePostEventHandler eventHandler = base.Events[s_EventUpdatePost] as UpdatePostEventHandler;
			if(eventHandler != null) {
				eventHandler(this, e);
			}
		}
		protected virtual void OnChangePostVisible(PostEventArgs e) {
			ChangePostVisibleEventHandler eventHandler = base.Events[s_EventChangePostVisible] as ChangePostVisibleEventHandler;
			if(eventHandler != null) {
				eventHandler(this, e);
			}
		}
		protected virtual void OnDeletePost(PostEventArgs e) {
			DeletePostEventHandler eventHandler = base.Events[s_EventDeletePost] as DeletePostEventHandler;
			if(eventHandler != null) {
				eventHandler(this, e);
			}
		}
		protected virtual void OnPostVisibleChanging(PostCancelEventArgs e) {
			PostVisibleChangingEventHandler eventHandler = base.Events[s_EventPostVisibleChanging] as PostVisibleChangingEventHandler;
			if(eventHandler != null) {
				eventHandler(this, e);
			}
		}
		protected virtual void OnPostVisibleChanged(PostEventArgs e) {
			PostVisibleChangedEventHandler eventHandler = base.Events[s_EventPostVisibleChanged] as PostVisibleChangedEventHandler;
			if(eventHandler != null) {
				eventHandler(this, e);
			}
		}
		protected virtual void OnPostVisibleChangeFailed(PostFailureEventArgs e) {
			PostVisibleChangeFailedEventHandler eventHandler = base.Events[s_EventPostVisibleChangeFailed] as PostVisibleChangeFailedEventHandler;
			if(eventHandler != null) {
				eventHandler(this, e);
			}
		}
		protected virtual void OnPostDeleting(PostCancelEventArgs e) {
			PostDeletingEventHandler eventHandler = base.Events[s_EventPostDeleting] as PostDeletingEventHandler;
			if(eventHandler != null) {
				eventHandler(this, e);
			}
		}
		protected virtual void OnPostDeleted(PostDeletedEventArgs e) {
			PostDeletedEventHandler eventHandler = base.Events[s_EventPostDeleted] as PostDeletedEventHandler;
			if(eventHandler != null) {
				eventHandler(this, e);
			}
		}
		#endregion

		#region Overridden Methods
		protected override void OnInit(EventArgs e) {
			EventHandler addPostEventHandler			= this.btnAddPost_Click;
			EventHandler updatePostEventHandler			= this.btnUpdatePost_Click;
			EventHandler changePostVisibleEventHandler	= this.btnChangePostVisible_Click;
			EventHandler deletePostEventHandler			= this.btnDeletePost_Click;
			btnHeaderAddPost.Click						+= addPostEventHandler;
			btnFooterAddPost.Click						+= addPostEventHandler;
			btnHeaderUpdatePost.Click					+= updatePostEventHandler;
			btnFooterUpdatePost.Click					+= updatePostEventHandler;
			btnHeaderChangePostVisible.Click			+= changePostVisibleEventHandler;
			btnFooterChangePostVisible.Click			+= changePostVisibleEventHandler;
			btnHeaderDeletePost.Click					+= deletePostEventHandler;
			btnFooterDeletePost.Click					+= deletePostEventHandler;
			base.OnInit(e);
		}
		public override void DataBind() {
			bool descendantsNull							= this.CurrentPost.Posts == null;
			bool hasDescendants								= !descendantsNull && !this.CurrentPost.Posts.Count.Equals(0);
			string author;
			ctrlHeaderAuthorAnchor.Text =
				ctrlFooterAuthorAnchor.Text					= this.CurrentPost.UserIdentity != null && this.AuthorResolver != null && (author = this.AuthorResolver.GetAuthor(this.CurrentPost.UserIdentity)) != null ? author : this.CurrentPost.Author;
			if(!(ctrlHeaderAuthorAnchor.NoLink = ctrlFooterAuthorAnchor.NoLink = this.UrlWithoutTrailingUserIdentity == null || this.CurrentPost.UserIdentity == null)) {
				ctrlHeaderAuthorAnchor.NavigateUrl =
					ctrlFooterAuthorAnchor.NavigateUrl		= this.UrlWithoutTrailingUserIdentity + HttpUtility.UrlEncode(this.CurrentPost.UserIdentity);
			}
			ctrlHeaderTimePosted.InnerText =
				ctrlFooterTimePosted.InnerText				= this.DateFormatter.FormatDate(this.CurrentPost.TimePosted);
			ctrlTitle.InnerHtml								= this.TextFormatter.FormatText(this.CurrentPost.Title);
			ctrlBody.InnerHtml								= this.TextFormatter.FormatText(this.CurrentPost.Body);
			SetVisibleFromVisibleAndPosition(true, this.PostInformationPosition, ctrlHeaderPostInformation, ctrlFooterPostInformation);
			bool realDescendantPostsInformationVisible		= IsVisible(this.DescendantPostsInformationVisibility);
			ctrlTitle.Visible								= IsVisible(this.TitleVisibility);
			ctrlHeaderDescendantPostsInformation.Visible	= realDescendantPostsInformationVisible;
			ctrlFooterDescendantPostsInformation.Visible	= realDescendantPostsInformationVisible;
			if(realDescendantPostsInformationVisible) {
				ctrlHeaderPostCount.InnerText =
					ctrlFooterPostCount.InnerText			= this.CurrentPost.PostCount.ToString(NumberFormatInfo.CurrentInfo);
				ctrlHeaderTimeLastPost.InnerText = 
					ctrlFooterTimeLastPost.InnerText		= this.DateFormatter.FormatDate(this.CurrentPost.TimeLastPost);
			}
			if(descendantsNull) { //do not show if we are not interested in descendants
				ctrlDescendantPostsDivision.Visible			= false;
			} else {
				bool realDescendantPostLabelVisible			= IsVisible(this.DescendantPostsLabelVisibility);
				bool realMessageNoDescendantsPostsVisible	= !hasDescendants && !this.MessageNoDescendantPostsVisibility.Equals(HierarchicalPostListVisibility.DoNotShowOnAnyPosts);
				ctrlDescendantPostsDivision.Visible			= hasDescendants || realDescendantPostLabelVisible || realMessageNoDescendantsPostsVisible;
				ctrlDescendantsPostsLabel.Visible			= realDescendantPostLabelVisible;
				ctrlMessageNoDescendantPosts.Visible		= realMessageNoDescendantsPostsVisible;
			}
			SetVisibleFromVisibleAndPosition(IsVisible(this.AddPostButtonVisibility), this.AddPostButtonPosition, btnHeaderAddPost, btnFooterAddPost);
			SetVisibleFromVisibleAndPosition(IsVisible(this.DeletePostButtonVisibility), this.DeletePostButtonPosition, btnHeaderDeletePost, btnFooterDeletePost);
			SetVisibleFromVisibleAndPosition(IsVisible(this.UpdatePostButtonVisibility), this.UpdatePostButtonPosition, btnHeaderUpdatePost, btnFooterUpdatePost);
			bool realChangePostVisibleButtonVisible			= IsVisible(this.ChangePostVisibleButtonVisibility);
			if(realChangePostVisibleButtonVisible) {
				btnHeaderChangePostVisible.InnerText = btnFooterChangePostVisible.InnerText = this.CurrentPost.Visible ? this.ChangePostVisibleButtonHideText : this.ChangePostVisibleButtonUnhideText;
			}
			SetVisibleFromVisibleAndPosition(realChangePostVisibleButtonVisible, this.ChangePostVisibleButtonPosition, btnHeaderChangePostVisible, btnFooterChangePostVisible);
			if(this.HierarchicalPostList != null) {
				this.HierarchicalPostList.PostCollection						= this.CurrentPost.Posts;
				this.HierarchicalPostList.AddPostReplySourcePostId				= GetLocationPostId(this.AddPostReplySourceLocation, this.AddPostReplySourcePostId);
				this.HierarchicalPostList.AddPostReplySourceLocation			= PostLocation.Specified;
				this.HierarchicalPostList.AddPostTargetPostId					= GetLocationPostId(this.AddPostTargetLocation, this.AddPostTargetPostId);
				this.HierarchicalPostList.AddPostTargetLocation					= PostLocation.Specified;
				this.HierarchicalPostList.AuthorResolver						= this.AuthorResolver;
				this.HierarchicalPostList.DateFormatter							= this.DateFormatter;
				this.HierarchicalPostList.PostHyperlinkPrefix					= this.PostHyperlinkPrefix;
				this.HierarchicalPostList.TextFormatter							= this.TextFormatter;
				this.HierarchicalPostList.UserIdentity							= this.UserIdentity;
				this.HierarchicalPostList.AddPostButtonVisibility				= GetDescendantVisibility(this.AddPostButtonVisibility);
				this.HierarchicalPostList.UpdatePostButtonVisibility			= this.UpdatePostButtonVisibility;
				this.HierarchicalPostList.ChangePostVisibleButtonVisibility		= this.ChangePostVisibleButtonVisibility;
				this.HierarchicalPostList.DeletePostButtonVisibility			= this.DeletePostButtonVisibility;
				this.HierarchicalPostList.TitleVisibility						= GetDescendantVisibility(this.TitleVisibility);
				this.HierarchicalPostList.DescendantPostsLabelVisibility		= GetDescendantVisibility(this.DescendantPostsLabelVisibility);
				this.HierarchicalPostList.DescendantPostsInformationVisibility	= GetDescendantVisibility(this.DescendantPostsInformationVisibility);
				this.HierarchicalPostList.MessageNoDescendantPostsVisibility	= GetDescendantVisibility(this.MessageNoDescendantPostsVisibility);
				this.HierarchicalPostList.AddPostButtonPosition					= this.AddPostButtonPosition;
				this.HierarchicalPostList.UpdatePostButtonPosition				= this.UpdatePostButtonPosition;
				this.HierarchicalPostList.ChangePostVisibleButtonPosition		= this.ChangePostVisibleButtonPosition;
				this.HierarchicalPostList.DeletePostButtonPosition				= this.DeletePostButtonPosition;
				this.HierarchicalPostList.PostInformationPosition				= this.PostInformationPosition;
				this.HierarchicalPostList.AddPostButtonText						= this.AddPostButtonText;
				this.HierarchicalPostList.UpdatePostButtonText					= this.UpdatePostButtonText;
				this.HierarchicalPostList.ChangePostVisibleButtonHideText		= this.ChangePostVisibleButtonHideText;
				this.HierarchicalPostList.ChangePostVisibleButtonUnhideText		= this.ChangePostVisibleButtonUnhideText;
				this.HierarchicalPostList.DeletePostButtonText					= this.DeletePostButtonText;
				this.HierarchicalPostList.AuthorLabelText						= this.AuthorLabelText;
				this.HierarchicalPostList.TimePostedLabelText					= this.TimePostedLabelText;
				this.HierarchicalPostList.PostCountLabelText					= this.PostCountLabelText;
				this.HierarchicalPostList.TimeLastPostLabelText					= this.TimeLastPostLabelText;
				this.HierarchicalPostList.DescendantPostsLabelText				= this.DescendantPostsLabelText;
				this.HierarchicalPostList.MessageNoDescendantPostsText			= this.MessageNoDescendantPostsText;
				ctrlDescendantPosts.Controls.Add(this.HierarchicalPostList);
				this.HierarchicalPostList.EnableViewState						= false;
				this.HierarchicalPostList.AddPost								+= this.ctrlHierarchicalPostList_AddPost;
				this.HierarchicalPostList.UpdatePost							+= this.ctrlHierarchicalPostList_UpdatePost;
				this.HierarchicalPostList.PostVisibleChanging					+= this.ctrlHierarchicalPostList_PostVisibleChanging;
				this.HierarchicalPostList.PostVisibleChanged					+= this.ctrlHierarchicalPostList_PostVisibleChanged;
				this.HierarchicalPostList.PostVisibleChangeFailed				+= this.ctrlHierarchicalPostList_PostVisibleChangeFailed;
				this.HierarchicalPostList.PostDeleting							+= this.ctrlHierarchicalPostList_PostDeleting;
				this.HierarchicalPostList.PostDeleted							+= this.ctrlHierarchicalPostList_PostDeleted;
				this.HierarchicalPostList.DataBind();
			}
			AddCssClass(ctrlPostDivision, this.AlternatingItem ? CSSCLASS_EVEN : CSSCLASS_ODD);
			if(!this.CurrentPost.EffectivelyVisible) {
				AddCssClass(ctrlPostDivision, this.CurrentPost.Visible ? CSSCLASS_HIDDENBYHIDDEN : CSSCLASS_HIDDEN);
			}
			ctrlPostDivision.ID				= this.PostHyperlinkPrefix + this.CurrentPost.Id;
			ctrlPostDivision.RenderId		= true;
		}
		#endregion

		#region Helper Methods
		private static void AddCssClass(WebGenericControl control, string cssClass) {
			const string SPACE	= " ";
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