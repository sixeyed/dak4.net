# Production Readiness - Config

---

A key benefit of containers is that you deploy the same image in every environment, so what's in production is exactly what you tested.

You should package images with a default set of config for running in dev, but you need a way to inject configuration from the environment into the container.

We'll do this next, extending the web image to read external configuration.

---

## Config in .NET Core

- default: appsettings.json, env
- custom providers

[Program.cs]()

---

## Configuration in Kubernetes

- env
- configmap
- secrets

---

## Environment variables in Kube manifests

kubectl apply -f ./k8s/prod-config/reference-data-api.yml

---

## Storing config data in ConfigMaps

kubectl create configmap save-handler-config --from-file=./k8s/prod-config/configMaps/save-handler/config.json

---

## Loading ConfigMaps into pods

kubectl apply -f ./k8s/prod-config/save-handler.yml

kubectl describe pod --selector='component=save-handler'

exec

---

## Managing ConfigMaps

kubectl get configmap

kubectl describe configmap save-handler-config

---

## Storing sensitive data in Secrets

kubectl create secret generic signup-web-secret --from-file=./k8s/prod-config/secrets/signup-web/secret.json

---

## Loading Secrets into pods

kubectl apply -f ./k8s/prod-config/signup-web.yml

kubectl describe pod --selector='component=web'

---

## Managing Secrets

kubectl get secrets

kubectl describe secret signup-web-secret

$secret = $(kubectl get secret signup-web-secret -o 'go-template={{index .data `secret.json`}}')

[System.Text.Encoding]::Ascii.GetString([System.Convert]::FromBase64String(\$secret))

---

## Overwriting default config

Your containerized apps should have default configuration settings bundled in the image, so the team can use `docker container run` without needing any extra setup.

But you need to be able to inject external configuration data into your container at runtime, which is what this pattern does. The config source is configurable, so it can come from the files in the image, or from the container platform.

> TODO - depends on app, .netfx needs different approach

We'll see that in action shortly.
