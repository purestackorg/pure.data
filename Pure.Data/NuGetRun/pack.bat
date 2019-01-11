@echo off
echo ================[Please check Env VAR If Not find Path........]================
::设置延缓环境变量
setlocal enabledelayedexpansion
cd..
echo ================[Initing Env VAR...]
echo ================[Start Find Project ...]
set projectName="null"
for %%a in (%cd%\*.csproj) do (
	set projectName=%%~na
)
if "%errorlevel%" NEQ "0" (
	echo ================[Find Project Fail...]
	rem goto end
) else (
	echo ================[Find Project Success...]
)
if "%projectName%"=="null" (
	echo ================[Get Project Fail...]
	rem goto end
) else (
	echo ================[Get Project Success...]
	echo ================[Project Name  : %projectName%]
)
set nugetRunFilePath=%cd%\NuGetRun\nuget.exe
set nuGetPath="bin\NuGet\"
set releaseFileName=%projectName%.Release.nupkg
set releaseSymbolsFileName=%projectName%.Release.Symbols.nupkg
set debugSymbolsFileName=%projectName%.Debug.Symbols.nupkg
set debugFileName=%projectName%.Debug.nupkg
set nugetApiKey="123456"
set nugetReleaseSource="http://192.168.6.56:8092/api/v2/package"
set nugetReleaseSymbolsSource="http://192.168.6.56:8092/api/v2/SymbolsReleaseDefault"
set nugetDebugSymbolsSource="http://192.168.6.56:8092/api/v2/SymbolsDebugDefault"
set nugetDebugSource="http://192.168.6.56:8092/api/v2/DebugDefault"
echo ================[Init Env VAR end......]
echo ================[Check If Exist NuGet Dir]
if not exist %nuGetPath:"=% (
	echo ================[NuGet Dir Not Exists and Creating...]
	mkdir %nuGetPath:"=%
) else (
	echo ================[NuGet Dir Has Existed...... ]
)
echo ================[Delete Nuget Dir....]
del /f /q %nuGetPath:"=%*.*
echo ================[Starting Pack Nuget......]
echo ================[Packing Release Version......]
%nugetRunFilePath:"=% pack %projectName%.csproj -symbols -properties Configuration=Release -outputdirectory %nuGetPath:"=%
if "%errorlevel%" NEQ "0" (
	echo ================[Pack Nuget Fail......]
	rem goto end
)
echo ================[Rename Release Package......]
for %%a in (%nuGetPath:"=%*) do (
	echo %%a | findstr /i /c:"symbols.nupkg" > nul && set /a isReleaseSymbols=1 || set /a isReleaseSymbols=0
	if !isReleaseSymbols! EQU 1 (
		set newReleaseFileName=%releaseSymbolsFileName%
	) else (
		set newReleaseFileName=%releaseFileName%
	)
	ren %%a !newReleaseFileName!
	echo ================[Rename Success......：!newReleaseFileName:"=!]
)
if "%errorlevel%" NEQ "0" (
	echo ================[Rename Fail......]
	rem goto end
)
echo ================[Packing Debug-Symbols Version]
%nugetRunFilePath:"=% pack %projectName%.csproj -symbols -outputdirectory %nuGetPath:"=%
if "%errorlevel%" NEQ "0" (
	echo ================[Pack Fail!]
	rem goto end
)
echo ================[Rename Debug Package......]
for %%a in (%nuGetPath:"=%*) do (
	echo %%a | findstr /i "Release" > nul && set /a isReleaseFile=1 || set /a isReleaseFile=0
	if !isReleaseFile! EQU 0 (
		echo %%a | findstr /i /c:"symbols.nupkg" > nul && set /a isDebugSymbols=1 || set /a isDebugSymbols=0
		if !isDebugSymbols! EQU 1 (
			set newDebugFileName=%debugSymbolsFileName%
		) else (
			set newDebugFileName=%debugFileName%
		)
		ren %%a !newDebugFileName!
		echo ================[Rename Success... ：!newDebugFileName:"=!]
	)
)
if "%errorlevel%" NEQ "0" (
	echo ================[Rename fail.... ]
	rem goto end
)
echo ================[Starting Commit to nuget]
echo ================[Commit to  Release Feed]
%nugetRunFilePath:"=% push %cd%\%nuGetPath:"=%%releaseFileName:"=% -apikey %nugetApiKey:"=%  -source %nugetReleaseSource:"=%
if "%errorlevel%" NEQ "0" (
	echo ================[Commit to  Release Feed Fail...]
	rem goto end
) else (
	echo ================[Commit to  Release Feed Success...]
)
echo ================[Commit to Release-Symbols Feed]
%nugetRunFilePath:"=% push %cd%\%nuGetPath:"=%%releaseSymbolsFileName:"=% -apikey %nugetApiKey:"=%  -source %nugetReleaseSymbolsSource:"=%
if "%errorlevel%" NEQ "0" (
	echo ================[Commit to  Release-Symbols Feed Fail...]
	rem goto end
) else (
	echo ================[Commit to  Release-Symbols Feed Success...]
)
echo ================[Commit to Debug-Symbols Feed]
%nugetRunFilePath:"=% push %cd%\%nuGetPath:"=%%debugSymbolsFileName:"=% -apikey %nugetApiKey:"=%  -source %nugetDebugSymbolsSource:"=%
if "%errorlevel%" NEQ "0" (
	echo ================[Commit to  Debug-Symbols Feed Fail...]
	rem goto end
) else (
	echo ================[Commit to  Debug-Symbols Feed Success...]
)
echo ================[Commit to Debug Feed]
%nugetRunFilePath:"=% push %cd%\%nuGetPath:"=%%debugFileName:"=% -apikey %nugetApiKey:"=%  -source %nugetDebugSource:"=%
if "%errorlevel%" NEQ "0" (
	echo ================[Commit to  Debug Feed Fail...]
	rem goto end
) else (
	echo ================[Commit to  Debug Feed Success...]
)
:end
echo ================[......The End......]================

pause