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
using System.Web.UI;
using System.Web.UI.WebControls;

using nJupiter.Web.UI;
using nJupiter.Web.UI.Controls;

namespace nJupiter.Services.Forum.UI.Web {

	public class CategorySelector : UserControl {
		#region Constants
		private const Category.Property Sortproperty = Category.Property.Name;
		private const bool Sortascending = true;
		private const bool Loadattributes = false;
		private const bool DefaultIncludehidden = false;
		private const bool DefaultIncludenoselectionitem = true;
		private const bool DefaultAutopostback = true;

#if DEBUG
		private const string			DebugPrefix							= "_";
#else
		private const string DebugPrefix = "";
#endif
		private const string DefaultFieldsetlabeltext = DebugPrefix + "Input fields";
		private const string DefaultCategoryselectorlabeltext = DebugPrefix + "Categories";
		private const string DefaultSubmitbuttontext = DebugPrefix + "Submit";
		private const string DefaultNoselectionitemtext = DebugPrefix + "Choose a category";
		#endregion

		#region Variables
		private ForumDao forumDao;
		private bool dataBound;
		private bool includeHidden = DefaultIncludehidden;

		private string noSelectionItemText = DefaultNoselectionitemtext;
		private bool includeNoSelectionItem = DefaultIncludenoselectionitem;

		private static readonly object EventCategorySelected = new object();
		#endregion

		#region Events
		public event CategorySelectedEventHandler CategorySelected {
			add { base.Events.AddHandler(EventCategorySelected, value); }
			remove { base.Events.RemoveHandler(EventCategorySelected, value); }
		}
		#endregion

		#region UI Members
		protected WebGenericControl ctrlFieldSetLabel;
		protected WebLabel lblCategories;
		protected WebDropDownList ctrlCategories;
		protected WebPlaceHolder ctrlNoScript;
		protected WebButton btnSubmit;
		#endregion

		#region Properties
		public ForumDao ForumDao { get { return this.forumDao ?? (this.forumDao = ForumDao.GetInstance()); } set { this.forumDao = value; } }

		public bool IncludeHidden { get { return this.includeHidden; } set { this.includeHidden = value; } }
		public string Domain { get; set; }
		public CategoryCollection CategoryCollection { get; set; }

		public bool IncludeNoSelectionItem { get { return this.includeNoSelectionItem; } set { this.includeNoSelectionItem = value; } }
		public string NoSelectionItemText { get { return this.noSelectionItemText; } set { this.noSelectionItemText = value; } }
		public bool AutoPostBack { get { return ctrlCategories.AutoPostBack; } set { ctrlCategories.AutoPostBack = value; } }
		public string FieldSetLabelText { get { return ctrlFieldSetLabel.InnerText; } set { ctrlFieldSetLabel.InnerText = value; } }
		public string CategorySelectorLabelText { get { return lblCategories.InnerText; } set { lblCategories.InnerText = value; } }
		public string SubmitButtonText { get { return btnSubmit.InnerText; } set { btnSubmit.InnerText = value; } }
		public string RedirectUrlWithoutTrailingCategoryId { get; set; }

		public CategoryId SelectedCategoryId { get; set; }
		#endregion

		#region Event Handlers
		private void SelectedIndexChanged(object sender, EventArgs e) {
			this.SelectedCategoryId = ctrlCategories.SelectedValue.Length.Equals(0) ? null : CategoryId.Parse(ctrlCategories.SelectedValue);
			OnCategorySelected(EventArgs.Empty);
			if(this.RedirectUrlWithoutTrailingCategoryId != null && this.SelectedCategoryId != null) {
				this.Response.Redirect(this.RedirectUrlWithoutTrailingCategoryId + this.SelectedCategoryId);
			}
		}
		#endregion

		#region Event Activators
		protected virtual void OnCategorySelected(EventArgs e) {
			CategorySelectedEventHandler eventHandler = base.Events[EventCategorySelected] as CategorySelectedEventHandler;
			if(eventHandler != null) {
				eventHandler(this, e);
			}
		}
		#endregion

		#region Overridden Methods
		protected override void OnInit(EventArgs e) {
			ctrlFieldSetLabel.EnableViewState = lblCategories.EnableViewState = ctrlNoScript.EnableViewState = false;

			this.FieldSetLabelText = DefaultFieldsetlabeltext;
			this.CategorySelectorLabelText = DefaultCategoryselectorlabeltext;
			this.SubmitButtonText = DefaultSubmitbuttontext;
			this.AutoPostBack = DefaultAutopostback;

			btnSubmit.Click += this.SelectedIndexChanged;
			ctrlCategories.SelectedIndexChanged += this.SelectedIndexChanged;
			base.OnInit(e);
		}
		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender(e);
			ctrlNoScript.SurroundingTag = this.AutoPostBack ? HtmlTag.Noscript : null;
			ctrlFieldSetLabel.Visible = this.FieldSetLabelText != null && !this.FieldSetLabelText.Length.Equals(0);
			if(!this.dataBound) {
				this.DataBind();
			}
		}
		public override void DataBind() {
			if(!this.Page.IsPostBack) {
				if(this.CategoryCollection != null && !this.CategoryCollection.Count.Equals(0)) {
					ctrlCategories.Items.AddRange(GetListItemsFromCategories(this.CategoryCollection));
				} else {
					CategoriesResultConfiguration resultConfiguration = new CategoriesResultConfiguration(this.IncludeHidden, Loadattributes, Sortproperty, Sortascending);
					ThreadedPostsResultConfiguration postsResultConfiguration = new ThreadedPostsResultConfiguration(0);
					ctrlCategories.Items.AddRange(GetListItemsFromCategories(this.Domain == null ?
						this.ForumDao.GetCategories(resultConfiguration, postsResultConfiguration) : this.ForumDao.GetCategories(this.Domain, resultConfiguration, postsResultConfiguration)));
				}
				if(this.IncludeNoSelectionItem) {
					ctrlCategories.Items.Insert(0, new ListItem(this.NoSelectionItemText, string.Empty));
				}
			}
			ctrlCategories.SelectedValue = this.SelectedCategoryId == null ? string.Empty : this.SelectedCategoryId.ToString();
			this.dataBound = true;
		}
		#endregion

		#region Helper Methods
		private static ListItem[] GetListItemsFromCategories(CategoryCollection categories) {
			ListItem[] listItems = new ListItem[categories.Count];
			int i = 0;
			foreach(Category category in categories) {
				listItems[i++] = new ListItem(category.Name, category.Id.ToString());
			}
			return listItems;
		}
		#endregion
	}

}