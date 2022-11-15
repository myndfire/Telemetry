
$downloadroot = 'C:\WK\SureTax\Download'
$appinstallroot = 'C:\WK\SureTax\Tools'
$nssm="$appinstallroot\nssm\nssm-2.24\win64\nssm.exe"
$prometheus="$appinstallroot\prometheus\prometheus-2.18.1.windows-amd64\prometheus.exe"

function downloadBinaries
{
Write-Host "Downloading Installers Started" -ForegroundColor Yellow
(new-object System.Net.WebClient).DownloadFile("https://artifacts.elastic.co/downloads/elasticsearch/elasticsearch-7.6.2-windows-x86_64.zip", "$downloadroot\elasticsearch-7.6.2-windows-x86_64.zip")
(new-object System.Net.WebClient).DownloadFile("https://artifacts.elastic.co/downloads/kibana/kibana-7.6.2-windows-x86_64.zip", "$downloadroot\kibana-7.6.2-windows-x86_64.zip")
(new-object System.Net.WebClient).DownloadFile("https://www.7-zip.org/a/7z1900-x64.msi", "$downloadroot\7z1900-x64.msi")
(new-object System.Net.WebClient).DownloadFile("https://nssm.cc/release/nssm-2.24.zip", "$downloadroot\nssm-2.24.zip")
(new-object System.Net.WebClient).DownloadFile("https://github.com/prometheus/prometheus/releases/download/v2.18.1/prometheus-2.18.1.windows-amd64.tar.gz", "$downloadroot\prometheus-2.18.1.windows-amd64.tar.gz")
(new-object System.Net.WebClient).DownloadFile("https://github.com/martinlindhe/wmi_exporter/releases/download/v0.11.1/wmi_exporter-0.11.1-amd64.msi", "$downloadroot\wmi_exporter-0.11.1-amd64.msi")
(new-object System.Net.WebClient).DownloadFile("https://dl.grafana.com/oss/release/grafana-7.0.1.windows-amd64.msi", "$downloadroot\grafana-7.0.1.windows-amd64.msi")
(new-object System.Net.WebClient).DownloadFile("https://dl.grafana.com/oss/release/grafana-7.0.1.windows-amd64.zip", "$downloadroot\grafana-7.0.1.windows-amd64.zip")
(new-object System.Net.WebClient).DownloadFile("http://nginx.org/download/nginx-1.18.0.zip", "$downloadroot\nginx-1.18.0.zip")
Write-Host "Downloading Monitoring Tools Completed" -ForegroundColor Green
}
downloadBinaries