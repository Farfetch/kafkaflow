namespace KafkaFlow
{
    using System;
    using System.Reflection;
    using KafkaFlow.Admin;
    using KafkaFlow.Admin.Handlers;
    using KafkaFlow.Configuration;
    using KafkaFlow.Serializer;

    /// <summary>
    /// No needed
    /// </summary>
    public static class ClusterConfigurationBuilderExtensions
    {
        /// <summary>
        /// Creates the admin producer and consumer to manage the application consumers
        /// </summary>
        /// <param name="cluster">The cluster configuration builder</param>
        /// <param name="topic">The topic to be used by the admin commands</param>
        /// <param name="consumerGroup">The consumer group prefix</param>
        /// <param name="topicPartition">The partition used to produce and consumer admin messages</param>
        /// <returns></returns>
        public static IClusterConfigurationBuilder EnableAdminMessages(
            this IClusterConfigurationBuilder cluster,
            string topic,
            string consumerGroup = null,
            int topicPartition = 0)
        {
            consumerGroup ??= $"Admin-{Assembly.GetEntryAssembly()!.GetName().Name}";

            cluster.DependencyConfigurator
                .AddSingleton<IAdminProducer>(
                    resolver => new AdminProducer(
                        resolver.Resolve<IMessageProducer<AdminProducer>>(),
                        topicPartition))
                .AddSingleton<IConsumerAdmin, ConsumerAdmin>();

            return cluster
                .AddProducer<AdminProducer>(
                    producer => producer
                        .DefaultTopic(topic)
                        .AddMiddlewares(
                            middlewares => middlewares
                                .AddSerializer<ProtobufNetSerializer>()))
                .AddConsumer(
                    consumer => consumer
                        .ManualAssignPartitions(topic, new[] { topicPartition })
                        .WithGroupId(consumerGroup)
                        .WithoutStoringOffsets()
                        .WithWorkersCount(1)
                        .WithBufferSize(1)
                        .WithAutoOffsetReset(AutoOffsetReset.Latest)
                        .DisableManagement()
                        .AddMiddlewares(
                            middlewares => middlewares
                                .AddDeserializer<ProtobufNetDeserializer>()
                                .AddTypedHandlers(
                                    handlers => handlers
                                        .WithHandlerLifetime(InstanceLifetime.Singleton)
                                        .AddHandlers(
                                            new[]
                                            {
                                                typeof(ChangeConsumerWorkersCountHandler),
                                                typeof(PauseConsumerByNameHandler),
                                                typeof(PauseConsumersByGroupHandler),
                                                typeof(ResetConsumerOffsetHandler),
                                                typeof(RestartConsumerByNameHandler),
                                                typeof(ResumeConsumerByNameHandler),
                                                typeof(ResumeConsumersByGroupHandler),
                                                typeof(RewindConsumerOffsetToDateTimeHandler),
                                                typeof(StartConsumerByNameHandler),
                                                typeof(StopConsumerByNameHandler),
                                            }))));
        }

        /// <summary>
        /// Creates the telemetry producer and consumer to send and receive metric messages
        /// </summary>
        /// <param name="cluster">The cluster configuration builder</param>
        /// <param name="topicName">The topic to be used by the metric commands</param>
        /// <param name="consumerGroup">The consumer group prefix</param>
        /// <param name="cleanRunInterval">How often run storage cleanup. Every 10 minutes by default</param>
        /// <param name="expiryTime">Cleanup will remove metrics older than specified interval. 6 hours by default</param>
        /// <param name="topicPartition">The partition used to produce and consumer telemetry data</param>
        /// <returns></returns>
        public static IClusterConfigurationBuilder EnableTelemetry(
            this IClusterConfigurationBuilder cluster,
            string topicName,
            string consumerGroup,
            TimeSpan? cleanRunInterval = null,
            TimeSpan? expiryTime = null,
            int topicPartition = 0)
        {
            cluster.DependencyConfigurator
                .AddSingleton<ITelemetryScheduler, TelemetryScheduler>()
                .AddSingleton<ITelemetryStorage>(
                    resolver =>
                        new MemoryTelemetryStorage(
                            cleanRunInterval ?? TimeSpan.FromSeconds(10),
                            expiryTime ?? TimeSpan.FromMinutes(5),
                            resolver.Resolve<IDateTimeProvider>()));

            var telemetryId = $"telemetry-{Convert.ToBase64String(Guid.NewGuid().ToByteArray())}";

            return cluster
                .AddProducer(
                    telemetryId,
                    producer => producer
                        .DefaultTopic(topicName)
                        .AddMiddlewares(
                            middlewares => middlewares
                                .AddSerializer<ProtobufNetSerializer>()))
                .AddConsumer(
                    consumer => consumer
                        .ManualAssignPartitions(topicName, new[] { topicPartition })
                        .WithName(telemetryId)
                        .WithGroupId(consumerGroup)
                        .WithoutStoringOffsets()
                        .WithWorkersCount(1)
                        .DisableManagement()
                        .WithBufferSize(10)
                        .WithAutoOffsetReset(AutoOffsetReset.Latest)
                        .AddMiddlewares(
                            middlewares => middlewares
                                .AddDeserializer<ProtobufNetDeserializer>()
                                .AddTypedHandlers(
                                    handlers => handlers
                                        .WithHandlerLifetime(InstanceLifetime.Singleton)
                                        .AddHandler<ConsumerTelemetryMetricHandler>())))
                .OnStarted(resolver => resolver.Resolve<ITelemetryScheduler>().Start(telemetryId, topicName, topicPartition))
                .OnStopping(resolver => resolver.Resolve<ITelemetryScheduler>().Stop(telemetryId));
        }

        /// <inheritdoc cref="EnableTelemetry(KafkaFlow.Configuration.IClusterConfigurationBuilder,string,string)"/>
        public static IClusterConfigurationBuilder EnableTelemetry(
            this IClusterConfigurationBuilder cluster,
            string topicName)
        {
            return cluster.EnableTelemetry(topicName, $"Telemetry-{Assembly.GetEntryAssembly().GetName().Name}");
        }
    }
}
