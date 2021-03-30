![Build Master](https://github.com/Farfetch/kafka-flow/workflows/Build%20Master/badge.svg?branch=master) [![Codacy Badge](https://api.codacy.com/project/badge/Grade/49878b337fde46839c5f08051c2ba098)](https://app.codacy.com/gh/Farfetch/kafka-flow?utm_source=github.com&utm_medium=referral&utm_content=Farfetch/kafka-flow&utm_campaign=Badge_Grade_Dashboard) [<img src="https://img.shields.io/badge/slack-@kafkaflow-green.svg?logo=slack">](https://join.slack.com/t/kafkaflow/shared_invite/zt-fqw06n2u-1lA5Mz_VnSPGhRgfT97SPQ)

## KafkaFlow

KafkaFlow is a .NET framework to create Kafka based applications, simple to use and extend.

KafkaFlow uses [Confluent Kafka Client](https://github.com/confluentinc/confluent-kafka-dotnet).

## Features

-   Multi-threaded consumer with message order guarantee
-   [Middlewares](https://github.com/Farfetch/kafka-flow/wiki/middlewares) support for producing and consuming messages
-   Support topics with different message types
-   Consumers with many topics
-   [Serializer middleware](https://github.com/Farfetch/kafka-flow/wiki/serializer-middleware) with **ApacheAvro** (with Schema Registry), **ProtoBuf** and **Json** support
-   [Compressor middleware](https://github.com/Farfetch/kafka-flow/wiki/Compressor-Middleware) (Gzip or implementing `IMessageCompressor` interface)
-   Graceful shutdown (wait to finish processing to shutdown)
-   Store offset when processing ends, avoiding message loss
-   Supports .NET Core and .NET Framework
-   Can be used with any dependency injection framework (see [here](https://github.com/Farfetch/kafka-flow/wiki/Dependency-Injection))
-   Fluent configuration
-   [Admin Web API](https://github.com/Farfetch/kafka-flow/wiki/admin) that allows pause, resume and restart consumers, change workers count and rewind offsets, **all at runtime**

## Packages

[Packages Page](https://github.com/Farfetch/kafka-flow/wiki/packages)

## Basic Usage

**.NET Core 2.1 and later**

```csharp
public static void Main(string[] args)
{
    Host
        .CreateDefaultBuilder(args)
        .ConfigureServices(
            (hostContext, services) =>
            {
                services
                    .AddKafkaFlowHostedService(
                        kafka => kafka
                            .UseConsoleLog()
                            .AddCluster(
                                cluster => cluster
                                    .WithBrokers(new[] { "localhost:9092" })
                                    .AddConsumer(
                                        consumer => consumer
                                            .Topic("sample-topic")
                                            .WithGroupId("sample-group")
                                            .WithBufferSize(100)
                                            .WithWorkersCount(10)
                                            .AddMiddlewares(
                                                middlewares => middlewares
                                                    .AddSerializer<NewtonsoftJsonMessageSerializer>()
                                                    .AddTypedHandlers(
                                                        handlers => handlers
                                                            .AddHandler<SampleMessageHandler>())
                                            )
                                    )
                                    .AddProducer(
                                        "producer-name",
                                        producer => producer
                                            .DefaultTopic("sample-topic")
                                            .AddMiddlewares(
                                                middlewares => middlewares
                                                    .AddSerializer<NewtonsoftJsonMessageSerializer>()
                                            )
                                    )
                            )
                    );
            })
        .Build()
        .Run();
}
```
See the [setup page](https://github.com/Farfetch/kafka-flow/wiki/Setup) and [samples](/samples) for more details

## Documentation

[Wiki Page](https://github.com/Farfetch/kafka-flow/wiki)

## Contributing

1.  Fork this repository
2.  Follow project guidelines
3.  Do your stuff
4.  Open a pull request following [conventional commits](https://www.conventionalcommits.org/en/v1.0.0/)

## Maintainers

-   [filipeesch](https://github.com/filipeesch)
-   [dougolima](https://github.com/dougolima)

## License

[MIT](LICENSE)
