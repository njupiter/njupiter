
You can configure in which directories nJupiter.Configuration shall look for
config files, you do so by adding the following keys to your
app.config/web.config. It is recommended to do so if you care about
performance.

If you do not do this confguration nJupiter.Configuration will automatically
try to recursively load all files with the extension .config in the currenct
application directory (the web root if your application execute in a
web context).

<configuration>
	<configSections>
		<!-- nJupiter.Configuration Config Section -->
		<section name="nJupiterConfiguration" type="nJupiter.Configuration.nJupiterConfigurationSectionHandler, nJupiter.Configuration" />
		<!-- ... other keys -->
	</configSections>
	<nJupiterConfiguration>
		<!-- You can disable file watching if you do not want the configurations to be automatically discarded when a -->
		<!-- config file is updated by seeing the fileWatchingDisabled attribute to true, if you leve this out the default is false -->
		<!-- You can set a different config suffix by changing the configSuffix, if you leve this out the default is .config -->
		<!-- If you want nJupiter.Configuration to load all config files in the directories on initialization (usualy when -->
		<!-- you call the Config Handler for the first time) you have to set the loadAllConfigFilesOnInit attribute to true, -->
		<!-- if you leave this attribute out the default is false -->
		<configDirectories fileWatchingDisabled="false" configSuffix=".config" loadAllConfigFilesOnInit="true">
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