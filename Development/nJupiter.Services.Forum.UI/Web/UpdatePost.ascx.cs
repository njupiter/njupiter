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

using nJupiter.Web.UI.Controls;

namespace nJupiter.Services.Forum.UI.Web {

	public class UpdatePost : UserControl {
		#region Constants
		private const bool DefaultIgnoreconcurrentupdate = false;

		private const int TextboxMaximumlength = 255;

		private const bool DefaultMandatoryfieldslabelvisible = true;
		private const bool DefaultTitletextboxvisible = true;
		private const bool DefaultAuthortextboxvisible = true;
		private const bool DefaultAuthortextboxbeforetitleandbodytextboxes = true;
		private const bool DefaultVisiblecheckboxvisible = true;
		private const bool DefaultCancelbuttonvisible = true;
		private const bool DefaultSubmitbuttonbeforecancelbutton = true;
		private const bool DefaultBodyismandatory = true;
		private const int DefaultMaximumauthorlength = TextboxMaximumlength;
		private const int DefaultMaximumtitlelength = TextboxMaximumlength;
		private const int DefaultMaximumbodylength = int.MaxValue;

#if DEBUG
		private const string	DebugPrefix										= "_";
#else
		private const string DebugPrefix = "";
#endif

		private const string DefaultMandatoryfieldindicator = " *";

		private const string DefaultFieldsetlabeltext = DebugPrefix + "Input fields";
		private const string DefaultMandatoryfieldslabeltext = DebugPrefix + "* indicates mandatory field";
		private const string DefaultAuthortextboxlabeltext = DebugPrefix + "Author";
		private const string DefaultTitletextboxlabeltext = DebugPrefix + "Title";
		private const string DefaultBodytextboxlabeltext = DebugPrefix + "Text";
		private const string DefaultVisiblecheckboxtext = DebugPrefix + "Visible";
		private const string DefaultSubmitbuttontext = DebugPrefix + "Submit";
		private const string DefaultCancelbuttontext = DebugPrefix + "Cancel";
		private const string DefaultErrormessagegeneral = DebugPrefix + "The form was not submitted because of the following error(s). If possible, correct them, and submit again.";
		private const string DefaultErrormessageauthornotspecified = DebugPrefix + "You have to specify an author.";
		private const string DefaultErrormessagetitlenotspecified = DebugPrefix + "You have to specify a title.";
		private const string DefaultErrormessagebodynotspecified = DebugPrefix + "You have to specify a text.";
		private const string DefaultErrormessagebodytoolong = DebugPrefix + "You have to specify a shorter text.";
		private const string DefaultErrormessagepostconcurrentlyupdated = DebugPrefix + "This post has been concurrently updated and this update can not proceed. Reopen the form and try again.";
		private const string DefaultErrormessagepostconcurrentlydeleted = DebugPrefix + "This post has been concurrently deleted.";

		private const string ViewstatePostid = "v_PostId";
		private const string ViewstatePost = "v_Post";
		private const string ViewstateOriginalauthor = "v_OriginalAuthor";
		private const string ViewstateOriginaltitle = "v_OriginalTitle";
		private const string ViewstateOriginalbody = "v_OriginalBody";
		private const string ViewstateOriginalvisible = "v_OriginalVisible";

		private const string CssclassErrormessage = "portlet-msg-error";
		#endregion

		#region Variables
		private ForumDao forumDao;
		private bool dataBound;
		private bool ignoreConcurrentUpdate = DefaultIgnoreconcurrentupdate;

		private bool mandatoryFieldsLabelVisible = DefaultMandatoryfieldslabelvisible;
		private bool authorTextBoxVisible = DefaultAuthortextboxvisible;
		private bool authorTextBoxBeforeTitleAndBodyTextBoxes = DefaultAuthortextboxbeforetitleandbodytextboxes;
		private bool titleTextBoxVisible = DefaultTitletextboxvisible;
		private bool visibleCheckBoxVisible = DefaultVisiblecheckboxvisible;
		private bool cancelButtonVisible = DefaultCancelbuttonvisible;
		private bool submitButtonBeforeCancelButton = DefaultSubmitbuttonbeforecancelbutton;
		private bool bodyIsMandatory = DefaultBodyismandatory;
		private int maximumBodyLength = DefaultMaximumbodylength;
		private string mandatoryFieldIndicator = DefaultMandatoryfieldindicator;
		private string errorMessageGeneral = DefaultErrormessagegeneral;
		private string errorMessageAuthorNotSpecified = DefaultErrormessageauthornotspecified;
		private string errorMessageTitleNotSpecified = DefaultErrormessagetitlenotspecified;
		private string errorMessageBodyNotSpecified = DefaultErrormessagebodynotspecified;
		private string errorMessageBodyTooLong = DefaultErrormessagebodytoolong;
		private string errorMessagePostConcurrentlyUpdated = DefaultErrormessagepostconcurrentlyupdated;
		private string errorMessagePostConcurrentlyDeleted = DefaultErrormessagepostconcurrentlydeleted;

		private static readonly object EventPostUpdating = new object();
		private static readonly object EventPostUpdated = new object();
		private static readonly object EventPostUpdateFailed = new object();
		private static readonly object EventPostUpdateDiscarded = new object();
		#endregion

		#region Events
		public event PostUpdatingEventHandler PostUpdating {
			add { base.Events.AddHandler(EventPostUpdating, value); }
			remove { base.Events.RemoveHandler(EventPostUpdating, value); }
		}
		public event PostUpdatedEventHandler PostUpdated {
			add { base.Events.AddHandler(EventPostUpdated, value); }
			remove { base.Events.RemoveHandler(EventPostUpdated, value); }
		}
		public event PostUpdateFailedEventHandler PostUpdateFailed {
			add { base.Events.AddHandler(EventPostUpdateFailed, value); }
			remove { base.Events.RemoveHandler(EventPostUpdateFailed, value); }
		}
		public event PostUpdateDiscardedEventHandler PostUpdateDiscarded {
			add { base.Events.AddHandler(EventPostUpdateDiscarded, value); }
			remove { base.Events.RemoveHandler(EventPostUpdateDiscarded, value); }
		}
		#endregion

		#region UI Members
		protected WebParagraph ctrlGeneralError;
		protected WebParagraph ctrlAuthorError;
		protected WebPlaceHolder ctrlAuthorErrorAltPosition;
		protected WebParagraph ctrlTitleError;
		protected WebParagraph ctrlBodyError;
		protected WebParagraph ctrlOtherError;
		protected WebGenericControl ctrlFieldSetLabel;
		protected WebParagraph ctrlMandatoryFieldsLabel;
		protected WebParagraph ctrlAuthor;
		protected WebLabel lblAuthor;
		protected TextBox txtAuthor;
		protected WebPlaceHolder ctrlAuthorAltPosition;
		protected WebParagraph ctrlTitle;
		protected WebLabel lblTitle;
		protected TextBox txtTitle;
		protected WebLabel lblBody;
		protected TextBox txtBody;
		protected WebParagraph ctrlVisible;
		protected WebCheckBox chkVisible;
		protected WebButton btnSubmit;
		protected WebPlaceHolder ctrlSubmitAltPosition;
		protected WebButton btnCancel;
		#endregion

		#region Properties
		public ForumDao ForumDao { get { return this.forumDao ?? (this.forumDao = ForumDao.GetInstance()); } set { this.forumDao = value; } }
		public bool IgnoreConcurrentUpdate { get { return this.ignoreConcurrentUpdate; } set { this.ignoreConcurrentUpdate = value; } }
		public PostId PostId {
			get { return (PostId)this.ViewState[ViewstatePostid]; }
			set {
				this.ViewState.Add(ViewstatePostid, value);
				this.Post = null;
			}
		}

		public bool MandatoryFieldsLabelVisible { get { return this.mandatoryFieldsLabelVisible; } set { this.mandatoryFieldsLabelVisible = value; } }
		public bool AuthorTextBoxVisible { get { return this.authorTextBoxVisible; } set { this.authorTextBoxVisible = value; } }
		public bool AuthorTextBoxBeforeTitleAndBodyTextBoxes { get { return this.authorTextBoxBeforeTitleAndBodyTextBoxes; } set { this.authorTextBoxBeforeTitleAndBodyTextBoxes = value; } }
		public bool TitleTextBoxVisible { get { return this.titleTextBoxVisible; } set { this.titleTextBoxVisible = value; } }
		public bool VisibleCheckBoxVisible { get { return this.visibleCheckBoxVisible; } set { this.visibleCheckBoxVisible = value; } }
		public bool CancelButtonVisible { get { return this.cancelButtonVisible; } set { this.cancelButtonVisible = value; } }
		public bool SubmitButtonBeforeCancelButton { get { return this.submitButtonBeforeCancelButton; } set { this.submitButtonBeforeCancelButton = value; } }
		public int MaximumAuthorLength {
			get { return txtAuthor.MaxLength; }
			set {
				if(value > TextboxMaximumlength || value < 1) {
					throw new ArgumentOutOfRangeException("value");
				}
				txtAuthor.MaxLength = value;
			}
		}
		public int MaximumTitleLength {
			get { return txtTitle.MaxLength; }
			set {
				if(value > TextboxMaximumlength || value < 1) {
					throw new ArgumentOutOfRangeException("value");
				}
				txtTitle.MaxLength = value;
			}
		}
		public int MaximumBodyLength {
			get { return this.maximumBodyLength; }
			set {
				if(value < 1) {
					throw new ArgumentOutOfRangeException("value");
				}
				this.maximumBodyLength = value;
			}
		}
		public bool BodyIsMandatory { get { return this.bodyIsMandatory; } set { this.bodyIsMandatory = value; } }
		public string MandatoryFieldIndicator { get { return this.mandatoryFieldIndicator; } set { this.mandatoryFieldIndicator = value; } }

		public string FieldSetLabelText { get { return ctrlFieldSetLabel.InnerText; } set { ctrlFieldSetLabel.InnerText = value; } }
		public string MandatoryFieldsLabelText { get { return ctrlMandatoryFieldsLabel.InnerText; } set { ctrlMandatoryFieldsLabel.InnerText = value; } }
		public string AuthorTextBoxLabelText { get { return lblAuthor.InnerText; } set { lblAuthor.InnerText = value; } }
		public string TitleTextBoxLabelText { get { return lblTitle.InnerText; } set { lblTitle.InnerText = value; } }
		public string BodyTextBoxLabelText { get { return lblBody.InnerText; } set { lblBody.InnerText = value; } }
		public string VisibleCheckBoxText { get { return chkVisible.Text; } set { chkVisible.Text = value; } }
		public string SubmitButtonText { get { return btnSubmit.InnerText; } set { btnSubmit.InnerText = value; } }
		public string CancelButtonText { get { return btnCancel.InnerText; } set { btnCancel.InnerText = value; } }
		public string ErrorMessageGeneral { get { return this.errorMessageGeneral; } set { this.errorMessageGeneral = value; } }
		public string ErrorMessageAuthorNotSpecified { get { return this.errorMessageAuthorNotSpecified; } set { this.errorMessageAuthorNotSpecified = value; } }
		public string ErrorMessageTitleNotSpecified { get { return this.errorMessageTitleNotSpecified; } set { this.errorMessageTitleNotSpecified = value; } }
		public string ErrorMessageBodyNotSpecified { get { return this.errorMessageBodyNotSpecified; } set { this.errorMessageBodyNotSpecified = value; } }
		public string ErrorMessageBodyTooLong { get { return this.errorMessageBodyTooLong; } set { this.errorMessageBodyTooLong = value; } }
		public string ErrorMessagePostConcurrentlyUpdated { get { return this.errorMessagePostConcurrentlyUpdated; } set { this.errorMessagePostConcurrentlyUpdated = value; } }
		public string ErrorMessagePostConcurrentlyDeleted { get { return this.errorMessagePostConcurrentlyDeleted; } set { this.errorMessagePostConcurrentlyDeleted = value; } }

		private Post Post { get { return (Post)this.ViewState[ViewstatePost]; } set { this.ViewState.Add(ViewstatePost, value); } }
		private string OriginalAuthor { get { return (string)this.ViewState[ViewstateOriginalauthor]; } set { this.ViewState.Add(ViewstateOriginalauthor, value); } }
		private string OriginalTitle { get { return (string)this.ViewState[ViewstateOriginaltitle]; } set { this.ViewState.Add(ViewstateOriginaltitle, value); } }
		private string OriginalBody { get { return (string)this.ViewState[ViewstateOriginalbody]; } set { this.ViewState.Add(ViewstateOriginalbody, value); } }
		private bool OriginalVisible { get { return (bool)this.ViewState[ViewstateOriginalvisible]; } set { this.ViewState.Add(ViewstateOriginalvisible, value); } }
		#endregion

		#region Event Handlers
		private void SubmitClick(object sender, EventArgs e) {
			ResetErrorFields();
			SetPostProperties();
			PostCancelEventArgs cancelEventArgs = new PostCancelEventArgs(this.Post);
			OnPostUpdating(cancelEventArgs);
			if(!cancelEventArgs.Cancel) {
				bool isValid = true;
				Exception exception = null;
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
							this.OriginalAuthor = this.Post.Author;
							this.OriginalTitle = this.Post.Title;
							this.OriginalBody = this.Post.Body;
							this.OriginalVisible = this.Post.Visible;
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
			PostUpdatingEventHandler eventHandler = base.Events[EventPostUpdating] as PostUpdatingEventHandler;
			if(eventHandler != null) {
				eventHandler(this, e);
			}
		}
		protected virtual void OnPostUpdated(PostEventArgs e) {
			PostUpdatedEventHandler eventHandler = base.Events[EventPostUpdated] as PostUpdatedEventHandler;
			if(eventHandler != null) {
				eventHandler(this, e);
			}
		}
		protected virtual void OnPostUpdateFailed(PostFailureEventArgs e) {
			PostUpdateFailedEventHandler eventHandler = base.Events[EventPostUpdateFailed] as PostUpdateFailedEventHandler;
			if(eventHandler != null) {
				eventHandler(this, e);
			}
		}
		protected virtual void OnPostUpdateDiscarded(EventArgs e) {
			PostUpdateDiscardedEventHandler eventHandler = base.Events[EventPostUpdateDiscarded] as PostUpdateDiscardedEventHandler;
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
			this.MaximumAuthorLength = DefaultMaximumauthorlength;
			this.MaximumTitleLength = DefaultMaximumtitlelength;
			this.MaximumBodyLength = DefaultMaximumbodylength;
			this.FieldSetLabelText = DefaultFieldsetlabeltext;
			this.MandatoryFieldsLabelText = DefaultMandatoryfieldslabeltext;
			this.AuthorTextBoxLabelText = DefaultAuthortextboxlabeltext;
			this.TitleTextBoxLabelText = DefaultTitletextboxlabeltext;
			this.BodyTextBoxLabelText = DefaultBodytextboxlabeltext;
			this.VisibleCheckBoxText = DefaultVisiblecheckboxtext;
			this.SubmitButtonText = DefaultSubmitbuttontext;
			this.CancelButtonText = DefaultCancelbuttontext;

			btnSubmit.Click += this.SubmitClick;
			btnCancel.Click += this.btnCancel_Click;

			base.OnInit(e);
		}
		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender(e);
			ctrlMandatoryFieldsLabel.Visible = this.MandatoryFieldsLabelVisible;
			ctrlAuthor.Visible = this.AuthorTextBoxVisible;
			ctrlTitle.Visible = this.TitleTextBoxVisible;
			ctrlVisible.Visible = this.VisibleCheckBoxVisible;
			btnCancel.Visible = this.CancelButtonVisible;
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
			if(!this.dataBound) {
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
			this.dataBound = true;
		}
		#endregion

		#region Helper Methods
		private void SetPostProperties() {
			this.Post.Author = txtAuthor.Text;
			this.Post.Title = txtTitle.Text;
			this.Post.Body = txtBody.Text;
			this.Post.Visible = chkVisible.Checked;
		}
		private void ResetFields() {
			txtAuthor.Text = this.OriginalAuthor;
			txtTitle.Text = this.OriginalTitle;
			txtBody.Text = this.OriginalBody;
			chkVisible.Checked = this.OriginalVisible;
			ResetErrorFields();
		}
		private void ResetErrorFields() {
			ctrlGeneralError.Visible = ctrlAuthorError.Visible = ctrlTitleError.Visible
				= ctrlBodyError.Visible = ctrlOtherError.Visible = false;
			lblAuthor.CssClass = lblTitle.CssClass = lblBody.CssClass = string.Empty;
		}
		private Literal GetMandatorFieldIndicator() {
			Literal fieldIndicator = new Literal();
			fieldIndicator.Text = this.MandatoryFieldIndicator;
			return fieldIndicator;
		}
		private static void SetErrorField(WebLabel label, WebParagraph paragraph, string error) {
			if(label != null) {
				label.CssClass = CssclassErrormessage;
			}
			paragraph.InnerText = error;
			paragraph.Visible = true;
		}
		#endregion
	}

}