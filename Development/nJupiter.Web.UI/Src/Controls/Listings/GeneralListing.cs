#region Copyright & License
// 
// 	Copyright (c) 2005-2012 nJupiter
// 
// 	Permission is hereby granted, free of charge, to any person obtaining a copy
// 	of this software and associated documentation files (the "Software"), to deal
// 	in the Software without restriction, including without limitation the rights
// 	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// 	copies of the Software, and to permit persons to whom the Software is
// 	furnished to do so, subject to the following conditions:
// 
// 	The above copyright notice and this permission notice shall be included in
// 	all copies or substantial portions of the Software.
// 
// 	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// 	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// 	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// 	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// 	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// 	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// 	THE SOFTWARE.
// 
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace nJupiter.Web.UI.Controls.Listings {
	[Obsolete("Try using System.Web.UI.WebControls.ListView instead if possible")]
	[ParseChildren(true), PersistChildren(false), ToolboxItem(true)]
	public class GeneralListing : UserControl {
		private const string ListCollectionKey = "v_ListCollection";
		private const string ViewstateList = "v_ViewStateList";
		private const string ViewstateRepeater = "v_ViewStateRepeater";

		private Repeater rptList;
		private WebPlaceHolder plhHeader;
		private WebPlaceHolder plhFooter;

		private bool filtered;
		private bool dataBound;
		private bool listSet;
		private ArrayList listCollection = new ArrayList();
		private int visibleItems;
		private ListItemCollection listItemCollection = new ListItemCollection();

		public int VisibleItems { get { return visibleItems; } }
		public IComparer Comparer { get; set; }
		public bool SuppressAutoDataBinding { get; set; }
		public ListItemCollection ListItems { get { return listItemCollection; } }

		public WebPlaceHolder HeaderControl {
			get {
				EnsureChildControls();
				return plhHeader;
			}
		}

		public WebPlaceHolder FooterControl {
			get {
				EnsureChildControls();
				return plhFooter;
			}
		}

		public RepeaterItemCollection Items { get { return RepeaterControl.Items; } }

		protected ArrayList InnerListCollection { get { return listCollection; } set { ListCollection = value; } }
		protected bool ListSet { get { return listSet; } }

		public bool ViewStateList {
			get {
				if(ViewState[ViewstateList] == null) {
					return false;
				}
				return (bool)ViewState[ViewstateList];
			}
			set { ViewState[ViewstateList] = value; }
		}

		public bool ViewStateRepeater {
			get {
				if(ViewState[ViewstateRepeater] == null) {
					return false;
				}
				return (bool)ViewState[ViewstateRepeater];
			}
			set { ViewState[ViewstateRepeater] = value; }
		}

		public ICollection ListCollection {
			get { return InnerListCollection; }
			set {
				dataBound = false;
				listSet = true;
				listCollection = value != null ? new ArrayList(value) : new ArrayList();
			}
		}

		[TemplateContainer(typeof(RepeaterItem)), PersistenceMode(PersistenceMode.InnerProperty)]
		public ITemplate HeaderTemplate { get; set; }

		[TemplateContainer(typeof(RepeaterItem)), PersistenceMode(PersistenceMode.InnerProperty)]
		public ITemplate ItemTemplate { get; set; }

		[TemplateContainer(typeof(RepeaterItem)), PersistenceMode(PersistenceMode.InnerProperty)]
		public ITemplate AlternatingItemTemplate { get; set; }

		[TemplateContainer(typeof(RepeaterItem)), PersistenceMode(PersistenceMode.InnerProperty)]
		public ITemplate SeparatorTemplate { get; set; }

		[TemplateContainer(typeof(RepeaterItem)), PersistenceMode(PersistenceMode.InnerProperty)]
		public ITemplate FooterTemplate { get; set; }

		protected Repeater RepeaterControl {
			get {
				EnsureChildControls();
				return rptList;
			}
		}

		private static readonly object EventItemCreated = new object();
		private static readonly object EventItemDataBound = new object();
		private static readonly object EventBeforeDataBinding = new object();
		private static readonly object EventAfterDataBinding = new object();
		private static readonly object EventListItemDataBinding = new object();

		public event RepeaterCommandEventHandler ItemCommand { add { RepeaterControl.ItemCommand += value; } remove { RepeaterControl.ItemCommand -= value; } }
		public event ListItemEventHandler ItemCreated { add { base.Events.AddHandler(EventItemCreated, value); } remove { base.Events.RemoveHandler(EventItemCreated, value); } }
		public event ListItemEventHandler ItemDataBound { add { base.Events.AddHandler(EventItemDataBound, value); } remove { base.Events.RemoveHandler(EventItemDataBound, value); } }

		public event EventHandler BeforeDataBinding { add { base.Events.AddHandler(EventBeforeDataBinding, value); } remove { base.Events.RemoveHandler(EventBeforeDataBinding, value); } }

		public event EventHandler AfterDataBinding { add { base.Events.AddHandler(EventAfterDataBinding, value); } remove { base.Events.RemoveHandler(EventAfterDataBinding, value); } }

		public event ListItemEventHandler ListItemDataBinding { add { base.Events.AddHandler(EventListItemDataBinding, value); } remove { base.Events.RemoveHandler(EventListItemDataBinding, value); } }

		private void ListItemDataBound(object sender, RepeaterItemEventArgs e) {
			OnItemDataBound(e);
		}

		private void ListItemCreated(object sender, RepeaterItemEventArgs e) {
			OnItemCreated(e);
		}

		protected void DoSorting() {
			if(Comparer != null) {
				InnerListCollection.Sort(Comparer);
			}
		}

		public override void DataBind() {
			EnsureChildControls();
			OnBeforeDataBinding(EventArgs.Empty);
			PopulateList();
			dataBound = true;
			OnAfterDataBinding(EventArgs.Empty);
		}

		protected virtual void PopulateList() {
			DoFiltering();
			DoSorting();
			visibleItems = 0;
			listItemCollection = new ListItemCollection();
			RepeaterControl.DataSource = ListCollection;
			RepeaterControl.DataBind();
		}

		protected void DoFiltering() {
			if(!filtered) {
				// TODO: Implement filtering
				filtered = true;
			}
		}

		protected virtual void PopulateListControls() {
			Controls.Add(plhHeader);
			Controls.Add(rptList);
			Controls.Add(plhFooter);
		}

		protected override void CreateChildControls() {
			Controls.Clear();

			rptList = new Repeater();
			plhHeader = new WebPlaceHolder();
			plhFooter = new WebPlaceHolder();

			if(ItemTemplate != null) {
				rptList.ItemTemplate = ItemTemplate;
			}
			if(AlternatingItemTemplate != null) {
				rptList.ItemTemplate = AlternatingItemTemplate;
			}
			if(SeparatorTemplate != null) {
				rptList.SeparatorTemplate = SeparatorTemplate;
			}

			PopulateListControls();

			if(HeaderTemplate != null) {
				HeaderTemplate.InstantiateIn(plhHeader);
			}
			if(FooterTemplate != null) {
				FooterTemplate.InstantiateIn(plhFooter);
			}

			rptList.ItemCreated += ListItemCreated;
			rptList.ItemDataBound += ListItemDataBound;
		}

		public override ControlCollection Controls {
			get {
				EnsureChildControls();
				return base.Controls;
			}
		}

		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender(e);
			if(!SuppressAutoDataBinding && !dataBound) {
				DataBind();
			}
		}

		protected virtual void OnBeforeDataBinding(EventArgs e) {
			var eventHandler = base.Events[EventBeforeDataBinding] as EventHandler;
			if(eventHandler != null) {
				eventHandler(this, e);
			}
		}

		protected virtual void OnAfterDataBinding(EventArgs e) {
			var eventHandler = base.Events[EventAfterDataBinding] as EventHandler;
			if(eventHandler != null) {
				eventHandler(this, e);
			}
		}

		protected virtual void OnListItemDataBinding(ListItemEventArgs e) {
			var eventHandler = base.Events[EventListItemDataBinding] as ListItemEventHandler;
			if(eventHandler != null) {
				eventHandler(this, e);
			}
		}

		protected virtual void OnItemCreated(RepeaterItemEventArgs e) {
			if(e.Item.DataItem != null &&
			   (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)) {
				var listItem = (ListItemBase)ControlFinder.Instance.FindFirstControlOnType(e.Item, typeof(ListItemBase));
				ListItemEventArgs eventArgs;
				var itemIndex = visibleItems;
				visibleItems++;
				if(listItem != null) {
					listItem.ListItem = e.Item.DataItem;
					listItem.ListingObject = this;
					listItem.RepeaterItem = e.Item;
					if(e.Item.ItemType == ListItemType.AlternatingItem) {
						listItem.AlternatingItem = true;
					}
					listItem.ItemIndex = itemIndex;
					eventArgs = new ListItemEventArgs(listItem);
					OnListItemDataBinding(eventArgs);
					listItemCollection.Add(listItem);
				} else {
					eventArgs = new ListItemEventArgs(e.Item.DataItem, this, e.Item, itemIndex);
				}
				var eventHandler = base.Events[EventItemCreated] as ListItemEventHandler;
				if(eventHandler != null) {
					eventHandler(this, eventArgs);
				}
			}
		}

		protected virtual void OnItemDataBound(RepeaterItemEventArgs e) {
			if(e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem) {
				var listItem = (ListItemBase)ControlFinder.Instance.FindFirstControlOnType(e.Item, typeof(ListItemBase));
				var eventArgs = listItem != null
				                              	? new ListItemEventArgs(listItem)
				                              	: new ListItemEventArgs(e.Item.DataItem, this, e.Item, visibleItems - 1);
				var eventHandler = base.Events[EventItemDataBound] as ListItemEventHandler;
				if(eventHandler != null) {
					eventHandler(this, eventArgs);
				}
			}
		}

		protected override void LoadViewState(object savedState) {
			base.LoadViewState(savedState);
			if(ViewStateList && IsPostBack && ViewState[ListCollectionKey] != null) {
				var viewStatedList = (ArrayList)ViewState[ListCollectionKey];
				listItemCollection = new ListItemCollection();
				if(!SuppressAutoDataBinding) {
					RepeaterControl.DataSource = viewStatedList;
					RepeaterControl.DataBind();
				}
				if(!listSet && viewStatedList != null) {
					listCollection = viewStatedList;
				}
			}
			if(ViewStateRepeater && IsPostBack) {
				dataBound = true;
			}
		}

		protected override object SaveViewState() {
			if(ViewStateList) {
				ViewState[ListCollectionKey] = ListCollection;
			}
			if(ViewStateRepeater) {
				ViewState[ViewstateRepeater] = true;
					// Of some reason I have to touch the viewstate collection here else the repeater wont be veiwstated
			} else {
				RepeaterControl.EnableViewState = false;
			}
			return base.SaveViewState();
		}
	}
}