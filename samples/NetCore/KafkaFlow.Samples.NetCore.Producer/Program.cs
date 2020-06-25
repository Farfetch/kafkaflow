namespace KafkaFlow.Samples.NetCore.Producer
{
    using System;
    using global::Microsoft.Extensions.DependencyInjection;
    using KafkaFlow.Compressor;
    using KafkaFlow.Compressor.Gzip;
    using KafkaFlow.Samples.Common;
    using KafkaFlow.Serializer;
    using KafkaFlow.Serializer.Json;
    using KafkaFlow.Serializer.ProtoBuf;

    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();

            services.AddKafka(
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

            services.AddTransient<PrintConsoleProtobufProducer>();
            services.AddTransient<PrintConsoleJsonProducer>();

            var provider = services.BuildServiceProvider();

            var bus = provider.CreateKafkaBus();

            bus.StartAsync().GetAwaiter().GetResult();

            var printConsole = provider.GetService<PrintConsoleProtobufProducer>();
            var printConsoleJson = provider.GetService<PrintConsoleJsonProducer>();

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
