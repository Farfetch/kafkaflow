namespace KafkaFlow.Sample.SchemaRegistryConsumer
{
    using System.Threading.Tasks;
    using Confluent.SchemaRegistry;
    using global::Microsoft.Extensions.DependencyInjection;
    using KafkaFlow.Producers;
    using KafkaFlow.Serializer;
    using KafkaFlow.TypedHandler;

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
                            .AddConsumer(
                                c => c
                                    .Topic("test-topic-schema-registry")
                                    .WithGroupId("test-group")
                                    .WithWorkersCount(5)
                                    .WithBufferSize(100)
                                    .AddMiddlewares(
                                        middlewares => middlewares
                                            .DeserializeWithSchemaRegistry<ApacheAvroMessageSerializer>()
                                            .AddTypedHandlers(h => h.AddHandler<TestMessageV2Handler>())
                                    )
                            )
                    )
            );

            var provider = services.BuildServiceProvider();

            var bus = provider.CreateKafkaBus();

            await bus.StartAsync();

            await Task.Delay(int.MaxValue);
        }
    }
}
