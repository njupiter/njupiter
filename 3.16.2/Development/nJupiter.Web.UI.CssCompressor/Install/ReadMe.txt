You have to add an HttpHandler to your web.config
<system.web>
	<httpHandlers>
		<add verb="*" path="*.css" type="nJupiter.Web.UI.CssCompressor.CssHandler, nJupiter.Web.UI.CssCompressor"/>
	</httpHandlers>

You have to add an extension mapping in the IIS admin properties of your web site (Home Directory -> Configuration -> Mappings)
to make .NET process .css files. It should point to the aspnet_isapi.dll just like the aspx extension. Check "All Verbs" and 
"Script Engine" and uncheck "Check that file exists"

If you use EPiServer, you have to set execute permissions in IIS to "Scripts only" for the EPiServer folder /Util/Styles