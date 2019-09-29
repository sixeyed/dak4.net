# Kubernetes 101

## Clean up from swarm mode

docker stack rm $(docker stack ls)

## Switch to Linux containers

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




