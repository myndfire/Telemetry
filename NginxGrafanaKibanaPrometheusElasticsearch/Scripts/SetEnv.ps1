	$downloadroot = 'C:\WK\SureTax\Download'
$appinstallroot = 'C:\WK\SureTax\Applications'

function waitforservicetostart()
{
	param ($servicename)
	Write-Output "Waiting for service: $servicename to start..."
	$service = Get-Service -Name $servicename
	if($service.status -eq "Stopped") {
		$service.WaitForStatus("Running")
		Write-Host "$service Started!" -ForegroundColor Yellow
	}
	else
	{
		Write-Host "$service Started!" -ForegroundColor Yellow
	}
}
function configureRabbitMq()
{
    param ($downloadroot,$appinstallroot)
	$appdir = "$appinstallroot\rabbitmqnew"
	$rabbitdir = $appdir

	[System.Environment]::SetEnvironmentVariable("RABBITMQ_BASE", $rabbitdir, [System.EnvironmentVariableTarget]::Machine)
	if (-not [Environment]::GetEnvironmentVariable('RABBITMQ_BASE', 'Machine'))
	{
			Write-Host "!!!! RABBITMQ_BASE Env not defined!!!!" -ForegroundColor Red
			Write-Host "RabbitMQ configuration Terminated and Rabbit MQ Service removed.." -ForegroundColor Red
			exit
	}
	$rabbitbase_env=[Environment]::GetEnvironmentVariable('RABBITMQ_BASE', 'Machine')
	Write-Host "RABBITMQ_BASE=$rabbitbase_env" -ForegroundColor Yellow
    Write-Host "Env setting completed" -ForegroundColor Green   
}
configureRabbitMq -downloadroot $downloadroot -appinstallroot $appinstallroot

