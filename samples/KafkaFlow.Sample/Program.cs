namespace KafkaFlow.Sample
{
    using System;
    using global::Microsoft.Extensions.DependencyInjection;
    using KafkaFlow.Compressor;
    using KafkaFlow.Compressor.Gzip;
    using KafkaFlow.Consumers;
    using KafkaFlow.Producers;
    using KafkaFlow.Serializer;
    using KafkaFlow.Serializer.ProtoBuf;
    using KafkaFlow.TypedHandler;

    internal static class Program
    {
        private static void Main()
        {
            var services = new ServiceCollection();

            const string producerName = "PrintConsole";

            services.AddKafka(
                kafka => kafka
                    .UseConsoleLog()
                    .AddCluster(
                        cluster => cluster
                            .WithBrokers(new[] { "localhost:9092" })
                            .AddProducer(
                                producerName,
                                producer => producer
                                    .DefaultTopic("test-topic")
                                    .AddMiddlewares(
                                        middlewares => middlewares
                                            .AddSerializer<ProtobufMessageSerializer>()
                                            .AddCompressor<GzipMessageCompressor>()
                                    )
                                    .WithAcks(Acks.All)
                            )
                            .AddConsumer(
                                consumer => consumer
                                    .Topic("test-topic")
                                    .WithGroupId("print-console-handler")
                                    .WithBufferSize(100)
                                    .WithWorkersCount(10)
                                    .WithAutoOffsetReset(AutoOffsetReset.Latest)
                                    .AddMiddlewares(
                                        middlewares => middlewares
                                            .AddCompressor<GzipMessageCompressor>()
                                            .AddSerializer<ProtobufMessageSerializer>()
                                            .AddTypedHandlers(
                                                handlers => handlers
                                                    .WithHandlerLifetime(InstanceLifetime.Singleton)
                                                    .AddHandler<PrintConsoleHandler>())
                                    )
                            )
                    )
            );

            var provider = services.BuildServiceProvider();

            var bus = provider.CreateKafkaBus();

            bus.StartAsync().GetAwaiter().GetResult();
            var consumers = provider.GetRequiredService<IConsumerAccessor>();
            var producers = provider.GetRequiredService<IProducerAccessor>();

            while (true)
            {
                Console.Write("Number of messages to produce, Pause, Resume, or Exit:");
                var input = Console.ReadLine().ToLower();

                switch (input)
                {
                    case var _ when int.TryParse(input, out var count):
                        for (var i = 0; i < count; i++)
                        {
                            producers[producerName]
                                .Produce(
                                    Guid.NewGuid().ToString(),
                                    new TestMessage { Text = $"Message: {Guid.NewGuid()}" });
                        }

                        break;

                    case "pause":
                        foreach (var consumer in consumers.All)
                        {
                            consumer.Pause(consumer.Assignment);
                        }

                        Console.WriteLine("Consumer paused");

                        break;

                    case "resume":
                        foreach (var consumer in consumers.All)
                        {
                            consumer.Resume(consumer.Assignment);
                        }

                        Console.WriteLine("Consumer resumed");

                        break;

                    case "exit":
                        return;
                }
            }
        }
    }
}
