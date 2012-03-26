<%@Control Language="c#" AutoEventWireup="false" Codebehind="HierarchicalPostList.ascx.cs" Inherits="nJupiter.Services.Forum.UI.Web.HierarchicalPostList"%>
<%@Register TagPrefix="nJupiterCtrl" Namespace="nJupiter.Web.UI.Controls" Assembly="nJupiter.Web.UI"%>
<%@Register TagPrefix="nJupiterListCtrl" Namespace="nJupiter.Web.UI.Controls.Listings" Assembly="nJupiter.Web.UI"%>
<%@Register TagPrefix="nJupiter" TagName="HierarchicalPostListItem" Src="HierarchicalPostListItem.ascx"%>
<nJupiterListCtrl:GeneralListing ID="ctrlGeneralListing" Runat="server">
<ItemTemplate><nJupiter:HierarchicalPostListItem Runat="server"/></ItemTemplate>
<FooterTemplate><nJupiterCtrl:WebParagraph ID="ctrlMessageNoPosts" CssClass="portlet-msg-alert" Runat="server"/></FooterTemplate>
</nJupiterListCtrl:GeneralListing>