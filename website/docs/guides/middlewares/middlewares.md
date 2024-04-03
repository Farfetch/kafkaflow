---
sidebar_position: 1
---

# Middleware Introduction

In this section, we will learn what Middlewares are and how to create them.

KafkaFlow is middleware-oriented. Messages are delivered to a middleware and then forwarded to another middleware, and so on. It's a middleware pipeline that will be invoked in sequence.

:::info

Middlewares are executed in the same order they are defined in the configuration. 
Every product/consumer has its own Middlewares instances, so the instances are not shared between different consumers/producers, but when consuming, the instances are shared between the workers of the same consumer. 

:::

Middlewares are instantiated by the configured dependency injection container, so every dependency configured by your container can be delivered in the middleware constructor.

:::info

Use overloads of the `Add<TMiddleware>(MiddlewareLifetime)` method when registering middlewares with your DI container to control middleware instance lifetime.

:::

## Use Cases

Middlewares can perform several jobs. As an example, Middlewares can be used to:

-   Transform the messages, allowing them to apply serialization and compression. 
-   Log the messages
-   Handle exceptions
-   Apply retry policies (take a look into [KafkaFlow Retry Extensions](https://github.com/Farfetch/kafkaflow-retry-extensions))
-   Collect metrics
-   etc. 

## Built-in middlewares

KafkaFlow provides the following middlewares out of the box:

-   [Serializer](serializer-middleware)
-   [Compressor](compressor-middleware)
-   [Typed Handler](typed-handler-middleware)
-   [Batch Consume](batch-consume-middleware)
-   [Consumer Throttling](consumer-throttling-middleware)

## When Consuming​

The message will be delivered as a byte array to the first middleware; you will choose the middlewares to process the messages (you will probably create a middleware to do it or use the [Typed Handler](typed-handler-middleware)). The next message will only be processed after all configured middlewares execute for that consumer.

## When Producing

The middlewares are called when the `Produce` or `ProduceAsync` of the `IMessageProducer` is called. After all the middlewares execute, the message will be published to Kafka.

## Creating a middleware

You can create your own middlewares by implementing the `IMessageMiddleware` interface. You must implement the `Invoke` method, which receives two parameters: the `IMessageContext` and the `MiddlewareDelegate`. The `IMessageContext` is an object that has the message, message metadata, and consumer/producer information. The `MiddlewareDelegate` is an `async` method that calls the next middleware, it receives an `IMessageContext` parameter which is the context that will be passed to the next middleware.

### Some middleware samples

#### Log messages and processing time

```csharp
public class LoggingMiddleware : IMessageMiddleware
{
    private readonly ILogger log;

    public LoggingMiddleware(ILogger log)
    {
        this.log = log ?? throw new ArgumentNullException(nameof(log));
    }

    public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
    {
        this.log.Info("Kafka Message Received");

        var sw = Stopwatch.StartNew();

        await next(context).ConfigureAwait(false);

        sw.Stop();

        this.log.Info(
            "Kafka Message processed",
            () => new
            {
                MessageType = context.Message?.GetType().FullName,
                ProcessingTime = sw.ElapsedMilliseconds
            });
    }
}
```

#### Log any exception

```csharp
public class ErrorHandlingMiddleware : IMessageMiddleware
{
    private readonly ILogger log;

    public ErrorHandlingMiddleware(ILogger log)
    {
        this.log = log ?? throw new ArgumentNullException(nameof(log));
    }

    public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
    {
        try
        {
            await next(context).ConfigureAwait(false);
        }
        catch(Exception ex)
        {
            this.log.Error("Error processing message", ex);
        }
    }
}
```

#### Ignore messages

```csharp
public class IgnoreMessageMiddleware : IMessageMiddleware
{
    public Task Invoke(IMessageContext context, MiddlewareDelegate next)
    {
        return UnwantedMessage(context) ?
            Task.CompletedTask :
            next(context);
    }

    private bool UnwantedMessage(IMessageContext context)
    {
        ...
    }
}
```

#### Transform messages

:::info

This is only a sample, use the [Serializer Middleware](serializer-middleware) instead.

:::

```csharp
public class JsonDeserializeMiddleware : IMessageMiddleware
{
    public Task Invoke(IMessageContext context, MiddlewareDelegate next)
    {
        if(!(context.Message is byte[] rawMessage))
            throw new InvalidOperationException();

        var type = Type.GetType(context.Headers.GetString("Message-Type"));

        var jsonMessage = Encoding.UTF8.GetString(rawMessage);

        context.TransformMessage(JsonConvert.Deserialize(jsonMessage, MessageType));

        return next(context);
    }
}
```

#### Pause Consumer when an exception is raised

Add the following middleware to the beginning of your Consumer middleware pipeline.

:::info
This middleware is part of a sample you can find [here](https://github.com/Farfetch/kafkaflow/tree/master/samples/KafkaFlow.Sample.PauseConsumerOnError).
:::

```csharp
using KafkaFlow.Consumers;

public class PauseConsumerOnExceptionMiddleware : IMessageMiddleware
{
    private readonly IConsumerAccessor consumerAccessor;
    private readonly ILogHandler logHandler;

    public PauseConsumerOnExceptionMiddleware(IConsumerAccessor consumerAccessor, ILogHandler logHandler)
    {
        this.consumerAccessor = consumerAccessor;
        this.logHandler = logHandler;
    }

    public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            context.ConsumerContext.ShouldStoreOffset = false;
            this.logHandler.Error("Error handling message", exception,
                new
                {
                    context.Message,
                    context.ConsumerContext.Topic,
                    MessageKey = context.Message.Key,
                    context.ConsumerContext.ConsumerName,
                });

            var consumer = this.consumerAccessor[context.ConsumerContext.ConsumerName];
            consumer.Pause(consumer.Assignment);

            this.logHandler.Warning("Consumer stopped", context.ConsumerContext.ConsumerName);
        }
    }
}
```
