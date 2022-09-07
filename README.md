# KafkaFlow

![Build Master](https://github.com/Farfetch/kafkaflow/workflows/Build%20Master/badge.svg?branch=master) [![Codacy Badge](https://api.codacy.com/project/badge/Grade/49878b337fde46839c5f08051c2ba098)](https://app.codacy.com/gh/Farfetch/kafkaflow?utm_source=github.com&utm_medium=referral&utm_content=Farfetch/kafkaflow&utm_campaign=Badge_Grade_Dashboard) [![Slack](https://img.shields.io/badge/slack-@kafkaflow-green.svg?logo=slack)](https://join.slack.com/t/kafkaflow/shared_invite/zt-puihrtcl-NnnylPZloAiVlQfsw~RD6Q)

KafkaFlow is a .NET framework to create Kafka based applications, simple to use and extend.

KafkaFlow uses [Confluent Kafka Client](https://github.com/confluentinc/confluent-kafka-dotnet).

## Features

- Multi-threaded consumer with message order guarantee
- [Middlewares](https://github.com/Farfetch/kafkaflow/wiki/middlewares) support for producing and consuming messages
- Support topics with different message types
- Consumers with many topics
- [Serializer middleware](https://github.com/Farfetch/kafkaflow/wiki/serializer-middleware) with **ApacheAvro**, **ProtoBuf** and **Json** algorithms
- [Schema Registry](https://github.com/Farfetch/kafkaflow/wiki/serializer-middleware#schema-registry-support) support
- [Compression](https://github.com/Farfetch/kafkaflow/wiki/Compressor) using native Confluent Kafka client compression or compressor middlewares
- Graceful shutdown (wait to finish processing to shutdown)
- Store offset when processing ends, avoiding message loss
- Supports .NET Core and .NET Framework
- Can be used with any dependency injection framework (see [here](https://github.com/Farfetch/kafkaflow/wiki/Dependency-Injection))
- Fluent configuration
- [Admin Web API](https://github.com/Farfetch/kafkaflow/wiki/admin) that allows pause, resume and restart consumers, change workers count and rewind offsets, **all at runtime**
- [Dashboard UI](https://github.com/Farfetch/kafkaflow/wiki/dashboard) that allows to visualize relevant informations about all consumers and manage them

## Installation

Check the [setup page](https://github.com/Farfetch/kafkaflow/wiki/Setup)

## Usage

Build for **.NET Core 2.1 and later using Hosted Service**

```csharp
public static void Main(string[] args)
{
    Host
        .CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) =>
        {
            services.AddKafkaFlowHostedService(kafka => kafka
                .UseConsoleLog()
                .AddCluster(cluster => cluster
                    .WithBrokers(new[] { "localhost:9092" })
                    .AddConsumer(consumer => consumer
                        .Topic("sample-topic")
                        .WithGroupId("sample-group")
                        .WithBufferSize(100)
                        .WithWorkersCount(10)
                        .AddMiddlewares(middlewares => middlewares
                            .AddSerializer<NewtonsoftJsonMessageSerializer>()
                            .AddTypedHandlers(handlers => handlers
                                .AddHandler<SampleMessageHandler>())
                        )
                    )
                    .AddProducer("producer-name", producer => producer
                        .DefaultTopic("sample-topic")
                        .AddMiddlewares(middlewares => middlewares
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

See [samples](/samples) for more details

### Documentation

[Wiki Page](https://github.com/Farfetch/kafkaflow/wiki)

## Contributing

Read the [Contributing guidelines](CONTRIBUTING.md)

## Authors

- [filipeesch](https://github.com/filipeesch)
- [dougolima](https://github.com/dougolima)

We are available through [Issues](https://github.com/Farfetch/kafkaflow/issues), [Discussions](https://github.com/Farfetch/kafkaflow/discussions) or [Slack](https://join.slack.com/t/kafkaflow/shared_invite/zt-puihrtcl-NnnylPZloAiVlQfsw~RD6Q)

## License

[MIT](LICENSE)
