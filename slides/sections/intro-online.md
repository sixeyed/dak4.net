### Pre-requisites

If you want to follow along with the workshop you will need:

- Windows 10 with [Docker Desktop](https://docs.docker.com/docker-for-windows/install/)

(You can use Windows Server 2019 with [Docker Enterprise](https://docs.docker.com/install/windows/docker-ee/) but you can only do the Docker parts, not the Kubernetes section).

You will build images and push them to Docker Hub during the workshop, so they are available to use later. You'll need a Docker ID to push images.

- Sign up for a free Docker ID on [Docker Hub](https://hub.docker.com)

---

## Now - get the code

All the source code for the workshop is on GitHub at [sixeyed/dak4.net](https://github.com/sixeyed/dak4.net)

_Open a PowerShell prompt from the start menu and run:_

```
git clone https://github.com/sixeyed/dak4.net.git
```

> **Do not use PowerShell ISE for the workshop!** It has a strange relationship with some `docker` commands.

---

## Set up your environment

Just to make the workflow easier, set up an environment variable for the path to your workshop code.

_In your same PowerShell session run:_

```
$env:workshop="$(pwd)\dak4.net"

cd $env:workshop
```

> You can always get back to the root of the repo by running `cd $env:workshop`

---

## We're ready!

Here we go :)

