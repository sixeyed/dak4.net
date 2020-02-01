param(    
    [string] $branch = 'master'
)

$ErrorActionPreference = "SilentlyContinue"

# turn off firewall and Defender *this is meant for short-lived lab VMs*
Set-NetFirewallProfile -Profile Domain,Public,Private -Enabled False
Set-MpPreference -DisableRealtimeMonitoring $true

Write-Output '-VM setup script starting-'

Write-Output '* Installing Chocolatey'
Set-ExecutionPolicy -ExecutionPolicy Unrestricted -Force
Invoke-Expression ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))

Write-Output '* Installing tools'
choco install -y git
choco install -y visualstudiocode
choco install -y firefox
choco install -y docker-desktop
# or for edge:
# choco install -y docker-desktop --pre 

Write-Output '* Configuring environment'
refreshenv
$env:PATH=$env:PATH + ';C:\ProgramData\DockerDesktop\version-bin;C:\Program Files\Docker\Docker\Resources\bin;C:\Program Files\Mozilla Firefox;C:\Program Files\Git\bin'
[Environment]::SetEnvironmentVariable('PATH', $env:PATH, [EnvironmentVariableTarget]::Machine)
$env:workshop='C:\scm\dak4.net'
[Environment]::SetEnvironmentVariable('workshop', $env:workshop, [EnvironmentVariableTarget]::Machine)

Write-Output '* Cloning the workshop repo'
mkdir C:\scm -ErrorAction Ignore
cd C:\scm
git clone https://github.com/sixeyed/dak4.net.git
git checkout $branch
$branch | Out-File C:\branch.txt

Write-Output '* Pulling Windows images'
$images = Get-Content -path C:\scm\dak4.net\workshop\lab-vm\images-windows.txt
foreach ($tag in $images) {
    Write-Output "** Processing tag: $tag"
    & docker image pull $tag
}

Write-Output '-VM setup script done-'

# Manual steps: 
# - launch Docker Desktop (requires reboot)
# - pull Windows images
# - switch to Linux containers
# - bump VM RAM to 4GB
# - enable Kubernetes

# - pull Linux images
# $images = Get-Content -path C:\scm\dak4.net\workshop\lab-vm\images-linux.txt
# foreach ($tag in $images) {
#    Write-Output "** Processing tag: $tag"
#    & docker image pull $tag
# }

# - switch back to Windows