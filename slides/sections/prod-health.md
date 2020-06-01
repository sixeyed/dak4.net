# Production Readiness - Health

---

Healthchecks are the final piece to making your apps behave like good citizens in the container platform. With health and readiness checks you can make your apps self-healing.

The healthcheck should exercise key logic in your app and make sure it's functioning properly. You can do that by adding a dedicated `/health` API endpoint to your app - .NET Core provides [healthchecks](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-3.0) for this.

You don't need application logic though, Kubernetes supports health checks with [container probes](https://kubernetes.io/docs/concepts/workloads/pods/pod-lifecycle/#container-probes).

---

## Readiness probe

There are two types of health check. The first is **readiness** - checking that the application is ready to work.

When a container gets started it could take a few seconds for the app to be available, or it might need to check dependencies are available.

[Readiness probes](https://kubernetes.io/docs/concepts/workloads/pods/pod-lifecycle/#when-should-you-use-a-readiness-probe) fire when the Pod is created, and a Kubernetes Service won't send any traffic to the Pod until the probe is successful.

---

## HTTP readiness probe

The reference data API has a [/ready](./src/SignUp.Api.ReferenceData/Controllers/ReadyController.cs) endpoint which checks it has access to the SQL Server database.

This is a simple dependency check which is used in a readiness probe in [k8s/prod-health/reference-data-api-broken.yml](./k8s/prod-health/reference-data-api-broken.yml).

> This spec is misconfigured so the readiness check will always fail


## Check the current API Service

There's a single Pod running the API, with a label matching the Service's label selector.

Check the Service status:

```
kubectl get svc reference-data-api -o wide
```

And the endpoints - these are the Pod IP address registered for the Service:

```
kubectl get endpoints reference-data-api
```

## Deploy the broken API

This deployment adds a readiness check which always fails. Update the API:

```
kubectl apply -f k8s/prod-health/reference-data-api-broken.yml
```

Check the Pod status:

```
kubectl get pods -l component=api
```

> The new Pod never enters the Ready state, so the old Pod is not replaced

---

## Check the API

The Deployment makes sure the old Pod stays up until the new Pod is ready.

The Service won't replace the endpoint of the old Pod either.

```
kubectl get endpoints reference-data-api
```

> Readiness checks prevent misconfigurations breaking your app

## Let's fix the API

The updated API spec in [k8s/prod-health/reference-data-api.yml](./k8s/prod-health/reference-data-api.yml) uses the same readiness check but fixes the configuration.

```
kubectl apply -f k8s/prod-health/reference-data-api.yml
```

_Watch the Pods update:_

```
kubectl get pods -l component=api --watch
```

> It'll take about 25 seconds for the new Pod to become ready; _Ctrl-C_ to exit


## Take the database down

If the database goes offline the API can't function, so it makes sense to stop receiving traffic in case mutiple attempts to use the database make the situation worse.

_Pausing the database container makes it unavailable:_

```
docker ps -q -f label="io.kubernetes.container.name=signup-db"
```

```
docker pause $(docker ps -q -f label="io.kubernetes.container.name=signup-db")
```

## And see what happens when the check fails

_Watch the API Pod:_

```
kubectl get pods -l component=api --watch
```

_It will switch to not being ready; Ctrl-C and check endpoints:_

```
kubectl get endpoints reference-data-api
```

---

## Fix the API again

There are no Pod IP addresses in the API Service because no Pods are ready. Start the database container again and the readiness check will pass, adding the Pod back into the Service.

```
docker unpause $(docker ps -q -f label="io.kubernetes.container.name=signup-db")
```

_Check the Service endpoints:_

```
kubectl get endpoints reference-data-api
```

---

## Liveness probes

**Liveness** is the second type of health check. Containers have a process they run at startup - the `dotnet` runtime for .NET Core apps. If that process crashes then the container exits and Kubernetes restarts the Pod.

But the process could be running even when the app is unhealthy. A web app could return `503` responses to every request, but the `dotnet` process is still running so the container is up and the pod looks healthy.

[Liveness probes](https://kubernetes.io/docs/concepts/workloads/pods/pod-lifecycle/#when-should-you-use-a-liveness-probe) fix that - they check the app is actually healthy. If there are multiple failures the Pod will restart.

---

## HTTP liveness probe

The web app doesn't have a custom healthcheck endpoint, but we can add a basic check to make sure the app is responding.

An HTTP liveness probe in [k8s/prod-health/signup-web.yml](./k8s/prod-health/signup-web.yml) does that.

```
kubectl apply -f k8s/prod-health/signup-web.yml
```

---

## Take the website offline

There's a helpful page in the web app which knocks it permanently offline.

_Watch the app Pod:_

```
kubectl get pods -l component=web --watch
```

> Browse to http://localhost/app/takedown - now the app returns a 503 from the SignUp page

---

## Verify the liveness check

You'll see the Pod restart when the check gets the 503 response. Refresh the app and it's working again.

A Pod "restart" actually creates a new application container, so if your app maintains state in memory or in the local container filesystem, that gets lost on a restart.

> You usually add liveness and readiness checks together, so a restarted Pod won't receive traffic until it's ready

---

## Types of container probe

We've used [HTTP probes](https://kubernetes.io/docs/reference/generated/kubernetes-api/v1.18/#httpgetaction-v1-core) so far for the web app and API. Kubernetes also supports [TCP socket probes](https://kubernetes.io/docs/reference/generated/kubernetes-api/v1.18/#tcpsocketaction-v1-core) and [command probes](https://kubernetes.io/docs/reference/generated/kubernetes-api/v1.18/#execaction-v1-core).

TCP probes are good for services which use custom network protocols - like databases and message queues.

Command probes are good for components which don't receive network requests - like message handlers - or for legacy apps where you want to add health check logic without changing code.

---

## TCP probes for non-HTTP services

SQL Server and NATS use custom TCP ports - there are container probes added in the new Deployment specs: [k8s/prod-health/message-queue.yml](./k8s/prod-health/message-queue.yml) and [k8s/prod-health/signup-db.yml](./k8s/prod-health/signup-db.yml).

_Deploy the updates:_

```
kubectl apply -f k8s/prod-health/message-queue.yml -f k8s/prod-health/signup-db.yml
```

> These are changed Pod specs, so the Deployment creates new Pods

---

## And now we've broken the API

The new database is empty, without a database schema - and the API doesn't have the functionality to create the schema from scratch.

_The API readiness check has failed:_

```
kubectl get pods -l component=api
```

---

## But we can fix it 

The save message handler does have the logic to create the database schema, when it connects it checks the schema exists and if not creates it and adds the seed data.

Adding a liveness check to the message handler to check the database is available will fix this issue - there's a command probe in the spec [k8s/prod-health/save-handler.yml](./k8s/prod-health/save-handler.yml) which does that.

```
kubectl apply -f k8s/prod-health/save-handler.yml
```

_And check the API again:_

```
kubectl get pods -l component=api
```

---

## Self-healing applications

Health checks lets the container platform test if your application is working correctly. If it's not the platform can restart or replace it.

It's a pre-requsite for production apps, so they can ride out temporary faults, and to protect your apps against bad deployments. It's also a good mitigation for apps which have known issues - say a memory leak. A liveness check lets Kubernetes repair the app with minimal downtime and with no manual intervention.

Production readiness with these patterns applies to new .NET Core apps and old .NET framework apps, so all your apps can behave in the same way.
