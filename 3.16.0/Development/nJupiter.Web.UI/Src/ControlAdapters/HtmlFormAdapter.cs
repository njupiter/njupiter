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
using System.Web.UI.HtmlControls;

using nJupiter.Web.UI.Controls;

namespace nJupiter.Web.UI.ControlAdapters {

	public class HtmlFormAdapter : ControlAdapterBase {

		private HtmlForm formControl;

		#region Properties
		private HtmlForm FormControl {
			get {
				if(this.formControl == null) {
					this.formControl = base.Control as HtmlForm;
					if(this.formControl != null) {
						if(this.formControl is WebForm) {
							this.formControl = null;
						}
					}
				}
				return this.formControl;
			}
		}

		private bool XhtmlStrictRendering { get { return string.Compare(this.FormControl.Attributes["XhtmlStrictRendering"], "false", StringComparison.InvariantCultureIgnoreCase) != 0; } }
		private string CssClass { get { return this.FormControl.Attributes["CssClass"]; } }
		#endregion

		#region Methods
		protected virtual void RenderAttributes(HtmlTextWriter writer) {
			if(this.CssClass.Length > 0)
				this.FormControl.Attributes.Add(HtmlAttribute.Class, this.CssClass);
			this.FormControl.Attributes.Remove("XhtmlStrictRendering");
			this.FormControl.Attributes.Remove("CssClass");
			this.FormControl.Attributes.Render(writer);
		}

		protected override void Render(HtmlTextWriter writer) {
			if(this.FormControl != null && this.XhtmlStrictRendering && writer.GetType().Equals(typeof(HtmlTextWriter)) && this.AdapterEnabled) {
				StrictHtmlTextWriter w = new StrictHtmlTextWriter(writer);
				base.Render(w);
			} else {
				base.Render(writer);
			}
		}
		#endregion

	}
}
