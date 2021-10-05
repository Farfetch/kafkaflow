using System;

namespace KafkaFlow.Sample.Throttling
{
    using System.Threading.Tasks;
    using Client.Protocol;
    using Confluent.Kafka;
    using Extensions.ProducerThrottling;
    using Microsoft.Extensions.DependencyInjection;
    using Producers;
    using Serializer;

    internal static class Program
    {
        private static async Task Main()
        {
            var services = new ServiceCollection();

            const string producerName = "PrintConsole";

            const string consumerName = "test";

            services.AddKafka(
                kafka => kafka
                    .UseConsoleLog()
                    .AddCluster(
                        cluster => cluster
                            .WithBrokers(new[] { "localhost:9092" })
                            .WithLagReader(new [] { new BrokerAddress("localhost", 9092)})
                            .AddProducer(
                                producerName,
                                producer => producer
                                    .DefaultTopic("test-topic")
                                    .PausingConsumerWhenLagExceedsThreshold(
                                        "test-topic",
                                        "print-console-handler",
                                        10,
                                        consumerName)
                                    .WithCompression(CompressionType.Gzip)
                                    .AddMiddlewares(middlewares => middlewares.AddSerializer<ProtobufNetSerializer>())
                            )
                            .AddConsumer(
                                consumer => consumer
                                    .Topic("test-topic")
                                    .WithGroupId("print-console-handler")
                                    .WithName(consumerName)
                                    .WithBufferSize(100)
                                    .WithWorkersCount(20)
                                    .AddMiddlewares(middlewares => middlewares.AddSerializer<ProtobufNetSerializer>())
                            )
                    )
            );

            var provider = services.BuildServiceProvider();

            var bus = provider.CreateKafkaBus();

            await bus.StartAsync();

            var producers = provider.GetRequiredService<IProducerAccessor>();

            while (true)
            {
                Console.Write("Number of messages to produce or Exit:");
                var input = Console.ReadLine()?.ToLower();

                switch (input)
                {
                    case var _ when int.TryParse(input, out var count):
                        for (int i = 0; i < count; i++)
                        {
                            await producers[producerName].ProduceAsync(
                                "test-topic",
                                Guid.NewGuid().ToString(),
                                new TestMessage { Text = $"Message: {Guid.NewGuid()}" });

                            Console.WriteLine($"Publicada mensagem {i}");
                        }

                        break;

                    case "exit":
                        await bus.StopAsync();
                        return;
                }
            }
        }
    }
}
