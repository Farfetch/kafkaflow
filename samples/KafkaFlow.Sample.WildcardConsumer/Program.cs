using System.Text;
using Confluent.Kafka;
using KafkaFlow;
using KafkaFlow.Producers;
using Microsoft.Extensions.DependencyInjection;
using AutoOffsetReset = KafkaFlow.AutoOffsetReset;

var services = new ServiceCollection();

const string producerName = "RandomProducer";
const string topicPrefix = "random-topic-";

services.AddKafka(
    kafka => kafka
        .UseConsoleLog()
        .AddCluster(
            cluster => cluster
                .WithBrokers(new[] { "localhost:9092" })
                .AddProducer(
                    producerName, _ => { })
                .AddConsumer(
                    consumer => consumer
                        .Topic($"^{topicPrefix}*") // Any topic starting with `random-topic-*`
                        .WithGroupId("random-topic-handler")
                        .WithBufferSize(5)
                        .WithWorkersCount(3)
                        .WithAutoOffsetReset(AutoOffsetReset.Earliest)
                        .WithConsumerConfig(new ConsumerConfig()
                        {
                            TopicMetadataRefreshIntervalMs = 5000 // discover new topics every 5 seconds
                        })
                        .AddMiddlewares(
                            middlewares => middlewares
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
    Console.WriteLine("Type the name of a topic to send a message or 'exit' to quit:");

    var input = Console.ReadLine();

    if (input is null)
        continue;

    if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        await bus.StopAsync();
        break;
    }

    await producer.ProduceAsync(
        $"{topicPrefix}{input}",
        Guid.NewGuid().ToString(),
        Encoding.UTF8.GetBytes(
            $"Message to {input}: {Guid.NewGuid()}"));
}
