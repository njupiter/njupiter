<%@Control Language="c#" AutoEventWireup="false" Codebehind="HierarchicalPostListItem.ascx.cs" Inherits="nJupiter.Services.Forum.UI.Web.HierarchicalPostListItem"%>
<%@Register TagPrefix="nJupiterCtrl" Namespace="nJupiter.Web.UI.Controls" Assembly="nJupiter.Web.UI"%>
<nJupiterCtrl:WebGenericControl ID="ctrlPostDivision" Runat="server">
	<nJupiterCtrl:WebButton ID="btnHeaderAddPost" InnerSpan="true" CssClass="add-post" Runat="server"/>
	<nJupiterCtrl:WebButton ID="btnHeaderUpdatePost" InnerSpan="true" CssClass="update-post" Runat="server"/>
	<nJupiterCtrl:WebButton ID="btnHeaderChangePostVisible" InnerSpan="true" CssClass="change-post-visible"  Runat="server"/>
	<nJupiterCtrl:WebButton ID="btnHeaderDeletePost" InnerSpan="true" CssClass="delete-post" Runat="server"/>
	<nJupiterCtrl:WebGenericControl ID="ctrlHeaderPostInformation" TagName="dl" CssClass="header-post-information" Runat="server">
		<nJupiterCtrl:WebGenericControl ID="ctrlHeaderAuthorLabel" TagName="dt" CssClass="author" Runat="server"/>
		<dd class="author"><nJupiterCtrl:WebAnchor ID="ctrlHeaderAuthorAnchor" Runat="server"/></dd>
		<nJupiterCtrl:WebGenericControl ID="ctrlHeaderTimePostedLabel" TagName="dt" CssClass="date" Runat="server"/>
		<nJupiterCtrl:WebGenericControl ID="ctrlHeaderTimePosted" TagName="dd" CssClass="date" Runat="server"/>
		<nJupiterCtrl:WebPlaceHolder ID="ctrlHeaderDescendantPostsInformation" Runat="server">
			<nJupiterCtrl:WebGenericControl ID="ctrlHeaderPostCountLabel" TagName="dt" Runat="server"/>
			<nJupiterCtrl:WebGenericControl ID="ctrlHeaderPostCount" TagName="dd" Runat="server"/>
			<nJupiterCtrl:WebGenericControl ID="ctrlHeaderTimeLastPostLabel" TagName="dt" Runat="server"/>
			<nJupiterCtrl:WebGenericControl ID="ctrlHeaderTimeLastPost" TagName="dd" Runat="server"/>
		</nJupiterCtrl:WebPlaceHolder>
	</nJupiterCtrl:WebGenericControl>
	<nJupiterCtrl:WebHeading ID="ctrlTitle" Level="3" CssClass="post-title" Runat="server"/>
	<nJupiterCtrl:WebParagraph ID="ctrlBody" CssClass="post-body" Runat="server"/>
	<nJupiterCtrl:WebGenericControl ID="ctrlFooterPostInformation" TagName="dl" CssClass="footer-post-information" Runat="server">
		<nJupiterCtrl:WebGenericControl ID="ctrlFooterAuthorLabel" TagName="dt" CssClass="author" Runat="server"/>
		<dd class="author"><nJupiterCtrl:WebAnchor ID="ctrlFooterAuthorAnchor" Runat="server"/></dd>
		<nJupiterCtrl:WebGenericControl ID="ctrlFooterTimePostedLabel" TagName="dt" CssClass="date" Runat="server"/>
		<nJupiterCtrl:WebGenericControl ID="ctrlFooterTimePosted" TagName="dd" CssClass="date" Runat="server"/>
		<nJupiterCtrl:WebPlaceHolder ID="ctrlFooterDescendantPostsInformation" Runat="server">
			<nJupiterCtrl:WebGenericControl ID="ctrlFooterPostCountLabel" TagName="dt" Runat="server"/>
			<nJupiterCtrl:WebGenericControl ID="ctrlFooterPostCount" TagName="dd" Runat="server"/>
			<nJupiterCtrl:WebGenericControl ID="ctrlFooterTimeLastPostLabel" TagName="dt" Runat="server"/>
			<nJupiterCtrl:WebGenericControl ID="ctrlFooterTimeLastPost" TagName="dd" Runat="server"/>
		</nJupiterCtrl:WebPlaceHolder>
	</nJupiterCtrl:WebGenericControl>
	<nJupiterCtrl:WebButton ID="btnFooterAddPost" InnerSpan="true" CssClass="add-post" Runat="server"/>
	<nJupiterCtrl:WebButton ID="btnFooterUpdatePost" InnerSpan="true" CssClass="update-post" Runat="server"/>
	<nJupiterCtrl:WebButton ID="btnFooterChangePostVisible" InnerSpan="true" CssClass="change-post-visible" Runat="server"/>
	<nJupiterCtrl:WebButton ID="btnFooterDeletePost" InnerSpan="true" CssClass="delete-post" Runat="server"/>
	<nJupiterCtrl:WebGenericControl ID="ctrlDescendantPostsDivision" CssClass="descendant-posts" Runat="server">
		<nJupiterCtrl:WebGenericControl ID="ctrlDescendantsPostsLabel" TagName="strong" Runat="server"/>
		<nJupiterCtrl:WebParagraph ID="ctrlMessageNoDescendantPosts" CssClass="portlet-msg-alert" Runat="server"/>
		<nJupiterCtrl:WebPlaceHolder ID="ctrlDescendantPosts" Runat="server"/>
	</nJupiterCtrl:WebGenericControl>
</nJupiterCtrl:WebGenericControl>