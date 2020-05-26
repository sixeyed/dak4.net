# Kubernetes 101

Kubernetes has many moving parts which run as separate containers. Deploying Kubernetes is not simple  (see [Kubernetes the Hard Way](https://github.com/kelseyhightower/kubernetes-the-hard-way)).

It's a complex platform but all the major cloud providers offer a managed Kubernetes service which removes a lot of the complexity - and makes your apps portable across the data centre and any cloud.

That's one reason why Kubernetes is so popular, and why it's worth investing in.

---

## Kubernetes in Docker Desktop

Docker Desktop is the easiest way to run Kubernetes locally, it spins up a single-node cluster in a VM and manages it for you.

This is a Linux-only cluster, so we can run .NET Core apps in Linux containers, but not .NET Framework apps in Windows containers.

---

## Clean up from earlier

First we'll clean up all the running Windows containers.

_Delete all containers:_

```
docker container rm -f $(docker container ls -aq)
```

---

## Enable Kubernetes

Kubernetes isn't deployed by default. You install it by right-clicking the whale icon, selecting _Settings_, then  _Kubernetes_ and checking _Enable Kubernetes_:

![Enabling Kubernetes in Docker Desktop](/img/screenshots/enable-kubernetes.png)

> You only see this option in Linux container mode. It will take a while to download and start all the components.

---

## Checking Kubernetes

Kubernetes uses a container runtime like Docker to run containers, so you work with the Kubernetes API and it works with the Docker API.

You use the `kubectl` command line to work with Kubernetes:

```
kubectl get nodes
```

> This is a single node cluster, but you work with clusters of any size in the same way.

---

## Understanding Pods

The basic unit of work in Kubernetes is not a container - containers are wrapped in _Pods_. One [Pod](https://kubernetes.io/docs/concepts/workloads/pods/pod-overview/) hosts one or more containers, and they share the same network and filesystem space.

Pods are responsible for keeping their container(s) running - if the app process ends and the container exits, then the Pod replaces it with a new container.

---

## Creating Pods

You deploy apps to Kubernetes by appying YAML files that describe the desired state. We'll start with a simple [Pod that runs a sleep container](./k8s/101/sleep-pod.yaml).

_Run a simple application in a pod:_

```
kubectl apply -f ./k8s/101/sleep-pod.yaml
```

---

## Check the Pod

Kubectl is for managing resources, and has a standard set of commands which you can use with different types  of object.

_Show the basic details of the Pod, and then the full information:_

```
kubectl get pods 

kubectl describe pod sleep
```

---

## Manage the Pod

You can interact with the app container in the Pod, using similar commands you use in Docker. 

- `kubectl exec` is like `docker exec`, running a command in the Pod container.
- `kubectl logs` is like `docker logs`, showing the output from the container process.

```
kubectl exec sleep -- hostname

kubectl logs sleep
```

---

## Pods manage containers

The Pod is a layer of abstraction over the container, which manages the container for you. 

If the container is removed, or the container process exits, the Pod restarts by creating a new container.

_Cause the sleep container to exit by killing its processes:_

```
docker container ls

kubectl exec sleep -- killall5

docker container ls

kubectl get pod sleep
```

---

## What manages Pods?

A Pod is assigned to a single node in the Kubernetes cluster. 

The Pod we've created doesn't have any other objects managing it, so if the Pod is deleted it won't be replaced.

_Delete the Pod and check what's running:_

```
kubectl delete pod sleep

kubectl get pods

docker container ls
```

---

## Understanding Deployments

You don't usually create Pods directly like this - you create a _controller_ which manages Pods for you.

The `Deployment` is the most common type of controller. Deployments are responsible for running a specified number of Pods, and they have management features for rolling out application updates.

Deployments contain a Pod spec which they use as a template to create Pods. This [Deployment spec](./k8s/101/sleep-deployment.yaml) runs the same sleep app.

```
kubectl apply -f ./k8s/101/sleep-deployment.yaml
```

---

## Navigation by label

Deployments add their own identifiers to Pod names, so it's hard to work with the Pod by name.

Kubernetes stores metadata for objects, which can include _labels_. Labels are key-value pairs which can contain anything that helps you work with the object.

_Show the labels for the new Pod and use a label selector to print logs:_

```
kubectl get pods --show-labels

kubectl logs -l app=sleep
```

> Labels are how Deployments find the Pods they own

---

## Deployments manage Pods

A Deployment is another abstraction in the compute layer, sitting on top of Pods. Pods ensure their container(s) are running, and Deployments ensure the right number of Pods are running.

_Delete the Pod and the Deployment creates a replacement:_

```
kubectl delete pods -l app=sleep

kubectl get pods --show-labels

kubectl logs -l app=sleep
```

---

## That's the compute layer

Well, not really. Deployments actually manage another type of controller called a ReplicaSet, and it manages Pods. There are other Pod controllers for different types of application too.

Deployments are the ones you'll use most frequently, and now we've had an introduction to how they work, how you define them, how Kubernetes uses labels, and how you use Kubectl to manage apps. 
