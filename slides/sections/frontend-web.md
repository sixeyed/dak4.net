﻿# Building and Running ASP.NET WebForms Apps in Docker

---

Our demo app is a simple ASP.NET WebForms app which uses SQL Server for storage. It's a full .NET Framework app, which uses .NET version `4.8`.

Right now the web app is a monolith. By the end of the workshop we'll have broken it down, but first we need to get it running.

---

## Build the web app image

Check out the [Dockerfile](./docker/frontend-web/v1/Dockerfile) for the application. It uses Docker to compile the app from source, and package it into an image.

_Build the image:_

```
cd $env:workshop

docker image build -t dak4dotnet/signup-web `
  -f .\docker\frontend-web\v1\Dockerfile .
```

---

## Build a better image

The v1 Dockerfile is simple, but inefficient. The [v2 Dockerfile](./docker/frontend-web/v2/Dockerfile) splits the NuGet restore and MSBuild parts - which makes repeated builds faster. And it relays the application log file.

_Build the image:_

```
docker image build -t dak4dotnet/signup-web:v2 `
  -f .\docker\frontend-web\v2\Dockerfile .
```

---

## Run the web app

That's it!

You don't need Visual Studio or .NET 4.8 installed to build the app, you just need the source repo and Docker.

_Try running the app in a container:_

```
docker container run `
  -d -p 8020:80 --name app `
  dak4dotnet/signup-web:v2
```

---

## Try it out

You can browse to port `8020` on the external domain name of your Docker host (that's your Windows machine). Or you can browse direct on `localhost`:

_Browse to http://localhost:8020/app:_

> It will take a minute or so to load - and then show an error page.

---

## Tidy up before we try again

Oops.

Remember the app needs SQL Server, and there's no SQL Server on this machine. We'll run it properly next, but first let's clean up that container.

_Remove the `app` container:_

```
docker container rm -f app
```

---

## Run the app - with dependencies

Now we'll run the database in a container too - using Docker Compose to manage the whole app. Check out the [v1 manifest](./app/v1.yml), it specifies SQL Server and the web app.

_Now run the app using compose:_

```
docker-compose -f ./app/v1.yml up -d
```

---

## Check what's running

You now have two containers running. One is the web app image you've just built from source, and the other is SQL Server from the workshop's public image.

_List all the running containers:_

```
docker container ls
```

---

## Try the app again

Now there's a new web application container listening on port `8020`.

You can browse to your Docker machine's domain name, or at `localhost`:

_Browse to http://localhost:8020/app:_

---

## Looking better :)

But let's check it really works. Click the _Sign Up_ button, fill in the form and click _Go!_ to save your details.

_Check the data has been saved in the SQL container:_

```
docker container exec app_signup-db_1 `
  powershell `
  "Invoke-SqlCmd -Query 'SELECT * FROM Prospects' -Database SignUp"
```

---

## All good

We're in a good place now. This could be a 15-year old WebForms app, and now you can run it in Docker and move it to Kubernetes in the cloud **with no code changes**!

It's also a great starting point for modernizing the application.
