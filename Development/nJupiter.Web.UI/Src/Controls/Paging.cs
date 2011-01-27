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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Globalization;
using System.ComponentModel;

using nJupiter.Web.UI.Events;

namespace nJupiter.Web.UI.Controls {

	[ToolboxItem(true)]
	[ParseChildren(true), PersistChildren(false)]
	public class Paging : UserControl {
		#region Constants
		private const string ListCollectionKey = "v_ListCollection";
		#endregion

		#region Static Holders
		private static readonly object EventPagingChanged = new object();

		private readonly object padlock = new object();
		#endregion

		#region Events
		public event PagingEventHandler PagingChanged {
			add { base.Events.AddHandler(Paging.EventPagingChanged, value); }
			remove { base.Events.RemoveHandler(Paging.EventPagingChanged, value); }
		}
		#endregion

		#region Enums
		public enum PagingType {
			Buttons = 0,
			Links = 1,
			Anchors = 2
		}
		#endregion

		#region Constants
#if DEBUG
		private const string	DebugPrefix = "_";
#else
		private const string DebugPrefix = "";
#endif
		#endregion

		#region UI Members
		private readonly Repeater rptPaging = new Repeater();
		#endregion

		#region Members
		private int itemsPerPage = 10;
		private int numberOfPages = 5;
		private int visiblePages = 5;
		private int count;
		private int currentPageNumber;
		private int index;
		private bool visibleSet;
		private string wrapPlaceHoldersTag = HtmlTag.Span;
		private string fromCssClass = "from";
		private string toCssClass = "to";
		private string totalCountCssClass = "total-count";
		private string numberOfPagesCssClass = "number-of-pages";
		private string currentPageCssClass = string.Empty;
		private string resultText = DebugPrefix + "{0}-{1} of {2}";
		private string numberOfPagesText = DebugPrefix + "{0} pages:";
		private string nextPageText = DebugPrefix + "Next page";
		private string previousPageText = DebugPrefix + "Previous page";
		private string nextIncrementText = DebugPrefix + "...";
		private string previousIncrementText = DebugPrefix + "...";
		private string nextPageToolTipText = string.Empty;
		private string previousPageToolTipText = string.Empty;
		private string nextIncrementToolTipText = string.Empty;
		private string previousIncrementToolTipText = string.Empty;

		private string pageUrl;
		private string pageNumberQueryKey = "pageNumber";
		#endregion

		#region Properties
		public int Count { get { return this.count; } set { this.count = value; } }
		public int CurrentPageNumber { get { return this.currentPageNumber; } set { this.currentPageNumber = value; } }
		public int ItemsPerPage { get { return this.itemsPerPage; } set { this.itemsPerPage = value; } }
		public int VisiblePages { get { return this.visiblePages; } set { this.visiblePages = value; } }
		public int NumberOfPages { get { return this.numberOfPages; } set { this.numberOfPages = value; } }
		public PagingType Type { get; set; }
		public virtual bool WrapAround { get; set; }
		public virtual bool WrapPlaceHoldersInTags { get; set; }
		public virtual string WrapPlaceHoldersTag { get { return this.wrapPlaceHoldersTag; } set { this.wrapPlaceHoldersTag = value; } }
		public virtual string FromCssClass { get { return this.fromCssClass; } set { this.fromCssClass = value; } }
		public virtual string ToCssClass { get { return this.toCssClass; } set { this.toCssClass = value; } }
		public virtual string TotalCountCssClass { get { return this.totalCountCssClass; } set { this.totalCountCssClass = value; } }
		public virtual string NumberOfPagesCssClass { get { return this.numberOfPagesCssClass; } set { this.numberOfPagesCssClass = value; } }
		public virtual bool HideNumberOfPages { get; set; }
		public virtual bool HideResult { get; set; }
		public virtual bool HideDisabledButtons { get; set; }
		public virtual bool CurrentPageForceInnerSpan { get; set; }
		public virtual string CurrentPageCssClass { get { return this.currentPageCssClass; } set { this.currentPageCssClass = value;} }
		public virtual string ResultText { get { return this.resultText; } set { this.resultText = value; } }
		public virtual string NumberOfPagesText { get { return this.numberOfPagesText; } set { this.numberOfPagesText = value; } }
		public virtual string PreviousPageText { get { return this.previousPageText; } set { this.previousPageText = value; } }
		public virtual string NextPageText { get { return this.nextPageText; } set { this.nextPageText = value; } }
		public virtual string NextIncrementText { get { return this.nextIncrementText; } set { this.nextIncrementText = value; } }
		public virtual string PreviousIncrementText { get { return this.previousIncrementText; } set { this.previousIncrementText = value; } }
		public virtual string PreviousPageToolTipText { get { return this.previousPageToolTipText; } set { this.previousPageToolTipText = value; } }
		public virtual string NextPageToolTipText { get { return this.nextPageToolTipText; } set { this.nextPageToolTipText = value; } }
		public virtual string NextIncrementToolTipText { get { return this.nextIncrementToolTipText; } set { this.nextIncrementToolTipText = value; } }
		public virtual string PreviousIncrementToolTipText { get { return this.previousIncrementToolTipText; } set { this.previousIncrementToolTipText = value; } }
		public virtual string PageNumberQueryKey { get { return this.pageNumberQueryKey; } set { this.pageNumberQueryKey = value; } }

		public override bool Visible {
			get { return base.Visible; }
			set {
				base.Visible = value;
				this.visibleSet = true;
			}
		}

		private string PageUrl {
			get {
				if(this.pageUrl != null)
					return this.pageUrl;
				lock(this.padlock) {
					if(this.pageUrl == null) {
						if(this.Request.HttpMethod.Equals("GET") || this.Type == PagingType.Anchors) {
							this.pageUrl = UrlHandler.AddQueryParams(UrlHandler.CurrentPath, UrlHandler.GetQueryString(UrlHandler.CurrentQueryString, true, this.PageNumberQueryKey));
						} else {
							ArrayList systemKeys = new ArrayList();
							foreach(string key in UrlHandler.CurrentForm.Keys) {
								if(key.StartsWith("__") || key.IndexOf('$') > 0) {
									systemKeys.Add(key);
								}
							}
							this.pageUrl = UrlHandler.AddQueryParams(UrlHandler.CurrentPath, UrlHandler.GetQueryString(UrlHandler.CurrentForm, (string[])systemKeys.ToArray(typeof(string))));
							string[] keysToRemove = new string[UrlHandler.CurrentForm.AllKeys.Length + 1];
							UrlHandler.CurrentForm.AllKeys.CopyTo(keysToRemove, 0);
							keysToRemove[UrlHandler.CurrentForm.AllKeys.Length] = this.PageNumberQueryKey;
							this.pageUrl = UrlHandler.AddQueryParams(this.pageUrl, UrlHandler.GetQueryString(UrlHandler.CurrentQueryString, keysToRemove));
						}
					}
				}
				return this.pageUrl;
			}
		}

		[TemplateContainer(typeof(RepeaterItem)), PersistenceMode(PersistenceMode.InnerProperty)]
		public ITemplate HeaderTemplate {
			get {
				return rptPaging.HeaderTemplate;
			}
			set {
				rptPaging.HeaderTemplate = value;
			}
		}

		[TemplateContainer(typeof(RepeaterItem)), PersistenceMode(PersistenceMode.InnerProperty)]
		public ITemplate ItemTemplate {
			get {
				return rptPaging.ItemTemplate;
			}
			set {
				rptPaging.ItemTemplate = value;
			}
		}

		[TemplateContainer(typeof(RepeaterItem)), PersistenceMode(PersistenceMode.InnerProperty)]
		public ITemplate AlternatingItemTemplate {
			get {
				return rptPaging.AlternatingItemTemplate;
			}
			set {
				rptPaging.AlternatingItemTemplate = value;
			}
		}

		[TemplateContainer(typeof(RepeaterItem)), PersistenceMode(PersistenceMode.InnerProperty)]
		public ITemplate SeparatorTemplate {
			get {
				return rptPaging.SeparatorTemplate;
			}
			set {
				rptPaging.SeparatorTemplate = value;
			}
		}

		[TemplateContainer(typeof(RepeaterItem)), PersistenceMode(PersistenceMode.InnerProperty)]
		public ITemplate FooterTemplate {
			get {
				return rptPaging.FooterTemplate;
			}
			set {
				rptPaging.FooterTemplate = value;
			}
		}

		#endregion

		#region Event Handlers
		private void PagingItemCommand(object source, RepeaterCommandEventArgs e) {
			OnPagingChanged(new PagingEventArgs(int.Parse(e.CommandArgument.ToString(), NumberFormatInfo.InvariantInfo)));
		}

		private void PagingItemDataBound(object sender, RepeaterItemEventArgs e) {
			switch(e.Item.ItemType) {
				case ListItemType.Header:
					int from = ((this.currentPageNumber - 1) * this.itemsPerPage) + 1;
					int to = Math.Min(this.count, from + this.itemsPerPage - 1);
					int totalPageCountHeader = (int)Math.Ceiling(this.count / (double)ItemsPerPage);

					WebParagraph resultTextParagraph = e.Item.FindControl("resultText") as WebParagraph;
					if(resultTextParagraph != null) {
						if(this.HideResult) {
							resultTextParagraph.Visible = false;
						} else {
							if(this.WrapPlaceHoldersInTags) {
								string tagFormat = "<" + this.WrapPlaceHoldersTag + " " + HtmlAttribute.Class + "=\"{0}\">{1}</" + this.WrapPlaceHoldersTag + ">";
								resultTextParagraph.InnerHtml = string.Format(CultureInfo.CurrentCulture, this.Server.HtmlEncode(this.ResultText),
									string.Format(CultureInfo.CurrentCulture, tagFormat, this.FromCssClass, from),
									string.Format(CultureInfo.CurrentCulture, tagFormat, this.ToCssClass, to),
									string.Format(CultureInfo.CurrentCulture, tagFormat, this.TotalCountCssClass, this.count));
							} else {
								resultTextParagraph.InnerText = string.Format(CultureInfo.CurrentCulture, this.ResultText, from, to, this.count);
							}
						}
					}

					WebParagraph numberOfPagesTextParagraph = e.Item.FindControl("numberOfPagesText") as WebParagraph;
					if(numberOfPagesTextParagraph != null) {
						if(this.HideResult) {
							numberOfPagesTextParagraph.Visible = false;
						} else {
							if(this.WrapPlaceHoldersInTags) {
								string tagFormat = "<" + this.WrapPlaceHoldersTag + " " + HtmlAttribute.Class + "=\"{0}\">{1}</" + this.WrapPlaceHoldersTag + ">";
								numberOfPagesTextParagraph.InnerHtml = string.Format(CultureInfo.CurrentCulture, this.Server.HtmlEncode(this.NumberOfPagesText),
									string.Format(CultureInfo.CurrentCulture, tagFormat, this.NumberOfPagesCssClass, totalPageCountHeader));
							} else {
								numberOfPagesTextParagraph.InnerText = string.Format(CultureInfo.CurrentCulture, this.NumberOfPagesText, totalPageCountHeader);
							}
						}
					}
					Control prev = e.Item.FindControl("previousPage");
					Control prevIncr = e.Item.FindControl("previousIncrement");

					if(this.currentPageNumber > 0) {
						WebButton buttonPrev = prev as WebButton;
						WebLinkButton linkButtonPrev = prev as WebLinkButton;
						WebAnchor ancPrev = prev as WebAnchor;

						if(buttonPrev != null) {
							buttonPrev.InnerText = this.PreviousPageText;
							if(!string.IsNullOrEmpty(this.PreviousPageToolTipText)) {
								buttonPrev.Attributes.Add(HtmlAttribute.Title, this.PreviousPageToolTipText);
							}
							if(this.currentPageNumber > 1 || this.WrapAround) {
								string pageNum = (this.currentPageNumber > 1 ? this.currentPageNumber - 1 : totalPageCountHeader).ToString(NumberFormatInfo.InvariantInfo);
								buttonPrev.CommandArgument = pageNum;
								buttonPrev.Disabled = false;
							} else if(this.HideDisabledButtons) {
								buttonPrev.Visible = false;
							} else {
								buttonPrev.Disabled = true;
							}
						}
						if(linkButtonPrev != null) {
							linkButtonPrev.Text = this.PreviousPageText;
							if(!string.IsNullOrEmpty(this.PreviousPageToolTipText)) {
								linkButtonPrev.ToolTip = this.PreviousPageToolTipText;
							}
							if(this.currentPageNumber > 1 || this.WrapAround) {
								string pageNum = (this.currentPageNumber > 1 ? this.currentPageNumber - 1 : totalPageCountHeader).ToString(NumberFormatInfo.InvariantInfo);
								linkButtonPrev.NavigateUrl = UrlHandler.AddQueryKeyValue(this.PageUrl, this.PageNumberQueryKey, pageNum);
								linkButtonPrev.CommandArgument = pageNum;
								linkButtonPrev.NoLink = false;
							} else if(this.HideDisabledButtons) {
								linkButtonPrev.Visible = false;
							} else {
								linkButtonPrev.NoLink = true;
							}
						}
						if(ancPrev != null) {
							ancPrev.Text = this.PreviousPageText;
							if(!string.IsNullOrEmpty(this.PreviousPageToolTipText)) {
								ancPrev.ToolTip = this.PreviousPageToolTipText;
							}
							if(this.currentPageNumber > 1 || this.WrapAround) {
								string pageNum = (this.currentPageNumber > 1 ? this.currentPageNumber - 1 : totalPageCountHeader).ToString(NumberFormatInfo.InvariantInfo);
								ancPrev.NavigateUrl = UrlHandler.AddQueryKeyValue(this.PageUrl, this.PageNumberQueryKey, pageNum);
								ancPrev.NoLink = false;
							} else if(this.HideDisabledButtons) {
								ancPrev.Visible = false;
							} else {
								ancPrev.NoLink = true;
							}
						}
						WebButton buttonPrevIncr = prevIncr as WebButton;
						WebLinkButton linkButtonPrevIncr = prevIncr as WebLinkButton;
						WebAnchor ancPrevIncr = prevIncr as WebAnchor;
						if(buttonPrevIncr != null) {
							buttonPrevIncr.InnerText = this.PreviousIncrementText;
							if(!string.IsNullOrEmpty(this.PreviousIncrementToolTipText)) {
								buttonPrevIncr.Attributes.Add(HtmlAttribute.Title, this.PreviousIncrementToolTipText);
							}
							if(this.index > 1 || this.WrapAround) {
								string pageNum = (this.index > 1 ? this.index - this.VisiblePages : totalPageCountHeader).ToString(NumberFormatInfo.InvariantInfo);
								buttonPrevIncr.CommandArgument = pageNum;
								buttonPrevIncr.Visible = true;
							} else {
								buttonPrevIncr.Visible = false;
							}
						}
						if(linkButtonPrevIncr != null) {
							linkButtonPrevIncr.Text = this.PreviousIncrementText;
							if(!string.IsNullOrEmpty(this.PreviousIncrementToolTipText)) {
								linkButtonPrevIncr.ToolTip = this.PreviousIncrementToolTipText;
							}
							if(this.index > 1 || this.WrapAround) {
								string pageNum = (this.index > 1 ? this.index - this.VisiblePages : totalPageCountHeader).ToString(NumberFormatInfo.InvariantInfo);
								linkButtonPrevIncr.NavigateUrl = UrlHandler.AddQueryKeyValue(this.PageUrl, this.PageNumberQueryKey, pageNum);
								linkButtonPrevIncr.CommandArgument = pageNum;
								linkButtonPrevIncr.Visible = true;
							} else {
								linkButtonPrevIncr.Visible = false;
							}
						}
						if(ancPrevIncr != null) {
							ancPrevIncr.Text = this.PreviousIncrementText;
							if(!string.IsNullOrEmpty(this.PreviousIncrementToolTipText)) {
								ancPrevIncr.ToolTip = this.PreviousIncrementToolTipText;
							}
							if(this.index > 1 || this.WrapAround) {
								string pageNum = (this.index > 1 ? this.index - this.VisiblePages : totalPageCountHeader).ToString(NumberFormatInfo.InvariantInfo);
								ancPrevIncr.NavigateUrl = UrlHandler.AddQueryKeyValue(this.PageUrl, this.PageNumberQueryKey, pageNum);
								ancPrevIncr.Visible = true;
							} else {
								ancPrevIncr.Visible = false;
							}
						}
					}
					break;
				case ListItemType.Item:
				case ListItemType.AlternatingItem:
					WebButton button = (WebButton)ControlHandler.FindFirstControlOnType(e.Item, typeof(WebButton));
					WebLinkButton linkButton = (WebLinkButton)ControlHandler.FindFirstControlOnType(e.Item, typeof(WebLinkButton));
					WebAnchor anchor = (WebAnchor)ControlHandler.FindFirstControlOnType(e.Item, typeof(WebAnchor));

					int pageNumber = (int)e.Item.DataItem;
					bool isCurrentPage = false;
					bool visible = true;
					string buttonText = null;
					string buttonCssClass = null;
					string navigateUrl;
					if(pageNumber.Equals(this.currentPageNumber)) {
						buttonText = this.currentPageNumber.ToString(NumberFormatInfo.CurrentInfo);
						buttonCssClass = this.CurrentPageCssClass;
						isCurrentPage = true;
					} else if(pageNumber >= this.index && pageNumber <= this.index + this.VisiblePages - 1) {
						buttonText = pageNumber.ToString(NumberFormatInfo.CurrentInfo);
					} else {
						visible = false;
					}
					if(button != null && (button.Visible = visible)) {
						button.CommandArgument = pageNumber.ToString(NumberFormatInfo.InvariantInfo);
						button.InnerText = buttonText;
						button.Disabled = isCurrentPage;
						if(isCurrentPage && !button.InnerSpan) {
							button.InnerSpan = this.CurrentPageForceInnerSpan;
						}
						if(!string.IsNullOrEmpty(buttonCssClass)) {
							button.CssClass += (!string.IsNullOrEmpty(button.CssClass)?  " " : string.Empty) + buttonCssClass;
						}
					}
					if(this.PageUrl.Contains(this.PageNumberQueryKey)) {
						navigateUrl = UrlHandler.ReplaceQueryKeyValue(this.PageUrl, this.PageNumberQueryKey, pageNumber.ToString(NumberFormatInfo.InvariantInfo));
					} else {
						navigateUrl = UrlHandler.AddQueryKeyValue(this.PageUrl, this.PageNumberQueryKey, pageNumber.ToString(NumberFormatInfo.InvariantInfo));
					}
					if(linkButton != null && (linkButton.Visible = visible)) {
						linkButton.NavigateUrl = navigateUrl;
						linkButton.CommandArgument = pageNumber.ToString(NumberFormatInfo.InvariantInfo);
						linkButton.Text = buttonText;
						linkButton.NoLink = isCurrentPage;
						if(isCurrentPage && !linkButton.InnerSpan) {
							linkButton.InnerSpan = this.CurrentPageForceInnerSpan;
						}
						if(!string.IsNullOrEmpty(buttonCssClass)) {
							linkButton.CssClass += (!string.IsNullOrEmpty(linkButton.CssClass)?  " " : string.Empty) + buttonCssClass;
						}
					}
					if(anchor != null && (anchor.Visible = visible)) {
						anchor.NavigateUrl = navigateUrl;
						anchor.Text = buttonText;
						anchor.NoLink = isCurrentPage;
						if(isCurrentPage && !anchor.InnerSpan) {
							anchor.InnerSpan = this.CurrentPageForceInnerSpan;
						}
						if(!string.IsNullOrEmpty(buttonCssClass)) {
							anchor.CssClass += (!string.IsNullOrEmpty(anchor.CssClass)?  " " : string.Empty) + buttonCssClass;
						}
					}
					break;
				case ListItemType.Footer:
					int totalPageCountFooter = (int)Math.Ceiling(this.count / (double)ItemsPerPage);
					Control next = e.Item.FindControl("nextPage");
					Control nextIncr = e.Item.FindControl("nextIncrement");

					if(this.currentPageNumber > 0) {
						WebButton buttonNext = next as WebButton;
						WebLinkButton linkButtonNext = next as WebLinkButton;
						WebAnchor ancNext = next as WebAnchor;
						if(buttonNext != null) {
							buttonNext.InnerText = this.NextPageText;
							if(!string.IsNullOrEmpty(this.NextPageToolTipText)) {
								buttonNext.Attributes.Add(HtmlAttribute.Title, this.NextPageToolTipText);
							}
							if(this.currentPageNumber != totalPageCountFooter || this.WrapAround) {
								string pageNum = (this.currentPageNumber != totalPageCountFooter ? this.currentPageNumber + 1 : 1).ToString(NumberFormatInfo.InvariantInfo);
								buttonNext.CommandArgument = pageNum;
								buttonNext.Disabled = false;
							} else if(this.HideDisabledButtons) {
								buttonNext.Visible = false;
							} else {
								buttonNext.Disabled = true;
							}
						}
						if(linkButtonNext != null) {
							linkButtonNext.Text = this.NextPageText;
							if(!string.IsNullOrEmpty(this.NextPageToolTipText)) {
								linkButtonNext.ToolTip = this.NextPageToolTipText;
							}
							if(this.currentPageNumber != totalPageCountFooter || this.WrapAround) {
								string pageNum = (this.currentPageNumber != totalPageCountFooter ? this.currentPageNumber + 1 : 1).ToString(NumberFormatInfo.InvariantInfo);
								linkButtonNext.NavigateUrl = UrlHandler.AddQueryKeyValue(this.PageUrl, this.PageNumberQueryKey, pageNum);
								linkButtonNext.CommandArgument = pageNum;
								linkButtonNext.NoLink = false;
							} else if(this.HideDisabledButtons) {
								linkButtonNext.Visible = false;
							} else {
								linkButtonNext.NoLink = true;
							}
						}
						if(ancNext != null) {
							ancNext.Text = this.NextPageText;
							if(!string.IsNullOrEmpty(this.NextPageToolTipText)) {
								ancNext.ToolTip = this.NextPageToolTipText;
							}
							if(this.currentPageNumber != totalPageCountFooter || this.WrapAround) {
								string pageNum = (this.currentPageNumber != totalPageCountFooter ? this.currentPageNumber + 1 : 1).ToString(NumberFormatInfo.InvariantInfo);
								ancNext.NavigateUrl = UrlHandler.AddQueryKeyValue(this.PageUrl, this.PageNumberQueryKey, pageNum);
								ancNext.NoLink = false;
							} else if(this.HideDisabledButtons) {
								ancNext.Visible = false;
							} else {
								ancNext.NoLink = true;
							}
						}

						WebButton buttonNextIncr = nextIncr as WebButton;
						WebLinkButton linkButtonNextIncr = nextIncr as WebLinkButton;
						WebAnchor ancNextIncr = nextIncr as WebAnchor;
						if(buttonNextIncr != null) {
							buttonNextIncr.InnerText = this.NextIncrementText;
							if(!string.IsNullOrEmpty(this.NextIncrementToolTipText)) {
								buttonNextIncr.Attributes.Add(HtmlAttribute.Title, this.NextIncrementToolTipText);
							}
							if(this.index + this.VisiblePages - 1 < totalPageCountFooter || this.WrapAround) {
								string pageNum = (this.index + this.VisiblePages - 1 < totalPageCountFooter ? this.index + this.VisiblePages : 1).ToString(NumberFormatInfo.InvariantInfo);
								buttonNextIncr.CommandArgument = pageNum;
								buttonNextIncr.Visible = true;
							} else {
								buttonNextIncr.Visible = false;
							}
						}
						if(linkButtonNextIncr != null) {
							linkButtonNextIncr.Text = this.NextIncrementText;
							if(!string.IsNullOrEmpty(this.NextIncrementToolTipText)) {
								linkButtonNextIncr.ToolTip = this.NextIncrementToolTipText;
							}
							if(this.index + this.VisiblePages - 1 < totalPageCountFooter || this.WrapAround) {
								string pageNum = (this.index + this.VisiblePages - 1 < totalPageCountFooter ? this.index + this.VisiblePages : 1).ToString(NumberFormatInfo.InvariantInfo);
								linkButtonNextIncr.NavigateUrl = UrlHandler.AddQueryKeyValue(this.PageUrl, this.PageNumberQueryKey, pageNum);
								linkButtonNextIncr.CommandArgument = pageNum;
								linkButtonNextIncr.Visible = true;
							} else {
								linkButtonNextIncr.Visible = false;
							}
						}
						if(ancNextIncr != null) {
							ancNextIncr.Text = this.NextIncrementText;
							if(!string.IsNullOrEmpty(this.NextIncrementToolTipText)) {
								ancNextIncr.ToolTip = this.NextIncrementToolTipText;
							}
							if(this.index + this.VisiblePages - 1 < totalPageCountFooter || this.WrapAround) {
								string pageNum = (this.index + this.VisiblePages - 1 < totalPageCountFooter ? this.index + this.VisiblePages : 1).ToString(NumberFormatInfo.InvariantInfo);
								ancNextIncr.NavigateUrl = UrlHandler.AddQueryKeyValue(this.PageUrl, this.PageNumberQueryKey, pageNum);
								ancNextIncr.Visible = true;
							} else {
								ancNextIncr.Visible = false;
							}
						}
					}
					break;
			}
		}
		#endregion

		#region Event Activators
		protected override void LoadViewState(object savedState) {
			base.LoadViewState(savedState);
			if(IsPostBack && rptPaging != null && this.ViewState[ListCollectionKey] != null) {
				int[] viewStatedList = (int[])this.ViewState[ListCollectionKey];
				rptPaging.DataSource = viewStatedList;
				rptPaging.DataBind();
			}
		}

		protected override void OnInit(EventArgs e) {
			base.OnInit(e);

			if(this.HeaderTemplate == null) {
				switch(this.Type) {
					case PagingType.Anchors:
					this.HeaderTemplate = new DefaultAnchorHeaderTemplate();
					break;
					case PagingType.Buttons:
					this.HeaderTemplate = new DefaultButtonHeaderTemplate();
					break;
					case PagingType.Links:
					this.HeaderTemplate = new DefaultLinkHeaderTemplate();
					break;
				}
			}
			if(this.ItemTemplate == null) {
				switch(this.Type) {
					case PagingType.Anchors:
					this.ItemTemplate = new DefaultAnchorItemTemplate();
					break;
					case PagingType.Buttons:
					this.ItemTemplate = new DefaultButtonItemTemplate();
					break;
					case PagingType.Links:
					this.ItemTemplate = new DefaultLinkItemTemplate();
					break;
				}
			}
			if(this.FooterTemplate == null) {
				switch(this.Type) {
					case PagingType.Anchors:
					this.FooterTemplate = new DefaultAnchorFooterTemplate();
					break;
					case PagingType.Buttons:
					this.FooterTemplate = new DefaultButtonFooterTemplate();
					break;
					case PagingType.Links:
					this.FooterTemplate = new DefaultLinkFooterTemplate();
					break;
				}
			}
			this.Controls.Add(rptPaging);
			rptPaging.ItemDataBound += this.PagingItemDataBound;
			rptPaging.ItemCommand += this.PagingItemCommand;
		}

		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			if(!this.IsPostBack && this.Request[this.PageNumberQueryKey] != null) {
				try {
					int pageNumber = int.Parse(this.Request[this.PageNumberQueryKey], NumberFormatInfo.InvariantInfo);
					this.OnPagingChanged(new PagingEventArgs(pageNumber));
				} catch(FormatException ex) {
					//We do not want the original exception to be logged as fatal, therefore we wrap the exception in an http exception
					throw new HttpException(500, ex.Message, ex);
				}
			}
		}

		protected virtual void OnPagingChanged(PagingEventArgs e) {
			PagingEventHandler eventHandler = base.Events[Paging.EventPagingChanged] as PagingEventHandler;
			if(eventHandler != null) {
				eventHandler(this, e);
			}
		}
		#endregion

		#region Overridden Methods
		public override void DataBind() {
			if(!this.visibleSet) {
				this.Visible = this.count > 0;
			}

			this.index = this.currentPageNumber - ((this.currentPageNumber - 1) % this.VisiblePages);
			int totalPageCount = (int)Math.Ceiling(this.count / (double)ItemsPerPage);
			int howMany = Math.Min(totalPageCount - this.index + 1, this.VisiblePages);
			int[] pageNumbers = new int[howMany];
			for(int i = 0; i < howMany; i++) {
				pageNumbers[i] = this.index + i;
			}

			rptPaging.DataSource = this.ViewState[ListCollectionKey] = pageNumbers;
			rptPaging.DataBind();
		}
		#endregion

		#region Inner Classes
		private sealed class DefaultButtonItemTemplate : ITemplate {
			public void InstantiateIn(Control container) {
				WebButton button = new WebButton();
				button.CssClass = "page-number";
				button.InnerSpan = true;
				button.TrailingLinefeed = true;
				container.Controls.Add(button);
			}
		}
		private sealed class DefaultLinkItemTemplate : ITemplate {
			public void InstantiateIn(Control container) {
				WebLinkButton link = new WebLinkButton();
				link.CssClass = "page-number";
				link.InnerSpan = true;
				link.TrailingLinefeed = true;
				container.Controls.Add(link);
			}
		}
		private sealed class DefaultAnchorItemTemplate : ITemplate {
			public void InstantiateIn(Control container) {
				WebAnchor anchor = new WebAnchor();
				anchor.CssClass = "page-number";
				anchor.InnerSpan = true;
				anchor.TrailingLinefeed = true;
				container.Controls.Add(anchor);
			}
		}

		private abstract class DefaultHeaderTemplateBase : ITemplate {
			public void InstantiateIn(Control container) {
				WebPlaceHolder wrapperBeginning = new WebPlaceHolder();
				wrapperBeginning.InnerHtml = @"<div class=""paging"">";
				container.Controls.Add(wrapperBeginning);

				WebParagraph resultText = new WebParagraph();
				resultText.ID = "resultText";
				resultText.CssClass = "paging-result";
				container.Controls.Add(resultText);

				WebParagraph numberOfPagesText = new WebParagraph();
				numberOfPagesText.ID = "numberOfPagesText";
				numberOfPagesText.CssClass = "paging-pages";
				container.Controls.Add(numberOfPagesText);

				WebPlaceHolder buttonWrapperBeginning = new WebPlaceHolder();
				buttonWrapperBeginning.InnerHtml = @"<p class=""paging-buttons"">";
				container.Controls.Add(buttonWrapperBeginning);

				this.InstantiateInputControls(buttonWrapperBeginning);
			}
			public abstract void InstantiateInputControls(Control container);
		}
		private sealed class DefaultButtonHeaderTemplate : DefaultHeaderTemplateBase {
			public override void InstantiateInputControls(Control container) {
				WebButton previousPage = new WebButton();
				previousPage.InnerSpan = true;
				previousPage.TrailingLinefeed = true;
				previousPage.CssClass = "prev";
				previousPage.ID = "previousPage";
				container.Controls.Add(previousPage);

				WebButton previousIncrement = new WebButton();
				previousIncrement.InnerSpan = true;
				previousIncrement.TrailingLinefeed = true;
				previousIncrement.CssClass = "prev-incr";
				previousIncrement.ID = "previousIncrement";
				container.Controls.Add(previousIncrement);
			}
		}
		private sealed class DefaultLinkHeaderTemplate : DefaultHeaderTemplateBase {
			public override void InstantiateInputControls(Control container) {
				WebLinkButton previousPage = new WebLinkButton();
				previousPage.InnerSpan = true;
				previousPage.TrailingLinefeed = true;
				previousPage.CssClass = "prev";
				previousPage.ID = "previousPage";
				container.Controls.Add(previousPage);

				WebLinkButton previousIncrement = new WebLinkButton();
				previousIncrement.InnerSpan = true;
				previousIncrement.TrailingLinefeed = true;
				previousIncrement.CssClass = "prev-incr";
				previousIncrement.ID = "previousIncrement";
				container.Controls.Add(previousIncrement);
			}
		}
		private sealed class DefaultAnchorHeaderTemplate : DefaultHeaderTemplateBase {
			public override void InstantiateInputControls(Control container) {
				WebAnchor previousPage = new WebAnchor();
				previousPage.InnerSpan = true;
				previousPage.TrailingLinefeed = true;
				previousPage.CssClass = "prev";
				previousPage.ID = "previousPage";
				container.Controls.Add(previousPage);

				WebAnchor previousIncrement = new WebAnchor();
				previousIncrement.InnerSpan = true;
				previousIncrement.TrailingLinefeed = true;
				previousIncrement.CssClass = "prev-incr";
				previousIncrement.ID = "previousIncrement";
				container.Controls.Add(previousIncrement);
			}
		}

		private abstract class DefaultFooterTemplateBase : ITemplate {
			public void InstantiateIn(Control container) {
				this.InstantiateInputControls(container);

				WebPlaceHolder buttonWrapperEnd = new WebPlaceHolder();
				buttonWrapperEnd.InnerHtml = @"</p>";
				container.Controls.Add(buttonWrapperEnd);

				WebPlaceHolder wrapperEnd = new WebPlaceHolder();
				wrapperEnd.InnerHtml = "</div>";
				container.Controls.Add(wrapperEnd);

				WebGenericControl clearer = new WebGenericControl(HtmlTag.Div);
				clearer.CssClass = "clearer";
				clearer.InnerHtml = "&nbsp;";
				container.Controls.Add(clearer);
			}

			protected abstract void InstantiateInputControls(Control container);
		}
		private sealed class DefaultButtonFooterTemplate : DefaultFooterTemplateBase {
			protected override void InstantiateInputControls(Control container) {
				WebButton nextIncrement = new WebButton();
				nextIncrement.InnerSpan = true;
				nextIncrement.TrailingLinefeed = true;
				nextIncrement.CssClass = "next-incr";
				nextIncrement.ID = "nextIncrement";
				container.Controls.Add(nextIncrement);

				WebButton nextPage = new WebButton();
				nextPage.InnerSpan = true;
				nextPage.TrailingLinefeed = true;
				nextPage.CssClass = "next";
				nextPage.ID = "nextPage";
				container.Controls.Add(nextPage);
			}
		}
		private sealed class DefaultLinkFooterTemplate : DefaultFooterTemplateBase {
			protected override void InstantiateInputControls(Control container) {
				WebLinkButton nextIncrement = new WebLinkButton();
				nextIncrement.InnerSpan = true;
				nextIncrement.TrailingLinefeed = true;
				nextIncrement.CssClass = "next-incr";
				nextIncrement.ID = "nextIncrement";
				container.Controls.Add(nextIncrement);

				WebLinkButton nextPage = new WebLinkButton();
				nextPage.InnerSpan = true;
				nextPage.TrailingLinefeed = true;
				nextPage.CssClass = "next";
				nextPage.ID = "nextPage";
				container.Controls.Add(nextPage);
			}
		}
		private sealed class DefaultAnchorFooterTemplate : DefaultFooterTemplateBase {
			protected override void InstantiateInputControls(Control container) {
				WebAnchor nextIncrement = new WebAnchor();
				nextIncrement.InnerSpan = true;
				nextIncrement.TrailingLinefeed = true;
				nextIncrement.CssClass = "next-incr";
				nextIncrement.ID = "nextIncrement";
				container.Controls.Add(nextIncrement);

				WebAnchor nextPage = new WebAnchor();
				nextPage.InnerSpan = true;
				nextPage.TrailingLinefeed = true;
				nextPage.CssClass = "next";
				nextPage.ID = "nextPage";
				container.Controls.Add(nextPage);
			}
		}
		#endregion
	}
}
