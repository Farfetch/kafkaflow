namespace KafkaFlow.Sample.ProducerThrottling
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Producers;
    using Serializer;

    internal static class Program
    {
        private static async Task Main()
        {
            var services = new ServiceCollection();

            const string producerName = "producer";
            const string consumerName = "consumer";
            const string consumerGroup = "throttling-group";
            const string topicName = "test-topic";

            services.AddKafka(
                kafka => kafka
                    .UseConsoleLog()
                    .AddCluster(
                        cluster => cluster
                            .WithBrokers(new[] { "localhost:9092" })
                            .AddProducer(
                                producerName,
                                producer => producer
                                    .DefaultTopic(topicName)
                                    .AddMiddlewares(
                                        middlewares =>
                                            middlewares
                                                .AddSerializer<ProtobufNetSerializer>()
                                                .AddThrottling(
                                                    throttling =>
                                                    {
                                                        throttling
                                                            .AddLagEvaluation(TimeSpan.FromSeconds(5), topicName, consumerGroup)
                                                            .AddDelayAction(1000, TimeSpan.FromMilliseconds(500))
                                                            .AddDelayAction(10000, TimeSpan.FromSeconds(3));
                                                    })
                                    )
                            )
                            .AddConsumer(
                                consumer => consumer
                                    .Topic(topicName)
                                    .WithGroupId(consumerGroup)
                                    .WithName(consumerName)
                                    .WithBufferSize(10)
                                    .WithWorkersCount(1)
                                    .AddMiddlewares(
                                        middlewares =>
                                            middlewares
                                                .AddSerializer<ProtobufNetSerializer>()
                                                .Add<PrintConsoleMiddleware>())
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
                Console.Write("Number of messages to produce or Exit:");
                var input = Console.ReadLine()?.ToLower();

                switch (input)
                {
                    case var _ when int.TryParse(input, out var count):
                        for (var i = 0; i < count; i++)
                        {
                            await producer.ProduceAsync(
                                Guid.NewGuid().ToString(),
                                new TestMessage { Text = $"Message: {Guid.NewGuid()}" });

                            Console.WriteLine("Message Produced: {0}", i);
                        }

                        break;

                    case "exit":
                        await bus.StopAsync();
                        return;
                }
            }
        }
    }

    internal class PrintConsoleMiddleware : IMessageMiddleware
    {
        public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
        {
            await Task.Delay(100);
        }
    }
}
