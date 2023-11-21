using System;
using System.Threading.Tasks;
using KafkaFlow.Configuration;
using KafkaFlow.OpenTelemetry;
using KafkaFlow.Producers;
using KafkaFlow.Serializer;
using KafkaFlow.TypedHandler;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace KafkaFlow.Sample.OpenTelemetry;

public class Program
{
    public static async Task Main()
    {
        var services = new ServiceCollection();

        const string producerName = "PrintConsole";
        const string topicName = "otel-sample-topic";

        using var tracerProvider = Sdk.CreateTracerProviderBuilder()
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(
                serviceName: "DemoApp",
                serviceVersion: "1.0.0"))
            .AddSource(KafkaFlowInstrumentation.ActivitySourceName)
            .AddConsoleExporter()
            .AddOtlpExporter()
            .Build();

        services.AddKafka(
            kafka => kafka
                .UseConsoleLog()
                .AddOpenTelemetryInstrumentation()
                .AddCluster(
                    cluster => cluster
                        .WithBrokers(new[]
                        {
                            "localhost:9092"
                        })
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
        );

        var provider = services.BuildServiceProvider();

        var bus = provider.CreateKafkaBus();

        await bus.StartAsync();

        var producer = provider
            .GetRequiredService<IProducerAccessor>()
            .GetProducer(producerName);

        while (true)
        {
            Console.WriteLine("Type the message you want to send or 'exit' to quit:");
            var input = Console.ReadLine();


            if (input!.Equals("exit", StringComparison.OrdinalIgnoreCase))
            {
                await bus.StopAsync();
                break;
            }

            await producer.ProduceAsync(
                topicName,
                Guid.NewGuid().ToString(),
                new TestMessage { Text = $"Message: {input}" });
        }

        await Task.Delay(3000);
    }
}