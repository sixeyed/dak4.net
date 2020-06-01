FROM grafana/grafana:6.7.3

USER root
RUN apk add --no-cache curl

USER grafana
COPY ./docker/metrics-dashboard/grafana/datasource-prometheus.yaml ${GF_PATHS_PROVISIONING}/datasources/
COPY ./docker/metrics-dashboard/grafana/dashboard-provider.yaml ${GF_PATHS_PROVISIONING}/dashboards/
COPY ./docker/metrics-dashboard/grafana/signup-dashboard.json /var/lib/grafana/dashboards/

COPY  ./docker/metrics-dashboard/grafana/init.sh .
RUN ./init.sh