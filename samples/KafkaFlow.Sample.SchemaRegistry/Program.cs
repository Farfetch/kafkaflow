using System;
using System.Threading.Tasks;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using KafkaFlow;
using KafkaFlow.Producers;
using KafkaFlow.Sample.SchemaRegistry.Handlers;
using Microsoft.Extensions.DependencyInjection;
using SchemaRegistry;

var services = new ServiceCollection();

const string avroProducerName = "avro-producer";
const string jsonProducerName = "json-producer";
const string protobufProducerName = "protobuf-producer";
const string avroTopic = "avro-topic";
const string jsonTopic = "json-topic";
const string protobufTopic = "protobuf-topic";

services.AddKafka(
    kafka => kafka
        .UseConsoleLog()
        .AddCluster(
            cluster => cluster
                .WithBrokers(new[] {"localhost:9092"})
                .WithSchemaRegistry(config => config.Url = "localhost:8081")
                .CreateTopicIfNotExists(avroTopic, 1, 1)
                .CreateTopicIfNotExists(jsonTopic, 1, 1)
                .CreateTopicIfNotExists(protobufTopic, 1, 1)
                .AddProducer(
                    avroProducerName,
                    producer => producer
                        .DefaultTopic(avroTopic)
                        .AddMiddlewares(
                            middlewares => middlewares
                                .AddSchemaRegistryAvroSerializer(
                                    new AvroSerializerConfig
                                    {
                                        SubjectNameStrategy = SubjectNameStrategy.TopicRecord
                                    }))
                )
                .AddProducer(
                    jsonProducerName,
                    producer => producer
                        .DefaultTopic(jsonTopic)
                        .AddMiddlewares(
                            middlewares => middlewares
                                .AddSchemaRegistryJsonSerializer<JsonLogMessage>(
                                    new JsonSerializerConfig
                                    {
                                        SubjectNameStrategy = SubjectNameStrategy.TopicRecord
                                    }))
                )
                .AddProducer(
                    protobufProducerName,
                    producer => producer
                        .DefaultTopic(protobufTopic)
                        .AddMiddlewares(
                            middlewares => middlewares
                                .AddSchemaRegistryProtobufSerializer(
                                    new ProtobufSerializerConfig
                                    {
                                        SubjectNameStrategy = SubjectNameStrategy.TopicRecord
                                    })
                        )
                )
                .AddConsumer(
                    consumer => consumer
                        .Topic(avroTopic)
                        .WithGroupId("avro-group-id")
                        .WithBufferSize(100)
                        .WithWorkersCount(20)
                        .WithAutoOffsetReset(AutoOffsetReset.Latest)
                        .AddMiddlewares(
                            middlewares => middlewares
                                .AddSchemaRegistryAvroDeserializer()
                                .AddTypedHandlers(
                                    handlers => handlers
                                        .AddHandler<AvroMessageHandler>()
                                        .AddHandler<AvroMessageHandler2>())
                        )
                ) 
                .AddConsumer(
                    consumer => consumer
                        .Topic(jsonTopic)
                        .WithGroupId("json-group-id")
                        .WithBufferSize(100)
                        .WithWorkersCount(20)
                        .WithAutoOffsetReset(AutoOffsetReset.Latest)
                        .AddMiddlewares(
                            middlewares => middlewares
                                .AddSchemaRegistryJsonSerializer<JsonLogMessage>()
                                .AddTypedHandlers(handlers => handlers.AddHandler<JsonMessageHandler>())
                        )
                )
                .AddConsumer(
                    consumer => consumer
                        .Topic(protobufTopic)
                        .WithGroupId("protobuf-group-id")
                        .WithBufferSize(100)
                        .WithWorkersCount(20)
                        .WithAutoOffsetReset(AutoOffsetReset.Latest)
                        .AddMiddlewares(
                            middlewares => middlewares
                                .AddSchemaRegistryProtobufDeserializer()
                                .AddTypedHandlers(handlers => handlers.AddHandler<ProtobufMessageHandler>())
                        )
                )
        )
);

var provider = services.BuildServiceProvider();
var bus = provider.CreateKafkaBus();
await bus.StartAsync();

var producers = provider.GetRequiredService<IProducerAccessor>();

while (true)
{
    Console.WriteLine("Number of messages to produce or exit:");
    var input = Console.ReadLine()!.ToLower();

    switch (input)
    {
        case var _ when int.TryParse(input, out var count):
            for (var i = 0; i < count; i++)
            {
                await Task.WhenAll(
                    producers[avroProducerName].ProduceAsync(
                        Guid.NewGuid().ToString(),
                        new AvroLogMessage {Severity = LogLevel.Info}),
                    producers[avroProducerName].ProduceAsync(
                        Guid.NewGuid().ToString(),
                        new AvroLogMessage2 {Message = Guid.NewGuid().ToString()}),
                    producers[jsonProducerName].ProduceAsync(
                        Guid.NewGuid().ToString(),
                        new JsonLogMessage {Message = Guid.NewGuid().ToString()}),
                    producers[protobufProducerName].ProduceAsync(
                        Guid.NewGuid().ToString(),
                        new ProtobufLogMessage {Message = Guid.NewGuid().ToString()})
                );
            }

            break;

        case "exit":
            await bus.StopAsync();
            return;
    }
}