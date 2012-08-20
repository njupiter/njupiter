<%@Control Language="c#" AutoEventWireup="false" Codebehind="FlatTabularPostList.ascx.cs" Inherits="nJupiter.Services.Forum.UI.Web.FlatTabularPostList"%>
<%@Register TagPrefix="nJupiterCtrl" Namespace="nJupiter.Web.UI.Controls" Assembly="nJupiter.Web.UI"%>
<%@Register TagPrefix="nJupiterListCtrl" Namespace="nJupiter.Web.UI.Controls.Listings" Assembly="nJupiter.Web.UI"%>
<%@Register TagPrefix="nJupiter" TagName="FlatTabularPostListItem" Src="FlatTabularPostListItem.ascx"%>
<nJupiterListCtrl:PagedListing ID="ctrlPagedListing" Runat="server">
<HeaderTemplate>
<nJupiterCtrl:WebGenericControl ID="ctrlHeaderFieldSet" TagName="fieldset" CssClass="header-controls" Runat="server">
	<nJupiterCtrl:WebGenericControl ID="ctrlHeaderFieldSetLabel" TagName="legend" Runat="server"/>
	<nJupiterCtrl:WebParagraph ID="ctrlHeaderSearch" CssClass="search" Runat="server">
		<nJupiterCtrl:WebLabel ID="lblHeaderSearch" For="txtHeaderSearch" Runat="server"/>
		<asp:TextBox ID="txtHeaderSearch" Runat="server"/>
	</nJupiterCtrl:WebParagraph>
	<nJupiterCtrl:WebParagraph ID="ctrlHeaderNumberOfItems" CssClass="number-of-items" Runat="server">
		<nJupiterCtrl:WebLabel ID="lblHeaderNumberOfItemsSelector" For="ctrlHeaderNumberOfItemsSelector" Runat="server"/>
		<nJupiterListCtrl:NumberOfItemsSelector ID="ctrlHeaderNumberOfItemsSelector" Runat="server"/>
	</nJupiterCtrl:WebParagraph>
	<nJupiterCtrl:WebPlaceHolder ID="ctrlHeaderNoScript" Runat="server">
		<p class="submit"><nJupiterCtrl:WebButton ID="btnHeaderSubmit" InnerSpan="true" Runat="server"/></p>
	</nJupiterCtrl:WebPlaceHolder>
	<nJupiterCtrl:WebParagraph ID="ctrlHeaderAddPost" CssClass="add-post" Runat="server">
		<nJupiterCtrl:WebButton ID="btnHeaderAddPost" InnerSpan="true" Runat="server"/>
	</nJupiterCtrl:WebParagraph>
</nJupiterCtrl:WebGenericControl>
<nJupiterCtrl:WebPlaceHolder ID="ctrlTableTop" Runat="server">
	<nJupiterCtrl:Paging ID="ctrlHeaderPaging" Runat="server"/>
	<table cellspacing="0" cellpadding="2">
		<nJupiterCtrl:WebPlaceHolder ID="cptTableCaption" Runat="server"/>
		<nJupiterCtrl:WebGenericControl ID="ctrlHeaderColumnHeaders" TagName="thead" Runat="server">
			<tr>
				<nJupiterCtrl:WebGenericControl ID="ctrlHeaderTitle" TagName="th" scope="col" CssClass="header1" Runat="server">
					<nJupiterCtrl:WebLinkButton ID="lbtnTitle" Runat="server"/>
				</nJupiterCtrl:WebGenericControl>
				<nJupiterCtrl:WebGenericControl ID="ctrlHeaderBody" TagName="th" scope="col" CssClass="header2" Runat="server">
					<nJupiterCtrl:WebLinkButton ID="lbtnBody" Runat="server"/>
				</nJupiterCtrl:WebGenericControl>
				<nJupiterCtrl:WebGenericControl ID="ctrlHeaderRootPostTitle" TagName="th" scope="col" CssClass="header3" Runat="server">
					<nJupiterCtrl:WebLinkButton ID="lbtnRootPostTitle" Runat="server"/>
				</nJupiterCtrl:WebGenericControl>
				<nJupiterCtrl:WebGenericControl ID="ctrlHeaderCategory" TagName="th" scope="col" CssClass="header4" Runat="server">
					<nJupiterCtrl:WebLinkButton ID="lbtnCategory" Runat="server"/>
				</nJupiterCtrl:WebGenericControl>
				<nJupiterCtrl:WebGenericControl ID="ctrlHeaderTimePosted" TagName="th" scope="col" CssClass="header5" Runat="server">
					<nJupiterCtrl:WebLinkButton ID="lbtnTimePosted" Runat="server"/>
				</nJupiterCtrl:WebGenericControl>
				<nJupiterCtrl:WebGenericControl ID="ctrlHeaderAuthor" TagName="th" scope="col" CssClass="header6" Runat="server">
					<nJupiterCtrl:WebLinkButton ID="lbtnAuthor" Runat="server"/>
				</nJupiterCtrl:WebGenericControl>
				<nJupiterCtrl:WebGenericControl ID="ctrlHeaderPostCount" TagName="th" scope="col" CssClass="header7" Runat="server">
					<nJupiterCtrl:WebLinkButton ID="lbtnPostCount" Runat="server"/>
				</nJupiterCtrl:WebGenericControl>
				<nJupiterCtrl:WebGenericControl ID="ctrlHeaderTimeLastPost" TagName="th" scope="col" CssClass="header8" Runat="server">
					<nJupiterCtrl:WebLinkButton ID="lbtnTimeLastPost" Runat="server"/>
				</nJupiterCtrl:WebGenericControl>
				<nJupiterCtrl:WebGenericControl ID="ctrlHeaderSearchRelevance" TagName="th" scope="col" CssClass="header9" Runat="server">
					<nJupiterCtrl:WebLinkButton ID="lbtnSearchRelevance" Runat="server"/>
				</nJupiterCtrl:WebGenericControl>
				<nJupiterCtrl:WebGenericControl ID="ctrlHeaderVisible" TagName="th" scope="col" CssClass="header10" Runat="server">
					<nJupiterCtrl:WebLinkButton ID="lbtnVisible" Runat="server"/>
				</nJupiterCtrl:WebGenericControl>
				<nJupiterCtrl:WebGenericControl ID="ctrlHeaderEffectivelyVisible" TagName="th" scope="col" CssClass="header11" Runat="server">
					<nJupiterCtrl:WebLinkButton ID="lbtnEffectivelyVisible" Runat="server"/>
				</nJupiterCtrl:WebGenericControl>
			</tr>
		</nJupiterCtrl:WebGenericControl>
		<tbody>
</nJupiterCtrl:WebPlaceHolder>
</HeaderTemplate>
<ItemTemplate><nJupiter:FlatTabularPostListItem Runat="server"/></ItemTemplate>
<FooterTemplate>
<nJupiterCtrl:WebPlaceHolder ID="ctrlTableBottom" Runat="server">
		</tbody>	
	</table>
	<nJupiterCtrl:Paging ID="ctrlFooterPaging" Runat="server"/>
</nJupiterCtrl:WebPlaceHolder>
<nJupiterCtrl:WebParagraph ID="ctrlMessageNoPosts" CssClass="portlet-msg-alert" Runat="server"/>
<nJupiterCtrl:WebGenericControl ID="ctrlFooterFieldSet" TagName="fieldset" CssClass="footer-controls" Runat="server">
	<nJupiterCtrl:WebGenericControl ID="ctrlFooterFieldSetLabel" TagName="legend" Runat="server"/>
	<nJupiterCtrl:WebParagraph ID="ctrlFooterSearch" CssClass="search" Runat="server">
		<nJupiterCtrl:WebLabel ID="lblFooterSearch" For="txtFooterSearch" Runat="server"/>
		<asp:TextBox ID="txtFooterSearch" Runat="server"/>
	</nJupiterCtrl:WebParagraph>
	<nJupiterCtrl:WebPlaceHolder ID="ctrlFooterNoScript" Runat="server">
		<p class="submit"><nJupiterCtrl:WebButton ID="btnFooterSubmit" InnerSpan="true" Runat="server"/></p>
	</nJupiterCtrl:WebPlaceHolder>
	<nJupiterCtrl:WebParagraph ID="ctrlFooterAddPost" CssClass="add-post" Runat="server">
		<nJupiterCtrl:WebButton ID="btnFooterAddPost" InnerSpan="true" Runat="server"/>
	</nJupiterCtrl:WebParagraph>
</nJupiterCtrl:WebGenericControl>
</FooterTemplate>
</nJupiterListCtrl:PagedListing>