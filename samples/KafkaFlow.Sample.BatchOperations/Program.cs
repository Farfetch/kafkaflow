using System;
using System.Linq;
using KafkaFlow;
using KafkaFlow.Admin;
using KafkaFlow.BatchConsume;
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
                .CreateTopicIfNotExists("kafka-flow.admin", 1, 1)
                .EnableAdminMessages("kafka-flow.admin")
                .EnableTelemetry("kafka-flow.admin")
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
                        .WithGroupId("kafka-flow-sample")
                        .WithName("my-consumer")
                        .WithBufferSize(100)
                        .WithWorkersCount(1)
                        .WithManualStoreOffsets()
                        .AddMiddlewares(
                            middlewares => middlewares
                                .AddSerializer<JsonCoreSerializer>()
                                .BatchConsume(10, TimeSpan.FromSeconds(10))
                                .Add<PrintConsoleMiddleware>(MiddlewareLifetime.Worker)
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

var consumerAdmin = provider.GetRequiredService<IConsumerAdmin>();

while (true)
{
    Console.Write("Number of messages to produce: ");
    var input = Console.ReadLine()!.ToLower();

    if (int.TryParse(input, out var count))
    {
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
    }

    if (input!.Equals("reset", StringComparison.OrdinalIgnoreCase))
    {
        await consumerAdmin.ResetOffsetsAsync("my-consumer", new[] { batchTestTopic });
    }

    if (input!.Equals("rewind", StringComparison.OrdinalIgnoreCase))
    {
        await consumerAdmin.RewindOffsetsAsync("my-consumer", DateTime.UtcNow.AddMinutes(-5), new[] { batchTestTopic });
    }

    if (input!.Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        await bus.StopAsync();
        break;
    }
}
