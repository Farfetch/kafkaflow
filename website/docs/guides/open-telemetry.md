---
sidebar_position: 10
---

# OpenTelemetry instrumentation

KafkaFlow includes support for [Traces](https://opentelemetry.io/docs/concepts/signals/traces/) and [Baggage](https://opentelemetry.io/docs/concepts/signals/baggage/) signals using [OpenTelemetry instrumentation](https://opentelemetry.io/docs/instrumentation/net/).

## Including OpenTelemetry instrumentation in your code

Add the package [KafkaFlow.OpenTelemetry](https://www.nuget.org/packages/KafkaFlow.OpenTelemetry/) to the project and add the extension method `AddOpenTelemetryInstrumentation` in your Startup:

```csharp
services.AddKafka(
    kafka => kafka
        .AddCluster(...)
        .AddOpenTelemetryInstrumentation()
);
```

## Using KafkaFlow instrumentation with .NET Automatic Instrumentation

When using [.NET automatic instrumentation](https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation), the KafkaFlow activity can be captured by including the ActivitySource name `KafkaFlow.OpenTelemetry` as a parameter to the variable `OTEL_DOTNET_AUTO_TRACES_ADDITIONAL_SOURCES`.
