# Exporting Application Metrics

---

The next level of detail is application-level metrics, recording details about what your app is doing. You surface those through a metrics API in the same way as the runtime metrics.

There are application metrics in the message handlers, so we can see the flow of messages through the system.

---

## Expose metrics from the message handlers

The message handlers use the same `prometheus-net` NuGet package, but they explicitly record app metrics.

You can see this in the [QueueWorker.cs](./src/SignUp.MessageHandlers.SaveProspectCore/Workers\QueueWorker.cs) file for the save handler, and the [QueueWorker.cs](./src/SignUp.MessageHandlers.IndexProspect/Workers/QueueWorker.cs) file for the index handler.

> `prometheus-net` is a .NET Standard library, so you can use it from .NET Framework and .NET Core apps.

---

## About Prometheus metrics

Prometheus uses a time-series database - it grabs metrics on a schedule and stores every value along with a timestamp. You can aggregate across dimensions or drill down to specific values.

You should record metrics at a fairly coarse level - "Event count" in this example. Then add detail with labels, like the processing status and the hostname of the handler.

---

## A new spec for the handlers

There's a feature flag in the handler code which turns metrics on or off - the default is off.

There are new YAML files to turn the flag on and add a `ClusterIP` service for the handlers:

- [k8s/metrics-application/index-handler.yml](./k8s/metrics-application/index-handler.yml)
- [k8s/metrics-application/save-handler.yml](./k8s/metrics-application/save-handler.yml)

> These components need a service so the Prometheus pod can reach them

---

## Re-deploy the handlers

Environments are fixed for the life of pods, so a new pod spec means the pods will be replaced.

_Deploy the changes from the `metrics-application` folder:_

```
kubectl apply -f ./k8s/metrics-application
```

> Now we have new containers from the same images, but configured to expose metrics

---

## Open up the metrics

The metrics in here aren't public either, but we have a `ClusterIP` service so we can use port forwarding to check what's being recorded.

_Forward the port for index-handler deployment:_

```
kubectl port-forward deployment/index-handler 38000:50505
```

---

## Check the metrics

Browse to http://localhost:38000/metrics/ and you'll see help text for the `IndexHandler_Events` metric, but no data.

> Go back to http://localhost/app/signup and sign up again

Refresh the metrics and you'll see data for the `received` and `processed` labels.

---

## Application metrics

Metrics about what your application is actually doing give you useful insight into your application health, and how work is being distribuited among containers.

You need to write code to get this level of detail, but all the major languages have Prometheus client libraries which make it very easy to capture and export metrics.
