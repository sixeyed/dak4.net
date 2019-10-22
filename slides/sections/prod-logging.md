# Production Readiness - Logging

---

- logs out from containers to platform
- aggregation at platform level

---

## Logging in console apps

- .NET Core standard
- default stdout

kubectl logs --selector component=index-handler

---

## Logging in ASP.NET Core apps

- same deal

kubectl logs --selector component=reference-data-api

---

## Logging to other sinks

- file, ETW, event logs
- needs a different approach

[Program.cs]()

kubectl logs --selector component=signup-web

---

## Sidecar container

- two containers in pod
- share filesystem
- web writes logs, other tails

[signup-web.yaml]()

---

## Deploy

kubectl apply -f .\k8s\prod-logging

kubectl describe pod --selector component=signup-web

## Check logs

kubectl logs --selector component=signup-web

kubectl logs --selector component=signup-web --container signup-web

kubectl logs --selector component=signup-web --container signup-web-logs

---

## Echoing logs to Docker

This is a simple pattern to get logs from existing apps into Docker, without changing application code.

You can use it to echo logs from any source in the container - like log files or the [Event Log](https://github.com/Microsoft/mssql-docker/blob/a3020afeec9be1eb2d67645ac739438eb8f2c545/windows/mssql-server-windows-express/start.ps1#L75).

Docker has a pluggable logging system, so as long as you get your logs into Docker, it can automatically ship them to Splunk, Elasticsearch etc.
