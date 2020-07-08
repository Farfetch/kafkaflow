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

### .NET Core

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

### .NET Framework or other DI containers

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

## Workers

Workers are responsible for processing the messages when consuming. You define how many workers a consumer will have. The workers process the messages in parallel, and by default, messages with the same partition key are processed by the same worker, so that the message order is respected for the same partition key. Every worker has a buffer to avoid idling when many messages arrive with the same partition key for any other worker. The buffer size should be dimensioned depending on how many messages arrive with the same partition key, on average. When the bus is requested to stop, every worker receives the stop command and it only releases the stop call when it ends the current message and stores it in the OffsetManager. The OffsetManager is a component that receives all the offsets from the workers and orchestrates them before storing into Kafka; this avoids an offset override when we have many messages being processed at the same time. Even when you choose to use the manual offset store option, you will store the offset in the OffsetManager, and will then store the offsets in Kafka when possible. When the application stops, there is a big chance to have processed messages already stored in OffsetManager but not stored in Kafka. In this scenario, when the application starts again, these messages will be processed again. Your application must be prepared to deal with it.

## It's all about middlewares

KafkaFlow is very middleware-oriented. Messages will be delivered to a middleware, will then be passed to another middleware, and so on. The middlewares can transform the messages, allowing them to apply serialization and compression, for example. You can log the messages, handle exceptions, apply retry policies (maybe using [Polly](https://github.com/App-vNext/Polly)), collect metrics, and many other things. **The middlewares are executed in the same order that they are defined in the configuration. Every product/consumer has its own middlewares instances, so, the instances are not shared between different consumers/producers, but when consuming, the instances are shared between the workers of the same consumer**

Every middleware must implement the `IMessageMiddleware` interface. You must implement the method `Invoke`, which receives two parameters: the `IMessageContext` and the `MiddlewareDelegate`. The `IMessageContext` is an object that has the message, message metadata, and consumer/producer information. The `MiddlewareDelegate` is a `async` method that calls the next middleware; you can short-circuit the message execution by just not calling the next middleware or putting it inside a `try` block so you can catch any exceptions thrown by the next middlewares.

When consuming, the message will be delivered as a byte array to the first middleware; you will choose the middlewares that will process the messages (you will probably create a middleware to do it or use the [Typed Handler](https://www.nuget.org/packages/KafkaFlow.TypedHandler) depending on the message type). The next message will only be processed after the execution of all the configured middlewares for that consumer.

When producing, the middlewares are called when the `Produce` or `PoduceAsync` of the `IMessageProducer<YourProducer>` is called. After the execution of all middlewares, the message will be published to Kafka. A `YourProducer` class is needed when configuring the producer to bind the producer configuration to the `IMessageProducer<YourProducer>` instance that will be delivered through dependency injection; you can use any arbitrary type, but we highly recommend that you use a type that conveys what the producer does, like `ProductCommandProducer`. This allows the client app to create two producers with different middlewares configuration to produce messages for the same topic, for example.


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
