<%@Page language="c#" Codebehind="ForumTest.aspx.cs" AutoEventWireup="false" Inherits="nJupiter.Services.Forum.UI.Web.ForumTest"%>
<%@Register TagPrefix="nJupiterCtrl" Namespace="nJupiter.Web.UI.Controls" Assembly="nJupiter.Web.UI"%>
<%@Register TagPrefix="nJupiter" TagName="FlatTabularPostList" Src="~/FlatTabularPostList.ascx"%>
<%@Register TagPrefix="nJupiter" TagName="HierarchicalPostList" Src="~/HierarchicalPostList.ascx"%>
<%@Register TagPrefix="nJupiter" TagName="AddPost" Src="~/AddPost.ascx"%>
<%@Register TagPrefix="nJupiter" TagName="UpdatePost" Src="~/UpdatePost.ascx"%>
<%@Register TagPrefix="nJupiter" TagName="CategorySelector" Src="~/CategorySelector.ascx"%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>Forum Test</title>
<style type="text/css">
	div div.descendant-posts			{ padding-left:30px }
	tr.hidden, div.hidden				{ color:#808080 }
	tr.hidden-by-hidden,
		div.hidden-by-hidden			{ color:#778899 }
	tr.hidden a, 
		div.hidden a					{ color:#6495ED } 
	tr.hiddenbyhidden a, 
		div.hidden-by-hidden a			{ color:#1E90FF } 
	tr.hidden a:visited, 
		div.hidden a:visited			{ color:#EE82EE } 
	tr.hidden-by-hidden a:visited, 
		div.hidden-by-hidden a:visited	{ color:#DA70D6 } 
	a.ascending:after					{ content:"[asc]" }
	a.descending:after					{ content:"[desc]" }
</style>
</head>
<body>
<nJupiterCtrl:WebForm XHTMLStrictRendering="true" ID="myForm" Runat="server">
<nJupiter:CategorySelector ID="ctrlCategorySelector" Runat="server"/>
<nJupiter:AddPost ID="ctrlAddPost" Visible="false" Runat="server"/>
<nJupiter:UpdatePost ID="ctrlUpdatePost" Visible="false" Runat="server"/>
<nJupiter:FlatTabularPostList ID="ctrlFlatTabularPostList" Runat="server"/>
<nJupiter:HierarchicalPostList ID="ctrlHierarchicalPostList" Runat="server"/>
</nJupiterCtrl:WebForm>
</body>
</html>