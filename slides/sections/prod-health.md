# Production Readiness - Health

---

Healthchecks are the final piece to making your apps behave like good citizens in the container platform. With health and readiness checks you can make your apps self-healing.

The healthecheck should exercise key logic in your app and make sure it's functioning properly. You can do that by adding a dedicated `/health` API endpoint to your app - .NET Core provides [healthchecks](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-3.0) for this.

You don't need application logic though, Kubernetes supports health checks with [probes]().

---

## Readiness probe

There are two types of health check. The first is **readiness** - checking that the application is ready to work.

When a container gets started it could take a few seconds for the app to be available, or it might need to check dependencies are available.

[Readiness probes]() fire when the pod is created, and Kubernetes won't send any traffic to the pod until the probe is successful.

---

## HTTP readiness probe

The reference data API has a [/ready]() endpoint which checks it has access to the SQL Server database.

This is a simple dependency check which is used in a readiness probe in [k8s/prod-health/reference-data-api.yml](./k8s/prod-health/reference-data-api.yml).

```
kubectl apply -f ./k8s/prod-health/reference-data-api.yml
```

---

## Liveness probes

**Liveness** is the second type of health check. Containers have a process they run at startup - the `dotnet` runtime for .NET Core apps. If that process crashes then the container exits anfd Kubernetes will schedule a replacement pod.

But the process could be running even when the app is unhealthy. A web app could return `503` responses to every request, but the `dotnet` process is still running so the container is up and the pod looks healthy.

[Liveness probes]() fix that - they check the app is actually healthy. If there are multiple failures the pod will restart.

---

## HTTP liveness probe

The web app doesn't have a custom healthcheck endpoint, but we can add a basic check to make sure the app is responding.

An HTTP liveness probe in [k8s/prod-health/signup-web.yml](./k8s/prod-health/signup-web.yml) does that.

```
kubectl apply -f ./k8s/prod-health/signup-web.yml
```

---

## Command probes

Kubernetes also support command probes, which let you run any command as a liveness or readiness probe.

This works nicely for apps which don't have HTTP endpoints, or for legacy Windows apps where you want to add health check logic without changing code.

---

## Self-healing applications

Health checks lets the container platform test if your application is working correctly. If it's not the platform can restart or replace it.

This is perfect for apps which have known issues - say a memory leak. A liveness check lets Kubernetes repair the app with minimal downtime and with no manual intervention.

Production readiness with these patterns applies to new .NET Core apps and old .NET framework apps, so all your apps can behave in the same way.
