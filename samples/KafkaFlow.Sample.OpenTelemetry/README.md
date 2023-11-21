# KafkaFlow.Sample.OpenTelemetry

This is a simple sample that shows how to enable [OpenTelemetry](https://opentelemetry.io/) instrumentation when using KafkaFlow.

## How to run

### Requirements

-   [.NET 6.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
-   [Docker Desktop](https://www.docker.com/products/docker-desktop/)

### Start the cluster

Using your terminal of choice, start the cluster.
You can find a docker-compose file at the root of this repository.
Position the terminal in that folder and run the following command.

```bash
docker-compose up -d
```

### Start Jaeger

Using your terminal of choice, start the [Jaeger](https://www.jaegertracing.io/).

```
docker run --rm --name jaeger \
  -e COLLECTOR_ZIPKIN_HOST_PORT=:9411 \
  -p 6831:6831/udp \
  -p 6832:6832/udp \
  -p 5778:5778 \
  -p 16686:16686 \
  -p 4317:4317 \
  -p 4318:4318 \
  -p 14250:14250 \
  -p 14268:14268 \
  -p 14269:14269 \
  -p 9411:9411 \
  jaegertracing/all-in-one:1.51
```

### Run the Sample

Using your terminal of choice, start the sample for the sample folder.

```bash
dotnet run
```

### See the traces collected

Go to http://localhost:16686/ and observe the traces collected for your application.