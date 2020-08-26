namespace KafkaFlow.Sample
{
    using System;
    using System.Threading.Tasks;
    using global::Microsoft.Extensions.DependencyInjection;
    using KafkaFlow.Admin;
    using KafkaFlow.Admin.Messages;
    using KafkaFlow.Compressor;
    using KafkaFlow.Compressor.Gzip;
    using KafkaFlow.Consumers;
    using KafkaFlow.Producers;
    using KafkaFlow.Serializer;
    using KafkaFlow.Serializer.ProtoBuf;
    using KafkaFlow.TypedHandler;

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
                            .EnableAdminMessages("kafka-flow.admin", Guid.NewGuid().ToString())
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
                                    .WithName(consumerName)
                                    .WithBufferSize(100)
                                    .WithWorkersCount(20)
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

            await bus.StartAsync();

            var consumers = provider.GetRequiredService<IConsumerAccessor>();
            var producers = provider.GetRequiredService<IProducerAccessor>();

            var adminProducer = provider.GetService<IAdminProducer>();

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

                    case "reset":
                        await adminProducer.ProduceAsync(new ResetConsumerOffset { ConsumerName = consumerName });

                        break;

                    case "rewind":
                        Console.Write("Input a time: ");
                        var timeInput = Console.ReadLine();

                        if (DateTime.TryParse(timeInput, out var time))
                        {
                            adminProducer.ProduceAsync(
                                new RewindConsumerOffsetToDateTime
                                {
                                    ConsumerName = consumerName,
                                    DateTime = time
                                });
                        }

                        break;

                    case "workers":
                        Console.Write("Input a new worker count: ");
                        var workersInput = Console.ReadLine();

                        if (int.TryParse(workersInput, out var workers))
                        {
                            await adminProducer.ProduceAsync(
                                new ChangeConsumerWorkerCount
                                {
                                    ConsumerName = consumerName,
                                    WorkerCount = workers
                                });
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
