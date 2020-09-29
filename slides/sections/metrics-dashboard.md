# Running a Metrics Dashboard

---

Exposing metrics endpoints from all your app containers is the first step to getting consistent monitoring.

Next you need to run two other components - a metrics server, which grabs and stores all the metrics from your containers, and a dashboard which presents the data in a usable way.

We'll do that by running [Prometheus](https://prometheus.io) and [Grafana](https://grafana.com) - the leading tools in this space - in containers alongside our app.

---

## About Prometheus

Prometheus is a metrics server. It runs a time-series database to store instrumentation data, polls configured endpoints to collect data, and provides an API (and a simple Web UI) to retrieve the raw or aggregated data.

The Prometheus team maintain a Docker image for Linux on Docker Hub: [prom/prometheus](https://hub.docker.com/r/prom/prometheus).

---

## How Prometheus uses the Kubernetes API

Prometheus can use service discovery to find targets to scrape. 

The [Prometheus ConfigMap](./k8s/metrics-dashboard/prometheus/prometheus-config.yaml) sets it up to connect to the Kubernetes API and find all the running Pods.

Any Pods in the default namespace are added to the target list.

---

## Run Prometheus

Deploy Prometheus from [prometheus.yaml]() - this creates the Deployment, ConfigMap and a LoadBalancer Service.

```
kubectl apply -f k8s/metrics-dashboard/prometheus/
```

> Browse to the Prometheus target list at http://localhost:9090/targets

---

## Check the app versions

All the app components publish a version number metric which is useful for correlation.

Browse to the graph UI in Prometheus at http://localhost:9090/graph.

Choose `app_info` from the metric list and select _Execute_.

> You can add the version number to other metrics in queries.

---

## CPU metrics in Prometheus

Try looking at the `process_cpu_seconds_total` metric in Graph view. This shows the amount of CPU uses by the `dotnet` process in each container.

The Prometheus UI is good for browsing the collected metrics and building up complex queries.

But the Prometheus UI isn't featured enough for a dashboard - that's why we have Grafana.

---

## About Grafana

Grafana is a dashboard server. It can connect to data sources and provide rich dashboards to show the overall health of your app.

The Grafana team maintain a Docker image for Linux on Docker Hub: [grafana/grafana](https://hub.docker.com/r/grafana/grafana).

Grafana has a couple of options for automating setup, and we'll use them to run a fully-configured dashboard.

---

## Customizing Grafana

To run a customized Grafana server you need to configure a data source and deploy your own dashboard. The Grafana Dockerfile confiuguration) does that:

- [grafana-config.yaml](./k8s/metrics-dashboard/grafana/grafana-config.yaml) sets up  [data source provisioning](http://docs.grafana.org/administration/provisioning/#datasources) and [dashboard provisioning](http://docs.grafana.org/administration/provisioning/#dashboards)

- [grafana-dashboard-signup.yaml](./k8s/metrics-dashboard/grafana/grafana-dashboard-signup.yaml) contains the JSON model of the application dashboard 

---

## Run Grafana

Deploy Grafana from [grafana.yaml]() - this creates the Deployment which uses the ConfigMap and a LoadBalancer Service.

```
kubectl apply -f k8s/metrics-dashboard/grafana/
```

> Browse to Grafana at http://localhost:3000 and sign in with `admin`/`admin`

---

## Check the dashboard

Open the dashboard called _Sign Up_.

The dashboard shows how HTTP responses and duration from the web and API components, and how many events the handlers have received, processed and failed.

It also shows memory and CPU usage for the apps inside the containers, so at a glance you can see how hard your containers are working and what they're doing.

---

## Run a load test

There's not much to see right now, but we can push some load into the site and light up the graphs.

We'll use a tool called [Fortio]() for the test, running Kubernetes Jobs defined in [](k8s\metrics-dashboard\load-test\fortio-api.yml) and [](k8s\metrics-dashboard\load-test\fortio-web.yml).

```
kubectl apply -f k8s/metrics-dashboard/load-test/
```

> This also updates the web and API to introduce some delays and errors

---

# Monitor the dashboard

Switch Grafana to refresh the dashboard every 10 seconds during the load test.

You'll see the number of error responses creep up to about 10% of the incoming load.

And you'll see the web and API responses are much slower.


> This is the kind of detail that gives you an instant health overview

---

## Ready for production...

Containerized apps run on dynamic container platforms. maybe with hundreds of containers running across dozens of servers in production.

A metrics dashboard like this is essential to being ready for production - so when you go live you can be confident that your app is working correctly.

> There's one missing piece from this dashboard - metrics from the Docker platform itself. I cover that in [Monitoring Containerized Application Health](https://pluralsight.pxf.io/c/1197078/424552/7490?u=https%3A%2F%2Fwww.pluralsight.com%2Fcourses%2Fmonitoring-containerized-app-health-docker).
