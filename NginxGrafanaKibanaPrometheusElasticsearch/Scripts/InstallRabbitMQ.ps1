
$downloadroot = 'C:\WK\SureTax\Download'
$appinstallroot = 'C:\WK\SureTax\Applications'

if(Test-Path $appinstallroot)
{
Write-Output "Installing at $appinstallroot"
}
else
{
Write-Output "Creating Folder:  $appinstallroot"
New-Item -ItemType Directory -Force -Path $appinstallroot
Write-Output "Installing at $appinstallroot"
}
function installRabbitMq()
{
    param ($downloadroot,$appinstallroot)
	$appdir = "$appinstallroot\rabbitmq"
	$rabbitdir = $appdir

	Write-Host "Installing $downloadroot\rabbitmq-server-3.8.3.exe"
	#Get-ChildItem Env:ERLANG_HOME
	Start-Process -Wait -FilePath "$downloadroot\rabbitmq-server-3.8.3.exe";
}
installRabbitMq -downloadroot $downloadroot -appinstallroot $appinstallroot
Write-Output "Installing RabbitMQ Completed"


