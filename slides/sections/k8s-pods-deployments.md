# Kubernetes 101

Kubernetes has many moving parts which run as separate containers. Deploying Kubernetes is not as simple as `docker swarm init`.

> See [Kubernetes the Hard Way](https://github.com/kelseyhightower/kubernetes-the-hard-way)

---

## Kubernetes in Docker Desktop

Docker Desktop is the easiest way to run Kubernetes locally, it spins up a single-node cluster in a VM and manages it for you.

This is a Linux-only cluster, so we can run .NET Core apps in Linux containers, but not .NET Framework apps in Windows containers.

---

## Clean up from swarm mode

First we'll clean up all the running swarm services.

_Delete all swarm stacks:_

```
docker stack rm $(docker stack ls)
```

---

## Switch to Linux containers

Right-click the Docker whale icon in the taskbar, and select _Switch to Linux containers_

![Switching to Linux containers in Docker Desktop](/img/screenshots/linux-containers.png)

---

## Enable Kubernetes

Kubernetes isn't deployed by default. You install it by right-clicking the whale icon, selecting _Settings_ and then enabling Kubernetes:

![Enabling Kubernetes in Docker Desktop](/img/screenshots/enable-kubernetes.png)

> You only see this option in Linux container mode

---

## Checking Kubernetes

Kubernetes uses Docker to run containers, but it uses the Docker API - you don't (usually) interact with Docker directly.

Instead you use the `kubectl` command line:

```
kubectl get nodes
```

---

## Understanding pods

The basic unit of work in Kubernetes is not a container - containers are wrapped in _pods_. One [pod](https://kubernetes.io/docs/concepts/workloads/pods/pod-overview/) hosts one or more containers, and they share the same network, process and memory space.

This is the fundamental different between Kubernetes and other orchestrators. It enables some powerful scenarios, like [sidecar containers](https://kubernetes.io/blog/2015/06/the-distributed-system-toolkit-patterns/).

---

## Creating pods

You deploy apps to Kubernetes by appying manifest files - this is the declarative approach. We'll explore the basics using the imperative approach, but that's not recommended for real work.

_Run a simple application in a pod:_

```
kubectl run pinger --image debian:9.11 --command ping -- localhost
```

---

## More than just a pod

The pod is the basic unit of work in Kubernetes, but you can't create a pod by itself.

Pods are managed by other resources called _controllers_, which let you work at different layers of abstraction.

_Check the resources Kubernetes created:_

```
kubectl get all
```

---

## Pods, ReplicaSets and Deployments

The `Deployment` resource is a type of _controller_ - it describes the desired state of other resources. Kubernetes monitors the cluster to ensure the desired state of deployments is met.

A `ReplicaSet` manages a replicated set of pods. When you scale up in Kubernetes you get multiple pods, and that's managed by the ReplicaSet. The ReplicaSet is managed by the Deployment :)

---

## Managing Kubernetes resources

`kubectl` has a standard set of verbs to work with resources. Most can return a YAML or JSON response to support automation.

The `describe` command gives you a human-readable description of one or more resources:

```
kubectl describe deployment/pinger

kubectl describe replicaset

kubectl describe pods

```

---

## Checking logs

All the management of your Docker containers can be done with Kubernetes. You view the container logs from the pod resource.

_You can use templates with `kubectl` to get specific fields. This gets the pod name and shows its logs:_

```
$pod=$(kubectl get pods --template '{{range .items}}{{.metadata.name}}{{end}}')

kubectl logs $pod
```

---

## Pod logs _are_ container logs

Kubernetes is managing all the containers in the cluster, but in a single-node cluster those containers are all on your machine.

_You can also manage them with Docker:_

```
$container=$(docker container ls --format '{{.ID}}')

docker container logs $container
```

---

## Scale

Kubernetes generated a ReplicasSet with a scale of one, so you have one pod running.

_Scale up by specifying more replicas:_

```
kubectl scale --replicas=3 deployment/pinger

kubectl get pods
```

---

## Get logs by label

There's no option to fetch all the logs for a deployment or a ReplicaSet. Instead you can fetch logs for all pods using a label selector.

```
kubectl logs --selector run=pinger
```

> The `run=pinger` label was applied by the deployment. Labels and selectors are the basic mechanism for decoupling resources.

---

## Updating deployments

You started your pod with `kubectl run` but Kubernetes created a deployment resource for you. You can use that deployment to update the running app.

_Set a new image to upgrade to the latest Debian image:_

```
kubectl set image deployment/pinger pinger=debian:10.1
```

---

## Check the update

Changes to the pod specification are rolled out across the cluster. When you check running pods you may find them in different states while the deployment stabilises.

_Check your running pods and containers:_

```
kubectl get all

docker container ls --all
```

---

## Working with rollouts

You can manage deployment updates with the `rollout` commands. Kubernetes maintains the history of changes.

_Check the rollout history:_

```
kubectl rollout history deployment/pinger
```

> You can add notes to updates which would be shown here

---

## Rolling back

Rolling back an update is a one-line command, and this will be a gradual rollback.

_Rollback the update and check the history:_

```
kubectl rollout undo deployment/pinger

kubectl rollout history deployment/pinger
```

> Kubernetes can maintain longer histories than Docker Swarm

---

## Deleting pods

You can delete resources at different levels, but you may not get the results you expect.

_Try deleting all the pods for the app:_

```
kubectl delete pods --all --force --grace-period=0

kubectl get all
```

> They were recreated, because the ReplicaSet still exists and its job is to keep three pods running

---

## Deleting replica sets

Resources have cascading deletes, so if we delete the ReplicaSet that manages the pods, it will delete all the pods too.

_Won't it?_

```
kubectl delete rs --all --force --grace-period=0

kubectl get all
```

> Yes, but the Deployment resource still exists, and its job is to keep the ReplicaSet deployed

---

## Deleting deployments

So the deployment is the top-level resource. Deleting that will delete the ReplicaSet, which in turn deletes the pods.

_Finally:_

```
kubectl delete deployment pinger

kubectl get all
```
