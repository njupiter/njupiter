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
using System.Web.UI;
using System.Web.UI.WebControls;

using nJupiter.Web.UI;
using nJupiter.Web.UI.Controls;
using nJupiter.Web.UI.Controls.Listings;

namespace nJupiter.Services.Forum.UI.Web {

	public class FlatTabularPostList : UserControl {
		#region Constants
		private const Post.Property DefaultSortproperty = Post.Property.TimeLastPost;
		private const string DefaultSortattributename = null;
		private const bool DefaultSortascending = true;
		private const bool DefaultIncludehidden = false;
		private const bool DefaultLoadattributes = false;
		private const int DefaultMaximumbodylength = 255;
		private const int DefaultMaximumtitlelength = 255;
		private const int DefaultMaximumrootposttitlelength = 255;
		private static readonly DateTime DefaultDatefilterfrom = DateTime.MinValue;
		private static readonly DateTime DefaultDatefilterto = DateTime.MaxValue;
		private const Post.DateProperty DefaultDatefilterproperty = Post.DateProperty.TimePosted;

		private const bool DefaultNumberofitemsselectorvisible = true;
		private const bool DefaultNumberofitemsselectorincludeallitem = false;
		private const bool DefaultSearchtextboxvisible = true;
		private const bool DefaultAddpostbuttonvisible = true;
		private const bool DefaultPagingvisible = true;
		private const bool DefaultTablecolumnheadersvisible = true;
		private const bool DefaultTablecolumnauthorvisible = true;
		private const bool DefaultTablecolumnbodyvisible = false;
		private const bool DefaultTablecolumncategoryvisible = true;
		private const bool DefaultTablecolumneffectivelyvisiblevisible = false;
		private const bool DefaultTablecolumnpostcountvisible = true;
		private const bool DefaultTablecolumnrootposttitlevisible = false;
		private const bool DefaultTablecolumnsearchrelevancevisible = false;
		private const bool DefaultTablecolumntimelastpostvisible = true;
		private const bool DefaultTablecolumntimepostedvisible = true;
		private const bool DefaultTablecolumntitlevisible = true;
		private const bool DefaultTablecolumnvisiblevisible = false;

		private const Position DefaultSearchtextboxposition = Position.AboveAndBelow;
		private const Position DefaultAddpostbuttonposition = Position.AboveAndBelow;
		private const Position DefaultPagingposition = Position.AboveAndBelow;

		private const bool DefaultNumberofitemsselectorautopostback = true;
		private const bool DefaultSortingenabled = true;
		private const Paging.PagingType DefaultPagingtype = Paging.PagingType.Buttons;

#if DEBUG
		private const string				DebugPrefix									= "_";
#else
		private const string DebugPrefix = "";
#endif
		private const string DefaultOmissionindicator = "\u2026";
		private const string DefaultTablecaptiontext = DebugPrefix + "Listing of posts";
		private const string DefaultTablecolumnauthortext = DebugPrefix + "Author";
		private const string DefaultTablecolumnbodytext = DebugPrefix + "Text";
		private const string DefaultTablecolumncategorytext = DebugPrefix + "Category";
		private const string DefaultTablecolumneffectivelyvisibletext = DebugPrefix + "Effectively visible";
		private const string DefaultTablecolumnpostcounttext = DebugPrefix + "Replies";
		private const string DefaultTablecolumnrootposttitletext = DebugPrefix + "Thread title";
		private const string DefaultTablecolumnsearchrelevancetext = DebugPrefix + "Relevance";
		private const string DefaultTablecolumntimelastposttext = DebugPrefix + "Last post";
		private const string DefaultTablecolumntimepostedtext = DebugPrefix + "Created";
		private const string DefaultTablecolumntitletext = DebugPrefix + "Title";
		private const string DefaultTablecolumnvisibletext = DebugPrefix + "Visible";
		private const string DefaultTablevalueeffectivelyvisibletruetext = DebugPrefix + "Yes";
		private const string DefaultTablevalueeffectivelyvisiblefalsetext = DebugPrefix + "No";
		private const string DefaultTablevaluevisibletruetext = DefaultTablevalueeffectivelyvisibletruetext;
		private const string DefaultTablevaluevisiblefalsetext = DefaultTablevalueeffectivelyvisiblefalsetext;
		private const string DefaultMessagenopoststext = DebugPrefix + "No posts were found.";
		private const string DefaultFieldsetlabeltext = DebugPrefix + "Input fields";
		private const string DefaultSearchtextboxlabeltext = DebugPrefix + "Search for";
		private const string DefaultNumberofitemsselectorlabeltext = DebugPrefix + "Number of posts per page";
		private const string DefaultNumberofitemsselectorallitemtext = DebugPrefix + "All";
		private const string DefaultSubmitbuttontext = DebugPrefix + "Show";
		private const string DefaultAddpostbuttontext = DebugPrefix + "Add new post";

		private const string ViewstateSortproperty = "v_SortProperty";
		private const string ViewstateSortattributename = "v_SortAttributeName";
		private const string ViewstateSortascending = "v_SortAscending";
		private const string ViewstateAddpostbuttonvisible = "v_AddPostButtonVisible";
		private const string ViewstateSearchtext = "v_SearchText";

		private const string CssclassAscending = "column-sorted ascending";
		private const string CssclassDescending = "column-sorted descending";
		#endregion

		#region Variables
		private ForumDao forumDao;

		private TextBox changedSearchTextBox;
		private WebPlaceHolder head;
		private WebPlaceHolder foot;
		private bool dataBound;

		private DateFormatter dateFormatter;

		private string domain;
		private string categoryName;
		private CategoryId categoryId;
		private PostId postId;
		private bool includeHidden = DefaultIncludehidden;
		private bool loadAttributes = DefaultLoadattributes;
		private int maximumBodyLength = DefaultMaximumbodylength;
		private int maximumTitleLength = DefaultMaximumtitlelength;
		private int maximumRootPostTitleLength = DefaultMaximumrootposttitlelength;
		private DateTime dateFilterFrom = DefaultDatefilterfrom;
		private DateTime dateFilterTo = DefaultDatefilterto;
		private Post.DateProperty dateFilterProperty = DefaultDatefilterproperty;
		private AttributeCriterionCollection attributeCriteria;

		private bool numberOfItemsSelectorVisible = DefaultNumberofitemsselectorvisible;
		private bool numberOfItemsSelectorIncludeAllItem = DefaultNumberofitemsselectorincludeallitem;
		private bool searchTextBoxVisible = DefaultSearchtextboxvisible;
		private bool pagingVisible = DefaultPagingvisible;
		private bool tableColumnAuthorVisible = DefaultTablecolumnauthorvisible;
		private bool tableColumnBodyVisible = DefaultTablecolumnbodyvisible;
		private bool tableColumnCategoryVisible = DefaultTablecolumncategoryvisible;
		private bool tableColumnEffectivelyVisibleVisible = DefaultTablecolumneffectivelyvisiblevisible;
		private bool tableColumnPostCountVisible = DefaultTablecolumnpostcountvisible;
		private bool tableColumnRootPostTitleVisible = DefaultTablecolumnrootposttitlevisible;
		private bool tableColumnSearchRelevanceVisible = DefaultTablecolumnsearchrelevancevisible;
		private bool tableColumnTimeLastPostVisible = DefaultTablecolumntimelastpostvisible;
		private bool tableColumnTimePostedVisible = DefaultTablecolumntimepostedvisible;
		private bool tableColumnTitleVisible = DefaultTablecolumntitlevisible;
		private bool tableColumnVisibleVisible = DefaultTablecolumnvisiblevisible;

		private Position addPostButtonPosition = DefaultAddpostbuttonposition;
		private Position searchTextBoxPosition = DefaultSearchtextboxposition;
		private Position pagingPosition = DefaultPagingposition;

		private bool numberOfItemsSelectorAutoPostBack = DefaultNumberofitemsselectorautopostback;
		private bool tableColumnHeadersVisible = DefaultTablecolumnheadersvisible;
		private bool sortingEnabled = DefaultSortingenabled;
		private Paging.PagingType pagingType = DefaultPagingtype;

		private string omissionIndicator = DefaultOmissionindicator;
		private string fieldSetLabelText = DefaultFieldsetlabeltext;
		private string messageNoPostsText = DefaultMessagenopoststext;
		private string tableCaptionText = DefaultTablecaptiontext;
		private string tableColumnAuthorText = DefaultTablecolumnauthortext;
		private string tableColumnBodyText = DefaultTablecolumnbodytext;
		private string tableColumnCategoryText = DefaultTablecolumncategorytext;
		private string tableColumnEffectivelyVisibleText = DefaultTablecolumneffectivelyvisibletext;
		private string tableColumnPostCountText = DefaultTablecolumnpostcounttext;
		private string tableColumnRootPostTitleText = DefaultTablecolumnrootposttitletext;
		private string tableColumnSearchRelevanceText = DefaultTablecolumnsearchrelevancetext;
		private string tableColumnTimeLastPostText = DefaultTablecolumntimelastposttext;
		private string tableColumnTimePostedText = DefaultTablecolumntimepostedtext;
		private string tableColumnTitleText = DefaultTablecolumntitletext;
		private string tableColumnVisibleText = DefaultTablecolumnvisibletext;
		private string tableValueEffectivelyVisibleTrueText = DefaultTablevalueeffectivelyvisibletruetext;
		private string tableValueEffectivelyVisibleFalseText = DefaultTablevalueeffectivelyvisiblefalsetext;
		private string tableValueVisibleTrueText = DefaultTablevaluevisibletruetext;
		private string tableValueVisibleFalseText = DefaultTablevaluevisiblefalsetext;
		private string numberOfItemsSelectorLabelText = DefaultNumberofitemsselectorlabeltext;
		private string numberOfItemsSelectorAllItemText = DefaultNumberofitemsselectorallitemtext;
		private string searchTextBoxLabelText = DefaultSearchtextboxlabeltext;
		private string submitButtonText = DefaultSubmitbuttontext;
		private string addPostButtonText = DefaultAddpostbuttontext;

		private static readonly object EventAddPost = new object();
		private static readonly object EventSearch = new object();
		private static readonly object EventSearchDiscarded = new object();
		private static readonly object EventPostsSorting = new object();
		#endregion

		#region Events
		public event AddPostEventHandler AddPost {
			add { base.Events.AddHandler(EventAddPost, value); }
			remove { base.Events.RemoveHandler(EventAddPost, value); }
		}
		public event SearchEventHandler Search {
			add { base.Events.AddHandler(EventSearch, value); }
			remove { base.Events.RemoveHandler(EventSearch, value); }
		}
		public event SearchDiscardedEventHandler SearchDiscarded {
			add { base.Events.AddHandler(EventSearchDiscarded, value); }
			remove { base.Events.RemoveHandler(EventSearchDiscarded, value); }
		}
		public event PostsSortingEventHandler PostsSorting {
			add { base.Events.AddHandler(EventPostsSorting, value); }
			remove { base.Events.RemoveHandler(EventPostsSorting, value); }
		}
		#endregion

		#region UI Members
		protected PagedListing ctrlPagedListing;

		private TextBox txtHeaderSearch;
		private TextBox txtFooterSearch;
		private WebButton btnHeaderAddPost;
		private WebButton btnFooterAddPost;
		private WebButton btnHeaderSubmit;
		private WebButton btnFooterSubmit;
		private WebLinkButton lbtnAuthor;
		private WebLinkButton lbtnBody;
		private WebLinkButton lbtnCategory;
		private WebLinkButton lbtnEffectivelyVisible;
		private WebLinkButton lbtnPostCount;
		private WebLinkButton lbtnRootPostTitle;
		private WebLinkButton lbtnSearchRelevance;
		private WebLinkButton lbtnTimeLastPost;
		private WebLinkButton lbtnTimePosted;
		private WebLinkButton lbtnTitle;
		private WebLinkButton lbtnVisible;
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
		public PostType PostType { get; set; }
		public string SearchText { get { return (string)this.ViewState[ViewstateSearchtext]; } set { this.ViewState.Add(ViewstateSearchtext, value); } }
		public string UserIdentity { get; set; }
		public bool IncludeHidden { get { return this.includeHidden; } set { this.includeHidden = value; } }
		public bool LoadAttributes { get { return this.loadAttributes; } set { this.loadAttributes = value; } }
		public int MaximumBodyLength {
			get { return this.maximumBodyLength; }
			set {
				if(value < 1) {
					throw new ArgumentOutOfRangeException("value");
				}
				this.maximumBodyLength = value;
			}
		}
		public int MaximumTitleLength {
			get { return this.maximumTitleLength; }
			set {
				if(value < 1) {
					throw new ArgumentOutOfRangeException("value");
				}
				this.maximumTitleLength = value;
			}
		}
		public int MaximumRootPostTitleLength {
			get { return this.maximumRootPostTitleLength; }
			set {
				if(value < 1) {
					throw new ArgumentOutOfRangeException("value");
				}
				this.maximumRootPostTitleLength = value;
			}
		}
		public DateTime DateFilterFrom { get { return this.dateFilterFrom; } set { this.dateFilterFrom = value; } }
		public DateTime DateFilterTo { get { return this.dateFilterTo; } set { this.dateFilterTo = value; } }
		public Post.DateProperty DateFilterProperty { get { return this.dateFilterProperty; } set { this.dateFilterProperty = value; } }
		public AttributeCriterionCollection AttributeCriteria { get { return this.attributeCriteria ?? (this.attributeCriteria = new AttributeCriterionCollection()); } }
		public PostCollection PostCollection { get; set; }

		public string UrlWithoutTrailingPostId { get; set; }
		public string UrlWithoutTrailingUserIdentity { get; set; }
		public string UrlWithoutTrailingCategoryId { get; set; }
		public string UrlWithoutTrailingRootPostId { get; set; }
		public AuthorResolver AuthorResolver { get; set; }
		public DateFormatter DateFormatter { get { return this.dateFormatter ?? (this.dateFormatter = new FriendlyDateFormatter()); } set { this.dateFormatter = value; } }

		public bool NumberOfItemsSelectorVisible { get { return this.numberOfItemsSelectorVisible; } set { this.numberOfItemsSelectorVisible = value; } }
		public bool NumberOfItemsSelectorIncludeAllItem { get { return this.numberOfItemsSelectorIncludeAllItem; } set { this.numberOfItemsSelectorIncludeAllItem = value; } }
		public bool SearchTextBoxVisible { get { return this.searchTextBoxVisible; } set { this.searchTextBoxVisible = value; } }
		public bool AddPostButtonVisible { get { return (bool)(this.ViewState[ViewstateAddpostbuttonvisible] == null ? this.ViewState.Add(ViewstateAddpostbuttonvisible, DefaultAddpostbuttonvisible).Value : this.ViewState[ViewstateAddpostbuttonvisible]); } set { this.ViewState.Add(ViewstateAddpostbuttonvisible, value); } }
		public bool PagingVisible { get { return this.pagingVisible; } set { this.pagingVisible = value; } }
		public bool TableColumnAuthorVisible { get { return this.tableColumnAuthorVisible; } set { this.tableColumnAuthorVisible = value; } }
		public bool TableColumnBodyVisible { get { return this.tableColumnBodyVisible; } set { this.tableColumnBodyVisible = value; } }
		public bool TableColumnCategoryVisible { get { return this.tableColumnCategoryVisible; } set { this.tableColumnCategoryVisible = value; } }
		public bool TableColumnEffectivelyVisibleVisible { get { return this.tableColumnEffectivelyVisibleVisible; } set { this.tableColumnEffectivelyVisibleVisible = value; } }
		public bool TableColumnPostCountVisible { get { return this.tableColumnPostCountVisible; } set { this.tableColumnPostCountVisible = value; } }
		public bool TableColumnRootPostTitleVisible { get { return this.tableColumnRootPostTitleVisible; } set { this.tableColumnRootPostTitleVisible = value; } }
		public bool TableColumnSearchRelevanceVisible { get { return this.tableColumnSearchRelevanceVisible; } set { this.tableColumnSearchRelevanceVisible = value; } }
		public bool TableColumnTimeLastPostVisible { get { return this.tableColumnTimeLastPostVisible; } set { this.tableColumnTimeLastPostVisible = value; } }
		public bool TableColumnTimePostedVisible { get { return this.tableColumnTimePostedVisible; } set { this.tableColumnTimePostedVisible = value; } }
		public bool TableColumnTitleVisible { get { return this.tableColumnTitleVisible; } set { this.tableColumnTitleVisible = value; } }
		public bool TableColumnVisibleVisible { get { return this.tableColumnVisibleVisible; } set { this.tableColumnVisibleVisible = value; } }

		public Position SearchTextBoxPosition { get { return this.searchTextBoxPosition; } set { this.searchTextBoxPosition = value; } }
		public Position AddPostButtonPosition { get { return this.addPostButtonPosition; } set { this.addPostButtonPosition = value; } }
		public Position PagingPosition { get { return this.pagingPosition; } set { this.pagingPosition = value; } }

		public bool NumberOfItemsSelectorAutoPostBack { get { return this.numberOfItemsSelectorAutoPostBack; } set { this.numberOfItemsSelectorAutoPostBack = value; } }
		public bool TableColumnHeadersVisible { get { return this.tableColumnHeadersVisible; } set { this.tableColumnHeadersVisible = value; } }
		public bool SortingEnabled { get { return this.sortingEnabled; } set { this.sortingEnabled = value; } }
		public bool SortAscending { get { return (bool)(this.ViewState[ViewstateSortascending] == null ? this.ViewState.Add(ViewstateSortascending, DefaultSortascending).Value : this.ViewState[ViewstateSortascending]); } set { this.ViewState.Add(ViewstateSortascending, value); } }
		public Post.Property SortProperty { get { return (Post.Property)(this.ViewState[ViewstateSortproperty] == null ? this.ViewState.Add(ViewstateSortproperty, DefaultSortproperty).Value : this.ViewState[ViewstateSortproperty]); } set { this.ViewState.Add(ViewstateSortproperty, value); } }
		public string SortAttributeName { get { return (string)(this.ViewState[ViewstateSortattributename] == null ? this.ViewState.Add(ViewstateSortattributename, DefaultSortattributename).Value : this.ViewState[ViewstateSortattributename]); } set { this.ViewState.Add(ViewstateSortattributename, value); } }
		public int NumberOfItems { get { return ctrlPagedListing.ItemsPerPage; } set { ctrlPagedListing.ItemsPerPage = value; } }
		public Paging.PagingType PagingType { get { return this.pagingType; } set { this.pagingType = value; } }

		public string OmissionIndicator { get { return this.omissionIndicator; } set { this.omissionIndicator = value; } }
		public string FieldSetLabelText { get { return this.fieldSetLabelText; } set { this.fieldSetLabelText = value; } }
		public string MessageNoPostsText { get { return this.messageNoPostsText; } set { this.messageNoPostsText = value; } }
		public string TableCaptionText { get { return this.tableCaptionText; } set { this.tableCaptionText = value; } }
		public string TableColumnAuthorText { get { return this.tableColumnAuthorText; } set { this.tableColumnAuthorText = value; } }
		public string TableColumnBodyText { get { return this.tableColumnBodyText; } set { this.tableColumnBodyText = value; } }
		public string TableColumnCategoryText { get { return this.tableColumnCategoryText; } set { this.tableColumnCategoryText = value; } }
		public string TableColumnEffectivelyVisibleText { get { return this.tableColumnEffectivelyVisibleText; } set { this.tableColumnEffectivelyVisibleText = value; } }
		public string TableColumnPostCountText { get { return this.tableColumnPostCountText; } set { this.tableColumnPostCountText = value; } }
		public string TableColumnRootPostTitleText { get { return this.tableColumnRootPostTitleText; } set { this.tableColumnRootPostTitleText = value; } }
		public string TableColumnSearchRelevanceText { get { return this.tableColumnSearchRelevanceText; } set { this.tableColumnSearchRelevanceText = value; } }
		public string TableColumnTimeLastPostText { get { return this.tableColumnTimeLastPostText; } set { this.tableColumnTimeLastPostText = value; } }
		public string TableColumnTimePostedText { get { return this.tableColumnTimePostedText; } set { this.tableColumnTimePostedText = value; } }
		public string TableColumnTitleText { get { return this.tableColumnTitleText; } set { this.tableColumnTitleText = value; } }
		public string TableColumnVisibleText { get { return this.tableColumnVisibleText; } set { this.tableColumnVisibleText = value; } }
		public string TableValueEffectivelyVisibleTrueText { get { return this.tableValueEffectivelyVisibleTrueText; } set { this.tableValueEffectivelyVisibleTrueText = value; } }
		public string TableValueEffectivelyVisibleFalseText { get { return this.tableValueEffectivelyVisibleFalseText; } set { this.tableValueEffectivelyVisibleFalseText = value; } }
		public string TableValueVisibleTrueText { get { return this.tableValueVisibleTrueText; } set { this.tableValueVisibleTrueText = value; } }
		public string TableValueVisibleFalseText { get { return this.tableValueVisibleFalseText; } set { this.tableValueVisibleFalseText = value; } }
		public string NumberOfItemsSelectorLabelText { get { return this.numberOfItemsSelectorLabelText; } set { this.numberOfItemsSelectorLabelText = value; } }
		public string NumberOfItemsSelectorAllItemText { get { return this.numberOfItemsSelectorAllItemText; } set { this.numberOfItemsSelectorAllItemText = value; } }
		public string SearchTextBoxLabelText { get { return this.searchTextBoxLabelText; } set { this.searchTextBoxLabelText = value; } }
		public string AddPostButtonText { get { return this.addPostButtonText; } set { this.addPostButtonText = value; } }
		public string SubmitButtonText { get { return this.submitButtonText; } set { this.submitButtonText = value; } }
		public string PagingResultText { get { return ctrlPagedListing.PagingResultText; } set { ctrlPagedListing.PagingResultText = value; } }
		public string PagingNumberOfPagesText { get { return ctrlPagedListing.PagingNumberOfPagesText; } set { ctrlPagedListing.PagingNumberOfPagesText = value; } }
		public string PagingPreviousPageText { get { return ctrlPagedListing.PagingPreviousPageText; } set { ctrlPagedListing.PagingPreviousPageText = value; } }
		public string PagingNextPageText { get { return ctrlPagedListing.PagingNextPageText; } set { ctrlPagedListing.PagingNextPageText = value; } }
		public string PagingNextIncrementText { get { return ctrlPagedListing.PagingNextIncrementText; } set { ctrlPagedListing.PagingNextIncrementText = value; } }
		public string PagingPreviousIncrementText { get { return ctrlPagedListing.PagingPreviousIncrementText; } set { ctrlPagedListing.PagingPreviousIncrementText = value; } }
		#endregion

		#region Event Handlers
		private void PagedListingAfterDataBinding(object sender, EventArgs e) {
			WebParagraph ctrlMessageNoPosts = (WebParagraph)this.foot.FindControl("ctrlMessageNoPosts");
			ctrlMessageNoPosts.EnableViewState = false;
			Control ctrlTableTop = this.head.FindControl("ctrlTableTop");
			Control ctrlTableBottom = this.foot.FindControl("ctrlTableBottom");
			if(ctrlPagedListing.VisibleItems.Equals(0)) {
				ctrlMessageNoPosts.InnerText = this.MessageNoPostsText;
				ctrlMessageNoPosts.Visible = !(ctrlTableTop.Visible = ctrlTableBottom.Visible = false);
			} else {
				ctrlMessageNoPosts.Visible = !(ctrlTableTop.Visible = ctrlTableBottom.Visible = true);
			}
		}
		private void PagedListingListItemDataBinding(object sender, ListItemEventArgs e) {
			FlatTabularPostListItem listItem = (FlatTabularPostListItem)e.ListItem;
			listItem.UrlWithoutTrailingPostId = this.UrlWithoutTrailingPostId;
			listItem.UrlWithoutTrailingUserIdentity = this.UrlWithoutTrailingUserIdentity;
			listItem.UrlWithoutTrailingCategoryId = this.UrlWithoutTrailingCategoryId;
			listItem.UrlWithoutTrailingRootPostId = this.UrlWithoutTrailingRootPostId;
			listItem.OmissionIndicator = this.OmissionIndicator;
			listItem.MaximumBodyLength = this.MaximumBodyLength;
			listItem.MaximumTitleLength = this.MaximumTitleLength;
			listItem.MaximumRootPostTitleLength = this.MaximumRootPostTitleLength;
			listItem.AuthorResolver = this.AuthorResolver;
			listItem.DateFormatter = this.DateFormatter;
			listItem.TableColumnAuthorVisible = this.TableColumnAuthorVisible;
			listItem.TableColumnBodyVisible = this.TableColumnBodyVisible;
			listItem.TableColumnCategoryVisible = this.TableColumnCategoryVisible;
			listItem.TableColumnEffectivelyVisibleVisible = this.TableColumnEffectivelyVisibleVisible;
			listItem.TableColumnPostCountVisible = this.TableColumnPostCountVisible;
			listItem.TableColumnRootPostTitleVisible = this.TableColumnRootPostTitleVisible;
			listItem.TableColumnSearchRelevanceVisible = this.TableColumnSearchRelevanceVisible;
			listItem.TableColumnTimeLastPostVisible = this.TableColumnTimeLastPostVisible;
			listItem.TableColumnTimePostedVisible = this.TableColumnTimePostedVisible;
			listItem.TableColumnTitleVisible = this.TableColumnTitleVisible;
			listItem.TableColumnVisibleVisible = this.TableColumnVisibleVisible;
			listItem.TableValueEffectivelyVisibleTrueText = this.TableValueEffectivelyVisibleTrueText;
			listItem.TableValueEffectivelyVisibleFalseText = this.TableValueEffectivelyVisibleFalseText;
			listItem.TableValueVisibleTrueText = this.TableValueVisibleTrueText;
			listItem.TableValueVisibleFalseText = this.TableValueVisibleFalseText;
			listItem.EnableViewState = false;
		}
		private void AuthorClick(object sender, EventArgs e) {
			SortOnProperty(Post.Property.Author);
		}
		private void BodyClick(object sender, EventArgs e) {
			SortOnProperty(Post.Property.Body);
		}
		private void CategoryClick(object sender, EventArgs e) {
			SortOnProperty(Post.Property.EffectiveCategoryName);
		}
		private void EffectivelyVisibleClick(object sender, EventArgs e) {
			SortOnProperty(Post.Property.EffectivelyVisible);
		}
		private void PostCountClick(object sender, EventArgs e) {
			SortOnProperty(Post.Property.PostCount);
		}
		private void RootPostTitleClick(object sender, EventArgs e) {
			SortOnProperty(Post.Property.RootPostTitle);
		}
		private void SearchRelevanceClick(object sender, EventArgs e) {
			SortOnProperty(Post.Property.SearchRelevance);
		}
		private void TimeLastPostClick(object sender, EventArgs e) {
			SortOnProperty(Post.Property.TimeLastPost);
		}
		private void TimePostedClick(object sender, EventArgs e) {
			SortOnProperty(Post.Property.TimePosted);
		}
		private void TitleClick(object sender, EventArgs e) {
			SortOnProperty(Post.Property.Title);
		}
		private void VisibleClick(object sender, EventArgs e) {
			SortOnProperty(Post.Property.Visible);
		}
		private void AddPostClick(object sender, EventArgs e) {
			if(this.PostId != null) {
				OnAddPost(new AddPostEventArgs(this.PostId));
			} else if(this.CategoryId != null) {
				OnAddPost(new AddPostEventArgs(this.CategoryId));
			} else if(this.Domain != null && this.CategoryName != null) {
				OnAddPost(new AddPostEventArgs(this.Domain, this.CategoryName));
			}
		}
		private void SearchTextChanged(object sender, EventArgs e) {
			this.changedSearchTextBox = (TextBox)sender;
		}
		private void SubmitClick(object sender, EventArgs e) {
			if(this.changedSearchTextBox != null &&
				((this.changedSearchTextBox.Equals(txtHeaderSearch) && sender.Equals(btnHeaderSubmit)) ||
				(this.changedSearchTextBox.Equals(txtFooterSearch) && sender.Equals(btnFooterSubmit)))) {
				this.SearchText = this.changedSearchTextBox.Text == null || this.changedSearchTextBox.Text.TrimEnd().Length.Equals(0) ? null : this.changedSearchTextBox.Text;
				if(this.SearchText == null) {
					OnSearchDiscarded(EventArgs.Empty);
				} else {
					OnSearch(new SearchEventArgs(this.SearchText));
				}
			}
		}
		#endregion

		#region Event Activators
		protected virtual void OnAddPost(AddPostEventArgs e) {
			AddPostEventHandler eventHandler = base.Events[EventAddPost] as AddPostEventHandler;
			if(eventHandler != null) {
				eventHandler(this, e);
			}
		}
		protected virtual void OnSearch(SearchEventArgs e) {
			SearchEventHandler eventHandler = base.Events[EventSearch] as SearchEventHandler;
			if(eventHandler != null) {
				eventHandler(this, e);
			}
		}
		protected virtual void OnSearchDiscarded(EventArgs e) {
			SearchDiscardedEventHandler eventHandler = base.Events[EventSearchDiscarded] as SearchDiscardedEventHandler;
			if(eventHandler != null) {
				eventHandler(this, e);
			}
		}
		protected virtual void OnPostsSorting(PostsSortEventArgs e) {
			PostsSortingEventHandler eventHandler = base.Events[EventPostsSorting] as PostsSortingEventHandler;
			if(eventHandler != null) {
				eventHandler(this, e);
			}
		}
		#endregion

		#region Overridden Methods
		protected override void OnInit(EventArgs e) {
			this.head = ctrlPagedListing.HeaderControl;
			this.foot = ctrlPagedListing.FooterControl;

			txtHeaderSearch = (TextBox)this.head.FindControl("txtHeaderSearch");
			txtFooterSearch = (TextBox)this.foot.FindControl("txtFooterSearch");
			lbtnAuthor = (WebLinkButton)this.head.FindControl("lbtnAuthor");
			lbtnBody = (WebLinkButton)this.head.FindControl("lbtnBody");
			lbtnCategory = (WebLinkButton)this.head.FindControl("lbtnCategory");
			lbtnEffectivelyVisible = (WebLinkButton)this.head.FindControl("lbtnEffectivelyVisible");
			lbtnPostCount = (WebLinkButton)this.head.FindControl("lbtnPostCount");
			lbtnRootPostTitle = (WebLinkButton)this.head.FindControl("lbtnRootPostTitle");
			lbtnSearchRelevance = (WebLinkButton)this.head.FindControl("lbtnSearchRelevance");
			lbtnTimeLastPost = (WebLinkButton)this.head.FindControl("lbtnTimeLastPost");
			lbtnTimePosted = (WebLinkButton)this.head.FindControl("lbtnTimePosted");
			lbtnTitle = (WebLinkButton)this.head.FindControl("lbtnTitle");
			lbtnVisible = (WebLinkButton)this.head.FindControl("lbtnVisible");
			btnHeaderAddPost = (WebButton)this.head.FindControl("btnHeaderAddPost");
			btnFooterAddPost = (WebButton)this.foot.FindControl("btnFooterAddPost");
			btnHeaderSubmit = (WebButton)this.head.FindControl("btnHeaderSubmit");
			btnFooterSubmit = (WebButton)this.foot.FindControl("btnFooterSubmit");

			btnHeaderAddPost.EnableViewState = btnFooterAddPost.EnableViewState =
				btnHeaderSubmit.EnableViewState = btnFooterSubmit.EnableViewState = false;

			lbtnAuthor.Click += this.AuthorClick;
			lbtnBody.Click += this.BodyClick;
			lbtnCategory.Click += this.CategoryClick;
			lbtnEffectivelyVisible.Click += this.EffectivelyVisibleClick;
			lbtnPostCount.Click += this.PostCountClick;
			lbtnRootPostTitle.Click += this.RootPostTitleClick;
			lbtnSearchRelevance.Click += this.SearchRelevanceClick;
			lbtnTimeLastPost.Click += this.TimeLastPostClick;
			lbtnTimePosted.Click += this.TimePostedClick;
			lbtnTitle.Click += this.TitleClick;
			lbtnVisible.Click += this.VisibleClick;
			EventHandler addPostEventHandler = this.AddPostClick;
			EventHandler submitEventHandler = this.SubmitClick;
			EventHandler textSearchEventHandler = this.SearchTextChanged;
			btnHeaderAddPost.Click += addPostEventHandler;
			btnFooterAddPost.Click += addPostEventHandler;
			btnHeaderSubmit.Click += submitEventHandler;
			btnFooterSubmit.Click += submitEventHandler;
			txtHeaderSearch.TextChanged += textSearchEventHandler;
			txtFooterSearch.TextChanged += textSearchEventHandler;

			ctrlPagedListing.AfterDataBinding += this.PagedListingAfterDataBinding;
			ctrlPagedListing.ListItemDataBinding += this.PagedListingListItemDataBinding;

			base.OnInit(e);
		}
		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender(e);
			if(this.SortingEnabled) {
				SetSortingCss(this.SortProperty);
			}
			Paging ctrlHeaderPaging = (Paging)this.head.FindControl("ctrlHeaderPaging");
			Paging ctrlFooterPaging = (Paging)this.head.FindControl("ctrlFooterPaging");
			SetVisibility(this.PagingVisible, this.PagingPosition, ctrlHeaderPaging, ctrlFooterPaging);
			if(this.PagingVisible) {
				ctrlHeaderPaging.Type = ctrlFooterPaging.Type = this.PagingType;
			}
			//do not show add post button if we are searching and when we do not have a context where we can add a post 
			//(fetching all posts or from domain or /*searching or filtering on attributes*/)
			bool realAddPostButtonVisible = this.AddPostButtonVisible &&
				/*this.SearchText == null && this.UserIdentity == null && this.AttributeCriteria.Count.Equals(0) && */
				!((this.PostId == null && this.CategoryId == null && this.Domain == null && this.CategoryName == null) ||
				(this.Domain != null && this.CategoryName == null));
			Control ctrlHeaderAddPost = this.head.FindControl("ctrlHeaderAddPost");
			Control ctrlFooterAddPost = this.foot.FindControl("ctrlFooterAddPost");
			SetVisibility(realAddPostButtonVisible, this.AddPostButtonPosition, ctrlHeaderAddPost, ctrlFooterAddPost);
			ctrlHeaderAddPost.EnableViewState = ctrlFooterAddPost.EnableViewState = false;
			if(realAddPostButtonVisible) {
				btnHeaderAddPost.InnerText = btnFooterAddPost.InnerText = this.AddPostButtonText;
			}
			Control ctrlHeaderSearch = this.head.FindControl("ctrlHeaderSearch");
			Control ctrlFooterSearch = this.foot.FindControl("ctrlFooterSearch");
			SetVisibility(this.SearchTextBoxVisible, this.SearchTextBoxPosition, ctrlHeaderSearch, ctrlFooterSearch);
			if(this.SearchTextBoxVisible) {
				WebLabel lblHeaderSearch = (WebLabel)this.head.FindControl("lblHeaderSearch");
				WebLabel lblFooterSearch = (WebLabel)this.foot.FindControl("lblFooterSearch");
				lblHeaderSearch.EnableViewState = lblFooterSearch.EnableViewState = false;
				lblHeaderSearch.InnerText = lblFooterSearch.InnerText = this.SearchTextBoxLabelText;
				txtHeaderSearch.Text = txtFooterSearch.Text = this.SearchText;
			}
			Control ctrlHeaderNumberOfItems = this.head.FindControl("ctrlHeaderNumberOfItems");
			bool realNumberOfItemSelectorAutoPostBack = ctrlHeaderSearch.Visible ? false : this.NumberOfItemsSelectorAutoPostBack;
			if(this.NumberOfItemsSelectorVisible) {
				ctrlHeaderNumberOfItems.Visible = ctrlHeaderNumberOfItems.Parent.Visible = true;
				WebLabel lblHeaderNumberOfItemsSelector = (WebLabel)this.head.FindControl("lblHeaderNumberOfItemsSelector");
				lblHeaderNumberOfItemsSelector.EnableViewState = false;
				lblHeaderNumberOfItemsSelector.InnerText = this.NumberOfItemsSelectorLabelText;
				NumberOfItemsSelector ctrlHeaderNumberOfItemsSelector = (NumberOfItemsSelector)this.head.FindControl("ctrlHeaderNumberOfItemsSelector");
				ctrlHeaderNumberOfItemsSelector.AutoPostBack = realNumberOfItemSelectorAutoPostBack;
				ctrlHeaderNumberOfItemsSelector.IncludeAllItem = this.NumberOfItemsSelectorIncludeAllItem;
				ctrlHeaderNumberOfItemsSelector.AllItemText = this.NumberOfItemsSelectorAllItemText;
			} else {
				ctrlHeaderNumberOfItems.Visible = false;
			}
			WebPlaceHolder ctrlHeaderNoScript = (WebPlaceHolder)this.head.FindControl("ctrlHeaderNoScript");
			WebPlaceHolder ctrlFooterNoScript = (WebPlaceHolder)this.foot.FindControl("ctrlFooterNoScript");
			ctrlHeaderNoScript.EnableViewState = ctrlFooterNoScript.EnableViewState = false;
			if(ctrlHeaderNoScript.Visible = ctrlHeaderSearch.Visible || ctrlHeaderNumberOfItems.Visible) {
				((WebButton)this.head.FindControl("btnHeaderSubmit")).InnerText = this.SubmitButtonText;
			}
			if(ctrlFooterNoScript.Visible = ctrlFooterSearch.Visible) {
				((WebButton)this.foot.FindControl("btnFooterSubmit")).InnerText = this.SubmitButtonText;
			}
			ctrlHeaderNoScript.SurroundingTag = realNumberOfItemSelectorAutoPostBack ? HtmlTag.Noscript : null;
			ctrlFooterNoScript.SurroundingTag = ctrlFooterSearch.Visible ? null : HtmlTag.Noscript;
			if(!ctrlHeaderNoScript.Visible && !ctrlHeaderAddPost.Visible) {
				ctrlHeaderNoScript.Parent.Visible = false;
			}
			if(!ctrlFooterNoScript.Visible && !ctrlFooterAddPost.Visible) {
				ctrlFooterNoScript.Parent.Visible = false;
			}
			if(this.head.FindControl("ctrlHeaderFieldSet").Visible) {
				WebGenericControl ctrlHeaderFieldSetLabel = (WebGenericControl)this.head.FindControl("ctrlHeaderFieldSetLabel");
				if(ctrlHeaderFieldSetLabel.Visible = this.FieldSetLabelText != null && !this.FieldSetLabelText.Length.Equals(0)) {
					ctrlHeaderFieldSetLabel.EnableViewState = false;
					ctrlHeaderFieldSetLabel.InnerText = this.FieldSetLabelText;
				}
			}
			if(this.foot.FindControl("ctrlFooterFieldSet").Visible) {
				WebGenericControl ctrlFooterFieldSetLabel = (WebGenericControl)this.foot.FindControl("ctrlFooterFieldSetLabel");
				if(ctrlFooterFieldSetLabel.Visible = this.FieldSetLabelText != null && !this.FieldSetLabelText.Length.Equals(0)) {
					ctrlFooterFieldSetLabel.EnableViewState = false;
					ctrlFooterFieldSetLabel.InnerText = this.FieldSetLabelText;
				}
			}
			if(!string.IsNullOrEmpty(this.TableCaptionText)) {
				WebPlaceHolder cptTableCaption = (WebPlaceHolder)this.head.FindControl("cptTableCaption");
				cptTableCaption.EnableViewState = false;
				cptTableCaption.SurroundingTag = HtmlTag.Caption;
				cptTableCaption.InnerText = this.TableCaptionText;
			}
			Control ctrlHeaderColumnHeaders = this.head.FindControl("ctrlHeaderColumnHeaders");
			if(ctrlHeaderColumnHeaders.Visible = this.TableColumnHeadersVisible) {
				ConfigureTableColumn((WebGenericControl)this.head.FindControl("ctrlHeaderAuthor"), lbtnAuthor, this.TableColumnAuthorVisible, this.SortingEnabled, this.TableColumnAuthorText);
				ConfigureTableColumn((WebGenericControl)this.head.FindControl("ctrlHeaderBody"), lbtnBody, this.TableColumnBodyVisible, this.SortingEnabled, this.TableColumnBodyText);
				ConfigureTableColumn((WebGenericControl)this.head.FindControl("ctrlHeaderCategory"), lbtnCategory, this.TableColumnCategoryVisible, this.SortingEnabled, this.TableColumnCategoryText);
				ConfigureTableColumn((WebGenericControl)this.head.FindControl("ctrlHeaderEffectivelyVisible"), lbtnEffectivelyVisible, this.TableColumnEffectivelyVisibleVisible, this.SortingEnabled, this.TableColumnEffectivelyVisibleText);
				ConfigureTableColumn((WebGenericControl)this.head.FindControl("ctrlHeaderPostCount"), lbtnPostCount, this.TableColumnPostCountVisible, this.SortingEnabled, this.TableColumnPostCountText);
				ConfigureTableColumn((WebGenericControl)this.head.FindControl("ctrlHeaderRootPostTitle"), lbtnRootPostTitle, this.TableColumnRootPostTitleVisible, this.SortingEnabled, this.TableColumnRootPostTitleText);
				ConfigureTableColumn((WebGenericControl)this.head.FindControl("ctrlHeaderSearchRelevance"), lbtnSearchRelevance, this.TableColumnSearchRelevanceVisible, this.SortingEnabled, this.TableColumnSearchRelevanceText);
				ConfigureTableColumn((WebGenericControl)this.head.FindControl("ctrlHeaderTimeLastPost"), lbtnTimeLastPost, this.TableColumnTimeLastPostVisible, this.SortingEnabled, this.TableColumnTimeLastPostText);
				ConfigureTableColumn((WebGenericControl)this.head.FindControl("ctrlHeaderTimePosted"), lbtnTimePosted, this.TableColumnTimePostedVisible, this.SortingEnabled, this.TableColumnTimePostedText);
				ConfigureTableColumn((WebGenericControl)this.head.FindControl("ctrlHeaderTitle"), lbtnTitle, this.TableColumnTitleVisible, this.SortingEnabled, this.TableColumnTitleText);
				ConfigureTableColumn((WebGenericControl)this.head.FindControl("ctrlHeaderVisible"), lbtnVisible, this.TableColumnVisibleVisible, this.SortingEnabled, this.TableColumnVisibleText);
			} else {
				ctrlHeaderColumnHeaders.EnableViewState = false;
			}
			if(!this.dataBound) {
				this.DataBind();
			}
		}
		public override void DataBind() {
			//the looping here rests on the behaviour of the PagedListing automatically adjusting CurrentPageNumber when it is out of bound; 
			//if we find that we have zero items in the list, then loop until we have items, but stop if we are at page one.
			do {
				if(this.PostCollection != null && !this.PostCollection.Count.Equals(0)) {
					ctrlPagedListing.ListCollection = this.PostCollection;
				} else {
					ThreadedPostsResultConfiguration threadedResultConfiguration = null;
					PostsResultConfiguration resultConfiguration = null;
					PagingConfiguration pagingConfiguration = new PagingConfiguration(this.NumberOfItems, ctrlPagedListing.CurrentPageNumber);
					SearchCriteria searchCriteria = this.SearchText == null && this.UserIdentity == null && this.AttributeCriteria.Count.Equals(0) ? null : new SearchCriteria(this.SearchText, this.UserIdentity, this.AttributeCriteria);
					if(searchCriteria == null) {
						threadedResultConfiguration = new ThreadedPostsResultConfiguration(1, this.IncludeHidden, this.LoadAttributes, this.SortProperty, this.SortAttributeName, this.SortAscending, this.DateFilterFrom, this.DateFilterTo, this.DateFilterProperty);
					} else {
						resultConfiguration = new PostsResultConfiguration(this.IncludeHidden, this.LoadAttributes, this.SortProperty, this.SortAttributeName, this.SortAscending, this.DateFilterFrom, this.DateFilterTo, this.DateFilterProperty);
					}
					PagedPostsResult pagedResult;
					if(this.PostId != null) {
						pagedResult = searchCriteria == null ?
							this.ForumDao.GetPosts(this.PostId, this.PostType, threadedResultConfiguration, pagingConfiguration) :
							this.ForumDao.SearchPosts(this.PostId, this.PostType, searchCriteria, resultConfiguration, pagingConfiguration);
					} else if(this.CategoryId != null) {
						pagedResult = searchCriteria == null ?
							this.ForumDao.GetPosts(this.CategoryId, threadedResultConfiguration, pagingConfiguration) :
							this.ForumDao.SearchPosts(this.CategoryId, searchCriteria, resultConfiguration, pagingConfiguration);
					} else if(this.Domain != null) {
						pagedResult = this.CategoryName == null ?
							searchCriteria == null ?
								this.ForumDao.GetPosts(this.Domain, threadedResultConfiguration, pagingConfiguration) :
								this.ForumDao.SearchPosts(this.Domain, searchCriteria, resultConfiguration, pagingConfiguration) :
							searchCriteria == null ?
								this.ForumDao.GetPosts(this.Domain, this.CategoryName, threadedResultConfiguration, pagingConfiguration) :
								this.ForumDao.SearchPosts(this.Domain, this.CategoryName, searchCriteria, resultConfiguration, pagingConfiguration);
					} else {
						pagedResult = searchCriteria == null ?
							this.ForumDao.GetPosts(threadedResultConfiguration, pagingConfiguration) :
							this.ForumDao.SearchPosts(searchCriteria, resultConfiguration, pagingConfiguration);
					}
					ctrlPagedListing.DisablePaging = true;
					ctrlPagedListing.TotalNumberOfItems = pagedResult.TotalCount;
					ctrlPagedListing.ListCollection = pagedResult.Posts;
				}
				ctrlPagedListing.DataBind();
			} while(ctrlPagedListing.ListCollection.Count.Equals(0) && this.ctrlPagedListing.CurrentPageNumber > 1);
			this.dataBound = true;
		}
		#endregion

		#region Helper Methods
		private WebLinkButton LinkButtonFromProperty(Post.Property property) {
			switch(property) {
				case Post.Property.Author:
				return lbtnAuthor;
				case Post.Property.Body:
				return lbtnBody;
				case Post.Property.EffectiveCategoryName:
				return lbtnCategory;
				case Post.Property.EffectivelyVisible:
				return lbtnEffectivelyVisible;
				case Post.Property.RootPostTitle:
				return lbtnRootPostTitle;
				case Post.Property.PostCount:
				return lbtnPostCount;
				case Post.Property.SearchRelevance:
				return lbtnSearchRelevance;
				case Post.Property.TimeLastPost:
				return lbtnTimeLastPost;
				case Post.Property.TimePosted:
				return lbtnTimePosted;
				case Post.Property.Title:
				return lbtnTitle;
				case Post.Property.Visible:
				return lbtnVisible;
				default:
				return null;
			}
		}
		private void SortOnProperty(Post.Property property) {
			PostsSortEventArgs sortEventArgs = new PostsSortEventArgs(property, null, this.SortProperty.Equals(property) ? !this.SortAscending : DefaultSortascending);
			OnPostsSorting(sortEventArgs);
			this.SortAscending = sortEventArgs.Ascending;
			this.SortProperty = sortEventArgs.Property;
			this.SortAttributeName = sortEventArgs.AttributeName;
		}
		private void SetSortingCss(Post.Property property) {
			const string SPACE = " ";

			lbtnAuthor.CssClass = lbtnBody.CssClass = lbtnCategory.CssClass = lbtnEffectivelyVisible.CssClass =
				lbtnPostCount.CssClass = lbtnRootPostTitle.CssClass = lbtnSearchRelevance.CssClass = lbtnTimeLastPost.CssClass =
				lbtnTimePosted.CssClass = lbtnTitle.CssClass = lbtnVisible.CssClass = string.Empty;
			if(this.SortAttributeName == null) {
				WebLinkButton linkButton = LinkButtonFromProperty(property);
				if(linkButton != null) {
					if(linkButton.CssClass.Length > 0) {
						linkButton.CssClass += SPACE;
					}
					linkButton.CssClass += this.SortAscending ? CssclassAscending : CssclassDescending;
				}
			}
		}
		private static void SetVisibility(bool visible, Position position, Control headerControl, Control footerControl) {
			if(visible) {
				switch(position) {
					case Position.Above:
					headerControl.Visible = headerControl.Parent.Visible = !(footerControl.Visible = false);
					break;
					case Position.Below:
					footerControl.Visible = footerControl.Parent.Visible = !(headerControl.Visible = false);
					break;
					case Position.AboveAndBelow:
					footerControl.Visible = footerControl.Parent.Visible =
						headerControl.Visible = headerControl.Parent.Visible = true;
					break;
				}
			} else {
				headerControl.Visible = footerControl.Visible = false;
			}
		}
		private static void ConfigureTableColumn(WebGenericControl tableColumn, WebLinkButton tableColumnSorter, bool visible, bool sortingEnabled, string text) {
			if(tableColumn.Visible = visible) {
				if(tableColumnSorter.Visible = sortingEnabled) {
					tableColumnSorter.Text = text;
				} else {
					tableColumn.InnerText = text;
				}
			}
			tableColumn.EnableViewState = false;
		}
		#endregion
	}

}