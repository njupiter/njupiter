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
using System.Web.UI;
using System.Web.UI.WebControls;

using nJupiter.Web.UI;
using nJupiter.Web.UI.Controls;
using nJupiter.Web.UI.Controls.Listings;

namespace nJupiter.Services.Forum.UI.Web {

	public class FlatTabularPostList : UserControl {
		#region Constants
		private const Post.Property			DEFAULT_SORTPROPERTY							= Post.Property.TimeLastPost;
		private const string				DEFAULT_SORTATTRIBUTENAME						= null;
		private const bool					DEFAULT_SORTASCENDING							= true;
		private const bool					DEFAULT_INCLUDEHIDDEN							= false;
		private const bool					DEFAULT_LOADATTRIBUTES							= false;
		private const int					DEFAULT_MAXIMUMBODYLENGTH						= 255;
		private const int					DEFAULT_MAXIMUMTITLELENGTH						= 255;
		private const int					DEFAULT_MAXIMUMROOTPOSTTITLELENGTH				= 255;
		private static readonly	DateTime	DEFAULT_DATEFILTERFROM							= DateTime.MinValue;
		private static readonly	DateTime	DEFAULT_DATEFILTERTO							= DateTime.MaxValue;
		private const Post.DateProperty		DEFAULT_DATEFILTERPROPERTY						= Post.DateProperty.TimePosted;

		private const bool					DEFAULT_NUMBEROFITEMSSELECTORVISIBLE			= true;
		private const bool					DEFAULT_NUMBEROFITEMSSELECTORINCLUDEALLITEM		= false;
		private const bool					DEFAULT_SEARCHTEXTBOXVISIBLE					= true;
		private const bool					DEFAULT_ADDPOSTBUTTONVISIBLE					= true;
		private const bool					DEFAULT_PAGINGVISIBLE							= true;
		private const bool					DEFAULT_TABLECOLUMNHEADERSVISIBLE				= true;
		private const bool					DEFAULT_TABLECOLUMNAUTHORVISIBLE				= true;
		private const bool					DEFAULT_TABLECOLUMNBODYVISIBLE					= false;
		private const bool					DEFAULT_TABLECOLUMNCATEGORYVISIBLE				= true;
		private const bool					DEFAULT_TABLECOLUMNEFFECTIVELYVISIBLEVISIBLE	= false;
		private const bool					DEFAULT_TABLECOLUMNPOSTCOUNTVISIBLE				= true;
		private const bool					DEFAULT_TABLECOLUMNROOTPOSTTITLEVISIBLE			= false;
		private const bool					DEFAULT_TABLECOLUMNSEARCHRELEVANCEVISIBLE		= false;
		private const bool					DEFAULT_TABLECOLUMNTIMELASTPOSTVISIBLE			= true;
		private const bool					DEFAULT_TABLECOLUMNTIMEPOSTEDVISIBLE			= true;
		private const bool					DEFAULT_TABLECOLUMNTITLEVISIBLE					= true;
		private const bool					DEFAULT_TABLECOLUMNVISIBLEVISIBLE				= false;
		
		private const Position				DEFAULT_SEARCHTEXTBOXPOSITION					= Position.AboveAndBelow;
		private const Position				DEFAULT_ADDPOSTBUTTONPOSITION					= Position.AboveAndBelow;
		private const Position				DEFAULT_PAGINGPOSITION							= Position.AboveAndBelow;
		
		private const bool					DEFAULT_NUMBEROFITEMSSELECTORAUTOPOSTBACK		= true;
		private const bool					DEFAULT_SORTINGENABLED							= true;
		private const Paging.PagingType		DEFAULT_PAGINGTYPE								= Paging.PagingType.Buttons;

#if DEBUG
		private const string				DEBUG_PREFIX									= "_";
#else
		private const string				DEBUG_PREFIX									= "";
#endif
		private const string				DEFAULT_OMISSIONINDICATOR						= "\u2026";
		private const string				DEFAULT_TABLECAPTIONTEXT						= DEBUG_PREFIX + "Listing of posts";
		private const string				DEFAULT_TABLECOLUMNAUTHORTEXT					= DEBUG_PREFIX + "Author";
		private const string				DEFAULT_TABLECOLUMNBODYTEXT						= DEBUG_PREFIX + "Text";
		private const string				DEFAULT_TABLECOLUMNCATEGORYTEXT					= DEBUG_PREFIX + "Category";
		private const string				DEFAULT_TABLECOLUMNEFFECTIVELYVISIBLETEXT		= DEBUG_PREFIX + "Effectively visible";
		private const string				DEFAULT_TABLECOLUMNPOSTCOUNTTEXT				= DEBUG_PREFIX + "Replies";
		private const string				DEFAULT_TABLECOLUMNROOTPOSTTITLETEXT			= DEBUG_PREFIX + "Thread title";
		private const string				DEFAULT_TABLECOLUMNSEARCHRELEVANCETEXT			= DEBUG_PREFIX + "Relevance";
		private const string				DEFAULT_TABLECOLUMNTIMELASTPOSTTEXT				= DEBUG_PREFIX + "Last post";
		private const string				DEFAULT_TABLECOLUMNTIMEPOSTEDTEXT				= DEBUG_PREFIX + "Created";
		private const string				DEFAULT_TABLECOLUMNTITLETEXT					= DEBUG_PREFIX + "Title";
		private const string				DEFAULT_TABLECOLUMNVISIBLETEXT					= DEBUG_PREFIX + "Visible";
		private const string				DEFAULT_TABLEVALUEEFFECTIVELYVISIBLETRUETEXT	= DEBUG_PREFIX + "Yes";
		private const string				DEFAULT_TABLEVALUEEFFECTIVELYVISIBLEFALSETEXT	= DEBUG_PREFIX + "No";
		private const string				DEFAULT_TABLEVALUEVISIBLETRUETEXT				= DEFAULT_TABLEVALUEEFFECTIVELYVISIBLETRUETEXT;
		private const string				DEFAULT_TABLEVALUEVISIBLEFALSETEXT				= DEFAULT_TABLEVALUEEFFECTIVELYVISIBLEFALSETEXT;
		private	const string				DEFAULT_MESSAGENOPOSTSTEXT						= DEBUG_PREFIX + "No posts were found.";
		private const string				DEFAULT_FIELDSETLABELTEXT						= DEBUG_PREFIX + "Input fields";
        private const string				DEFAULT_SEARCHTEXTBOXLABELTEXT					= DEBUG_PREFIX + "Search for";
		private const string				DEFAULT_NUMBEROFITEMSSELECTORLABELTEXT			= DEBUG_PREFIX + "Number of posts per page";
		private const string				DEFAULT_NUMBEROFITEMSSELECTORALLITEMTEXT		= DEBUG_PREFIX + "All";
		private const string				DEFAULT_SUBMITBUTTONTEXT						= DEBUG_PREFIX + "Show";
		private const string				DEFAULT_ADDPOSTBUTTONTEXT						= DEBUG_PREFIX + "Add new post";

		private const string				VIEWSTATE_SORTPROPERTY							= "v_SortProperty";
		private const string				VIEWSTATE_SORTATTRIBUTENAME						= "v_SortAttributeName";
		private const string				VIEWSTATE_SORTASCENDING							= "v_SortAscending";
		private const string				VIEWSTATE_ADDPOSTBUTTONVISIBLE					= "v_AddPostButtonVisible";
		private const string				VIEWSTATE_SEARCHTEXT							= "v_SearchText";

		private const string				CSSCLASS_ASCENDING								= "column-sorted ascending";
		private const string				CSSCLASS_DESCENDING								= "column-sorted descending";
		#endregion

		#region Variables
		private ForumDao						m_ForumDao;

		private TextBox							m_ChangedSearchTextBox;
		private WebPlaceHolder					m_Head;
		private WebPlaceHolder					m_Foot;
		private bool							m_DataBound;
		
		private string							m_UrlWithoutTrailingPostId;
		private string							m_UrlWithoutTrailingUserIdentity;
		private string							m_UrlWithoutTrailingCategoryId;
		private string							m_UrlWithoutTrailingRootPostId;
		private DateFormatter					m_DateFormatter;
		private AuthorResolver					m_AuthorResolver;

		private string							m_Domain;
		private string							m_CategoryName;
		private CategoryId						m_CategoryId;
		private PostId							m_PostId;
		private PostType						m_PostType;
		private string							m_UserIdentity;
		private bool							m_IncludeHidden							= DEFAULT_INCLUDEHIDDEN;
		private bool							m_LoadAttributes						= DEFAULT_LOADATTRIBUTES;
		private int								m_MaximumBodyLength						= DEFAULT_MAXIMUMBODYLENGTH;
		private int								m_MaximumTitleLength					= DEFAULT_MAXIMUMTITLELENGTH;
		private int								m_MaximumRootPostTitleLength			= DEFAULT_MAXIMUMROOTPOSTTITLELENGTH;
		private DateTime						m_DateFilterFrom						= DEFAULT_DATEFILTERFROM;
		private DateTime						m_DateFilterTo							= DEFAULT_DATEFILTERTO;
		private Post.DateProperty				m_DateFilterProperty					= DEFAULT_DATEFILTERPROPERTY;
		private AttributeCriterionCollection	m_AttributeCriteria;
		private PostCollection					m_PostCollection;

		private bool							m_NumberOfItemsSelectorVisible			= DEFAULT_NUMBEROFITEMSSELECTORVISIBLE;
		private bool							m_NumberOfItemsSelectorIncludeAllItem	= DEFAULT_NUMBEROFITEMSSELECTORINCLUDEALLITEM;
		private bool							m_SearchTextBoxVisible					= DEFAULT_SEARCHTEXTBOXVISIBLE;
		private bool							m_PagingVisible							= DEFAULT_PAGINGVISIBLE;
		private bool							m_TableColumnAuthorVisible				= DEFAULT_TABLECOLUMNAUTHORVISIBLE;
		private bool							m_TableColumnBodyVisible				= DEFAULT_TABLECOLUMNBODYVISIBLE;
		private bool							m_TableColumnCategoryVisible			= DEFAULT_TABLECOLUMNCATEGORYVISIBLE;
		private bool							m_TableColumnEffectivelyVisibleVisible	= DEFAULT_TABLECOLUMNEFFECTIVELYVISIBLEVISIBLE;
		private bool							m_TableColumnPostCountVisible			= DEFAULT_TABLECOLUMNPOSTCOUNTVISIBLE;
		private bool							m_TableColumnRootPostTitleVisible		= DEFAULT_TABLECOLUMNROOTPOSTTITLEVISIBLE;
		private bool							m_TableColumnSearchRelevanceVisible		= DEFAULT_TABLECOLUMNSEARCHRELEVANCEVISIBLE;
		private bool							m_TableColumnTimeLastPostVisible		= DEFAULT_TABLECOLUMNTIMELASTPOSTVISIBLE;
		private bool							m_TableColumnTimePostedVisible			= DEFAULT_TABLECOLUMNTIMEPOSTEDVISIBLE;
		private bool							m_TableColumnTitleVisible				= DEFAULT_TABLECOLUMNTITLEVISIBLE;
		private bool							m_TableColumnVisibleVisible				= DEFAULT_TABLECOLUMNVISIBLEVISIBLE;

		private Position						m_AddPostButtonPosition					= DEFAULT_ADDPOSTBUTTONPOSITION;
		private Position						m_SearchTextBoxPosition					= DEFAULT_SEARCHTEXTBOXPOSITION;
		private Position						m_PagingPosition						= DEFAULT_PAGINGPOSITION;

		private	bool							m_NumberOfItemsSelectorAutoPostBack		= DEFAULT_NUMBEROFITEMSSELECTORAUTOPOSTBACK;
		private bool							m_TableColumnHeadersVisible				= DEFAULT_TABLECOLUMNHEADERSVISIBLE;
		private bool							m_SortingEnabled						= DEFAULT_SORTINGENABLED;
		private Paging.PagingType				m_PagingType							= DEFAULT_PAGINGTYPE;

		private string							m_OmissionIndicator						= DEFAULT_OMISSIONINDICATOR;
		private string							m_FieldSetLabelText						= DEFAULT_FIELDSETLABELTEXT;
        private string							m_MessageNoPostsText					= DEFAULT_MESSAGENOPOSTSTEXT;
		private string							m_TableCaptionText						= DEFAULT_TABLECAPTIONTEXT;
		private string							m_TableColumnAuthorText					= DEFAULT_TABLECOLUMNAUTHORTEXT;
		private string							m_TableColumnBodyText					= DEFAULT_TABLECOLUMNBODYTEXT;
		private string							m_TableColumnCategoryText				= DEFAULT_TABLECOLUMNCATEGORYTEXT;
		private string							m_TableColumnEffectivelyVisibleText		= DEFAULT_TABLECOLUMNEFFECTIVELYVISIBLETEXT;
		private string							m_TableColumnPostCountText				= DEFAULT_TABLECOLUMNPOSTCOUNTTEXT;
		private string							m_TableColumnRootPostTitleText			= DEFAULT_TABLECOLUMNROOTPOSTTITLETEXT;
		private string							m_TableColumnSearchRelevanceText		= DEFAULT_TABLECOLUMNSEARCHRELEVANCETEXT;
		private string							m_TableColumnTimeLastPostText			= DEFAULT_TABLECOLUMNTIMELASTPOSTTEXT;
		private string							m_TableColumnTimePostedText				= DEFAULT_TABLECOLUMNTIMEPOSTEDTEXT;
		private string							m_TableColumnTitleText					= DEFAULT_TABLECOLUMNTITLETEXT;
		private string							m_TableColumnVisibleText				= DEFAULT_TABLECOLUMNVISIBLETEXT;
		private string							m_TableValueEffectivelyVisibleTrueText	= DEFAULT_TABLEVALUEEFFECTIVELYVISIBLETRUETEXT;
		private string							m_TableValueEffectivelyVisibleFalseText	= DEFAULT_TABLEVALUEEFFECTIVELYVISIBLEFALSETEXT;
		private string							m_TableValueVisibleTrueText				= DEFAULT_TABLEVALUEVISIBLETRUETEXT;
		private string							m_TableValueVisibleFalseText			= DEFAULT_TABLEVALUEVISIBLEFALSETEXT;
		private string							m_NumberOfItemsSelectorLabelText		= DEFAULT_NUMBEROFITEMSSELECTORLABELTEXT;
		private string							m_NumberOfItemsSelectorAllItemText		= DEFAULT_NUMBEROFITEMSSELECTORALLITEMTEXT;
		private string							m_SearchTextBoxLabelText				= DEFAULT_SEARCHTEXTBOXLABELTEXT;
		private string							m_SubmitButtonText						= DEFAULT_SUBMITBUTTONTEXT;
		private string							m_AddPostButtonText						= DEFAULT_ADDPOSTBUTTONTEXT;

		private static readonly object			s_EventAddPost							= new object();
		private static readonly object			s_EventSearch							= new object();
		private static readonly object			s_EventSearchDiscarded					= new object();
		private static readonly object			s_EventPostsSorting						= new object();
		#endregion

		#region Events
		public event AddPostEventHandler AddPost { 
			add { base.Events.AddHandler(s_EventAddPost, value); } 
			remove { base.Events.RemoveHandler(s_EventAddPost, value); } 
		}
		public event SearchEventHandler	Search { 
			add { base.Events.AddHandler(s_EventSearch, value); } 
			remove { base.Events.RemoveHandler(s_EventSearch, value); } 
		}
		public event SearchDiscardedEventHandler SearchDiscarded { 
			add { base.Events.AddHandler(s_EventSearchDiscarded, value); } 
			remove { base.Events.RemoveHandler(s_EventSearchDiscarded, value); } 
		}
		public event PostsSortingEventHandler PostsSorting {
			add { base.Events.AddHandler(s_EventPostsSorting, value); }
			remove { base.Events.RemoveHandler(s_EventPostsSorting, value); }
		}
		#endregion

		#region UI Members
		protected PagedListing			ctrlPagedListing;

		private TextBox					txtHeaderSearch;
		private TextBox					txtFooterSearch;
		private WebButton				btnHeaderAddPost;
		private WebButton				btnFooterAddPost;
		private WebButton				btnHeaderSubmit;
		private WebButton				btnFooterSubmit;
		private WebLinkButton			lbtnAuthor;
		private WebLinkButton			lbtnBody;
		private WebLinkButton			lbtnCategory;
		private WebLinkButton			lbtnEffectivelyVisible;
		private WebLinkButton			lbtnPostCount;
		private WebLinkButton			lbtnRootPostTitle;
		private WebLinkButton			lbtnSearchRelevance;
		private WebLinkButton			lbtnTimeLastPost;
		private WebLinkButton			lbtnTimePosted;
		private WebLinkButton			lbtnTitle;
		private WebLinkButton			lbtnVisible;
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
		public string SearchText { get { return (string)this.ViewState[VIEWSTATE_SEARCHTEXT]; } set { this.ViewState.Add(VIEWSTATE_SEARCHTEXT, value); } }
		public string UserIdentity { get { return m_UserIdentity; } set { m_UserIdentity = value; } }
		public bool IncludeHidden { get { return m_IncludeHidden; } set { m_IncludeHidden = value; } }
		public bool LoadAttributes { get { return m_LoadAttributes; } set { m_LoadAttributes = value; } }
		public int MaximumBodyLength { 
			get { return m_MaximumBodyLength; } 
			set { 
				if(value < 1) {
					throw new ArgumentOutOfRangeException("value");
				}
				m_MaximumBodyLength = value; 
			} 
		}
		public int MaximumTitleLength { 
			get { return m_MaximumTitleLength; } 
			set { 
				if(value < 1) {
					throw new ArgumentOutOfRangeException("value");
				}
				m_MaximumTitleLength = value; 
			} 
		}
		public int MaximumRootPostTitleLength {
			get { return m_MaximumRootPostTitleLength; }
			set {
				if(value < 1) {
					throw new ArgumentOutOfRangeException("value");
				}
				m_MaximumRootPostTitleLength = value; 
			}
		}
		public DateTime DateFilterFrom { get { return m_DateFilterFrom; } set { m_DateFilterFrom = value; } }
		public DateTime DateFilterTo { get { return m_DateFilterTo; } set { m_DateFilterTo = value; } }
		public Post.DateProperty DateFilterProperty { get { return m_DateFilterProperty; } set { m_DateFilterProperty = value; } }
		public AttributeCriterionCollection AttributeCriteria {	get { return this.m_AttributeCriteria ?? (this.m_AttributeCriteria = new AttributeCriterionCollection()); } }
		public PostCollection PostCollection { get { return m_PostCollection; } set { m_PostCollection = value; } }

		public string UrlWithoutTrailingPostId { get { return m_UrlWithoutTrailingPostId; } set { m_UrlWithoutTrailingPostId = value; } }
		public string UrlWithoutTrailingUserIdentity { get { return m_UrlWithoutTrailingUserIdentity; } set { m_UrlWithoutTrailingUserIdentity = value; } }
		public string UrlWithoutTrailingCategoryId { get { return m_UrlWithoutTrailingCategoryId; } set { m_UrlWithoutTrailingCategoryId = value; } }
		public string UrlWithoutTrailingRootPostId { get { return m_UrlWithoutTrailingRootPostId; } set { m_UrlWithoutTrailingRootPostId = value; } }
		public AuthorResolver AuthorResolver { get { return m_AuthorResolver; } set { m_AuthorResolver = value; } }
		public DateFormatter DateFormatter { get { return this.m_DateFormatter ?? (this.m_DateFormatter = new FriendlyDateFormatter()); } set { m_DateFormatter = value; } }

		public bool NumberOfItemsSelectorVisible { get { return m_NumberOfItemsSelectorVisible; } set { m_NumberOfItemsSelectorVisible = value; } }
		public bool NumberOfItemsSelectorIncludeAllItem { get { return m_NumberOfItemsSelectorIncludeAllItem; } set { m_NumberOfItemsSelectorIncludeAllItem = value; } }
		public bool SearchTextBoxVisible { get { return m_SearchTextBoxVisible; } set { m_SearchTextBoxVisible = value; } }
		public bool AddPostButtonVisible {	get { return (bool)(this.ViewState[VIEWSTATE_ADDPOSTBUTTONVISIBLE] == null ? this.ViewState.Add(VIEWSTATE_ADDPOSTBUTTONVISIBLE, DEFAULT_ADDPOSTBUTTONVISIBLE).Value : this.ViewState[VIEWSTATE_ADDPOSTBUTTONVISIBLE]); }	set { this.ViewState.Add(VIEWSTATE_ADDPOSTBUTTONVISIBLE, value); } }
		public bool PagingVisible { get { return m_PagingVisible; } set { m_PagingVisible = value; } }
		public bool TableColumnAuthorVisible { get { return m_TableColumnAuthorVisible; } set { m_TableColumnAuthorVisible = value; } }
		public bool TableColumnBodyVisible { get { return m_TableColumnBodyVisible; } set { m_TableColumnBodyVisible = value; } }
		public bool TableColumnCategoryVisible { get { return m_TableColumnCategoryVisible; } set { m_TableColumnCategoryVisible = value; } }
		public bool TableColumnEffectivelyVisibleVisible { get { return m_TableColumnEffectivelyVisibleVisible; } set { m_TableColumnEffectivelyVisibleVisible = value; } }
		public bool TableColumnPostCountVisible { get { return m_TableColumnPostCountVisible; } set { m_TableColumnPostCountVisible = value; } }
		public bool TableColumnRootPostTitleVisible { get { return m_TableColumnRootPostTitleVisible; } set { m_TableColumnRootPostTitleVisible = value; } }
		public bool TableColumnSearchRelevanceVisible { get { return m_TableColumnSearchRelevanceVisible; } set { m_TableColumnSearchRelevanceVisible = value; } }
		public bool TableColumnTimeLastPostVisible { get { return m_TableColumnTimeLastPostVisible; } set { m_TableColumnTimeLastPostVisible = value; } }
		public bool TableColumnTimePostedVisible { get { return m_TableColumnTimePostedVisible; } set { m_TableColumnTimePostedVisible = value; } }
		public bool TableColumnTitleVisible { get { return m_TableColumnTitleVisible; } set { m_TableColumnTitleVisible = value; } }
		public bool TableColumnVisibleVisible { get { return m_TableColumnVisibleVisible; } set { m_TableColumnVisibleVisible = value; } }
		
		public Position SearchTextBoxPosition { get { return m_SearchTextBoxPosition; } set { m_SearchTextBoxPosition = value; } }
		public Position AddPostButtonPosition { get { return m_AddPostButtonPosition; } set { m_AddPostButtonPosition = value; } }
		public Position PagingPosition { get { return m_PagingPosition; } set { m_PagingPosition = value; } }
		
		public bool NumberOfItemsSelectorAutoPostBack { get { return m_NumberOfItemsSelectorAutoPostBack; } set { m_NumberOfItemsSelectorAutoPostBack = value; } }
		public bool TableColumnHeadersVisible { get { return m_TableColumnHeadersVisible; } set { m_TableColumnHeadersVisible = value; } }
		public bool SortingEnabled { get { return m_SortingEnabled; } set { m_SortingEnabled = value; } }
		public bool SortAscending {	get { return (bool)(this.ViewState[VIEWSTATE_SORTASCENDING] == null ? this.ViewState.Add(VIEWSTATE_SORTASCENDING, DEFAULT_SORTASCENDING).Value : this.ViewState[VIEWSTATE_SORTASCENDING]); } set { this.ViewState.Add(VIEWSTATE_SORTASCENDING, value); } }
		public Post.Property SortProperty { get { return (Post.Property)(this.ViewState[VIEWSTATE_SORTPROPERTY] == null ? this.ViewState.Add(VIEWSTATE_SORTPROPERTY, DEFAULT_SORTPROPERTY).Value : this.ViewState[VIEWSTATE_SORTPROPERTY]); } set { this.ViewState.Add(VIEWSTATE_SORTPROPERTY, value); } }
		public string SortAttributeName { get { return (string)(this.ViewState[VIEWSTATE_SORTATTRIBUTENAME] == null ? this.ViewState.Add(VIEWSTATE_SORTATTRIBUTENAME, DEFAULT_SORTATTRIBUTENAME).Value : this.ViewState[VIEWSTATE_SORTATTRIBUTENAME]); } set { this.ViewState.Add(VIEWSTATE_SORTATTRIBUTENAME, value); } }
		public int NumberOfItems { get { return ctrlPagedListing.ItemsPerPage; } set { ctrlPagedListing.ItemsPerPage = value; } }
		public Paging.PagingType PagingType { get { return m_PagingType; } set { m_PagingType = value; } }

		public string OmissionIndicator { get { return m_OmissionIndicator; } set { m_OmissionIndicator = value; } }
		public string FieldSetLabelText { get { return m_FieldSetLabelText; } set { m_FieldSetLabelText = value; } }
		public string MessageNoPostsText { get { return m_MessageNoPostsText; } set { m_MessageNoPostsText = value; } }
		public string TableCaptionText { get { return m_TableCaptionText; } set { m_TableCaptionText = value; } }
		public string TableColumnAuthorText { get { return m_TableColumnAuthorText; } set { m_TableColumnAuthorText = value; } }
		public string TableColumnBodyText { get { return m_TableColumnBodyText; } set { m_TableColumnBodyText = value; } }
		public string TableColumnCategoryText { get { return m_TableColumnCategoryText; } set { m_TableColumnCategoryText = value; } }
		public string TableColumnEffectivelyVisibleText { get { return m_TableColumnEffectivelyVisibleText; } set { m_TableColumnEffectivelyVisibleText = value; } }
		public string TableColumnPostCountText { get { return m_TableColumnPostCountText; } set { m_TableColumnPostCountText = value; } }
		public string TableColumnRootPostTitleText { get { return m_TableColumnRootPostTitleText; } set { m_TableColumnRootPostTitleText = value; } }
		public string TableColumnSearchRelevanceText { get { return m_TableColumnSearchRelevanceText; } set { m_TableColumnSearchRelevanceText = value; } }
		public string TableColumnTimeLastPostText { get { return m_TableColumnTimeLastPostText; } set { m_TableColumnTimeLastPostText = value; } }
		public string TableColumnTimePostedText { get { return m_TableColumnTimePostedText; } set { m_TableColumnTimePostedText = value; } }
		public string TableColumnTitleText { get { return m_TableColumnTitleText; } set { m_TableColumnTitleText = value; } }
		public string TableColumnVisibleText { get { return m_TableColumnVisibleText; } set { m_TableColumnVisibleText = value; } }
		public string TableValueEffectivelyVisibleTrueText { get { return m_TableValueEffectivelyVisibleTrueText; } set { m_TableValueEffectivelyVisibleTrueText = value; } }
		public string TableValueEffectivelyVisibleFalseText { get { return m_TableValueEffectivelyVisibleFalseText; } set { m_TableValueEffectivelyVisibleFalseText = value; } }
		public string TableValueVisibleTrueText { get { return m_TableValueVisibleTrueText; } set { m_TableValueVisibleTrueText = value; } }
		public string TableValueVisibleFalseText { get { return m_TableValueVisibleFalseText; } set { m_TableValueVisibleFalseText = value; } }
		public string NumberOfItemsSelectorLabelText { get { return m_NumberOfItemsSelectorLabelText; } set { m_NumberOfItemsSelectorLabelText = value; } }
		public string NumberOfItemsSelectorAllItemText { get { return m_NumberOfItemsSelectorAllItemText; } set { m_NumberOfItemsSelectorAllItemText = value; } }
		public string SearchTextBoxLabelText { get { return m_SearchTextBoxLabelText; } set { m_SearchTextBoxLabelText = value; } }
		public string AddPostButtonText { get { return m_AddPostButtonText; } set { m_AddPostButtonText = value; } }
		public string SubmitButtonText { get { return m_SubmitButtonText; } set { m_SubmitButtonText = value; } }
		public string PagingResultText { get { return ctrlPagedListing.PagingResultText;} set { ctrlPagedListing.PagingResultText = value; } }
		public string PagingNumberOfPagesText { get { return ctrlPagedListing.PagingNumberOfPagesText;} set { ctrlPagedListing.PagingNumberOfPagesText = value; } }
		public string PagingPreviousPageText { get { return ctrlPagedListing.PagingPreviousPageText; } set { ctrlPagedListing.PagingPreviousPageText = value; } }
		public string PagingNextPageText { get { return ctrlPagedListing.PagingNextPageText; } set { ctrlPagedListing.PagingNextPageText = value; } }
		public string PagingNextIncrementText { get { return ctrlPagedListing.PagingNextIncrementText; } set { ctrlPagedListing.PagingNextIncrementText = value; } }
		public string PagingPreviousIncrementText { get { return ctrlPagedListing.PagingPreviousIncrementText; } set { ctrlPagedListing.PagingPreviousIncrementText = value; } }
		#endregion

		#region Event Handlers
		private void ctrlPagedListing_AfterDataBinding(object sender, EventArgs e) {
			WebParagraph ctrlMessageNoPosts		= (WebParagraph)m_Foot.FindControl("ctrlMessageNoPosts");
			ctrlMessageNoPosts.EnableViewState	= false;
			Control ctrlTableTop				= m_Head.FindControl("ctrlTableTop");
			Control ctrlTableBottom				= m_Foot.FindControl("ctrlTableBottom");
			if(ctrlPagedListing.VisibleItems.Equals(0)) {
				ctrlMessageNoPosts.InnerText		= this.MessageNoPostsText;
				ctrlMessageNoPosts.Visible			= !(ctrlTableTop.Visible = ctrlTableBottom.Visible = false);
			} else {
				ctrlMessageNoPosts.Visible			= !(ctrlTableTop.Visible = ctrlTableBottom.Visible = true);
			}
		}
		private void ctrlPagedListing_ListItemDataBinding(object sender, ListItemEventArgs e) {
			FlatTabularPostListItem listItem				= (FlatTabularPostListItem)e.ListItem;
			listItem.UrlWithoutTrailingPostId				= this.UrlWithoutTrailingPostId;
			listItem.UrlWithoutTrailingUserIdentity			= this.UrlWithoutTrailingUserIdentity;
			listItem.UrlWithoutTrailingCategoryId			= this.UrlWithoutTrailingCategoryId;
			listItem.UrlWithoutTrailingRootPostId			= this.UrlWithoutTrailingRootPostId;
			listItem.OmissionIndicator						= this.OmissionIndicator;
			listItem.MaximumBodyLength						= this.MaximumBodyLength;
			listItem.MaximumTitleLength						= this.MaximumTitleLength;
			listItem.MaximumRootPostTitleLength				= this.MaximumRootPostTitleLength;
			listItem.AuthorResolver							= this.AuthorResolver;
			listItem.DateFormatter							= this.DateFormatter;
			listItem.TableColumnAuthorVisible				= this.TableColumnAuthorVisible;
			listItem.TableColumnBodyVisible					= this.TableColumnBodyVisible;
			listItem.TableColumnCategoryVisible				= this.TableColumnCategoryVisible;
			listItem.TableColumnEffectivelyVisibleVisible	= this.TableColumnEffectivelyVisibleVisible;
			listItem.TableColumnPostCountVisible			= this.TableColumnPostCountVisible;
			listItem.TableColumnRootPostTitleVisible		= this.TableColumnRootPostTitleVisible;
			listItem.TableColumnSearchRelevanceVisible		= this.TableColumnSearchRelevanceVisible;
			listItem.TableColumnTimeLastPostVisible			= this.TableColumnTimeLastPostVisible;
			listItem.TableColumnTimePostedVisible			= this.TableColumnTimePostedVisible;
			listItem.TableColumnTitleVisible				= this.TableColumnTitleVisible;
			listItem.TableColumnVisibleVisible				= this.TableColumnVisibleVisible;
			listItem.TableValueEffectivelyVisibleTrueText	= this.TableValueEffectivelyVisibleTrueText;
			listItem.TableValueEffectivelyVisibleFalseText	= this.TableValueEffectivelyVisibleFalseText;
			listItem.TableValueVisibleTrueText				= this.TableValueVisibleTrueText;
			listItem.TableValueVisibleFalseText				= this.TableValueVisibleFalseText;
			listItem.EnableViewState						= false;
		}
		private void lbtnAuthor_Click(object sender, EventArgs e) {
			SortOnProperty(Post.Property.Author);
		}
		private void lbtnBody_Click(object sender, EventArgs e) {
			SortOnProperty(Post.Property.Body);
		}
		private void lbtnCategory_Click(object sender, EventArgs e) {
			SortOnProperty(Post.Property.EffectiveCategoryName);
		}
		private void lbtnEffectivelyVisible_Click(object sender, EventArgs e) {
			SortOnProperty(Post.Property.EffectivelyVisible);
		}
		private void lbtnPostCount_Click(object sender, EventArgs e) {
			SortOnProperty(Post.Property.PostCount);
		}
		private void lbtnRootPostTitle_Click(object sender, EventArgs e) {
			SortOnProperty(Post.Property.RootPostTitle);
		}
		private void lbtnSearchRelevance_Click(object sender, EventArgs e) {
			SortOnProperty(Post.Property.SearchRelevance);
		}
		private void lbtnTimeLastPost_Click(object sender, EventArgs e) {
			SortOnProperty(Post.Property.TimeLastPost);
		}
		private void lbtnTimePosted_Click(object sender, EventArgs e) {
			SortOnProperty(Post.Property.TimePosted);
		}
		private void lbtnTitle_Click(object sender, EventArgs e) {
			SortOnProperty(Post.Property.Title);
		}
		private void lbtnVisible_Click(object sender, EventArgs e) {
			SortOnProperty(Post.Property.Visible);
		}
		private void btnAddPost_Click(object sender, EventArgs e) {
			if(this.PostId != null) { 
				OnAddPost(new AddPostEventArgs(this.PostId));
			} else if(this.CategoryId != null) {
				OnAddPost(new AddPostEventArgs(this.CategoryId));
			} else if(this.Domain != null && this.CategoryName != null) {
				OnAddPost(new AddPostEventArgs(this.Domain, this.CategoryName));
			}
		}
		private void txtSearch_TextChanged(object sender, EventArgs e) {
			m_ChangedSearchTextBox = (TextBox)sender;
		}
		private void btnSubmit_Click(object sender, EventArgs e) {
			if(m_ChangedSearchTextBox != null && 
				((m_ChangedSearchTextBox.Equals(txtHeaderSearch) && sender.Equals(btnHeaderSubmit)) ||
				(m_ChangedSearchTextBox.Equals(txtFooterSearch) && sender.Equals(btnFooterSubmit)))) {
				this.SearchText = m_ChangedSearchTextBox.Text == null || m_ChangedSearchTextBox.Text.TrimEnd().Length.Equals(0) ? null : m_ChangedSearchTextBox.Text;
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
			AddPostEventHandler eventHandler = base.Events[s_EventAddPost] as AddPostEventHandler;
			if(eventHandler != null) {
				eventHandler(this, e);
			}
		}
		protected virtual void OnSearch(SearchEventArgs e) {
			SearchEventHandler eventHandler = base.Events[s_EventSearch] as SearchEventHandler;
			if(eventHandler != null) {
				eventHandler(this, e);
			}
		}
		protected virtual void OnSearchDiscarded(EventArgs e) {
			SearchDiscardedEventHandler eventHandler = base.Events[s_EventSearchDiscarded] as SearchDiscardedEventHandler;
			if(eventHandler != null) {
				eventHandler(this, e);
			}
		}
		protected virtual void OnPostsSorting(PostsSortEventArgs e) {
			PostsSortingEventHandler eventHandler = base.Events[s_EventPostsSorting] as PostsSortingEventHandler;
			if(eventHandler != null) {
				eventHandler(this, e);
			}
		}
		#endregion

		#region Overridden Methods
		protected override void OnInit(EventArgs e) {
			m_Head									= ctrlPagedListing.HeaderControl;
			m_Foot									= ctrlPagedListing.FooterControl;

			txtHeaderSearch							= (TextBox)m_Head.FindControl("txtHeaderSearch");
			txtFooterSearch							= (TextBox)m_Foot.FindControl("txtFooterSearch");
			lbtnAuthor								= (WebLinkButton)m_Head.FindControl("lbtnAuthor");
			lbtnBody								= (WebLinkButton)m_Head.FindControl("lbtnBody");
			lbtnCategory							= (WebLinkButton)m_Head.FindControl("lbtnCategory");
			lbtnEffectivelyVisible					= (WebLinkButton)m_Head.FindControl("lbtnEffectivelyVisible");
			lbtnPostCount							= (WebLinkButton)m_Head.FindControl("lbtnPostCount");
			lbtnRootPostTitle						= (WebLinkButton)m_Head.FindControl("lbtnRootPostTitle");
			lbtnSearchRelevance						= (WebLinkButton)m_Head.FindControl("lbtnSearchRelevance");
			lbtnTimeLastPost						= (WebLinkButton)m_Head.FindControl("lbtnTimeLastPost");
			lbtnTimePosted							= (WebLinkButton)m_Head.FindControl("lbtnTimePosted");
			lbtnTitle								= (WebLinkButton)m_Head.FindControl("lbtnTitle");
			lbtnVisible								= (WebLinkButton)m_Head.FindControl("lbtnVisible");
			btnHeaderAddPost						= (WebButton)m_Head.FindControl("btnHeaderAddPost");
			btnFooterAddPost						= (WebButton)m_Foot.FindControl("btnFooterAddPost");
			btnHeaderSubmit							= (WebButton)m_Head.FindControl("btnHeaderSubmit");
			btnFooterSubmit							= (WebButton)m_Foot.FindControl("btnFooterSubmit");

			btnHeaderAddPost.EnableViewState = btnFooterAddPost.EnableViewState = 
				btnHeaderSubmit.EnableViewState = btnFooterSubmit.EnableViewState = false;

			lbtnAuthor.Click						+= this.lbtnAuthor_Click;
			lbtnBody.Click							+= this.lbtnBody_Click;
			lbtnCategory.Click						+= this.lbtnCategory_Click;
			lbtnEffectivelyVisible.Click			+= this.lbtnEffectivelyVisible_Click;
			lbtnPostCount.Click						+= this.lbtnPostCount_Click;
			lbtnRootPostTitle.Click					+= this.lbtnRootPostTitle_Click;
			lbtnSearchRelevance.Click				+= this.lbtnSearchRelevance_Click;
			lbtnTimeLastPost.Click					+= this.lbtnTimeLastPost_Click;
			lbtnTimePosted.Click					+= this.lbtnTimePosted_Click;
			lbtnTitle.Click							+= this.lbtnTitle_Click;
			lbtnVisible.Click						+= this.lbtnVisible_Click;
			EventHandler addPostEventHandler		= this.btnAddPost_Click;
			EventHandler submitEventHandler			= this.btnSubmit_Click;
			EventHandler textSearchEventHandler		= this.txtSearch_TextChanged;
			btnHeaderAddPost.Click					+= addPostEventHandler;
			btnFooterAddPost.Click					+= addPostEventHandler;
			btnHeaderSubmit.Click					+= submitEventHandler;
			btnFooterSubmit.Click					+= submitEventHandler;
			txtHeaderSearch.TextChanged				+= textSearchEventHandler;
			txtFooterSearch.TextChanged				+= textSearchEventHandler;
			
			ctrlPagedListing.AfterDataBinding		+= this.ctrlPagedListing_AfterDataBinding;
			ctrlPagedListing.ListItemDataBinding	+= this.ctrlPagedListing_ListItemDataBinding;

			base.OnInit(e);
		}
		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender(e);
			if(this.SortingEnabled) {
				SetSortingCss(this.SortProperty);
			}
			Paging ctrlHeaderPaging		= (Paging)m_Head.FindControl("ctrlHeaderPaging");
			Paging ctrlFooterPaging		= (Paging)m_Head.FindControl("ctrlFooterPaging");
			SetVisibility(this.PagingVisible, this.PagingPosition, ctrlHeaderPaging, ctrlFooterPaging);
			if(this.PagingVisible) {
				ctrlHeaderPaging.Type = ctrlFooterPaging.Type = this.PagingType;
			}
			//do not show add post button if we are searching and when we do not have a context where we can add a post 
			//(fetching all posts or from domain or /*searching or filtering on attributes*/)
			bool realAddPostButtonVisible	= this.AddPostButtonVisible && 
				/*this.SearchText == null && this.UserIdentity == null && this.AttributeCriteria.Count.Equals(0) && */
				!((this.PostId == null && this.CategoryId == null && this.Domain == null && this.CategoryName == null) ||
				(this.Domain != null && this.CategoryName == null));
			Control ctrlHeaderAddPost		= m_Head.FindControl("ctrlHeaderAddPost");
			Control ctrlFooterAddPost		= m_Foot.FindControl("ctrlFooterAddPost");
			SetVisibility(realAddPostButtonVisible, this.AddPostButtonPosition, ctrlHeaderAddPost, ctrlFooterAddPost);
			ctrlHeaderAddPost.EnableViewState = ctrlFooterAddPost.EnableViewState = false;
			if(realAddPostButtonVisible) {
				btnHeaderAddPost.InnerText = btnFooterAddPost.InnerText = this.AddPostButtonText;
			}
			Control ctrlHeaderSearch	= m_Head.FindControl("ctrlHeaderSearch");
			Control ctrlFooterSearch	= m_Foot.FindControl("ctrlFooterSearch");
			SetVisibility(this.SearchTextBoxVisible, this.SearchTextBoxPosition, ctrlHeaderSearch, ctrlFooterSearch);
			if(this.SearchTextBoxVisible) {
				WebLabel lblHeaderSearch	= (WebLabel)m_Head.FindControl("lblHeaderSearch");
				WebLabel lblFooterSearch	= (WebLabel)m_Foot.FindControl("lblFooterSearch");
				lblHeaderSearch.EnableViewState = lblFooterSearch.EnableViewState = false;
				lblHeaderSearch.InnerText = lblFooterSearch.InnerText = this.SearchTextBoxLabelText;
				txtHeaderSearch.Text = txtFooterSearch.Text = this.SearchText;
			}
			Control ctrlHeaderNumberOfItems	= m_Head.FindControl("ctrlHeaderNumberOfItems");
			bool realNumberOfItemSelectorAutoPostBack = ctrlHeaderSearch.Visible ? false : this.NumberOfItemsSelectorAutoPostBack;
			if(this.NumberOfItemsSelectorVisible) {
				ctrlHeaderNumberOfItems.Visible = ctrlHeaderNumberOfItems.Parent.Visible = true;
				WebLabel lblHeaderNumberOfItemsSelector					= (WebLabel)m_Head.FindControl("lblHeaderNumberOfItemsSelector");
				lblHeaderNumberOfItemsSelector.EnableViewState			= false;
				lblHeaderNumberOfItemsSelector.InnerText				= this.NumberOfItemsSelectorLabelText;
				NumberOfItemsSelector ctrlHeaderNumberOfItemsSelector	= (NumberOfItemsSelector)m_Head.FindControl("ctrlHeaderNumberOfItemsSelector");
				ctrlHeaderNumberOfItemsSelector.AutoPostBack			= realNumberOfItemSelectorAutoPostBack;
				ctrlHeaderNumberOfItemsSelector.IncludeAllItem			= this.NumberOfItemsSelectorIncludeAllItem;
				ctrlHeaderNumberOfItemsSelector.AllItemText				= this.NumberOfItemsSelectorAllItemText;
			} else {
				ctrlHeaderNumberOfItems.Visible = false;
			}
			WebPlaceHolder ctrlHeaderNoScript	= (WebPlaceHolder)m_Head.FindControl("ctrlHeaderNoScript");
			WebPlaceHolder ctrlFooterNoScript	= (WebPlaceHolder)m_Foot.FindControl("ctrlFooterNoScript");
			ctrlHeaderNoScript.EnableViewState = ctrlFooterNoScript.EnableViewState = false;
			if(ctrlHeaderNoScript.Visible = ctrlHeaderSearch.Visible || ctrlHeaderNumberOfItems.Visible) {
				((WebButton)m_Head.FindControl("btnHeaderSubmit")).InnerText = this.SubmitButtonText;
			}
			if(ctrlFooterNoScript.Visible = ctrlFooterSearch.Visible) {
				((WebButton)m_Foot.FindControl("btnFooterSubmit")).InnerText = this.SubmitButtonText;
			}
			ctrlHeaderNoScript.SurroundingTag = realNumberOfItemSelectorAutoPostBack ? HtmlTag.Noscript : null;
			ctrlFooterNoScript.SurroundingTag = ctrlFooterSearch.Visible ? null : HtmlTag.Noscript;
			if(!ctrlHeaderNoScript.Visible && !ctrlHeaderAddPost.Visible) {
				ctrlHeaderNoScript.Parent.Visible = false;
			}
			if(!ctrlFooterNoScript.Visible && !ctrlFooterAddPost.Visible) {
				ctrlFooterNoScript.Parent.Visible = false;
			}
			if(m_Head.FindControl("ctrlHeaderFieldSet").Visible) {
				WebGenericControl ctrlHeaderFieldSetLabel	= (WebGenericControl)m_Head.FindControl("ctrlHeaderFieldSetLabel");
				if(ctrlHeaderFieldSetLabel.Visible = this.FieldSetLabelText != null && !this.FieldSetLabelText.Length.Equals(0)) {
					ctrlHeaderFieldSetLabel.EnableViewState	= false;
					ctrlHeaderFieldSetLabel.InnerText		= this.FieldSetLabelText;
				}
			}
			if(m_Foot.FindControl("ctrlFooterFieldSet").Visible) {
				WebGenericControl ctrlFooterFieldSetLabel	= (WebGenericControl)m_Foot.FindControl("ctrlFooterFieldSetLabel");
				if(ctrlFooterFieldSetLabel.Visible = this.FieldSetLabelText != null && !this.FieldSetLabelText.Length.Equals(0)) {
					ctrlFooterFieldSetLabel.EnableViewState	= false;
					ctrlFooterFieldSetLabel.InnerText		= this.FieldSetLabelText;
				}
			}
			if(!string.IsNullOrEmpty(this.TableCaptionText)) {
				WebPlaceHolder cptTableCaption	= (WebPlaceHolder)m_Head.FindControl("cptTableCaption");
				cptTableCaption.EnableViewState	= false;
				cptTableCaption.SurroundingTag	= HtmlTag.Caption;
				cptTableCaption.InnerText		= this.TableCaptionText;
			}
			Control ctrlHeaderColumnHeaders = m_Head.FindControl("ctrlHeaderColumnHeaders");
			if(ctrlHeaderColumnHeaders.Visible = this.TableColumnHeadersVisible) {
				ConfigureTableColumn((WebGenericControl)m_Head.FindControl("ctrlHeaderAuthor"), lbtnAuthor, this.TableColumnAuthorVisible, this.SortingEnabled, this.TableColumnAuthorText);
				ConfigureTableColumn((WebGenericControl)m_Head.FindControl("ctrlHeaderBody"), lbtnBody, this.TableColumnBodyVisible, this.SortingEnabled, this.TableColumnBodyText);
				ConfigureTableColumn((WebGenericControl)m_Head.FindControl("ctrlHeaderCategory"), lbtnCategory, this.TableColumnCategoryVisible, this.SortingEnabled, this.TableColumnCategoryText);
				ConfigureTableColumn((WebGenericControl)m_Head.FindControl("ctrlHeaderEffectivelyVisible"), lbtnEffectivelyVisible, this.TableColumnEffectivelyVisibleVisible, this.SortingEnabled, this.TableColumnEffectivelyVisibleText);
				ConfigureTableColumn((WebGenericControl)m_Head.FindControl("ctrlHeaderPostCount"), lbtnPostCount, this.TableColumnPostCountVisible, this.SortingEnabled, this.TableColumnPostCountText);
				ConfigureTableColumn((WebGenericControl)m_Head.FindControl("ctrlHeaderRootPostTitle"), lbtnRootPostTitle, this.TableColumnRootPostTitleVisible, this.SortingEnabled, this.TableColumnRootPostTitleText);
				ConfigureTableColumn((WebGenericControl)m_Head.FindControl("ctrlHeaderSearchRelevance"), lbtnSearchRelevance, this.TableColumnSearchRelevanceVisible, this.SortingEnabled, this.TableColumnSearchRelevanceText);
				ConfigureTableColumn((WebGenericControl)m_Head.FindControl("ctrlHeaderTimeLastPost"), lbtnTimeLastPost, this.TableColumnTimeLastPostVisible, this.SortingEnabled, this.TableColumnTimeLastPostText);
				ConfigureTableColumn((WebGenericControl)m_Head.FindControl("ctrlHeaderTimePosted"), lbtnTimePosted, this.TableColumnTimePostedVisible, this.SortingEnabled, this.TableColumnTimePostedText);
				ConfigureTableColumn((WebGenericControl)m_Head.FindControl("ctrlHeaderTitle"), lbtnTitle, this.TableColumnTitleVisible, this.SortingEnabled, this.TableColumnTitleText);
				ConfigureTableColumn((WebGenericControl)m_Head.FindControl("ctrlHeaderVisible"), lbtnVisible, this.TableColumnVisibleVisible, this.SortingEnabled, this.TableColumnVisibleText);
			} else {
				ctrlHeaderColumnHeaders.EnableViewState = false;
			}
			if(!m_DataBound) {
				this.DataBind();
			}
		}
		public override void DataBind() {
			//the looping here rests on the behaviour of the PagedListing automatically adjusting CurrentPageNumber when it is out of bound; 
			//if we find that we have zero items in the list, then loop until we have items, but stop if we are at page one.
			do {
				if(this.PostCollection != null && !this.PostCollection.Count.Equals(0)) {
					ctrlPagedListing.ListCollection									= this.PostCollection;
				} else {
					ThreadedPostsResultConfiguration threadedResultConfiguration	= null;
					PostsResultConfiguration resultConfiguration					= null;
					PagingConfiguration pagingConfiguration							= new PagingConfiguration(this.NumberOfItems, ctrlPagedListing.CurrentPageNumber);
					SearchCriteria searchCriteria									= this.SearchText == null && this.UserIdentity == null && this.AttributeCriteria.Count.Equals(0) ? null : new SearchCriteria(this.SearchText, this.UserIdentity, this.AttributeCriteria);
					if(searchCriteria == null) {
						threadedResultConfiguration									= new ThreadedPostsResultConfiguration(1, this.IncludeHidden, this.LoadAttributes, this.SortProperty, this.SortAttributeName, this.SortAscending, this.DateFilterFrom, this.DateFilterTo, this.DateFilterProperty);
					} else {
						resultConfiguration											= new PostsResultConfiguration(this.IncludeHidden, this.LoadAttributes, this.SortProperty, this.SortAttributeName, this.SortAscending, this.DateFilterFrom, this.DateFilterTo, this.DateFilterProperty);
					}
					PagedPostsResult pagedResult;
					if(this.PostId != null) {
						pagedResult		= searchCriteria == null ?
							this.ForumDao.GetPosts(this.PostId, this.PostType, threadedResultConfiguration, pagingConfiguration) :
							this.ForumDao.SearchPosts(this.PostId, this.PostType, searchCriteria, resultConfiguration, pagingConfiguration);
					} else if(this.CategoryId != null) {
						pagedResult		= searchCriteria == null ?
							this.ForumDao.GetPosts(this.CategoryId, threadedResultConfiguration, pagingConfiguration) :
							this.ForumDao.SearchPosts(this.CategoryId, searchCriteria, resultConfiguration, pagingConfiguration);
					} else if(this.Domain != null) {
						pagedResult		= this.CategoryName == null ?
							searchCriteria == null ?
								this.ForumDao.GetPosts(this.Domain, threadedResultConfiguration, pagingConfiguration) :
								this.ForumDao.SearchPosts(this.Domain, searchCriteria, resultConfiguration, pagingConfiguration) :
							searchCriteria == null ?
								this.ForumDao.GetPosts(this.Domain, this.CategoryName, threadedResultConfiguration, pagingConfiguration) :
								this.ForumDao.SearchPosts(this.Domain, this.CategoryName, searchCriteria, resultConfiguration, pagingConfiguration);
					} else {
						pagedResult		= searchCriteria == null ?
							this.ForumDao.GetPosts(threadedResultConfiguration, pagingConfiguration) :
							this.ForumDao.SearchPosts(searchCriteria, resultConfiguration, pagingConfiguration);
					}
					ctrlPagedListing.DisablePaging		= true;
					ctrlPagedListing.TotalNumberOfItems = pagedResult.TotalCount;
					ctrlPagedListing.ListCollection		= pagedResult.Posts;
				}
				ctrlPagedListing.DataBind();
			} while(ctrlPagedListing.ListCollection.Count.Equals(0) && this.ctrlPagedListing.CurrentPageNumber > 1);
			m_DataBound = true;
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
			PostsSortEventArgs sortEventArgs = new PostsSortEventArgs(property, null, this.SortProperty.Equals(property) ? !this.SortAscending : DEFAULT_SORTASCENDING);
			OnPostsSorting(sortEventArgs);
			this.SortAscending		= sortEventArgs.Ascending;
			this.SortProperty		= sortEventArgs.Property;
			this.SortAttributeName	= sortEventArgs.AttributeName;
		}
		private void SetSortingCss(Post.Property property) {
			const string SPACE	= " ";

			lbtnAuthor.CssClass = lbtnBody.CssClass = lbtnCategory.CssClass = lbtnEffectivelyVisible.CssClass = 
				lbtnPostCount.CssClass = lbtnRootPostTitle.CssClass = lbtnSearchRelevance.CssClass = lbtnTimeLastPost.CssClass = 
				lbtnTimePosted.CssClass = lbtnTitle.CssClass = lbtnVisible.CssClass = string.Empty;
 			if(this.SortAttributeName == null) {
				WebLinkButton linkButton = LinkButtonFromProperty(property);
				if(linkButton != null) {
					if(linkButton.CssClass.Length > 0) {
						linkButton.CssClass += SPACE;
					}
					linkButton.CssClass += this.SortAscending ? CSSCLASS_ASCENDING : CSSCLASS_DESCENDING;
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
			if(tableColumn.Visible				= visible) {
				if(tableColumnSorter.Visible	= sortingEnabled) {
					tableColumnSorter.Text		= text;
				} else {
					tableColumn.InnerText		= text;
				}
			}
			tableColumn.EnableViewState			= false;
		}
		#endregion
	}

}