### Pre-requisites

You will be provided with a Windows VM running in Azure to use for the workshop. The VM has Docker already installed.

You will build images and push them to Docker Hub during the workshop, so they are available to use later. You'll need a Docker ID to push images.

- Sign up for a free Docker ID on [Docker Hub](https://hub.docker.com)

---

## Now - connect to your VM 

You'll be given the connection details for your Windows VM during the workshop.

You can connect to the VM using RDP on Windows, [Microsoft Remote Desktop](https://itunes.apple.com/us/app/microsoft-remote-desktop-8-0/id715768417) from the Mac App Store or [Remmina](https://github.com/FreeRDP/Remmina/wiki#for-end-users) on Linux.

_RDP into the VM. The machine name will be something like:_

```
tnl19001.centralus.cloudapp.azure.com
```

---

## Run Docker

There's a _Docker Desktop_ shortcut on the desktop, launch that to run Docker. You'll see a small whale icon in the taskbar. 

> When the container animation on the whale stops, and it's fully loaded then Docker is running.

---

## Update your VM setup

Now run a script to make sure everything is up to date.

_Open a PowerShell prompt from the start menu and run:_

```
cd $env:workshop

.\workshop\lab-vm\update.ps1
```

> **Do not use PowerShell ISE for the workshop!** It has a strange relationship with some `docker` commands.

---

## We're ready!

Here we go :)

