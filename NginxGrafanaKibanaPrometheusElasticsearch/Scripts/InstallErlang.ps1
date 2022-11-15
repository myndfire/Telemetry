
$downloadroot = 'C:\WK\SureTax\Download'
$appinstallroot = 'C:\WK\SureTax\Applications'

function installErlang
{
 param ($downloadroot,$appinstallroot)
   Try{
        Write-Host "Installing Erlang.."
		New-Item -ItemType Directory -Force -Path $appinstallroot
        $appdir = "$appinstallroot\erlang"
		$ERLANG_HOME = "$appdir\erts-10.7"
		[System.Environment]::SetEnvironmentVariable("ERLANG_HOME", $ERLANG_HOME, [System.EnvironmentVariableTarget]::Machine)

		New-Item -ItemType Directory -Force -Path $appdir
		$powershellArguments = "/S /D=$appdir"
        Start-Process $downloadroot\otp_win64_22.3.exe -ArgumentList $powershellArguments -Wait -Passthru -NoNewWindow 

		# Add Erlang to the path if needed
		$system_path_elems = [System.Environment]::GetEnvironmentVariable("PATH", [System.EnvironmentVariableTarget]::Machine).Split(";")
		if (!$system_path_elems.Contains("%ERLANG_HOME%\bin") -and !$system_path_elems.Contains("$ERLANG_HOME\bin")) 
		{
			Write-Host "Adding erlang to path"
			$newpath = [System.String]::Join(";", $system_path_elems + "$ERLANG_HOME\bin")
			[System.Environment]::SetEnvironmentVariable("PATH", $newpath, [System.EnvironmentVariableTarget]::Machine)
		}
		Write-Host "Installing Erlang Completed.."

    }
    Catch{
        Write-Error "Erlang installation failed:" $_
        Exit 1
    }
}
installErlang -downloadroot $downloadroot -appinstallroot $appinstallroot

