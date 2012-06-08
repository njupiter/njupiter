You have to add an HttpHandler and a HttpModule to your web.config
<system.web>
	<httpHandlers>
		<add verb="*" path="EOContact.ashx" type="nJupiter.Web.UI.EmailObfuscator.FileHandler, nJupiter.Web.UI.EmailObfuscator"/>
	</httpHandlers>
	<httpModules>
		<add name="EmailObfuscatorModule" type="nJupiter.Web.UI.EmailObfuscator.EmailObfuscatorModule, nJupiter.Web.UI.EmailObfuscator" />
	</httpModules>


