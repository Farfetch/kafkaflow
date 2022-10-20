---
sidebar_position: 10
---

# Producers

To produce messages using KafkaFlow you have to configure the producers in the application setup. The producers also support [Middlewares](middlewares). You have two ways to configure the producers: [name-based producer](#named-producers) and [type-based producer](#type-based-producers). I highly recommend the read of [Confluent Producer documentation](https://github.com/confluentinc/confluent-kafka-dotnet/wiki/Producer) for better practices when producing messages.

## Name-based producers

Uses a name to bind the configuration to the producer instance.

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddKafka( kafka => kafka
            .AddCluster(cluster => cluster
                .WithBrokers(new[] { "localhost:9092" })
                .AddProducer(
                    "product-events" //the producer name
                    producer => producer
                    ...
                )
            )
        );
    }
}

public class ProductService : IProductService
{
    private readonly IProducerAccessor producers;

    public ProductEventsProducer(IProducerAccessor producers)
    {
        this.producers = producers;
    }

    public Task CreateProduct(Create)
    {
        ...
        await this.producers["product-events"].ProduceAsync(event.ProductId.ToString(), event);
        ...
    }        
}
```

## Type-based producers

Uses a class to bind the configuration to the producer instance, this is commonly used when you create a producer class to uncouple the framework stuff of your service classes. For example, you have a `ProductEventsProducer` in your app, you can use this class when configuring the producer to bind the configuration with the instance of `IMessageProducer<ProductEventsProducer>`.

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddKafka(kafka => kafka
            .AddCluster(cluster => cluster
                .WithBrokers(new[] { "localhost:9092" })
                .AddProducer<ProductEventsProducer>(
                    producer => producer
                    ...
                )
            )
        );
    }
}

public class ProductEventsProducer : IProductEventsProducer
{
    private readonly IMessageProducer<ProductEventsProducer> producer;

    public ProductEventsProducer(IMessageProducer<ProductEventsProducer> producer)
    {
        this.producer = producer;
    }

    public Task ProduceAync(IProductEvent event) =>
        this.producer.ProduceAsync(event.ProductId.ToString(), event);
}
```
