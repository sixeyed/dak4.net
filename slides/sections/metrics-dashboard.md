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

## Build the Prometheus image

Prometheus uses a simple configuration file, listing the endpoints to scrape for metrics.

[This Dockerfile](./docker/metrics-dashboard/prometheus/Dockerfile) bundles a custom [prometheus.yml](./docker/metrics-dashboard/prometheus/prometheus.yml) file on top of the standard Prometheus image.

```
docker image build -t dak4dotnet/prometheus:linux `
  -f ./docker/metrics-dashboard/prometheus/Dockerfile .
```

> This builds a Prometheus image packaged with custom configuration for the application.

---

## About Grafana

Grafana is a dashboard server. It can connect to data sources and provide rich dashboards to show the overall health of your app.

The Grafana team maintain a Docker image for Linux on Docker Hub: [grafana/grafana](https://hub.docker.com/r/grafana/grafana).

Grafana has a couple of options for automating setup, and we'll use them to build a custom Docker image.

---

## Customizing Grafana

To make a custom Grafana image you need to configure a data source, create users and deploy your own dashboard. The [Grafana Dockerfile](./docker/metrics-dashboard/grafana/Dockerfile) does that.

It uses a [data source provisioning](http://docs.grafana.org/administration/provisioning/#datasources) and [dashboard provisioning](http://docs.grafana.org/administration/provisioning/#dashboards), which is standard Grafana functionality, and the Grafana API to set up a read-only user.

---

## Build the Grafana image

_Build the custom Grafana image:_

```
docker image build -t dak4dotnet/grafana:linux `
  -f ./docker/metrics-dashboard/grafana/Dockerfile .
```

> This image is fully configured with a Grafana dashboard to drop into the application.

---

## Running Prometheus and Grafana

We'll run pods for Prometheus and Grafana.

[prometheus.yml](./k8s/metrics-dashboard/prometheus.yml) includes a `ClusterIP` service, so Grafana can read the data from Prometheus.

[grafana.yml](./k8s/metrics-dashboard/grafana.yml) includes a `LoadBalancer` service so we can access it publicly.

---

## Deploy the new components

Applying the new manifests will add Prometheus and Grafana to the default namespace.

_Apply the manifests:_

```
kubectl apply -f ./k8s/metrics-dashboard
```

> Metrics are good candidates for shared services running in their own namespace.

---

## Use the app to record some metrics

Browse to http://localhost/app/signup again and submit the form a few times.

This sends traffic through the web app, API, index handler and save handler, so we'll get metrics from all of them in Prometheus.

---

## Check the data in Prometheus

The web application and the message handlers are collecting metrics now, and Prometheus is scraping them.

> Browse to the Prometheus UI - http://localhost:9090

You can check _Status...Configuration_ and _Status...Targets_ to see everything is working.

---

## CPU metrics in Prometheus

Try looking at the `process_cpu_seconds_total` metric in Graph view. This shows the amount of CPU uses by the `dotnet` process in each container.

The Prometheus UI is good for browsing the collected metrics and building up complex queries.

But the Prometheus UI isn't featured enough for a dashboard - that's why we have Grafana.

---

## Browse to Grafana

The Grafana container is already running with a custom dashboard, reading the application and runtime metrics from Prometheus.

> Browse to Grafana - http://localhost:3000

---

## Open the dashboard

Login with the credentials for the read-only account created in the Grafana Docker image:

- _Username:_ **viewer**
- _Password:_ **readonly**

> You'll see the dashboard showing real-time data from the app. The app dashboard is set as the homepage for this user.

---

## Check out the dashboard

The dashboard shows how HTTP responses and duration from the web layer, and how many events the handlers have received, processed and failed.

It also shows memory and CPU usage for the apps inside the containers, so at a glance you can see how hard your containers are working and what they're doing.

---

## Ready for production...

Containerized apps run on dynamic container platforms. maybe with hundreds of containers running across dozens of servers in production.

A metrics dashboard like this is essential to being ready for production - so when you go live you can be confident that your app is working correctly.

> There's one missing piece from this dashboard - metrics from the Docker platform itself. I cover that in [Monitoring Containerized Application Health](https://pluralsight.pxf.io/c/1197078/424552/7490?u=https%3A%2F%2Fwww.pluralsight.com%2Fcourses%2Fmonitoring-containerized-app-health-docker).
