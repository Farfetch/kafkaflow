---
sidebar_position: 8
---

# Middlewares
    
KafkaFlow is very middleware-oriented. Messages will be delivered to a middleware, will then be passed to another middleware, and so on. The middlewares can transform the messages, allowing them to apply serialization and compression, for example. You can log the messages, handle exceptions, apply retry policies (maybe using [Polly](https://github.com/App-vNext/Polly)), collect metrics, and many other things. **The middlewares are executed in the same order that they are defined in the configuration. Every product/consumer has its own middlewares instances, so, the instances are not shared between different consumers/producers, but when consuming, the instances are shared between the workers of the same consumer**. The middlewares are instantiated by the configured dependency injection container, so every dependency configured by your container can be delivered in the middleware constructor.

### When consuming

The message will be delivered as a byte array to the first middleware; you will choose the middlewares that will process the messages (you will probably create a middleware to do it or use the [Typed Handler](typed-handler-middleware.md)). The next message will only be processed after the execution of all the configured middlewares for that consumer.

### When producing

The middlewares are called when the `Produce` or `PoduceAsync` of the `IMessageProducer` is called. After the execution of all middlewares, the message will be published to Kafka.

## Built-in middlewares

-   [Serializer](serializer-middleware)
-   Compressor
-   [Typed Handler](typed-handler-middleware)

## Creating a middleware

You can create your own middlewares implementing the `IMessageMiddleware` interface. You must implement the method `Invoke`, which receives two parameters: the `IMessageContext` and the `MiddlewareDelegate`. The `IMessageContext` is an object that has the message, message metadata, and consumer/producer information. The `MiddlewareDelegate` is a `async` method that calls the next middleware, it receives an `IMessageContext` parameter that is the context that will be passed to the next middleware.

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

```csharp
public class JsonDeserializeMiddleware : IMessageMiddleware
{
    public Task Invoke(IMessageContext context, MiddlewareDelegate next)
    {
        if(!(context.Message is byte[] rawMessage))
            throw new InvalidoperationException();

        var type = Type.GetType(context.Headers.GetString("Message-Type"));

        var jsonMessage = Encoding.UTF8.GetString(rawMessage);

        context.TransformMessage(JsonConvert.Deserialize(jsonMessage, MessageType));

        return next(context);
    }
}
```

*this is only a sample, use the [Serializer Middleware](serializer-middleware.md) to do this.
