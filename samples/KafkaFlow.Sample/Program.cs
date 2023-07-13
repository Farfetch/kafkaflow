using System;
using System.Threading.Tasks;
using KafkaFlow;
using KafkaFlow.Admin;
using KafkaFlow.Consumers;
using KafkaFlow.Producers;
using KafkaFlow.Sample;
using KafkaFlow.Serializer;
using KafkaFlow.TypedHandler;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

const string producerName = "PrintConsole";
const string topicName = "sample-topic";

services.AddKafka(
    kafka => kafka
        .UseConsoleLog()
        .AddCluster(
            cluster => cluster
                .WithBrokers(new[] { "localhost:9092" })
                .CreateTopicIfNotExists(topicName, 6, 1)
                .CreateTopicIfNotExists("kafka-flow.admin", 1, 1)
                .EnableAdminMessages("kafka-flow.admin")
                .EnableTelemetry("kafka-flow.admin")
                .AddProducer(
                    producerName,
                    producer => producer
                        .DefaultTopic(topicName)
                        .AddMiddlewares(m => m.AddSerializer<ProtobufNetSerializer>())
                )
                .AddConsumer(
                    consumer => consumer
                        .Topic(topicName)
                        .WithGroupId("print-console-handler")
                        .WithName("my-consumer")
                        .WithBufferSize(10)
                        .WithWorkersCount(1)
                        .WithManualStoreOffsets()
                        .AddMiddlewares(
                            middlewares => middlewares
                                .AddSerializer<ProtobufNetSerializer>()
                                .Add<PrintMiddleware>(MiddlewareLifetime.Worker)
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

Console.WriteLine("Type the number of messages to produce or 'exit' to quit:");

while (true)
{
    var input = Console.ReadLine();

    if (int.TryParse(input, out var count))
    {
        for (var i = 0; i < count; i++)
        {
            await producer.ProduceAsync(
                topicName,
                Guid.NewGuid().ToString(),
                new TestMessage { Text = $"Message: {Guid.NewGuid()}" });
        }
    }

    if (input!.Equals("reset", StringComparison.OrdinalIgnoreCase))
    {
        await consumerAdmin.ResetOffsetsAsync("my-consumer", new[] { topicName });
    }

    if (input!.Equals("rewind", StringComparison.OrdinalIgnoreCase))
    {
        await consumerAdmin.RewindOffsetsAsync("my-consumer", DateTime.UtcNow.AddMinutes(-5), new[] { topicName });
    }

    if (input!.Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        await bus.StopAsync();
        break;
    }
}

await Task.Delay(3000);


public class PrintMiddleware : IMessageMiddleware
{
    private readonly IWorker worker;

    public PrintMiddleware(IWorkerLifetimeContext workerLifetimeContext)
    {
        this.worker = workerLifetimeContext.Worker;
    }

    public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
    {
        await Task.Delay(1000);

        Task.Delay(5000).ContinueWith(_ => context.ConsumerContext.StoreOffset());

        Console.WriteLine(
            "Partition: {0} | Offset: {1} | Worker: {2}",
            context.ConsumerContext.Partition,
            context.ConsumerContext.Offset,
            this.worker.Id);
    }
}
