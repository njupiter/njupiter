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

using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

namespace nJupiter.Web.UI.Controls {

	public class WebImage : WebControl {

		private const string BrTag = "<" + HtmlTag.Br + " />";
		private const string RenderIdKey = "v_RenderId";
		private const string RenderOriginalIdKey = "v_RenderOriginalId";
		private const string MaxWidthKey = "v_MaxWidth";
		private const string MaxHeightKey = "v_MaxHeight";
		private const string ForceStreamingKey = "v_ForceStreaming";
		private const string AllowEnlargingKey = "v_AllowEnlarging";
		private const string AllowStretchingKey = "v_AllowStretching";
		private const string TrailingLinefeedKey = "v_TrailingLinefeed";
		private const string TrailingBreakKey = "v_TrailingBreak";
		private const string StreamPagePathKey = "v_StreamingPath";

		public WebImage() : base(HtmlTextWriterTag.Img) {
			WebImageResourceRegistrator.Register();
		}
	
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

		public virtual string AlternateText {
			get {
				var alternateText = (string)this.ViewState[HtmlAttribute.Alt];
				if(alternateText != null) {
					return alternateText;
				}
				return string.Empty;
			}
			set {
				this.ViewState[HtmlAttribute.Alt] = value;
			}
		}

		public virtual string DescriptionUrl {
			get {
				var descriptionUrl = (string)this.ViewState[HtmlAttribute.Longdesc];
				if(descriptionUrl != null) {
					return descriptionUrl;
				}
				return string.Empty;
			}
			set {
				this.ViewState[HtmlAttribute.Longdesc] = value;
			}
		}

		public virtual string ImageUrl {
			get {
				var imageUrl = (string)this.ViewState[HtmlAttribute.Src];
				if(imageUrl != null) {
					return imageUrl;
				}
				return string.Empty;
			}
			set {
				this.ViewState[HtmlAttribute.Src] = value;
			}
		}

		public int MaxWidth {
			get {
				if(this.ViewState[MaxWidthKey] == null)
					return 0;
				return (int)this.ViewState[MaxWidthKey];
			}
			set {
				this.ViewState[MaxWidthKey] = value;
			}
		}

		public int MaxHeight {
			get {
				if(this.ViewState[MaxHeightKey] == null)
					return 0;
				return (int)this.ViewState[MaxHeightKey];
			}
			set {
				this.ViewState[MaxHeightKey] = value;
			}
		}

		public bool ForceStreaming {
			get {
				if(this.ViewState[ForceStreamingKey] == null)
					return false;
				return (bool)this.ViewState[ForceStreamingKey];
			}
			set {
				this.ViewState[ForceStreamingKey] = value;
			}
		}

		public virtual string StreamingPath {
			get {
				var streamingPath = (string)this.ViewState[StreamPagePathKey];
				if(streamingPath != null) {
					return streamingPath;
				}
				return WebImageResourceRegistrator.StreamPagePath;
			}
			set {
				this.ViewState[StreamPagePathKey] = value;
			}
		}

		public bool AllowEnlarging {
			get {
				if(this.ViewState[AllowEnlargingKey] == null)
					return false;
				return (bool)this.ViewState[AllowEnlargingKey];
			}
			set {
				this.ViewState[AllowEnlargingKey] = value;
			}
		}

		public bool AllowStretching {
			get {
				if(this.ViewState[AllowStretchingKey] == null)
					return false;
				return (bool)this.ViewState[AllowStretchingKey];
			}
			set {
				this.ViewState[AllowStretchingKey] = value;
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

		protected override void AddAttributesToRender(HtmlTextWriter writer) {
			var text = this.ImageUrl;
			if(text.Length > 0) {
				writer.AddAttribute(HtmlTextWriterAttribute.Src, this.RenderImageUrl());
			}
			text = this.DescriptionUrl;
			if(text.Length > 0) {
				writer.AddAttribute(HtmlAttribute.Longdesc, text);
			}
			var toolTip = this.ToolTip;
			if(toolTip.Length > 0) {
				writer.AddAttribute(HtmlTextWriterAttribute.Title, toolTip);
				this.ToolTip = string.Empty;
			}
			text = this.AlternateText;
			writer.AddAttribute(HtmlTextWriterAttribute.Alt, text);
			base.AddAttributesToRender(writer);
			this.ToolTip = toolTip;
		}

		public virtual string RenderImageUrl() {
			if(this.MaxWidth > 0 || this.MaxHeight > 0 || this.ForceStreaming) {
				var streamingPath = UrlHandler.Instance.AddQueryParams(this.StreamingPath, "path=" + System.Web.HttpUtility.UrlEncode(this.ImageUrl));
				if(this.MaxWidth > 0 || this.MaxHeight > 0) {
					streamingPath = UrlHandler.Instance.AddQueryParams(streamingPath, "width=" + this.MaxWidth, "height=" + this.MaxHeight, "allowEnlarging=" + this.AllowEnlarging, "allowStretching=" + this.AllowStretching);
				}
				return streamingPath;
			}
			return this.ImageUrl;
		}

		protected override void Render(HtmlTextWriter writer) {
			if(this.ImageUrl.Length.Equals(0))
				return;
			if(!this.RenderOriginalId) {
				var originalId = this.ID;
				this.ID = null;
				if(this.RenderId)
					this.Attributes.Add(HtmlAttribute.Id, originalId);
			}
			base.Render(writer);
			if(this.TrailingBreak)
				writer.Write(BrTag);
			if(this.TrailingLinefeed)
				writer.WriteLine();
		}

		protected override void RenderContents(HtmlTextWriter writer) {
			// Supress content rendering.
		}

	}
}