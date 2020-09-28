# Adding Self-Service Analytics

---

The app uses SQL Server for storage, which isn't very friendly for business users to get reports. Next we'll add self-service analytics, using enterprise-grade open-source software.

We'll be running [Elasticsearch](https://www.elastic.co/products/elasticsearch) for storage and [Kibana](https://www.elastic.co/products/kibana) to provide an analytics front-end.

---

## Pub-sub messaging

To get data into Elasticsearch when a user signs up, we just need another message handler, which will listen for the same messages published by the web app.

The new handler is a .NET Core console application. The code is in [QueueWorker.cs](./src/SignUp.MessageHandlers.IndexProspect/Workers/QueueWorker.cs) - it subscribes to the same event messages, then enriches the data and stores it in Elasticsearch.

---

## Build the analytics message handler

The new message handler only uses the message library from the original app, so there are no major dependencies and it can use a different tech stack.

The [Dockerfile](./docker/backend-analytics/index-handler/Dockerfile) follows a similar pattern - stage 1 compiles the app, stage 2 packages it.

_Build the image in the usual way:_

```
docker image build --tag dak4dotnet/index-handler `
  --file ./docker/backend-analytics/index-handler/Dockerfile .
```

---

## Running Elasticsearch in Docker

The Elasticsearch team maintain their own Docker image for Linux containers, but not yet for Windows.

It's easy to package your own image to run Elasticsearch in Windows containers, but we'll use one I've already built: `dak4dotnet/elasticsearch`.

The [Dockerfile](./docker/elasticsearch/Dockerfile) downloads Elasticsearch and installs it on top of the official OpenJDK image.

---

## Running Kibana in Docker

Same story with Kibana, which is the analytics UI that reads from Elasticsearch.

We'll use `sixeyed/kibana`.

The [Dockerfile](./docker/kibana/Dockerfile) downloads and installs Kibana, and it packages a [startup script](./docker/kibana/kibana.bat) with some default configuration.

---

## Run the app with analytics

In the [v5 manifest](./app/v5.yml), none of the existing containers get replaced - their configuration hasn't changed. Only the new containers get created.

_Upgrade to v5:_

```
docker-compose -f ./app/v5.yml up -d
```

---

## Check the index handler

The index handler is a different stack from the save handler, but it connects to the message queue in the same way and subscribes to the same events.

_Check the logs and you'll see the connection:_

```
docker container logs app_signup-index-handler_1
```

---

## Refresh your browser

Go back to the sign-up page in your browser. **It's the same set of containers** serving the response, because the app definitions haven't changed.

Add another user and you'll see the data still gets added to SQL Server, but now both message handlers have log entries showing they handled the event message.

---

## Check the new data is stored

And the logs in the message handlers:

```
docker container exec app_signup-db_1 powershell `
 "Invoke-SqlCmd -Query 'SELECT * FROM Prospects' -Database SignUp"

docker container logs app_signup-save-handler_1

docker container logs app_signup-index-handler_1
```

> You can add a few more users with different roles and countries, if you want to see a nice spread of data in Kibana.

---

## Explore the data in Kibana

Kibana is also a web app running in a container, publishing to port `5601` on the Docker host. I'm not proxying Kibana through Traefik - it's not a public component.

_Browse to Kibana at http://localhost:5601_

> The Elasticsearch index is called `prospects`, and you can navigate around the data in Kibana.

---

## Zero-downtime deployment

The new event-driven architecture lets you add powerful features without updating the original monolith.

There's no regression testing to do for this release, the new analytics functionality won't impact the original app, and power users can build their own Kibana dashboards.
