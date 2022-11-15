
$downloadroot = 'C:\WK\SureTax\Download'
function downloadBinaries
{
Write-Host "Downloading Erlang.."
(new-object System.Net.WebClient).DownloadFile("http://erlang.org/download/otp_win64_22.3.exe", "$downloadroot\otp_win64_22.3.exe")
Write-Host "Downloading Erlang Completed.."

Write-Host "Downloading Postman.."
(new-object System.Net.WebClient).DownloadFile("https://dl.pstmn.io/download/latest/win64", "$downloadroot\PostmanInstaller.exe")
Write-Host "Downloading Postman completed.."

Write-Host "Downloading RabbitMQ.."
(new-object System.Net.WebClient).DownloadFile("https://github.com/rabbitmq/rabbitmq-server/releases/download/v3.8.3/rabbitmq-server-3.8.3.exe", "$downloadroot\rabbitmq-server-3.8.3.exe")
Write-Host "Downloading RabbitMQ Completed.."

Write-Host "Downloading DotNetCore.."
(new-object System.Net.WebClient).DownloadFile("https://download.visualstudio.microsoft.com/download/pr/5bed16f2-fd1a-4027-bee3-3d6a1b5844cc/dd22ca2820fadb57fd5378e1763d27cd/dotnet-hosting-3.1.4-win.exe", "$downloadroot\dotnet-hosting-3.1.4-win.exe")
Write-Host "Downloading DotNetCore completed.."

}
downloadBinaries


