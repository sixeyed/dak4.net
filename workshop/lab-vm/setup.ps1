param(    
    [string] $branch = 'master'
)

$ErrorActionPreference = "SilentlyContinue"

Write-Output 'Set Windows Updates to manual'
Cscript $env:WinDir\System32\SCregEdit.wsf /AU 1
Net stop wuauserv
Net start wuauserv

# turn off firewall and Defender *this is meant for short-lived lab VMs*
Set-NetFirewallProfile -Profile Domain,Public,Private -Enabled False
Set-MpPreference -DisableRealtimeMonitoring $true

Write-Output '-VM setup script starting-'

$images = 
'mcr.microsoft.com/windows/servercore:ltsc2019',
'mcr.microsoft.com/windows/nanoserver:1809',
'mcr.microsoft.com/windows/servercore/iis:windowsservercore-ltsc2019',
'mcr.microsoft.com/dotnet/framework/sdk:4.7.2-20190312-windowsservercore-ltsc2019',
'mcr.microsoft.com/dotnet/framework/aspnet:4.7.2-windowsservercore-ltsc2019',
'mcr.microsoft.com/dotnet/framework/aspnet',
'mcr.microsoft.com/dotnet/core/runtime:3.0',
'mcr.microsoft.com/dotnet/core/sdk:3.0.100',
'mcr.microsoft.com/dotnet/core/aspnet:3.0',
'dak4dotnet/sql-server:2017',
'nats:2.1.0',
'dockersamples/aspnet-monitoring-exporter:4.7.2-windowsservercore-ltsc2019',
'dockersamples/aspnet-monitoring-grafana:5.2.1-windowsservercore-ltsc2019',
'dockersamples/aspnet-monitoring-prometheus:2.3.1-windowsservercore-ltsc2019',
'sixeyed/elasticsearch:5.6.11-windowsservercore-ltsc2019',
'sixeyed/kibana:5.6.11-windowsservercore-ltsc2019',
'traefik:1.7.18-windowsservercore-1809'

#$images = Get-Content -path .\images-linux.txt

Write-Output '* Pulling images'
foreach ($tag in $images) {
    Write-Output "** Processing tag: $tag"
    & docker image pull $tag
}

Write-Output '* Installing Chocolatey'
Set-ExecutionPolicy -ExecutionPolicy Unrestricted -Force
Invoke-Expression ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))

Write-Output '* Installing tools'
choco install -y docker-compose
choco install -y poshgit
choco install -y visualstudiocode
choco install -y firefox

Write-Output '* Configuring environment'
refreshenv
$env:PATH=$env:PATH + ';C:\Program Files\Mozilla Firefox;C:\Program Files\Git\bin'
[Environment]::SetEnvironmentVariable('PATH', $env:PATH, [EnvironmentVariableTarget]::Machine)
$env:workshop='C:\scm\dak4.net'
[Environment]::SetEnvironmentVariable('workshop', $env:workshop, [EnvironmentVariableTarget]::Machine)

New-ItemProperty -Path HKLM:\Software\Microsoft\ServerManager -Name DoNotOpenServerManagerAtLogon -PropertyType DWORD -Value "1" -Force
New-ItemProperty -Path HKLM:\Software\Microsoft\ServerManager\Oobe -Name DoNotOpenInitialConfigurationTasksAtLogon -PropertyType DWORD -Value "1" -Force

Write-Output '* Cloning the workshop repo'
mkdir C:\scm -ErrorAction Ignore
cd C:\scm
git clone https://github.com/sixeyed/dak4.net.git
git checkout $branch
$branch | Out-File C:\branch.txt

Write-Output '-VM setup script done-'
