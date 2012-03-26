SET DOTNETPATH=%SYSTEMROOT%\Microsoft.NET\Framework\v3.5

:build
copy /y "Shared Resources\Libraries\Log4Net\V1.2.10.0\.NET 2.0\log4net.dll" "Shared Resources\Libraries\Log4Net\Compile Version\"
if %ERRORLEVEL% NEQ 0 goto copyerror

%DOTNETPATH%\MSBuild.exe nJupiter.sln /v:m /t:Rebuild /p:Configuration="Debug"
if %ERRORLEVEL% NEQ 0 goto builderror

REM Debug only
if "%2" == "Debug" goto end

%DOTNETPATH%\MSBuild.exe nJupiter.sln /v:m /t:Rebuild /p:Configuration="Release"
if %ERRORLEVEL% NEQ 0 goto builderror
%DOTNETPATH%\MSBuild.exe nJupiter.sln /v:m /t:Rebuild /p:Configuration="Release Signed"
if %ERRORLEVEL% NEQ 0 goto builderror

goto end
:copyerror
echo Failed to copy log4net
exit /B 1
goto end
:builderror
echo Build all failed
exit /B 1
:end