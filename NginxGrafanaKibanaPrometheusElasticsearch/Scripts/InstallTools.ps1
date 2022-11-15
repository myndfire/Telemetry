
$downloadroot = 'C:\WK\SureTax\Download'
$toolsinstallroot = 'C:\WK\SureTax\Tools'
$nssm="$toolsinstallroot\nssm\nssm-2.24\win64\nssm.exe"
$prometheus="$toolsinstallroot\prometheus\prometheus-2.18.1.windows-amd64\prometheus.exe"

function installGrafana()
{
    param ($downloadroot)
	Start-Process -Wait -FilePath "$downloadroot\grafana-7.0.1.windows-amd64.msi";
	Write-Host "Installing Grafana Completed" -ForegroundColor Green

}
function install7zip()
{
    param ($downloadroot)
	Start-Process -Wait -FilePath "$downloadroot\7z1900-x64.msi";
	Write-Host "Installing 7Zip Completed" -ForegroundColor Green

}
function unzipNssm()
{
    param ($downloadroot,$toolsinstallroot)
		$appdir = "$toolsinstallroot\nssm"
		New-Item -ItemType Directory -Force -Path $appdir
		expand-archive -Force -path $downloadroot\nssm-2.24.zip -destinationpath $appdir 		
        Copy-Item -Force -Path "$toolsinstallroot\nssm\nssm-2.24\win64\nssm.exe" -Destination "$appdir\nssm.exe"	
	    Write-Host "Installing nssm Completed" -ForegroundColor Green		
}
function unzipElasticsearch()
{
    param ($downloadroot,$toolsinstallroot)

		$appdir = "$toolsinstallroot\elasticsearch"
		New-Item -ItemType Directory -Force -Path $appdir
		set-alias sz "$env:ProgramFiles\7-Zip\7z.exe"
		sz x $downloadroot\elasticsearch-7.6.2-windows-x86_64.zip "-o$($appdir)" -y			
        Copy-Item -Force -Path "$toolsinstallroot\nssm\nssm-2.24\win64\nssm.exe" -Destination "$appdir\nssm.exe"
	    Write-Host "unzipElasticsearch  Completed" -ForegroundColor Green		
}
function unzipKibana()
{
    param ($downloadroot,$toolsinstallroot)

		$appdir = "$toolsinstallroot\kibana"
		New-Item -ItemType Directory -Force -Path $appdir
		set-alias sz "$env:ProgramFiles\7-Zip\7z.exe"
		sz x $downloadroot\kibana-7.6.2-windows-x86_64.zip "-o$($appdir)" -y			
        Copy-Item -Force -Path "$toolsinstallroot\nssm\nssm-2.24\win64\nssm.exe" -Destination "$appdir\nssm.exe"
	    Write-Host "unzipKibana  Completed" -ForegroundColor Green		
}
function installWmi()
{
    param ($downloadroot,$toolsinstallroot)
	$wmi="$downloadroot\wmi_exporter-0.11.1-amd64.msi"
	
	#$ArgumentList='/i ' + $wmi + ' ENABLED_COLLECTORS=os,cpu,cs,logical_disk,memory,net,system /quiet' 
	#Write-Output $ArgumentList;
	#Start-Process msiexec.exe -Wait -ArgumentList $ArgumentList;
	
	Start-Process msiexec.exe -Wait -ArgumentList @('/i', $wmi, 'ENABLED_COLLECTORS=os,cpu,cs,logical_disk,memory,net,system', 'LISTEN_PORT=9182')
	Write-Host "Installing wmi Completed" -ForegroundColor Green		
}
function installNginx()
{
    param ($downloadroot,$toolsinstallroot)

		$appdir = "$toolsinstallroot\nginx"
		$nginx="$appdir\nginx\nginx.exe"
		New-Item -ItemType Directory -Force -Path $appdir
		set-alias sz "$env:ProgramFiles\7-Zip\7z.exe"

        & sz x $downloadroot\nginx-1.18.0.zip "-o$($appdir)" -y			
		#Start-Process -FilePath $nssm -ArgumentList @('install', 'Nginx',$nginx ) -Wait
	    Write-Host "Installing nginx Completed" -ForegroundColor Green		
}
function unzipPrometheus()
{
    param ($downloadroot,$toolsinstallroot)

		$appdir = "$toolsinstallroot\prometheus"
		New-Item -ItemType Directory -Force -Path $appdir
		set-alias sz "$env:ProgramFiles\7-Zip\7z.exe"

        #sz x "-o$($appdir)" $downloadroot\prometheus-2.18.1.windows-amd64.tar.gz -r ;
        #& ${env:ProgramFiles}\7-Zip\7z.exe x $downloadroot\prometheus-2.18.1.windows-amd64.tar.gz "-o$($appdir)" -y	
		sz x $downloadroot\prometheus-2.18.1.windows-amd64.tar.gz "-o$($appdir)" -y	

       	sz x $appdir\prometheus-2.18.1.windows-amd64.tar "-o$($appdir)" -y	
        Remove-Item $appdir\prometheus-2.18.1.windows-amd64.tar -Force
				
		$powershellArguments = "install Prometheus  $prometheus"
		Copy-Item -Force -Path $nssm -Destination "$toolsinstallroot\prometheus\prometheus-2.18.1.windows-amd64\nssm.exe"	
		
		#Write-Host "Installing Service $prometheus "
	    #& $nssm remove Prometheus
		#& $nssm install Prometheus  $prometheus
        #Start-Process -FilePath $nssm -ArgumentList @('install', 'Prometheus',$prometheus ) -Wait
		#Start-Process -FilePath $nssm -ArgumentList @('start', 'Prometheus') -Wait
	    Write-Host "unzipPrometheus prometheus Completed" -ForegroundColor Green		
}
function unzipGrafana()
{
    param ($downloadroot,$toolsinstallroot)

		$appdir = "$toolsinstallroot\prometheus"
		New-Item -ItemType Directory -Force -Path $appdir
		set-alias sz "$env:ProgramFiles\7-Zip\7z.exe"

        #sz x "-o$($appdir)" $downloadroot\prometheus-2.18.1.windows-amd64.tar.gz -r ;
        #& ${env:ProgramFiles}\7-Zip\7z.exe x $downloadroot\prometheus-2.18.1.windows-amd64.tar.gz "-o$($appdir)" -y	
		sz x $downloadroot\prometheus-2.18.1.windows-amd64.tar.gz "-o$($appdir)" -y	

       	sz x $appdir\prometheus-2.18.1.windows-amd64.tar "-o$($appdir)" -y	
        Remove-Item $appdir\prometheus-2.18.1.windows-amd64.tar -Force
				
		$powershellArguments = "install Prometheus  $prometheus"
		Copy-Item -Force -Path $nssm -Destination "$toolsinstallroot\prometheus\prometheus-2.18.1.windows-amd64\nssm.exe"	
		
		#Write-Host "Installing Service $prometheus "
	    #& $nssm remove Prometheus
		#& $nssm install Prometheus  $prometheus
        #Start-Process -FilePath $nssm -ArgumentList @('install', 'Prometheus',$prometheus ) -Wait
		#Start-Process -FilePath $nssm -ArgumentList @('start', 'Prometheus') -Wait
	    Write-Host "unzipPrometheus prometheus Completed" -ForegroundColor Green		
}
function installPrometheus()
{
    param ($downloadroot,$toolsinstallroot)
		
		#Write-Host "Installing Service $prometheus "
	    #& $nssm remove Prometheus
		#& $nssm install Prometheus  $prometheus
        #Start-Process -FilePath $nssm -ArgumentList @('install', 'Prometheus',$prometheus ) -Wait
		#Start-Process -FilePath $nssm -ArgumentList @('start', 'Prometheus') -Wait
	    Write-Host "Installing prometheus Completed" -ForegroundColor Green		
}
#unzipNssm -downloadroot $downloadroot -toolsinstallroot $toolsinstallroot
#unzipElasticsearch -downloadroot $downloadroot -toolsinstallroot $toolsinstallroot
unzipKibana -downloadroot $downloadroot -toolsinstallroot $toolsinstallroot
#unzipPrometheus -downloadroot $downloadroot -toolsinstallroot $toolsinstallroot
#installPrometheus -downloadroot $downloadroot -toolsinstallroot $toolsinstallroot
#installGrafana -downloadroot $downloadroot -toolsinstallroot $toolsinstallroot
#installNginx -downloadroot $downloadroot -toolsinstallroot $toolsinstallroot
#installWmi -downloadroot $downloadroot -toolsinstallroot $toolsinstallroot
#Make sure the service restart is configured for 
#1. Grafana
#2. Prometheus
#3. Nginx
