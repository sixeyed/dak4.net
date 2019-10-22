# The Kubernetes Dashboard

---

Kubernetes has a standard web dashboard which you deploy as a Kube app with a YAML manifest in the usual way.

Most managed Kubernetes services support the dashboard, but they don't have it permanently enabled. That's because the dashboard lets you edit deployments as well as viewing them.

We can run the dashboard on Docker Desktop with the same behaviour you would get with AKS.

---

## Deploy the dashboard

The [kube-dashboard.yml](./k8s/dashboard/kube-dashboard.yml) specs the container to run for the web app, together with the [RBAC permissions](https://kubernetes.io/docs/reference/access-authn-authz/rbac/) needed for it to manage resources.

There's also a [ServiceAccount](https://kubernetes.io/docs/tasks/configure-pod-container/configure-service-account/) which we'll use to authenticate with the dashboard.

_Deploy the dashboard:_

```
kubectl apply -f ./k8s/dashboard/kube-dashboard.yml
```

---

## Get the auth token

You'll need to log in to the dashboard. It supports different authentication mechanisms, the easiest is to grab the token for a service account.

_Get the details of the account's secret:_

```
kubectl -n kubernetes-dashboard describe secret
```

> Copy the value of the `token` fied to the clipboard - you'll use it in the dashboard

---

## Expose the port

You can leave the dashboard running, but it's not a good idea to make it externally available all the time. There's no `LoadBalancer` service in the app manifest.

Instead you can use Kubernetes to proxy requests through your `kubectl` session, so the dashboard is only available form your local machine.

_Open the proxy:_

```
kubectl proxy
```

---

## Open the dashboard

Browse to the dashboard via your local proxy and login with the token:

> http://localhost:8001/api/v1/namespaces/kube-system/services/https:kubernetes-dashboard:/proxy/

You'll see a useful interface which lets you navigate around most Kubernetes resources, view and edit the YAML, and check logs from pods.
