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
using System.Web.UI.WebControls;
using System.ComponentModel;

namespace nJupiter.Web.UI.Controls {

	[ToolboxItem(true), DefaultEvent("Click")]
	public class WebButton : HtmlButton, IPostBackDataHandler, INamingContainer {

		private const string BrTag = "<" + HtmlTag.Br + " />";
		private const string InnerSpanKey = "v_InnerSpan";
		private const string CommandNameKey = "v_CommandName";
		private const string CommandArgKey = "v_CommandArgument";
		private const string TrailingLinefeedKey = "v_TrailingLinefeed";
		private const string TrailingBreakKey = "v_TrailingBreak";

		private static readonly object EventCommand = new object();
		private static readonly object EventClick = new object();
		private static readonly object EventServerClick = new object();

		new public event EventHandler ServerClick {
			add {
				base.Events.AddHandler(EventServerClick, value);
			}
			remove {
				base.Events.RemoveHandler(EventServerClick, value);
			}
		}

		public event EventHandler Click {
			add { base.Events.AddHandler(EventClick, value); }
			remove { base.Events.RemoveHandler(EventClick, value); }
		}

		public event CommandEventHandler Command {
			add { base.Events.AddHandler(EventCommand, value); }
			remove { base.Events.RemoveHandler(EventCommand, value); }
		}

		public string Type {
			get {
				var result = this.ViewState[HtmlAttribute.Type] as string;
				if(result != null) {
					return result;
				}
				return HtmlAttributeValue.Submit;
			}
			set {
				this.ViewState[HtmlAttribute.Type] = value;
			}
		}

		public string CssClass {
			get {
				var result = this.ViewState[HtmlAttribute.Class] as string;
				if(result != null) {
					return result;
				}
				return string.Empty;
			}
			set {
				this.ViewState[HtmlAttribute.Class] = value;
			}
		}

		public string OnClick {
			get {
				var result = this.ViewState[HtmlAttribute.Onclick] as string;
				if(result != null) {
					return result;
				}
				return string.Empty;
			}
			set {
				this.ViewState[HtmlAttribute.Onclick] = value;
			}
		}

		public string CommandArgument {
			get {
				var commandArgument = (string)this.ViewState[CommandArgKey];
				if(commandArgument != null) {
					return commandArgument;
				}
				return string.Empty;
			}
			set {
				this.ViewState[CommandArgKey] = value;
			}
		}

		public string CommandName {
			get {
				var commandName = (string)this.ViewState[CommandNameKey];
				if(commandName != null) {
					return commandName;
				}
				return string.Empty;
			}
			set {
				this.ViewState[CommandNameKey] = value;
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
		public bool IsClicked {
			get {
				if(UserAgent.Instance.IsPreIE7)
					return this.Page.Request.Form["__EVENTTARGET"] == this.UniqueID;
				return this.Page.Request.Form[this.UniqueID] != null;
			}
		}

		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender(e);
			if(UserAgent.Instance.IsPreIE7) {
				var onClick = this.Page.ClientScript.GetPostBackEventReference(this, string.Empty);
				var currentOnclick = this.OnClick;
				if(currentOnclick.IndexOf(onClick, StringComparison.Ordinal) < 0)
					this.Attributes.Add(HtmlAttribute.Onclick, (currentOnclick.Length > 0 ? currentOnclick + ";" + onClick : onClick));
			}
			if(!UserAgent.Instance.IsPreIE7 || this.Type != HtmlAttributeValue.Submit)
				this.Attributes.Add(HtmlAttribute.Type, this.Type);
		}

		protected virtual void OnClickEvent(EventArgs e) {
			var handler = (EventHandler)base.Events[EventClick];
			if(handler != null)
				handler(this, e);
		}

		protected virtual void OnCommandEvent(CommandEventArgs e) {
			var handler = (CommandEventHandler)base.Events[EventCommand];
			if(handler != null)
				handler(this, e);
			base.RaiseBubbleEvent(this, e);
		}

		protected override void OnServerClick(EventArgs e) {
			var handler = (EventHandler)base.Events[EventServerClick];
			if(handler != null) {
				handler(this, e);
			}
		}

		protected override void RenderAttributes(HtmlTextWriter writer) {
			if(this.CssClass.Length > 0)
				this.Attributes.Add(HtmlAttribute.Class, this.CssClass);

			this.Attributes.Add(HtmlAttribute.Name, this.UniqueID);
			base.Attributes.Remove(CommandArgKey);
			base.Attributes.Remove(CommandNameKey);

			var eventServerClickExists = base.Events[EventServerClick] != null;
			if((this.Page != null) && eventServerClickExists) {
				ControlHandler.Instance.WriteOnClickAttribute(writer, this, false, true, this.CausesValidation && (this.Page.GetValidators(this.ValidationGroup).Count > 0), this.ValidationGroup);
			}

			base.RenderAttributes(writer);
		}

		protected override void RenderBeginTag(HtmlTextWriter writer) {
			base.RenderBeginTag(writer);
			if(this.InnerSpan)
				writer.WriteFullBeginTag(HtmlTag.Span);
		}

		protected override void RenderEndTag(HtmlTextWriter writer) {
			if(this.InnerSpan)
				writer.WriteEndTag(HtmlTag.Span);
			base.RenderEndTag(writer);
			if(this.TrailingBreak)
				writer.Write(BrTag);
			if(this.TrailingLinefeed)
				writer.WriteLine();
		}

		bool IPostBackDataHandler.LoadPostData(string postDataKey, System.Collections.Specialized.NameValueCollection postCollection) {
			return LoadPostData();
		}

		protected static bool LoadPostData() {
			return true;
		}

		void IPostBackDataHandler.RaisePostDataChangedEvent() {
			RaisePostDataChangedEvent();
		}

		protected void RaisePostDataChangedEvent() {
			if(this.IsClicked) {
				this.OnClickEvent(EventArgs.Empty);
				this.OnCommandEvent(new CommandEventArgs(this.CommandName, this.CommandArgument));
			}
		}

		protected override void RaisePostBackEvent(string eventArgument) {
			if(this.CausesValidation) {
				this.Page.Validate(this.ValidationGroup);
			}
			this.OnServerClick(EventArgs.Empty);
			if(!UserAgent.Instance.IsPreIE7) {
				this.OnClickEvent(EventArgs.Empty);
			}
			this.OnCommandEvent(new CommandEventArgs(this.CommandName, this.CommandArgument));
		}

	}
}
