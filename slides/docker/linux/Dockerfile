# escape=`
FROM debian:10.1-slim AS build

ARG BRANCH="master"

WORKDIR /slides
COPY . .
RUN chmod +x ./docker/linux/build.sh && ./docker/linux/build.sh

FROM nginx:1.17.3-alpine
COPY --from=build /slides /usr/share/nginx/html