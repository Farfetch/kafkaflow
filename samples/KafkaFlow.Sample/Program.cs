using System;
using System.Diagnostics;
using System.Threading.Tasks;
using KafkaFlow;
using KafkaFlow.Configuration;
using KafkaFlow.OpenTelemetry.Trace;
using KafkaFlow.Producers;
using KafkaFlow.Sample;
using KafkaFlow.Serializer;
using KafkaFlow.TypedHandler;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Trace;

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
                        .WithBufferSize(100)
                        .WithWorkersCount(3)
                        .AddMiddlewares(
                            middlewares => middlewares
                                .AddSerializer<ProtobufNetSerializer>()
                                .AddTypedHandlers(h => h.AddHandler<PrintConsoleHandler>())
                        )
                )
        )
        .AddOpenTelemetryInstrumentation()
);

var provider = services.BuildServiceProvider();

var bus = provider.CreateKafkaBus();

await bus.StartAsync();

var producer = provider
    .GetRequiredService<IProducerAccessor>()
    .GetProducer(producerName);

Console.WriteLine("Type the number of messages to produce or 'exit' to quit:");

using var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .AddSource("KafkaFlow")
    .AddConsoleExporter()
    .Build();

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

    if (input!.Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        await bus.StopAsync();
        break;
    }
}

await Task.Delay(3000);
