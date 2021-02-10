namespace KafkaFlow.Avro.Sample
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Confluent.SchemaRegistry;
    using Confluent.SchemaRegistry.Serdes;
    using global::Microsoft.Extensions.DependencyInjection;
    using KafkaFlow.Admin;
    using KafkaFlow.Admin.Messages;
    using KafkaFlow.Compressor;
    using KafkaFlow.Compressor.Gzip;
    using KafkaFlow.Consumers;
    using KafkaFlow.Producers;
    using KafkaFlow.Sample;
    using KafkaFlow.TypedHandler;
    using MessageTypes;
    using Serializer.ApacheAvro;
    using AutoOffsetReset = KafkaFlow.AutoOffsetReset;

    internal static class Program
    {
        private static async Task Main()
        {
            var services = new ServiceCollection();

            const string producerName = "avro-producer";
            const string consumerName = "avro-consumer";
            const string topicName = "avro-topic";
            const string groupId = "avro-group-id";

            services.AddKafka(
                kafka => kafka
                    .UseConsoleLog()
                    .AddCluster(
                        cluster => cluster
                            .WithBrokers(new[] { "localhost:9092" })
                            .WithSchemaRegistry(config => config.Url = "localhost:8081")
                            .AddProducer(
                                producerName,
                                producer => producer
                                    .DefaultTopic(topicName)
                                    .AddMiddlewares(
                                        middlewares => middlewares
                                            .AddApacheAvroSerializer(
                                                new AvroSerializerConfig
                                                {
                                                    AutoRegisterSchemas = true,
                                                    SubjectNameStrategy = SubjectNameStrategy.Record
                                                })
                                    )
                            )
                            .AddConsumer(
                                consumer => consumer
                                    .Topic(topicName)
                                    .WithGroupId(groupId)
                                    .WithName(consumerName)
                                    .WithBufferSize(100)
                                    .WithWorkersCount(20)
                                    .WithAutoOffsetReset(AutoOffsetReset.Latest)
                                    .AddMiddlewares(
                                        middlewares => middlewares
                                            .AddApacheAvroSerializer()
                                            .AddTypedHandlers(
                                                handlers => handlers
                                                    .WithHandlerLifetime(InstanceLifetime.Singleton)
                                                    .AddHandler<AvroMessageHandler1>()
                                                    .AddHandler<AvroMessageHandler2>())
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
                Console.WriteLine("Number of messages to produce or exit:");
                var input = Console.ReadLine().ToLower();

                switch (input)
                {
                    case var _ when int.TryParse(input, out var count):
                        for (var i = 0; i < count; i++)
                        {
                            await Task.WhenAll(
                                producers[producerName].ProduceAsync(
                                        Guid.NewGuid().ToString(),
                                        new LogMessages1{Severity = LogLevel.Info}),
                                producers[producerName].ProduceAsync(
                                    Guid.NewGuid().ToString(),
                                    new LogMessages2{Message = Guid.NewGuid().ToString()}));
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