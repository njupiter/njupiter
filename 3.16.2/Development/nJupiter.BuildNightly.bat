
echo Build, debug only
call nJupiter.BuildAll.bat Debug
if %ERRORLEVEL% NEQ 0 goto builderror

REM Cd back to current directory
CD /d %~dp0

echo Find latest revision number
svn info "http://njupiter.googlecode.com/svn/trunk/" | find "Revision:" > revision.tmp

echo Copy build to nightly release directory
for /F "tokens=2 delims== " %%i in (revision.tmp) do xcopy /Y /E ..\Build C:\Releases\nJupiter\Nightly\%%i\
del revision.tmp

goto end

:builderror
echo Build nightly failed
exit /B 1
:end
