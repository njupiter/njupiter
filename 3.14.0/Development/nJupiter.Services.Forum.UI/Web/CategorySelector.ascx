<%@Control Language="c#" AutoEventWireup="false" Codebehind="CategorySelector.ascx.cs" Inherits="nJupiter.Services.Forum.UI.Web.CategorySelector"%>
<%@Register TagPrefix="nJupiter" Namespace="nJupiter.Web.UI.Controls" Assembly="nJupiter.Web.UI"%>
<fieldset>
	<nJupiter:WebGenericControl ID="ctrlFieldSetLabel" TagName="legend" Runat="server"/>
	<nJupiter:WebLabel ID="lblCategories" For="ctrlCategories" Runat="server"/>
	<nJupiter:WebDropDownList ID="ctrlCategories" Runat="server"/>
	<nJupiter:WebPlaceHolder ID="ctrlNoScript" Runat="server">
		<p class="submit"><nJupiter:WebButton InnerSpan="true" id="btnSubmit" Runat="server"/></p>
	</nJupiter:WebPlaceHolder>
</fieldset>