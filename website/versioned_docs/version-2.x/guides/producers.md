---
sidebar_position: 2
---

# Producers

In this section, we will learn how to add and configure a Producer on KafkaFlow.

To produce messages using KafkaFlow, Producers need to be configured in the application [configuration](configuration). 

The producers also support [Middlewares](middlewares). 

You have two ways to configure the producers: 
 - [Name-based producer](#named-producers) 
 - [Type-based producer](#type-based-producers)
 
:::tip
It's highly recommended to read [Confluent Producer documentation](https://github.com/confluentinc/confluent-kafka-dotnet/wiki/Producer) for better practices when producing messages.
:::

## Name-based producers

Uses a name to bind the configuration to the producer instance.
Use the name as a key to access the Producer when you want to produce a message.

```csharp
using KafkaFlow;
using KafkaFlow.Producers;
using Microsoft.Extensions.DependencyInjection;

services.AddKafka(kafka => kafka
    .AddCluster(cluster => cluster
        .WithBrokers(new[] { "localhost:9092" })
        .AddProducer(
            "product-events", //the producer name
            producer => 
                producer
        )
    )
);

public class ProductService : IProductService
{
    private readonly IProducerAccessor _producers;

    public ProductService(IProducerAccessor producers)
    {
        _producers = producers;
    }

    public async Task CreateProduct(Product product) =>
        await _producers["product-events"]
            .ProduceAsync(product.Id.ToString(), product);     
}
```

## Type-based producers

Uses a class to bind the configuration to the producer instance, this is commonly used when you create a producer class to decouple the framework from your service classes. 

For example, if you have a `ProductEventsProducer` in your app, you can use this class when configuring the producer to bind the configuration with the instance of `IMessageProducer<ProductEventsProducer>`.

```csharp
using KafkaFlow;
using KafkaFlow.Producers;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

services.AddKafka(kafka => kafka
    .AddCluster(cluster => cluster
        .WithBrokers(new[] { "localhost:9092" })
        .AddProducer<ProductEventsProducer>(
            producer => 
                producer
                ...
            )
    )
);

public class ProductEventsProducer : IProductEventsProducer
{
    private readonly IMessageProducer<ProductEventsProducer> _producer;

    public ProductEventsProducer(IMessageProducer<ProductEventsProducer> producer)
    {
        _producer = producer;
    }

    public Task ProduceAsync(Product product) =>
        _producer
            .ProduceAsync(product.Id.ToString(), product);
}
```

## How to produce a message to a given topic

There are two ways to specify the destination Topic of a message.

You can specify it as the first argument when the Produce method is invoked, as shown in the following example:

```csharp
await _producers["product-events"]
    .ProduceAsync("products-topic", product.Id.ToString(), product);
```           

You can also set the Default Topic that a Producer should produce to. 
You can do that, on the producer configuration.

```csharp
using KafkaFlow;
using KafkaFlow.Producers;
using Microsoft.Extensions.DependencyInjection;

services.AddKafka(kafka => kafka
    .AddCluster(cluster => cluster
        .WithBrokers(new[] { "localhost:9092" })
        .AddProducer(
            "product-events",
            producer => 
                producer
                    .DefaultTopic("products-topic")
        )
    )
);
```

## How to produce a message without Message Key

You can send the message key argument as `null` when the Produce method is invoked, as shown in the following example:

```csharp
await producer.ProduceAsync(null, product);
```      

## How to configure ACKS when publishing a message

An Ack is an acknowledgment that the producer receives from the broker to ensure that the message has been successfully committed.

The following table establishes the mapping between KafkaFlow and Kafka. You can find [here](https://docs.confluent.io/platform/current/installation/configuration/producer-configs.html#producerconfigs_acks) the meaning of each of those values.

| KafkaFlow | Kafka |
| -- | -- |
| Acks.None | `acks=0` |
| Acks.Leader | `acks=1` |
| Acks.All | `acks=all` |


```csharp
using KafkaFlow;
using KafkaFlow.Producers;
using Microsoft.Extensions.DependencyInjection;

services.AddKafka(kafka => kafka
    .AddCluster(cluster => cluster
        .WithBrokers(new[] { "localhost:9092" })
        .AddProducer(
            "product-events",
            producer => 
                producer
                    .WithAcks(Acks.Leader)
        )
    )
);
```

## How to customize compression

You can find more information in the [Compression guide](compression). 

