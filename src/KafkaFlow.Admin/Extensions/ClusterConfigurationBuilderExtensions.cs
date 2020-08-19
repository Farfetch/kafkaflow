namespace KafkaFlow
{
    using System;
    using KafkaFlow.Admin;
    using KafkaFlow.Admin.Handlers;
    using KafkaFlow.Configuration;
    using KafkaFlow.Serializer;
    using KafkaFlow.Serializer.ProtoBuf;
    using KafkaFlow.TypedHandler;

    public static class ClusterConfigurationBuilderExtensions
    {
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
                                .AddSerializer<ProtobufMessageSerializer>()
                        ))
                .AddConsumer(
                    consumer => consumer
                        .Topic(adminTopic)
                        .WithGroupId(adminConsumerGroup)
                        .WithWorkersCount(1)
                        .WithBufferSize(1)
                        .WithAutoOffsetReset(AutoOffsetReset.Latest)
                        .AddMiddlewares(
                            middlewares => middlewares
                                .AddSerializer<ProtobufMessageSerializer>()
                                .AddTypedHandlers(
                                    handlers => handlers
                                        .WithHandlerLifetime(InstanceLifetime.Singleton)
                                        .AddHandlersFromAssemblyOf<ResetConsumerOffsetHandler>()))
                );
        }

        public static IClusterConfigurationBuilder EnableAdminMessages(
            this IClusterConfigurationBuilder cluster,
            string adminTopic)
        {
            return cluster.EnableAdminMessages(adminTopic, $"Admin.{Environment.MachineName}");
        }
    }
}
