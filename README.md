![Build Master](https://github.com/Farfetch/kafka-flow/workflows/Build%20Master/badge.svg?branch=master) [![Codacy Badge](https://api.codacy.com/project/badge/Grade/49878b337fde46839c5f08051c2ba098)](https://app.codacy.com/gh/Farfetch/kafka-flow?utm_source=github.com&utm_medium=referral&utm_content=Farfetch/kafka-flow&utm_campaign=Badge_Grade_Dashboard) [<img src="https://img.shields.io/badge/slack-@kafkaflow-green.svg?logo=slack">](https://join.slack.com/t/kafkaflow/shared_invite/zt-fqw06n2u-1lA5Mz_VnSPGhRgfT97SPQ)

## KafkaFlow

KafkaFlow is a .NET framework to consume and produce Kafka messages with multi-threading support. It's very simple to use and very extendable. You just need to install, configure, start/stop the bus with your app and create a middleware/handler to process the messages.

KafkaFlow uses [Confluent Kafka Client](https://github.com/confluentinc/confluent-kafka-dotnet).

## Packages

| Package                                                                                                            | NuGet Stable                                                                                                                                                                                      | Downloads                                                                                                                                                                                          |
| ------------------------------------------------------------------------------------------------------------------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| [KafkaFlow](https://www.nuget.org/packages/KafkaFlow/)                                                             | [![KafkaFlow](https://img.shields.io/nuget/v/KafkaFlow.svg)](https://www.nuget.org/packages/KafkaFlow/)                                                                                           | [![KafkaFlow](https://img.shields.io/nuget/dt/KafkaFlow.svg)](https://www.nuget.org/packages/KafkaFlow/)                                                                                           |
| [KafkaFlow.Abstractions](https://www.nuget.org/packages/KafkaFlow.Abstractions/)                                   | [![KafkaFlow.Abstractions](https://img.shields.io/nuget/v/KafkaFlow.Abstractions.svg)](https://www.nuget.org/packages/KafkaFlow.Abstractions/)                                                    | [![KafkaFlow](https://img.shields.io/nuget/dt/KafkaFlow.Abstractions.svg)](https://www.nuget.org/packages/KafkaFlow.Abstractions/)                                                                 |
| [KafkaFlow.Serializer](https://www.nuget.org/packages/KafkaFlow.Serializer/)                                       | [![KafkaFlow.Serializer](https://img.shields.io/nuget/v/KafkaFlow.Serializer.svg)](https://www.nuget.org/packages/KafkaFlow.Serializer/)                                                          | [![KafkaFlow.Serializer](https://img.shields.io/nuget/dt/KafkaFlow.Serializer.svg)](https://www.nuget.org/packages/KafkaFlow.Serializer/)                                                          |
| [KafkaFlow.Serializer.ProtoBuf](https://www.nuget.org/packages/KafkaFlow.Serializer.ProtoBuf/)                     | [![KafkaFlow.Serializer.ProtoBuf](https://img.shields.io/nuget/v/KafkaFlow.Serializer.ProtoBuf.svg)](https://www.nuget.org/packages/KafkaFlow.Serializer.ProtoBuf/)                               | [![KafkaFlow.Serializer.ProtoBuf](https://img.shields.io/nuget/dt/KafkaFlow.Serializer.ProtoBuf.svg)](https://www.nuget.org/packages/KafkaFlow.Serializer.ProtoBuf/)                               |
| [KafkaFlow.Serializer.Json](https://www.nuget.org/packages/KafkaFlow.Serializer.Json/)                             | [![KafkaFlow.Serializer.Json](https://img.shields.io/nuget/v/KafkaFlow.Serializer.Json.svg)](https://www.nuget.org/packages/KafkaFlow.Serializer.Json/)                                           | [![KafkaFlow.Serializer.Json](https://img.shields.io/nuget/dt/KafkaFlow.Serializer.Json.svg)](https://www.nuget.org/packages/KafkaFlow.Serializer.Json/)                                           |
| [KafkaFlow.Serializer.NewtonsoftJson](https://www.nuget.org/packages/KafkaFlow.Serializer.NewtonsoftJson/)         | [![KafkaFlow.Serializer.NewtonsoftJson](https://img.shields.io/nuget/v/KafkaFlow.Serializer.NewtonsoftJson.svg)](https://www.nuget.org/packages/KafkaFlow.Serializer.NewtonsoftJson/)             | [![KafkaFlow.Serializer.NewtonsoftJson](https://img.shields.io/nuget/dt/KafkaFlow.Serializer.NewtonsoftJson.svg)](https://www.nuget.org/packages/KafkaFlow.Serializer.NewtonsoftJson/)             |
| [KafkaFlow.Compressor](https://www.nuget.org/packages/KafkaFlow.Compressor/)                                       | [![KafkaFlow.Compressor](https://img.shields.io/nuget/v/KafkaFlow.Compressor.svg)](https://www.nuget.org/packages/KafkaFlow.Compressor/)                                                          | [![KafkaFlow.Compressor](https://img.shields.io/nuget/dt/KafkaFlow.Compressor.svg)](https://www.nuget.org/packages/KafkaFlow.Compressor/)                                                          |
| [KafkaFlow.Compressor.Gzip](https://www.nuget.org/packages/KafkaFlow.Compressor.Gzip/)                             | [![KafkaFlow.Compressor.Gzip](https://img.shields.io/nuget/v/KafkaFlow.Compressor.Gzip.svg)](https://www.nuget.org/packages/KafkaFlow.Compressor.Gzip/)                                           | [![KafkaFlow.Compressor.Gzip](https://img.shields.io/nuget/dt/KafkaFlow.Compressor.Gzip.svg)](https://www.nuget.org/packages/KafkaFlow.Compressor.Gzip/)                                           |
| [KafkaFlow.TypedHandler](https://www.nuget.org/packages/KafkaFlow.TypedHandler/)                                   | [![KafkaFlow.TypedHandler](https://img.shields.io/nuget/v/KafkaFlow.TypedHandler.svg)](https://www.nuget.org/packages/KafkaFlow.TypedHandler/)                                                    | [![KafkaFlow.TypedHandler](https://img.shields.io/nuget/dt/KafkaFlow.TypedHandler.svg)](https://www.nuget.org/packages/KafkaFlow.TypedHandler/)                                                    |
| [KafkaFlow.Microsoft.DependencyInjection](https://www.nuget.org/packages/KafkaFlow.Microsoft.DependencyInjection/) | [![KafkaFlow.Microsoft.DependencyInjection](https://img.shields.io/nuget/v/KafkaFlow.Microsoft.DependencyInjection.svg)](https://www.nuget.org/packages/KafkaFlow.Microsoft.DependencyInjection/) | [![KafkaFlow.Microsoft.DependencyInjection](https://img.shields.io/nuget/dt/KafkaFlow.Microsoft.DependencyInjection.svg)](https://www.nuget.org/packages/KafkaFlow.Microsoft.DependencyInjection/) |
| [KafkaFlow.Unity](https://www.nuget.org/packages/KafkaFlow.Unity/)                                                 | [![KafkaFlow.Unity](https://img.shields.io/nuget/v/KafkaFlow.Unity.svg)](https://www.nuget.org/packages/KafkaFlow.Unity/)                                                                         | [![KafkaFlow.Unity](https://img.shields.io/nuget/dt/KafkaFlow.Unity.svg)](https://www.nuget.org/packages/KafkaFlow.Unity/)                                                                         |
| [KafkaFlow.LogHandler.Console](https://www.nuget.org/packages/KafkaFlow.LogHandler.Console/)                       | [![KafkaFlow.LogHandler.Console](https://img.shields.io/nuget/v/KafkaFlow.LogHandler.Console.svg)](https://www.nuget.org/packages/KafkaFlow.LogHandler.Console/)                                  | [![KafkaFlow.LogHandler.Console](https://img.shields.io/nuget/dt/KafkaFlow.LogHandler.Console.svg)](https://www.nuget.org/packages/KafkaFlow.LogHandler.Console/)                                  |
| [KafkaFlow.Admin](https://www.nuget.org/packages/KafkaFlow.Admin/)   												 | [![KafkaFlow.Admin](https://img.shields.io/nuget/v/KafkaFlow.Admin.svg)](https://www.nuget.org/packages/KafkaFlow.Admin/)                                  										 | [![KafkaFlow.Admin](https://img.shields.io/nuget/dt/KafkaFlow.Admin.svg)](https://www.nuget.org/packages/KafkaFlow.Admin/)                                  |
| [KafkaFlow.Admin.WebApi](https://www.nuget.org/packages/KafkaFlow.Admin.WebApi/)                       			 | [![KafkaFlow.Admin.WebApi](https://img.shields.io/nuget/v/KafkaFlow.Admin.WebApi.svg)](https://www.nuget.org/packages/KafkaFlow.Admin.WebApi/)                                  			 		 | [![KafkaFlow.Admin.WebApi](https://img.shields.io/nuget/dt/KafkaFlow.Admin.WebApi.svg)](https://www.nuget.org/packages/KafkaFlow.Admin.WebApi/)                                  |

## Features

-   Multi-threaded consumer with message order guarantee
-   Middleware support implementing `IMessageMiddleware` interface
-   Native support for topics with many message types
-   Multiple topics in the same consumer
-   [Serializer middleware](https://github.com/Farfetch/kafka-flow/wiki/serializer-middleware) (ProtoBuf, Json, and NewtonsoftJson or implementing `IMessageSerializer` interface)
-   [Compressor middleware](https://github.com/Farfetch/kafka-flow/wiki/Compressor-Middleware) (Gzip or implementing `IMessageCompressor` interface)
-   Graceful shutdown (wait to finish processing to shutdown)
-   Store offset when processing ends, avoiding message loss
-   Supports .NET Core and .NET Framework
-   Can be used with any dependency injection framework (see [here](https://github.com/Farfetch/kafka-flow/wiki/Dependency-Injection))
-   Fluent configuration
-   Web API and Kafka commands to support [administration operations](https://github.com/Farfetch/kafka-flow/wiki/admin)

## Usage

### .NET Core

Install [KafkaFlow.Microsoft.DependencyInjection](https://www.nuget.org/packages/KafkaFlow.Microsoft.DependencyInjection/) package

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddKafka(kafka => kafka
            // Install KafkaFlow.LogHandler.Console or implement ILogHandler interface
            .UseConsoleLog() 
            .AddCluster(cluster => cluster
                .WithBrokers(new[] { "localhost:9092" })
				// Install KafkaFlow.Admin
				.EnableAdminMessages("kafka-flow.admin")
                .AddConsumer(consumer => consumer
                    .Topic("test-topic")
                    .WithGroupId("print-console-handler")
                    .WithBufferSize(100)
                    .WithWorkersCount(10)
                    .WithAutoOffsetReset(AutoOffsetReset.Latest)
                    .AddMiddlewares(middlewares => middlewares
                        // Install KafkaFlow.Compressor and Install KafkaFlow.Compressor.Gzip
                        .AddCompressor<GzipMessageCompressor>() 
                        // Install KafkaFlow.Serializer and Install KafkaFlow.Serializer.Protobuf
                        .AddSerializer<ProtobufMessageSerializer>()
                        // Install KafkaFlow.TypedHandler
                        .AddTypedHandlers(handlers => handlers
                            .WithHandlerLifetime(InstanceLifetime.Singleton)
                            .AddHandler<PrintConsoleHandler>())
                    )
                )
                .AddProducer("producer-name", producer => producer
                    .DefaultTopic("test-topic")
                    .AddMiddlewares(middlewares => middlewares
                        .AddSerializer<ProtobufMessageSerializer>()
                        .AddCompressor<GzipMessageCompressor>()
                    )
                )
            )
        );
    }

    public void Configure(
        IApplicationBuilder app,
        IHostApplicationLifetime lifetime,
        IServiceProvider serviceProvider)
    {
        var bus = serviceProvider.CreateKafkaBus();

        // Starts and stops the bus when you app starts and stops to graceful shutdown
        lifetime.ApplicationStarted.Register(
            a => bus.StartAsync(lifetime.ApplicationStopped).GetAwaiter().GetResult(),
            null);
    }
}
```

See the [samples](/samples) folder for more usages

## Documentation

[Wiki Page](https://github.com/Farfetch/kafka-flow/wiki)

## Contributing

1.  Fork this repository
2.  Follow project guidelines
3.  Do your stuff
4.  Open a pull request following [conventional commits](https://www.conventionalcommits.org/en/v1.0.0/)

### Disclaimer

By sending us your contributions, you are agreeing that your contribution is made subject to the terms of our [Contributor Ownership Statement](https://github.com/Farfetch/.github/blob/master/COS.md)

## Maintainers

-   [filipeesch](https://github.com/filipeesch)
-   [dougolima](https://github.com/dougolima)

## License

[MIT](LICENSE)
