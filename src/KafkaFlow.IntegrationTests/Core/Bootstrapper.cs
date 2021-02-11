namespace KafkaFlow.IntegrationTests.Core
{
    using System;
    using System.IO;
    using System.Threading;
    using Confluent.SchemaRegistry;
    using Confluent.SchemaRegistry.Serdes;
    using global::Microsoft.Extensions.Configuration;
    using global::Microsoft.Extensions.DependencyInjection;
    using global::Microsoft.Extensions.Hosting;
    using KafkaFlow.Compressor;
    using KafkaFlow.Compressor.Gzip;
    using KafkaFlow.IntegrationTests.Core.Handlers;
    using KafkaFlow.IntegrationTests.Core.Messages;
    using KafkaFlow.IntegrationTests.Core.Middlewares;
    using KafkaFlow.IntegrationTests.Core.Middlewares.Producers;
    using KafkaFlow.Serializer;
    using KafkaFlow.Serializer.Json;
    using KafkaFlow.Serializer.ProtoBuf;
    using KafkaFlow.TypedHandler;
    using Serializer.ApacheAvro;

    public static class Bootstrapper
    {
        private const string ProtobufTopicName = "test-protobuf";
        private const string JsonTopicName = "test-json";
        private const string GzipTopicName = "test-gzip";
        private const string JsonGzipTopicName = "test-json-gzip";
        private const string ProtobufGzipTopicName = "test-protobuf-gzip";
        private const string ProtobufGzipTopicName2 = "test-protobuf-gzip-2";
        private const string AvroTopicName = "test-avro";

        private static readonly Lazy<IServiceProvider> lazyProvider = new Lazy<IServiceProvider>(SetupProvider);

        public static IServiceProvider GetServiceProvider() => lazyProvider.Value;

        private static IServiceProvider SetupProvider()
        {
            var builder = Host
                .CreateDefaultBuilder()
                .ConfigureAppConfiguration(
                    (builderContext, config) =>
                    {
                        config
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile(
                                "conf/appsettings.json",
                                false,
                                true)
                            .AddEnvironmentVariables();
                    })
                .ConfigureServices(SetupServices)
                .UseDefaultServiceProvider(
                    (context, options) =>
                    {
                        options.ValidateScopes = true;
                        options.ValidateOnBuild = true;
                    });

            var host = builder.Build();
            var bus = host.Services.CreateKafkaBus();
            bus.StartAsync().GetAwaiter().GetResult();
            //Wait partition assignment
            Thread.Sleep(10000);

            return host.Services;
        }

        private static void SetupServices(HostBuilderContext context, IServiceCollection services)
        {
            var kafkaBrokers = context.Configuration.GetValue<string>("Kafka:Brokers");
            var schemaRegistryUrl = context.Configuration.GetValue<string>("SchemaRegistry:Url");

            services.AddKafka(
                kafka => kafka
                    .UseLogHandler<TraceLogHandler>()
                    .AddCluster(
                        cluster => cluster
                            .WithBrokers(kafkaBrokers.Split(';'))
                            .WithSchemaRegistry(config => config.Url = schemaRegistryUrl)
                            .AddProducer<AvroProducer>(
                                producer => producer
                                    .DefaultTopic(AvroTopicName)
                                    .AddMiddlewares(
                                        middlewares => middlewares
                                            .AddSerializer(resolver => new ApacheAvroMessageSerializer(
                                                resolver, 
                                                new AvroSerializerConfig
                                                {
                                                    AutoRegisterSchemas = true,
                                                    SubjectNameStrategy = SubjectNameStrategy.Record
                                                }))
                                    )
                            )
                            .AddConsumer(
                                consumer => consumer
                                    .Topic(AvroTopicName)
                                    .WithGroupId("consumer-avro")
                                    .WithBufferSize(100)
                                    .WithWorkersCount(10)
                                    .WithAutoOffsetReset(AutoOffsetReset.Latest)
                                    .AddMiddlewares(
                                        middlewares => middlewares
                                            .AddSerializer<ApacheAvroMessageSerializer>()
                                            .AddTypedHandlers(
                                                handlers => handlers
                                                    .WithHandlerLifetime(InstanceLifetime.Singleton)
                                                    .AddHandler<AvroMessageHandler>())
                                    )
                            )
                    )
                    .AddCluster(
                        cluster => cluster
                            .WithBrokers(kafkaBrokers.Split(';'))
                            .AddConsumer(
                                consumer => consumer
                                    .Topic(ProtobufTopicName)
                                    .WithGroupId("consumer-protobuf")
                                    .WithBufferSize(100)
                                    .WithWorkersCount(10)
                                    .WithAutoOffsetReset(AutoOffsetReset.Latest)
                                    .AddMiddlewares(
                                        middlewares => middlewares
                                            .AddSingleTypeSerializer<TestMessage1, ProtobufMessageSerializer>()
                                            .AddTypedHandlers(
                                                handlers =>
                                                    handlers
                                                        .WithHandlerLifetime(InstanceLifetime.Singleton)
                                                        .AddHandler<MessageHandler>())
                                    )
                            )
                            .AddConsumer(
                                consumer => consumer
                                    .Topic(JsonTopicName)
                                    .WithGroupId("consumer-json")
                                    .WithBufferSize(100)
                                    .WithWorkersCount(10)
                                    .WithAutoOffsetReset(AutoOffsetReset.Latest)
                                    .AddMiddlewares(
                                        middlewares => middlewares
                                            .AddSerializer<JsonMessageSerializer>()
                                            .AddTypedHandlers(
                                                handlers =>
                                                    handlers
                                                        .WithHandlerLifetime(InstanceLifetime.Singleton)
                                                        .AddHandlersFromAssemblyOf<MessageHandler>())
                                    )
                            )
                            .AddConsumer(
                                consumer => consumer
                                    .Topics(GzipTopicName)
                                    .WithGroupId("consumer-gzip")
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
                                    .WithGroupId("consumer-json-gzip")
                                    .WithBufferSize(100)
                                    .WithWorkersCount(10)
                                    .WithAutoOffsetReset(AutoOffsetReset.Latest)
                                    .AddMiddlewares(
                                        middlewares => middlewares
                                            .AddCompressor(r => new GzipMessageCompressor())
                                            .AddSerializer(r => new JsonMessageSerializer())
                                            .AddTypedHandlers(
                                                handlers =>
                                                    handlers
                                                        .WithHandlerLifetime(InstanceLifetime.Singleton)
                                                        .AddHandler<MessageHandler>())
                                    )
                            )
                            .AddConsumer(
                                consumer => consumer
                                    .Topics(ProtobufGzipTopicName, ProtobufGzipTopicName2)
                                    .WithGroupId("consumer-protobuf-gzip")
                                    .WithBufferSize(100)
                                    .WithWorkersCount(10)
                                    .WithAutoOffsetReset(AutoOffsetReset.Latest)
                                    .WithAutoCommitIntervalMs(1)
                                    .AddMiddlewares(
                                        middlewares => middlewares
                                            .AddCompressor<GzipMessageCompressor>()
                                            .AddSerializer<ProtobufMessageSerializer>()
                                            .AddTypedHandlers(
                                                handlers =>
                                                    handlers
                                                        .WithHandlerLifetime(InstanceLifetime.Singleton)
                                                        .AddHandler<MessageHandler>())
                                    )
                            )
                            .AddProducer<JsonProducer>(
                                producer => producer
                                    .DefaultTopic(JsonTopicName)
                                    .AddMiddlewares(
                                        middlewares => middlewares
                                            .AddSerializer<JsonMessageSerializer>()
                                    )
                            )
                            .AddProducer<JsonGzipProducer>(
                                producer => producer
                                    .DefaultTopic(JsonGzipTopicName)
                                    .AddMiddlewares(
                                        middlewares => middlewares
                                            .AddSerializer<JsonMessageSerializer>()
                                            .AddCompressor<GzipMessageCompressor>()
                                    )
                            )
                            .AddProducer<ProtobufProducer>(
                                producer => producer
                                    .DefaultTopic(ProtobufTopicName)
                                    .AddMiddlewares(
                                        middlewares => middlewares
                                            .AddSingleTypeSerializer<TestMessage1, ProtobufMessageSerializer>()
                                    )
                            )
                            .AddProducer<ProtobufGzipProducer>(
                                producer => producer
                                    .DefaultTopic(ProtobufGzipTopicName)
                                    .AddMiddlewares(
                                        middlewares => middlewares
                                            .AddSerializer<ProtobufMessageSerializer>()
                                            .AddCompressor<GzipMessageCompressor>()
                                    )
                            )
                            .AddProducer<ProtobufGzipProducer2>(
                                producer => producer
                                    .DefaultTopic(ProtobufGzipTopicName2)
                                    .AddMiddlewares(
                                        middlewares => middlewares
                                            .AddSerializer(r => new ProtobufMessageSerializer())
                                            .AddCompressor(r => new GzipMessageCompressor())
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

            services.AddSingleton<JsonProducer>();
            services.AddSingleton<JsonGzipProducer>();
            services.AddSingleton<ProtobufProducer>();
            services.AddSingleton<ProtobufGzipProducer2>();
            services.AddSingleton<ProtobufGzipProducer>();
            services.AddSingleton<GzipProducer>();
        }
    }
}
