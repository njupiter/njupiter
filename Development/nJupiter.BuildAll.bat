SET DOTNETPATH=%SYSTEMROOT%\Microsoft.NET\Framework\v3.5

:build
copy /y "Shared Resources\Libraries\Log4Net\V1.2.10.0\.NET 2.0\log4net.dll" "Shared Resources\Libraries\Log4Net\Compile Version\"
if %ERRORLEVEL% NEQ 0 goto copyerror

%DOTNETPATH%\MSBuild.exe nJupiter.sln /v:m /t:Rebuild /p:Configuration="Debug"

REM Debug only
if "%2" == "Debug" goto end

%DOTNETPATH%\MSBuild.exe nJupiter.sln /v:m /t:Rebuild /p:Configuration="Release"
%DOTNETPATH%\MSBuild.exe nJupiter.sln /v:m /t:Rebuild /p:Configuration="Release Signed"

goto end
:copyerror
echo Failed to copy log4net
exit /B 1

:end