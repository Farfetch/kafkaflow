using System;
using System.Linq;
using KafkaFlow;
using KafkaFlow.Producers;
using KafkaFlow.Sample.BatchOperations;
using KafkaFlow.Serializer;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

const string producerName = "PrintConsole";
const string batchTestTopic = "batch-test-topic";

services.AddKafka(
    kafka => kafka
        .UseConsoleLog()
        .AddCluster(
            cluster => cluster
                .WithBrokers(new[] { "localhost:9092" })
                .CreateTopicIfNotExists(batchTestTopic, 1, 1)
                .AddProducer(
                    producerName,
                    producerBuilder => producerBuilder
                        .AddMiddlewares(
                            middlewares => middlewares
                                .AddSerializer<JsonCoreSerializer>()
                        )
                )
                .AddConsumer(
                    consumerBuilder => consumerBuilder
                        .Topic(batchTestTopic)
                        .WithGroupId("kafkaflow-sample")
                        .WithBufferSize(10000)
                        .WithWorkersCount(1)
                        .AddMiddlewares(
                            middlewares => middlewares
                                .AddDeserializer<JsonCoreDeserializer>()
                                .AddBatching(10, TimeSpan.FromSeconds(10))
                                .Add<PrintConsoleMiddleware>()
                        )
                )
        )
);

var provider = services.BuildServiceProvider();

var bus = provider.CreateKafkaBus();

await bus.StartAsync();

var producer = provider
    .GetRequiredService<IProducerAccessor>()
    .GetProducer(producerName);

while (true)
{
    Console.Write("Number of messages to produce: ");
    var input = Console.ReadLine()!.ToLower();

    switch (input)
    {
        case var _ when int.TryParse(input, out var count):
            await producer
                .BatchProduceAsync(
                    Enumerable
                        .Range(0, count)
                        .Select(
                            _ => new BatchProduceItem(
                                batchTestTopic,
                                Guid.NewGuid().ToString(),
                                new SampleBatchMessage { Text = Guid.NewGuid().ToString() },
                                null))
                        .ToList());

            break;

        case "exit":
            await bus.StopAsync();
            return;
    }
}