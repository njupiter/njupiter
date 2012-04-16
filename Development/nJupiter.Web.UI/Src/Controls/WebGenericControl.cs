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
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.ComponentModel;

namespace nJupiter.Web.UI.Controls {


	[ToolboxItem(true), ConstructorNeedsTag(false)]
	[Obsolete("Try using System.Web.UI.HtmlControls.HtmlGenericControl together with nJupiter.Web.UI.ControlAdapters.HtmlGenericAdapter (or ASP.NET 4)")]
	public class WebGenericControl : HtmlGenericControl {

		private const string BrTag = "<" + HtmlTag.Br + " />";
		private const string RenderIdKey = "v_RenderId";
		private const string RenderOriginalIdKey = "v_RenderOriginalId";
		private const string RenderContentOnlyKey = "v_RenderContentOnly";
		private const string TrailingLinefeedKey = "v_TrailingLinefeed";
		private const string TrailingBreakKey = "v_TrailingBreak";

		public bool RenderId {
			get {
				if(this.ViewState[RenderIdKey] == null)
					return false;
				return (bool)this.ViewState[RenderIdKey];
			}
			set {
				this.ViewState[RenderIdKey] = value;
			}
		}

		public bool RenderOriginalId {
			get {
				if(this.ViewState[RenderOriginalIdKey] == null)
					return false;
				return (bool)this.ViewState[RenderOriginalIdKey];
			}
			set {
				this.ViewState[RenderOriginalIdKey] = value;
			}
		}

		public bool RenderContentOnly {
			get {
				if(this.ViewState[RenderContentOnlyKey] == null)
					return false;
				return (bool)this.ViewState[RenderContentOnlyKey];
			}
			set {
				this.ViewState[RenderContentOnlyKey] = value;
			}
		}

		public string CssClass {
			get {
				var result = this.ViewState[HtmlAttribute.Class] as string;
				if(result != null)
					return result;
				return string.Empty;
			}
			set {
				this.ViewState[HtmlAttribute.Class] = value;
			}
		}

		public override string InnerHtml {
			get {
				return base.InnerHtml;
			}
			set {
				if(value == null)
					base.InnerHtml = string.Empty;
				base.InnerHtml = value;
			}
		}

		[DefaultValue("div"), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		new public string TagName {
			get {
				return base.TagName;
			}
			set {
				base.TagName = value;
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

		protected bool HasAttributes {
			get {
				var enumerator = this.Attributes.Keys.GetEnumerator();

				while(enumerator.MoveNext()) {
					var attributeName = enumerator.Current as string;
					if(attributeName != null) {
						var isAttribute = IsAttribute(attributeName);
						if(isAttribute) {
							return true;
						}
					}
				}
				return false;
			}
		}

		protected virtual bool IsAttribute(string name) {
			if(
				name == RenderIdKey ||
				name == RenderOriginalIdKey ||
				name == RenderContentOnlyKey ||
				name == TrailingLinefeedKey ||
				name == TrailingBreakKey ||
				name == "innerhtml") {
				return false;
			}
			return true;
		}

		protected virtual bool RenderElement {
			get {
				return !(
					this.IsLiteralContent() &&
					string.IsNullOrEmpty(this.InnerHtml) &&
					!this.RenderOriginalId &&
					!this.RenderId &&
					this.CssClass.Length == 0 &&
					!this.HasAttributes &&
					HtmlTag.IsBlockElement(this.TagName)
					);
			}
		}

		public WebGenericControl() {
			this.TagName = HtmlTag.Div;
		}

		public WebGenericControl(string tag)
			: this() {
			if(tag == null)
				throw new ArgumentNullException("tag");

			if(tag.IndexOf(':').Equals(-1)) {
				this.TagName = tag;
			}
		}

		protected override void Render(HtmlTextWriter writer) {
			if(this.RenderElement) {
				if(!this.RenderContentOnly) {
					this.RenderBeginTag(writer);
				}
				if(HtmlTag.RequiresEndTag(this.TagName) || this.Controls.Count > 0) {
					this.RenderChildren(writer);
					if(!this.RenderContentOnly) {
						this.RenderEndTag(writer);
					}
				}
				if(this.TrailingBreak)
					writer.Write(BrTag);
				if(this.TrailingLinefeed)
					writer.WriteLine();
			}
		}

		protected override void RenderBeginTag(HtmlTextWriter writer) {
			writer.WriteBeginTag(this.TagName);
			this.RenderAttributes(writer);
			if(HtmlTag.RequiresEndTag(this.TagName) || this.Controls.Count > 0) {
				writer.Write('>');
			} else {
				writer.Write(" />");
			}
		}

		protected override void RenderAttributes(HtmlTextWriter writer) {
			if(this.CssClass.Length > 0)
				this.Attributes.Add(HtmlAttribute.Class, this.CssClass);
			if(!this.RenderOriginalId) {
				var originalId = this.ID;
				this.ID = null;
				if(this.RenderId)
					this.Attributes.Add(HtmlAttribute.Id, originalId);
			}
			base.RenderAttributes(writer);
		}

	}
}
