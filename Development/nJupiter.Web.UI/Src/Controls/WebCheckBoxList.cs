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

using System.Collections;
using System.Web.UI.WebControls;

namespace nJupiter.Web.UI.Controls {
	public class WebCheckBoxList : WebCheckListControl {
		public WebCheckBoxList() : base(new WebCheckBox()) { }

		public int[] SelectedIndexes {
			get {
				ArrayList al = new ArrayList();
				for(int i = 0; i < this.Items.Count; i++) {
					ListItem li = this.Items[i];
					if(li != null && li.Selected)
						al.Add(i);
				}
				return (int[])al.ToArray(typeof(int));
			}
		}

		public string[] SelectedValues {
			get {
				ArrayList al = new ArrayList();
				for(int i = 0; i < this.Items.Count; i++) {
					ListItem li = this.Items[i];
					if(li != null && li.Selected)
						al.Add(li.Value);
				}
				return (string[])al.ToArray(typeof(string));

			}
		}

		public ListItemCollection SelectedItems {
			get {
				ListItemCollection lic = new ListItemCollection();
				for(int i = 0; i < this.Items.Count; i++) {
					ListItem li = this.Items[i];
					if(li != null && li.Selected)
						lic.Add(li);
				}
				return lic;
			}
		}

	}
}
