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
using System.Web.UI.WebControls;

namespace nJupiter.Web.UI.Controls.Listings {

	[Obsolete("Try using System.Web.UI.WebControls.ListView instead if possible")]
	public delegate void ListItemEventHandler(object sender, ListItemEventArgs e);

	[Obsolete("Try using System.Web.UI.WebControls.ListView instead if possible")]
	public sealed class ListItemEventArgs : EventArgs {

		private readonly ListItemBase listItem;
		private readonly object listItemObject;
		private readonly GeneralListing listingObject;
		private readonly RepeaterItem repeaterItem;
		private readonly int itemIndex;

		public ListItemEventArgs(ListItemBase listItem) {
			if(listItem == null) {
				throw new ArgumentNullException("listItem");
			}
			this.listItem = listItem;
			this.listItemObject = listItem.ListItem;
			this.listingObject = listItem.ListingObject;
			this.repeaterItem = listItem.RepeaterItem;
			this.itemIndex = listItem.ItemIndex;
		}

		public ListItemEventArgs(object listItemObject, GeneralListing listingObject, RepeaterItem repeaterItem, int itemIndex) {
			this.listItemObject = listItemObject;
			this.listingObject = listingObject;
			this.repeaterItem = repeaterItem;
			this.itemIndex = itemIndex;
		}

		public ListItemBase ListItem { get { return this.listItem; } }
		public object ListItemObject { get { return this.listItemObject; } }
		public GeneralListing Listing { get { return this.listingObject; } }
		public RepeaterItem Item { get { return this.repeaterItem; } }
		public bool AlternatingItem { get { return this.repeaterItem.ItemType == ListItemType.AlternatingItem; } }
		public int ItemIndex { get { return this.itemIndex; } }

	}
}