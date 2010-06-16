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

	[ToolboxItem(true)]
	public class ListItemBase : UserControl {
		#region Constants
		private const string CommandNameKey = "v_CommandName";
		private const string CommandArgKey = "v_CommandArgument";
		private const string EnableViewstateChangedKey = "v_EnableViewStateChanged";
		private const string ViewstateDisabledByDefaultKey = "v_ViewStateDisabledByDefault";
		#endregion

		#region Members
		private object listItem;
		private GeneralListing listingObject;
		private bool alternatingItem;
		private int itemIndex;
		private RepeaterItem repeaterItem;
		#endregion

		#region Properties
		public object ListItem { get { return this.listItem; } set { this.listItem = value; } }
		public GeneralListing ListingObject { get { return this.listingObject; } set { this.listingObject = value; } }
		public bool AlternatingItem { get { return this.alternatingItem; } set { this.alternatingItem = value; } }
		public int ItemIndex { get { return this.itemIndex; } set { this.itemIndex = value; } }
		public RepeaterItem RepeaterItem { get { return this.repeaterItem; } set { this.repeaterItem = value; } }

		public string CommandArgument {
			get {
				string commandArgument = (string)this.ViewState[CommandArgKey];
				if(commandArgument != null) {
					return commandArgument;
				}
				return string.Empty;
			}
			set { this.ViewState[CommandArgKey] = value; }
		}

		public string CommandName {
			get {
				string commandName = (string)this.ViewState[CommandNameKey];
				if(commandName != null) {
					return commandName;
				}
				return string.Empty;
			}
			set { this.ViewState[CommandNameKey] = value; }
		}

		public override bool EnableViewState {
			get { return base.EnableViewState; }
			set {
				this.EnableViewStateChanged = true;
				base.EnableViewState = value;
			}
		}

		private bool EnableViewStateChanged {
			get {
				if(this.ViewState[EnableViewstateChangedKey] == null) {
					return false;
				}
				return (bool)this.ViewState[EnableViewstateChangedKey];
			}
			set { this.ViewState[EnableViewstateChangedKey] = value; }
		}

		protected bool ViewStateDisabledByDefault {
			get {
				if(this.ViewState[ViewstateDisabledByDefaultKey] == null) {
					return false;
				}
				return (bool)this.ViewState[ViewstateDisabledByDefaultKey];
			}
			set { this.ViewState[ViewstateDisabledByDefaultKey] = value; }
		}
		#endregion

		#region Methods
		protected override void OnInit(EventArgs e) {
			if(!this.EnableViewStateChanged && this.ViewStateDisabledByDefault)
				base.EnableViewState = false;
			base.OnInit(e);
		}

		protected override bool OnBubbleEvent(object source, EventArgs args) {
			if(args is CommandEventArgs) {
				base.RaiseBubbleEvent(this, args);
				return true;
			}
			return false;
		}
		#endregion
	}

	#region ListItemCollection
	public sealed class ListItemCollection : ICollection {
		#region Members
		private readonly ArrayList items;
		#endregion

		#region Constructors
		public ListItemCollection() {
			this.items = new ArrayList();
		}
		#endregion

		#region Properties
		public int Count { get { return this.items.Count; } }
		public bool IsReadOnly { get { return this.items.IsReadOnly; } }
		public bool IsSynchronized { get { return this.items.IsSynchronized; } }
		public ListItemBase this[int index] { get { return (ListItemBase)this.items[index]; } }
		public object SyncRoot { get { return this; } }
		#endregion

		#region Methods
		internal void Add(ListItemBase item) {
			this.items.Add(item);
		}

		public void CopyTo(ListItemBase[] listItems, int index) {
			this.items.CopyTo(listItems, index);
		}

		void ICollection.CopyTo(Array array, int index) {
			this.items.CopyTo(array, index);
		}

		public IEnumerator GetEnumerator() {
			return this.items.GetEnumerator();
		}
		#endregion
	}
	#endregion
}