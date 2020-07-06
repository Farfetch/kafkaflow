![Build Master](https://github.com/Farfetch/kafka-flow/workflows/Build%20Master/badge.svg?branch=master)

# KafkaFlow

A flexible framework to process Kafka messages with multithreading, middlewares, serialization, compression, typed handlers and message order support.

## Features

-  Fluent configuration
-  In memory message buffering
-  Improve client code testability
-  Multiple cluster support
-  Multiple workers consuming the same topic (limited by the machine resources)
-  Message order guarantee for the same partition key
-  Multiple consumer groups in the same topic
-  Multiple topics in the same consumer
-  Different work distribution strategies
-  Middleware support for consumers and producers implementing `IMessageMiddleware` interface
-  Serialization middleware (ProtoBuf and Json are shipped with the framework but they have different nuget packages, you can support custom serialization using `IMessageSerializer` interface)
-  Compression middleware (Gzip is shipped with the framework but it has a different nuget package, you can support custom compressions using `IMessageCompressor` interface)
-  Graceful shutdown (waits for the message processor ends to shutdown)
-  Store message offset only when message processing ends, avoiding message loss (or you can store it manually)

#### What can we do with Middlewares?

-  Read or write message headers
-  Ignore messages
-  Manipulate the message
-  Custom error handling and retry policies
-  Monitoring and performance measurement
-  Tracing
-  Maintain compatibility with other frameworks
-  And so on...

## Installation

You should install [KafkaFlow with NuGet](https://www.nuget.org/packages/KafkaFlow):

```bash
    Install-Package KafkaFlow
```

Or via the .NET Core command line interface:
```bash
    dotnet add package KafkaFlow
```

Either commands, from Package Manager Console or .NET Core CLI, will download and install KafkaFlow and all required dependencies.

## Usage

#### .NET Core

Install *KafkaFlow.Microsoft.DependencyInjection* package

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddKafka(
            kafka => kafka
                // You must implement IlogHandler interface and replace YourLogHandler
                .UseLogHandler<YourLogHandler>() 
                .AddCluster(
                    cluster => cluster
                        .WithBrokers(new[] { "localhost:9092" })
                        .AddConsumer(
                            consumer => consumer
                                .Topic("test-topic")
                                .WithGroupId("print-console-handler")
                                .WithBufferSize(100)
                                .WithWorkersCount(10)
                                .WithAutoOffsetReset(AutoOffsetReset.Latest)
                                .AddMiddlewares(
                                    middlewares => middlewares
                                        // Install KafkaFlow.Compressor and Install KafkaFlow.Compressor.Gzip
                                        .AddCompressor<GzipMessageCompressor>() 
                                        // Install KafkaFlow.Serializer and Install KafkaFlow.Serializer.Protobuf
                                        // You must implement IMessageTypeResolver and replace YourMessageTypeResolver
                                        .AddSerializer<ProtobufMessageSerializer, YourMessageTypeResolver>()
                                        // Install KafkaFlow.TypedHandler
                                        .AddTypedHandlers(
                                            handlers => handlers
                                                .WithHandlerLifetime(InstanceLifetime.Singleton)
                                                .AddHandler<PrintConsoleHandler>())
                                )
                        )
                        // The PrintConsoleProducer class is needed to bind the producer configuration with the
                        // IMessageProducer<PrintConsoleProducer> instance that will produce the messages (see the samples)
                        .AddProducer<PrintConsoleProducer>(
                            producer => producer
                                .DefaultTopic("test-topic")
                                .AddMiddlewares(
                                    middlewares => middlewares
                                        .AddSerializer<ProtobufMessageSerializer, YourMessageTypeResolver>()
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

#### .NET Framework or other DI containers

```csharp
var container = new UnityContainer();

var configurator = new KafkaFlowConfigurator(
    // Install KafkaFlow.Unity package
    new UnityDependencyConfigurator(container),
    kafka => kafka
        // You must implement IlogHandler interface and replace YourLogHandler
        .UseLogHandler<YourLogHandler>() 
        .AddCluster(
            cluster => cluster
                .WithBrokers(new[] { "localhost:9092" })
                .AddConsumer(
                    consumer => consumer
                        .Topic("test-topic")
                        .WithGroupId("print-console-handler")
                        .WithBufferSize(100)
                        .WithWorkersCount(10)
                        .WithAutoOffsetReset(AutoOffsetReset.Latest)
                        .AddMiddlewares(
                            middlewares => middlewares
                                // Install KafkaFlow.Compressor and Install KafkaFlow.Compressor.Gzip packages
                                .AddCompressor<GzipMessageCompressor>() 
                                // Install KafkaFlow.Serializer and Install KafkaFlow.Serializer.Protobuf packages
                                // You must implement IMessageTypeResolver and replace YourMessageTypeResolver
                                .AddSerializer<ProtobufMessageSerializer, YourMessageTypeResolver>()
                                // Install KafkaFlow.TypedHandler package
                                .AddTypedHandlers(
                                    handlers => handlers
                                        .WithHandlerLifetime(InstanceLifetime.Singleton)
                                        .AddHandler<PrintConsoleHandler>())
                        )
                )
                // The PrintConsoleProducer class is needed to bind the producer configuration with the
                // IMessageProducer<PrintConsoleProducer> instance that will produce the messages (see the samples)
                .AddProducer<PrintConsoleProducer>(
                    producer => producer
                        .DefaultTopic("test-topic")
                        .AddMiddlewares(
                            middlewares => middlewares
                                .AddSerializer<ProtobufMessageSerializer, YourMessageTypeResolver>()
                                .AddCompressor<GzipMessageCompressor>()
                        )
                )
        )
);

//Call bus.StartAsync() when your app starts and bus.StopAsync() when your app stops
var bus = configurator.CreateBus(new UnityDependencyResolver(container));
```

-  See the [samples](/samples) folder for more usages
-  You can use other dependency injection frameworks implementing the *IDependencyConfigurator*, *IDependencyResolver* and *IDependencyResolverScope* interfaces

## Contributing

1.  Fork this repository
2.  Follow project guidelines
3.  Do your stuff
4.  Open a pull request using the template

### Disclaimer

By sending us your contributions, you are agreeing that your contribution is made subject to the terms of our [Contributor Ownership Statement](https://github.com/Farfetch/.github/blob/master/COS.md)

## Maintainers

*  [filipeesch](https://github.com/filipeesch)
*  [dougolima](https://github.com/dougolima)

## License

[MIT](LICENSE)
