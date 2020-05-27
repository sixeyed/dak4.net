# Deploying to Kubernetes

We've used `kubectl` to create and Kubernetes resources defined in YAML files, called _manifests_.

Manifests can be used to describe every part of your app, from the application and its configuration to the network traffic and the storage components you need.

---

## Kubernetes manifests

Kubernetes uses its own [API specification](https://kubernetes.io/docs/reference/generated/kubernetes-api/v1.18/), different from Docker Compose. It's much more verbose because there are more resource types; the relation between resources needs to be specified; and the resources themselves have more flexible configuration.

Kube manifests are typically many times larger than the equivalent Docker Compose manifests which Docker Swarm uses.They still use the declarative approach - you deploy the manifest to the cluster, and Kubernetes works out what resources to create or update.

---

## Create all the backend deployments

You can put multiple resource specifications in a single manifest.

There are three backend infrastructure components which have Deployments and Services specified - SQL Server in [signup-db.yml](./k8s/signup-db.yml), ElasticSearch in [elasticsearch.yml](./k8s/elasticsearch.yml) and NATS in [message-queue.yml](./k8s/message-queue.yml).

_Deploy all the backend components:_

```
kubectl apply -f ./k8s/signup-db.yml -f ./k8s/elasticsearch.yml -f ./k8s/message-queue.yml
```

---

## Check what's running

These are all much smaller images than the Windows versions and you have them all built or pulled locally so they should start quickly.

_See how the cluster is getting on with the deployments:_

```
kubectl get all -l app=signup
```

> You may see some deployments are not at capacity - that's just the time taken to spin up the containers

---

## Deploy message handlers

The message queue and databases are deployed, so we can deploy the message handlers.

There are simple Deployments for the handlers - [save-handler.yml](./k8s/save-handler.yml) and [index-handler.yml](./k8s/index-handler.yml). They're background workers so there's no Service for incoming connections.

```
kubectl apply -f ./k8s/save-handler.yml -f ./k8s/index-handler.yml
```

---

## Check the logs

Both message handlers write log entries to confirm they're successfully listening on the queue. You can check the logs by selecting the components by their labels:

```
kubectl logs --selector app=signup,component=save-handler
```

```
kubectl logs --selector app=signup,component=index-handler
```

> You may see an error in the save-handler, if the Pod starts before SQL Server is ready to accept clients. That's fine - the app fails, the container exits and Kubernetes restarts the Pod

---

## Deploy the front-end components

Two other components are published via the reverse proxy - the website in [signup-web.yml](./k8s/signup-web.yml) (now a Razor app), and the API in [reference-data-api.yml](./k8s/reference-data-api.yml).

_Deploy these to create the pods and `ClusterIP` services:_

```
kubectl apply -f ./k8s/signup-web.yml -f ./k8s/reference-data-api.yml
```

> These definitions don't include any Traefik labels. Traefik uses a different mechanism in Kubernetes.

---

## Deploy Traefik

Traefik runs as an [ingress controller](https://kubernetes.io/docs/concepts/services-networking/ingress-controllers/) in Kubernetes. That's a type of resource which receives external traffic and manages routing.

The exact same Traefik image which ran as a reverse proxy in Docker runs as an ingress controller in Kubernetes - although the [traefik.yml](./k8s/traefik.yml) setup is more complicated.

```
kubectl apply -f ./k8s/traefik.yml
```

> Traefik itself gets exposed as service of type `LoadBalancer`, which means it receives traffic from outside of the cluster

---

## Inspect `kube-system` resources

If you run `kubectl get all` now you won't see any Traefik resources, because they've been deployed into the Kubernetes system namespace.

Namespaces are a way of isolating resources, so you may have one namespace per app, or per environment, or a shared namespace for common components.

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

```
kubectl describe ingress signup
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

Now everything is deployed. You wouldn't normally deploy components individually - and `kubectl` can deploy all the manifests in a folder.

You can deploy the whole app again from the `k8s` folder. Kubernetes will compare all the running resources with the manifests, and show that everything is `unchanged`:

```
kubectl apply -f ./k8s/
```

```
kubectl logs -l app=signup
```

---

## Test the app

You can see the Traefik dashboard at http://localhost:8080/dashboard/, which should list all the routing rules from the ingress deployment.

Traefik is listening on standard HTTP port 80, so you can browse to the app too at http://localhost.

> Enter some data - everything should be working correctly.

---

## Check the message flow

The message handler pods will log output from processing messages.

```
kubectl logs --selector app=signup,component=index-handler
```

```
kubectl logs --selector app=signup,component=save-handler
```

---

## And the data

You can check Kibana at http://localhost:5601, creating a view for the `prospects` index.

You can also query SQL Server, but this is the Linux version so the command is different.

_Use the `sqlcmd` tool in the Linux image:_ 

```
kubectl exec deploy/signup-db -- /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P "DockerCon!!!" -Q "SELECT * FROM Prospects" -d SignUp -W
```

---

## We're up and running in Kubernetes!

We could spin up a Kubernetes cluster in AKS (or EKS or GKE) and deploy the app using the exact same manifests.

The only difference is availability and scale. On a multi-node cluster the Pods would be distributed around different servers, but the networking is the same and traffic gets routed to Pods whichever node they run on.

We're not done though. This is the minimum setup for a PoC, but the app needs work before it's production-ready.