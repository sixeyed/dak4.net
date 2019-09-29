# Kubernetes 101

Kubernetes has many moving parts which run as separate containers. Deploying Kubernetes is not as simple as `docker swarm init`.

> See [Kubernetes the Hard Way](https://github.com/kelseyhightower/kubernetes-the-hard-way)

## Kubernetes in Docker Desktop

Docker Desktop is the easiest way to run Kubernetes locally, it spins up a single-node cluster for you.

This is a Linux-only cluster, so we can run .NET Core apps in Linux containers, but not .NET Framework apps in Windows containers.

## Clean up from swarm mode

First we'll clean up all the running swarm services.

_Delete all swarm stacks:_

```
docker stack rm $(docker stack ls)
```

## Switch to Linux containers

Right-click the Docker whale icon in the taskbar, and select _Switch to Linux containers_

!["/img/screenshots/linux-containers.png"]

## Enable Kubernetes

## Creating pods

```
kubectl run pinger --image debian:9.11 --command ping -- localhost
```

## Not just pods

creates deployment, replica set, pod

```
kubectl get all
```

## Managing Kubernetes resources

kubectl describe deployment/pinger

kubectl describe replicaset

kubectl describe pods

```
kubectl logs ...
```

```
docker container ls
```

## Scale

kubectl scale --replicas=3 deployment/pinger

kubectl get pods

kubectl get logs?

## Get logs by label

kubectl describe pod...

kubectl logs --selector run=pinger

## Updating deployments


kubectl set image deployment/pinger pinger=debian:10.1

kubectl get all

docker container ls --all

kubectl delete deployment/pinger

## Examining rollouts

kubectl describe deployment/pinger

kubectl rollout history deployment/pinger


## Rolling back

kubectl rollout undo deployment/pinger

kubectl rollout history deployment/pinger


## Deleting pods

kubectl delete pods --all --force --grace-period=0

kubectl get all


## Deleting replica sets

kubectl delete rs --all --force --grace-period=0

kubectl get all

## Deleting deployments

kubectl delete deployment pinger

kubectl get all


## Deployments, Replica Sets and Pods

![]




