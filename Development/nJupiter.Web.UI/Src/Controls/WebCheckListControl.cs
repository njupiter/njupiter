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
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using System.Collections;
using System.Collections.Specialized;

namespace nJupiter.Web.UI.Controls {

	[Obsolete("Try using System.Web.UI.WebControls.CheckBoxList or System.Web.UI.WebControls.RadioButtonList instead if possible")]
	public class WebCheckListControl : ListControl, IRepeatInfoUser, IPostBackDataHandler, INamingContainer {

		private const string ListitemAttributes		= "v_ListItemAttributes";

		private StringWriter			stringWriter;
		private HtmlTextWriter			htmlTextWriter;
		private bool					hasChanged;

		private readonly CheckBox		controlToRepeat;
		private readonly RadioButton	radioButton;

		private HtmlTextWriter HtmlTextWriter{
			get { return this.htmlTextWriter ?? (this.htmlTextWriter = new HtmlTextWriter(this.StringWriter)); }
		}

		private StringWriter StringWriter{
			get { return this.stringWriter ?? (this.stringWriter = new StringWriter(CultureInfo.InvariantCulture)); }
		}

		public WebCheckListControl(CheckBox controlToRepeat) {
			if(controlToRepeat == null)
				throw new ArgumentNullException("controlToRepeat");
			//This line differs in the CheckBoxList implementation
			this.controlToRepeat						= controlToRepeat;
			this.radioButton							= this.controlToRepeat as RadioButton;
			this.controlToRepeat.ID					= "0";
			this.controlToRepeat.EnableViewState		= false;
			base.Controls.Add(this.controlToRepeat);
			this.hasChanged							= false;
		}

		/// <summary>Searches the current naming container for a server control with the specified ID and path offset. The method always returns the current WebCheckListControl object. </summary>
        /// <returns>The current WebCheckListControl</returns>
        /// <param name="id">The identifier for the control to find.</param>
        /// <param name="pathOffset">The number of controls up the page control hierarchy needed to reach a naming container. </param>
		protected override Control FindControl(string id, int pathOffset) {
			return this;
		}
		
		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender(e);
			this.controlToRepeat.AutoPostBack = this.AutoPostBack;
			if (this.Page != null && !this.EnableViewState) {
				for (var i = 0; i < this.Items.Count; i++){
					if (this.radioButton == null || this.Items[i].Selected){
						this.controlToRepeat.ID = i.ToString(NumberFormatInfo.InvariantInfo);
						this.Page.RegisterRequiresPostBack(this.controlToRepeat);
					}
				}
			}
		}

		protected override void LoadViewState(object savedState) {
			base.LoadViewState (savedState);
			// Put eventually viewstated attributes back in the ListItem
			if(this.ViewState[ListitemAttributes] != null){
				var viewStatedAttributes = (Hashtable)this.ViewState[ListitemAttributes];
				foreach(ListItem listItem in this.Items){
					var listItemEntry = new DictionaryEntry(listItem.Value, listItem.Text);
					if(viewStatedAttributes.Contains(listItemEntry)){
						var attributes = (ArrayList)viewStatedAttributes[listItemEntry];
						foreach(var o in attributes){
							var attribute = (DictionaryEntry)o;
							listItem.Attributes.Add(attribute.Key.ToString(), attribute.Value.ToString());
						}
					}
				}
			}
		}

		protected override object SaveViewState() {
			if (this.Page != null && this.EnableViewState) {
				for (var i = 0; i < this.Items.Count; i++){
					if (this.radioButton == null || this.Items[i].Selected){
						this.controlToRepeat.ID = i.ToString(NumberFormatInfo.InvariantInfo);
						this.Page.RegisterRequiresPostBack(this.controlToRepeat);
					}
				}
			}
			// .NET does not viewstate the attributes in a ListItem so we have to do it ourselvs
			var newAttributes = new Hashtable();
			foreach(ListItem listItem in this.Items){
				var attributes = new ArrayList();
				foreach(string key in listItem.Attributes.Keys){
					var attribute = new DictionaryEntry(key, listItem.Attributes[key]);
					attributes.Add(attribute);
				}
				if(attributes.Count > 0){
					var listItemEntry = new DictionaryEntry(listItem.Value, listItem.Text);
					newAttributes.Add(listItemEntry, attributes);
				}
			}
			this.ViewState[ListitemAttributes] = (newAttributes.Count > 0 ? newAttributes : null);
			
			return base.SaveViewState ();
		}

		protected override void Render(HtmlTextWriter writer) {
			var repeatInfo = new RepeatInfo();
			if (!this.ControlStyleCreated) {
				var tabIndex = this.TabIndex;
				var flag = false;

				if(Convert.ToBoolean(tabIndex)) {
					if (!this.ViewState.IsItemDirty("TabIndex"))
						flag = true;
					this.TabIndex = 0;
				}
				repeatInfo.RepeatLayout = RepeatLayout.Flow;

				// Create dummy htmlTextWriter because repeatInfo generates an unwanted span-tag
				// So we generate the items in the global private htmlTextWriter.
				using(var sw = new StringWriter(CultureInfo.InvariantCulture)) {
					using(var repWriter = new HtmlTextWriter(sw)) {
						repeatInfo.RenderRepeater(repWriter, this, null, this);
					}
				}
				
				writer.Write(this.StringWriter); // Copy the private htmlTextWriter into the current htmlTextWriter.

				if (Convert.ToBoolean(tabIndex))
					this.TabIndex = tabIndex;
				if (Convert.ToBoolean(flag))
					this.ViewState.SetItemDirty("TabIndex", false);
			}
		}

		bool IPostBackDataHandler.LoadPostData(string postDataKey, NameValueCollection postCollection) {
			return LoadPostData(postDataKey, postCollection);
		}
		
		protected bool LoadPostData(string postDataKey, NameValueCollection postCollection) {
			if(postDataKey == null)
				throw new ArgumentNullException("postDataKey");
			if(postCollection == null)
				throw new ArgumentNullException("postCollection");
			if(base.IsEnabled) {
				var postDataValue = postCollection[postDataKey];
				var selected = (postDataValue != null);

				this.EnsureDataBound();
				
				if(this.radioButton != null) {
					var selectedIndex = this.SelectedIndex;
					for(var i = 0; i < this.Items.Count; i++) {
						if(postDataValue == this.Items[i].Value) {
							if(i != selectedIndex) {
								this.hasChanged = true;
								this.SelectedIndex = i;
								return true;
							}
						}
					}
				} else {
					var keyLength = postDataKey.Substring(this.UniqueID.Length + 1);
					var intKey = int.Parse(keyLength, NumberFormatInfo.InvariantInfo);
					if(intKey >= 0 && intKey < this.Items.Count) {
						if(this.Items[intKey].Selected != selected) {
							this.Items[intKey].Selected = selected;
							if(!this.hasChanged) {
								this.hasChanged = true;
								return true;
							}
						}
					}
				}
			}
			return false; 
		}

		void IPostBackDataHandler.RaisePostDataChangedEvent() {
			RaisePostDataChangedEvent();
		}

		protected void RaisePostDataChangedEvent(){
			if (this.radioButton != null || this.hasChanged)
				this.OnSelectedIndexChanged(EventArgs.Empty);

		}

		protected static bool HasFooter { get { return false; } }

		protected static bool HasHeader { get { return false; } }

		protected static bool HasSeparators { get { return false; } }

		protected int RepeatedItemCount { get { return this.Items.Count; } }

		protected static Style GetItemStyle(ListItemType itemType, int repeatIndex) {
			return null;
		}

		protected void RenderItem(ListItemType itemType, int repeatIndex, RepeatInfo repeatInfo, HtmlTextWriter writer) {
			
			var listItem = this.Items[repeatIndex];
			
			this.controlToRepeat.Attributes.Clear();
			this.controlToRepeat.Attributes[HtmlAttribute.Value] = listItem.Value;
			foreach(string key in listItem.Attributes.Keys){
				this.controlToRepeat.Attributes.Add(key, listItem.Attributes[key]);
			}
			this.controlToRepeat.ID = repeatIndex.ToString(NumberFormatInfo.InvariantInfo);
			this.controlToRepeat.Checked = listItem.Selected;
			this.controlToRepeat.Enabled = listItem.Enabled && this.Enabled;


			this.HtmlTextWriter.WriteFullBeginTag(HtmlTag.P);
			this.controlToRepeat.RenderControl(this.HtmlTextWriter);

			//Render the label control right next to the checkbox
			this.RenderItemLabel(listItem.Text, this.controlToRepeat.ClientID, this.HtmlTextWriter);
			this.HtmlTextWriter.WriteEndTag(HtmlTag.P);
			this.HtmlTextWriter.WriteLine(string.Empty);
		}
		protected virtual void RenderItemLabel(string text, string clientId, HtmlTextWriter writer) {
			//Create a label control to show the text in
			var lblText	= new WebLabel();
			lblText.InnerText	= text;
			lblText.For			= clientId;
			//Render the label control right next to the checkbox
			lblText.RenderControl(writer);
		}
		bool IRepeatInfoUser.HasFooter { get { return HasFooter; } }

		bool IRepeatInfoUser.HasHeader { get { return HasHeader; } }

		bool IRepeatInfoUser.HasSeparators { get { return HasSeparators; } }

		int IRepeatInfoUser.RepeatedItemCount { get { return this.RepeatedItemCount; } }

		Style IRepeatInfoUser.GetItemStyle(ListItemType itemType, int repeatIndex) {
			return GetItemStyle(itemType, repeatIndex);
		}

		void IRepeatInfoUser.RenderItem(ListItemType itemType, int repeatIndex, RepeatInfo repeatInfo, HtmlTextWriter writer) {
			RenderItem(itemType, repeatIndex, repeatInfo, writer);
		}

	}
}
