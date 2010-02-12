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
using System.Web.UI;

using nJupiter.Web.UI.Controls;
using nJupiter.Web.UI.Controls.Listings;

namespace nJupiter.Services.Forum.UI.Web {

	public class HierarchicalPostList : UserControl {
		#region Constants
		private const string							UNDERSCORE										= "_";

		private const Post.Property						SORTPROPERTY									= Post.Property.TimePosted;
		private const PostType							DEFAULT_POSTTYPE								= PostType.This;
		private const bool								DEFAULT_GETONLYCHILDREN							= false;
		private const int								DEFAULT_LEVELS									= -1;
		private const bool								DEFAULT_SORTASCENDING							= true;
		private const bool								DEFAULT_INCLUDEHIDDEN							= false;
		private const bool								DEFAULT_LOADATTRIBUTES							= false;
		private static readonly	DateTime				DEFAULT_DATEFILTERFROM							= DateTime.MinValue;
		private static readonly	DateTime				DEFAULT_DATEFILTERTO							= DateTime.MaxValue;
		private const Post.DateProperty					DEFAULT_DATEFILTERPROPERTY						= Post.DateProperty.TimePosted;
		private const HierarchicalPostListVisibility	DEFAULT_TITLEVISIBILITY							= HierarchicalPostListVisibility.ShowOnAllPosts;
		private const HierarchicalPostListVisibility	DEFAULT_DESCENDANTPOSTSINFORMATIONVISIBILITY	= HierarchicalPostListVisibility.ShowOnAllPosts;
		private const HierarchicalPostListVisibility	DEFAULT_DESCENDANTPOSTSLABELVISIBILITY			= HierarchicalPostListVisibility.ShowOnRootPostsOnly;
		private const HierarchicalPostListVisibility	DEFAULT_MESSAGENODESCENDANTPOSTSVISIBILITY		= HierarchicalPostListVisibility.ShowOnRootPostsOnly;
		private const HierarchicalPostListVisibility	DEFAULT_ADDPOSTBUTTONVISIBILITY					= HierarchicalPostListVisibility.ShowOnAllPosts;
		private const PostAdministrationVisibility		DEFAULT_UPDATEPOSTBUTTONVISIBILITY				= PostAdministrationVisibility.ShowOnAllPosts;
		private const PostAdministrationVisibility		DEFAULT_CHANGEPOSTVISIBLEBUTTONVISIBILITY		= PostAdministrationVisibility.ShowOnAllPosts;
		private const PostAdministrationVisibility		DEFAULT_DELETEPOSTBUTTONVISIBILITY				= PostAdministrationVisibility.ShowOnAllPosts;
		private const Position							DEFAULT_ADDPOSTBUTTONPOSITION					= Position.AboveAndBelow;
		private const Position							DEFAULT_UPDATEPOSTBUTTONPOSITION				= Position.AboveAndBelow;
		private const Position							DEFAULT_CHANGEPOSTVISIBLEBUTTONPOSITION			= Position.AboveAndBelow;
		private const Position							DEFAULT_DELETEPOSTBUTTONPOSITION				= Position.AboveAndBelow;
		private const Position							DEFAULT_POSTINFORMATIONPOSITION					= Position.AboveAndBelow;
		private const PostLocation						DEFAULT_ADDPOSTTARGETLOCATION					= PostLocation.Current;
		private const PostLocation						DEFAULT_ADDPOSTREPLYSOURCELOCATION				= PostLocation.Current;
#if DEBUG
		private const string							DEBUG_PREFIX									= "_";
#else
		private const string							DEBUG_PREFIX									= "";
#endif
		private const string							DEFAULT_ADDPOSTBUTTONTEXT						= DEBUG_PREFIX + "Add new post";
		private const string							DEFAULT_UPDATEPOSTBUTTONTEXT					= DEBUG_PREFIX + "Update post";
		private const string							DEFAULT_CHANGEPOSTVISIBLEBUTTONHIDETEXT			= DEBUG_PREFIX + "Hide post";
		private const string							DEFAULT_CHANGEPOSTVISIBLEBUTTONUNHIDETEXT		= DEBUG_PREFIX + "Unhide post";
		private const string							DEFAULT_DELETEPOSTBUTTONTEXT					= DEBUG_PREFIX + "Delete post";
		private const string							DEFAULT_AUTHORLABELTEXT							= DEBUG_PREFIX + "Author";
		private const string							DEFAULT_TIMEPOSTEDLABELTEXT						= DEBUG_PREFIX + "Posted";
		private const string							DEFAULT_POSTCOUNTLABELTEXT						= DEBUG_PREFIX + "Replies";
		private const string							DEFAULT_TIMELASTPOSTLABELTEXT					= DEBUG_PREFIX + "Last post";
		private const string							DEFAULT_DESCENDANTPOSTSLABELTEXT				= DEBUG_PREFIX + "Replies";
		private	const string							DEFAULT_MESSAGENOPOSTSTEXT						= DEBUG_PREFIX + "No posts were found.";
		private	const string							DEFAULT_MESSAGENODESCENDANTPOSTSTEXT			= DEBUG_PREFIX + "No replies were found.";

		private const string							VIEWSTATE_ADDPOSTBUTTONVISIBILITY				= "v_AddPostButtonVisibility";
		private const string							VIEWSTATE_ADDPOSTTARGETPOSTID					= "v_AddPostTargetPostId";
		private const string							VIEWSTATE_ADDPOSTTARGETLOCATION					= "v_AddPostTargetLocation";
		private const string							VIEWSTATE_ADDPOSTREPLYSOURCEPOSTID				= "v_AddPostReplySourcePostId";
		private const string							VIEWSTATE_ADDPOSTREPLYSOURCELOCATION			= "v_AddPostReplySourceLocation";
		#endregion

		#region Variables
		private ForumDao							m_ForumDao;
		private bool								m_DataBound;

		private string								m_Domain;
		private string								m_CategoryName;
		private CategoryId							m_CategoryId;
		private PostId								m_PostId;
		private PostType							m_PostType								= DEFAULT_POSTTYPE;
		private bool								m_GetOnlyChildren						= DEFAULT_GETONLYCHILDREN;

		private int									m_Levels								= DEFAULT_LEVELS;
		private bool								m_SortAscending							= DEFAULT_SORTASCENDING;
		private bool								m_IncludeHidden							= DEFAULT_INCLUDEHIDDEN;
		private bool								m_LoadAttributes						= DEFAULT_LOADATTRIBUTES;
		private DateTime							m_DateFilterFrom						= DEFAULT_DATEFILTERFROM;
		private DateTime							m_DateFilterTo							= DEFAULT_DATEFILTERTO;
		private Post.DateProperty					m_DateFilterProperty					= DEFAULT_DATEFILTERPROPERTY;
		private PostCollection						m_PostCollection;

		private HierarchicalPostListVisibility		m_TitleVisibility						= DEFAULT_TITLEVISIBILITY;
		private HierarchicalPostListVisibility		m_DescendantPostsInformationVisibility	= DEFAULT_DESCENDANTPOSTSINFORMATIONVISIBILITY;
		private HierarchicalPostListVisibility		m_DescendantPostsLabelVisibility		= DEFAULT_DESCENDANTPOSTSLABELVISIBILITY;
		private HierarchicalPostListVisibility		m_MessageNoDescendantPostsVisibility	= DEFAULT_MESSAGENODESCENDANTPOSTSVISIBILITY;
		private PostAdministrationVisibility		m_UpdatePostButtonVisibility			= DEFAULT_UPDATEPOSTBUTTONVISIBILITY;
		private PostAdministrationVisibility		m_ChangePostVisibleButtonVisibility		= DEFAULT_CHANGEPOSTVISIBLEBUTTONVISIBILITY;
		private PostAdministrationVisibility		m_DeletePostButtonVisibility			= DEFAULT_DELETEPOSTBUTTONVISIBILITY;
		private Position							m_AddPostButtonPosition					= DEFAULT_ADDPOSTBUTTONPOSITION;
		private Position							m_UpdatePostButtonPosition				= DEFAULT_UPDATEPOSTBUTTONPOSITION;
		private Position							m_ChangePostVisibleButtonPosition		= DEFAULT_CHANGEPOSTVISIBLEBUTTONPOSITION;
		private Position							m_DeletePostButtonPosition				= DEFAULT_DELETEPOSTBUTTONPOSITION;
		private Position							m_PostInformationPosition				= DEFAULT_POSTINFORMATIONPOSITION;

		private AuthorResolver						m_AuthorResolver;
		private DateFormatter						m_DateFormatter;
		private string								m_PostHyperlinkPrefix;
		private TextFormatter						m_TextFormatter;
		private string								m_UrlWithoutTrailingUserIdentity;
		private string								m_UserIdentity;

		private string								m_AddPostButtonText						= DEFAULT_ADDPOSTBUTTONTEXT;
		private string								m_UpdatePostButtonText					= DEFAULT_UPDATEPOSTBUTTONTEXT;
		private string								m_ChangePostVisibleButtonHideText		= DEFAULT_CHANGEPOSTVISIBLEBUTTONHIDETEXT;
		private string								m_ChangePostVisibleButtonUnhideText		= DEFAULT_CHANGEPOSTVISIBLEBUTTONUNHIDETEXT;
		private string								m_DeletePostButtonText					= DEFAULT_DELETEPOSTBUTTONTEXT;
		private string								m_AuthorLabelText						= DEFAULT_AUTHORLABELTEXT;
		private string								m_TimePostedLabelText					= DEFAULT_TIMEPOSTEDLABELTEXT;
		private string								m_PostCountLabelText					= DEFAULT_POSTCOUNTLABELTEXT;
		private string								m_TimeLastPostLabelText					= DEFAULT_TIMELASTPOSTLABELTEXT;
		private string								m_DescendantPostsLabelText				= DEFAULT_DESCENDANTPOSTSLABELTEXT;
		private string								m_MessageNoPostsText					= DEFAULT_MESSAGENOPOSTSTEXT;
		private string								m_MessageNoDescendantPostsText			= DEFAULT_MESSAGENODESCENDANTPOSTSTEXT;
		
		private static readonly object				s_EventAddPost							= new object();
		private static readonly object				s_EventUpdatePost						= new object();
		private static readonly object				s_EventPostVisibleChanging				= new object();
		private static readonly object				s_EventPostVisibleChanged				= new object();
		private static readonly object				s_EventPostVisibleChangeFailed			= new object();
		private static readonly object				s_EventPostDeleting						= new object();
		private static readonly object				s_EventPostDeleted						= new object();
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
		protected GeneralListing	ctrlGeneralListing;
		#endregion

		#region Properties
		public ForumDao ForumDao { get { return this.m_ForumDao ?? (this.m_ForumDao = ForumDao.GetInstance()); } set { m_ForumDao = value; } }

		public string Domain { 
			get { return m_Domain; } 
			set { 
				if(value != null) {
					m_PostId = null; 
					m_CategoryId = null;
				}
				m_Domain = value; 
			} 
		}
		public string CategoryName { 
			get { return m_CategoryName; } 
			set { 
				if(value != null) {
					m_PostId = null;
					m_CategoryId = null;
				}
				m_CategoryName = value; 
			} 
		}
		public CategoryId CategoryId { 
			get { return m_CategoryId; } 
			set { 
				if(value != null) {
					m_PostId = null;
					m_Domain = null;
					m_CategoryName = null;
				}
				m_CategoryId = value; 
			} 
		}
		public PostId PostId { 
			get { return m_PostId; } 
			set { 
				if(value != null) {
					m_CategoryId = null;
					m_Domain = null; 
					m_CategoryName = null;
				}
				m_PostId = value; 
			} 
		}
		public PostType PostType { get { return m_PostType; } set { m_PostType = value; } }
		public bool GetOnlyChildren { get { return m_GetOnlyChildren; } set { m_GetOnlyChildren = value; } }
		public int Levels { 
			get { return m_Levels; } 
			set {
				if(value < -1) {
					throw new ArgumentOutOfRangeException("value");
				}
				m_Levels = value; 
			} 
		}
		public bool SortAscending { get { return m_SortAscending; } set { m_SortAscending = value; } }
		public bool IncludeHidden { get { return m_IncludeHidden; } set { m_IncludeHidden = value; } }
		public bool LoadAttributes { get { return m_LoadAttributes; } set { m_LoadAttributes = value; } }
		public DateTime DateFilterFrom { get { return m_DateFilterFrom; } set { m_DateFilterFrom = value; } }
		public DateTime DateFilterTo { get { return m_DateFilterTo; } set { m_DateFilterTo = value; } }
		public Post.DateProperty DateFilterProperty { get { return m_DateFilterProperty; } set { m_DateFilterProperty = value; } }
		public PostCollection PostCollection { get { return m_PostCollection; } set { m_PostCollection = value; } }

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
		public HierarchicalPostListVisibility AddPostButtonVisibility {	get { return (HierarchicalPostListVisibility)(this.ViewState[VIEWSTATE_ADDPOSTBUTTONVISIBILITY] == null ? this.ViewState.Add(VIEWSTATE_ADDPOSTBUTTONVISIBILITY, DEFAULT_ADDPOSTBUTTONVISIBILITY).Value : this.ViewState[VIEWSTATE_ADDPOSTBUTTONVISIBILITY]); } set { this.ViewState.Add(VIEWSTATE_ADDPOSTBUTTONVISIBILITY, value); } }
		public PostAdministrationVisibility UpdatePostButtonVisibility { get { return m_UpdatePostButtonVisibility; } set { m_UpdatePostButtonVisibility = value; } }
		public PostAdministrationVisibility ChangePostVisibleButtonVisibility { get { return m_ChangePostVisibleButtonVisibility; } set { m_ChangePostVisibleButtonVisibility = value; } }
		public PostAdministrationVisibility DeletePostButtonVisibility { get { return m_DeletePostButtonVisibility; } set { m_DeletePostButtonVisibility = value; } }
		public Position AddPostButtonPosition { get { return m_AddPostButtonPosition; } set { m_AddPostButtonPosition = value; } }
		public Position UpdatePostButtonPosition { get { return m_UpdatePostButtonPosition; } set { m_UpdatePostButtonPosition = value; } }
		public Position ChangePostVisibleButtonPosition { get { return m_ChangePostVisibleButtonPosition; } set { m_ChangePostVisibleButtonPosition = value; } }
		public Position DeletePostButtonPosition { get { return m_DeletePostButtonPosition; } set { m_DeletePostButtonPosition = value; } }
		public Position PostInformationPosition { get { return m_PostInformationPosition; } set { m_PostInformationPosition = value; } }

		public PostId AddPostTargetPostId { get { return (PostId)this.ViewState[VIEWSTATE_ADDPOSTTARGETPOSTID]; } set { this.ViewState[VIEWSTATE_ADDPOSTTARGETPOSTID] = value; } }
		public PostId AddPostReplySourcePostId { get { return (PostId)this.ViewState[VIEWSTATE_ADDPOSTREPLYSOURCEPOSTID]; } set { this.ViewState[VIEWSTATE_ADDPOSTREPLYSOURCEPOSTID] = value; } }
		public PostLocation AddPostTargetLocation { get { return (PostLocation)(this.ViewState[VIEWSTATE_ADDPOSTTARGETLOCATION] == null ? this.ViewState.Add(VIEWSTATE_ADDPOSTTARGETLOCATION, DEFAULT_ADDPOSTTARGETLOCATION).Value : this.ViewState[VIEWSTATE_ADDPOSTTARGETLOCATION]); } set { this.ViewState.Add(VIEWSTATE_ADDPOSTTARGETLOCATION, value); } }
		public PostLocation AddPostReplySourceLocation { get { return (PostLocation)(this.ViewState[VIEWSTATE_ADDPOSTREPLYSOURCELOCATION] == null ? this.ViewState.Add(VIEWSTATE_ADDPOSTREPLYSOURCELOCATION, DEFAULT_ADDPOSTREPLYSOURCELOCATION).Value : this.ViewState[VIEWSTATE_ADDPOSTREPLYSOURCELOCATION]); } set { this.ViewState.Add(VIEWSTATE_ADDPOSTREPLYSOURCELOCATION, value); } }
		public AuthorResolver AuthorResolver { get { return m_AuthorResolver; } set { m_AuthorResolver = value; } }
		public DateFormatter DateFormatter { get { return this.m_DateFormatter ?? (this.m_DateFormatter = new FriendlyDateFormatter()); } set { m_DateFormatter = value; } }
		public string PostHyperlinkPrefix {	get { return this.m_PostHyperlinkPrefix ?? (this.m_PostHyperlinkPrefix = this.ClientID + UNDERSCORE);	} set { m_PostHyperlinkPrefix = value; } }
		public TextFormatter TextFormatter { get { return this.m_TextFormatter ?? (this.m_TextFormatter = new WebTextFormatter()); } set { m_TextFormatter = value; } }
		public string UrlWithoutTrailingUserIdentity { get { return m_UrlWithoutTrailingUserIdentity; } set { m_UrlWithoutTrailingUserIdentity = value; } }
		public string UserIdentity { get { return m_UserIdentity; } set { m_UserIdentity = value; } }

		public string AddPostButtonText { get { return m_AddPostButtonText; } set { m_AddPostButtonText = value; } }
		public string UpdatePostButtonText { get { return m_UpdatePostButtonText; } set { m_UpdatePostButtonText = value; } }
		public string ChangePostVisibleButtonHideText { get { return m_ChangePostVisibleButtonHideText; } set { m_ChangePostVisibleButtonHideText = value; } }
		public string ChangePostVisibleButtonUnhideText { get { return m_ChangePostVisibleButtonUnhideText; } set { m_ChangePostVisibleButtonUnhideText = value; } }
		public string DeletePostButtonText { get { return m_DeletePostButtonText; } set { m_DeletePostButtonText = value; } }
		public string AuthorLabelText { get { return m_AuthorLabelText; } set { m_AuthorLabelText = value; } }
		public string TimePostedLabelText { get { return m_TimePostedLabelText; } set { m_TimePostedLabelText = value; } }
		public string PostCountLabelText { get { return m_PostCountLabelText; } set { m_PostCountLabelText = value; } }
		public string TimeLastPostLabelText { get { return m_TimeLastPostLabelText; } set { m_TimeLastPostLabelText = value; } }
		public string DescendantPostsLabelText { get { return m_DescendantPostsLabelText; } set { m_DescendantPostsLabelText = value; } }
		public string MessageNoPostsText { get { return m_MessageNoPostsText; } set { m_MessageNoPostsText = value; } }
		public string MessageNoDescendantPostsText { get { return m_MessageNoDescendantPostsText; } set { m_MessageNoDescendantPostsText = value; } }
		#endregion

		#region Event Handlers
		private void ctrlGeneralListing_AfterDataBinding(object sender, EventArgs e) {
			WebParagraph ctrlMessageNoPosts		= (WebParagraph)ctrlGeneralListing.FooterControl.FindControl("ctrlMessageNoPosts");
			ctrlMessageNoPosts.EnableViewState	= false;
			if(ctrlMessageNoPosts.Visible		= ctrlGeneralListing.VisibleItems.Equals(0)) {
				ctrlMessageNoPosts.InnerText	= this.MessageNoPostsText;
			}
		}
		private void ctrlGeneralListing_ListItemDataBinding(object sender, ListItemEventArgs e) {
			HierarchicalPostListItem listItem				= (HierarchicalPostListItem)e.ListItem;
			listItem.AddPostTargetPostId					= this.AddPostTargetPostId;
			listItem.AddPostReplySourcePostId				= this.AddPostReplySourcePostId;
			listItem.AddPostTargetLocation					= this.AddPostTargetLocation;
			listItem.AddPostReplySourceLocation				= this.AddPostReplySourceLocation;
			listItem.AuthorResolver							= this.AuthorResolver;
			listItem.DateFormatter							= this.DateFormatter;
			listItem.PostHyperlinkPrefix					= this.PostHyperlinkPrefix;
			listItem.TextFormatter							= this.TextFormatter;
			listItem.UrlWithoutTrailingUserIdentity			= this.UrlWithoutTrailingUserIdentity;
			listItem.UserIdentity							= this.UserIdentity;
			listItem.AddPostButtonVisibility				= this.AddPostButtonVisibility;
			listItem.UpdatePostButtonVisibility				= this.UpdatePostButtonVisibility;
			listItem.ChangePostVisibleButtonVisibility		= this.ChangePostVisibleButtonVisibility;
			listItem.DeletePostButtonVisibility				= this.DeletePostButtonVisibility;
			listItem.TitleVisibility						= this.TitleVisibility;
			listItem.DescendantPostsInformationVisibility	= this.DescendantPostsInformationVisibility;
			listItem.DescendantPostsLabelVisibility			= this.DescendantPostsLabelVisibility;
			listItem.MessageNoDescendantPostsVisibility		= this.MessageNoDescendantPostsVisibility;
			listItem.AddPostButtonPosition					= this.AddPostButtonPosition;
			listItem.UpdatePostButtonPosition				= this.UpdatePostButtonPosition;
			listItem.ChangePostVisibleButtonPosition		= this.ChangePostVisibleButtonPosition;
			listItem.DeletePostButtonPosition				= this.DeletePostButtonPosition;
			listItem.PostInformationPosition				= this.PostInformationPosition;
			listItem.AddPostButtonText						= this.AddPostButtonText;
			listItem.ChangePostVisibleButtonHideText		= this.ChangePostVisibleButtonHideText;
			listItem.ChangePostVisibleButtonUnhideText		= this.ChangePostVisibleButtonUnhideText;
			listItem.UpdatePostButtonText					= this.UpdatePostButtonText;
			listItem.DeletePostButtonText					= this.DeletePostButtonText;
			listItem.AuthorLabelText						= this.AuthorLabelText;
			listItem.TimePostedLabelText					= this.TimePostedLabelText;
			listItem.PostCountLabelText						= this.PostCountLabelText;
			listItem.TimeLastPostLabelText					= this.TimeLastPostLabelText;
			listItem.DescendantPostsLabelText				= this.DescendantPostsLabelText;
			listItem.MessageNoDescendantPostsText			= this.MessageNoDescendantPostsText;
			listItem.EnableViewState						= false;
			listItem.AddPost								+= this.listItem_AddPost;
			listItem.UpdatePost								+= this.listItem_UpdatePost;
			listItem.ChangePostVisible						+= this.listItem_ChangePostVisible;
			listItem.DeletePost								+= this.listItem_DeletePost;
			listItem.PostVisibleChanging					+= this.listItem_PostVisibleChanging;
			listItem.PostVisibleChanged						+= this.listItem_PostVisibleChanged;
			listItem.PostVisibleChangeFailed				+= this.listItem_PostVisibleChangeFailed;
			listItem.PostDeleting							+= this.listItem_PostDeleting;
			listItem.PostDeleted							+= this.listItem_PostDeleted;
		}
		private void listItem_AddPost(object sender, AddPostEventArgs e) {
			OnAddPost(e);
		}
		private void listItem_UpdatePost(object sender, UpdatePostEventArgs e) {
			OnUpdatePost(e);
		}
		private void listItem_ChangePostVisible(object sender, PostEventArgs e) {
			e.Post.Visible = !e.Post.Visible;
			PostCancelEventArgs cancelEventArgs = new PostCancelEventArgs(e.Post);
			OnPostVisibleChanging(cancelEventArgs);
			if(!cancelEventArgs.Cancel) {
				try {
					this.ForumDao.SavePost(e.Post);
					OnPostVisibleChanged(e);
				} catch(Exception ex) {
					OnPostVisibleChangeFailed(new PostFailureEventArgs(e.Post, ex));
				}
			}
		}
		private void listItem_DeletePost(object sender, PostEventArgs e) {
			PostCancelEventArgs cancelEventArgs = new PostCancelEventArgs(e.Post);
			OnPostDeleting(cancelEventArgs);
			if(!cancelEventArgs.Cancel) {
				int affectedPosts = this.ForumDao.DeletePost(e.Post.Id);
				//if affectedPosts = 0, we are silent since the post is gone already (consumers of this event also knows this fact)
				OnPostDeleted(new PostDeletedEventArgs(e.Post, affectedPosts));
			}
		}
		private void listItem_PostVisibleChanging(object sender, PostCancelEventArgs e) {
			OnPostVisibleChanging(e);
		}
		private void listItem_PostVisibleChanged(object sender, PostEventArgs e) {
			OnPostVisibleChanged(e);
		}
		private void listItem_PostVisibleChangeFailed(object sender, PostFailureEventArgs e) {
			OnPostVisibleChangeFailed(e);
		}
		private void listItem_PostDeleting(object sender, PostCancelEventArgs e) {
			OnPostDeleting(e);
		}
		private void listItem_PostDeleted(object sender, PostDeletedEventArgs e) {
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
			ctrlGeneralListing.AfterDataBinding		+= this.ctrlGeneralListing_AfterDataBinding;
			ctrlGeneralListing.ListItemDataBinding	+= this.ctrlGeneralListing_ListItemDataBinding;
			base.OnInit(e);
		}
		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender(e);
			if(!m_DataBound) {
				this.DataBind();
			}
		}
		public override void DataBind() {
			if(this.PostCollection != null && !this.PostCollection.Count.Equals(0)) {
				ctrlGeneralListing.ListCollection = this.PostCollection;
			} else {
				ThreadedPostsResultConfiguration resultConfiguration = new ThreadedPostsResultConfiguration(this.Levels, this.IncludeHidden, this.LoadAttributes, SORTPROPERTY, null, this.SortAscending, this.DateFilterFrom, this.DateFilterTo, this.DateFilterProperty);
				if(this.PostId != null) {
					if(this.GetOnlyChildren) {
						ctrlGeneralListing.ListCollection = this.ForumDao.GetPosts(this.PostId, this.PostType, resultConfiguration);
					} else {
						Post post = this.ForumDao.GetPost(this.PostId, this.PostType, resultConfiguration);
						ctrlGeneralListing.ListCollection = post == null ? new object[0] : new object[] { post };
					}
				} else if(this.CategoryId != null) {
					ctrlGeneralListing.ListCollection = this.ForumDao.GetPosts(this.CategoryId, resultConfiguration);
				} else if(this.Domain != null) {
					ctrlGeneralListing.ListCollection = this.CategoryName == null ?
						this.ForumDao.GetPosts(this.Domain, resultConfiguration) :
						this.ForumDao.GetPosts(this.Domain, this.CategoryName, resultConfiguration);
				} else {
					ctrlGeneralListing.ListCollection = this.ForumDao.GetPosts(resultConfiguration);
				}
			}
			ctrlGeneralListing.ViewStateList = !(this.AddPostButtonVisibility.Equals(HierarchicalPostListVisibility.DoNotShowOnAnyPosts) &&
				this.ChangePostVisibleButtonVisibility.Equals(PostAdministrationVisibility.DoNotShowOnAnyPosts) &&
				this.DeletePostButtonVisibility.Equals(PostAdministrationVisibility.DoNotShowOnAnyPosts) &&
				this.UpdatePostButtonVisibility.Equals(PostAdministrationVisibility.DoNotShowOnAnyPosts));
			ctrlGeneralListing.DataBind();
			m_DataBound = true;
		}
		#endregion
	}

}