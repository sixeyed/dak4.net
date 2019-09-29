# Deploying Stacks to Docker Swarm

---

Now we're ready to deploy the real application to the swarm.

> In a production swarm you'd have multiple managers and workers, but the workflow is the same - connecting to a manager node and running the same `docker` commands.

---

## But first, tidy up

Remove all the running services, which will also remove their containers:

```
docker service rm -f $(docker service ls -q)
```

---

## Compose files in swarm mode

You deploy apps to swarm using Docker Compose files. There are some attributes which only apply to swarm mode (like the `deploy` section), and some which are ignored in swarm mode (like `depends_on`).

You can combine multiple compose files to make a single file. That's useful for keeping the core solution in one compose file like [v11-core.yml](./app/v11-core.yml), and adding environment-specific overrides in other files like [v11-dev.yml](./app/v11-dev.yml) and [v11-prod.yml](./app/v11-prod.yml).

---

## Generate the application manifest

This joins together the core and production compose files.

```
cd $env:workshop

docker-compose `
  -f .\app\v11-core.yml `
  -f .\app\v11-prod.yml config > docker-stack.yml
```

> The generated `docker-stack.yml` file contains the merged contents, ready for deployment. It also uses [Docker config objects](https://docs.docker.com/engine/swarm/configs/) and [Docker secrets](https://docs.docker.com/engine/swarm/secrets/).

---

## Storing config in the swarm

A Docker Swarm cluster does more than just manage containers. There's a resilient, encrypted data store in the cluster which you can use with your containers.

Communication between swarm nodes is encrypted too, so you can safely store confidential data like passwords and keys in the swarm.

Docker surfaces config data as files inside the container, so it's all transparent to your app.

---

## Create the config object

There are two ways to store configuration data in Docker swarm. You use config objects for data which isn't confidential.

_Store the [log4net.config](./app/configs/log4net.config) file in the swarm:_

```
docker config create `
  netfx-log4net `
  ./app/configs/log4net.config
```

---

## Check the config object

Configs aren't secret, so you can read the values back out of the swarm.

_Check the config object is stored:_

```
docker config inspect --pretty netfx-log4net
```

> This is an XML config file. You can store any type of data in the swarm.

---

## Create the secret

Secrets are stored encrypted, but the API is very similar to config objects.

_Store the [connectionStrings.config](./app/secrets/connectionStrings.config) file in the swarm:_

```
docker secret create `
  netfx-connectionstrings `
  ./app/secrets/connectionStrings.config
```

---

## Check the secret object

Secrets are secret, you cannot read the original plain text.

_Check the secret object is stored:_

```
docker secret inspect --pretty netfx-connectionstrings
```

> It's still XML, but it's only delivered as plain text inside the container that needs it.

---

## Deploy the application as a stack

Stacks are a way to group all the parts of an app. You deploy them using Docker Compose files.

Deploy the stack:

```
docker stack deploy -c docker-stack.yml signup
```

> Docker creates all the resources in the stack: an overlay network, and a set of services. It will deploy service tasks across the swarm, on a multi-node cluster you would see containers running on different nodes.

---

## Managing the stack

Application stacks are first-class object in swarm mode. You can see the stacks which are running, and the services which are in the stack:

```
docker stack ls

docker stack ps signup
```

> You can navigate around the services, and make changes to the deployment. But your stack file is the source of truth, which lets you work declaratively.

---

## High availability and scale in swarm mode

The swarm keeps your app running at the desired service level. You can manualy remove containers from worker nodes, have workers leave the swarm, or even stop the worker VMs - Docker will keep the app running.

You can add more nodes to the swarm just by running the `swarm join` command, and immediately add capacity.
