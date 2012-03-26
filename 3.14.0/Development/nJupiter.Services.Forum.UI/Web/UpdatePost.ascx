<%@Control language="c#" Codebehind="UpdatePost.ascx.cs" AutoEventWireup="false" Inherits="nJupiter.Services.Forum.UI.Web.UpdatePost"%>
<%@Register TagPrefix="nJupiter" Namespace="nJupiter.Web.UI.Controls" Assembly="nJupiter.Web.UI"%>
<fieldset>
	<nJupiter:WebGenericControl ID="ctrlFieldSetLabel" TagName="legend" Runat="server"/>
	<nJupiter:WebParagraph ID="ctrlGeneralError" CssClass="portlet-msg-error" Runat="server"/>
	<nJupiter:WebParagraph ID="ctrlAuthorError" CssClass="portlet-msg-error" Runat="server"/>
	<nJupiter:WebParagraph ID="ctrlTitleError" CssClass="portlet-msg-error" Runat="server"/>
	<nJupiter:WebParagraph ID="ctrlBodyError" CssClass="portlet-msg-error" Runat="server"/>
	<nJupiter:WebPlaceHolder ID="ctrlAuthorErrorAltPosition" Runat="server"/>
	<nJupiter:WebParagraph ID="ctrlOtherError" CssClass="portlet-msg-error" Runat="server"/>
	<nJupiter:WebParagraph ID="ctrlMandatoryFieldsLabel" Runat="server"/>
	<nJupiter:WebParagraph ID="ctrlAuthor" Runat="server">
		<nJupiter:WebLabel ID="lblAuthor" For="txtAuthor" Runat="server"/>
		<asp:TextBox ID="txtAuthor" Runat="server"/>
	</nJupiter:WebParagraph>
	<nJupiter:WebParagraph ID="ctrlTitle" Runat="server">
		<nJupiter:WebLabel ID="lblTitle" For="txtTitle" Runat="server"/>
		<asp:TextBox ID="txtTitle" Runat="server"/>
	</nJupiter:WebParagraph>
	<p>
		<nJupiter:WebLabel ID="lblBody" For="txtBody" Runat="server"/>
		<asp:TextBox ID="txtBody" TextMode="MultiLine" Runat="server"/>
	</p>
	<nJupiter:WebPlaceHolder ID="ctrlAuthorAltPosition" Runat="server"/>	
	<nJupiter:WebParagraph ID="ctrlVisible" Runat="server">
		<nJupiter:WebCheckBox ID="chkVisible" Runat="server"/>
	</nJupiter:WebParagraph>
	<p>
		<nJupiter:WebButton ID="btnSubmit" InnerSpan="true" CssClass="submit" Runat="server"/>
		<nJupiter:WebButton ID="btnCancel" InnerSpan="true" CssClass="cancel"  Runat="server"/>
		<nJupiter:WebPlaceHolder ID="ctrlSubmitAltPosition" Runat="server"/>
	</p>
</fieldset>