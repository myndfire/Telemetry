#dotnet --list-sdks
#dotnet --list-runtimes
$downloadroot = 'C:\WK\SureTax\Download'

function installDotNetCore()
{
    param ($downloadroot)
	Write-Host "Installing $downloadroot\dotnet-hosting-3.1.4-win.exe"
	Start-Process -Wait -FilePath "$downloadroot\dotnet-hosting-3.1.4-win.exe";
}
installDotNetCore -downloadroot $downloadroot 
Write-Host "Installing $downloadroot\dotnet-hosting-3.1.4-win.exe Completed.."
