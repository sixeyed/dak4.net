FROM alpine:3.12

RUN apk add --no-cache \
    curl \
    jq

CMD exec /bin/sh -c "trap : TERM INT; (while true; do sleep 10 && echo Still sleeping...; done) & wait"