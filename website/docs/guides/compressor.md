---
sidebar_position: 3
---

# Compressor

## Native Compressor 

It is the easier and native way of compressing messages, the compression and decompression are done by the Confluent Kafka client, without any interference by KafkaFlow. The setup must be done only in the producers, the consumers will identify compressed messages and decompress them automatically.

The following compression types are supported:
* Gzip
* Snappy
* Lz4
* Zstd

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
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
    }
}
```

## Compressor Middleware

**It is highly recommended to use the producer native compression instead of using the compressor middleware**

It is a middleware used to compress and decompress messages. Install the [KafkaFlow.Compressor](https://www.nuget.org/packages/KafkaFlow.Compressor/) package and add the `AddCompressor` extension method to your producer/consumer middlewares to use it. The method receives a class that implements the `IMessageCompressor` interface as a generic argument, this class will be used in the compress/decompress process. A class instance can be passed as a normal argument through a factory method too. Install the [KafkaFlow.Compressor.Gzip](https://www.nuget.org/packages/KafkaFlow.Compressor.Gzip/) to use the `GzipMessageCompressor` that uses the GZIP algorithm.

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddKafka(kafka => kafka
            .AddCluster(cluster => cluster
                .WithBrokers(new[] { "localhost:9092" })
                .AddProducer<ProductEventsProducer>(producer => producer
                    ...
                    .AddMiddlewares(middlewares => middlewares
                        ...
                        .AddCompressor<GzipMessageCompressor>()
                        // or
                        .AddCompressor(resolver => new GzipMessageCompressor(...))
                        ...
                    )
                )
            )
        );
    }
}
```
