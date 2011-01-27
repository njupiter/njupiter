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
using System.ComponentModel;
using System.Web.UI;

using nJupiter.Web.UI.Controls;
using nJupiter.Web.UI.Controls.Listings;

namespace nJupiter.Services.Forum.UI.Web {

	public class HierarchicalPostList : UserControl {
		#region Constants
		private const string Underscore = "_";

		private const Post.Property Sortproperty = Post.Property.TimePosted;
		private const PostType DefaultPosttype = PostType.This;
		private const bool DefaultGetonlychildren = false;
		private const int DefaultLevels = -1;
		private const bool DefaultSortascending = true;
		private const bool DefaultIncludehidden = false;
		private const bool DefaultLoadattributes = false;
		private static readonly DateTime DefaultDatefilterfrom = DateTime.MinValue;
		private static readonly DateTime DefaultDatefilterto = DateTime.MaxValue;
		private const Post.DateProperty DefaultDatefilterproperty = Post.DateProperty.TimePosted;
		private const HierarchicalPostListVisibility DefaultTitlevisibility = HierarchicalPostListVisibility.ShowOnAllPosts;
		private const HierarchicalPostListVisibility DefaultDescendantpostsinformationvisibility = HierarchicalPostListVisibility.ShowOnAllPosts;
		private const HierarchicalPostListVisibility DefaultDescendantpostslabelvisibility = HierarchicalPostListVisibility.ShowOnRootPostsOnly;
		private const HierarchicalPostListVisibility DefaultMessagenodescendantpostsvisibility = HierarchicalPostListVisibility.ShowOnRootPostsOnly;
		private const HierarchicalPostListVisibility DefaultAddpostbuttonvisibility = HierarchicalPostListVisibility.ShowOnAllPosts;
		private const PostAdministrationVisibility DefaultUpdatepostbuttonvisibility = PostAdministrationVisibility.ShowOnAllPosts;
		private const PostAdministrationVisibility DefaultChangepostvisiblebuttonvisibility = PostAdministrationVisibility.ShowOnAllPosts;
		private const PostAdministrationVisibility DefaultDeletepostbuttonvisibility = PostAdministrationVisibility.ShowOnAllPosts;
		private const Position DefaultAddpostbuttonposition = Position.AboveAndBelow;
		private const Position DefaultUpdatepostbuttonposition = Position.AboveAndBelow;
		private const Position DefaultChangepostvisiblebuttonposition = Position.AboveAndBelow;
		private const Position DefaultDeletepostbuttonposition = Position.AboveAndBelow;
		private const Position DefaultPostinformationposition = Position.AboveAndBelow;
		private const PostLocation DefaultAddposttargetlocation = PostLocation.Current;
		private const PostLocation DefaultAddpostreplysourcelocation = PostLocation.Current;
#if DEBUG
		private const string							DebugPrefix									= "_";
#else
		private const string DebugPrefix = "";
#endif
		private const string DefaultAddpostbuttontext = DebugPrefix + "Add new post";
		private const string DefaultUpdatepostbuttontext = DebugPrefix + "Update post";
		private const string DefaultChangepostvisiblebuttonhidetext = DebugPrefix + "Hide post";
		private const string DefaultChangepostvisiblebuttonunhidetext = DebugPrefix + "Unhide post";
		private const string DefaultDeletepostbuttontext = DebugPrefix + "Delete post";
		private const string DefaultAuthorlabeltext = DebugPrefix + "Author";
		private const string DefaultTimepostedlabeltext = DebugPrefix + "Posted";
		private const string DefaultPostcountlabeltext = DebugPrefix + "Replies";
		private const string DefaultTimelastpostlabeltext = DebugPrefix + "Last post";
		private const string DefaultDescendantpostslabeltext = DebugPrefix + "Replies";
		private const string DefaultMessagenopoststext = DebugPrefix + "No posts were found.";
		private const string DefaultMessagenodescendantpoststext = DebugPrefix + "No replies were found.";

		private const string ViewstateAddpostbuttonvisibility = "v_AddPostButtonVisibility";
		private const string ViewstateAddposttargetpostid = "v_AddPostTargetPostId";
		private const string ViewstateAddposttargetlocation = "v_AddPostTargetLocation";
		private const string ViewstateAddpostreplysourcepostid = "v_AddPostReplySourcePostId";
		private const string ViewstateAddpostreplysourcelocation = "v_AddPostReplySourceLocation";
		#endregion

		#region Variables
		private ForumDao forumDao;
		private bool dataBound;

		private string domain;
		private string categoryName;
		private CategoryId categoryId;
		private PostId postId;
		private PostType postType = DefaultPosttype;
		private bool getOnlyChildren = DefaultGetonlychildren;

		private int levels = DefaultLevels;
		private bool sortAscending = DefaultSortascending;
		private bool includeHidden = DefaultIncludehidden;
		private bool loadAttributes = DefaultLoadattributes;
		private DateTime dateFilterFrom = DefaultDatefilterfrom;
		private DateTime dateFilterTo = DefaultDatefilterto;
		private Post.DateProperty dateFilterProperty = DefaultDatefilterproperty;

		private HierarchicalPostListVisibility titleVisibility = DefaultTitlevisibility;
		private HierarchicalPostListVisibility descendantPostsInformationVisibility = DefaultDescendantpostsinformationvisibility;
		private HierarchicalPostListVisibility descendantPostsLabelVisibility = DefaultDescendantpostslabelvisibility;
		private HierarchicalPostListVisibility messageNoDescendantPostsVisibility = DefaultMessagenodescendantpostsvisibility;
		private PostAdministrationVisibility updatePostButtonVisibility = DefaultUpdatepostbuttonvisibility;
		private PostAdministrationVisibility changePostVisibleButtonVisibility = DefaultChangepostvisiblebuttonvisibility;
		private PostAdministrationVisibility deletePostButtonVisibility = DefaultDeletepostbuttonvisibility;
		private Position addPostButtonPosition = DefaultAddpostbuttonposition;
		private Position updatePostButtonPosition = DefaultUpdatepostbuttonposition;
		private Position changePostVisibleButtonPosition = DefaultChangepostvisiblebuttonposition;
		private Position deletePostButtonPosition = DefaultDeletepostbuttonposition;
		private Position postInformationPosition = DefaultPostinformationposition;

		private DateFormatter dateFormatter;
		private string postHyperlinkPrefix;
		private TextFormatter textFormatter;

		private string addPostButtonText = DefaultAddpostbuttontext;
		private string updatePostButtonText = DefaultUpdatepostbuttontext;
		private string changePostVisibleButtonHideText = DefaultChangepostvisiblebuttonhidetext;
		private string changePostVisibleButtonUnhideText = DefaultChangepostvisiblebuttonunhidetext;
		private string deletePostButtonText = DefaultDeletepostbuttontext;
		private string authorLabelText = DefaultAuthorlabeltext;
		private string timePostedLabelText = DefaultTimepostedlabeltext;
		private string postCountLabelText = DefaultPostcountlabeltext;
		private string timeLastPostLabelText = DefaultTimelastpostlabeltext;
		private string descendantPostsLabelText = DefaultDescendantpostslabeltext;
		private string messageNoPostsText = DefaultMessagenopoststext;
		private string messageNoDescendantPostsText = DefaultMessagenodescendantpoststext;

		private static readonly object EventAddPost = new object();
		private static readonly object EventUpdatePost = new object();
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
		protected GeneralListing ctrlGeneralListing;
		#endregion

		#region Properties
		public ForumDao ForumDao { get { return this.forumDao ?? (this.forumDao = ForumDao.GetInstance()); } set { this.forumDao = value; } }

		public string Domain {
			get { return this.domain; }
			set {
				if(value != null) {
					this.postId = null;
					this.categoryId = null;
				}
				this.domain = value;
			}
		}
		public string CategoryName {
			get { return this.categoryName; }
			set {
				if(value != null) {
					this.postId = null;
					this.categoryId = null;
				}
				this.categoryName = value;
			}
		}
		public CategoryId CategoryId {
			get { return this.categoryId; }
			set {
				if(value != null) {
					this.postId = null;
					this.domain = null;
					this.categoryName = null;
				}
				this.categoryId = value;
			}
		}
		public PostId PostId {
			get { return this.postId; }
			set {
				if(value != null) {
					this.categoryId = null;
					this.domain = null;
					this.categoryName = null;
				}
				this.postId = value;
			}
		}
		public PostType PostType { get { return this.postType; } set { this.postType = value; } }
		public bool GetOnlyChildren { get { return this.getOnlyChildren; } set { this.getOnlyChildren = value; } }
		public int Levels {
			get { return this.levels; }
			set {
				if(value < -1) {
					throw new ArgumentOutOfRangeException("value");
				}
				this.levels = value;
			}
		}
		public bool SortAscending { get { return this.sortAscending; } set { this.sortAscending = value; } }
		public bool IncludeHidden { get { return this.includeHidden; } set { this.includeHidden = value; } }
		public bool LoadAttributes { get { return this.loadAttributes; } set { this.loadAttributes = value; } }
		public DateTime DateFilterFrom { get { return this.dateFilterFrom; } set { this.dateFilterFrom = value; } }
		public DateTime DateFilterTo { get { return this.dateFilterTo; } set { this.dateFilterTo = value; } }
		public Post.DateProperty DateFilterProperty { get { return this.dateFilterProperty; } set { this.dateFilterProperty = value; } }
		public PostCollection PostCollection { get; set; }

		public HierarchicalPostListVisibility TitleVisibility { get { return this.titleVisibility; } set { this.titleVisibility = value; } }
		public HierarchicalPostListVisibility DescendantPostsInformationVisibility { get { return this.descendantPostsInformationVisibility; } set { this.descendantPostsInformationVisibility = value; } }
		public HierarchicalPostListVisibility DescendantPostsLabelVisibility { get { return this.descendantPostsLabelVisibility; } set { this.descendantPostsLabelVisibility = value; } }
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
		public HierarchicalPostListVisibility AddPostButtonVisibility { get { return (HierarchicalPostListVisibility)(this.ViewState[ViewstateAddpostbuttonvisibility] == null ? this.ViewState.Add(ViewstateAddpostbuttonvisibility, DefaultAddpostbuttonvisibility).Value : this.ViewState[ViewstateAddpostbuttonvisibility]); } set { this.ViewState.Add(ViewstateAddpostbuttonvisibility, value); } }
		public PostAdministrationVisibility UpdatePostButtonVisibility { get { return this.updatePostButtonVisibility; } set { this.updatePostButtonVisibility = value; } }
		public PostAdministrationVisibility ChangePostVisibleButtonVisibility { get { return this.changePostVisibleButtonVisibility; } set { this.changePostVisibleButtonVisibility = value; } }
		public PostAdministrationVisibility DeletePostButtonVisibility { get { return this.deletePostButtonVisibility; } set { this.deletePostButtonVisibility = value; } }
		public Position AddPostButtonPosition { get { return this.addPostButtonPosition; } set { this.addPostButtonPosition = value; } }
		public Position UpdatePostButtonPosition { get { return this.updatePostButtonPosition; } set { this.updatePostButtonPosition = value; } }
		public Position ChangePostVisibleButtonPosition { get { return this.changePostVisibleButtonPosition; } set { this.changePostVisibleButtonPosition = value; } }
		public Position DeletePostButtonPosition { get { return this.deletePostButtonPosition; } set { this.deletePostButtonPosition = value; } }
		public Position PostInformationPosition { get { return this.postInformationPosition; } set { this.postInformationPosition = value; } }

		public PostId AddPostTargetPostId { get { return (PostId)this.ViewState[ViewstateAddposttargetpostid]; } set { this.ViewState[ViewstateAddposttargetpostid] = value; } }
		public PostId AddPostReplySourcePostId { get { return (PostId)this.ViewState[ViewstateAddpostreplysourcepostid]; } set { this.ViewState[ViewstateAddpostreplysourcepostid] = value; } }
		public PostLocation AddPostTargetLocation { get { return (PostLocation)(this.ViewState[ViewstateAddposttargetlocation] == null ? this.ViewState.Add(ViewstateAddposttargetlocation, DefaultAddposttargetlocation).Value : this.ViewState[ViewstateAddposttargetlocation]); } set { this.ViewState.Add(ViewstateAddposttargetlocation, value); } }
		public PostLocation AddPostReplySourceLocation { get { return (PostLocation)(this.ViewState[ViewstateAddpostreplysourcelocation] == null ? this.ViewState.Add(ViewstateAddpostreplysourcelocation, DefaultAddpostreplysourcelocation).Value : this.ViewState[ViewstateAddpostreplysourcelocation]); } set { this.ViewState.Add(ViewstateAddpostreplysourcelocation, value); } }
		public AuthorResolver AuthorResolver { get; set; }
		public DateFormatter DateFormatter { get { return this.dateFormatter ?? (this.dateFormatter = new FriendlyDateFormatter()); } set { this.dateFormatter = value; } }
		public string PostHyperlinkPrefix { get { return this.postHyperlinkPrefix ?? (this.postHyperlinkPrefix = this.ClientID + Underscore); } set { this.postHyperlinkPrefix = value; } }
		public TextFormatter TextFormatter { get { return this.textFormatter ?? (this.textFormatter = new WebTextFormatter()); } set { this.textFormatter = value; } }
		public string UrlWithoutTrailingUserIdentity { get; set; }
		public string UserIdentity { get; set; }

		public string AddPostButtonText { get { return this.addPostButtonText; } set { this.addPostButtonText = value; } }
		public string UpdatePostButtonText { get { return this.updatePostButtonText; } set { this.updatePostButtonText = value; } }
		public string ChangePostVisibleButtonHideText { get { return this.changePostVisibleButtonHideText; } set { this.changePostVisibleButtonHideText = value; } }
		public string ChangePostVisibleButtonUnhideText { get { return this.changePostVisibleButtonUnhideText; } set { this.changePostVisibleButtonUnhideText = value; } }
		public string DeletePostButtonText { get { return this.deletePostButtonText; } set { this.deletePostButtonText = value; } }
		public string AuthorLabelText { get { return this.authorLabelText; } set { this.authorLabelText = value; } }
		public string TimePostedLabelText { get { return this.timePostedLabelText; } set { this.timePostedLabelText = value; } }
		public string PostCountLabelText { get { return this.postCountLabelText; } set { this.postCountLabelText = value; } }
		public string TimeLastPostLabelText { get { return this.timeLastPostLabelText; } set { this.timeLastPostLabelText = value; } }
		public string DescendantPostsLabelText { get { return this.descendantPostsLabelText; } set { this.descendantPostsLabelText = value; } }
		public string MessageNoPostsText { get { return this.messageNoPostsText; } set { this.messageNoPostsText = value; } }
		public string MessageNoDescendantPostsText { get { return this.messageNoDescendantPostsText; } set { this.messageNoDescendantPostsText = value; } }
		#endregion

		#region Event Handlers
		private void GeneralListingAfterDataBinding(object sender, EventArgs e) {
			WebParagraph ctrlMessageNoPosts = (WebParagraph)ctrlGeneralListing.FooterControl.FindControl("ctrlMessageNoPosts");
			ctrlMessageNoPosts.EnableViewState = false;
			if(ctrlMessageNoPosts.Visible = ctrlGeneralListing.VisibleItems.Equals(0)) {
				ctrlMessageNoPosts.InnerText = this.MessageNoPostsText;
			}
		}
		private void GeneralListingListItemDataBinding(object sender, ListItemEventArgs e) {
			HierarchicalPostListItem listItem = (HierarchicalPostListItem)e.ListItem;
			listItem.AddPostTargetPostId = this.AddPostTargetPostId;
			listItem.AddPostReplySourcePostId = this.AddPostReplySourcePostId;
			listItem.AddPostTargetLocation = this.AddPostTargetLocation;
			listItem.AddPostReplySourceLocation = this.AddPostReplySourceLocation;
			listItem.AuthorResolver = this.AuthorResolver;
			listItem.DateFormatter = this.DateFormatter;
			listItem.PostHyperlinkPrefix = this.PostHyperlinkPrefix;
			listItem.TextFormatter = this.TextFormatter;
			listItem.UrlWithoutTrailingUserIdentity = this.UrlWithoutTrailingUserIdentity;
			listItem.UserIdentity = this.UserIdentity;
			listItem.AddPostButtonVisibility = this.AddPostButtonVisibility;
			listItem.UpdatePostButtonVisibility = this.UpdatePostButtonVisibility;
			listItem.ChangePostVisibleButtonVisibility = this.ChangePostVisibleButtonVisibility;
			listItem.DeletePostButtonVisibility = this.DeletePostButtonVisibility;
			listItem.TitleVisibility = this.TitleVisibility;
			listItem.DescendantPostsInformationVisibility = this.DescendantPostsInformationVisibility;
			listItem.DescendantPostsLabelVisibility = this.DescendantPostsLabelVisibility;
			listItem.MessageNoDescendantPostsVisibility = this.MessageNoDescendantPostsVisibility;
			listItem.AddPostButtonPosition = this.AddPostButtonPosition;
			listItem.UpdatePostButtonPosition = this.UpdatePostButtonPosition;
			listItem.ChangePostVisibleButtonPosition = this.ChangePostVisibleButtonPosition;
			listItem.DeletePostButtonPosition = this.DeletePostButtonPosition;
			listItem.PostInformationPosition = this.PostInformationPosition;
			listItem.AddPostButtonText = this.AddPostButtonText;
			listItem.ChangePostVisibleButtonHideText = this.ChangePostVisibleButtonHideText;
			listItem.ChangePostVisibleButtonUnhideText = this.ChangePostVisibleButtonUnhideText;
			listItem.UpdatePostButtonText = this.UpdatePostButtonText;
			listItem.DeletePostButtonText = this.DeletePostButtonText;
			listItem.AuthorLabelText = this.AuthorLabelText;
			listItem.TimePostedLabelText = this.TimePostedLabelText;
			listItem.PostCountLabelText = this.PostCountLabelText;
			listItem.TimeLastPostLabelText = this.TimeLastPostLabelText;
			listItem.DescendantPostsLabelText = this.DescendantPostsLabelText;
			listItem.MessageNoDescendantPostsText = this.MessageNoDescendantPostsText;
			listItem.EnableViewState = false;
			listItem.AddPost += this.ListItemAddPost;
			listItem.UpdatePost += this.ListItemUpdatePost;
			listItem.ChangePostVisible += this.ListItemChangePostVisible;
			listItem.DeletePost += this.ListItemDeletePost;
			listItem.PostVisibleChanging += this.ListItemPostVisibleChanging;
			listItem.PostVisibleChanged += this.ListItemPostVisibleChanged;
			listItem.PostVisibleChangeFailed += this.ListItemPostVisibleChangeFailed;
			listItem.PostDeleting += this.ListItemPostDeleting;
			listItem.PostDeleted += this.ListItemPostDeleted;
		}
		private void ListItemAddPost(object sender, AddPostEventArgs e) {
			OnAddPost(e);
		}
		private void ListItemUpdatePost(object sender, UpdatePostEventArgs e) {
			OnUpdatePost(e);
		}
		private void ListItemChangePostVisible(object sender, PostEventArgs e) {
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
		private void ListItemDeletePost(object sender, PostEventArgs e) {
			PostCancelEventArgs cancelEventArgs = new PostCancelEventArgs(e.Post);
			OnPostDeleting(cancelEventArgs);
			if(!cancelEventArgs.Cancel) {
				int affectedPosts = this.ForumDao.DeletePost(e.Post.Id);
				//if affectedPosts = 0, we are silent since the post is gone already (consumers of this event also knows this fact)
				OnPostDeleted(new PostDeletedEventArgs(e.Post, affectedPosts));
			}
		}
		private void ListItemPostVisibleChanging(object sender, PostCancelEventArgs e) {
			OnPostVisibleChanging(e);
		}
		private void ListItemPostVisibleChanged(object sender, PostEventArgs e) {
			OnPostVisibleChanged(e);
		}
		private void ListItemPostVisibleChangeFailed(object sender, PostFailureEventArgs e) {
			OnPostVisibleChangeFailed(e);
		}
		private void ListItemPostDeleting(object sender, PostCancelEventArgs e) {
			OnPostDeleting(e);
		}
		private void ListItemPostDeleted(object sender, PostDeletedEventArgs e) {
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
			ctrlGeneralListing.AfterDataBinding += this.GeneralListingAfterDataBinding;
			ctrlGeneralListing.ListItemDataBinding += this.GeneralListingListItemDataBinding;
			base.OnInit(e);
		}
		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender(e);
			if(!this.dataBound) {
				this.DataBind();
			}
		}
		public override void DataBind() {
			if(this.PostCollection != null && !this.PostCollection.Count.Equals(0)) {
				ctrlGeneralListing.ListCollection = this.PostCollection;
			} else {
				ThreadedPostsResultConfiguration resultConfiguration = new ThreadedPostsResultConfiguration(this.Levels, this.IncludeHidden, this.LoadAttributes, Sortproperty, null, this.SortAscending, this.DateFilterFrom, this.DateFilterTo, this.DateFilterProperty);
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
			this.dataBound = true;
		}
		#endregion
	}

}