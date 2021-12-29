namespace KafkaFlow.Sample
{
    using System;
    using System.Linq;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;
    using KafkaFlow.Admin;
    using Confluent.Kafka;
    using global::Microsoft.Extensions.DependencyInjection;
    using KafkaFlow.Admin.Messages;
    using KafkaFlow.Consumers;
    using KafkaFlow.Producers;
    using KafkaFlow.Serializer;
    using KafkaFlow.TypedHandler;
    using Acks = KafkaFlow.Acks;
    using AutoOffsetReset = KafkaFlow.AutoOffsetReset;
    using SaslMechanism = KafkaFlow.Configuration.SaslMechanism;
    using SecurityProtocol = KafkaFlow.Configuration.SecurityProtocol;

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
                            .WithBrokers(new[] { "localhost:19094" })
                            .WithSecurityInformation(
                                information =>
                                {
                                    information.SaslMechanism = SaslMechanism.ScramSha512;
                                    information.SecurityProtocol = SecurityProtocol.SaslSsl;
                                    information.SaslUsername = "user";
                                    information.SaslPassword = "password";
                                    information.SslCaLocation = "";
                                })
                            .AddProducer(
                                producerName,
                                producer => producer
                                    .DefaultTopic("test-topic")
                                    .WithCompression(CompressionType.Gzip)
                                    .AddMiddlewares(
                                        middlewares => middlewares
                                            .AddSerializer<ProtobufNetSerializer>()
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
                                    .WithPendingOffsetsStatisticsHandler(
                                        (resolver, offsets) =>
                                            resolver.Resolve<ILogHandler>()
                                                .Info(
                                                    "Offsets pending to be committed",
                                                    new
                                                    {
                                                        Offsets = offsets.Select(
                                                            o =>
                                                                new
                                                                {
                                                                    Partition = o.Partition.Value,
                                                                    Offset = o.Offset.Value,
                                                                    o.Topic
                                                                })
                                                    }),
                                        new TimeSpan(0, 0, 1))
                                    .AddMiddlewares(
                                        middlewares => middlewares
                                            .AddSerializer<ProtobufNetSerializer>()
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

            var producers = provider.GetRequiredService<IProducerAccessor>();

            while (true)
            {
                Console.Write("Number of messages to produce, Pause, Resume, or Exit:");
                var input = Console.ReadLine()?.ToLower();

                switch (input)
                {
                    case var _ when int.TryParse(input, out var count):
                        await producers[producerName]
                            .BatchProduceAsync(
                                Enumerable
                                    .Range(0, count)
                                    .Select(
                                        x => new BatchProduceItem(
                                            "test-topic",
                                            Guid.NewGuid().ToString(),
                                            new TestMessage { Text = $"Message: {Guid.NewGuid()}" },
                                            null))
                                    .ToList());

                        break;

                    case "exit":
                        await bus.StopAsync();
                        return;
                }
            }
        }
    }
}
