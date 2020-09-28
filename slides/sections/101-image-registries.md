# Docker Images and Registries

---

Images are the portable package that contains your application - your binaries, all the dependencies and the default configuration.

You share images by pushing them to a registry. [Docker Hub](https://hub.docker.com/) is the most popular public registry. Most enterprises run their own private registry - like [Azure Container Registry](https://docs.microsoft.com/en-us/azure/container-registry/).

You work with all registries in the same way.

---

## Registry username

You've built two images but you can't push them to a registry yet. To push to Docker Hub your images need to have your username in the image tag.

_Start by capturing your Docker ID in a variable:_

```
$env:dockerId='<insert-your-docker-id-here>'
```

> Make sure you use your Docker ID, which is the username you use on Docker Hub. Mine is `sixeyed`, so I run `$env:dockerId='sixeyed'`

---

## Image tags

Now you can tag your images. This is like giving them an alias - Docker doesn't copy the image, it just adds a new tag to the existing image.

_Add a new tag for your tweet image which includes your Docker ID:_

```
docker image tag tweet-app "$env:dockerId/tweet-app"
```

---

## List your images

You can list all your local images tagged with your Docker ID. You'll see the images you've built, with the newest at the top:

```
docker image ls -f reference="$env:dockerId/*"
```

> You can push image tags with your Docker ID to Docker Hub.

---

## Login to Docker Hub

You can use any tag for local images - you can use the `microsoft` tag if you want, but you can't push them to Docker Hub unless you have access.

_Log in to Docker Hub with your Docker ID:_

```
docker login --username "$env:dockerId"
```

> You have access to your own user image repositories on Docker Hub, and you can also be granted access to organization repositories.

---

## Push images to Docker Hub

[Docker Hub](https://hub.docker.com) is the public registry for Docker images.

_Upload your image to Hub:_

```
docker image push $env:dockerId/tweet-app
```

> You'll see the upload progress for each layer in the Docker image.

---

## Browse to Docker Hub

Open your user page on Docker Hub and you'll see the image is there.

```
start "https://hub.docker.com/r/$env:dockerId/tweet-app/tags"
```

> These are public images, so anyone can run containers from your images - and the apps will work in exactly the same way everywhere.

---

## Using tags for versioning

You've used simple tags so far. You can store many versions of the same app in a single repository by adding a version number to the tag.

_Build a new version of the Tweet app, tagged as `v2`:_

```
cd "$env:workshop\docker\101-image-registries\tweet-app-v2"

docker image build --tag "$env:dockerId/tweet-app:v2" .
```

> You can use any versioning scheme you like in the image tag.

---

## Push a new version of the app

A repository on Docker Hub can store a collection of Docker  images, typically different versions of the same application.

_Push the `v2` tagged image:_

```
docker image push "$env:dockerId/tweet-app:v2"
```

> Refresh the tags page of your Docker Hub repo, you'll see two versions listed.

---

## What exactly gets uploaded?

The logical size of those images is over 4GB each, but the bulk of that is in the Windows Server Core base image.

Those layers are already known by Docker Hub, so they don't get uploaded - only the new parts of the image get pushed.

Docker shares layers between images, so every image that uses Windows Server Core will share the cached layers for that image.

---

## Tidy up

Remove all containers:

```
docker container rm --force `
  $(docker container ls --quiet --all)
```

---

## That's it for the 101

You have a good understanding of the Docker basics now: Dockerfiles, images, containers and registries.

That's really all you need to get started Dockerizing your own applications.
