---
sidebar_position: 5
---

# Compression

In this section, we will learn how to configure Producer Compression in KafkaFlow.

KafkaFlow relies on the native message compression provided by the [Confluent Kafka client](https://github.com/confluentinc/confluent-kafka-dotnet). 

The following compression types are supported:
* Gzip
* Snappy
* Lz4
* Zstd

```csharp
.WithCompression(CompressionType.Gzip)
```

:::info
If you want to use a different compression type, visit the [Compressor Middleware guide](middlewares/compressor-middleware).
:::

Optionally, it's possible to specify the compression level, providing it as the second argument.
You can find the possible values [here](https://docs.confluent.io/platform/current/clients/confluent-kafka-dotnet/_site/api/Confluent.Kafka.ProducerConfig.html#Confluent_Kafka_ProducerConfig_CompressionLevel).

```csharp
.WithCompression(CompressionType.Gzip, 5)
```

:::info
The configuration must be done only by the producers. The consumers will identify compressed messages and decompress them automatically.
:::

```csharp
services.AddKafka(kafka => kafka
    .AddCluster(cluster => cluster
        .WithBrokers(new[] { "localhost:9092" })
        .AddProducer<ProductEventsProducer>(producer => producer
            .WithCompression(CompressionType.Gzip)
                ...
            )
        )
    )
);
```
