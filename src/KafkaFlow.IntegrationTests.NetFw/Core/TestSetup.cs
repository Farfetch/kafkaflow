namespace KafkaFlow.IntegrationTests.NetFw.Core
{
    using System.Configuration;
    using System.Threading;
    using Compressor;
    using Compressor.Gzip;
    using Configuration;
    using global::Unity;
    using Handlers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Middlewares;
    using Producers;
    using Serializer;
    using Serializer.Json;
    using Serializer.ProtoBuf;
    using TypedHandler;
    using TypeResolvers;
    using Unity;

    [TestClass]
    public static class TestSetup
    {
        private const string ProtobufTopicName = "topic-protobuf-netfw";
        private const string JsonTopicName = "topic-json-netfw";
        private const string GzipTopicName = "topic-gzip-netfw";
        private const string JsonGzipTopicName = "topic-json-gzip-netfw";
        private const string ProtobufGzipTopicName = "topic-protobuf-gzip-netfw";
        private const string ProtobufGzipTopicName2 = "topic-protobuf-gzip-2-netfw";

        private const string ProtobufConsumerId = "consumer-protobuf-netfw";
        private const string JsonConsumerId = "consumer-protobuf-netfw";
        private const string GzipConsumerId = "consumer-gzip-netfw";
        private const string JsonGzipConsumerId = "consumer-json-gzip-netfw";
        private const string ProtobufGzipConsumerId = "consumer-protobuf-gzip-netfw";

        private static UnityContainer container;
        private static IKafkaBus kafkaBus;

        public static UnityContainer GetContainer() => container;
            
        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            container = new UnityContainer();

            var configurator = new KafkaFlowConfigurator(
                new UnityDependencyConfigurator(container),
                kafka => kafka
                    .UseLogHandler<TraceLoghandler>()
                    .AddCluster(
                        cluster => cluster
                            .WithBrokers(new[] { ConfigurationManager.AppSettings["Kafka:Brokers"]})
                            .AddConsumer(
                                consumer => consumer
                                    .Topic(ProtobufTopicName)
                                    .WithGroupId(ProtobufConsumerId)
                                    .WithBufferSize(100)
                                    .WithWorkersCount(10)
                                    .WithAutoOffsetReset(AutoOffsetReset.Latest)
                                    .AddMiddlewares(
                                        middlewares => middlewares
                                            .AddSerializer<ProtobufMessageSerializer, SampleMessageTypeResolver>()
                                            .AddTypedHandlers(
                                                handlers =>
                                                    handlers
                                                        .WithHandlerLifetime(KafkaFlow.InstanceLifetime.Singleton)
                                                        .AddHandler<MessageHandler>())
                                    )
                            )
                            .AddConsumer(
                                consumer => consumer
                                    .Topic(JsonTopicName)
                                    .WithGroupId(JsonConsumerId)
                                    .WithBufferSize(100)
                                    .WithWorkersCount(10)
                                    .WithAutoOffsetReset(AutoOffsetReset.Latest)
                                    .AddMiddlewares(
                                        middlewares => middlewares
                                            .AddSerializer<JsonMessageSerializer, SampleMessageTypeResolver>()
                                            .AddTypedHandlers(
                                                handlers =>
                                                    handlers
                                                        .WithHandlerLifetime(KafkaFlow.InstanceLifetime.Singleton)
                                                        .AddHandlersFromAssemblyOf<MessageHandler>())
                                    )
                            )
                            .AddConsumer(
                                consumer => consumer
                                    .Topics(GzipTopicName)
                                    .WithGroupId(GzipConsumerId)
                                    .WithBufferSize(100)
                                    .WithWorkersCount(10)
                                    .WithAutoOffsetReset(AutoOffsetReset.Latest)
                                    .AddMiddlewares(
                                        middlewares => middlewares
                                            .AddCompressor<GzipMessageCompressor>()
                                            .Add<GzipMiddleware>()
                                    )
                            )
                            .AddConsumer(
                                consumer => consumer
                                    .Topics(JsonGzipTopicName)
                                    .WithGroupId(JsonGzipConsumerId)
                                    .WithBufferSize(100)
                                    .WithWorkersCount(10)
                                    .WithAutoOffsetReset(AutoOffsetReset.Latest)
                                    .AddMiddlewares(
                                        middlewares => middlewares
                                            .AddCompressor<GzipMessageCompressor>()
                                            .AddSerializer<JsonMessageSerializer, SampleMessageTypeResolver>()
                                            .AddTypedHandlers(
                                                handlers =>
                                                    handlers
                                                        .WithHandlerLifetime(KafkaFlow.InstanceLifetime.Singleton)
                                                        .AddHandler<MessageHandler>())
                                    )
                            )
                            .AddConsumer(
                                consumer => consumer
                                    .Topics(ProtobufGzipTopicName, ProtobufGzipTopicName2)
                                    .WithGroupId(ProtobufGzipConsumerId)
                                    .WithBufferSize(100)
                                    .WithWorkersCount(10)
                                    .WithAutoOffsetReset(AutoOffsetReset.Latest)
                                    .WithAutoCommitIntervalMs(1)
                                    .AddMiddlewares(
                                        middlewares => middlewares
                                            .AddCompressor<GzipMessageCompressor>()
                                            .AddSerializer<ProtobufMessageSerializer, SampleMessageTypeResolver>()
                                            .AddTypedHandlers(
                                                handlers =>
                                                    handlers
                                                        .WithHandlerLifetime(KafkaFlow.InstanceLifetime.Singleton)
                                                        .AddHandler<MessageHandler>())
                                    )
                            )
                            .AddProducer<JsonProducer>(
                                producer => producer
                                    .DefaultTopic(JsonTopicName)
                                    .AddMiddlewares(
                                        middlewares => middlewares
                                            .AddSerializer<JsonMessageSerializer, SampleMessageTypeResolver>()
                                    )
                            )
                            .AddProducer<JsonGzipProducer>(
                                producer => producer
                                    .DefaultTopic(JsonGzipTopicName)
                                    .AddMiddlewares(
                                        middlewares => middlewares
                                            .AddSerializer<JsonMessageSerializer, SampleMessageTypeResolver>()
                                            .AddCompressor<GzipMessageCompressor>()
                                    )
                            )
                            .AddProducer<ProtobufProducer>(
                                producer => producer
                                    .DefaultTopic(ProtobufTopicName)
                                    .AddMiddlewares(
                                        middlewares => middlewares
                                            .AddSerializer<ProtobufMessageSerializer, SampleMessageTypeResolver>()
                                    )
                            )
                            .AddProducer<ProtobufGzipProducer>(
                                producer => producer
                                    .DefaultTopic(ProtobufGzipTopicName)
                                    .AddMiddlewares(
                                        middlewares => middlewares
                                            .AddSerializer<ProtobufMessageSerializer, SampleMessageTypeResolver>()
                                            .AddCompressor<GzipMessageCompressor>()
                                    )
                            )
                            .AddProducer<ProtobufGzipProducer2>(
                                producer => producer
                                    .DefaultTopic(ProtobufGzipTopicName2)
                                    .AddMiddlewares(
                                        middlewares => middlewares
                                            .AddSerializer<ProtobufMessageSerializer, SampleMessageTypeResolver>()
                                            .AddCompressor<GzipMessageCompressor>()
                                    )
                            )
                            .AddProducer<GzipProducer>(
                                producer => producer
                                    .DefaultTopic(GzipTopicName)
                                    .AddMiddlewares(
                                        middlewares => middlewares
                                            .AddCompressor<GzipMessageCompressor>()
                                    )
                            )
                    )
            );

            container.RegisterType<JsonProducer>();
            container.RegisterType<JsonGzipProducer>();
            container.RegisterType<ProtobufProducer>();
            container.RegisterType<ProtobufGzipProducer2>();
            container.RegisterType<ProtobufGzipProducer>();
            container.RegisterType<GzipProducer>();
            
            kafkaBus = configurator.CreateBus(new UnityDependencyResolver(container));
            kafkaBus.StartAsync().GetAwaiter().GetResult();
            
            //Wait partition assignment
            Thread.Sleep(10000);
        }
        
        [AssemblyCleanup]
        public static void AssemblyCleanUp()
        {
            kafkaBus.StopAsync().GetAwaiter().GetResult();
        }
    }
}