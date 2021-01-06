namespace KafkaFlow.Sample
{
    using System;
    using System.Threading.Tasks;
    using Confluent.SchemaRegistry;
    using Confluent.SchemaRegistry.Serdes;
    using global::Microsoft.Extensions.DependencyInjection;
    using KafkaFlow.Admin;
    using KafkaFlow.Admin.Messages;
    using KafkaFlow.Compressor;
    using KafkaFlow.Compressor.Gzip;
    using KafkaFlow.Consumers;
    using KafkaFlow.Producers;
    using KafkaFlow.Serializer;
    using KafkaFlow.TypedHandler;
    using MessageTypes;
    using Serializer.ApacheAvro;
    using Serializer.Json;

    internal static class Program
    {
        const string ProducerName = "ProducerName";
        const string ProducerAvroName = "ProducerAvroName";
        
        const string ConsumerName = "ConsumerName";
        const string ConsumerAvroName = "ConsumerAvroName";

        const string AdminTopicName = "kafka-flow.admin";
        const string TopicName = "topic-kafka-flow";
        const string TopicAvroName = "topic-avro-kafka-flow";
        
        const string BrokerUrl = "localhost:9092";
        //const string SchemaRegistryUrl = "https://fast-data-dev.demo.landoop.com/api/schema-registry";
        const string SchemaRegistryUrl = "localhost:8081";
        
        const string ConsumerGroupId = "consumer-kafka-flow";
        
        const int BufferSize = 100;
        const int WorkersCount = 2;
        
        private static async Task Main()
        {
            var services = new ServiceCollection();

            services.AddKafka(
                kafka => kafka
                    .UseConsoleLog()
                    .AddCluster(
                        cluster => cluster
                            .WithBrokers(new[] {BrokerUrl})
                            .WithSchemaRegistry(config =>
                            {
                                config.Url = SchemaRegistryUrl;
                            })
                            .EnableAdminMessages(AdminTopicName)
                            .AddProducer(
                                ProducerName,
                                producer => producer
                                    .DefaultTopic(TopicName)
                                    .AddMiddlewares(
                                        middlewares => middlewares
                                            .AddSerializer<JsonMessageSerializer>()
                                            .AddCompressor<GzipMessageCompressor>()
                                    )
                                    .WithAcks(Acks.All)
                            )
                            .AddProducer(
                                ProducerAvroName,
                                producer => producer
                                    .DefaultTopic(TopicAvroName)
                                    .AddMiddlewares(middlewares =>
                                        middlewares
                                            .AddSerializer(s =>
                                                new ApacheAvroMessageSerializer(new AvroSerializerConfig
                                                {
                                                    AutoRegisterSchemas = true,
                                                    SubjectNameStrategy = SubjectNameStrategy.Record
                                                })))
                                    .WithAcks(Acks.Leader)
                            )
                            .AddConsumer(
                                consumer => consumer
                                    .Topic(TopicAvroName)
                                    .WithGroupId(ConsumerGroupId)
                                    .WithName(ConsumerAvroName)
                                    .WithBufferSize(BufferSize)
                                    .WithWorkersCount(WorkersCount)
                                    .WithAutoOffsetReset(AutoOffsetReset.Latest)
                                    .AddMiddlewares(
                                        middlewares => middlewares
                                            .AddSerializer<ApacheAvroMessageSerializer>()
                                            .AddTypedHandlers(
                                                handlers => handlers
                                                    .WithHandlerLifetime(InstanceLifetime.Singleton)
                                                    .AddHandler<AvroMessageHandler1>()
                                                    .AddHandler<AvroMessageHandler2>())
                                    )
                            )
                            .AddConsumer(
                                consumer => consumer
                                    .Topic(TopicName)
                                    .WithGroupId(ConsumerGroupId)
                                    .WithName(ConsumerName)
                                    .WithBufferSize(BufferSize)
                                    .WithWorkersCount(WorkersCount)
                                    .WithAutoOffsetReset(AutoOffsetReset.Latest)
                                    .AddMiddlewares(
                                        middlewares => middlewares
                                            .AddCompressor<GzipMessageCompressor>()
                                            .AddSerializer<JsonMessageSerializer>()
                                            .AddTypedHandlers(
                                                handlers => handlers
                                                    .WithHandlerLifetime(InstanceLifetime.Singleton)
                                                    .AddHandler<JsonMessageHandler>()
                                            )
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
                            await producers[ProducerAvroName]
                                .ProduceAsync(
                                    Guid.NewGuid().ToString(),
                                    new LogMessages1()
                                    {
                                        Severity = LogLevel.Error
                                    });
                                        
                            await producers[ProducerAvroName]
                                .ProduceAsync(
                                    Guid.NewGuid().ToString(),
                                    new LogMessages2()
                                    {
                                        Message = "Message 2 - " + i.ToString(),
                                    });
                                
                            await producers[ProducerName]
                                .ProduceAsync(
                                    Guid.NewGuid().ToString(),
                                    new TestMessage()
                                    {
                                        Text = "Json message"
                                    });
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
                        await adminProducer.ProduceAsync(new ResetConsumerOffset { ConsumerName = ConsumerName });

                        break;

                    case "rewind":
                        Console.Write("Input a time: ");
                        var timeInput = Console.ReadLine();

                        if (DateTime.TryParse(timeInput, out var time))
                        {
                            await adminProducer.ProduceAsync(
                                new RewindConsumerOffsetToDateTime
                                {
                                    ConsumerName = ConsumerName,
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
                                    ConsumerName = ConsumerName,
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
