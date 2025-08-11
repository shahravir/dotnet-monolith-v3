# PowerShell script to build and run .NET Framework 4.0 WCF Service Application

Write-Host "Building .NET Framework 4.0 WCF Service Application..." -ForegroundColor Green
Write-Host ""

# Check if MSBuild is available
$msbuildPath = Get-Command msbuild -ErrorAction SilentlyContinue
if (-not $msbuildPath) {
    Write-Host "Error: MSBuild not found. Please ensure .NET Framework 4.0 is installed." -ForegroundColor Red
    Write-Host "You can also open the solution in Visual Studio and build from there." -ForegroundColor Yellow
    Read-Host "Press Enter to exit"
    exit 1
}

# Build the solution
Write-Host "Building solution..." -ForegroundColor Cyan
$buildResult = & msbuild WcfService.sln /p:Configuration=Release /p:Platform="Any CPU" /verbosity:minimal

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "Build failed! Please check the error messages above." -ForegroundColor Red
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Host ""
Write-Host "Build completed successfully!" -ForegroundColor Green
Write-Host ""
Write-Host "To run the application:" -ForegroundColor Yellow
Write-Host "1. Start the service host: WcfServiceHost\bin\Release\WcfServiceHost.exe" -ForegroundColor White
Write-Host "2. Test with client: WcfServiceClient\bin\Release\WcfServiceClient.exe" -ForegroundColor White
Write-Host ""
Write-Host "Press Enter to exit..." -ForegroundColor Cyan
Read-Host
