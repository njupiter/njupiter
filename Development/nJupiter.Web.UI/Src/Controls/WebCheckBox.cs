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
using System.Web.UI.WebControls;
using System.Globalization;

namespace nJupiter.Web.UI.Controls {

	public class WebCheckBox : CheckBox {

		public string Value {
			get {
				var result = this.ViewState[HtmlAttribute.Value] as string;
				if(result != null) {
					return result;
				}
				return string.Empty;
			}
			set {
				this.ViewState[HtmlAttribute.Value] = value;
			}
		}

		protected override void Render(HtmlTextWriter writer) {
			if(this.Page != null) {
				this.Page.VerifyRenderingInServerForm(this);
			}

			var text = this.Text;
			var clientId = this.ClientID;
			if(text.Length != 0) {
				if(this.TextAlign == TextAlign.Left) {
					RenderLabel(writer, text, clientId);
					RenderInputTag(writer, clientId);
				} else {
					RenderInputTag(writer, clientId);
					RenderLabel(writer, text, clientId);
				}
			} else {
				this.RenderInputTag(writer, clientId);
			}
		}

		private void RenderInputTag(HtmlTextWriter writer, string clientId) {
			string onClick = null;
			if(this.Attributes.Count > 0) {
				var attributes = base.Attributes;
				var attributeValue = attributes[HtmlAttribute.Value];
				if(attributeValue != null) {
					attributes.Remove(HtmlAttribute.Value);
				}
				onClick = attributes[HtmlAttribute.Onclick];
				if(onClick != null) {
					attributes.Remove(HtmlAttribute.Onclick);
				}
				if(attributes.Count != 0) {
					attributes.AddAttributes(writer);
				}
				if(attributeValue != null) {
					attributes[HtmlAttribute.Value] = attributeValue;
				}
			}
			if(!string.IsNullOrEmpty(this.Value)) {
				writer.AddAttribute(HtmlTextWriterAttribute.Value, this.Value);
			}

			writer.AddAttribute(HtmlTextWriterAttribute.Id, clientId);
			writer.AddAttribute(HtmlTextWriterAttribute.Type, HtmlAttributeValue.Checkbox);
			writer.AddAttribute(HtmlTextWriterAttribute.Name, this.UniqueID);

			var toolTip = this.ToolTip;
			if(toolTip.Length > 0) {
				writer.AddAttribute(HtmlTextWriterAttribute.Title, toolTip);
			}

			if(this.CssClass.Length > 0)
				writer.AddAttribute(HtmlTextWriterAttribute.Class, this.CssClass);

			if(this.Checked) {
				writer.AddAttribute(HtmlTextWriterAttribute.Checked, HtmlAttributeValue.Checked);
			}
			if(!this.Enabled) {
				writer.AddAttribute(HtmlTextWriterAttribute.Disabled, HtmlAttributeValue.Disabled);
			}
			if(this.AutoPostBack) {
				if(onClick != null)
					onClick = onClick + this.Page.ClientScript.GetPostBackEventReference(this, string.Empty);
				else
					onClick = this.Page.ClientScript.GetPostBackEventReference(this, string.Empty);
				writer.AddAttribute(HtmlTextWriterAttribute.Onclick, onClick);
			} else if(onClick != null) {
				writer.AddAttribute(HtmlTextWriterAttribute.Onclick, onClick);
			}

			var accessKey = this.AccessKey;
			if(accessKey.Length > 0) {
				writer.AddAttribute(HtmlTextWriterAttribute.Accesskey, accessKey);
			}
			int tabIndex = this.TabIndex;
			if(tabIndex != 0) {
				writer.AddAttribute(HtmlTextWriterAttribute.Tabindex, tabIndex.ToString(NumberFormatInfo.InvariantInfo));
			}
			if(!this.InputAttributes.Count.Equals(0)) {
				this.InputAttributes.AddAttributes(writer);
			}
			writer.RenderBeginTag(HtmlTextWriterTag.Input);
			writer.RenderEndTag();
		}

		private static void RenderLabel(HtmlTextWriter writer, string text, string clientId) {
			//TODO: we should take care of CheckBox.LabelAttributes here also
			writer.AddAttribute(HtmlTextWriterAttribute.For, clientId);
			writer.RenderBeginTag(HtmlTextWriterTag.Label);
			writer.Write(text);
			writer.RenderEndTag();
		}

	}
}
