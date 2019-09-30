# Deploying to Kubernetes

So far we've used `kubectl` to imperatively create and remove Kubernetes resources. You wouldn't do that for anything other than quick exploration.

For everything else you describe the components of your app in Kubernetes manifests. This is the declarative approach - you deploy the manifest to the cluster, and Kubernetes works out what resources to create.

---

## Kubernetes manifests

Kubernetes uses its own manifest spec, different from Docker Compose. It's much more verbose, because there are more resource types; the relation between resources needs to be specified; and the resources themselves have more flexible configuration.

Kube manifests are typically many times larger than the equivalent Docker Compose manifests which Docker Swarm uses.

---


kubectl apply -f ./k8s/homepage-pod.yml

kubectl port-forward deployment/homepage 8090:80

http://localhost:8090

kubectl logs --selector app=signup,component=homepage

---

clusterip

kubectl apply -f ./k8s/homepage-service.yml

---

backend infra

svc & pod

kubectl apply -f ./k8s/signup-db.yml
kubectl apply -f ./k8s/elasticsearch.yml
kubectl apply -f ./k8s/message-queue.yml

---

kubectl apply -f ./k8s/save-handler.yml
kubectl apply -f ./k8s/index-handler.yml

kubectl logs --selector app=signup,component=save-handler
kubectl logs --selector app=signup,component=index-handler

---

kubectl apply -f ./k8s/signup-web.yml
kubectl apply -f ./k8s/reference-data-api.yml

---

kubectl apply -f ./k8s/traefik.yml

kubectl get all -n kube-system

http://localhost:8080

---

kubectl apply -f ./k8s/ingress.yml

---

kubectl apply -f ./k8s/kibana.yml

---

kubectl apply -f ./k8s/

---

http://localhost:8080/dashboard/

http://localhost

