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
using System.Collections;
using System.ComponentModel;

namespace nJupiter.Web.UI.Controls.Listings {

	[ParseChildren(true), PersistChildren(false), ToolboxItem(true)]
	public class GeneralListing : UserControl {
		#region Constants
		private const string ListCollectionKey	= "v_ListCollection";
		private const string ViewstateList			= "v_ViewStateList";
		private const string ViewstateRepeater		= "v_ViewStateRepeater";
		#endregion

		#region UI Members
		private Repeater			rptList;
		private WebPlaceHolder		plhHeader;
		private WebPlaceHolder		plhFooter;
		#endregion

		#region Members
		private bool				filtered;
		private bool				dataBound;
		private bool				suppressAutoDataBinding;
		private bool				listSet;
		private IComparer			comparer;
		private ArrayList			listCollection = new ArrayList();
		private ITemplate			headerTemplate;
		private ITemplate			itemTemplate;
		private ITemplate			alternatingItemTemplate;
		private ITemplate			separatorTemplate;
		private ITemplate			footerTemplate;
		private int					visibleItems;
		private ListItemCollection	listItemCollection = new ListItemCollection();
		#endregion

		#region Properties
		public int						VisibleItems			{ get { return this.visibleItems; } }
		public IComparer				Comparer				{ get { return this.comparer; }					set { this.comparer = value; } }
		public bool						SuppressAutoDataBinding	{ get { return this.suppressAutoDataBinding; }	set { this.suppressAutoDataBinding = value; } }
		public ListItemCollection		ListItems				{ get { return this.listItemCollection; } }
		public WebPlaceHolder			HeaderControl			{ get { this.EnsureChildControls(); return plhHeader; } }
		public WebPlaceHolder			FooterControl			{ get { this.EnsureChildControls(); return plhFooter; } }
		public RepeaterItemCollection	Items					{ get { return this.RepeaterControl.Items; } }

		protected ArrayList	InnerListCollection		{ get {	return this.listCollection; }	set { this.ListCollection = value; } }
		protected bool		ListSet					{ get { return this.listSet; } }

		public bool	ViewStateList {
			get {
				if(this.ViewState[ViewstateList] == null)
					return false;
				return (bool)this.ViewState[ViewstateList];
			}
			set {
				this.ViewState[ViewstateList] = value;
			}
		}

		public bool	ViewStateRepeater {
			get {
				if(this.ViewState[ViewstateRepeater] == null)
					return false;
				return (bool)this.ViewState[ViewstateRepeater];
			}
			set {
				this.ViewState[ViewstateRepeater] = value;
			}
		}

		public ICollection	ListCollection {
			get { return this.InnerListCollection; }
			set {
				this.dataBound			= false;
				this.listSet			= true;
				this.listCollection	= value != null ? new ArrayList(value) : new ArrayList();
			}
		}

		[TemplateContainer(typeof(RepeaterItem)), PersistenceMode(PersistenceMode.InnerProperty)]
		public ITemplate HeaderTemplate { get { return this.headerTemplate; } set { this.headerTemplate = value; } }
		[TemplateContainer(typeof(RepeaterItem)), PersistenceMode(PersistenceMode.InnerProperty)]
		public ITemplate ItemTemplate { get { return this.itemTemplate; } set { this.itemTemplate = value; } }
		[TemplateContainer(typeof(RepeaterItem)), PersistenceMode(PersistenceMode.InnerProperty)]
		public ITemplate AlternatingItemTemplate { get { return this.alternatingItemTemplate; } set { this.alternatingItemTemplate = value; } }
		[TemplateContainer(typeof(RepeaterItem)), PersistenceMode(PersistenceMode.InnerProperty)]
		public ITemplate SeparatorTemplate { get { return this.separatorTemplate; } set { this.separatorTemplate = value; } }
		[TemplateContainer(typeof(RepeaterItem)), PersistenceMode(PersistenceMode.InnerProperty)]
		public ITemplate FooterTemplate { get { return this.footerTemplate; } set { this.footerTemplate = value; } }

		protected Repeater	RepeaterControl	{
			get {
				this.EnsureChildControls();
				return rptList;
			}
		}
		#endregion
		
		#region Static Holders
		private static readonly object eventItemCreated			= new object();
		private static readonly object eventItemDataBound		= new object();
		private static readonly object eventBeforeDataBinding	= new object();
		private static readonly object eventAfterDataBinding	= new object();
		private static readonly object eventListItemDataBinding	= new object();
		#endregion

		#region Events
		public event RepeaterCommandEventHandler	ItemCommand { 
			add { this.RepeaterControl.ItemCommand += value; } 
			remove {this.RepeaterControl.ItemCommand -= value; } 
		}
		public event ListItemEventHandler			ItemCreated {
			add { base.Events.AddHandler(GeneralListing.eventItemCreated, value); }
			remove { base.Events.RemoveHandler(GeneralListing.eventItemCreated, value); }
		}
		public event ListItemEventHandler			ItemDataBound {
			add { base.Events.AddHandler(GeneralListing.eventItemDataBound, value); }
			remove { base.Events.RemoveHandler(GeneralListing.eventItemDataBound, value); }
		}
		
		public event EventHandler					BeforeDataBinding {
			add { base.Events.AddHandler(GeneralListing.eventBeforeDataBinding, value); }
			remove { base.Events.RemoveHandler(GeneralListing.eventBeforeDataBinding, value); }
		}
		
		public event EventHandler					AfterDataBinding {
			add { base.Events.AddHandler(GeneralListing.eventAfterDataBinding, value); }
			remove { base.Events.RemoveHandler(GeneralListing.eventAfterDataBinding, value); }
		}
		
		public event ListItemEventHandler			ListItemDataBinding {
			add { base.Events.AddHandler(GeneralListing.eventListItemDataBinding, value); }
			remove { base.Events.RemoveHandler(GeneralListing.eventListItemDataBinding, value); }
		}
		#endregion

		#region Event Handlers
		private void ListItemDataBound(object sender, RepeaterItemEventArgs e) {
			OnItemDataBound(e);
		}

		private void ListItemCreated(object sender, RepeaterItemEventArgs e) {
			OnItemCreated(e);
		}
		#endregion

		#region Methods
		protected void DoSorting(){
			if(this.Comparer != null) {
				this.InnerListCollection.Sort(this.Comparer);
			}
		}
		
		public override void DataBind() {
			this.EnsureChildControls();
			OnBeforeDataBinding(EventArgs.Empty);
			PopulateList();
			this.dataBound = true;
			OnAfterDataBinding(EventArgs.Empty);
		}

		protected virtual void PopulateList() {
			this.DoFiltering();
			this.DoSorting();
			this.visibleItems = 0;
			this.listItemCollection = new ListItemCollection();
			this.RepeaterControl.DataSource = this.ListCollection;
			this.RepeaterControl.DataBind();
		}

		protected void DoFiltering(){
			if(!this.filtered){
				// TODO: Implement filtering
				this.filtered = true;
			}
		}
		#endregion

		#region Event Activators
		protected virtual void PopulateListControls() {
			this.Controls.Add(plhHeader);
			this.Controls.Add(rptList);
			this.Controls.Add(plhFooter);
		}
		
		protected override void CreateChildControls() {
			this.Controls.Clear();

			rptList		= new Repeater();
			plhHeader	= new WebPlaceHolder();
			plhFooter	= new WebPlaceHolder();

			if(this.ItemTemplate != null)
				rptList.ItemTemplate = this.ItemTemplate;
			if(this.AlternatingItemTemplate != null)
				rptList.ItemTemplate = this.AlternatingItemTemplate;
			if(this.SeparatorTemplate != null)
				rptList.SeparatorTemplate = this.SeparatorTemplate;
			
			this.PopulateListControls();

			if(this.HeaderTemplate != null)
				this.HeaderTemplate.InstantiateIn(plhHeader);
			if(this.FooterTemplate != null)
				this.FooterTemplate.InstantiateIn(plhFooter);

			this.rptList.ItemCreated	+= this.ListItemCreated;
			this.rptList.ItemDataBound	+= this.ListItemDataBound;
		}

		public override ControlCollection Controls {
			get {
				this.EnsureChildControls();
				return base.Controls;
			}
		}
		
		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender(e);
			if(!this.SuppressAutoDataBinding && !this.dataBound) {
				this.DataBind();
			}
		}
		
		protected virtual void OnBeforeDataBinding(EventArgs e) {
			EventHandler eventHandler = base.Events[GeneralListing.eventBeforeDataBinding] as EventHandler;
			if(eventHandler != null) {
				eventHandler(this, e);
			}
		}
		
		protected virtual void OnAfterDataBinding(EventArgs e) {
			EventHandler eventHandler = base.Events[GeneralListing.eventAfterDataBinding] as EventHandler;
			if(eventHandler != null) {
				eventHandler(this, e);
			}
		}
		
		protected virtual void OnListItemDataBinding(ListItemEventArgs e) {
			ListItemEventHandler eventHandler = base.Events[GeneralListing.eventListItemDataBinding] as ListItemEventHandler;
			if(eventHandler != null) {
				eventHandler(this, e);
			}
		}
		

		protected virtual void OnItemCreated(RepeaterItemEventArgs e) {
			if(e.Item.DataItem != null && (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)) {
				ListItemBase listItem = (ListItemBase)ControlHandler.FindFirstControlOnType(e.Item, typeof(ListItemBase));
				ListItemEventArgs eventArgs;
				int itemIndex = this.visibleItems;
				this.visibleItems++;
				if(listItem != null) {
					listItem.ListItem				= e.Item.DataItem;
					listItem.ListingObject			= this;
					listItem.RepeaterItem			= e.Item;
					if(e.Item.ItemType == ListItemType.AlternatingItem) {
						listItem.AlternatingItem = true;
					}
					listItem.ItemIndex				= itemIndex;
					eventArgs = new ListItemEventArgs(listItem);
					OnListItemDataBinding(eventArgs);
					this.listItemCollection.Add(listItem);
				} else {
					eventArgs = new ListItemEventArgs(e.Item.DataItem, this, e.Item, itemIndex);
				}
				ListItemEventHandler eventHandler = base.Events[GeneralListing.eventItemCreated] as ListItemEventHandler;
				if(eventHandler != null) {
					eventHandler(this, eventArgs);
				}
			}
		}
		
		protected virtual void OnItemDataBound(RepeaterItemEventArgs e) {
			if(e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem) {

				ListItemBase listItem = (ListItemBase)ControlHandler.FindFirstControlOnType(e.Item, typeof(ListItemBase));
				ListItemEventArgs eventArgs = listItem != null ? new ListItemEventArgs(listItem) : new ListItemEventArgs(e.Item.DataItem, this, e.Item, this.visibleItems - 1);
				ListItemEventHandler eventHandler = base.Events[GeneralListing.eventItemDataBound] as ListItemEventHandler;
				if(eventHandler != null) {
					eventHandler(this, eventArgs);
				}
			}
		}
		#endregion

		#region Overridden Methods
		protected override void LoadViewState(object savedState) {
			base.LoadViewState(savedState);
			if(this.ViewStateList && IsPostBack && ViewState[ListCollectionKey] != null) {
				ArrayList viewStatedList = (ArrayList)ViewState[ListCollectionKey];
				this.listItemCollection = new ListItemCollection();
				if(!this.SuppressAutoDataBinding) {
					this.RepeaterControl.DataSource = viewStatedList;
					this.RepeaterControl.DataBind();
				}
				if(!this.listSet && viewStatedList != null) {
					this.listCollection = viewStatedList;
				}
			}
			if(this.ViewStateRepeater && this.IsPostBack)
				this.dataBound = true;
		}

		protected override object SaveViewState() {
			if(this.ViewStateList)
				ViewState[ListCollectionKey] = this.ListCollection;
			if(this.ViewStateRepeater)
				ViewState[ViewstateRepeater] = true; // Of some reason I have to touch the viewstate collection here else the repeater wont be veiwstated
			else
				this.RepeaterControl.EnableViewState = false;
			return base.SaveViewState();
		}
		#endregion
	}
}
