---
sidebar_position: 3
---

# Serializer Middleware

In this section, we will learn how to use the Serializer Middleware.

The Serializer Middleware is used to serialize and deserialize messages.

You can use one of the following common serializers or build your own:
-   [KafkaFlow.Serializer.ProtoBufNet](https://www.nuget.org/packages/KafkaFlow.Serializer.ProtobufNet/)
-   [KafkaFlow.Serializer.JsonCore](https://www.nuget.org/packages/KafkaFlow.Serializer.JsonCore/)
-   [KafkaFlow.Serializer.NewtonsoftJson](https://www.nuget.org/packages/KafkaFlow.Serializer.NewtonsoftJson/)

## How to use it

On the configuration, add the `AddSerializer`/`AddDeserializer` extension method to your producer/consumer middlewares to use it. 

The `AddSerializer`/`AddDeserializer` method has two arguments: 
 - The first one must implement the `ISerializer`/`IDeserializer` interface. 
 - The second one is optional and must implement the `IMessageTypeResolver` interface. If the parameter is not provided, then the `DefaultTypeResolver` will be used. 
Both classes can be provided as an argument through a factory method too. 

:::tip
For topics that have just one message type, use the `AddSingleTypeSerializer`/`AddSingleTypeDeserializer` method.
:::


```csharp
services.AddKafka(kafka => kafka
    .AddCluster(cluster => cluster
        .WithBrokers(new[] { "localhost:9092" })
        .AddProducer<ProductEventsProducer>(producer => producer
            ...
            .AddMiddlewares(middlewares => middleware
                ...
                .AddSerializer<JsonMessageSerializer>() // Using the DefaultMessageTypeResolver
                // or
                .AddSerializer<JsonMessageSerializer, YourTypeResolver>()
                // or
                .AddSerializer(
                    resolver => new JsonMessageSerializer(...),
                    resolver => new YourTypeResolver(...))
                // or
                .AddSingleTypeSerializer<JsonMessageSerializer, YourMessageType>()
                // or
                .AddSingleTypeSerializer<YourMessageType>(resolver => new JsonMessageSerializer(...))
                ...
            )
        )
    )
);

```

## Adding Schema Registry support
Serializer middlewares can be used along with schema registry allowing the evolution of schemas according to the configured compatibility setting.

Install the [KafkaFlow.SchemaRegistry](https://www.nuget.org/packages/KafkaFlow.SchemaRegistry/) package, configure the schema registry broker, and use one of the following packages to use all the schema registry integration features.

-   [KafkaFlow.Serializer.SchemaRegistry.ConfluentJson](https://www.nuget.org/packages/KafkaFlow.Serializer.SchemaRegistry.ConfluentJson/)
-   [KafkaFlow.Serializer.SchemaRegistry.ConfluentAvro](https://www.nuget.org/packages/KafkaFlow.Serializer.SchemaRegistry.ConfluentAvro/)
-   [KafkaFlow.Serializer.SchemaRegistry.ConfluentProtobuf](https://www.nuget.org/packages/KafkaFlow.Serializer.ConfluentProtobuf/)

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddKafka(
            kafka => kafka
                .AddCluster(
                    cluster => cluster
                        .WithBrokers(new[] { "localhost:9092" })
                        .WithSchemaRegistry(config => config.Url = "localhost:8081")
                        .AddProducer(
                            ...
                            .AddMiddlewares(middlewares => 
                                    middlewares.AddSchemaRegistryAvroSerializer(new AvroSerializerConfig{ SubjectNameStrategy = SubjectNameStrategy.TopicRecord })
                        )
                       .AddConsumer(
                            ...
                            .AddMiddlewares(middlewares => middlewares.AddSchemaRegistryAvroDeserializer()
                        )
                    )
            );
    }
}
```
:::info
[ConfluentAvro](https://www.nuget.org/packages/KafkaFlow.Serializer.SchemaRegistry.ConfluentAvro/) and [ConfluentProtobuf](https://www.nuget.org/packages/KafkaFlow.Serializer.ConfluentProtobuf/) type resolvers can support multiple types per topic however, due to the JSON serialization format used by [confluent-kafka-dotnet](https://docs.confluent.io/platform/current/clients/confluent-kafka-dotnet/_site/api/Confluent.SchemaRegistry.Serdes.JsonSerializer-1.html), [ConfluentJson](https://www.nuget.org/packages/KafkaFlow.Serializer.SchemaRegistry.ConfluentJson/) type resolver can only resolve a single type of message per topic. 
:::

:::info
To be able to publish multiple type messages per topic, `SubjectNameStrategy.Record` or `SubjectNameStrategy.TopicRecord` must be used. 
You can see a detailed explanation [here](https://docs.confluent.io/platform/current/schema-registry/serdes-develop/index.html#subject-name-strategy).
:::


## Creating a Message Type Resolver

A type resolver is needed to instruct the middleware where to find the destination message type in the message metadata when consuming and where to store it when producing. 

The framework has the `DefaultTypeResolver` that will be used omitting the second type parameter in the `AddSerializer`/`AddDeserializer` method. You can create your own implementation of `IMessageTypeResolver` to allow communication with other frameworks.

```csharp
public class SampleMessageTypeResolver : IMessageTypeResolver
{
    private const string MessageType = "MessageType";

    public Type OnConsume(IMessageContext context)
    {
        var typeName = context.Headers.GetString(MessageType);

        return Type.GetType(typeName);
    }

    public void OnProduce(IMessageContext context)
    {
        context.Headers.SetString(
            MessageType,
            $"{context.Message.GetType().FullName}, {context.Message.GetType().Assembly.GetName().Name}");
    }
}
```
