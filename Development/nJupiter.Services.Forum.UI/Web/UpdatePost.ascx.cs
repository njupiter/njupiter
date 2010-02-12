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

	public class UpdatePost : UserControl {
		#region Constants
		private const bool		DEFAULT_IGNORECONCURRENTUPDATE						= false;

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
		private const string	DEFAULT_ERRORMESSAGEGENERAL							= DEBUG_PREFIX + "The form was not submitted because of the following error(s). If possible, correct them, and submit again.";
		private const string	DEFAULT_ERRORMESSAGEAUTHORNOTSPECIFIED				= DEBUG_PREFIX + "You have to specify an author.";
		private const string	DEFAULT_ERRORMESSAGETITLENOTSPECIFIED				= DEBUG_PREFIX + "You have to specify a title.";
		private const string	DEFAULT_ERRORMESSAGEBODYNOTSPECIFIED				= DEBUG_PREFIX + "You have to specify a text.";
		private const string	DEFAULT_ERRORMESSAGEBODYTOOLONG						= DEBUG_PREFIX + "You have to specify a shorter text.";
		private const string	DEFAULT_ERRORMESSAGEPOSTCONCURRENTLYUPDATED			= DEBUG_PREFIX + "This post has been concurrently updated and this update can not proceed. Reopen the form and try again.";
		private const string	DEFAULT_ERRORMESSAGEPOSTCONCURRENTLYDELETED			= DEBUG_PREFIX + "This post has been concurrently deleted.";
		
		private const string	VIEWSTATE_POSTID									= "v_PostId";
		private const string	VIEWSTATE_POST										= "v_Post";
		private const string	VIEWSTATE_ORIGINALAUTHOR							= "v_OriginalAuthor";
		private const string	VIEWSTATE_ORIGINALTITLE								= "v_OriginalTitle";
		private const string	VIEWSTATE_ORIGINALBODY								= "v_OriginalBody";
		private const string	VIEWSTATE_ORIGINALVISIBLE							= "v_OriginalVisible";

		private const string	CSSCLASS_ERRORMESSAGE								= "portlet-msg-error";
		#endregion

		#region Variables
		private ForumDao				m_ForumDao;
		private bool					m_DataBound;
		private bool					m_IgnoreConcurrentUpdate					= DEFAULT_IGNORECONCURRENTUPDATE;

		private bool					m_MandatoryFieldsLabelVisible				= DEFAULT_MANDATORYFIELDSLABELVISIBLE;
		private bool					m_AuthorTextBoxVisible						= DEFAULT_AUTHORTEXTBOXVISIBLE;
		private bool					m_AuthorTextBoxBeforeTitleAndBodyTextBoxes	= DEFAULT_AUTHORTEXTBOXBEFORETITLEANDBODYTEXTBOXES;
		private bool					m_TitleTextBoxVisible						= DEFAULT_TITLETEXTBOXVISIBLE;
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
		private string					m_ErrorMessagePostConcurrentlyUpdated		= DEFAULT_ERRORMESSAGEPOSTCONCURRENTLYUPDATED;
		private string					m_ErrorMessagePostConcurrentlyDeleted		= DEFAULT_ERRORMESSAGEPOSTCONCURRENTLYDELETED;

		private static readonly object	s_EventPostUpdating							= new object();
		private static readonly object	s_EventPostUpdated							= new object();
		private static readonly object	s_EventPostUpdateFailed						= new object();
		private static readonly object	s_EventPostUpdateDiscarded					= new object();
		#endregion

		#region Events
		public event PostUpdatingEventHandler PostUpdating { 
			add { base.Events.AddHandler(s_EventPostUpdating, value); } 
			remove { base.Events.RemoveHandler(s_EventPostUpdating, value); } 
		}
		public event PostUpdatedEventHandler PostUpdated {	
			add { base.Events.AddHandler(s_EventPostUpdated, value); } 
			remove { base.Events.RemoveHandler(s_EventPostUpdated, value); } 
		}
		public event PostUpdateFailedEventHandler PostUpdateFailed  {	
			add { base.Events.AddHandler(s_EventPostUpdateFailed, value); } 
			remove { base.Events.RemoveHandler(s_EventPostUpdateFailed, value); } 
		}
		public event PostUpdateDiscardedEventHandler PostUpdateDiscarded  {	
			add { base.Events.AddHandler(s_EventPostUpdateDiscarded, value); } 
			remove { base.Events.RemoveHandler(s_EventPostUpdateDiscarded, value); } 
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
		protected WebPlaceHolder	ctrlSubmitAltPosition;
		protected WebButton			btnCancel;
		#endregion

		#region Properties
		public ForumDao ForumDao { get { return this.m_ForumDao ?? (this.m_ForumDao = ForumDao.GetInstance()); } set { m_ForumDao = value; } }
		public bool IgnoreConcurrentUpdate { get { return m_IgnoreConcurrentUpdate; } set { m_IgnoreConcurrentUpdate = value; } }
		public PostId PostId {
			get { return (PostId)this.ViewState[VIEWSTATE_POSTID]; }
			set {
				this.ViewState.Add(VIEWSTATE_POSTID, value);
				this.Post = null;
			}
		}

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
		public string ErrorMessagePostConcurrentlyUpdated { get { return m_ErrorMessagePostConcurrentlyUpdated; } set { m_ErrorMessagePostConcurrentlyUpdated = value; } }
		public string ErrorMessagePostConcurrentlyDeleted { get { return m_ErrorMessagePostConcurrentlyDeleted; } set { m_ErrorMessagePostConcurrentlyDeleted = value; } }

		private Post Post { get { return (Post)this.ViewState[VIEWSTATE_POST]; } set { this.ViewState.Add(VIEWSTATE_POST, value); } }
		private string OriginalAuthor { get { return (string)this.ViewState[VIEWSTATE_ORIGINALAUTHOR]; } set { this.ViewState.Add(VIEWSTATE_ORIGINALAUTHOR, value); } }
		private string OriginalTitle { get { return (string)this.ViewState[VIEWSTATE_ORIGINALTITLE]; } set { this.ViewState.Add(VIEWSTATE_ORIGINALTITLE, value); } }
		private string OriginalBody { get { return (string)this.ViewState[VIEWSTATE_ORIGINALBODY]; } set { this.ViewState.Add(VIEWSTATE_ORIGINALBODY, value); } }
		private bool OriginalVisible { get { return (bool)this.ViewState[VIEWSTATE_ORIGINALVISIBLE]; } set { this.ViewState.Add(VIEWSTATE_ORIGINALVISIBLE, value); } }
		#endregion
		
		#region Event Handlers
		private void btnSubmit_Click(object sender, EventArgs e) {
			ResetErrorFields();
			SetPostProperties();
			PostCancelEventArgs cancelEventArgs	= new PostCancelEventArgs(this.Post);
			OnPostUpdating(cancelEventArgs);
			if(!cancelEventArgs.Cancel) {
				bool isValid		= true;
				Exception exception	= null;
				if(this.AuthorTextBoxVisible && (txtAuthor.Text == null || txtAuthor.Text.TrimEnd().Length.Equals(0))) {
					SetErrorField(lblAuthor, ctrlAuthorError, this.ErrorMessageAuthorNotSpecified);
					isValid = false;
				}
				if(this.TitleTextBoxVisible && txtTitle.Text == null || txtTitle.Text.TrimEnd().Length.Equals(0)) {
					SetErrorField(lblTitle, ctrlTitleError, this.ErrorMessageTitleNotSpecified);
					isValid = false;
				}
				//if title text box is not visible, body is always mandatory
				if((this.BodyIsMandatory || !this.TitleTextBoxVisible) && (txtBody.Text == null || txtBody.Text.TrimEnd().Length.Equals(0))) {
					SetErrorField(lblBody, ctrlBodyError, this.ErrorMessageBodyNotSpecified);
					isValid = false;
				} else if(txtBody.Text != null && txtBody.Text.Length > this.MaximumBodyLength) {
					SetErrorField(lblBody, ctrlBodyError, this.ErrorMessageBodyTooLong);
					isValid = false;
				}
				if(isValid) {
					while(true) {
						try {
							this.ForumDao.SavePost(this.Post);
							this.OriginalAuthor		= this.Post.Author;
							this.OriginalTitle		= this.Post.Title;
							this.OriginalBody		= this.Post.Body;
							this.OriginalVisible	= this.Post.Visible;
							OnPostUpdated(new PostEventArgs(this.Post));
							ResetFields();
						} catch(Exception ex) {
							string otherError;
							if(ex is ConcurrentUpdateException) {
								if(this.IgnoreConcurrentUpdate) {
									if((this.Post = this.ForumDao.GetPost(this.PostId, new ThreadedPostsResultConfiguration(1))) == null) {
										otherError = this.ErrorMessagePostConcurrentlyDeleted;
									} else {
										SetPostProperties();
										continue;
									}
								} else {
									otherError = this.ErrorMessagePostConcurrentlyUpdated;
								}
							} else if(ex is PostNotFoundException) {
								otherError = this.ErrorMessagePostConcurrentlyDeleted;
							} else {
								otherError = ex.Message;
							}
							SetErrorField(null, ctrlOtherError, otherError);
							exception = ex;
							isValid = false;
						}
						break;
					}
				}
				if(!isValid) {
					SetErrorField(null, ctrlGeneralError, this.ErrorMessageGeneral);
					OnPostUpdateFailed(new PostFailureEventArgs(this.Post, exception));
				}
			}
		}
		private void btnCancel_Click(object sender, EventArgs e) {
			ResetFields();
			OnPostUpdateDiscarded(EventArgs.Empty);
		}
		#endregion

		#region Event Activators
		protected virtual void OnPostUpdating(PostCancelEventArgs e) {
			PostUpdatingEventHandler eventHandler = base.Events[s_EventPostUpdating] as PostUpdatingEventHandler;
			if(eventHandler != null) {
				eventHandler(this, e);
			}
		}
		protected virtual void OnPostUpdated(PostEventArgs e) {
			PostUpdatedEventHandler eventHandler = base.Events[s_EventPostUpdated] as PostUpdatedEventHandler;
			if(eventHandler != null) {
				eventHandler(this, e);
			}
		}
		protected virtual void OnPostUpdateFailed(PostFailureEventArgs e) {
			PostUpdateFailedEventHandler eventHandler = base.Events[s_EventPostUpdateFailed] as PostUpdateFailedEventHandler;
			if(eventHandler != null) {
				eventHandler(this, e);
			}
		}
		protected virtual void OnPostUpdateDiscarded(EventArgs e) {
			PostUpdateDiscardedEventHandler eventHandler = base.Events[s_EventPostUpdateDiscarded] as PostUpdateDiscardedEventHandler;
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
					lblAuthor.Controls.Add(GetMandatorFieldIndicator());
				}
				if(this.TitleTextBoxVisible) {
					lblTitle.Controls.Add(GetMandatorFieldIndicator());
				}
				//if title text box is not visible, body is always mandatory
				if(this.BodyIsMandatory || !this.TitleTextBoxVisible) {
					lblBody.Controls.Add(GetMandatorFieldIndicator());
				}
			}
			ctrlFieldSetLabel.Visible = this.FieldSetLabelText != null && !this.FieldSetLabelText.Length.Equals(0);
			if(!m_DataBound) {
				this.DataBind();
			}
		}
		public override void DataBind() {
			if(this.Post == null) {
				if(this.PostId == null) {
					throw new InvalidOperationException("The PostId property must be set to a non null value.");
				}
				if((this.Post = this.ForumDao.GetPost(this.PostId, new ThreadedPostsResultConfiguration(1))) == null) {
					throw new InvalidOperationException("The PostId property must be that of an existing post.");
				}
				this.OriginalAuthor = txtAuthor.Text = this.Post.Author;
				this.OriginalTitle = txtTitle.Text = this.Post.Title;
				this.OriginalBody = txtBody.Text = this.Post.Body;
				this.OriginalVisible = chkVisible.Checked = this.Post.Visible;
			}
			m_DataBound = true;
		}
		#endregion

		#region Helper Methods
		private void SetPostProperties() {
			this.Post.Author	= txtAuthor.Text;
			this.Post.Title		= txtTitle.Text;
			this.Post.Body		= txtBody.Text;
			this.Post.Visible	= chkVisible.Checked;
		}
		private void ResetFields() {
			txtAuthor.Text		= this.OriginalAuthor;
			txtTitle.Text		= this.OriginalTitle;
			txtBody.Text		= this.OriginalBody;
			chkVisible.Checked	= this.OriginalVisible;
			ResetErrorFields();
		}
		private void ResetErrorFields() {
			ctrlGeneralError.Visible = ctrlAuthorError.Visible = ctrlTitleError.Visible 
				= ctrlBodyError.Visible = ctrlOtherError.Visible = false;
			lblAuthor.CssClass = lblTitle.CssClass = lblBody.CssClass = string.Empty;
		}
		private Literal GetMandatorFieldIndicator() {
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