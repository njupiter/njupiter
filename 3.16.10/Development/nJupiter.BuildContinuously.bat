@echo off
SET DOTNETPATH=%SYSTEMROOT%\Microsoft.NET\Framework\v3.5

REM Build
copy /y "Shared Resources\Libraries\Log4Net\V1.2.10.0\.NET 2.0\log4net.dll" "Shared Resources\Libraries\Log4Net\Compile Version\"
%DOTNETPATH%\MSBuild.exe nJupiter.sln /nologo /noconsolelogger /logger:ThoughtWorks.CruiseControl.MsBuild.XmlLogger,C:\Projects\Tools\ThoughtWorks.CruiseControl.MsBuild.dll;"%CCNetArtifactDirectory%\msbuild-results.xml" /v:q /t:Build /p:Configuration="Debug"

