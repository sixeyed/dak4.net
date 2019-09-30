# Docker and Kubernetes for .NET Developers

A workshop by [@EltonStoneman](https://twitter.com/EltonStoneman). This repo contains all the content. The latest release is published at [dak4.net](https://dak4.net).

> This is the full workshop content, which can be tailored for individual deliveries.

## Outline

Docker is a platform for running applications in lightweight units of compute called containers, and Kubernetes is an orchestrator for managing containers at scale. In this workshop you'll learn all about building and running .NET Framework and .NET Core apps in Docker containers, how to make your Dockerized apps ready for production, and how to run and manage containers in Kubernetes.

Migrating your .NET apps to Docker is an easy way to power a cloud migration with no code changes, but it's also a great starting point for decomposing complex and brittle monoliths. You'll learn how to break features out of the monolith and run them in separate containers, using Docker and Kubernetes to plug everything together. You'll see how to use containers to modernize your architecture and adopt new patterns and technologies without a full rewrite.

> It doesn't matter if you're from a dev or an ops background, you'll learn how the Docker platform benefits all aspects of IT.

### Previous versions

[Docker and Kubernetes for .NET Developers](https://dak4.net) is an evolution of my [Docker Windows Workshop](https://github.com/sixeyed/docker-windows-workshop). 

### Repo Structure

```
├───app
├───docker
├───slides
├───src
├───workshop
└───_archive
```

* `app` - Docker Compose files covering the evolution of the demo app
* `docker` - Dockerfiles and content for the container images
* `slides` - workshop content published with Reveal.js to [dak4.net](https://dak4.net)
* `src` - source code for the .NET solution
* `workshop` - logistics for in-person workshops

