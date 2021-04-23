namespace KafkaFlow
{
    using System;
    using System.Reflection;
    using KafkaFlow.Admin;
    using KafkaFlow.Admin.Handlers;
    using KafkaFlow.Configuration;
    using KafkaFlow.Serializer;
    using KafkaFlow.TypedHandler;

    /// <summary>
    /// No needed
    /// </summary>
    public static class ClusterConfigurationBuilderExtensions
    {
        /// <summary>
        /// Creates the admin producer and consumer to control the application consumers and telemetry messages
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
            cluster.DependencyConfigurator.AddSingleton<IAdminProducer, AdminProducer>();

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
    }
}
