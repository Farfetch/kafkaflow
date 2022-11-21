---
sidebar_position: 2
---

# Add Consumers

In this section, we will learn how to add and configure a Consumer on KafkaFlow.

To add a consumer you need to configure the Topic that the consumer will be listening to and also the Consumer Group that it will be part of. You can find an example below.


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
