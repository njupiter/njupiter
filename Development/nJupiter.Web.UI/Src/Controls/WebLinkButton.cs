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
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

namespace nJupiter.Web.UI.Controls {

	public class WebLinkButton : LinkButton {
		#region Constants
		private const string BrTag = "<" + HtmlTag.Br + " />";
		private const string InnerSpanKey = "v_InnerSpan";
		private const string NoLinkKey = "v_NoLink";
		private const string DisableEncodingKey = "v_DisableEncoding";
		private const string HasControlsKey = "v_HasControls";
		private const string SuppressPostback = "v_SuppressJavascriptPostBack";
		private const string RenderIdKey = "v_RenderId";
		private const string RenderOriginalIdKey = "v_RenderOriginalId";
		private const string NavigateUrlKey = "v_NavigateUrl";
		private const string TrailingLinefeedKey = "v_TrailingLinefeed";
		private const string TrailingBreakKey = "v_TrailingBreak";
		#endregion

		#region Properties
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
		public bool NoLink {
			get {
				if(this.ViewState[NoLinkKey] == null)
					return false;
				return (bool)this.ViewState[NoLinkKey];
			}
			set { this.ViewState[NoLinkKey] = value; }
		}

		public bool SuppressJavascriptPostBack {
			get {
				if(this.ViewState[SuppressPostback] == null)
					return false;
				return (bool)this.ViewState[SuppressPostback];
			}
			set { this.ViewState[SuppressPostback] = value; }
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

		public string NavigateUrl {
			get {
				string navigateUrl = (string)this.ViewState[NavigateUrlKey];
				if(navigateUrl != null)
					return navigateUrl;
				return string.Empty;
			}
			set {
				this.ViewState[NavigateUrlKey] = value;
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
		protected override void RenderContents(HtmlTextWriter writer) {
			if(this.HasControlsInternal) {
				base.RenderContents(writer);
			} else {
				writer.Write(this.DisableEncoding ? this.Text : HttpUtility.HtmlEncode(this.Text));
			}
		}
		protected override void Render(HtmlTextWriter writer) {
			if(this.NoLink) {
				if(this.InnerSpan) {
					if(!string.IsNullOrEmpty(this.CssClass)) {
						writer.WriteBeginTag(HtmlTag.Span);
						writer.WriteAttribute(HtmlAttribute.Class, this.CssClass);
						writer.Write(HtmlTextWriter.TagRightChar);
					} else {
						writer.WriteFullBeginTag(HtmlTag.Span);
					}
				}
				this.RenderContents(writer);
				if(this.InnerSpan)
					writer.WriteEndTag(HtmlTag.Span);
			} else {
				if(!this.RenderOriginalId && this.NavigateUrl.Length > 0 && this.SuppressJavascriptPostBack) {
					string originalId = this.ID;
					this.ID = null;
					if(this.RenderId)
						this.Attributes.Add(HtmlAttribute.Id, originalId);
				}
				base.Render(new CustomHtmlTextWriter(writer.InnerWriter, HtmlTextWriter.DefaultTabString, this.NavigateUrl, this.SuppressJavascriptPostBack));
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
		#endregion

		#region Inner Classes
		private sealed class CustomHtmlTextWriter : HtmlTextWriter {

			private readonly string href;
			private readonly bool suppressJavascriptPostBack;

			public CustomHtmlTextWriter(TextWriter writer, string tabString, string altHref, bool suppressJavaScriptPostBack)
				: base(writer, tabString) {
				this.href = altHref;
				this.suppressJavascriptPostBack = suppressJavaScriptPostBack;
			}

			public override void AddAttribute(HtmlTextWriterAttribute key, string value) {
				if(value == null) {
					throw new ArgumentNullException("value");
				}
				if(key.Equals(HtmlTextWriterAttribute.Href)) {
					if(!string.IsNullOrEmpty(this.href)) {
						base.AddAttribute(HtmlTextWriterAttribute.Href, this.href);
						if(value.StartsWith("javascript:") && !this.suppressJavascriptPostBack) {
							base.AddAttribute(HtmlTextWriterAttribute.Onclick, value.Substring("javascript:".Length) + "; return false;");
						}
					} else if(!value.Equals("javascript:void(0)")) {
						if(value.StartsWith("javascript:")) {
							base.AddAttribute(HtmlTextWriterAttribute.Href, "#");
							if(!this.suppressJavascriptPostBack) {
								base.AddAttribute(HtmlTextWriterAttribute.Onclick, value.Substring("javascript:".Length) + "; return false;");
							}
						} else {
							base.AddAttribute(HtmlTextWriterAttribute.Href, value);
						}
					} else {
						base.AddAttribute(HtmlTextWriterAttribute.Href, "#");
					}
				} else {
					base.AddAttribute(key, value);
				}
			}
		}
		#endregion

	}

}
