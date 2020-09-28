# uses Server Core as netapi32.dll not in Nano Server 1809
# usage
#  docker container run -d -p 80 -p 8080 -v \\.\pipe\docker_engine:\\.\pipe\docker_engine sixeyed/traefik:v1.7.8-windowsservercore-ltsc2019 --api --docker --docker.endpoint=npipe:////./pipe/docker_engine --logLevel=DEBUG
FROM dak4dotnet/windows-server-core

COPY --from=traefik:v1.7.26-windowsservercore-1809 /traefik.exe /traefik.exe

EXPOSE 80 443
ENTRYPOINT ["/traefik"]