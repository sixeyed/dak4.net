# Container Orchestration

---

Orchestration is the fancy name for running containers on a cluster of servers. Each machine in the cluster can run Docker containers, and the orchestrator manages the cluster as a whole.

You run applications by deploying a manifest which describes the containers that make up your app. The cluster decides which servers will run the containers, and it monitors them to maintain your service levels.

---

<section data-background-image="/img/orchestration/overview.png">

---

## Docker Swarm and Kubernetes

These are the main container orchestrators. Swarm is built into Docker, Kubernetes is a separate deployment. Both run apps as Docker containers and they use the same images.

Swarm uses the Docker Compose format to describe apps for the cluster. Kubernetes uses its own manifest format, also written in YAML.

---

## Architecture

Swarm and Kubernetes both use a master-worker architecture. The master nodes control the cluster, scheduling containers to run on worker nodes and monitoring the nodes.

Workers run application containers. Kubernetes and Swarm both support mixed clusters, with Linux and Windows worker nodes.  Swarm also supports an all-Windows cluster, but Kubernetes clusters must have Linux manager nodes.

---

## Managed Platforms
 
The major clouds all provide a managed Kubernetes service: [AKS](https://docs.microsoft.com/en-us/azure/aks/), [EKS](TODO) and [GKE](TODO) are the most popular. 

You can run a Swarm cluster in the cloud but not as a managed service, you will need to provision and manage VMs.

> Windows support in Kubernetes is in early stages. The major clouds support Windows nodes in preview versions of their managed Kubernetes services.

---

## Learning Path

Kubernetes is flexible and hugely powerful, but complex to learn. Docker Swarm is an opinionated orchestrator with fewer features, but it is much simpler to use.

It's a good idea to get experience with Swarm, even if you plan on using Kubernetes in production. It's easy to migrate from Docker Compose to Docker Swarm, and it's easier to migrate from Docker Swarm to Kubernetes than to learn Kubernetes from scratch.

---

## Core Concepts

We'll cover the major orchestration concepts using Docker Swarm and Kubernetes:

- application deployments and updates
- high availability for containers
- service availability and discoverability
- application healthchecks
- configuration, logging and monitoring
