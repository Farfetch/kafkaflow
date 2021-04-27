namespace KafkaFlow.Sample.BatchConsume
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using global::Microsoft.Extensions.DependencyInjection;
    using KafkaFlow.BatchConsume;
    using KafkaFlow.Producers;
    using KafkaFlow.Serializer;

    internal static class Program
    {
        private static async Task Main()
        {
            var services = new ServiceCollection();

            const string producerName = "PrintConsole";

            const string batchTestTopic = "batch-test-topic";

            services.AddKafka(
                kafka => kafka
                    .UseConsoleLog()
                    .AddCluster(
                        cluster => cluster
                            .WithBrokers(new[] { "localhost:9092" })
                            .AddProducer(
                                producerName,
                                producerBuilder => producerBuilder
                                    .AddMiddlewares(
                                        middlewares => middlewares
                                            .AddSerializer<JsonCoreSerializer>()
                                    )
                            )
                            .AddConsumer(
                                consumerBuilder => consumerBuilder
                                    .Topic(batchTestTopic)
                                    .WithGroupId("kafka-flow-sample")
                                    .WithBufferSize(10000)
                                    .WithWorkersCount(1)
                                    .AddMiddlewares(
                                        middlewares => middlewares
                                            .AddSerializer<JsonCoreSerializer>()
                                            .BatchConsume(10, TimeSpan.FromSeconds(10))
                                            .Add<PrintConsoleMiddleware>()
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
                Console.Write("Number of messages to produce: ");
                var input = Console.ReadLine().ToLower();

                switch (input)
                {
                    case var _ when int.TryParse(input, out var count):
                        await producer
                            .BatchProduceAsync(
                                Enumerable
                                    .Range(0, count)
                                    .Select(
                                        x => new BatchProduceItem(
                                            batchTestTopic,
                                            Guid.NewGuid().ToString(),
                                            new SampleBatchMessage { Text = Guid.NewGuid().ToString() },
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

    internal class SampleBatchMessage
    {
        public string Text { get; set; }
    }

    internal class PrintConsoleMiddleware : IMessageMiddleware
    {
        public Task Invoke(IMessageContext context, MiddlewareDelegate next)
        {
            var batch = context.GetMessagesBatch();

            var text = string.Join(
                '\n',
                batch.Select(ctx => ((SampleBatchMessage) ctx.Message.Value).Text));

            Console.WriteLine(text);

            return Task.CompletedTask;
        }
    }
}
