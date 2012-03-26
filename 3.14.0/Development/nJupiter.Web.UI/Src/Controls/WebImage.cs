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
using System.ComponentModel;

namespace nJupiter.Web.UI.Controls {

	public class WebImage : WebControl {

		#region Constants
		private const string BrTag					= "<" + HtmlTag.Br + " />";
		private const string RenderIDKey			= "v_RenderId";
		private const string RenderOriginalIDKey	= "v_RenderOriginalId";
		private const string MaxWidthKey			= "v_MaxWidth";
		private const string MaxHeightKey			= "v_MaxHeight";
		private const string AllowEnlargingKey		= "v_AllowEnlarging";
		private const string AllowStretchingKey		= "v_AllowStretching";
		private const string TrailingLinefeedKey	= "v_TrailingLinefeed";
		private const string TrailingBreakKey		= "v_TrailingBreak";
		private const string StreamPagePathKey		= "v_StreamingPath";
		private const string StreamPagePath			= "/nJupiter/nJupiter.Web.UI/Web/StreamImage.aspx";
		#endregion
		
		#region Constructors
		public WebImage() : base(HtmlTextWriterTag.Img) {}
		#endregion

		#region Properties
		public bool RenderId{
			get {
				if(this.ViewState[RenderIDKey] == null)
					return false;
				return (bool)this.ViewState[RenderIDKey];
			}
			set {
				this.ViewState[RenderIDKey] = value;
			}
		}
		
		public bool RenderOriginalId{
			get {
				if(this.ViewState[RenderOriginalIDKey] == null)
					return false;
				return (bool)this.ViewState[RenderOriginalIDKey];
			}
			set {
				this.ViewState[RenderOriginalIDKey] = value;
			}
		}

		public virtual string AlternateText {
			get {
				string alternateText = (string) this.ViewState[HtmlAttribute.Alt];
				if (alternateText != null) {
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
				string descriptionUrl = (string) this.ViewState[HtmlAttribute.Longdesc];
				if (descriptionUrl != null) {
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
				string imageUrl = (string) this.ViewState[HtmlAttribute.Src];
				if (imageUrl != null) {
					return imageUrl;
				}
				return string.Empty;
			}
			set {
				this.ViewState[HtmlAttribute.Src] = value;
			}
		}

		public int MaxWidth{
			get {
				if(this.ViewState[MaxWidthKey] == null)
					return 0;
				return (int)this.ViewState[MaxWidthKey];
			}
			set {
				this.ViewState[MaxWidthKey] = value;
			}
		}

		public int MaxHeight{
			get {
				if(this.ViewState[MaxHeightKey] == null)
					return 0;
				return (int)this.ViewState[MaxHeightKey];
			}
			set {
				this.ViewState[MaxHeightKey] = value;
			}
		}

		public virtual string StreamingPath {
			get {
				string streamingPath = (string)this.ViewState[StreamPagePathKey];
				if (streamingPath != null) {
					return streamingPath;
				}
				return StreamPagePath;
			}
			set {
				this.ViewState[StreamPagePathKey] = value;
			}
		}
		
		public bool AllowEnlarging{
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
				if (this.ViewState[TrailingBreakKey] == null) {
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
				if (this.ViewState[TrailingLinefeedKey] == null) {
					return false;
				}
				return (bool)this.ViewState[TrailingLinefeedKey];
			}
			set {
				this.ViewState[TrailingLinefeedKey] = value;
			}
		}
		#endregion

		#region Methods
		protected override void AddAttributesToRender(HtmlTextWriter writer) {
			string text = this.ImageUrl;
			if (text.Length > 0) {
				writer.AddAttribute(HtmlTextWriterAttribute.Src, this.RenderImageUrl());
			}
			text = this.DescriptionUrl;
			if (text.Length > 0) {
				writer.AddAttribute(HtmlAttribute.Longdesc, text);
			}
			string toolTip = this.ToolTip;
			if (toolTip.Length > 0) {
				writer.AddAttribute(HtmlTextWriterAttribute.Title, toolTip);
				this.ToolTip = string.Empty;
			}
			text = this.AlternateText;
			writer.AddAttribute(HtmlTextWriterAttribute.Alt, text);
			base.AddAttributesToRender(writer);
			this.ToolTip = toolTip;
		}

		public virtual string RenderImageUrl() {
			if(this.MaxWidth > 0 || this.MaxHeight > 0){
				return nJupiter.Web.UrlHandler.AddQueryParams(this.StreamingPath, "path=" + System.Web.HttpUtility.UrlEncode(this.ImageUrl), "width=" + this.MaxWidth, "height=" + this.MaxHeight, "allowEnlarging=" + this.AllowEnlarging, "allowStretching=" + this.AllowStretching);
			}
			return this.ImageUrl;
		}

		protected override void Render(HtmlTextWriter writer) {
			if(this.ImageUrl.Length.Equals(0))
				return;
			if(!this.RenderOriginalId){
				string originalID = this.ID;
				this.ID = null;
				if(this.RenderId)
					this.Attributes.Add(HtmlAttribute.Id, originalID);
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
		#endregion

	}
}