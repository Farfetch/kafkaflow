namespace KafkaFlow.Sample.SchemaRegistryProducer
{
    using System;
    using System.Threading.Tasks;
    using Confluent.SchemaRegistry;
    using global::Microsoft.Extensions.DependencyInjection;
    using KafkaFlow.Producers;
    using KafkaFlow.Serializer;

    internal static class Program
    {
        private static async Task Main()
        {
            var services = new ServiceCollection();

            const string producerName = "PrintConsole";

            services.AddKafka(
                kafka => kafka
                    .UseConsoleLog()
                    .WithSchemaRegistry(new SchemaRegistryConfig { Url = "http://localhost:8081/" })
                    .AddCluster(
                        cluster => cluster
                            .WithBrokers(new[] { "localhost:9092" })
                            .AddProducer(
                                producerName,
                                p => p
                                    .DefaultTopic("test-topic-schema-registry")
                                    .AddMiddlewares(
                                        middlewares => middlewares
                                            .SerializeWithSchemaRegistry<ApacheAvroMessageSerializer>()
                                    )
                                    .WithAcks(Acks.All)
                            )
                    )
            );

            var provider = services.BuildServiceProvider();

            var bus = provider.CreateKafkaBus();

            await bus.StartAsync();

            var producer = provider
                .GetRequiredService<IProducerAccessor>()[producerName];

            while (true)
            {
                Console.Write("Number of messages to produce or Exit:");
                var input = Console.ReadLine().ToLower();

                switch (input)
                {
                    case var _ when int.TryParse(input, out var count):
                        for (var i = 0; i < count; i++)
                        {
                            await producer.ProduceAsync(
                                Guid.NewGuid().ToString(),
                                new TestMessageV1 { Text = $"Message: {Guid.NewGuid()}" });
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
