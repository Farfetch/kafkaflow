namespace KafkaFlow.Samples.NetFramework.Producer
{
    using System;
    using global::Unity;
    using KafkaFlow.Unity;
    using KafkaFlow.Compressor;
    using KafkaFlow.Compressor.Gzip;
    using KafkaFlow.Configuration;
    using KafkaFlow.Samples.Common;
    using KafkaFlow.Serializer;
    using KafkaFlow.Serializer.Json;
    using KafkaFlow.Serializer.ProtoBuf;

    class Program
    {
        static void Main(string[] args)
        {
            var container = new UnityContainer();

            var configurator = new KafkaFlowConfigurator(
                new UnityDependencyConfigurator(container),
                kafka => kafka
                    .UseLogHandler<ConsoleLogHandler>()
                    .AddCluster(
                        cluster => cluster
                            .WithBrokers(new[] { "localhost:9092" })
                            .AddProducer<PrintConsoleProtobufProducer>(
                                producer => producer
                                    .DefaultTopic("test-topic")
                                    .AddMiddlewares(
                                        middlewares => middlewares
                                            .AddSerializer<ProtobufMessageSerializer, SampleMessageTypeResolver>()
                                            .AddCompressor<GzipMessageCompressor>()
                                    )
                                    .WithAcks(Acks.All)
                            )
                            .AddProducer<PrintConsoleJsonProducer>(
                                producer => producer
                                    .DefaultTopic("test-topic-json")
                                    .AddMiddlewares(
                                        middlewares => middlewares
                                            .AddSerializer<JsonMessageSerializer, SampleMessageTypeResolver>()
                                            .AddCompressor<GzipMessageCompressor>()
                                    )
                                    .WithAcks(Acks.All)
                            )
                    )
            );

            container.RegisterType<PrintConsoleProtobufProducer>();
            container.RegisterType<PrintConsoleJsonProducer>();

            var bus = configurator.CreateBus(new UnityDependencyResolver(container));

            bus.StartAsync().GetAwaiter().GetResult();

            var printConsole = container.Resolve<PrintConsoleProtobufProducer>();
            var printConsoleJson = container.Resolve<PrintConsoleJsonProducer>();

            while (true)
            {
                Console.Write("Number of messages to produce: ");
                var count = int.Parse(Console.ReadLine());

                for (var i = 0; i < count; i++)
                {
                    printConsole.ProduceAsync(new TestMessage { Text = $"Protobuf Message: {Guid.NewGuid()}" });
                    printConsole.ProduceAsync(new TestMessage2 { Value = $"Protobuf Message 2: {Guid.NewGuid()}" });
                    printConsoleJson.ProduceAsync(new TestMessage { Text = $"Json Message: {Guid.NewGuid()}" });
                }
            }
        }
    }
}
