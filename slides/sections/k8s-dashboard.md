# The Kubernetes Dashboard

---

## Dashboard

kubectl apply -f .\k8s\dashboard\kube-dashboard.yml

kubectl -n kubernetes-dashboard describe secret

- copy token

kubectl proxy

http://localhost:8001/api/v1/namespaces/kube-system/services/https:kubernetes-dashboard:/proxy/

login with token
