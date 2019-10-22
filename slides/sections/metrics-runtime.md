# Exporting Runtime Metrics

---

Containerized applications give you new opportunities for monitoring. You export metrics from each container, collect them centrally and show your whole application health in a dashboard.

The metrics collector and dashboard run in containers too, so now you can run the exact same metrics stack in dev that you use in production.

---

## Application runtime metrics

Runtime metrics tell you how hard .NET is working to run your app. Apps in Windows containers already have those metrics - Windows Performance Counters are collected in containers in the same way that they are on Windows Server.

You can [export IIS and .NET Framework Performance Counters](https://github.com/dockersamples/aspnet-monitoring) from Windows containers.

.NET Core is cross-platform and there are no Windows Performance Counters on Linux, so we need to collect runtime metrics inside the app.

---

## Prometheus .NET Client

The [prometheus-net](https://github.com/prometheus-net/prometheus-net) NuGet package does that for us. It collects key metrics for .NET Core and ASP.NET Core apps.

It's already configured in:

- [Startup.cs for the REST API](./src/SignUp.Api.ReferenceData/Startup.cs) and
- [Startup.cs for the Blazor app](./src/SignUp.Web.Blazor/Startup.cs)

---

## Check the metrics

You can run the apps in standlone containers just to check the metrics.

_Run the Blazor app:_

```
docker container run -d --publish-all `
  --name blazor dak4dotnet/signup-web-blazor:linux
```

> `publish-all` publishes the container port to a random port on the host

---

## Generate some load

HTTP requests to the new container will put some load through the app, and the default metrics will be collected.

_Grab the port of the container and send in some requests:_

```
$port = $(docker container port blazor 80).Replace('0.0.0.0:', '')

for ($i=0; $i -le 10; $i++) { Invoke-WebRequest "http://localhost:$port/app" -UseBasicParsing | Out-Null}
```

> This snippet just finds the container port and makes some GET requests

---

## Check out the runtime metrics

Now you can look at the metrics which the exporter utility makes available. You'll see stats in there covering memory and CPU for the .NET process, and ASP.NET Core performance.

_Fetch the metrics port and browse to the exporter endpoint:_

```
firefox "http://localhost:$port/metrics"
```

> This is Prometheus format. Prometheus is the most popular metrics server for cloud-native apps, and the format is widely used.

---

## Tidy up

The metrics endpoint isn't meant for humans to read, it's an API for Prometheus to consume.

Now we know how the metrics look, let's remove the new container:

```
docker container rm --force blazor
```

> `force` removes a container even if it's still running

---

## Runtime metrics in Kubernetes

The apps running in Kubernetes already have metrics available.

You can't browse to them from outside the cluster because there's no [ingress rule](./k8s/ingress.yml) for the `/metrics` path.

But other pods can reach the `/metrics` URL and that's how we can have Prometheus polling metrics without making them publicly visible.

---

## Key runtime metrics

Runtime metrics tell you how hard your app is working and how well it's handling requests. The .NET Prometheus client gets you some way towards the [SRE Golden Signals](https://www.infoq.com/articles/monitoring-SRE-golden-signals/):

- latency: `http_request_duration_seconds{code="200"}`
- traffic: `http_requests_received_total`
- errors: `http_request_duration_seconds{code="500"}`
- saturation: based on `dotnet_total_memory_bytes` and `process_cpu_seconds_total`

> You can get the same effect using an exporter utility for legacy apps, without code changes.
