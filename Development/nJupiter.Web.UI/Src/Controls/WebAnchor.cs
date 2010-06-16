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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

namespace nJupiter.Web.UI.Controls {

	public class WebAnchor : HyperLink {
		#region Constants
		private const string HashChar = "#";
		private const string BrTag = "<" + HtmlTag.Br + " />";
		private const string InnerSpanKey = "v_InnerSpan";
		private const string RenderIdKey = "v_RenderId";
		private const string RenderOriginalIdKey = "v_RenderOriginalId";
		private const string NoLinkKey = "v_NoLink";
		private const string DisableEncodingKey = "v_DisableEncoding";
		private const string HasControlsKey = "v_HasControls";
		private const string TrailingLinefeedKey = "v_TrailingLinefeed";
		private const string TrailingBreakKey = "v_TrailingBreak";
		#endregion

		#region Properties
		public bool RenderId {
			get {
				if(this.ViewState[RenderIdKey] == null)
					return false;
				return (bool)this.ViewState[RenderIdKey];
			}
			set { this.ViewState[RenderIdKey] = value; }
		}

		public bool RenderOriginalId {
			get {
				if(this.ViewState[RenderOriginalIdKey] == null)
					return false;
				return (bool)this.ViewState[RenderOriginalIdKey];
			}
			set { this.ViewState[RenderOriginalIdKey] = value; }
		}

		public bool NoLink {
			get {
				if(this.ViewState[NoLinkKey] == null)
					return false;
				return (bool)this.ViewState[NoLinkKey];
			}
			set { this.ViewState[NoLinkKey] = value; }
		}

		public bool DisableEncoding {
			get {
				if(this.ViewState[DisableEncodingKey] == null)
					return false;
				return (bool)this.ViewState[DisableEncodingKey];
			}
			set {
				this.ViewState[DisableEncodingKey] = value;
			}
		}

		private bool HasControlsInternal {
			get {
				if(this.ViewState[HasControlsKey] == null)
					return false;
				return (bool)this.ViewState[HasControlsKey];
			}
			set {
				this.ViewState[HasControlsKey] = value;
			}
		}

		public bool InnerSpan {
			get {
				if(this.ViewState[InnerSpanKey] == null) {
					return false;
				}
				return (bool)this.ViewState[InnerSpanKey];
			}
			set {
				this.ViewState[InnerSpanKey] = value;
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
		#endregion

		#region Method
		protected override void AddParsedSubObject(object obj) {
			if(!(obj is LiteralControl) && !(obj is Literal))
				this.DisableEncoding = true;
			this.HasControlsInternal = true;
			base.AddParsedSubObject(obj);
		}

		protected override void Render(HtmlTextWriter writer) {
			if(this.NoLink) {
				if(this.InnerSpan) {
					if(!string.IsNullOrEmpty(this.CssClass)) {
						writer.WriteBeginTag(HtmlTag.Span);
						writer.WriteAttribute(HtmlAttribute.Class, this.CssClass);
						this.Attributes.Render(writer);
						if(!this.RenderOriginalId) {
							string originalId = this.ID;
							this.ID = null;
							if(this.RenderId)
								writer.WriteAttribute(HtmlAttribute.Id, originalId);
						}
						writer.Write(HtmlTextWriter.TagRightChar);
					} else {
						writer.WriteFullBeginTag(HtmlTag.Span);
					}
				}
				this.RenderContents(writer);
				if(this.InnerSpan)
					writer.WriteEndTag(HtmlTag.Span);

			} else {
				if(!this.RenderOriginalId) {
					string originalId = this.ID;
					this.ID = null;
					if(this.RenderId)
						this.Attributes.Add(HtmlAttribute.Id, originalId);
				}
				base.Render(writer);
			}
			if(this.TrailingBreak)
				writer.Write(BrTag);
			if(this.TrailingLinefeed)
				writer.WriteLine();
		}

		public override void RenderBeginTag(HtmlTextWriter writer) {
			if(writer == null) {
				throw new ArgumentNullException("writer");
			}
			base.RenderBeginTag(writer);
			if(this.InnerSpan)
				writer.WriteFullBeginTag(HtmlTag.Span);
		}

		public override void RenderEndTag(HtmlTextWriter writer) {
			if(writer == null) {
				throw new ArgumentNullException("writer");
			}
			if(this.InnerSpan)
				writer.WriteEndTag(HtmlTag.Span);
			base.RenderEndTag(writer);
		}

		protected override void RenderContents(HtmlTextWriter writer) {
			string text = this.ImageUrl;
			if(text.Length > 0) {
				WebImage image = new WebImage();
				image.ImageUrl = text;
				text = this.ToolTip;
				if(text.Length != 0) {
					image.ToolTip = text;
				}
				text = this.Text;
				if(text.Length != 0) {
					image.AlternateText = text;
				}
				image.RenderControl(writer);
			} else if(this.HasControlsInternal) {
				base.RenderContents(writer);
			} else {
				writer.Write(this.DisableEncoding ? this.Text : HttpUtility.HtmlEncode(this.Text));
			}
		}

		protected override void AddAttributesToRender(HtmlTextWriter writer) {
			if(this.NavigateUrl.StartsWith(HashChar)) {
				string origUrl = this.NavigateUrl;
				this.NavigateUrl = string.Empty;
				base.AddAttributesToRender(writer);
				if(origUrl.Length > 0 && this.Enabled) {
					writer.AddAttribute(HtmlTextWriterAttribute.Href, origUrl);
				}
			} else {
				base.AddAttributesToRender(writer);
			}
		}
		#endregion
	}
}
