namespace KafkaFlow
{
    using System;
    using System.Reflection;
    using KafkaFlow.Admin;
    using KafkaFlow.Admin.Handlers;
    using KafkaFlow.Configuration;
    using KafkaFlow.Serializer;
    using KafkaFlow.TypedHandler;
    using Microsoft.Extensions.Caching.Memory;

    /// <summary>
    /// No needed
    /// </summary>
    public static class ClusterConfigurationBuilderExtensions
    {
        /// <summary>
        /// Creates the admin producer and consumer to manage the application consumers
        /// </summary>
        /// <param name="cluster">The cluster configuration builder</param>
        /// <param name="adminTopic">The topic to be used by the admin commands</param>
        /// <param name="adminConsumerGroup">The consumer group prefix</param>
        /// <returns></returns>
        public static IClusterConfigurationBuilder EnableAdminMessages(
            this IClusterConfigurationBuilder cluster,
            string adminTopic,
            string adminConsumerGroup)
        {
            cluster.DependencyConfigurator
                .AddSingleton<IAdminProducer, AdminProducer>();

            return cluster
                .AddProducer<AdminProducer>(
                    producer => producer
                        .DefaultTopic(adminTopic)
                        .AddMiddlewares(
                            middlewares => middlewares
                                .AddSerializer<ProtobufNetSerializer>()))
                .AddConsumer(
                    consumer => consumer
                        .Topic(adminTopic)
                        .WithGroupId(
                            $"{adminConsumerGroup}-{Environment.MachineName}-{Convert.ToBase64String(Guid.NewGuid().ToByteArray())}")
                        .WithWorkersCount(1)
                        .WithBufferSize(1)
                        .WithAutoOffsetReset(AutoOffsetReset.Latest)
                        .DisableManagement()
                        .AddMiddlewares(
                            middlewares => middlewares
                                .AddSerializer<ProtobufNetSerializer>()
                                .AddTypedHandlers(
                                    handlers => handlers
                                        .WithHandlerLifetime(InstanceLifetime.Singleton)
                                        .AddHandlersFromAssemblyOf<ResetConsumerOffsetHandler>())));
        }

        /// <inheritdoc cref="EnableAdminMessages(KafkaFlow.Configuration.IClusterConfigurationBuilder,string,string)"/>
        public static IClusterConfigurationBuilder EnableAdminMessages(
            this IClusterConfigurationBuilder cluster,
            string adminTopic)
        {
            return cluster.EnableAdminMessages(adminTopic, $"Admin-{Assembly.GetEntryAssembly().GetName().Name}");
        }

        /// <summary>
        /// Creates the telemetry producer and consumer to send and receive metric messages
        /// </summary>
        /// <param name="cluster">The cluster configuration builder</param>
        /// <param name="topicName">The topic to be used by the metric commands</param>
        /// <param name="consumerGroup">The consumer group prefix</param>
        /// <returns></returns>
        public static IClusterConfigurationBuilder EnableTelemetry(
            this IClusterConfigurationBuilder cluster,
            string topicName,
            string consumerGroup)
        {
            cluster.DependencyConfigurator
                .AddSingleton<IMemoryCache, MemoryCache>()
                .AddSingleton<ITelemetryStorage, MemoryCacheTelemetryStorage>()
                .AddSingleton<ITelemetryScheduler, TelemetryScheduler>();

            var groupId = $"{consumerGroup}-{Environment.MachineName}-{Convert.ToBase64String(Guid.NewGuid().ToByteArray())}";
            var name = $"telemetry-{Convert.ToBase64String(Guid.NewGuid().ToByteArray())}";

            return cluster
                .AddProducer(
                    name,
                    producer => producer
                        .DefaultTopic(topicName)
                        .AddMiddlewares(
                            middlewares => middlewares
                                .AddSerializer<ProtobufNetSerializer>()))
                .AddConsumer(
                    consumer => consumer
                        .Topic(topicName)
                        .WithName(name)
                        .WithGroupId(groupId)
                        .WithWorkersCount(1)
                        .DisableManagement()
                        .WithBufferSize(10)
                        .WithAutoOffsetReset(AutoOffsetReset.Latest)
                        .AddMiddlewares(
                            middlewares => middlewares
                                .AddSerializer<ProtobufNetSerializer>()
                                .AddTypedHandlers(
                                    handlers => handlers
                                        .WithHandlerLifetime(InstanceLifetime.Singleton)
                                        .AddHandlersFromAssemblyOf<ConsumerMetricHandler>())))
                .OnStarted(resolver => resolver.Resolve<ITelemetryScheduler>().Start(name, topicName))
                .OnStopping(resolver => resolver.Resolve<ITelemetryScheduler>().Stop(name));
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
