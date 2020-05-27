# Production Readiness - Logging

The basic goal of logging is to get your application logs out of the container, so you can read them with `docker container logs` or `kubectl logs`.

You can do log aggregation in your container platform, collecting and storing all the log entries from all your containers in a central system - but first you need to get them out from the container.

---

## Logging in console apps

That's easy for .NET Core apps - they run as console apps under the `dotnet` runtime.

Any log entries written to the console appender (or `Console.WriteLine`) are written to the standard output stream, which is where Docker fetches log entries.

_Kubernetes shows logs at the Pod level, with filtering options:_

```
kubectl logs -l component=index-handler --since=6h --tail=2
```

---

## Logging in ASP.NET Core apps

.NET Core web apps use the same approach, running as console apps and using the console appender by default.

_Check the logs for the REST API:_

```
kubectl logs --selector component=api
```

> Kubectl commands usually have long and short forms for options, `--help` shows you them

---

## Logging to other sinks

But some apps don't write to the console. Windows containers running ASP.NET apps on IIS are background processes so there's no console app for Docker to monitor.

Those apps may write to another log sink, like the Event Log or a text file. The Razor web app does this too - in [Program.cs](./src/SignUp.Web.Core/Program.cs) you can see it uses a Serilog file sink.

_No console appender means there are no logs:_

```
kubectl logs --selector component=web
```

---

## Kubernetes only sees standard output streams

If there are logs in other sinks, they're not part of the container output so Docker and Kubernetes don't know about them.

_The log entries are there in a file in the container:_

```
kubectl exec deploy/signup-web -- cat /logs/signup-web.log
```

> But that means this component doesn't have the same management API as the others

---

## Sidecar container

This is where sidecar containers are really useful. You can run multiple containers in the same Pod, the main app container and some helper containers.

Containers in the same Pod can share parts of their filesystem, so a logging sidecar can read the log file written by the app container and echo it out as container logs.

This updated [signup-web.yaml](./k8s/prod-logging/signup-web.yaml) specifies a logging sidecar. The main app writes its log file to a shared `volumeMount`. The log relay uses `tail` to read out the logs.

---

## Deploy the sidecar pod

"Sidecar" is just the name for a pattern where there's an extra container in the pod doing some ancillary work.

You should still model your Pod to run one logical component (a web app or API), the sidecar container helps to integrate the app with the container platform.

_You deploy pods with sidecars in the same way:_

```
kubectl apply -f ./k8s/prod-logging
```

---

## Examine the pod setup

Now the pod has two containers. They share the same network space, and they have a shared filesystem in the `logs` volume.

_Describing the pod shows you the two containers, and you can check their shared directory:_

```
kubectl describe pod --selector component=web
```

```
kubectl exec deploy/signup-web -c signup-web -- ls -l /logs
```

```
kubectl exec deploy/signup-web -c signup-web-logs -- ls -l /logs
```

> You use the `-c` flag to specify a container in a multi-container Pod

## Check logs

There are two containers in the Pod, so there are two log streams. The output from the app container is still empty, but the log lines are echoed out in the sidecar container.

_Check the app logs are available now:_

```
kubectl logs --selector component=web

kubectl logs --selector component=web --container signup-web

kubectl logs --selector component=web --container signup-web-logs
```

---

## Echoing logs with a sidecar

This is a simple pattern to get logs from existing apps into Docker and Kubernetes, without changing application code.

You can use it to echo logs from any source in the container - like log files, ETW or the Event Log. It's inefficient because it duplicates your logs, but it means all apps can be configured to work in the same way.

Container platforms have pluggable logging systems, so as long as you get your logs out from your containers you can set them up to ship to Splunk or Elasticsearch or whatever tool you use.
