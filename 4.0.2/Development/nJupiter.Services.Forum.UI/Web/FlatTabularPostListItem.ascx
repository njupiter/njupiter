<%@Control Language="c#" AutoEventWireup="false" Codebehind="FlatTabularPostListItem.ascx.cs" Inherits="nJupiter.Services.Forum.UI.Web.FlatTabularPostListItem"%>
<%@Register TagPrefix="nJupiterCtrl" Namespace="nJupiter.Web.UI.Controls" Assembly="nJupiter.Web.UI"%>
<nJupiterCtrl:WebGenericControl ID="ctrlTableRow" TagName="tr" Runat="server">
	<nJupiterCtrl:WebGenericControl ID="ctrlTitle" TagName="td" CssClass="column1" Runat="server">
		<nJupiterCtrl:WebAnchor ID="ctrlTitleAnchor" Runat="server"/>
	</nJupiterCtrl:WebGenericControl>
	<nJupiterCtrl:WebGenericControl ID="ctrlBody" TagName="td" CssClass="column2" Runat="server">
		<nJupiterCtrl:WebAnchor ID="ctrlBodyAnchor" Runat="server"/>
	</nJupiterCtrl:WebGenericControl>
	<nJupiterCtrl:WebGenericControl ID="ctrlRootPostTitle" TagName="td" CssClass="column3" Runat="server">
		<nJupiterCtrl:WebAnchor ID="ctrlRootPostTitleAnchor" Runat="server"/>
	</nJupiterCtrl:WebGenericControl>
	<nJupiterCtrl:WebGenericControl ID="ctrlCategory" TagName="td" CssClass="column4" Runat="server">
		<nJupiterCtrl:WebAnchor ID="ctrlCategoryAnchor" Runat="server"/>
	</nJupiterCtrl:WebGenericControl>
	<nJupiterCtrl:WebGenericControl ID="ctrlTimePosted" TagName="td" CssClass="column5" Runat="server"/>
	<nJupiterCtrl:WebGenericControl ID="ctrlAuthor" TagName="td" CssClass="column6" Runat="server">
		<nJupiterCtrl:WebAnchor ID="ctrlAuthorAnchor" Runat="server"/>
	</nJupiterCtrl:WebGenericControl>
	<nJupiterCtrl:WebGenericControl ID="ctrlPostCount" TagName="td" CssClass="column7" Runat="server"/>
	<nJupiterCtrl:WebGenericControl ID="ctrlTimeLastPost" TagName="td" CssClass="column8" Runat="server"/>
	<nJupiterCtrl:WebGenericControl ID="ctrlSearchRelevance" TagName="td" CssClass="column9" Runat="server"/>
	<nJupiterCtrl:WebGenericControl ID="ctrlVisible" TagName="td" CssClass="column10" Runat="server">
		<nJupiterCtrl:WebGenericControl ID="ctrlVisibleSpan" TagName="span" Runat="server"/>
	</nJupiterCtrl:WebGenericControl>
	<nJupiterCtrl:WebGenericControl ID="ctrlEffectivelyVisible" TagName="td" CssClass="column11" Runat="server">
		<nJupiterCtrl:WebGenericControl ID="ctrlEffectivelyVisibleSpan" TagName="span" Runat="server"/>
	</nJupiterCtrl:WebGenericControl>
</nJupiterCtrl:WebGenericControl>