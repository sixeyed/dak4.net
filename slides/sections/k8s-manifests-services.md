# Deploying to Kubernetes

So far we've used `kubectl` to imperatively create and remove Kubernetes resources. You wouldn't do that for anything other than quick exploration.

For everything else you describe the components of your app in Kubernetes manifests. This is the declarative approach - you deploy the manifest to the cluster, and Kubernetes works out what resources to create.

---

## Kubernetes manifests

Kubernetes uses its own manifest spec, different from Docker Compose. It's much more verbose because there are more resource types; the relation between resources needs to be specified; and the resources themselves have more flexible configuration.

Kube manifests are typically many times larger than the equivalent Docker Compose manifests which Docker Swarm uses.

---

## Pod deployments

The simplest part of our workshop app is the homepage. The Kubernetes manifest for that is in [homepage-pod.yml](./k8s/homepage-pod.yml).

It describes a deployment, which will create a ReplicaSet, which will manage pods. The pod will run a Docker container hosting the homepage.

_Deploy the manifest:_

```
kubectl apply -f ./k8s/homepage-pod.yml
```

> This manifest shows how labels are used to decouple resources

---

## Pod networking

Pods have IP addresses within the cluster, but they are not directly accesible outside of the cluster.

_You can temporarily send network traffic into the container by forwarding the port:_

```
kubectl port-forward deployment/homepage 8090:80
```

> Now browse to http://localhost:8090

---

## Checking application logs

Port-forwarding creates a tunnel between your local machine and the Kubernetes pod. It only lasts while you have the port-forward command running.

_Exit the port-forward with **Ctrl-C** and check the logs:_

```
kubectl logs --selector app=signup,component=homepage
```

> Labels and selectors are used to link resources, and they can also be used in operations

---

## A service for the pod

The homepage pod isn't public - it's used by the reverse proxy and not external users.

You provide access to a pod by creating a `service`. (Pods could communicate using IP address but that's hard to discover and not reliable - when pods are replaced they get new addresses).

_Deploy a service for the homepage:_

```
kubectl apply -f ./k8s/homepage-service.yml
```

> This service type is `ClusterIP` - accessible to other pods in the cluster using the service name as DNS name.

---

## Create all the backend deployments

You can put multiple resource specifications in a single manifest.

There are three backend infrastructure components which have deployments and services specified - [SQL Server](./k8s/signup-db.yml), [ElasticSearch](./k8s/elasticsearch.yml) and [NATS](./k8s/message-queue.yml).

_Deploy all the backend components:_

```
kubectl apply -f ./k8s/signup-db.yml
kubectl apply -f ./k8s/elasticsearch.yml
kubectl apply -f ./k8s/message-queue.yml
```

---

## Check what's running

These are all much smaller images than the Windows versions and you have them all built locally so they should start quickly.

_See how the cluster is getting on with the deployments:_

```
kubectl get all
```

> You may see some deployments are not at capacity - that's just the time taken to spin up the containers

---

## Deploy message handlers

The message queue and databases are deployed, so we can deploy the message handlers.

There are simple deployments for the [save-handler](./k8s/save-handler.yml) and [index-handler](./k8s/index-handler.yml). They're background workers so there's no service for incoming connections.

```
kubectl apply -f ./k8s/save-handler.yml
kubectl apply -f ./k8s/index-handler.yml
```

---

## Check the logs

Both message handlers write log entries to confirm they're successfully listening on the queue. You can check the logs by selecting the components by their labels:

```
kubectl logs --selector app=signup,component=save-handler
kubectl logs --selector app=signup,component=index-handler
```

> You may see an error in the save-handler, if the pod starts before SQL Server is ready to accept clients. That's fine - the app fails, the container exits and Kubernetes will schedule a replacement pod.

---

## Deploy the front-end components

Two other components are published via the reverse proxy - the [sign up website](./k8s/signup-web.yml) (now a Blazor app), and the [reference data API](./k8s/reference-data-api.yml).

_Deploy these to create the pods and `ClusterIP` services:_

```
kubectl apply -f ./k8s/signup-web.yml
kubectl apply -f ./k8s/reference-data-api.yml
```

> These definitions don't include any Traefik labels. Traefik uses a different mechanism in Kubernetes.

---

## Deploy Traefik

Traefik runs as an [ingress controller](https://kubernetes.io/docs/concepts/services-networking/ingress-controllers/) in Kubernetes. That's a type of resource which receives external traffic and manages routing.

The exact same Traefik image which ran as a reverse proxy in Docker Swarm runs as an ingress controller in Kubernetes - although the [traefik.yml](./k8s/traefik.yml) setup is more complicated.

```
kubectl apply -f ./k8s/traefik.yml
```

> Traefik itself gets exposed as service of type `LoadBalancer`, which means it receives traffic from outside of the cluster

---

## Inspect `kube-system` resources

If you run `kubectl get all` now you won't see any Traefik resources, because they've been deployed into the Kubernetes system namespace.

Namespaces are a way of isolating resources, so you may have one namespace per app, or a shared namespace for common components.

_Check the system namespace to see the Traefik resources:_

```
kubectl get all -n kube-system
```

> Browse to http://localhost:8080 to see the Traefik configuration

---

## Deploy ingress rules

The ingress controller is the component which manages traffic, but you also need a one or more ingress resources to specify the routing.

[ingress.yml](./k8s/ingress.yml) is the simplest yet. It includes the Traefik rules to match incoming URL paths to other services, which lets ingress proxy the app and API.

```
kubectl apply -f ./k8s/ingress.yml
```

---

## Deploy Kibana

The last thing to run is the Kibana analytics front-end. We could route this through Traefik too, but we'll keep it separate and run on the standard port.

[kibana.yml](./k8s/kibana.yml) describes a deployment spec and a service of type `LoadBalancer`. You can mix ingress controllers and load balancer services in the same cluster, as long as they listen on different ports.

```
kubectl apply -f ./k8s/kibana.yml
```

---

## Deploy the whole app

Now the whole app is deployed. You wouldn't normally deploy components individually - and `kubectl` can deploy all the manifests in a folder.

You can deploy the whole app again from the `k8s` folder. Kubernetes will compare all the running resources with the manifests, and show that everything is `unchanged`:

```
kubectl apply -f ./k8s/
```

---

## Test the app

You can see the Traefik dashboard at http://localhost:8080/dashboard/, which should list all the routing rules from the ingress deployment.

Traefik is listening on standard HTTP port 80, so you can browse to the app too at http://localhost.

> Enter some data - everything should be working correctly.

---

## Check the data

The message handler pods will log output from processing messages:

```
kubectl logs --selector app=signup,component=index-handler
```

```
kubectl logs --selector app=signup,component=save-handler
```

And you can check Kibana at http://localhost:5601
