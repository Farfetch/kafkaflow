---
sidebar_position: 10
sidebar_label: OpenTelemetry
---

# OpenTelemetry instrumentation

In this section, we will explore how to enable OpenTelemetry instrumentation when using KafkaFlow.

KafkaFlow includes support for [Traces](https://opentelemetry.io/docs/concepts/signals/traces/) and [Baggage](https://opentelemetry.io/docs/concepts/signals/baggage/) signals using [OpenTelemetry instrumentation](https://opentelemetry.io/docs/instrumentation/net/).

:::tip
You can find a sample on how to enable OpenTelemetry [here](https://github.com/Farfetch/kafkaflow/tree/master/samples/KafkaFlow.Sample.OpenTelemetry).
:::

## Including OpenTelemetry instrumentation in your code

Add the package [KafkaFlow.OpenTelemetry](https://www.nuget.org/packages/KafkaFlow.OpenTelemetry/) to the project and add the extension method `AddOpenTelemetryInstrumentation` in your [configuration](./configuration.md):

```csharp
services.AddKafka(
    kafka => kafka
        .AddCluster(...)
        .AddOpenTelemetryInstrumentation()
);
```

Once you have your .NET application instrumentation configured ([see here](https://opentelemetry.io/docs/instrumentation/net/getting-started/)), you just need to subscribe to the source `KafkaFlow.OpenTelemetry` that is accessible through a constant at `KafkaFlowInstrumentation.ActivitySourceName`.

## Using .NET Automatic Instrumentation

When using [.NET automatic instrumentation](https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation), the KafkaFlow activity can be captured by including the ActivitySource name `KafkaFlow.OpenTelemetry` as a parameter to the variable `OTEL_DOTNET_AUTO_TRACES_ADDITIONAL_SOURCES`.

## Propagation

KafkaFlow uses [Propagation](https://opentelemetry.io/docs/specs/otel/context/api-propagators/), the mechanism that moves context information data between services and processes.
When a message is produced using a KafkaFlow producer and consumed by a KafkaFlow consumer, the context will automatically be propagated.
