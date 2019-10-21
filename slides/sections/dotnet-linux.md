# .NET Core in Linux containers

We'll be using Kubernetes from now on. You can deploy hybrid Kube clusters with Linux masters, Linux workers and Windows workers.

But on a single-node cluster, you can only run Linux pods.

> That's fine because the latest app upgrade migrates everything to .NET Core

---

## Build the Linux app versions

There's a new .NET Core version of the [save message handler](./src/SignUp.MessageHandlers.SaveProspectCore), and a new [Blazor front-end web project](./src/SignUp.Web.Blazor).

Those are .NET Core 3.0 apps, so they can be built to run in Docker containers on Windows or Linux (or Arm processor like the Raspberry Pi - but save that for another day).

_Build the Linux versions using the [docker-compose-build-linux.yml](.ci/docker-compose-build-linux.yml) manifest:_

```
docker-compose -f ./ci/docker-compose-build-linux.yml build
```

---

## Using multi-arch images

The [.NET Core save handler Dockerfile]() and the [Blazor web app Dockerfile]() use Microsoft's .NET Core images to build and package the app.

Those are multi-architecture images, which means there are different variants with the same image name. When you use one of those containers, Docker will pull the matching variant for your current runtime.

On Windows you'll build a Windows Docker image, and on Linux it will be a Linux image - from the same Dockerfile and source code.

---

## How about the other components?

SQL Server is supported on Linux too now, and Microsoft publish a Linux Docker image.

The [NATS]() message queue and the [Traefik]() reverse proxy are also multi-arch images on Docker Hub, so we can use the same image names in Linux container mode to work with the Linux variants.

So we have everything we need to deploy the app to Kubernetes.

---

## Check that with Compose

> TODO

compose up && stack deploy to kube
