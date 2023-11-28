---
sidebar_position: 5
---

# Batch Consume Middleware

In this section, we will learn how to use the Batch Consume Middleware.

The Batch Consume Middleware is used to accumulate a number of messages or wait some time to build a collection of messages and deliver them to the next middleware to be processed, as it was just one message.

## How to use it

Install the [KafkaFlow.BatchConsume](https://www.nuget.org/packages/KafkaFlow.BatchConsume) package. 

```bash
dotnet add package KafkaFlow.BatchConsume
```

On the configuration, use the `BatchConsume` extension method to add the middleware to your consumer middlewares. 

The `BatchConsume` method has two arguments: 
 - The first one must define the maximum batch size. 
 - The second one defines the `TimeSpan` that the Middleware waits for new messages to be part of the batch.


```csharp
services.AddKafka(kafka => kafka
    .AddCluster(cluster => cluster
        .WithBrokers(new[] { "localhost:9092" })
        .AddConsumer(
            consumerBuilder => consumerBuilder
            ...
            .AddMiddlewares(
                middlewares => middlewares
                    ...
                    .BatchConsume(100, TimeSpan.FromSeconds(10)) // Configuration of the BatchConsumeMiddleware
                    .Add<HandlingMiddleware>() // Middleware to process the batch
            )
        )
    )
);
```

To access the batch from the next middleware, use the `GetMessagesBatch` method accessible through the `context` argument.

:::warning
When using the `Batch Consume` middleware, the `IServiceScopeFactory` should be used to create scopes instead of the `IServiceProvider`, as the latter may dispose the scope.
:::

```csharp
using KafkaFlow.BatchConsume;

internal class HandlingMiddleware : IMessageMiddleware
{
    public Task Invoke(IMessageContext context, MiddlewareDelegate next)
    {
        var batch = context.GetMessagesBatch();
        
        (...)

        return Task.CompletedTask;
    }
}
```
:::tip
You can find a sample on batch processing [here](https://github.com/Farfetch/kafkaflow/tree/master/samples/KafkaFlow.Sample.BatchOperations).
:::