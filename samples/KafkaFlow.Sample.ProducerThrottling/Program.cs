namespace KafkaFlow.Sample.ProducerThrottling
{
    using System;
    using System.Threading.Tasks;
    using KafkaFlow.Configuration;
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
                            .WithBrokers(new[] { "localhost:19092" })
                            // .WithSecurityInformation(
                            //     information =>
                            //     {
                            //         information.SecurityProtocol = SecurityProtocol.SaslSsl;
                            //         information.SaslMechanism = SaslMechanism.ScramSha512;
                            //         information.SaslUsername = "user";
                            //         information.SaslPassword = "password";
                            //     })
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
                                                            .AddLagEvaluation(TimeSpan.FromSeconds(10), topicName, consumerGroup)
                                                            .AddDelayAction(100, TimeSpan.FromMilliseconds(100))
                                                            .AddDelayAction(10000, TimeSpan.FromSeconds(1));
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

            // var message = context.Message.Value as TestMessage;
            // Console.WriteLine("Message Received: {0}", message.Text);
        }
    }
}
