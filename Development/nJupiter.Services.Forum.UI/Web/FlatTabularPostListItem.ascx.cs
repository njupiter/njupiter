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
		private const string CssclassOdd = "odd";
		private const string CssclassEven = "even";
		private const string CssclassHidden = "hidden";
		private const string CssclassHiddenbyhidden = "hidden-by-hidden";
		private const string CssclassYes = "yes";
		private const string CssclassNo = "no";
		#endregion

		#region Variables
		private Post currentPost;
		#endregion

		#region UI Members
		protected WebGenericControl ctrlAuthor;
		protected WebAnchor ctrlAuthorAnchor;
		protected WebGenericControl ctrlBody;
		protected WebAnchor ctrlBodyAnchor;
		protected WebGenericControl ctrlCategory;
		protected WebAnchor ctrlCategoryAnchor;
		protected WebGenericControl ctrlEffectivelyVisible;
		protected WebGenericControl ctrlEffectivelyVisibleSpan;
		protected WebGenericControl ctrlRootPostTitle;
		protected WebAnchor ctrlRootPostTitleAnchor;
		protected WebGenericControl ctrlPostCount;
		protected WebGenericControl ctrlSearchRelevance;
		protected WebGenericControl ctrlTimeLastPost;
		protected WebGenericControl ctrlTimePosted;
		protected WebGenericControl ctrlTitle;
		protected WebAnchor ctrlTitleAnchor;
		protected WebGenericControl ctrlTableRow;
		protected WebGenericControl ctrlVisible;
		protected WebGenericControl ctrlVisibleSpan;
		#endregion

		#region Properties
		protected Post CurrentPost { get { return this.currentPost ?? (this.currentPost = (Post)this.ListItem); } }

		public int MaximumBodyLength { get; set; }
		public int MaximumTitleLength { get; set; }
		public int MaximumRootPostTitleLength { get; set; }
		public string OmissionIndicator { get; set; }
		public string UrlWithoutTrailingPostId { get; set; }
		public string UrlWithoutTrailingUserIdentity { get; set; }
		public string UrlWithoutTrailingCategoryId { get; set; }
		public string UrlWithoutTrailingRootPostId { get; set; }
		public AuthorResolver AuthorResolver { get; set; }
		public DateFormatter DateFormatter { get; set; }

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
		public string TableValueEffectivelyVisibleTrueText { get; set; }
		public string TableValueEffectivelyVisibleFalseText { get; set; }
		public string TableValueVisibleTrueText { get; set; }
		public string TableValueVisibleFalseText { get; set; }
		#endregion

		#region Overridden Methods
		public override void DataBind() {
			const string formatPercent = "p0";

			if(this.TableColumnAuthorVisible) {
				string author;
				ctrlAuthorAnchor.Text = this.CurrentPost.UserIdentity != null && this.AuthorResolver != null && (author = this.AuthorResolver.GetAuthor(this.CurrentPost.UserIdentity)) != null ? author : this.CurrentPost.Author;
				if(!(ctrlAuthorAnchor.NoLink = this.UrlWithoutTrailingUserIdentity == null || this.CurrentPost.UserIdentity == null)) {
					ctrlAuthorAnchor.NavigateUrl = this.UrlWithoutTrailingUserIdentity + HttpUtility.UrlEncode(this.CurrentPost.UserIdentity);
				}
			}
			if(this.TableColumnBodyVisible) {
				ctrlBodyAnchor.Text = this.CurrentPost.Body.Length > this.MaximumBodyLength ? this.CurrentPost.Body.Substring(0, this.MaximumBodyLength) + this.OmissionIndicator : this.CurrentPost.Body;
				if(!(ctrlBodyAnchor.NoLink = this.UrlWithoutTrailingPostId == null && this.UrlWithoutTrailingRootPostId == null)) {
					//if UrlWithoutTrailingPostId is not specified, then use UrlWithoutTrailingRootPostId instead
					ctrlBodyAnchor.NavigateUrl = this.UrlWithoutTrailingPostId == null ? this.UrlWithoutTrailingRootPostId + this.CurrentPost.RootPostId : this.UrlWithoutTrailingPostId + this.CurrentPost.Id;
				}
			}
			if(this.TableColumnCategoryVisible) {
				ctrlCategoryAnchor.Text = this.CurrentPost.EffectiveCategoryName;
				if(!(ctrlCategoryAnchor.NoLink = this.UrlWithoutTrailingCategoryId == null)) {
					ctrlCategoryAnchor.NavigateUrl = this.UrlWithoutTrailingCategoryId + this.CurrentPost.EffectiveCategoryId;
				}
			}
			if(this.TableColumnEffectivelyVisibleVisible) {
				ctrlEffectivelyVisibleSpan.InnerText = this.CurrentPost.EffectivelyVisible ? this.TableValueEffectivelyVisibleTrueText : this.TableValueEffectivelyVisibleFalseText;
				AddCssClass(ctrlEffectivelyVisible, this.CurrentPost.EffectivelyVisible ? CssclassYes : CssclassNo);
			}
			if(this.TableColumnPostCountVisible) {
				ctrlPostCount.InnerText = this.CurrentPost.PostCount.ToString(NumberFormatInfo.CurrentInfo);
			}
			if(this.TableColumnRootPostTitleVisible) {
				ctrlRootPostTitleAnchor.Text = this.CurrentPost.RootPostTitle.Length > this.MaximumRootPostTitleLength ? this.CurrentPost.RootPostTitle.Substring(0, this.MaximumRootPostTitleLength) + this.OmissionIndicator : this.CurrentPost.RootPostTitle;
				if(!(ctrlRootPostTitleAnchor.NoLink = this.UrlWithoutTrailingRootPostId == null)) {
					ctrlRootPostTitleAnchor.NavigateUrl = this.UrlWithoutTrailingRootPostId + this.CurrentPost.RootPostId;
				}
			}
			if(this.TableColumnSearchRelevanceVisible) {
				ctrlSearchRelevance.InnerText = this.CurrentPost.SearchRelevance.ToString(formatPercent, NumberFormatInfo.CurrentInfo);
			}
			if(this.TableColumnTimeLastPostVisible) {
				ctrlTimeLastPost.InnerText = this.DateFormatter.FormatDate(this.CurrentPost.TimeLastPost);
			}
			if(this.TableColumnTimePostedVisible) {
				ctrlTimePosted.InnerText = this.DateFormatter.FormatDate(this.CurrentPost.TimePosted);
			}
			if(this.TableColumnTitleVisible) {
				ctrlTitleAnchor.Text = this.CurrentPost.Title.Length > this.MaximumTitleLength ? this.CurrentPost.Title.Substring(0, this.MaximumTitleLength) + this.OmissionIndicator : this.CurrentPost.Title;
				if(!(ctrlTitleAnchor.NoLink = this.UrlWithoutTrailingPostId == null && this.UrlWithoutTrailingRootPostId == null)) {
					//if UrlWithoutTrailingPostId is not specified, then use UrlWithoutTrailingRootPostId instead
					ctrlTitleAnchor.NavigateUrl = this.UrlWithoutTrailingPostId == null ? this.UrlWithoutTrailingRootPostId + this.CurrentPost.RootPostId : this.UrlWithoutTrailingPostId + this.CurrentPost.Id;
				}
			}
			if(this.TableColumnVisibleVisible) {
				ctrlVisibleSpan.InnerText = this.CurrentPost.Visible ? this.TableValueVisibleTrueText : this.TableValueVisibleFalseText;
				AddCssClass(ctrlVisible, this.CurrentPost.Visible ? CssclassYes : CssclassNo);
			}
			AddCssClass(ctrlTableRow, this.AlternatingItem ? CssclassEven : CssclassOdd);
			if(!this.CurrentPost.Visible) {
				AddCssClass(ctrlTableRow, CssclassHidden);
			} else if(!this.CurrentPost.EffectivelyVisible) {
				AddCssClass(ctrlTableRow, CssclassHiddenbyhidden);
			}
		}
		#endregion

		#region Helper Methods
		private static void AddCssClass(WebGenericControl control, string cssClass) {
			const string space = " ";
			if(control.CssClass.Length > 0) {
				control.CssClass += space;
			}
			control.CssClass += cssClass;
		}
		#endregion
	}

}