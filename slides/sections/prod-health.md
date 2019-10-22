# Production Readiness - Health

---

Healthchecks are the final piece to making your old apps behave like new apps when they're running in containers.

The healthecheck should exercise key logic in your app and make sure it's functioning properly. You can do that by adding a dedicated `/health` API endpoint to your app - .NET Core provides [healthchecks](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-3.0) for this.

You can add basic healthchecks with KUbe

---

## Liveness checks

- app process
- pod restart
- liveness check

---

## HTTP readiness probe

> ref data api, startup delay
> readiness check

---

## HTTP liveness probe

> signup web healthchecks

kubectl apply -f ./k8s/prod-health/signup-web.yml

kubectl get pods

---

## Command probes

> handler apps to write file on listen, delete on exit
> liveness check

## Failure recovery

- failed probe(s)
- container restart
- all in pod

## Self-healing applications

A healthcheck lets the container platform test if your application is working correctly. If it's not the platform can kill an unhealthy container and start a replacement.

This is perfect for old apps which have known issues - if they're known to crash and stop responding, the healthcheck means Docker will repair the app with minimal downtime and with no manual intervention.

Production readiness with these patterns means our legacy ASP.NET WebForms app will behave just like a cloud-native app when we deploy it to a Docker cluster.
