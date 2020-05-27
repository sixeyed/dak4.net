# Production Readiness - Config

A key benefit of containers is that you deploy the same image in every environment, so what's in production is exactly what you tested.

You should package images with a default set of config for running in dev, but you need a way to inject configuration from the environment into the container.

We'll explore the options Kubernetes provides for that.

---

## Config in .NET Apps

.NET Core supports loading config from multiple sources. The default is to start with a local `appsettings.json` file, and then load environment variables which can override individual settings.

That works fine for containers, and you can extend it to give more control to the container platform. [Program.cs]() does this, adding two providers after the environment variables - `config.json` and `secret.json`. They'll be empty in a local run, but they can be populated by the platform.

> You can do a similar thing with [.NET Framework config files](https://anthonychu.ca/post/overriding-web-config-settings-environment-variables-containerized-aspnet-apps/).

---

## Configuration in Kubernetes

Docker and Kubernetes supply app configuration by setting up the container environment. Kubernetes has three options for that:

- environment variables set in the Pod spec
- [ConfigMaps](https://kubernetes.io/docs/concepts/configuration/configmap/) surfaced as files or environment variables
- [Secrets](https://kubernetes.io/docs/concepts/configuration/secret/) surfaced as files or environment variables

---

## Environment variables in Pod specs

Environment variables are the simplest. They're fine for individual values which are different between environments.

The updates [reference-data-api.yml](./k8s/prod-config/reference-data-api.yml) uses an environment variable for the database connection string. Not a great option but simple to demonstrate (Kubernetes uses double-underscores as separators instead of the usual colons).

_Deploy the API - its behaviour is the same:_

```
kubectl apply -f ./k8s/prod-config/reference-data-api.yml

kubectl get pods -l component=api
```

> This is a new Pod because the spec has changed

---

## Environment variables are not safe

They're simple to use, but you should avoid them for any sensitive data.

_You can read all the environment variables from `kubectl`_:

```
kubectl describe pod -l component=api
```

> Apps might print out all environment variables during a crash too

---

## Storing config data in ConfigMaps

ConfigMaps are more flexible. You can create them from files so they can contain JSON, XML or key-value pairs - whatever your app needs.

They also decouple the app definition from the actual config, so the app spec file can live in GitHub but the config specs could be managed by ops in a separate system.

_Deploy a ConfigMap for the save handler [save-handler-configMap.yml](./k8s/prod-config/configMaps/save-handler-configMap.yml):_

```
kubectl apply -f ./k8s/prod-config/configMaps/save-handler-configMap.yml

kubectl get configmaps
```

---

## Loading ConfigMaps into Pods

ConfigMaps can be loaded into pods as volume mounts. The updated [save-handler.yml](./k8s/prod-config/save-handler.yml) loads the contents of the config volume into the path `/app/config` in the container.

The `config` volume is loaded from the `save-handler-config` ConfigMap, so that `config.json` file is materialized into the container in the location the app is expecting.

_Deploy the handler using the ConfigMap:_

```
kubectl apply -f ./k8s/prod-config/save-handler.yml

kubectl exec deploy/save-handler -- cat /app/configs/config.json
```

---

## ConfigMaps can be locked down

You can't see the contents of a ConfigMap by describing the pod - you can only see that a ConfigMap is used and where it gets mounted.

_This doesn't show you the database connection string:_

```
kubectl describe pod -l component=save-handler
```

> But you can read it if you have access to the Pod

---

## Managing ConfigMaps

Kubernetes supports [role-based access control](https://kubernetes.io/docs/reference/access-authn-authz/rbac/) for object management, which lets you secure access. Roles could have permission to use Pods, but not ConfigMaps.

If you have permission you read the contents of a ConfigMap:

```
kubectl describe configmap save-handler-config
```

> RBAC is not deployed by default in most Kubernetes distros

---

## Storing sensitive data in Secrets

Secrets are better for sensitive data like connection strings and API keys. They work like ConfigMaps but they can be encrypted at rest in the cluster database, and in transit to the nodes.

_Deploy a Secret for the web app defined in [signup-web-secret.yml](./k8s/prod-config/secrets/signup-web-secret.yml):_

```
kubectl apply -f ./k8s/prod-config/secrets/signup-web-secret.yml
```

---

## Loading Secrets into pods

Secrets can be the source for volume mounts too. The updated [signup-web.yml](./k8s/prod-config/signup-web.yml) uses a secret for the database connection string.

The secret `signup-web-secret` gets loaded into the path `/app/secrets`. There's one file in the secret, so the app finds it at `/app/secrets/secret.json`.

_Deploy the new web app using the secret:_

```
kubectl apply -f ./k8s/prod-config/signup-web.yml
```

---

## Secrets aren't shown

Describing the pod will show you which secrets are loaded and where they're surfaced, but not the contents of the secret.

_You'll see here there's a default Kubernetes secret too:_

```
kubectl describe pod -l component=web
```

> Anyone with `exec` permissions can still read the secret in the pod

---

## Managing Secrets

Secrets can be encrypted at rest, they get surfaced as plaintext inside pods, and they're base-64 encoded through the Kubernetes API.

You can list and describe secrets - subject to RBAC permissions.

_You won't see the plain text though:_

```
kubectl get secrets -l dak4dotnet

kubectl describe secret signup-web-secret
```

---

## Decoding Secrets

Base-64 encoding just adds obscurity to stop easy leaking of secrets. This isn't encryption so if someone has access to read secrets they can easily get the plain text.

_Linux and Windows shells support base-64 decoding:_

```
kubectl get secret signup-web-secret -o 'go-template={{index .data `secret.json`}}'

$secret = $(kubectl get secret signup-web-secret -o 'go-template={{index .data `secret.json`}}')

[System.Text.Encoding]::Ascii.GetString([System.Convert]::FromBase64String($secret))
```

---

## Overwriting default config

Your containerized apps should have default configuration settings bundled in the image, so the dev team can use `docker container run` without needing any extra setup.

But you need to be able to inject external configuration data into your container at runtime, which is what this pattern does. The config source is configurable, so it can come from the files in the image, or from the container platform.
