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
		private const Category.Property	SORTPROPERTY							= Category.Property.Name;
		private const bool				SORTASCENDING							= true;
		private const bool				LOADATTRIBUTES							= false;
		private const bool				DEFAULT_INCLUDEHIDDEN					= false;
		private const bool				DEFAULT_INCLUDENOSELECTIONITEM			= true;
		private const bool				DEFAULT_AUTOPOSTBACK					= true;

#if DEBUG
		private const string			DEBUG_PREFIX							= "_";
#else
		private const string			DEBUG_PREFIX							= "";
#endif
		private const string			DEFAULT_FIELDSETLABELTEXT				= DEBUG_PREFIX + "Input fields";
		private const string			DEFAULT_CATEGORYSELECTORLABELTEXT		= DEBUG_PREFIX + "Categories";
		private const string			DEFAULT_SUBMITBUTTONTEXT				= DEBUG_PREFIX + "Submit";
		private const string			DEFAULT_NOSELECTIONITEMTEXT				= DEBUG_PREFIX + "Choose a category";
		#endregion

		#region Variables
		private ForumDao				m_ForumDao;
		private bool					m_DataBound;
		private string					m_Domain;
		private bool					m_IncludeHidden							= DEFAULT_INCLUDEHIDDEN;
		private CategoryCollection		m_CategoryCollection;

		private string					m_NoSelectionItemText					= DEFAULT_NOSELECTIONITEMTEXT;
		private bool					m_IncludeNoSelectionItem				= DEFAULT_INCLUDENOSELECTIONITEM;
		private string					m_RedirectUrlWithoutTrailingCategoryId;

		private CategoryId				m_SelectedCategoryId;

		private static readonly object	s_EventCategorySelected					= new object();
		#endregion

		#region Events
		public event CategorySelectedEventHandler CategorySelected { 
			add { base.Events.AddHandler(s_EventCategorySelected, value); } 
			remove { base.Events.RemoveHandler(s_EventCategorySelected, value); } 
		}
		#endregion

		#region UI Members
		protected WebGenericControl ctrlFieldSetLabel;
		protected WebLabel			lblCategories;
		protected WebDropDownList	ctrlCategories;
		protected WebPlaceHolder	ctrlNoScript;
		protected WebButton			btnSubmit;
		#endregion

		#region Properties
		public ForumDao ForumDao { get { return this.m_ForumDao ?? (this.m_ForumDao = ForumDao.GetInstance()); } set { m_ForumDao = value; } }
		
		public bool IncludeHidden { get { return m_IncludeHidden; } set { m_IncludeHidden = value; } }
		public string Domain { get { return m_Domain; } set { m_Domain = value; } }
		public CategoryCollection CategoryCollection { get { return m_CategoryCollection; } set { m_CategoryCollection = value; } }

		public bool IncludeNoSelectionItem { get { return m_IncludeNoSelectionItem; } set { m_IncludeNoSelectionItem = value; } }
		public string NoSelectionItemText { get { return m_NoSelectionItemText; } set { m_NoSelectionItemText = value; } }
		public bool AutoPostBack { get { return ctrlCategories.AutoPostBack; } set { ctrlCategories.AutoPostBack = value; } }
		public string FieldSetLabelText { get { return ctrlFieldSetLabel.InnerText; } set { ctrlFieldSetLabel.InnerText = value; } }
		public string CategorySelectorLabelText { get { return lblCategories.InnerText; } set { lblCategories.InnerText = value; } }
		public string SubmitButtonText { get { return btnSubmit.InnerText; } set { btnSubmit.InnerText = value; } }
		public string RedirectUrlWithoutTrailingCategoryId { get { return m_RedirectUrlWithoutTrailingCategoryId; } set { m_RedirectUrlWithoutTrailingCategoryId = value; } }

		public CategoryId SelectedCategoryId { get { return m_SelectedCategoryId; } set { m_SelectedCategoryId = value; } }
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
			CategorySelectedEventHandler eventHandler = base.Events[s_EventCategorySelected] as CategorySelectedEventHandler;
			if(eventHandler != null) {
				eventHandler(this, e);
			}
		}
		#endregion

		#region Overridden Methods
		protected override void OnInit(EventArgs e) {
			ctrlFieldSetLabel.EnableViewState = lblCategories.EnableViewState = ctrlNoScript.EnableViewState = false;

			this.FieldSetLabelText			= DEFAULT_FIELDSETLABELTEXT;
			this.CategorySelectorLabelText	= DEFAULT_CATEGORYSELECTORLABELTEXT;
			this.SubmitButtonText			= DEFAULT_SUBMITBUTTONTEXT;
			this.AutoPostBack				= DEFAULT_AUTOPOSTBACK;

			btnSubmit.Click						+= this.SelectedIndexChanged;
			ctrlCategories.SelectedIndexChanged	+= this.SelectedIndexChanged;
			base.OnInit(e);
		}
		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender(e);
			ctrlNoScript.SurroundingTag		= this.AutoPostBack ? HtmlTag.Noscript : null;
			ctrlFieldSetLabel.Visible		= this.FieldSetLabelText != null && !this.FieldSetLabelText.Length.Equals(0);
			if(!m_DataBound) {
				this.DataBind();
			}
		}
		public override void DataBind() {
			if(!this.Page.IsPostBack) {
				if(this.CategoryCollection != null && !this.CategoryCollection.Count.Equals(0)) {
					ctrlCategories.Items.AddRange(GetListItemsFromCategories(this.CategoryCollection));
				} else {
					CategoriesResultConfiguration resultConfiguration			= new CategoriesResultConfiguration(this.IncludeHidden, LOADATTRIBUTES, SORTPROPERTY, SORTASCENDING);
					ThreadedPostsResultConfiguration postsResultConfiguration	= new ThreadedPostsResultConfiguration(0);
					ctrlCategories.Items.AddRange(GetListItemsFromCategories(this.Domain == null ? 
						this.ForumDao.GetCategories(resultConfiguration, postsResultConfiguration) : this.ForumDao.GetCategories(this.Domain, resultConfiguration, postsResultConfiguration)));
				}
				if(this.IncludeNoSelectionItem) {
					ctrlCategories.Items.Insert(0, new ListItem(this.NoSelectionItemText, string.Empty));
				}
			}
			ctrlCategories.SelectedValue	= this.SelectedCategoryId == null ? string.Empty : this.SelectedCategoryId.ToString();
			m_DataBound						= true;
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