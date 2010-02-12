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

using System.Web;
using System.Globalization;

using nJupiter.Web.UI.Controls;
using nJupiter.Web.UI.Controls.Listings;

namespace nJupiter.Services.Forum.UI.Web {

	public class FlatTabularPostListItem : ListItemBase {
		#region Constants
		private const string	CSSCLASS_ODD			= "odd";
		private const string	CSSCLASS_EVEN			= "even";
		private const string	CSSCLASS_HIDDEN			= "hidden";
		private const string	CSSCLASS_HIDDENBYHIDDEN	= "hidden-by-hidden";
		private const string	CSSCLASS_YES			= "yes";
		private const string	CSSCLASS_NO				= "no";
		#endregion

		#region Variables
		private Post			m_CurrentPost;
		private int				m_MaximumBodyLength;
		private int				m_MaximumTitleLength;
		private int				m_MaximumRootPostTitleLength;
		private string			m_OmissionIndicator;
		private string			m_UrlWithoutTrailingPostId;
		private string			m_UrlWithoutTrailingUserIdentity;
		private string			m_UrlWithoutTrailingCategoryId;
		private string			m_UrlWithoutTrailingRootPostId;
		private DateFormatter	m_DateFormatter;
		private AuthorResolver	m_AuthorResolver;
		private string			m_TableValueEffectivelyVisibleTrueText;
		private string			m_TableValueEffectivelyVisibleFalseText;
		private string			m_TableValueVisibleTrueText;
		private string			m_TableValueVisibleFalseText;
		#endregion

		#region UI Members
		protected WebGenericControl	ctrlAuthor;
		protected WebAnchor			ctrlAuthorAnchor;
		protected WebGenericControl ctrlBody;
		protected WebAnchor			ctrlBodyAnchor;
		protected WebGenericControl ctrlCategory;
		protected WebAnchor			ctrlCategoryAnchor;
		protected WebGenericControl ctrlEffectivelyVisible;
		protected WebGenericControl ctrlEffectivelyVisibleSpan;
		protected WebGenericControl ctrlRootPostTitle;
		protected WebAnchor			ctrlRootPostTitleAnchor;
		protected WebGenericControl	ctrlPostCount;
		protected WebGenericControl ctrlSearchRelevance;
		protected WebGenericControl	ctrlTimeLastPost;
		protected WebGenericControl	ctrlTimePosted;
		protected WebGenericControl	ctrlTitle;
		protected WebAnchor			ctrlTitleAnchor;
		protected WebGenericControl	ctrlTableRow;
		protected WebGenericControl ctrlVisible;
		protected WebGenericControl ctrlVisibleSpan;
		#endregion

		#region Properties
		protected Post CurrentPost { get { return this.m_CurrentPost ?? (this.m_CurrentPost = (Post)this.ListItem); } }

		public int MaximumBodyLength { get { return m_MaximumBodyLength; } set { m_MaximumBodyLength = value; } }
		public int MaximumTitleLength { get { return m_MaximumTitleLength; } set { m_MaximumTitleLength = value; } }
		public int MaximumRootPostTitleLength { get { return m_MaximumRootPostTitleLength; } set { m_MaximumRootPostTitleLength = value; } }
		public string OmissionIndicator { get { return m_OmissionIndicator; } set { m_OmissionIndicator = value; } }
		public string UrlWithoutTrailingPostId { get { return m_UrlWithoutTrailingPostId; } set { m_UrlWithoutTrailingPostId = value; } }
		public string UrlWithoutTrailingUserIdentity { get { return m_UrlWithoutTrailingUserIdentity; } set { m_UrlWithoutTrailingUserIdentity = value; } }
		public string UrlWithoutTrailingCategoryId { get { return m_UrlWithoutTrailingCategoryId; } set { m_UrlWithoutTrailingCategoryId = value; } }
		public string UrlWithoutTrailingRootPostId { get { return m_UrlWithoutTrailingRootPostId; } set { m_UrlWithoutTrailingRootPostId = value; } }
		public AuthorResolver AuthorResolver { get { return m_AuthorResolver; } set { m_AuthorResolver = value; } }
		public DateFormatter DateFormatter { get { return m_DateFormatter; } set { m_DateFormatter = value; } }
		
		public bool TableColumnAuthorVisible { get { return ctrlAuthor.Visible; } set { ctrlAuthor.Visible = value; } }
		public bool TableColumnBodyVisible { get { return ctrlBody.Visible; } set { ctrlBody.Visible = value; } }
		public bool TableColumnCategoryVisible { get { return ctrlCategory.Visible; } set { ctrlCategory.Visible = value; } }
		public bool TableColumnEffectivelyVisibleVisible { get { return ctrlEffectivelyVisible.Visible; } set { ctrlEffectivelyVisible.Visible = value; } }
		public bool TableColumnPostCountVisible { get { return ctrlPostCount.Visible; } set { ctrlPostCount.Visible = value; } }
		public bool TableColumnRootPostTitleVisible { get { return ctrlRootPostTitle.Visible; } set { ctrlRootPostTitle.Visible = value; } }
		public bool TableColumnSearchRelevanceVisible { get { return ctrlSearchRelevance.Visible; } set { ctrlSearchRelevance.Visible = value; } }
		public bool TableColumnTimeLastPostVisible { get { return ctrlTimeLastPost.Visible; } set { ctrlTimeLastPost.Visible = value; } }
		public bool TableColumnTimePostedVisible { get { return ctrlTimePosted.Visible; } set { ctrlTimePosted.Visible = value; } }
		public bool TableColumnTitleVisible { get { return ctrlTitle.Visible; } set { ctrlTitle.Visible = value; } }
		public bool TableColumnVisibleVisible { get { return ctrlVisible.Visible; } set { ctrlVisible.Visible = value; } }
		public string TableValueEffectivelyVisibleTrueText { get { return m_TableValueEffectivelyVisibleTrueText; } set { m_TableValueEffectivelyVisibleTrueText = value; } }
		public string TableValueEffectivelyVisibleFalseText { get { return m_TableValueEffectivelyVisibleFalseText; } set { m_TableValueEffectivelyVisibleFalseText = value; } }
		public string TableValueVisibleTrueText { get { return m_TableValueVisibleTrueText; } set { m_TableValueVisibleTrueText = value; } }
		public string TableValueVisibleFalseText { get { return m_TableValueVisibleFalseText; } set { m_TableValueVisibleFalseText = value; } }
		#endregion

		#region Overridden Methods
		public override void DataBind() {
			const string FORMAT_PERCENT					= "p0";

			if(this.TableColumnAuthorVisible) {
				string author;
				ctrlAuthorAnchor.Text					= this.CurrentPost.UserIdentity != null && this.AuthorResolver != null && (author = this.AuthorResolver.GetAuthor(this.CurrentPost.UserIdentity)) != null ? author : this.CurrentPost.Author;
				if(!(ctrlAuthorAnchor.NoLink			= this.UrlWithoutTrailingUserIdentity == null || this.CurrentPost.UserIdentity == null)) {
					ctrlAuthorAnchor.NavigateUrl		= this.UrlWithoutTrailingUserIdentity + HttpUtility.UrlEncode(this.CurrentPost.UserIdentity);
				}
			}
			if(this.TableColumnBodyVisible) {
				ctrlBodyAnchor.Text						= this.CurrentPost.Body.Length > this.MaximumBodyLength ? this.CurrentPost.Body.Substring(0, this.MaximumBodyLength) + this.OmissionIndicator : this.CurrentPost.Body;
				if(!(ctrlBodyAnchor.NoLink				= this.UrlWithoutTrailingPostId == null && this.UrlWithoutTrailingRootPostId == null)) {
					//if UrlWithoutTrailingPostId is not specified, then use UrlWithoutTrailingRootPostId instead
					ctrlBodyAnchor.NavigateUrl			= this.UrlWithoutTrailingPostId == null ? this.UrlWithoutTrailingRootPostId + this.CurrentPost.RootPostId : this.UrlWithoutTrailingPostId + this.CurrentPost.Id;
				}
			}
			if(this.TableColumnCategoryVisible) {
				ctrlCategoryAnchor.Text					= this.CurrentPost.EffectiveCategoryName;
				if(!(ctrlCategoryAnchor.NoLink			= this.UrlWithoutTrailingCategoryId == null)) {
					ctrlCategoryAnchor.NavigateUrl		= this.UrlWithoutTrailingCategoryId + this.CurrentPost.EffectiveCategoryId;
				}
			}
			if(this.TableColumnEffectivelyVisibleVisible) {
				ctrlEffectivelyVisibleSpan.InnerText	= this.CurrentPost.EffectivelyVisible ? this.TableValueEffectivelyVisibleTrueText : this.TableValueEffectivelyVisibleFalseText;
				AddCssClass(ctrlEffectivelyVisible, this.CurrentPost.EffectivelyVisible ? CSSCLASS_YES : CSSCLASS_NO);
			}
			if(this.TableColumnPostCountVisible) {
				ctrlPostCount.InnerText					= this.CurrentPost.PostCount.ToString(NumberFormatInfo.CurrentInfo);
			}
			if(this.TableColumnRootPostTitleVisible) {
				ctrlRootPostTitleAnchor.Text			= this.CurrentPost.RootPostTitle.Length > this.MaximumRootPostTitleLength ? this.CurrentPost.RootPostTitle.Substring(0, this.MaximumRootPostTitleLength) + this.OmissionIndicator : this.CurrentPost.RootPostTitle;
				if(!(ctrlRootPostTitleAnchor.NoLink		= this.UrlWithoutTrailingRootPostId == null)) {
					ctrlRootPostTitleAnchor.NavigateUrl	= this.UrlWithoutTrailingRootPostId + this.CurrentPost.RootPostId;
				}
			}
			if(this.TableColumnSearchRelevanceVisible) {
				ctrlSearchRelevance.InnerText			= this.CurrentPost.SearchRelevance.ToString(FORMAT_PERCENT, NumberFormatInfo.CurrentInfo);
			}
			if(this.TableColumnTimeLastPostVisible) {
				ctrlTimeLastPost.InnerText				= this.DateFormatter.FormatDate(this.CurrentPost.TimeLastPost);
			}
			if(this.TableColumnTimePostedVisible) {
				ctrlTimePosted.InnerText				= this.DateFormatter.FormatDate(this.CurrentPost.TimePosted);
			}
			if(this.TableColumnTitleVisible) {
				ctrlTitleAnchor.Text					= this.CurrentPost.Title.Length > this.MaximumTitleLength ? this.CurrentPost.Title.Substring(0, this.MaximumTitleLength) + this.OmissionIndicator : this.CurrentPost.Title;
				if(!(ctrlTitleAnchor.NoLink				= this.UrlWithoutTrailingPostId == null && this.UrlWithoutTrailingRootPostId == null)) {
					//if UrlWithoutTrailingPostId is not specified, then use UrlWithoutTrailingRootPostId instead
					ctrlTitleAnchor.NavigateUrl			= this.UrlWithoutTrailingPostId == null ? this.UrlWithoutTrailingRootPostId + this.CurrentPost.RootPostId : this.UrlWithoutTrailingPostId + this.CurrentPost.Id;
				}
			}
			if(this.TableColumnVisibleVisible) {
				ctrlVisibleSpan.InnerText				= this.CurrentPost.Visible ? this.TableValueVisibleTrueText : this.TableValueVisibleFalseText;
				AddCssClass(ctrlVisible, this.CurrentPost.Visible ? CSSCLASS_YES : CSSCLASS_NO);
			}
			AddCssClass(ctrlTableRow, this.AlternatingItem ? CSSCLASS_EVEN : CSSCLASS_ODD);
			if(!this.CurrentPost.Visible) {
				AddCssClass(ctrlTableRow, CSSCLASS_HIDDEN);
			} else if(!this.CurrentPost.EffectivelyVisible) {
				AddCssClass(ctrlTableRow, CSSCLASS_HIDDENBYHIDDEN);
			}
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
		#endregion
	}

}