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
using System.Collections;
using System.Web.UI;
using System.Web.UI.HtmlControls;

using nJupiter.Web.UI.Controls;

namespace nJupiter.Web.UI.ControlAdapters {
	public class HtmlControlAdapter : ControlAdapterBase {
		private HtmlControl htmlControl;

		#region Constants
		private const string RenderIdAttributeName			= "RenderId";
		private const string RenderOriginalIdAttributeName	= "RenderOriginalId";
		private const string RenderContentOnlyAttributeName	= "RenderContentOnly";
		private const string TrailingLinefeedAttributeName	= "TrailingLinefeed";
		private const string TrailingBreakAttributeName		= "TrailingBreak";
		private const string CssClassAttributeName			= "CssClass";
		private const string InnerSpanAttributeName			= "InnerSpan";
		
		private const string InnerHtmlAttributeName			= "innerhtml";
		private const string True							= "true";

		protected const string BrTag	= "<" + HtmlTag.Br + " />";
		#endregion

		#region Properties
		protected virtual HtmlControl HtmlControl {
			get {
				if(this.htmlControl == null) {
					this.htmlControl = base.Control as HtmlControl;
					if(	this.htmlControl is WebGenericControl ||
						this.htmlControl is WebButton) {
						this.htmlControl = null;
					}
				}
				return this.htmlControl;
			} 
		}

		protected virtual bool RenderElement {
			get {
				return !(
					this.HtmlControl != null &&
					this.IsLiteralContent &&
					!this.RenderOriginalId &&
					!this.RenderId &&
					string.IsNullOrEmpty(this.CssClass) &&
					!this.HasAttributes &&
					HtmlTag.IsBlockElement(this.HtmlControl.TagName)
					);
			}
		}

		protected virtual bool IsLiteralContent {
			get {
				if(this.HtmlControl.Controls != null && this.HtmlControl.Controls.Count == 1) {
					return (this.HtmlControl.Controls[0] is LiteralControl);
				}
				return false;
			}
		}

		protected bool HasAttributes {
			get {
			 	IEnumerator enumerator = this.htmlControl.Attributes.Keys.GetEnumerator();

				while(enumerator.MoveNext()) {
					string attributeName = enumerator.Current as string;
					if(attributeName != null) {
						bool isAttribute = IsAttribute(attributeName);
						if(isAttribute) {
							return true;
						}
					}
				}
				return false;
			}
		}

		protected virtual bool IsAttribute(string name) {
			if(	name == RenderIdAttributeName ||
				name == RenderOriginalIdAttributeName ||
				name == RenderContentOnlyAttributeName ||
				name == TrailingLinefeedAttributeName ||
				name == TrailingBreakAttributeName ||
				name == CssClassAttributeName ||
				name == InnerSpanAttributeName ||
				name == InnerHtmlAttributeName) {
				return false;
			}
			return true;
		}

		protected bool RenderId				{ get { return string.Compare(this.HtmlControl.Attributes[RenderIdAttributeName], True, StringComparison.InvariantCultureIgnoreCase) == 0; } }
		protected bool RenderOriginalId		{ get { return string.Compare(this.HtmlControl.Attributes[RenderOriginalIdAttributeName], True, StringComparison.InvariantCultureIgnoreCase) == 0; } }
		protected bool RenderContentOnly	{ get { return string.Compare(this.HtmlControl.Attributes[RenderContentOnlyAttributeName], True, StringComparison.InvariantCultureIgnoreCase) == 0; } }
		protected bool TrailingLinefeed		{ get { return string.Compare(this.HtmlControl.Attributes[TrailingLinefeedAttributeName], True, StringComparison.InvariantCultureIgnoreCase) == 0; } }
		protected bool TrailingBreak		{ get { return string.Compare(this.HtmlControl.Attributes[TrailingBreakAttributeName], True, StringComparison.InvariantCultureIgnoreCase) == 0; } }
		protected bool InnerSpan			{ get { return string.Compare(this.HtmlControl.Attributes[InnerSpanAttributeName], True, StringComparison.InvariantCultureIgnoreCase) == 0; } }
		protected string CssClass			{ get { return this.HtmlControl.Attributes[CssClassAttributeName]; } }
		#endregion

		protected override void Render(HtmlTextWriter writer) {
			if(this.HtmlControl != null && this.AdapterEnabled) {
				if(this.RenderElement) {
					if(!this.RenderContentOnly) {
						this.RenderBeginTag(writer);
					}
					if(HtmlTag.RequiresEndTag(this.HtmlControl.TagName) || this.Control.Controls.Count > 0) {
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
			} else {
				base.Render(writer);
			}
		}

		protected virtual void RenderBeginTag(HtmlTextWriter writer) {
			writer.WriteBeginTag(this.HtmlControl.TagName);
			this.RenderAttributes(writer);
			if(HtmlTag.RequiresEndTag(this.HtmlControl.TagName) || this.Control.Controls.Count > 0) {
				writer.Write('>');
				if(this.InnerSpan){
					writer.WriteFullBeginTag(HtmlTag.Span);
				}
			} else {
				writer.Write(" />");
			}
		}

		protected virtual void RenderEndTag(HtmlTextWriter writer) {
			if(this.InnerSpan) {
				writer.WriteEndTag(HtmlTag.Span);
			}
			writer.WriteEndTag(this.HtmlControl.TagName);
		}

		protected virtual void RenderAttributes(HtmlTextWriter writer) {
			if(!string.IsNullOrEmpty(this.CssClass))
				this.HtmlControl.Attributes.Add(HtmlAttribute.Class, this.CssClass);
			if(!this.RenderOriginalId) {
				string originalId = this.Control.ID;
				this.Control.ID = null;
				if(this.RenderId)
					this.HtmlControl.Attributes.Add(HtmlAttribute.Id, originalId);
			} else {
				this.HtmlControl.Attributes.Add(HtmlAttribute.Id, this.Control.ClientID);
			}
			this.HtmlControl.Attributes.Remove(RenderIdAttributeName);
			this.HtmlControl.Attributes.Remove(RenderOriginalIdAttributeName);
			this.HtmlControl.Attributes.Remove(RenderContentOnlyAttributeName);
			this.HtmlControl.Attributes.Remove(TrailingLinefeedAttributeName);
			this.HtmlControl.Attributes.Remove(TrailingBreakAttributeName);
			this.HtmlControl.Attributes.Remove(CssClassAttributeName);
			this.HtmlControl.Attributes.Remove(InnerHtmlAttributeName);
			this.HtmlControl.Attributes.Remove(InnerSpanAttributeName);
            this.HtmlControl.Attributes.Render(writer);
		}
	}
}
