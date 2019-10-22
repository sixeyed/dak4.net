# .NET Core in Linux containers

We'll be using Kubernetes from now on. You can deploy hybrid Kube clusters with Linux masters, Linux workers and Windows workers.

But on a single-node cluster you can only run Linux pods.

> That's fine because the latest app upgrade migrates everything to .NET Core

---

## Building apps with Docker Compose

Compose files can include a `build` section for each service, specifying the details you'd normally run in `docker image build`.

The file [docker-compose-build-linux.yml](.ci/docker-compose-build-linux.yml) has build details for Linux versions of each component.

Compose files like this are used in CI/CD pipelines, so your whole app gets built and shared with `docker-compose build` and `docker-compose push`.

---

## Build the Linux app versions

There's a new .NET Core version of the [save message handler](./src/SignUp.MessageHandlers.SaveProspectCore), and a new [Blazor front-end web project](./src/SignUp.Web.Blazor).

Those are .NET Core 3.0 apps, so they can be built to run in Docker containers on Windows or Linux (or Arm processor like the Raspberry Pi - but save that for another day).

_Build the Linux versions:_

```
docker-compose -f ./ci/docker-compose-build-linux.yml build
```

---

## Using multi-arch images

The [.NET Core save handler Dockerfile](./docker/backend-async-messaging/save-handler-core/Dockerfile) and the [Blazor web app Dockerfile](./docker/frontend-web/signup-web-blazor/Dockerfile) use Microsoft's .NET Core images to build and package the app.

Those are multi-architecture images, which means there are different variants with the same image name. When you use one of those containers Docker will pull the matching variant for your current runtime.

On Windows you'll build a Windows Docker image, and on Linux it will be a Linux image - from the same Dockerfile and source code.

---

## How about the other components?

SQL Server is supported on Linux too now, and Microsoft publish a Linux Docker image.

The [NATS](https://hub.docker.com/_/nats) message queue and the [Traefik](https://hub.docker.com/_/traefik) reverse proxy are also multi-arch images on Docker Hub, so we can use the same image names in Linux container mode to work with the Linux variants.

So we have everything we need to deploy the app to Kubernetes.

---

## But we should test it first

Docker Desktop is running in Linux container mode, so we can run everything locally using Linux images and Docker Compose.

The [v5 Linux Compose file](./app/v5-linux.yml) doesn't have any Swarm specs, so this is fine for a development environment or a smoke test.

_Run the Linux version:_

```
docker-compose -f ./app/v5-linux.yml up -d
```

---

## Check out the app

A quick test will do...

> Open up http://localhost:8020 as usual

Now we're ready to move onto Kubernetes.
