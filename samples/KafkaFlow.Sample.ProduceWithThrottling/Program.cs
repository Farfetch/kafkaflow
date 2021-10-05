namespace KafkaFlow.Sample.ProduceWithThrottling
{
    using System;
    using System.Threading.Tasks;
    using Client.Protocol;
    using Confluent.Kafka;
    using Extensions.ProducerThrottling.Actions;
    using Extensions.ProducerThrottling.Evaluations;
    using Extensions.ProducerThrottling.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using Producers;
    using Serializer;

    internal static class Program
    {
        private static async Task Main()
        {
            var services = new ServiceCollection();

            const string producerName = "producer-throttling";
            const string consumerName = "consumer-example";
            const string consumerGroup = "print-console-handler";
            const string topicName = "test-topic";

            services.AddKafka(
                kafka => kafka
                    .UseConsoleLog()
                    .AddCluster(
                        cluster => cluster
                            .WithBrokers(new[] { "localhost:9092" })
                            .WithLagReader(new[] { new BrokerAddress("localhost", 9092) })
                            .AddProducer(
                                producerName,
                                producer => producer
                                    .DefaultTopic(topicName)
                                    .WithCompression(CompressionType.Gzip)
                                    .AddMiddlewares(middlewares =>
                                        middlewares
                                            .AddSerializer<ProtobufNetSerializer>()
                                            .WithThrottling(throttling =>
                                            {
                                                throttling
                                                    .AddEvaluation(resolver => new LagEvaluation(resolver, topicName, consumerGroup))
                                                    .AddAction(10, resolver => new DelayAction(resolver, TimeSpan.FromMilliseconds(100)))
                                                    .AddAction(11, resolver => new DelayAction(resolver, TimeSpan.FromMilliseconds(1000)))
                                                    .AddAction(12, resolver => new PauseConsumerAction(resolver, consumerName));
                                            })
                                    )
                            )
                            .AddConsumer(
                                consumer => consumer
                                    .Topic("json-topic")
                                    .WithGroupId("consumerGroup")
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
                        for (var i = 0; i < count; i++)
                        {
                            await producers[producerName].ProduceAsync(
                                Guid.NewGuid().ToString(),
                                new TestMessage { Text = $"Message: {Guid.NewGuid()}"});

                            Console.WriteLine($"Message published number {i}");
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
