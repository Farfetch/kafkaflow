---
sidebar_position: 2
---

# Typed Handler Middleware

In this section, we will learn how to use the Typed Handler middleware.

The Typed Handler Middleware allows you to execute different handlers depending on the message type. 

:::tip
Use it when the topic has different message types. 
:::

When a message with a given type arrives, the middleware will call the appropriate message handler for that message type. 


## Configure Typed Handler

There are three ways to add handlers to a consumer:
-   **`AddHandler<HandlerType>()`:** adds one handler per call.
-   **`AddHandlers(IEnumerable<Type> handlers)`:** adds many handlers per call.
-   **`AddHandlersFromAssemblyOf<HandlerType>()`:** adds all classes on the given assembly type that implement the `IMessageHandler` interface.

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

## Create a Message Handler

A message handler can be created by implementing the `IMessageHandler<MessageType>` interface. 

The handler's instance is created by the configured dependency injection container, any handler dependency will be injected through the constructor, and the instance lifetime can be configured in the configuration. 

:::warning
If there's no handler defined for the arriving message, it will be ignored.
:::

```csharp
public class ProductCreatedHandler : IMessageHandler<ProductCreatedEvent>
{
    public Task Handle(IMessageContext context, ProductCreatedEvent message)
    {
        ...
    }
}
```

## Configuring Handler Lifetime

The Handler lifetime can be configured to one of the following modes:
 - **Singleton:** A single class instance will be created for the entire application.
 - **Scoped:** A new class instance will be created for each scope.
 - **Transient:** A new class instance will be created every time it is requested.

:::info
By default, the handler lifetime is Singleton.
:::

## Handling No Handler Found event

If there's no handler defined for the arriving message, it will be ignored.

It is possible to handle those events. As an example, the following code writes to the console when a message can't be handled.


```csharp
services.AddKafka(kafka => kafka
    .AddCluster(cluster => cluster
        .WithBrokers(new[] { "localhost:9092" })
        .AddConsumer(consumer => consumer
            ...
            .AddMiddlewares(middlewares => middlewares
                ...
                .AddTypedHandlers(handlers => handlers
                    .AddHandler<ProductCreatedHandler>()
                    .WhenNoHandlerFound(context => 
                        Console.WriteLine("Message not handled > Partition: {0} | Offset: {1}",
                            context.ConsumerContext.Partition,
                            context.ConsumerContext.Offset)
                        )
                )
            )
        )
    )
);
```