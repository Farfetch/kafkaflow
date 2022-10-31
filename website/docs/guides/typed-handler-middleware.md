---
sidebar_position: 12
---

# Typed Handler Middleware

It's a middleware that calls different classes (the handlers) depending on the message type. It should be used when the topic has different messages types. When a message with a specific type arrives, the middleware will call the appropriate message handler for that message type. A message handler can be created by implementing the `IMessageHandler<MessageType>` interface. The handler's instance is created by the configured dependency injection container, any handler dependency will be injected through the constructor and the instance lifetime can be configured in the setup process. If there is no handler defined for the arriving message, it will be ignored.

### Configuring

There are three ways to add handlers to a consumer:
-   `AddHandler<HandlerType>()` adds one handler per call
-   `AddHandlers(IEnumerable<Type> handlers)` adds many handlers per call
-   `AddHandlersFromAssemblyOf<HandlerType>()` adds all classes that implements `IMessageHandler` interface of the assembly's given type

```csharp
services.AddKafka(kafka => kafka
    .AddCluster(cluster => cluster
        .WithBrokers(new[] { "localhost:9092" })
        .AddConsumer(consumer => consumer
            ...
            .AddMiddlewares(middlewares => middlewares
                ...
                .AddTypedHandlers(handlers => handlers
                    .WithHandlerLifetime(InstanceLifetime.Singleton)
                    .AddHandler<ProductCreatedHandler>()
                    // or
                    .AddHandlers( ... )
                    // or
                    .AddHandlersFromAssemblyOf<ProductCreatedHandler>()
                )
            )
        )
    )
);
```

### Message Handler sample

```csharp
public class ProductCreatedHandler : IMessageHandler<ProductCreatedEvent>
{
    public Task Handle(IMessageContext context, ProductCreatedEvent message)
    {
        ...
    }
}
```