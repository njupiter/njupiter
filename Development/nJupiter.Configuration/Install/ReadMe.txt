To use nJupiter.Configuration you have to add the following keys to your app.config/web.config

<configuration>
	<configSections>
		<!-- nJupiter.Configuration Config Section -->
		<section name="nJupiterConfiguration" type="nJupiter.Configuration.nJupiterConfigurationSectionHandler, nJupiter.Configuration" />
		<!-- ... other keys -->
	</configSections>
	<nJupiterConfiguration>
		<configDirectories>
			<!-- The configDirectory tells in wich folders nJupiter.Configuration shall look for config files -->
			<!-- You can add one or more directories. A directory higher up in the list has higher priority -->
			<!-- If a config with the same name exists in different dirs nJupiter.Configuration choose the first one -->
			<configDirectory value="C:\Projects\MyOtherProject\Development\Config" />
			<configDirectory value="C:\Projects\nJupiter\Development\Shared Resources\Config" />
			<!-- if you run inside a httpcontext you can also use a relative path like this -->
			<configDirectory value="~/Config" />
		</configDirectories>
    </nJupiterConfiguration>
	<!-- ... other config -->
</configuration>