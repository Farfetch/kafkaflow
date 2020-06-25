namespace KafkaFlow.Samples.NetFramework.Consumer
{
    using System;
    using global::Unity;
    using KafkaFlow.Compressor;
    using KafkaFlow.Compressor.Gzip;
    using KafkaFlow.Configuration;
    using KafkaFlow.Consumers;
    using KafkaFlow.Samples.Common;
    using KafkaFlow.Serializer;
    using KafkaFlow.Serializer.Json;
    using KafkaFlow.Serializer.ProtoBuf;
    using KafkaFlow.TypedHandler;
    using KafkaFlow.Unity;

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
                            .AddConsumer(
                                consumer => consumer
                                    .Topic("test-topic")
                                    .WithGroupId("print-console-handler")
                                    .WithBufferSize(100)
                                    .WithWorkersCount(10)
                                    .WithAutoOffsetReset(AutoOffsetReset.Latest)
                                    .AddMiddlewares(
                                        middlewares => middlewares
                                            .AddCompressor<GzipMessageCompressor>()
                                            .AddSerializer<ProtobufMessageSerializer, SampleMessageTypeResolver>()
                                            .AddTypedHandlers(
                                                handlers => handlers
                                                    .WithHandlerLifetime(global::KafkaFlow.InstanceLifetime.Singleton)
                                                    .AddHandler<PrintConsoleHandler>())
                                    )
                            )
                            .AddConsumer(
                                consumer => consumer
                                    .Topic("test-topic-json")
                                    .WithGroupId("print-console-handler")
                                    .WithBufferSize(100)
                                    .WithWorkersCount(10)
                                    .WithAutoOffsetReset(AutoOffsetReset.Latest)
                                    .AddMiddlewares(
                                        middlewares => middlewares
                                            .AddCompressor<GzipMessageCompressor>()
                                            .AddSerializer<JsonMessageSerializer, SampleMessageTypeResolver>()
                                            .AddTypedHandlers(
                                                handlers => handlers
                                                    .WithHandlerLifetime(global::KafkaFlow.InstanceLifetime.Singleton)
                                                    .AddHandler<PrintConsoleHandler>())
                                    )
                            )
                    )
            );

            var bus = configurator.CreateBus(new UnityDependencyResolver(container));

            bus.StartAsync().GetAwaiter().GetResult();

            var consumerAcessor = container.Resolve<IConsumerAccessor>();

            while (true)
            {
                var input = Console.ReadLine();

                switch (input)
                {
                    case "pause":

                        foreach (var consumer in consumerAcessor.All)
                        {
                            consumer.Pause(consumer.Assignment);
                        }

                        break;

                    case "resume":

                        foreach (var consumer in consumerAcessor.All)
                        {
                            consumer.Resume(consumer.Assignment);
                        }

                        break;
                    case "exit":
                        return;
                }
            }
        }
    }
}
