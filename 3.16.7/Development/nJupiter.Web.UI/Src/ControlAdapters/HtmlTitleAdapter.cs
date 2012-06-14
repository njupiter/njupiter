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

using System.Web.UI.HtmlControls;

namespace nJupiter.Web.UI.ControlAdapters {

	public class HtmlTitleAdapter : HtmlControlAdapter {

		#region Properties
		private HtmlTitle TitleControl {
			get {
				return this.HtmlControl as HtmlTitle;
			}
		}

		protected override bool RenderElement {
			get {
				return !(
					this.TitleControl != null &&
					this.IsLiteralContent &&
					string.IsNullOrEmpty(this.TitleControl.Text) &&
					!this.RenderOriginalId &&
					!this.RenderId &&
					string.IsNullOrEmpty(this.CssClass) &&
					!this.HasAttributes &&
					HtmlTag.IsBlockElement(this.TitleControl.TagName)
					);
			}
		}

		protected override void Render(System.Web.UI.HtmlTextWriter writer) {
			if(this.TitleControl != null && this.AdapterEnabled) {
				if(this.RenderElement) {
					if(!this.RenderContentOnly) {
						this.RenderBeginTag(writer);
					}
					if(HtmlTag.RequiresEndTag(this.TitleControl.TagName) || this.Control.Controls.Count > 0) {
						writer.Write(this.TitleControl.Text);
						if(!this.RenderContentOnly) {
							this.RenderEndTag(writer);
						}
					}
					if(this.TrailingBreak)
						writer.Write(BrTag);
					if(this.TrailingLinefeed)
						writer.WriteLine();
				}
			} else {
				base.Render(writer);
			}
		}
		#endregion
	}

}
