
$downloadroot = 'C:\WK\SureTax\Download'
$toolsinstallroot = 'C:\WK\SureTax\Tools'
$nssm="$toolsinstallroot\nssm\nssm-2.24\win64\nssm.exe"
$prometheus="$toolsinstallroot\prometheus\prometheus-2.18.1.windows-amd64\prometheus.exe"

function downloadTools
{
Write-Host "Downloading Installers Started" -ForegroundColor Yellow
(new-object System.Net.WebClient).DownloadFile("https://www.7-zip.org/a/7z1900-x64.msi", "$downloadroot\7z1900-x64.msi")
(new-object System.Net.WebClient).DownloadFile("https://nssm.cc/release/nssm-2.24.zip", "$downloadroot\nssm-2.24.zip")
(new-object System.Net.WebClient).DownloadFile("https://artifacts.elastic.co/downloads/elasticsearch/elasticsearch-7.6.2-windows-x86_64.zip", "$downloadroot\elasticsearch-7.6.2-windows-x86_64.zip")
(new-object System.Net.WebClient).DownloadFile("https://artifacts.elastic.co/downloads/kibana/kibana-7.6.2-windows-x86_64.zip", "$downloadroot\kibana-7.6.2-windows-x86_64.zip")
Write-Host "Downloading Monitoring Tools Completed" -ForegroundColor Green
}

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
downloadTools
install7zip -downloadroot $downloadroot -toolsinstallroot $toolsinstallroot
unzipNssm -downloadroot $downloadroot -toolsinstallroot $toolsinstallroot
#unzipElasticsearch -downloadroot $downloadroot -toolsinstallroot $toolsinstallroot
#unzipKibana -downloadroot $downloadroot -toolsinstallroot $toolsinstallroot

#-------------------- Post Script Installation ------------------
#Update elasticsearch and kibana config
#modify/copy elasticsearch/kibana config yml to appropriate folder for elasticsearch and kibana
#start elasticesearch service:
#nssm install Elaticsearch  C:\WK\SureTax\Tools\elasticsearch\elasticsearch-7.6.2\bin\elasticsearch.bat
#start kibana service:
#nssm install Kibana  C:\WK\SureTax\Tools\kibana\kibana-7.6.2-windows-x86_64\bin\kibana.bat
 "C:\Windows\Microsoft.NET\Framework\v4.0.30319\installutil.exe" "c:\"(path to bat files)
 sc create "Kibana 5.5.0" binPath= "path_to_kibana/bin/kibana.bat" depend= "elasticsearch-service-x64"
#--------------To remove services--------------
#nssm stop Elaticsearch
#nssm remove Elaticsearch

#nssm stop Kibana
#nssm remove Kibana
