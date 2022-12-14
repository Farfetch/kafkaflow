---
sidebar_position: 2
---

# Add Consumers

In this section, we will learn how to add and configure a Consumer on KafkaFlow.

To add a consumer, you need to configure the Topic that the consumer will be listening to and the Consumer Group that will be part of. You can find an example below.


```csharp
using KafkaFlow;
using Microsoft.Extensions.DependencyInjection;

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

## Automatic Partitions Assignment

Using the `Topic()` or `Topics()` methods, the consumer will trigger the automatic partition assignment that will distribute the topic partitions across the application instances.


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
            ...
        )
    )
);
```

## Manual Partitions Assignment

The client application can specify the topic partitions manually using the `ManualAssignPartitions()` method instead of using the `Topic()` or `Topics()` methods.


```csharp
using KafkaFlow;
using KafkaFlow.Serializer;
using Microsoft.Extensions.DependencyInjection;
using KafkaFlow.TypedHandler;

services.AddKafka(kafka => kafka
    .AddCluster(cluster => cluster
        .WithBrokers(new[] { "localhost:9092" })
        .AddConsumer(consumer => consumer
            .ManualAssignPartitions("topic-name", new[] { 1, 2, 3 })
            ...
        )
    )
);
```

## Offset Strategy

You can configure the Offset Strategy for a consumer group in case the Consumer Group has no offset stored in Kafka. 
There are two options:
 - Earliest: Reads from the beginning of the Topic.
 - Latest: Only reads new messages.

```csharp
using KafkaFlow;
using Microsoft.Extensions.DependencyInjection;

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
using Microsoft.Extensions.DependencyInjection;

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

## Disable Store Offsets

In a scenario where storing offsets it's not needed,  you can disable it by calling the `WithoutStoringOffsets()` method in the consumer setup.


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
            .WithoutStoringOffsets()
            ...
        )
    )
);
```
