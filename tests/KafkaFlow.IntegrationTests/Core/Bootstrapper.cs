using System;
using System.IO;
using System.Threading;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using global::Microsoft.Extensions.Configuration;
using global::Microsoft.Extensions.DependencyInjection;
using global::Microsoft.Extensions.Hosting;
using KafkaFlow.Compressor.Gzip;
using KafkaFlow.IntegrationTests.Core.Handlers;
using KafkaFlow.IntegrationTests.Core.Messages;
using KafkaFlow.IntegrationTests.Core.Middlewares;
using KafkaFlow.IntegrationTests.Core.Producers;
using KafkaFlow.Serializer;
using KafkaFlow.Serializer.SchemaRegistry;

namespace KafkaFlow.IntegrationTests.Core;

internal static class Bootstrapper
{
    public const string PauseResumeTopicName = "test-pause-resume";
    public const int MaxPollIntervalMs = 7000;

    internal const string ProtobufGroupId = "consumer-protobuf";
    internal const string GzipGroupId = "consumer-gzip";
    internal const string JsonGzipGroupId = "consumer-json-gzip";
    internal const string ProtobufGzipGroupId = "consumer-protobuf-gzip";
    internal const string PauseResumeGroupId = "consumer-pause-resume";
    internal const string AvroGroupId = "consumer-avro";
    internal const string JsonGroupId = "consumer-json";
    internal const string NullGroupId = "consumer-null";
    internal const string OffsetTrackerGroupId = "consumer-offset-tracker";


    private const string ProtobufTopicName = "test-protobuf";
    private const string ProtobufSchemaRegistryTopicName = "test-protobuf-sr";
    private const string JsonSchemaRegistryTopicName = "test-json-sr";
    private const string JsonTopicName = "test-json";
    private const string GzipTopicName = "test-gzip";
    private const string JsonGzipTopicName = "test-json-gzip";
    private const string ProtobufGzipTopicName = "test-protobuf-gzip";
    private const string ProtobufGzipTopicName2 = "test-protobuf-gzip-2";
    private const string AvroTopicName = "test-avro";
    private const string NullTopicName = "test-null";
    private const string DefaultParamsTopicName = "test-default-params";
    internal const string OffsetTrackerTopicName = "test-offset-tracker";

    private static readonly Lazy<IServiceProvider> s_lazyProvider = new(SetupProvider);

    public static IServiceProvider GetServiceProvider() => s_lazyProvider.Value;

    private static IServiceProvider SetupProvider()
    {
        var builder = Host
            .CreateDefaultBuilder()
            .ConfigureAppConfiguration(
                (_, config) =>
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
                (_, options) =>
                {
                    options.ValidateScopes = true;
                    options.ValidateOnBuild = true;
                });

        var host = builder.Build();
        var bus = host.Services.CreateKafkaBus();
        bus.StartAsync().GetAwaiter().GetResult();

        // Wait partition assignment
        Thread.Sleep(10000);

        return host.Services;
    }

    private static void SetupServices(HostBuilderContext context, IServiceCollection services)
    {
        var kafkaBrokers = context.Configuration.GetValue<string>("Kafka:Brokers");
        var schemaRegistryUrl = context.Configuration.GetValue<string>("SchemaRegistry:Url");

        ConsumerConfig defaultConfig = new()
        {
            Acks = Confluent.Kafka.Acks.All,
            AllowAutoCreateTopics = false,
            AutoCommitIntervalMs = 5000,
            AutoOffsetReset = Confluent.Kafka.AutoOffsetReset.Latest,
            LogConnectionClose = false,
            ReconnectBackoffMs = 1000,
            ReconnectBackoffMaxMs = 6000
        };

        services.AddKafka(
            kafka =>
            {
                kafka.UseLogHandler<TraceLogHandler>()
                .AddCluster(
                    cluster => cluster
                        .WithBrokers(kafkaBrokers.Split(';'))
                        .WithSchemaRegistry(config => config.Url = schemaRegistryUrl)
                        .CreateTopicIfNotExists(AvroTopicName, 1, 1)
                        .CreateTopicIfNotExists(ProtobufSchemaRegistryTopicName, 2, 1)
                        .CreateTopicIfNotExists(JsonSchemaRegistryTopicName, 2, 1)
                        .AddProducer<AvroProducer>(
                            producer => producer
                                .DefaultTopic(AvroTopicName)
                                .AddMiddlewares(
                                    middlewares => middlewares
                                        .AddSerializer(
                                            resolver => new ConfluentAvroSerializer(
                                                resolver,
                                                new AvroSerializerConfig
                                                {
                                                    AutoRegisterSchemas = true,
                                                    SubjectNameStrategy = SubjectNameStrategy.Record,
                                                }))))
                        .AddConsumer(
                            consumer => consumer
                                .Topic(AvroTopicName)
                                .WithGroupId(AvroGroupId)
                                .WithBufferSize(100)
                                .WithWorkersCount(10)
                                .WithAutoOffsetReset(AutoOffsetReset.Latest)
                                .WithConsumerConfig(defaultConfig)
                                .AddMiddlewares(
                                    middlewares => middlewares
                                        .AddDeserializer<ConfluentAvroDeserializer>()
                                        .AddTypedHandlers(
                                            handlers => handlers
                                                .WithHandlerLifetime(InstanceLifetime.Singleton)
                                                .AddHandler<AvroMessageHandler>())))
                        .AddProducer<ConfluentProtobufProducer>(
                            producer => producer
                                .DefaultTopic(ProtobufSchemaRegistryTopicName)
                                .AddMiddlewares(
                                    middlewares => middlewares
                                        .AddSerializer(
                                            resolver => new ConfluentProtobufSerializer(
                                                resolver,
                                                new ProtobufSerializerConfig
                                                {
                                                    AutoRegisterSchemas = true,
                                                    SubjectNameStrategy = SubjectNameStrategy.Record,
                                                }))))
                        .AddConsumer(
                            consumer => consumer
                                .Topic(ProtobufSchemaRegistryTopicName)
                                .WithGroupId(ProtobufGroupId)
                                .WithBufferSize(100)
                                .WithWorkersCount(10)
                                .WithAutoOffsetReset(AutoOffsetReset.Latest)
                                .WithConsumerConfig(defaultConfig)
                                .AddMiddlewares(
                                    middlewares => middlewares
                                        .AddDeserializer<ConfluentProtobufDeserializer>()
                                        .AddTypedHandlers(
                                            handlers => handlers
                                                .WithHandlerLifetime(InstanceLifetime.Singleton)
                                                .AddHandler<ConfluentProtobufMessageHandler>())))
                        .AddProducer<ConfluentJsonProducer>(
                            producer => producer
                                .DefaultTopic(JsonSchemaRegistryTopicName)
                                .AddMiddlewares(
                                    middlewares => middlewares
                                        .AddSerializer(
                                            resolver => new ConfluentJsonSerializer(
                                                resolver,
                                                new JsonSerializerConfig
                                                {
                                                    AutoRegisterSchemas = true,
                                                    SubjectNameStrategy = SubjectNameStrategy.Record,
                                                }))))
                        .AddConsumer(
                            consumer => consumer
                                .Topic(JsonSchemaRegistryTopicName)
                                .WithGroupId(JsonGroupId)
                                .WithBufferSize(100)
                                .WithWorkersCount(10)
                                .WithAutoOffsetReset(AutoOffsetReset.Latest)
                                .WithConsumerConfig(defaultConfig)
                                .AddMiddlewares(
                                    middlewares => middlewares
                                        .AddDeserializer<ConfluentJsonDeserializer>()
                                        .AddTypedHandlers(
                                            handlers => handlers
                                                .WithHandlerLifetime(InstanceLifetime.Singleton)
                                                .AddHandler<ConfluentJsonMessageHandler>()))))
                .AddCluster(
                    cluster => cluster
                        .WithBrokers(kafkaBrokers.Split(';'))
                        .CreateTopicIfNotExists(ProtobufTopicName, 2, 1)
                        .CreateTopicIfNotExists(PauseResumeTopicName, 2, 1)
                        .CreateTopicIfNotExists(JsonTopicName, 1, 1)
                        .CreateTopicIfNotExists(GzipTopicName, 2, 1)
                        .CreateTopicIfNotExists(JsonGzipTopicName, 2, 1)
                        .CreateTopicIfNotExists(ProtobufGzipTopicName, 2, 1)
                        .CreateTopicIfNotExists(ProtobufGzipTopicName2, 2, 1)
                        .CreateTopicIfNotExists(NullTopicName, 1, 1)
                        .CreateTopicIfNotExists(OffsetTrackerTopicName, 1, 1)
                        .CreateTopicIfNotExists(DefaultParamsTopicName)
                        .AddConsumer(
                            consumer => consumer
                                .Topic(ProtobufTopicName)
                                .WithGroupId(ProtobufGroupId)
                                .WithBufferSize(100)
                                .WithWorkersCount(10)
                                .WithAutoOffsetReset(AutoOffsetReset.Latest)
                                .AddMiddlewares(
                                    middlewares => middlewares
                                        .AddSingleTypeDeserializer<ProtobufNetDeserializer>(typeof(TestMessage1))
                                        .AddTypedHandlers(
                                            handlers =>
                                                handlers
                                                    .WithHandlerLifetime(InstanceLifetime.Singleton)
                                                    .AddHandler<MessageHandler>())))
                        .AddConsumer(
                            consumer => consumer
                                .Topic(PauseResumeTopicName)
                                .WithGroupId(PauseResumeGroupId)
                                .WithBufferSize(3)
                                .WithWorkersCount(3)
                                .WithAutoOffsetReset(AutoOffsetReset.Latest)
                                .WithConsumerConfig(
                                    new ConsumerConfig
                                    {
                                        MaxPollIntervalMs = MaxPollIntervalMs,
                                        SessionTimeoutMs = MaxPollIntervalMs,
                                    })
                                .AddMiddlewares(
                                    middlewares => middlewares
                                        .AddSingleTypeDeserializer<PauseResumeMessage, ProtobufNetDeserializer>()
                                        .AddTypedHandlers(
                                            handlers =>
                                                handlers
                                                    .WithHandlerLifetime(InstanceLifetime.Singleton)
                                                    .AddHandler<PauseResumeHandler>())))
                        .AddConsumer(
                            consumer => consumer
                                .Topic(JsonTopicName)
                                .WithGroupId(JsonGzipGroupId)
                                .WithBufferSize(100)
                                .WithWorkersCount(10)
                                .WithAutoOffsetReset(AutoOffsetReset.Latest)
                                .AddMiddlewares(
                                    middlewares => middlewares
                                        .AddDeserializer<JsonCoreDeserializer>()
                                        .AddTypedHandlers(
                                            handlers =>
                                                handlers
                                                    .WithHandlerLifetime(InstanceLifetime.Singleton)
                                                    .AddHandlersFromAssemblyOf<MessageHandler>())))
                        .AddConsumer(
                            consumer => consumer
                                .Topic(NullTopicName)
                                .WithGroupId(NullGroupId)
                                .WithBufferSize(100)
                                .WithWorkersCount(10)
                                .WithAutoOffsetReset(AutoOffsetReset.Latest)
                                .AddMiddlewares(
                                    middlewares => middlewares
                                        .AddTypedHandlers(
                                            handlers =>
                                                handlers
                                                    .WithHandlerLifetime(InstanceLifetime.Singleton)
                                                    .AddHandler<NullMessageHandler>()
                                        )))
                        .AddConsumer(
                            consumer => consumer
                                .Topic(OffsetTrackerTopicName)
                                .WithGroupId(OffsetTrackerGroupId)
                                .WithBufferSize(100)
                                .WithWorkersCount(10)
                                .WithAutoOffsetReset(AutoOffsetReset.Latest)
                                .AddMiddlewares(
                                    middlewares => middlewares
                                        .AddDeserializer<JsonCoreDeserializer>()
                                        .AddTypedHandlers(
                                            handlers =>
                                                handlers
                                                    .WithHandlerLifetime(InstanceLifetime.Singleton)
                                                    .AddHandler<OffsetTrackerMessageHandler>()
                                        )))
                        .AddConsumer(
                            consumer => consumer
                                .Topics(GzipTopicName)
                                .WithGroupId(GzipGroupId)
                                .WithBufferSize(100)
                                .WithWorkersCount(10)
                                .WithAutoOffsetReset(AutoOffsetReset.Latest)
                                .AddMiddlewares(
                                    middlewares => middlewares
                                        .AddDecompressor<GzipMessageDecompressor>()
                                        .Add<GzipMiddleware>()))
                        .AddConsumer(
                            consumer => consumer
                                .Topics(JsonGzipTopicName)
                                .WithGroupId(JsonGzipGroupId)
                                .WithBufferSize(100)
                                .WithWorkersCount(10)
                                .WithAutoOffsetReset(AutoOffsetReset.Latest)
                                .AddMiddlewares(
                                    middlewares => middlewares
                                        .AddDeserializer(_ => new JsonCoreDeserializer())
                                        .AddTypedHandlers(
                                            handlers =>
                                                handlers
                                                    .WithHandlerLifetime(InstanceLifetime.Singleton)
                                                    .AddHandler<MessageHandler>())))
                        .AddConsumer(
                            consumer => consumer
                                .Topics(ProtobufGzipTopicName, ProtobufGzipTopicName2)
                                .WithGroupId(ProtobufGzipGroupId)
                                .WithBufferSize(100)
                                .WithWorkersCount(10)
                                .WithAutoOffsetReset(AutoOffsetReset.Latest)
                                .WithAutoCommitIntervalMs(1)
                                .AddMiddlewares(
                                    middlewares => middlewares
                                        .AddDecompressor<GzipMessageDecompressor>()
                                        .AddDeserializer<ProtobufNetDeserializer>()
                                        .AddTypedHandlers(
                                            handlers =>
                                                handlers
                                                    .WithHandlerLifetime(InstanceLifetime.Singleton)
                                                    .AddHandler<MessageHandler>())))
                        .AddProducer<JsonProducer>(
                            producer => producer
                                .DefaultTopic(JsonTopicName)
                                .AddMiddlewares(
                                    middlewares => middlewares
                                        .AddSerializer<JsonCoreSerializer>()))
                        .AddProducer<NullProducer>(
                            producer => producer
                                .DefaultTopic(NullTopicName)
                                .AddMiddlewares(
                                    middlewares => middlewares
                                        .AddSerializer<JsonCoreSerializer>()))
                        .AddProducer<OffsetTrackerProducer>(
                            producer => producer
                                .DefaultTopic(OffsetTrackerTopicName)
                                .AddMiddlewares(
                                    middlewares => middlewares
                                        .AddSerializer<JsonCoreSerializer>()))
                        .AddProducer<JsonGzipProducer>(
                            producer => producer
                                .DefaultTopic(JsonGzipTopicName)
                                .WithCompression(CompressionType.Gzip)
                                .AddMiddlewares(
                                    middlewares => middlewares
                                        .AddSerializer<JsonCoreSerializer>()))
                        .AddProducer<ProtobufProducer>(
                            producer => producer
                                .DefaultTopic(ProtobufTopicName)
                                .AddMiddlewares(
                                    middlewares => middlewares
                                        .AddSingleTypeSerializer<TestMessage1, ProtobufNetSerializer>()))
                        .AddProducer<ProtobufGzipProducer>(
                            producer => producer
                                .DefaultTopic(ProtobufGzipTopicName)
                                .AddMiddlewares(
                                    middlewares => middlewares
                                        .AddSerializer<ProtobufNetSerializer>()
                                        .AddCompressor<GzipMessageCompressor>()))
                        .AddProducer<ProtobufGzipProducer2>(
                            producer => producer
                                .DefaultTopic(ProtobufGzipTopicName2)
                                .AddMiddlewares(
                                    middlewares => middlewares
                                        .AddSerializer(_ => new ProtobufNetSerializer())
                                        .AddCompressor(_ => new GzipMessageCompressor())))
                        .AddProducer<GzipProducer>(
                            producer => producer
                                .DefaultTopic(GzipTopicName)
                                .AddMiddlewares(
                                    middlewares => middlewares
                                        .AddCompressor<GzipMessageCompressor>())));
            });

        services.AddSingleton<JsonProducer>();
        services.AddSingleton<JsonGzipProducer>();
        services.AddSingleton<ProtobufProducer>();
        services.AddSingleton<ProtobufGzipProducer2>();
        services.AddSingleton<ProtobufGzipProducer>();
        services.AddSingleton<GzipProducer>();
    }
}
