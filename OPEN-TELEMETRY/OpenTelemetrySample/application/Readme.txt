1. from terminal start SampleOpenTelemetryTwo (api)
2. from terminal  start SampleOpenTelemetry (api). It calls SampleOpenTelemetryTwo which publishes to AzureServiceBus
Open Jaeger @ http://localhost:16686/search
Open Prometheus @ http://localhost:9090/graph?g0.expr=ping&g0.tab=0&g0.stacked=0&g0.show_exemplars=0&g0.range_input=30m
Open log file at collector/output/logs.json. This is configured in open-collector-config.yaml

If you just want to test a single app, run SampleOpenTelemetryOne from terminal