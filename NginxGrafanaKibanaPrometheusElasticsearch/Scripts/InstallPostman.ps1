#Download and Install Postman
$downloadroot = 'C:\WK\SureTax\Download'


function installPostman()
{
    param ($downloadroot)
	Write-Output "Installing Postman"
	Start-Process -Wait -FilePath "$downloadroot\PostmanInstaller.exe";
}
installPostman -downloadroot $downloadroot 
Write-Host "Installing Postman Completed"

