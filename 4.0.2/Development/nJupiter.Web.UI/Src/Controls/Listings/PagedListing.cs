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
using System.Globalization;
using System.Collections;

using nJupiter.Web.UI.Events;

namespace nJupiter.Web.UI.Controls.Listings {

	public class PagedListing : GeneralListing {
		#region Constants
#if DEBUG
		private const string	DebugPrefix = "_";
#else
		private const string DebugPrefix = "";
#endif

		private const string DefaultPagingresulttext = DebugPrefix + "{0}-{1} of {2}";
		private const string DefaultPagingnumberofpagestext = DebugPrefix + "{0} pages:";
		private const string DefaultPagingnextpagetext = DebugPrefix + "Next page";
		private const string DefaultPagingpreviouspagetext = DebugPrefix + "Previous page";
		private const string DefaultPagingnextincrementtext = DebugPrefix + "...";
		private const string DefaultPagingpreviousincrementtext = DebugPrefix + "...";
		#endregion

		#region Members
		private int itemsPerPage = 10;
		private int currentPageNumber = 1;
		private int pagingNumberOfPages = 5;
		private string pagingResultText = DefaultPagingresulttext;
		private string pagingNumberOfPagesText = DefaultPagingnumberofpagestext;
		private string pagingNextPageText = DefaultPagingnextpagetext;
		private string pagingPreviousPageText = DefaultPagingpreviouspagetext;
		private string pagingNextIncrementText = DefaultPagingnextincrementtext;
		private string pagingPreviousIncrementText = DefaultPagingpreviousincrementtext;
		private string pagingNextPageToolTipText = "";
		private string pagingPreviousPageToolTipText = "";
		private string pagingNextIncrementToolTipText = "";
		private string pagingPreviousIncrementToolTipText = "";

		private Paging headerPaging;
		private string headerPagingId;
		private Paging footerPaging;
		private string footerPagingId;
		private WebDropDownList numberOfItemsSelector;
		#endregion

		#region Properties
		public string PagingResultText { get { return this.pagingResultText; } set { this.pagingResultText = value; } }
		public string PagingNumberOfPagesText { get { return this.pagingNumberOfPagesText; } set { this.pagingNumberOfPagesText = value; } }
		public string PagingPreviousPageText { get { return this.pagingPreviousPageText; } set { this.pagingPreviousPageText = value; } }
		public string PagingNextPageText { get { return this.pagingNextPageText; } set { this.pagingNextPageText = value; } }
		public string PagingNextIncrementText { get { return this.pagingNextIncrementText; } set { this.pagingNextIncrementText = value; } }
		public string PagingPreviousIncrementText { get { return this.pagingPreviousIncrementText; } set { this.pagingPreviousIncrementText = value; } }
		public string PagingPreviousPageToolTipText { get { return this.pagingPreviousPageToolTipText; } set { this.pagingPreviousPageToolTipText = value; } }
		public string PagingNextPageToolTipText { get { return this.pagingNextPageToolTipText; } set { this.pagingNextPageToolTipText = value; } }
		public string PagingNextIncrementToolTipText { get { return this.pagingNextIncrementToolTipText; } set { this.pagingNextIncrementToolTipText = value; } }
		public string PagingPreviousIncrementToolTipText { get { return this.pagingPreviousIncrementToolTipText; } set { this.pagingPreviousIncrementToolTipText = value; } }

		public int PagingNumberOfPages { get { return this.pagingNumberOfPages; } set { this.pagingNumberOfPages = value; } }
		public int TotalNumberOfItems { get; set; }
		public bool DisablePaging { get; set; }
		public string HeaderPagingId { get { return this.headerPagingId; } set { this.headerPagingId = value; } }
		public string FooterPagingId { get { return this.footerPagingId; } set { this.footerPagingId = value; } }

		public Paging HeaderPaging {
			get {
				return this.headerPaging;
			}
			set {
				this.headerPaging = value;
				if(this.headerPaging != null) {
					this.headerPaging.PagingChanged += this.PagingChanged;
				}
			}
		}

		public Paging FooterPaging {
			get {
				return this.footerPaging;
			}
			set {
				this.footerPaging = value;
				if(this.footerPaging != null) {
					this.footerPaging.PagingChanged += this.PagingChanged;
				}
			}
		}


		public int CurrentPageNumber {
			get { return this.currentPageNumber; }
			set {
				if(this.HeaderPaging != null) {
					this.HeaderPaging.CurrentPageNumber = value;
				}
				if(this.FooterPaging != null) {
					this.FooterPaging.CurrentPageNumber = value;
				}
				this.currentPageNumber = value;
			}
		}
		public int ItemsPerPage {
			get {
				if(this.NumberOfItemsSelector != null && this.NumberOfItemsSelector.Items.FindByValue(this.itemsPerPage.ToString(NumberFormatInfo.CurrentInfo)) != null) {
					this.itemsPerPage = int.Parse(this.NumberOfItemsSelector.SelectedValue, NumberFormatInfo.CurrentInfo);
				}
				return this.itemsPerPage;
			}
			set { this.itemsPerPage = value; }
		}
		public WebDropDownList NumberOfItemsSelector {
			get {
				if(this.numberOfItemsSelector == null) {
					NumberOfItemsSelector dropDownControl = (NumberOfItemsSelector)ControlHandler.FindFirstControlOnType(this.HeaderControl, typeof(NumberOfItemsSelector), true);
					if(dropDownControl != null) {
						this.numberOfItemsSelector = dropDownControl;
					}
				}
				return this.numberOfItemsSelector;
			}
			set { this.numberOfItemsSelector = value; }
		}
		#endregion

		#region Event Activators
		protected override void OnInit(EventArgs e) {
			base.OnInit(e);
			if(this.headerPaging == null) {
				if(!string.IsNullOrEmpty(this.headerPagingId) && this.Parent != null) {
					this.HeaderPaging = (Paging)this.Parent.FindControl(this.headerPagingId);
				} else {
					this.HeaderPaging = (Paging)ControlHandler.FindFirstControlOnType(this.HeaderControl, typeof(Paging), true);
				}
			}
			if(this.footerPaging == null) {
				if(!string.IsNullOrEmpty(this.footerPagingId) && this.Parent != null) {
					this.FooterPaging = (Paging)this.Parent.FindControl(this.footerPagingId);
				} else {
					this.FooterPaging = (Paging)ControlHandler.FindFirstControlOnType(this.FooterControl, typeof(Paging), true);
				}
			}
		}
		#endregion

		#region Event Handlers
		protected void PagingChanged(object sender, PagingEventArgs e) {
			this.CurrentPageNumber = e.PageNumber;
		}
		#endregion

		#region Overridden Methods
		protected override void PopulateList() {
			this.DoFiltering();
			this.DoSorting();
			if(!this.DisablePaging) {
				this.TotalNumberOfItems = this.ListCollection.Count;
			}
			//protect from getting out of bounds of list (choosing ItemsPerPage and clicking next and 
			//next page being outside of bounds)
			int maxNumberOfPages = (int)Math.Ceiling(this.TotalNumberOfItems / (double)this.ItemsPerPage);
			if(this.CurrentPageNumber > maxNumberOfPages && this.CurrentPageNumber > 1) {
				this.CurrentPageNumber = maxNumberOfPages;
			}
			this.PopulatePaging();
			base.PopulateList();
			InitPaging(this.HeaderPaging);
			InitPaging(this.FooterPaging);
			DataBindPaging();
			if(this.HeaderPaging != null) {
				this.PagingResultText = this.HeaderPaging.ResultText;
				this.PagingNumberOfPagesText = this.HeaderPaging.NumberOfPagesText;
			}
			if(this.FooterPaging != null) {
				this.PagingResultText = this.FooterPaging.ResultText;
				this.PagingNumberOfPagesText = this.FooterPaging.NumberOfPagesText;
			}
		}
		#endregion

		#region Methods
		protected virtual void PopulatePaging() {
			if(!this.DisablePaging) {
				this.ListCollection = GetPagedCollection(this.InnerListCollection);
			}
		}
		protected void DataBindPaging() {
			if(this.HeaderPaging != null) {
				this.HeaderPaging.DataBind();
			}
			if(this.FooterPaging != null) {
				this.FooterPaging.DataBind();
			}
		}
		#endregion

		#region Helper Methods
		private void InitPaging(Paging paging) {
			if(paging != null) {
				paging.ItemsPerPage = this.ItemsPerPage;
				paging.Count = this.TotalNumberOfItems;
				paging.CurrentPageNumber = this.CurrentPageNumber;
				paging.ResultText = this.PagingResultText;
				paging.NumberOfPagesText = this.PagingNumberOfPagesText;
				paging.PreviousPageText = this.PagingPreviousPageText;
				paging.NextPageText = this.PagingNextPageText;
				paging.PreviousIncrementText = this.PagingPreviousIncrementText;
				paging.NextIncrementText = this.PagingNextIncrementText;
				paging.PreviousPageToolTipText = this.PagingPreviousPageToolTipText;
				paging.NextPageToolTipText = this.PagingNextPageToolTipText;
				paging.PreviousIncrementToolTipText = this.PagingPreviousIncrementToolTipText;
				paging.NextIncrementToolTipText = this.PagingNextIncrementToolTipText;
			}
		}
		private ArrayList GetPagedCollection(ArrayList list) {
			try {
				if(this.currentPageNumber == 1) {
					if(list.Count > this.ItemsPerPage) {
						list.RemoveRange(this.ItemsPerPage, list.Count - this.ItemsPerPage);
					}
				} else {
					// tail
					int index = this.currentPageNumber * this.ItemsPerPage;
					if(index < list.Count) {
						list.RemoveRange(index, list.Count - index);
					}
					list.RemoveRange(0, (this.currentPageNumber - 1) * this.ItemsPerPage);
				}
			} catch(ArgumentException) { }
			return list;
		}
		#endregion
	}
}
