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

using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using System.Collections.Specialized;

namespace nJupiter.Web.UI.Controls {
	public class WebRadioButton : RadioButton {

		#region Members
		private string uniqueGroupName;
		private bool idExplicitlySet;
		#endregion

		#region Properties
		public override string ID {
			get { return base.ID; }
			set {
				this.idExplicitlySet = true;
				base.ID = value;
			}
		}
		public string Value {
			get { return base.Attributes[HtmlAttribute.Value]; }
			set { base.Attributes[HtmlAttribute.Value] = value; }
		}

		private string ValueAttribute {
			get {
				string attributeValue = base.Attributes[HtmlAttribute.Value];
				if(attributeValue != null) {
					return attributeValue;
				}
				if(this.ID != null && this.idExplicitlySet && this.NamingContainer is WebRadioButtonList) {
					return this.ID;
				}
				return this.UniqueID;
			}
		}
		#endregion

		#region Methods
		protected override void Render(HtmlTextWriter writer) {
			if (this.Page != null) {
				this.Page.VerifyRenderingInServerForm(this);
			}

			string text = this.Text;
			string clientID = this.ClientID;
			if (text.Length != 0) {
				if (this.TextAlign == System.Web.UI.WebControls.TextAlign.Left) {
					RenderLabel(writer, text, clientID);
					RenderInputTag(writer, clientID);
				} else {
					RenderInputTag(writer, clientID);
					RenderLabel(writer, text, clientID);
				}
			} else {
				this.RenderInputTag(writer, clientID);
			}
		}

		private void RenderInputTag(HtmlTextWriter writer, string clientID) {

			string onClick = null;
			if (this.Attributes.Count > 0) {
				System.Web.UI.AttributeCollection attributes = base.Attributes;
				string attributeValue = attributes[HtmlAttribute.Value];
				if (attributeValue != null) {
					attributes.Remove(HtmlAttribute.Value);
				}
				onClick = attributes[HtmlAttribute.Onclick];
				if (onClick != null) {
					attributes.Remove(HtmlAttribute.Onclick);
				}
				if (attributes.Count != 0) {
					attributes.AddAttributes(writer);
				}
				if (attributeValue != null) {
					attributes[HtmlAttribute.Value] = attributeValue;
				}
			}

			writer.AddAttribute(HtmlTextWriterAttribute.Id, clientID);
			writer.AddAttribute(HtmlTextWriterAttribute.Type, HtmlAttributeValue.Radio);
			writer.AddAttribute(HtmlTextWriterAttribute.Name, this.UniqueGroupName);
			writer.AddAttribute(HtmlTextWriterAttribute.Value, this.ValueAttribute);

			string toolTip = this.ToolTip;
			if (toolTip.Length > 0) {
				writer.AddAttribute(HtmlTextWriterAttribute.Title, toolTip);
			}

			if (this.CssClass.Length > 0)
				writer.AddAttribute(HtmlTextWriterAttribute.Class, this.CssClass);

			if (this.Checked) {
				writer.AddAttribute(HtmlTextWriterAttribute.Checked, HtmlAttributeValue.Checked);
			}
			if (!this.Enabled) {
				writer.AddAttribute(HtmlTextWriterAttribute.Disabled, HtmlAttributeValue.Disabled);
			}
			if (this.AutoPostBack) {
				if (onClick != null)
					onClick = onClick + this.Page.ClientScript.GetPostBackEventReference(this, string.Empty);
				else
					onClick = this.Page.ClientScript.GetPostBackEventReference(this, string.Empty);
				writer.AddAttribute(HtmlTextWriterAttribute.Onclick, onClick);
			} else if (onClick != null) {
				writer.AddAttribute(HtmlTextWriterAttribute.Onclick, onClick);
			}

			string accessKey = this.AccessKey;
			if (accessKey.Length > 0) {
				writer.AddAttribute(HtmlTextWriterAttribute.Accesskey, accessKey);
			}
			int tabIndex = this.TabIndex;
			if (tabIndex != 0) {
				writer.AddAttribute(HtmlTextWriterAttribute.Tabindex, tabIndex.ToString(NumberFormatInfo.InvariantInfo));
			}
			writer.RenderBeginTag(HtmlTextWriterTag.Input);
			writer.RenderEndTag();
		}

		private static void RenderLabel(HtmlTextWriter writer, string text, string clientID) {
			writer.AddAttribute(HtmlTextWriterAttribute.For, clientID);
			writer.RenderBeginTag(HtmlTextWriterTag.Label);
			writer.Write(text);
			writer.RenderEndTag();
		}

		private string UniqueGroupName {
			get {
				if(this.uniqueGroupName == null){
					string groupName = this.GroupName;
					string uniqueID = this.UniqueID;
					if(uniqueID != null) {
						if(this.NamingContainer is WebRadioButtonList) {
							int lastIndexOf = uniqueID.LastIndexOf(base.IdSeparator);
							groupName = uniqueID.Substring(0, lastIndexOf);
						}
						if(groupName.Length == 0) {
							groupName = uniqueID;
						}
					}
					this.uniqueGroupName = groupName;
				}
				return this.uniqueGroupName;
			}
		}

		protected override bool LoadPostData(string postDataKey, NameValueCollection postCollection) {
			string uniqueId = postCollection[this.UniqueGroupName];
			bool loadPostData = false;
			if ((uniqueId != null) && uniqueId.Equals(this.ValueAttribute)) {
				if (!this.Checked) {
					this.Checked = true;
					loadPostData = true;
				}
				return loadPostData;
			}
			if (this.Checked) {
				this.Checked = false;
			}
			return loadPostData;
		}
		#endregion

	}
}
