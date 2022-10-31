---
sidebar_position: 11
---

# Serializer Middleware

It's a middleware used to serialize and deserialize messages. Install the [KafkaFlow.Serializer](https://www.nuget.org/packages/KafkaFlow.Serializer/) package and add the `AddSerializer` extension method to your producer/consumer middlewares to use it. The method can receive two classes as generic arguments. The first one must implement the `IMessageSerializer` interface. The second one **is optional** and must implement the `IMessageTypeResolver` interface, if the parameter is not passed then the `DefaultTypeResolver` will be used. Both classes can be passed as a normal argument through a factory method too. For topics that have just one message type use the `AddSingleTypeSerializer` method.

#### These packages provide some commonly used serialization algorithms

-   [KafkaFlow.Serializer.ProtoBufNet](https://www.nuget.org/packages/KafkaFlow.Serializer.ProtobufNet/)
-   [KafkaFlow.Serializer.JsonCore](https://www.nuget.org/packages/KafkaFlow.Serializer.JsonCore/)
-   [KafkaFlow.Serializer.NewtonsoftJson](https://www.nuget.org/packages/KafkaFlow.Serializer.NewtonsoftJson/)

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
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
    }
}
```

## Schema Registry support
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
                            .AddMiddlewares(middlewares => middlewares.AddSchemaRegistryAvroSerializer()
                        )
                    )
            );
    }
}
```
[ConfluentAvro](https://www.nuget.org/packages/KafkaFlow.Serializer.SchemaRegistry.ConfluentAvro/) and [ConfluentProtobuf](https://www.nuget.org/packages/KafkaFlow.Serializer.ConfluentProtobuf/) type resolvers can support multiple types per topic however, due to json serialization format used by [confluent-kafka-dotnet](https://docs.confluent.io/platform/current/clients/confluent-kafka-dotnet/_site/api/Confluent.SchemaRegistry.Serdes.JsonSerializer-1.html), [ConfluentJson](https://www.nuget.org/packages/KafkaFlow.Serializer.SchemaRegistry.ConfluentJson/) type resolver can only resolve a single type of message per topic. 

In order to be able to publish multiple type messages per topic, `SubjectNameStrategy.Record` or `SubjectNameStrategy.TopicRecord` must be used. 

You can see a detailed explanation [here](https://docs.confluent.io/platform/current/schema-registry/serdes-develop/index.html#subject-name-strategy).



## Message Type Resolver

A type resolver is needed to instruct the middleware where to find the destination message type in the message metadata when consuming and where to store it when producing. The framework has the `DefaultTypeResolver` that will be used omitting the second type parameter in the `AddSerializer` method. You can create your own implementation of `IMessageTypeResolver` to allow communication with other frameworks.

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
