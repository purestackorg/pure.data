@echo off
echo ================[如果出现找不到路径，请检查批处理的环境变量]================
::设置延缓环境变量
setlocal enabledelayedexpansion
cd..
echo ================[Init Env VAR......]
echo ================[Starting Find Project...]
set projectName="null"
for %%a in (%cd%\*.csproj) do (
	set projectName=%%~na
)
if "%errorlevel%" NEQ "0" (
	echo ================[Find Project fail]
	goto end
) else (
	echo ================[Find Project success]
)
if "%projectName%"=="null" (
	echo ================[Get Project fail]
	goto end
) else (
	echo ================[Get Project success]
	echo ================[Project Name : %projectName%]
)
set nugetRunFilePath=%cd%"\NuGetRun\nuget.exe"
set packagesconfigFilePath="packages.config"
set packagesPath="..\packages"
set nugetReleaseSource="http://localhost:8001/nuget/ReleaseDefault"
set nugetReleaseSymbolsSource="http://localhost:8001/nuget/SymbolsReleaseDefault"
set nugetDebugSymbolsSource="http://localhost:8001/nuget/SymbolsDebugDefault"
set nugetDebugSource="http://localhost:8001/nuget/DebugDefault"
echo ================[Init Env VAR End......]
echo ================[Restoring NuGet Packages]
%nugetRunFilePath:"=% restore -source %nugetReleaseSource:"=% -configfile %packagesconfigFilePath:"=% -packagesdirectory %packagesPath:"=%
if "%errorlevel%" NEQ "0" (
	echo ================[Restoring NuGet Packages Fail]
	rem goto end
) else (
	echo ================[Restoring NuGet Packages Success]
)
:end
echo ================[......The End......]================
pause & exit