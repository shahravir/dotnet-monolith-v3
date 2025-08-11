@echo off
echo Building .NET Framework 4.0 WCF Service Application...
echo.

REM Check if MSBuild is available
where msbuild >nul 2>nul
if %errorlevel% neq 0 (
    echo Error: MSBuild not found. Please ensure .NET Framework 4.0 is installed.
    echo You can also open the solution in Visual Studio and build from there.
    pause
    exit /b 1
)

REM Build the solution
echo Building solution...
msbuild WcfService.sln /p:Configuration=Release /p:Platform="Any CPU" /verbosity:minimal

if %errorlevel% neq 0 (
    echo.
    echo Build failed! Please check the error messages above.
    pause
    exit /b 1
)

echo.
echo Build completed successfully!
echo.
echo To run the application:
echo 1. Start the service host: WcfServiceHost\bin\Release\WcfServiceHost.exe
echo 2. Test with client: WcfServiceClient\bin\Release\WcfServiceClient.exe
echo.
echo Press any key to exit...
pause >nul
