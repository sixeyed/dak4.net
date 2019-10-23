# Production Readiness - Logging

---

The basic goal of logging is to get your application logs out of the container, so you can read them with `docker container logs` or `kubectl logs`.

You can do log aggregation in your container platform, collecting and storing all the log entries from all your containers in a central system - but first you need to get them out from the container.

---

## Logging in console apps

That's easy for .NET Core apps - they run as console apps under the `dotnet` runtime.

Any log entries written to the console appender (or `Console.WriteLine`) are written to the standard output stream, which is where Docker fetches log entries.

_You've already seen these logs in the handlers:_

```
kubectl logs --selector component=index-handler
```

---

## Logging in ASP.NET Core apps

The same is true for .NET Core web apps, which also run as console apps and use the console appender by default.

_Check the logs for the REST API:_

```
kubectl logs --selector component=api
```

> The logs are there, inside a file in the container - but Docker doesn't know about that.

---

## Logging to other sinks

But some apps don't write to the console. Windows containers running ASP.NET apps on IIS are background processes so there's no console app for Docker to monitor.

Those apps may write to another log sink, like the Event Log or a text file. The Blazor web app does this too - in [Program.cs]() you'll se it uses [Serilog]().

_No console appender means there are no logs:_

```
kubectl logs --selector component=web
```

---

## Sidecar container

This is where sidecar containers are really useful. You run a second container in the same pod , whose role is just to echo logs from the app container.

[k82/prod-logging/signup-web.yaml]() specifies a log-relay sidecar. The main app writes its log file to a shared `volumeMount`. The log relay uses `tail` to read out the logs

---

## Deploy the sidecar pod

"Sidecar" is just the name for a pattern where there's an extra container in the pod doing some ancillary work.

_You deploy pods with sidecars in the same way:_

```
kubectl apply -f .\k8s\prod-logging
```

---

## Examine the pod setup

Now the pod has two containers. They share the same network space, and they have a shared filesystem in the `logs` volumes.

_Describing the pod shows you the two containers:_

```
kubectl describe pod --selector component=web
```

## Check logs

Now there are two containers in the pod, you need to specify which one you want with `kubectl exec` or `kubectl logs`.

_Check the app logs are available now:_

```
kubectl logs --selector component=web

kubectl logs --selector component=web --container signup-web

kubectl logs --selector component=web --container signup-web-logs
```

---

## Echoing logs to Docker

This is a simple pattern to get logs from existing apps into Docker, without changing application code.

You can use it to echo logs from any source in the container - like log files, ETW or the Event Log.

Container platforms have pluggable logging systems, so as long as you get your logs into Dockeryou can set them up to ship to Splunk, Elasticsearch etc.
