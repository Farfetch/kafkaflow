---
sidebar_position: 2
---

# Add Consumers

In this section, we will learn how to add and configure a Consumer on KafkaFlow.

To add a consumer, you need to configure the Topic that the consumer will be listening to and the Consumer Group that will be part of. You can find an example below.


```csharp
using KafkaFlow;
using KafkaFlow.Serializer;
using Microsoft.Extensions.DependencyInjection;
using KafkaFlow.TypedHandler;

services.AddKafka(kafka => kafka
    .AddCluster(cluster => cluster
        .WithBrokers(new[] { "localhost:9092" })
        .AddConsumer(consumer => consumer
            .Topic("topic-name")
            .WithGroupId("sample-group")
            .WithBufferSize(100)
            .WithWorkersCount(10)
            .AddMiddlewares(middlewares => middlewares
                ...
            )
        )
    )
);
```

On a Consumer, you can configure the Middlewares that will be invoked. You can find more information on how to configure Middlewares [here](../middlewares).


## Offset Strategy


You can configure the Offset Strategy for a consumer group in case the Consumer Group has no offset stored in Kafka. 
There are two options:
 - Earliest: Reads from the beginning of the Topic.
 - Latest: Only reads new messages.

```csharp
using KafkaFlow;
using KafkaFlow.Serializer;
using Microsoft.Extensions.DependencyInjection;
using KafkaFlow.TypedHandler;

services.AddKafka(kafka => kafka
    .AddCluster(cluster => cluster
        .WithBrokers(new[] { "localhost:9092" })
        .AddConsumer(consumer => consumer
            .Topic("topic-name")
            .WithGroupId("sample-group")
            .WithAutoOffsetReset(AutoOffsetReset.Earliest)
            ...
        )
    )
);
```

## Store Offsets Manually

By default, offsets are stored after the handler and middleware execution automatically.
This is the equivalent to configure the consumer as `WithAutoStoreOffsets()`.

To control Offset storing, it's possible to configure using `WithManualStoreOffsets()` as the following example:


```csharp
using KafkaFlow;
using KafkaFlow.Serializer;
using Microsoft.Extensions.DependencyInjection;
using KafkaFlow.TypedHandler;

services.AddKafka(kafka => kafka
    .AddCluster(cluster => cluster
        .WithBrokers(new[] { "localhost:9092" })
        .AddConsumer(consumer => consumer
            .Topic("topic-name")
            .WithGroupId("sample-group")
            .WithManualStoreOffsets()
            ...
        )
    )
);
```
