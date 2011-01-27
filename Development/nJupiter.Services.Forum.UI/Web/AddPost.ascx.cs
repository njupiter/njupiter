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

	public class AddPost : UserControl {
		#region Constants
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
		private const bool DefaultPostvisible = true;
		private const bool DefaultUsetitlefromreplysource = false;
		private const bool DefaultUsebodyfromreplysource = false;
		private const string DefaultReplyindicator = "Re: ";

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
		private const string DefaultErrormessagegeneral = DebugPrefix + "The form was not submitted because of the following error(s). If possible, correct them and submit again.";
		private const string DefaultErrormessageauthornotspecified = DebugPrefix + "You have to specify an author.";
		private const string DefaultErrormessagetitlenotspecified = DebugPrefix + "You have to specify a title.";
		private const string DefaultErrormessagebodynotspecified = DebugPrefix + "You have to specify a text.";
		private const string DefaultErrormessagebodytoolong = DebugPrefix + "You have to specify a shorter text.";
		private const string DefaultErrormessagecategorynotfound = DebugPrefix + "The category to add this post in does not exist.";
		private const string DefaultErrormessagepostnotfound = DebugPrefix + "The post to add this post in does not exist.";

		private const string ViewstateDomain = "v_Domain";
		private const string ViewstateCategoryname = "v_CategoryName";
		private const string ViewstateCategoryid = "v_CategoryId";
		private const string ViewstatePostid = "v_PostId";
		private const string ViewstateUseridentity = "v_UserIdentity";
		private const string ViewstateUsedpostid = "v_UsedPostId";

		private const string CssclassErrormessage = "portlet-msg-error";
		#endregion

		#region Variables
		private ForumDao forumDao;
		private bool dataBound;
		private PostId replySourcePostId;
		private PostQuoter postQuoter;
		private bool titleTextBoxVisible = DefaultTitletextboxvisible;
		private bool useTitleFromReplySource = DefaultUsetitlefromreplysource;
		private bool useBodyFromReplySource = DefaultUsebodyfromreplysource;
		private string replyIndicator = DefaultReplyindicator;
		private bool mandatoryFieldsLabelVisible = DefaultMandatoryfieldslabelvisible;
		private bool authorTextBoxVisible = DefaultAuthortextboxvisible;
		private bool authorTextBoxBeforeTitleAndBodyTextBoxes = DefaultAuthortextboxbeforetitleandbodytextboxes;
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
		private string errorMessageCategoryNotFound = DefaultErrormessagecategorynotfound;
		private string errorMessagePostNotFound = DefaultErrormessagepostnotfound;

		private static readonly object EventPostAdding = new object();
		private static readonly object EventPostAdded = new object();
		private static readonly object EventPostAdditionFailed = new object();
		private static readonly object EventPostAdditionDiscarded = new object();
		#endregion

		#region Events
		public event PostAddingEventHandler PostAdding {
			add { base.Events.AddHandler(EventPostAdding, value); }
			remove { base.Events.RemoveHandler(EventPostAdding, value); }
		}
		public event PostAddedEventHandler PostAdded {
			add { base.Events.AddHandler(EventPostAdded, value); }
			remove { base.Events.RemoveHandler(EventPostAdded, value); }
		}
		public event PostAdditionFailedEventHandler PostAdditionFailed {
			add { base.Events.AddHandler(EventPostAdditionFailed, value); }
			remove { base.Events.RemoveHandler(EventPostAdditionFailed, value); }
		}
		public event PostAdditionDiscardedEventHandler PostAdditionDiscarded {
			add { base.Events.AddHandler(EventPostAdditionDiscarded, value); }
			remove { base.Events.RemoveHandler(EventPostAdditionDiscarded, value); }
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
		protected WebButton btnCancel;
		protected WebPlaceHolder ctrlSubmitAltPosition;
		#endregion

		#region Properties
		public ForumDao ForumDao { get { return this.forumDao ?? (this.forumDao = ForumDao.GetInstance()); } set { this.forumDao = value; } }

		public string Domain {
			get { return (string)this.ViewState[ViewstateDomain]; }
			set {
				if(value != null) {
					this.ViewState.Remove(ViewstatePostid);
					this.ViewState.Remove(ViewstateCategoryid);
				}
				this.ViewState.Add(ViewstateDomain, value);
			}
		}
		public string CategoryName {
			get { return (string)this.ViewState[ViewstateCategoryname]; }
			set {
				if(value != null) {
					this.ViewState.Remove(ViewstatePostid);
					this.ViewState.Remove(ViewstateCategoryid);
				}
				this.ViewState.Add(ViewstateCategoryname, value);
			}
		}
		public CategoryId CategoryId {
			get { return (CategoryId)this.ViewState[ViewstateCategoryid]; }
			set {
				if(value != null) {
					this.ViewState.Remove(ViewstatePostid);
					this.ViewState.Remove(ViewstateDomain);
					this.ViewState.Remove(ViewstateCategoryname);
				}
				this.ViewState.Add(ViewstateCategoryid, value);
			}
		}
		public PostId PostId {
			get { return (PostId)this.ViewState[ViewstatePostid]; }
			set {
				if(value != null) {
					this.ViewState.Remove(ViewstateCategoryid);
					this.ViewState.Remove(ViewstateDomain);
					this.ViewState.Remove(ViewstateCategoryname);
				}
				this.ViewState.Add(ViewstatePostid, value);
			}
		}
		public string UserIdentity { get { return (string)this.ViewState[ViewstateUseridentity]; } set { this.ViewState.Add(ViewstateUseridentity, value); } }
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

		public bool UseTitleFromReplySource { get { return this.useTitleFromReplySource; } set { this.useTitleFromReplySource = value; } }
		public bool UseBodyFromReplySource { get { return this.useBodyFromReplySource; } set { this.useBodyFromReplySource = value; } }
		public PostId ReplySourcePostId { get { return this.replySourcePostId; } set { this.replySourcePostId = value; } }
		public string ReplyIndicator { get { return this.replyIndicator; } set { this.replyIndicator = value; } }
		public PostQuoter PostQuoter { get { return this.postQuoter; } set { this.postQuoter = value; } }

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
		public string ErrorMessageCategoryNotFound { get { return this.errorMessageCategoryNotFound; } set { this.errorMessageCategoryNotFound = value; } }
		public string ErrorMessagePostNotFound { get { return this.errorMessagePostNotFound; } set { this.errorMessagePostNotFound = value; } }

		private PostId UsedPostId { get { return (PostId)this.ViewState[ViewstateUsedpostid]; } set { this.ViewState.Add(ViewstateUsedpostid, value); } }
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

			post.Author = this.Author;
			post.UserIdentity = this.UserIdentity;
			post.Title = this.Title;
			post.Body = this.Body;
			post.Visible = this.PostVisible;
			PostCancelEventArgs cancelEventArgs = new PostCancelEventArgs(post);
			OnPostAdding(cancelEventArgs);
			if(!cancelEventArgs.Cancel) {
				bool isValid = true;
				Exception exception = null;
				if(this.AuthorTextBoxVisible && (this.Author == null || this.Author.TrimEnd().Length.Equals(0))) {
					SetErrorField(lblAuthor, ctrlAuthorError, this.ErrorMessageAuthorNotSpecified);
					isValid = false;
				}
				if(this.TitleTextBoxVisible && (this.Title == null || this.Title.TrimEnd().Length.Equals(0))) {
					SetErrorField(lblTitle, ctrlTitleError, this.ErrorMessageTitleNotSpecified);
					isValid = false;
				}
				//if title text box is not visible, body is always mandatory
				if((this.BodyIsMandatory || !this.TitleTextBoxVisible) && (this.Body == null || this.Body.TrimEnd().Length.Equals(0))) {
					SetErrorField(lblBody, ctrlBodyError, this.ErrorMessageBodyNotSpecified);
					isValid = false;
				} else if(this.Body != null && this.Body.Length > this.MaximumBodyLength) {
					SetErrorField(lblBody, ctrlBodyError, this.ErrorMessageBodyTooLong);
					isValid = false;
				}
				if(isValid) {
					try {
						this.ForumDao.SavePost(post);
						OnPostAdded(new PostEventArgs(post));
						ResetFields();
					} catch(Exception ex) {
						string otherError;
						if(ex is CategoryNotFoundException) {
							otherError = this.ErrorMessageCategoryNotFound;
						} else if(ex is PostNotFoundException) {
							otherError = this.ErrorMessagePostNotFound;
						} else {
							otherError = ex.Message;
						}
						SetErrorField(null, ctrlOtherError, otherError);
						exception = ex;
						isValid = false;
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
			PostAddingEventHandler eventHandler = base.Events[EventPostAdding] as PostAddingEventHandler;
			if(eventHandler != null) {
				eventHandler(this, e);
			}
		}
		protected virtual void OnPostAdded(PostEventArgs e) {
			PostAddedEventHandler eventHandler = base.Events[EventPostAdded] as PostAddedEventHandler;
			if(eventHandler != null) {
				eventHandler(this, e);
			}
		}
		protected virtual void OnPostAdditionFailed(PostFailureEventArgs e) {
			PostAdditionFailedEventHandler eventHandler = base.Events[EventPostAdditionFailed] as PostAdditionFailedEventHandler;
			if(eventHandler != null) {
				eventHandler(this, e);
			}
		}
		protected virtual void OnPostAdditionDiscarded(EventArgs e) {
			PostAdditionDiscardedEventHandler eventHandler = base.Events[EventPostAdditionDiscarded] as PostAdditionDiscardedEventHandler;
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

			this.PostVisible = DefaultPostvisible;

			btnSubmit.Click += this.btnSubmit_Click;
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
			if(!this.dataBound) {
				this.DataBind();
			}
		}
		public override void DataBind() {
			Post post;
			bool useTitle = this.UseTitleFromReplySource && (this.Title == null || this.Title.TrimEnd().Length.Equals(0));
			bool useBody = this.UseBodyFromReplySource && (this.Body == null || this.Body.TrimEnd().Length.Equals(0));
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
			this.dataBound = true;
		}
		#endregion

		#region Helper Methods
		private void ResetFields() {
			this.Author = this.Title = this.Body = null;
			this.PostVisible = DefaultPostvisible;
			this.UsedPostId = null;
			ResetErrorFields();
		}
		private void ResetErrorFields() {
			ctrlGeneralError.Visible = ctrlAuthorError.Visible = ctrlTitleError.Visible =
				ctrlBodyError.Visible = ctrlOtherError.Visible = false;
			lblAuthor.CssClass = lblTitle.CssClass = lblBody.CssClass = string.Empty;
		}
		private Literal GetMandatoryFieldIndicator() {
			Literal mandatoryFieldIndicator = new Literal();
			mandatoryFieldIndicator.Text = this.MandatoryFieldIndicator;
			return mandatoryFieldIndicator;
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