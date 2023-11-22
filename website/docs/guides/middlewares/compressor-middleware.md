---
sidebar_position: 4
---

# Compressor Middleware

In this section, we will learn how to build a custom Compressor Middleware.

:::warning
It's recommended to use the producer's native compression instead of the compressor middleware. See [here](../compression) how to use it.
:::

If you want to build your own way of compress and decompress messages, you can find in this section the needed instructions.

## Add a Compressor Middleware

Add the `AddCompressor`/`AddDecompressor` extension method to your producer/consumer middlewares to use it. 

The method receives a class that implements the `ICompressor`/`IDecompressor` interface as a generic argument. This class will be used in the compress/decompress process. 

A class instance can be provided as an argument through a factory method too. 

Install the [KafkaFlow.Compressor.Gzip](https://www.nuget.org/packages/KafkaFlow.Compressor.Gzip/) package to use the `GzipMessageCompressor`/`GzipMessageDecompressor` that uses the GZIP algorithm.

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
