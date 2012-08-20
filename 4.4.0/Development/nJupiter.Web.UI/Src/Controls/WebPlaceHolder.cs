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

using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

namespace nJupiter.Web.UI.Controls {
	[ControlBuilder(typeof(PlaceHolderControlBuilder))]
	public class WebPlaceHolder : Control {

		private const string BrTag = "<" + HtmlTag.Br + " />";
		private const string TrailingLinefeedKey = "v_TrailingLinefeed";
		private const string TrailingBreakKey = "v_TrailingBreak";

		public string SurroundingTag {
			get {
				if(this.ViewState["v_SurroundingTag"] == null)
					return string.Empty;
				return (string)this.ViewState["v_SurroundingTag"];
			}
			set {
				this.ViewState["v_SurroundingTag"] = value;
			}
		}
		public virtual string InnerHtml {
			get {
				if(base.IsLiteralContent()) {
					return ((LiteralControl)this.Controls[0]).Text;
				}
				if((this.HasControls() && (this.Controls.Count == 1)) && (this.Controls[0] is DataBoundLiteralControl)) {
					return ((DataBoundLiteralControl)this.Controls[0]).Text;
				}
				if(this.Controls.Count != 0) {
					throw new HttpException("Inner Content not literal.");
				}
				return string.Empty;
			}
			set {
				this.Controls.Clear();
				this.Controls.Add(new LiteralControl(value));
				this.ViewState["innerhtml"] = value;
			}
		}
		public virtual string InnerText {
			get {
				return HttpUtility.HtmlDecode(this.InnerHtml);
			}
			set {
				this.InnerHtml = HttpUtility.HtmlEncode(value);
			}
		}

		protected override void LoadViewState(object savedState) {
			if(savedState != null) {
				base.LoadViewState(savedState);
				var text = (string)this.ViewState["innerhtml"];
				if(text != null) {
					this.InnerHtml = text;
				}
			}
		}

		[DefaultValue(false)]
		public bool TrailingBreak {
			get {
				if(this.ViewState[TrailingBreakKey] == null) {
					return false;
				}
				return (bool)this.ViewState[TrailingBreakKey];
			}
			set {
				this.ViewState[TrailingBreakKey] = value;
			}
		}

		[DefaultValue(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool TrailingLinefeed {
			get {
				if(this.ViewState[TrailingLinefeedKey] == null) {
					return false;
				}
				return (bool)this.ViewState[TrailingLinefeedKey];
			}
			set {
				this.ViewState[TrailingLinefeedKey] = value;
			}
		}

		protected override void Render(HtmlTextWriter writer) {
			if(!string.IsNullOrEmpty(this.SurroundingTag))
				writer.WriteFullBeginTag(this.SurroundingTag);
			base.Render(writer);
			if(!string.IsNullOrEmpty(this.SurroundingTag))
				writer.WriteEndTag(this.SurroundingTag);
			if(this.TrailingBreak)
				writer.Write(BrTag);
			if(this.TrailingLinefeed)
				writer.WriteLine();
		}

		new public void EnsureChildControls() {
			base.EnsureChildControls();
		}

	}
}
