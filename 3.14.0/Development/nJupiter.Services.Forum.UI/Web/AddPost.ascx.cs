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

using nJupiter.Web.UI.Controls;

namespace nJupiter.Services.Forum.UI.Web {

	public class AddPost : UserControl {
		#region Constants
		private const int		TEXTBOX_MAXIMUMLENGTH								= 255;

		private const bool		DEFAULT_MANDATORYFIELDSLABELVISIBLE					= true;
		private	const bool		DEFAULT_TITLETEXTBOXVISIBLE							= true;
		private const bool		DEFAULT_AUTHORTEXTBOXVISIBLE						= true;
		private const bool		DEFAULT_AUTHORTEXTBOXBEFORETITLEANDBODYTEXTBOXES	= true;
		private const bool		DEFAULT_VISIBLECHECKBOXVISIBLE						= true;
		private const bool		DEFAULT_CANCELBUTTONVISIBLE							= true;
		private const bool		DEFAULT_SUBMITBUTTONBEFORECANCELBUTTON				= true;
		private const bool		DEFAULT_BODYISMANDATORY								= true;
		private const int		DEFAULT_MAXIMUMAUTHORLENGTH							= TEXTBOX_MAXIMUMLENGTH;
		private const int		DEFAULT_MAXIMUMTITLELENGTH							= TEXTBOX_MAXIMUMLENGTH;
		private const int		DEFAULT_MAXIMUMBODYLENGTH							= int.MaxValue;
		private const bool		DEFAULT_POSTVISIBLE									= true;
		private const bool		DEFAULT_USETITLEFROMREPLYSOURCE						= false;
		private const bool		DEFAULT_USEBODYFROMREPLYSOURCE						= false;
		private const string	DEFAULT_REPLYINDICATOR								= "Re: ";

#if DEBUG
		private const string	DEBUG_PREFIX										= "_";
#else
		private const string	DEBUG_PREFIX										= "";
#endif

		private const string	DEFAULT_MANDATORYFIELDINDICATOR						= " *";

		private const string	DEFAULT_FIELDSETLABELTEXT							= DEBUG_PREFIX + "Input fields";
		private const string	DEFAULT_MANDATORYFIELDSLABELTEXT					= DEBUG_PREFIX + "* indicates mandatory field";
		private const string	DEFAULT_AUTHORTEXTBOXLABELTEXT						= DEBUG_PREFIX + "Author";
		private const string	DEFAULT_TITLETEXTBOXLABELTEXT						= DEBUG_PREFIX + "Title";
		private const string	DEFAULT_BODYTEXTBOXLABELTEXT						= DEBUG_PREFIX + "Text";
		private const string	DEFAULT_VISIBLECHECKBOXTEXT							= DEBUG_PREFIX + "Visible";
		private const string	DEFAULT_SUBMITBUTTONTEXT							= DEBUG_PREFIX + "Submit";
		private const string	DEFAULT_CANCELBUTTONTEXT							= DEBUG_PREFIX + "Cancel";
		private const string	DEFAULT_ERRORMESSAGEGENERAL							= DEBUG_PREFIX + "The form was not submitted because of the following error(s). If possible, correct them and submit again.";
		private const string	DEFAULT_ERRORMESSAGEAUTHORNOTSPECIFIED				= DEBUG_PREFIX + "You have to specify an author.";
		private const string	DEFAULT_ERRORMESSAGETITLENOTSPECIFIED				= DEBUG_PREFIX + "You have to specify a title.";
		private const string	DEFAULT_ERRORMESSAGEBODYNOTSPECIFIED				= DEBUG_PREFIX + "You have to specify a text.";
		private const string	DEFAULT_ERRORMESSAGEBODYTOOLONG						= DEBUG_PREFIX + "You have to specify a shorter text.";
		private const string	DEFAULT_ERRORMESSAGECATEGORYNOTFOUND				= DEBUG_PREFIX + "The category to add this post in does not exist.";
		private const string	DEFAULT_ERRORMESSAGEPOSTNOTFOUND					= DEBUG_PREFIX + "The post to add this post in does not exist.";

		private const string	VIEWSTATE_DOMAIN									= "v_Domain";
		private const string	VIEWSTATE_CATEGORYNAME								= "v_CategoryName";
		private const string	VIEWSTATE_CATEGORYID								= "v_CategoryId";
		private const string	VIEWSTATE_POSTID									= "v_PostId";
		private const string	VIEWSTATE_USERIDENTITY								= "v_UserIdentity";
		private const string	VIEWSTATE_USEDPOSTID								= "v_UsedPostId";

		private const string	CSSCLASS_ERRORMESSAGE								= "portlet-msg-error";
		#endregion

		#region Variables
		private ForumDao				m_ForumDao;
		private bool					m_DataBound;
		private PostId					m_ReplySourcePostId;
		private PostQuoter				m_PostQuoter;
		private bool					m_TitleTextBoxVisible						= DEFAULT_TITLETEXTBOXVISIBLE;
		private bool					m_UseTitleFromReplySource					= DEFAULT_USETITLEFROMREPLYSOURCE;
		private bool					m_UseBodyFromReplySource					= DEFAULT_USEBODYFROMREPLYSOURCE;
		private string					m_ReplyIndicator							= DEFAULT_REPLYINDICATOR;
		private bool					m_MandatoryFieldsLabelVisible				= DEFAULT_MANDATORYFIELDSLABELVISIBLE;
		private bool					m_AuthorTextBoxVisible						= DEFAULT_AUTHORTEXTBOXVISIBLE;
		private bool					m_AuthorTextBoxBeforeTitleAndBodyTextBoxes	= DEFAULT_AUTHORTEXTBOXBEFORETITLEANDBODYTEXTBOXES;
		private bool					m_VisibleCheckBoxVisible					= DEFAULT_VISIBLECHECKBOXVISIBLE;
		private bool					m_CancelButtonVisible						= DEFAULT_CANCELBUTTONVISIBLE;
		private bool					m_SubmitButtonBeforeCancelButton			= DEFAULT_SUBMITBUTTONBEFORECANCELBUTTON;
		private bool					m_BodyIsMandatory							= DEFAULT_BODYISMANDATORY;
		private int						m_MaximumBodyLength							= DEFAULT_MAXIMUMBODYLENGTH;
		private string					m_MandatoryFieldIndicator					= DEFAULT_MANDATORYFIELDINDICATOR;
		private string					m_ErrorMessageGeneral						= DEFAULT_ERRORMESSAGEGENERAL;
		private string					m_ErrorMessageAuthorNotSpecified			= DEFAULT_ERRORMESSAGEAUTHORNOTSPECIFIED;
		private string					m_ErrorMessageTitleNotSpecified				= DEFAULT_ERRORMESSAGETITLENOTSPECIFIED;
		private string					m_ErrorMessageBodyNotSpecified				= DEFAULT_ERRORMESSAGEBODYNOTSPECIFIED;
		private string					m_ErrorMessageBodyTooLong					= DEFAULT_ERRORMESSAGEBODYTOOLONG;
		private string					m_ErrorMessageCategoryNotFound				= DEFAULT_ERRORMESSAGECATEGORYNOTFOUND;
		private string					m_ErrorMessagePostNotFound					= DEFAULT_ERRORMESSAGEPOSTNOTFOUND;

		private static readonly object	s_EventPostAdding							= new object();
		private static readonly object	s_EventPostAdded							= new object();
		private static readonly object	s_EventPostAdditionFailed					= new object();
		private static readonly object	s_EventPostAdditionDiscarded				= new object();
		#endregion

		#region Events
		public event PostAddingEventHandler PostAdding { 
			add { base.Events.AddHandler(s_EventPostAdding, value); } 
			remove { base.Events.RemoveHandler(s_EventPostAdding, value); } 
		}
		public event PostAddedEventHandler PostAdded {	
			add { base.Events.AddHandler(s_EventPostAdded, value); } 
			remove { base.Events.RemoveHandler(s_EventPostAdded, value); } 
		}
		public event PostAdditionFailedEventHandler	PostAdditionFailed  {	
			add { base.Events.AddHandler(s_EventPostAdditionFailed, value); } 
			remove { base.Events.RemoveHandler(s_EventPostAdditionFailed, value); } 
		}
		public event PostAdditionDiscardedEventHandler PostAdditionDiscarded  {	
			add { base.Events.AddHandler(s_EventPostAdditionDiscarded, value); } 
			remove { base.Events.RemoveHandler(s_EventPostAdditionDiscarded, value); } 
		}
		#endregion

		#region UI Members
		protected WebParagraph		ctrlGeneralError;
		protected WebParagraph		ctrlAuthorError;
		protected WebPlaceHolder	ctrlAuthorErrorAltPosition;
		protected WebParagraph		ctrlTitleError;
		protected WebParagraph		ctrlBodyError;
		protected WebParagraph		ctrlOtherError;
		protected WebGenericControl	ctrlFieldSetLabel;
		protected WebParagraph		ctrlMandatoryFieldsLabel;
		protected WebParagraph		ctrlAuthor;
		protected WebLabel			lblAuthor;
		protected TextBox			txtAuthor;
		protected WebPlaceHolder	ctrlAuthorAltPosition;
		protected WebParagraph		ctrlTitle;
		protected WebLabel			lblTitle;
		protected TextBox			txtTitle;
		protected WebLabel			lblBody;
		protected TextBox			txtBody;
		protected WebParagraph		ctrlVisible;
		protected WebCheckBox		chkVisible;
		protected WebButton			btnSubmit;
		protected WebButton			btnCancel;
		protected WebPlaceHolder	ctrlSubmitAltPosition;
		#endregion

		#region Properties
		public ForumDao ForumDao { get { return this.m_ForumDao ?? (this.m_ForumDao = ForumDao.GetInstance()); } set { m_ForumDao = value; } }

		public string Domain { 
			get { return (string)this.ViewState[VIEWSTATE_DOMAIN]; } 
			set { 
				if(value != null) {
					this.ViewState.Remove(VIEWSTATE_POSTID);
					this.ViewState.Remove(VIEWSTATE_CATEGORYID);
				}
				this.ViewState.Add(VIEWSTATE_DOMAIN, value);
			}
		}
		public string CategoryName {
			get { return (string)this.ViewState[VIEWSTATE_CATEGORYNAME]; }
			set { 
				if(value != null) {
					this.ViewState.Remove(VIEWSTATE_POSTID);
					this.ViewState.Remove(VIEWSTATE_CATEGORYID);
				}
				this.ViewState.Add(VIEWSTATE_CATEGORYNAME, value);
			}
		}
		public CategoryId CategoryId { 
			get { return (CategoryId)this.ViewState[VIEWSTATE_CATEGORYID]; } 
			set { 
				if(value != null) {
					this.ViewState.Remove(VIEWSTATE_POSTID);
					this.ViewState.Remove(VIEWSTATE_DOMAIN);
					this.ViewState.Remove(VIEWSTATE_CATEGORYNAME);
				}
				this.ViewState.Add(VIEWSTATE_CATEGORYID, value); 
			} 
		}
		public PostId PostId { 
			get { return (PostId)this.ViewState[VIEWSTATE_POSTID]; } 
			set { 
				if(value != null) {
					this.ViewState.Remove(VIEWSTATE_CATEGORYID);
					this.ViewState.Remove(VIEWSTATE_DOMAIN);
					this.ViewState.Remove(VIEWSTATE_CATEGORYNAME);
				}
				this.ViewState.Add(VIEWSTATE_POSTID, value); 
			} 
		}
		public string UserIdentity { get { return (string)this.ViewState[VIEWSTATE_USERIDENTITY]; } set { this.ViewState.Add(VIEWSTATE_USERIDENTITY, value); } }
		public string Author { 
			get { return txtAuthor.Text; } 
			set {
				if(value != null && value.Length > this.MaximumAuthorLength) {
					throw new ArgumentOutOfRangeException("value");
				}
				txtAuthor.Text = value; 
			} 
		}
		public string Title { 
			get { return txtTitle.Text; } 
			set { 
				if(value != null && value.Length > this.MaximumTitleLength) {
					throw new ArgumentOutOfRangeException("value");
				}
				txtTitle.Text = value;
			} 
		}
		public string Body { get { return txtBody.Text; } set { txtBody.Text = value; } }
		public bool PostVisible { get { return chkVisible.Checked; } set { chkVisible.Checked = value; } }

		public bool MandatoryFieldsLabelVisible { get { return m_MandatoryFieldsLabelVisible; } set { m_MandatoryFieldsLabelVisible = value; } }
		public bool AuthorTextBoxVisible { get { return m_AuthorTextBoxVisible; } set { m_AuthorTextBoxVisible = value; } }
		public bool AuthorTextBoxBeforeTitleAndBodyTextBoxes { get { return m_AuthorTextBoxBeforeTitleAndBodyTextBoxes; } set { m_AuthorTextBoxBeforeTitleAndBodyTextBoxes = value; } }
		public bool TitleTextBoxVisible { get { return m_TitleTextBoxVisible; } set { m_TitleTextBoxVisible = value; } } 
		public bool VisibleCheckBoxVisible { get { return m_VisibleCheckBoxVisible; } set { m_VisibleCheckBoxVisible = value; } }
		public bool CancelButtonVisible { get { return m_CancelButtonVisible; } set { m_CancelButtonVisible = value; } }
		public bool SubmitButtonBeforeCancelButton { get { return m_SubmitButtonBeforeCancelButton; } set { m_SubmitButtonBeforeCancelButton = value; }	}
		public int MaximumAuthorLength {
			get { return txtAuthor.MaxLength; }
			set {
				if(value > TEXTBOX_MAXIMUMLENGTH || value < 1) {
					throw new ArgumentOutOfRangeException("value");
				}
				txtAuthor.MaxLength = value;
			}
		}
		public int MaximumTitleLength {
			get { return txtTitle.MaxLength; }
			set {
				if(value > TEXTBOX_MAXIMUMLENGTH || value < 1) {
					throw new ArgumentOutOfRangeException("value");
				}
				txtTitle.MaxLength = value;
			}
		}
		public int MaximumBodyLength { 
			get { return m_MaximumBodyLength; } 
			set { 
				if(value < 1) {
					throw new ArgumentOutOfRangeException("value");
				}
				m_MaximumBodyLength = value; 
			} 
		}
		public bool BodyIsMandatory { get { return m_BodyIsMandatory; } set { m_BodyIsMandatory = value; } }
		public string MandatoryFieldIndicator { get { return m_MandatoryFieldIndicator; } set { m_MandatoryFieldIndicator = value; } }

		public bool UseTitleFromReplySource { get { return m_UseTitleFromReplySource; } set { m_UseTitleFromReplySource = value; } }
		public bool UseBodyFromReplySource { get { return m_UseBodyFromReplySource; } set { m_UseBodyFromReplySource = value; } }
		public PostId ReplySourcePostId { get { return m_ReplySourcePostId; } set { m_ReplySourcePostId = value; } }
		public string ReplyIndicator { get { return m_ReplyIndicator; } set { m_ReplyIndicator = value; } }
		public PostQuoter PostQuoter { get { return m_PostQuoter; } set { m_PostQuoter = value; } }

		public string FieldSetLabelText { get { return ctrlFieldSetLabel.InnerText; } set { ctrlFieldSetLabel.InnerText = value; } }
		public string MandatoryFieldsLabelText { get { return ctrlMandatoryFieldsLabel.InnerText; } set { ctrlMandatoryFieldsLabel.InnerText = value; } }
		public string AuthorTextBoxLabelText { get { return lblAuthor.InnerText; } set { lblAuthor.InnerText = value; } }
		public string TitleTextBoxLabelText { get { return lblTitle.InnerText; } set { lblTitle.InnerText = value; } }
		public string BodyTextBoxLabelText { get { return lblBody.InnerText; } set { lblBody.InnerText = value; } }
		public string VisibleCheckBoxText { get { return chkVisible.Text; } set { chkVisible.Text = value; } }
		public string SubmitButtonText { get { return btnSubmit.InnerText; } set { btnSubmit.InnerText = value; } }
		public string CancelButtonText { get { return btnCancel.InnerText; } set { btnCancel.InnerText = value; } }
		public string ErrorMessageGeneral { get { return m_ErrorMessageGeneral; } set { m_ErrorMessageGeneral = value; } }
		public string ErrorMessageAuthorNotSpecified { get { return m_ErrorMessageAuthorNotSpecified; } set { m_ErrorMessageAuthorNotSpecified = value; } }
		public string ErrorMessageTitleNotSpecified { get { return m_ErrorMessageTitleNotSpecified; } set { m_ErrorMessageTitleNotSpecified = value; } }
		public string ErrorMessageBodyNotSpecified { get { return m_ErrorMessageBodyNotSpecified; } set { m_ErrorMessageBodyNotSpecified = value; } }
		public string ErrorMessageBodyTooLong { get { return m_ErrorMessageBodyTooLong; } set { m_ErrorMessageBodyTooLong = value; } }
		public string ErrorMessageCategoryNotFound { get { return m_ErrorMessageCategoryNotFound; } set { m_ErrorMessageCategoryNotFound = value; } }
		public string ErrorMessagePostNotFound { get { return m_ErrorMessagePostNotFound; } set { m_ErrorMessagePostNotFound = value; } }

		private PostId UsedPostId { get { return (PostId)this.ViewState[VIEWSTATE_USEDPOSTID]; } set { this.ViewState.Add(VIEWSTATE_USEDPOSTID, value); } }
		#endregion
		
		#region Event Handlers
		private void btnSubmit_Click(object sender, EventArgs e) {
			if(this.CategoryId == null && this.PostId == null && !(this.Domain != null && this.CategoryName != null)) {
				throw new InvalidOperationException(@"One and only one of the ""CategoryId"" or ""PostId"" properties or else both the ""Domain"" and ""CategoryName"" properties must be set to a non null value.");
			}
			ResetErrorFields();
			Post post;

			if(this.PostId != null) {
				post = this.ForumDao.CreatePostInstance(this.PostId);
			} else if(this.CategoryId != null) {
				post = this.ForumDao.CreatePostInstance(this.CategoryId);
			} else {
				post = this.ForumDao.CreatePostInstance(this.Domain, this.CategoryName);
			}

			post.Author							= this.Author;
			post.UserIdentity					= this.UserIdentity;
			post.Title							= this.Title;
			post.Body							= this.Body;
			post.Visible						= this.PostVisible;
			PostCancelEventArgs cancelEventArgs	= new PostCancelEventArgs(post);
			OnPostAdding(cancelEventArgs);
			if(!cancelEventArgs.Cancel) {
				bool isValid		= true;
				Exception exception	= null;
				if(this.AuthorTextBoxVisible && (this.Author == null || this.Author.TrimEnd().Length.Equals(0))) {
					SetErrorField(lblAuthor, ctrlAuthorError, this.ErrorMessageAuthorNotSpecified);
					isValid			= false;
				}
				if(this.TitleTextBoxVisible && (this.Title == null || this.Title.TrimEnd().Length.Equals(0))) {
					SetErrorField(lblTitle, ctrlTitleError, this.ErrorMessageTitleNotSpecified);
					isValid			= false;
				}
				//if title text box is not visible, body is always mandatory
				if((this.BodyIsMandatory || !this.TitleTextBoxVisible) && (this.Body == null || this.Body.TrimEnd().Length.Equals(0))) {
					SetErrorField(lblBody, ctrlBodyError, this.ErrorMessageBodyNotSpecified);
					isValid			= false;
				} else if(this.Body != null && this.Body.Length > this.MaximumBodyLength) {
					SetErrorField(lblBody, ctrlBodyError, this.ErrorMessageBodyTooLong);
					isValid			= false;
				}
				if(isValid) {
					try {
						this.ForumDao.SavePost(post);
						OnPostAdded(new PostEventArgs(post));
						ResetFields();
					} catch(Exception ex) {
						string otherError;
						if(ex is CategoryNotFoundException) {
							otherError	= this.ErrorMessageCategoryNotFound;
						} else if(ex is PostNotFoundException) {
							otherError	= this.ErrorMessagePostNotFound;
						} else {
							otherError	= ex.Message;
						}
						SetErrorField(null, ctrlOtherError, otherError);
						exception		= ex;
						isValid			= false;
					}
				}
				if(!isValid) {
					SetErrorField(null, ctrlGeneralError, this.ErrorMessageGeneral);
					OnPostAdditionFailed(new PostFailureEventArgs(post, exception));
				}
			}
		}
		private void btnCancel_Click(object sender, EventArgs e) {
			ResetFields();
			OnPostAdditionDiscarded(EventArgs.Empty);
		}
		#endregion

		#region Event Activators
		protected virtual void OnPostAdding(PostCancelEventArgs e) {
			PostAddingEventHandler eventHandler = base.Events[s_EventPostAdding] as PostAddingEventHandler;
			if(eventHandler != null) {
				eventHandler(this, e);
			}
		}
		protected virtual void OnPostAdded(PostEventArgs e) {
			PostAddedEventHandler eventHandler = base.Events[s_EventPostAdded] as PostAddedEventHandler;
			if(eventHandler != null) {
				eventHandler(this, e);
			}
		}
		protected virtual void OnPostAdditionFailed(PostFailureEventArgs e) {
			PostAdditionFailedEventHandler eventHandler = base.Events[s_EventPostAdditionFailed] as PostAdditionFailedEventHandler;
			if(eventHandler != null) {
				eventHandler(this, e);
			}
		}
		protected virtual void OnPostAdditionDiscarded(EventArgs e) {
			PostAdditionDiscardedEventHandler eventHandler = base.Events[s_EventPostAdditionDiscarded] as PostAdditionDiscardedEventHandler;
			if(eventHandler != null) {
				eventHandler(this, e);
			}
		}
		#endregion

		#region Overridden Methods
		protected override void OnInit(EventArgs e) {
			ctrlGeneralError.EnableViewState = ctrlAuthorError.EnableViewState = ctrlTitleError.EnableViewState = 
				ctrlBodyError.EnableViewState = ctrlOtherError.EnableViewState = lblAuthor.EnableViewState = lblTitle.EnableViewState = 
				lblBody.EnableViewState = ctrlFieldSetLabel.EnableViewState = ctrlMandatoryFieldsLabel.EnableViewState =
				ctrlAuthorErrorAltPosition.EnableViewState = ctrlAuthorAltPosition.EnableViewState = ctrlSubmitAltPosition.EnableViewState = 
				btnSubmit.EnableViewState = btnCancel.EnableViewState = false;

			ResetErrorFields();
			this.MaximumAuthorLength		= DEFAULT_MAXIMUMAUTHORLENGTH;
			this.MaximumTitleLength			= DEFAULT_MAXIMUMTITLELENGTH;
			this.MaximumBodyLength			= DEFAULT_MAXIMUMBODYLENGTH;
			this.FieldSetLabelText			= DEFAULT_FIELDSETLABELTEXT;
			this.MandatoryFieldsLabelText	= DEFAULT_MANDATORYFIELDSLABELTEXT;
			this.AuthorTextBoxLabelText		= DEFAULT_AUTHORTEXTBOXLABELTEXT;
			this.TitleTextBoxLabelText		= DEFAULT_TITLETEXTBOXLABELTEXT;
			this.BodyTextBoxLabelText		= DEFAULT_BODYTEXTBOXLABELTEXT;
			this.VisibleCheckBoxText		= DEFAULT_VISIBLECHECKBOXTEXT;
			this.SubmitButtonText			= DEFAULT_SUBMITBUTTONTEXT;
			this.CancelButtonText			= DEFAULT_CANCELBUTTONTEXT;

			this.PostVisible				= DEFAULT_POSTVISIBLE;

			btnSubmit.Click					+= this.btnSubmit_Click;
			btnCancel.Click					+= this.btnCancel_Click;

			base.OnInit(e);
		}
		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender(e);
			ctrlMandatoryFieldsLabel.Visible	= this.MandatoryFieldsLabelVisible;
			ctrlAuthor.Visible					= this.AuthorTextBoxVisible;
			ctrlTitle.Visible					= this.TitleTextBoxVisible;
			ctrlVisible.Visible					= this.VisibleCheckBoxVisible;
			btnCancel.Visible					= this.CancelButtonVisible;
			if(!this.AuthorTextBoxBeforeTitleAndBodyTextBoxes && this.AuthorTextBoxVisible) {
				this.Controls.Remove(ctrlAuthor);
				this.Controls.AddAt(this.Controls.IndexOf(ctrlAuthorAltPosition), ctrlAuthor);
				this.Controls.Remove(ctrlAuthorError);
				this.Controls.AddAt(this.Controls.IndexOf(ctrlAuthorErrorAltPosition), ctrlAuthorError);
			}
			if(!this.SubmitButtonBeforeCancelButton && this.CancelButtonVisible) {
				this.Controls.Remove(btnSubmit);
				this.Controls.AddAt(this.Controls.IndexOf(ctrlSubmitAltPosition), btnSubmit);
			}
			if(this.MandatoryFieldIndicator != null) {
				if(this.AuthorTextBoxVisible) {
					lblAuthor.Controls.Add(GetMandatoryFieldIndicator());
				}
				if(this.TitleTextBoxVisible) {
					lblTitle.Controls.Add(GetMandatoryFieldIndicator());
				}
				//if title text box is not visible, body is always mandatory
				if(this.BodyIsMandatory || !this.TitleTextBoxVisible) {
					lblBody.Controls.Add(GetMandatoryFieldIndicator());
				}
			}
			ctrlFieldSetLabel.Visible = this.FieldSetLabelText != null && !this.FieldSetLabelText.Length.Equals(0);
			if(!m_DataBound) {
				this.DataBind();
			}
		}
		public override void DataBind() {
			Post post;
			bool useTitle	= this.UseTitleFromReplySource && (this.Title == null || this.Title.TrimEnd().Length.Equals(0));
			bool useBody	= this.UseBodyFromReplySource && (this.Body == null || this.Body.TrimEnd().Length.Equals(0));
			if((useTitle || useBody) && this.ReplySourcePostId != null && !this.ReplySourcePostId.Equals(this.UsedPostId) && 
				(post = this.ForumDao.GetPost(this.ReplySourcePostId, new ThreadedPostsResultConfiguration(1))) != null) {
				if(useTitle) {
					string newTitle = this.ReplyIndicator == null || post.Title.StartsWith(this.ReplyIndicator) ? post.Title : this.ReplyIndicator + post.Title;
					this.Title = newTitle.Length > this.MaximumTitleLength ? newTitle.Substring(0, this.MaximumTitleLength) : newTitle;
				}
				if(useBody) {
					this.Body = this.PostQuoter != null ? this.PostQuoter.QuotePost(post) : post.Body;
				}
				this.UsedPostId = this.ReplySourcePostId;
			}
			m_DataBound = true;
		}
		#endregion

		#region Helper Methods
		private void ResetFields() {
			this.Author = this.Title = this.Body = null;
			this.PostVisible	= DEFAULT_POSTVISIBLE;
			this.UsedPostId		= null;
			ResetErrorFields();
		}
		private void ResetErrorFields() {
			ctrlGeneralError.Visible = ctrlAuthorError.Visible = ctrlTitleError.Visible = 
				ctrlBodyError.Visible = ctrlOtherError.Visible = false;
			lblAuthor.CssClass = lblTitle.CssClass = lblBody.CssClass = string.Empty;
		}
		private Literal GetMandatoryFieldIndicator() {
			Literal mandatoryFieldIndicator	= new Literal();
			mandatoryFieldIndicator.Text	= this.MandatoryFieldIndicator;
			return mandatoryFieldIndicator;
		}
		private static void SetErrorField(WebLabel label, WebParagraph paragraph, string error) {
			if(label != null) {
				label.CssClass	= CSSCLASS_ERRORMESSAGE;
			}
			paragraph.InnerText	= error;
			paragraph.Visible	= true;
		}
		#endregion
	}

}