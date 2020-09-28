# Kubernetes 101 - Networking

---

Pods have an IP address assigned by Kubernetes and every Pod can communicate with other Pods by IP address.

Pods are not permanent though, they get replaced if they fail and during app updates, and new Pods get new IP addresses.

Kubernetes has a DNS server running in the cluster, just like Docker, but DNS names are managed in Kubernetes using dedicated objects - [Services](https://kubernetes.io/docs/concepts/services-networking/service/).

---

## Services and DNS names

Services are a networking abstraction. A Service has a static IP address of its own, and it's loosely coupled to Pods using a label selector.

A Service can be the front end to zero or many Pods. If there are multiple Pods, the traffic gets load-balanced between them, but that's transparent to the consumer which always uses the Service name and IP address. 

---

## Pods with labels

The simplest part of our workshop app is the homepage. The Kubernetes manifest for that is in [homepage-pod.yml](./k8s/homepage-pod.yml).

It describes a deployment, which will create a ReplicaSet, which will manage pods. The pod will run a Docker container hosting the homepage.

_Deploy the manifest:_

```
kubectl apply -f ./k8s/homepage-pod.yml
```

> The label selector is used by the Deployment, and it can also be used by a Service

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

> Labels and selectors are used to link resources, and they can also be used for management

---

## A Service for the pod

The homepage pod isn't public - it's used by the reverse proxy and not external users.

The reverse proxy will access the homepage using the Service defined in [homepage-service.yml](./k8s/homepage-service.yml).

_Deploy the Service for the homepage:_

```
kubectl apply -f ./k8s/homepage-service.yml
```

> This service type is `ClusterIP` - only accessible to other Pods in the cluster

---

## Service and Pods

The Service is loosely coupled to Pods using labels - the label selector for the Service is the same one we can use to work with the Pod.

_Check the IP addresses in the Service and Pod:_


```
kubectl get endpoints homepage
```

```
kubectl get pods -l app=signup,component=homepage -o wide
```

---

## Pod replacement

If the Pod gets replaced, the Service sees that and removes the old Pod's IP address from its list, replacing it with the new Pod's address.

```
kubectl delete pod -l app=signup,component=homepage 
```

```
kubectl get pods -l app=signup,component=homepage -o wide
```

```
kubectl get endpoints homepage
```

---

## Pod scale-up

The ReplicaSet for the homepage Deployment is configured to use a single replica, which means it makes sure one Pod is running in the cluster.

_Scale up and the ReplicaSet creates more Pods from the template; they have the same labels so they get added to the Service too:_

```
kubectl scale deploy/homepage --replicas 3
```

```
kubectl get pods -l app=signup,component=homepage -o wide
```

```
kubectl get endpoints homepage
```

---

## Pod-to-Pod communication

Kubernetes has a flat networking model, any Pods can communicate with any other Pod.

_Use the sleep app to check on the homepage:_

```
kubectl exec deploy/sleep -- nslookup homepage
```

```
kubectl exec deploy/sleep -- sh -c 'curl -v -s --head http://homepage'
```

> There's no equivalent to Docker networks, all Pods are connected to one big network
