
@echo off
echo ================[Please check Env VAR If Not find Path........]================
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
::MSBuild路径
set mSBuildPath="C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"
::编译输出目录
set debugOutputPath="bin\Debug"
set releaseOutputPaht="bin\Release"
::编译平台(Any CPU,x86,x64)
set debugPlatform="Any CPU"
set releasePlatform="Any CPU"
::生成xml文档
set debugGenerateDocumentation=true
set releaseGenerateDocumentation=true
::xml文档输出路径
set debugDocumentationFile="bin\Debug\"%projectName%".xml"
set releaseDocumentationFile="bin\Release\"%projectName%".xml"
::输出pdb文件
set debugSymbols=true
set releaseSymbols=true
echo ================[Init Env VAR End......]
echo ================[Delete Compile files......]
del /f /q %debugOutputPath:"=%\*.*
del /f /q %releaseOutputPaht:"=%\*.*
echo ================[Delete Compile files End]
echo ================[Compiling Debug Version]
%mSBuildPath:"=% %projectName:"=%.csproj /p:OutputPath=%debugOutputPath:"=%;Platform=%debugPlatform%;Configuration=Debug;GenerateDocumentation=%debugGenerateDocumentation%;DocumentationFile=%debugDocumentationFile:"=%;DebugSymbols=%debugSymbols%
if "%errorlevel%" NEQ "0" (
	echo ================[Compiling Debug fail]
	goto end
) else (
	echo ================[Compiling Debug success]
)
echo ================[Compiling Release Version]
%mSBuildPath:"=% %projectName:"=%.csproj /p:OutputPath=%releaseOutputPaht:"=%;Platform=%releasePlatform%;Configuration=Release;GenerateDocumentation=%releaseGenerateDocumentation%;DocumentationFile=%releaseDocumentationFile:"=%;DebugSymbols=%releaseSymbols%
if "%errorlevel%" NEQ "0" (
	echo ================[Compiling Release fail]
	goto end
) else (
	echo ================[Compiling Release success]
)
:end
echo ================[......The End......]================
pause


